﻿using UnityEngine;

namespace VarVarGamejam.SO
{
    [CreateAssetMenu(menuName = "ScriptableObject/MapInfo", fileName = "MapInfo")]
    public class MapInfo : ScriptableObject
    {
        [Tooltip("Full size of the map")]
        public int MapSize;
    }
}
