using UnityEngine;

namespace VarVarGamejam.SO
{
    [CreateAssetMenu(menuName = "ScriptableObject/PlayerInfo", fileName = "PlayerInfo")]
    public class PlayerInfo : ScriptableObject
    {
        [Range(0, 10f)]
        public float HorizontalLookMultiplier = 1f;
        [Range(0, 10f)]
        public float VerticalLookMultiplier = 1f;
        [Range(0f, 1000000f)]
        public float ForceMultiplier = 1f;
        public float SpeedRunningMultiplicator;
        public float JumpForce;
    }
}
