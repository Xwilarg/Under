using UnityEngine;

namespace VarVarGamejam.SO
{
    [CreateAssetMenu(menuName = "ScriptableObject/MapInfo", fileName = "MapInfo")]
    public class MapInfo : ScriptableObject
    {
        [Tooltip("Full size of the map")]
        public int MapSize;

        [Tooltip("Prefab used for walls and floors")]
        public GameObject WallPrefab, FloorPrefab, GoalPrefab;
    }
}
