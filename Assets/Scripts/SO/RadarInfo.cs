using UnityEngine;

namespace VarVarGamejam.SO
{
    [CreateAssetMenu(menuName = "ScriptableObject/RadarInfo", fileName = "RadarInfo")]
    public class RadarInfo : ScriptableObject
    {
        public float LineRotSpeed;
        public float FadeSpeed;
    }
}
