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

            List<Vector2Int> pending = new();
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
                pending.Add(new Vector2Int(mid + dir.y * 2, mid + dir.x * 2));
            }
            _map[mid][mid] = TileType.Empty;

            // Prim algorithm implementation
            while (pending.Any())
            {
                var rand = pending[Random.Range(0, pending.Count)];
                _map[rand.y][rand.x] = TileType.Empty; // Current room

                List<Vector2Int> availableDirs = new(allDirs);
                // Remove positions that doesn't lead to an empty space
                // TODO: keep track of previous pos to avoid loops in maze
                availableDirs.RemoveAll(x => _map[rand.x + x.y * 2][rand.y + x.x * 2] == TileType.Empty);
                var dir = availableDirs[Random.Range(0, availableDirs.Count)];
                var pos = rand + dir * 2;
                _map[rand.y + dir.y][rand.x + dir.x] = TileType.Empty; // Corridor
                foreach (var d in allDirs)
                {
                    var newY = pos.y + d.y * 2;
                    var newX = pos.x + d.x * 2;

                    // Check if position is valid
                    if (newY <= 0 || newX <= 0 || newY >= _info.MapSize - 1 || newX >= _info.MapSize - 1)
                    {
                        continue;
                    }

                    var newPos = new Vector2Int(newY, newX);
                    if (_map[newPos.y][newPos.x] == TileType.Pending)
                    {
                        pending.Add(newPos);
                    }
                }
            }
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