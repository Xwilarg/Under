using UnityEngine;

namespace VarVarGamejam.Player
{
    public class MinimapCamera : MonoBehaviour
    {
        public static MinimapCamera Instance { get; private set; }
        private void Awake()
        {
            Instance = this;
        }

        public Light PlayerLight { set; private get; }

        private void OnPreCull()
        {
            PlayerLight.enabled = false;
        }

        private void OnPreRender()
        {
            PlayerLight.enabled = false;
        }

        private void OnPostRender()
        {
            PlayerLight.enabled = true;
        }
    }
}
