using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using VarVarGamejam.SO;

namespace VarVarGamejam.Map
{
    public class MapGeneration : MonoBehaviour
    {
        public static MapGeneration Instance { get; private set; }

        [SerializeField]
        private MapInfo _info;

        [SerializeField]
        private GameObject _playerPrefab;

        private TileType[][] _map;
        private readonly List<GameObject> _walls = new();

        private Vector2Int? _cache;
        private bool _canGoBackward;

        private GameObject _wallParent;

        private readonly List<Vector2Int> _allDirs = new()
        {
            Vector2Int.left,
            Vector2Int.right,
            Vector2Int.up,
            Vector2Int.down
        };

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            Assert.AreEqual(1, _info.MapSize % 2, "Map size must be an odd number");
            Assert.IsTrue(_info.MapSize >= 5, "Map size must be bigger than 4");

            _wallParent = new GameObject("Map");
            Generate(firstTime: true);
        }

        public bool IsGoingBackward(Vector2Int pos)
        {
            if (_canGoBackward || (_cache != null && _cache == pos))
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

        public IEnumerator Regenerate(Vector2Int pos) // TODO: Prevent wall to spawn in player
        {
            // Add walls to prevent user to leave
            List<GameObject> playerSecurity = new();
            foreach (var dir in _allDirs)
            {
                if (_map[pos.y + dir.y][pos.x + dir.x] == TileType.Empty || _map[pos.y + dir.y][pos.x + dir.x] == TileType.EmptyTaken)
                {
                    playerSecurity.Add(Instantiate(_info.WallPrefab, new Vector3(pos.x + dir.x, .5f, pos.y + dir.y), Quaternion.identity));
                }
            }

            yield return new WaitForSeconds(1f);

            _cache = null;
            _canGoBackward = true;

            foreach (var wall in _walls)
            {
                Destroy(wall);
            }
            _walls.Clear();

            Generate(firstTime: false);

            yield return new WaitForSeconds(1f);

            // Remove "player cage"
            foreach (var wall in playerSecurity)
            {
                Destroy(wall);
            }
        }

        private void Generate(bool firstTime)
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
            _map[0][posEntrance] = TileType.Entrance;
            _map[_info.MapSize - 1][posExit] = TileType.Exit;

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
                        var go = Instantiate(_info.WallPrefab, new Vector3(x, .5f, y), Quaternion.identity);
                        go.transform.parent = _wallParent.transform;
                        _map[y][x] = TileType.Wall;
                        _walls.Add(go);
                    }
                }
            }

            // The following actions shouldn't be done twice
            if (firstTime)
            {
                // Spawn floor
                var floorPos = Mathf.FloorToInt(_info.MapSize / 2f);
                var floor = Instantiate(_info.FloorPrefab, new Vector3(floorPos, 0f, floorPos), Quaternion.identity);
                floor.transform.localScale = new Vector3(_info.MapSize / 10f, 1f, _info.MapSize / 10f);

                // Spawn player and goal
                Instantiate(_playerPrefab, new Vector3(posEntrance, .5f, 0f), Quaternion.identity);
                Instantiate(_info.GoalPrefab, new Vector3(posExit, _info.GoalPrefab.transform.localScale.y / 2f, _info.MapSize - 1), Quaternion.identity);
                _walls.Add(Instantiate(_info.WallPrefab, new Vector3(posEntrance, .5f, -1f), Quaternion.identity));
                _walls.Add(Instantiate(_info.WallPrefab, new Vector3(posExit, .5f, _info.MapSize), Quaternion.identity));
            }
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