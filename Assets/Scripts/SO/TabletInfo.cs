using UnityEngine;

namespace VarVarGamejam.SO
{
    [CreateAssetMenu(menuName = "ScriptableObject/TabletInfo", fileName = "TabletInfo")]
    public class TabletInfo : ScriptableObject
    {
        public Sprite[] BatteryImages;

        public float BatteryDuration;
    }
}
