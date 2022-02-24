using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using VarVarGamejam.SO;

namespace VarVarGamejam.Map
{
    public class MapGeneration : MonoBehaviour
    {
        [SerializeField]
        private MapInfo _info;

        private TileType[][] _map;

        private void Start()
        {
            Assert.AreEqual(1, _info.MapSize % 2, "Map size must be an odd number");
            Assert.IsTrue(_info.MapSize >= 5, "Map size must be bigger than 4");

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
            List<Vector2Int> allDirs = new()
            {
                Vector2Int.left,
                Vector2Int.right,
                Vector2Int.up,
                Vector2Int.down
            };

            // Place first point
            var mid = (_info.MapSize - 1) / 2 + 1;
            foreach (var dir in allDirs)
            {
                pending.Add(new()
                {
                    Pos = new Vector2Int(mid + dir.x * 2, mid + dir.y * 2),
                    Dir = -dir
                });
            }
            _map[mid][mid] = TileType.Entrance;

            // Prim algorithm implementation
            while (pending.Any())
            {
                // Get random tile and remove it from the list
                var pIndex = Random.Range(0, pending.Count);
                var rand = pending[pIndex];
                pending.RemoveAt(pIndex);

                _map[rand.Pos.y][rand.Pos.x] = TileType.Empty; // Current room

                _map[rand.Pos.y + rand.Dir.y][rand.Pos.x + rand.Dir.x] = TileType.Empty; // Corridor
                foreach (var d in allDirs)
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

            // Once we are done, we replace unused "corridors" by walls
            for (int y = 0; y < _info.MapSize; y++)
            {
                for (int x = 0; x < _info.MapSize; x++)
                {
                    if (_map[y][x] == TileType.Pending)
                    {
                        _map[y][x] = TileType.Wall;
                    }
                }
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

        private enum Direction
        {
            Left, Right, Up, Down
        }

        private void OnDrawGizmos()
        {
            if (_map == null)
            {
                return;
            }
            for (int y = 0; y < _info.MapSize; y++)
            {
                for (int x = 0; x < _info.MapSize; x++)
                {
                    Gizmos.color = _map[y][x] switch
                    {
                        TileType.Entrance => Color.green,
                        TileType.Empty => Color.white,
                        TileType.Wall => Color.black,
                        _ => Color.red
                    };
                    Gizmos.DrawCube(new Vector3(x, 0f, y), Vector3.one);
                }
            }
        }
    }

}