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
            }

            // Place first point
            var mid = (_info.MapSize - 1) / 2;
            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 3; x++)
                {
                    _map[mid + y - 1][mid + x - 1] = TileType.PendingWall;
                }
            }
            _map[mid][mid] = TileType.Wall;

            // Prim algorithm implementation
            //while (_map.Any(y => y.Any(x => x == TileType.PendingWall)))
            {

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