using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using VarVarGamejam.SO;
using VarVarGamejam.Tablet;

namespace VarVarGamejam.Map
{
    public class MapGeneration : MonoBehaviour
    {
        public static MapGeneration Instance { get; private set; }

        [SerializeField]
        private MapInfo _info;

        [SerializeField]
        private GameObject _playerPrefab;

        [SerializeField]
        private AudioClip[] _wallsAudio;

        [SerializeField]
        private Material _floorMatFirst, _floorMatSecond;

        [SerializeField]
        private GameObject _entranceHelp;

        private TileType[][] _map;
        private readonly List<GameObject> _walls = new();

        private Vector2Int? _cache;
        private bool _canGoBackward = true;

        private GameObject _wallParent;

        private readonly List<ObjPos> _playerTrap = new();
        private readonly Timer _trapTimer = new();

        public float Middle { get; private set; }

        private GameObject _floor;

        private readonly List<Vector2Int> _allDirs = new()
        {
            Vector2Int.left,
            Vector2Int.right,
            Vector2Int.up,
            Vector2Int.down
        };
        private GameObject _goal;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            Assert.AreEqual(1, _info.MapSize % 2, "Map size must be an odd number");
            Assert.IsTrue(_info.MapSize >= 5, "Map size must be bigger than 4");

            _wallParent = new GameObject("Map");
            Generate(safePos: null, firstTime: true);
        }

        public bool IsGoingBackward(Vector2Int pos)
        {
            if (_canGoBackward || // Is backward prevention mechanism enabled yet
                (_cache != null && _cache == pos) || // Is the player still on the same tile as before
                (_map[pos.y][pos.x] == TileType.Exit || _map[pos.y][pos.x] == TileType.Entrance)) // Let's not stuck the player right on a goal tile!
            {
                return false;
            }
            _cache = pos;
            if (_map[pos.y][pos.x] == TileType.EmptyTaken)
            {
                return true;
            }
            _map[pos.y][pos.x] = TileType.EmptyTaken;
            return false;
        }

        public void EnableBackwardPrevention()
        {
            _canGoBackward = false;
        }

        public void StartTPSView()
        {
            _floor.GetComponent<MeshRenderer>().material = _floorMatSecond;
        }

        private IEnumerator EnclosePlayer(List<AudioSource> sources, Vector2Int pos)
        {
            var randAudio = _wallsAudio[Random.Range(0, _wallsAudio.Length)];

            // Add walls to prevent user to leave
            foreach (var dir in _allDirs)
            {
                if (_map[pos.y + dir.y][pos.x + dir.x] == TileType.Empty || _map[pos.y + dir.y][pos.x + dir.x] == TileType.EmptyTaken)
                {
                    var w = Instantiate(_info.WallPrefab, new Vector3(pos.x + dir.x, .5f, pos.y + dir.y), Quaternion.identity);
                    var s = w.GetComponent<AudioSource>();
                    s.clip = randAudio;
                    s.Play();
                    sources.Add(s);
                    _playerTrap.Add(new(pos + dir, w));
                }
            }

            _trapTimer.Start(_info.TimerWall, goUp: true);
            yield return new WaitForSeconds(_info.TimerWall + _info.TimerWallRest);
        }

        public IEnumerator KillPlayer(Vector2Int pos) // Game over
        {
            List<AudioSource> sources = new();
            yield return EnclosePlayer(sources, pos);
        }

        public IEnumerator Regenerate(Vector2Int pos)
        {
            List<AudioSource> sources = new();
            yield return EnclosePlayer(sources, pos);

            _cache = null;

            foreach (var s in sources)
            {
                s.Play();
            }
            foreach (var wall in _walls)
            {
                Destroy(wall);
            }
            _walls.Clear();

            Generate(safePos: pos, firstTime: false);

            _trapTimer.Start(_info.TimerWall, goUp: false);
            yield return new WaitForSeconds(_info.TimerWall);

            // Remove "player cage"
            foreach (var wall in _playerTrap)
            {
                Destroy(wall.Obj);
            }
            _playerTrap.Clear();
        }

        private void Update()
        {
            _trapTimer.Update(Time.deltaTime);
            foreach (var wall in _playerTrap)
            {
                wall.Obj.transform.position = new Vector3(wall.Obj.transform.position.x, -.5f + _trapTimer.Lerp(1f), wall.Obj.transform.position.z);
            }
        }

        private void Generate(Vector2Int? safePos, bool firstTime)
        {
            // Init map
            _map = new TileType[_info.MapSize][];
            for (int i = 0; i < _info.MapSize; i++)
            {
                _map[i] = new TileType[_info.MapSize];
            }

            // Draw external walls
            for (int i = 0; i < _info.MapSize; i++)
            {
                _map[0][i] = TileType.Wall;
                _map[_info.MapSize - 1][i] = TileType.Wall;
                _map[i][0] = TileType.Wall;
                _map[i][_info.MapSize - 1] = TileType.Wall;

                if (i != 0 && i != _info.MapSize - 1 && i % 2 == 0)
                {
                    for (int j = 2; j < _info.MapSize - 1; j += 2)
                    {
                        _map[i][j] = TileType.Wall;
                    }
                }
            }

            List<PosDir> pending = new();

            // Place first point
            var mid = (_info.MapSize - 1) / 2 + 1;
            foreach (var dir in _allDirs)
            {
                pending.Add(new()
                {
                    Pos = new Vector2Int(mid + dir.x * 2, mid + dir.y * 2),
                    Dir = -dir
                });
            }
            _map[mid][mid] = TileType.Empty;

            // Prim algorithm implementation
            while (pending.Any())
            {
                // Get random tile and remove it from the list
                var pIndex = Random.Range(0, pending.Count);
                var rand = pending[pIndex];
                pending.RemoveAt(pIndex);

                _map[rand.Pos.y][rand.Pos.x] = TileType.Empty; // Current room

                _map[rand.Pos.y + rand.Dir.y][rand.Pos.x + rand.Dir.x] = TileType.Empty; // Corridor
                foreach (var d in _allDirs)
                {
                    var newY = rand.Pos.y + d.y * 2;
                    var newX = rand.Pos.x + d.x * 2;

                    // Check if position is valid
                    if (IsOutOfBounds(newY, newX, _info.MapSize))
                    {
                        continue;
                    }

                    if (_map[newY][newX] == TileType.Pending)
                    {
                        var pPos = new Vector2Int(newX, newY);
                        var curr = pending.FirstOrDefault(x => x.Pos == pPos);
                        if (curr == null)
                        {
                            pending.Add(new()
                            {
                                Pos = new(newX, newY),
                                Dir = -d
                            });
                        }
                        else
                        {
                            curr.Dir = -d;
                        }
                    }
                }
            }

            // Add entrance and exit
            var s = (_info.MapSize - 3) / 2;
            var posEntrance = (Random.Range(0, s) * 2) + 1;
            var posExit = (Random.Range(0, s) * 2) + 1;

            (Vector2Int Pos, Vector2Int Dir) entrance, exit;

            if (safePos == null)
            {
                entrance = (new(posEntrance, 0), Vector2Int.down);
                exit = (new(posExit, _info.MapSize - 1), Vector2Int.up);
            }
            else
            {
                (Vector2Int Pos, Vector2Int Dir) longestPos = new (Vector2Int Pos, Vector2Int Dir)[]
                {
                    (new(0, posEntrance), Vector2Int.left),
                    (new(_info.MapSize - 1, posEntrance), Vector2Int.right),
                    (new(posEntrance, 0), Vector2Int.down),
                    (new(posEntrance, _info.MapSize - 1), Vector2Int.up)
                }.OrderByDescending(x => Vector2.Distance(safePos.Value, x.Pos)).ElementAt(0);
                entrance = longestPos;
                exit = (new((_info.MapSize - 1) - longestPos.Pos.x, (_info.MapSize - 1) - longestPos.Pos.y), -longestPos.Dir);
            }
            _map[entrance.Pos.y][entrance.Pos.x] = TileType.Entrance;
            _map[exit.Pos.y][exit.Pos.x] = TileType.Exit;

            // Once we are done, we replace unused "corridors" by walls
            for (int y = 0; y < _info.MapSize; y++)
            {
                for (int x = 0; x < _info.MapSize; x++)
                {
                    var isWall = _map[y][x] switch
                    {
                        TileType.Wall => true,
                        TileType.Pending => true,
                        _ => false
                    };
                    if (isWall)
                    {
                        if (safePos != null && safePos.Value.x == x && safePos.Value.y == y)
                        {
                            // When regenerating the maze, we make sure the player doesn't stay stuck in a wall
                            _map[y][x] = TileType.Empty;
                        }
                        else
                        {
                            var matchingTrap = _playerTrap.FirstOrDefault(t => t.Pos.x == x && t.Pos.y == y);
                            if (matchingTrap != null) // There is already a wall here so we just replace it
                            {
                                Destroy(matchingTrap.Obj);
                                _playerTrap.Remove(matchingTrap);
                            }
                            var go = Instantiate(_info.WallPrefab, new Vector3(x, .5f, y), Quaternion.identity);
                            go.transform.parent = _wallParent.transform;
                            _map[y][x] = TileType.Wall;
                            _walls.Add(go);
                        }
                    }
                }
            }

            // The following actions shouldn't be done twice
            if (firstTime)
            {
                Middle = Mathf.Floor(_info.MapSize / 2f);

                // Spawn floor
                var floorPos = Mathf.FloorToInt(_info.MapSize / 2f);
                _floor = Instantiate(_info.FloorPrefab, new Vector3(floorPos, 0f, floorPos), Quaternion.identity);
                _floor.transform.localScale = new Vector3(_info.MapSize / 10f, 1f, _info.MapSize / 10f);
                _floor.GetComponent<MeshRenderer>().material = _floorMatFirst;

                // Set minimap position
                TabletManager.Instance.SetMinimapCamera(Middle, Middle, _info.MapSize / 2f);

                // Spawn player and goal
                Instantiate(_playerPrefab, new Vector3(posEntrance, .5f, 0f), Quaternion.identity);
                _goal = Instantiate(_info.GoalPrefab);
            }

            _goal.transform.position = new Vector3(exit.Pos.x, _info.GoalPrefab.transform.localScale.y / 2f, exit.Pos.y);
            _walls.Add(Instantiate(_entranceHelp, new Vector3(entrance.Pos.x, .5f, entrance.Pos.y), Quaternion.identity));
            _walls.Add(Instantiate(_info.WallPrefab, new Vector3(entrance.Pos.x + entrance.Dir.x, .5f, entrance.Pos.y + entrance.Dir.y), Quaternion.identity));
            _walls.Add(Instantiate(_info.WallPrefab, new Vector3(exit.Pos.x + (firstTime ? exit.Dir.x : 0f), .5f, exit.Pos.y + (firstTime ? exit.Dir.y : 0f)), Quaternion.identity));
        }

        private bool IsOutOfBounds(int y, int x, int size)
        {
            return y <= 0 || x <= 0 || y >= size - 1 || x >= size - 1;
        }

        private record PosDir
        {
            public Vector2Int Pos;
            public Vector2Int Dir;
        }

        private void OnDrawGizmos()
        {
            if (_map == null)
            {
                return;
            }
            Gizmos.color = new(1f, 0f, 0f, .2f);
            for (int y = 0; y < _info.MapSize; y++)
            {
                for (int x = 0; x < _info.MapSize; x++)
                {
                    if (_map[y][x] == TileType.EmptyTaken)
                    {
                        Gizmos.DrawCube(new Vector3(x, 0f, y), Vector3.one);
                    }
                }
            }
        }
    }

}