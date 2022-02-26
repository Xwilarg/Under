using UnityEngine;

namespace VarVarGamejam.Tablet
{
    public class MinimapCamera : MonoBehaviour
    {
        [SerializeField]
        private Light _globalLight;

        public static MinimapCamera Instance { get; private set; }
        private void Awake()
        {
            Instance = this;
        }

        public Light PlayerLight { set; private get; }

        private bool _isMainLightEnabled;

        private void OnPreCull()
        {
            PlayerLight.enabled = false;

            _isMainLightEnabled = _globalLight.enabled;
            _globalLight.enabled = true;
        }

        private void OnPreRender()
        {
            PlayerLight.enabled = false;
            _globalLight.enabled = true;
        }

        private void OnPostRender()
        {
            PlayerLight.enabled = true;
            _globalLight.enabled = _isMainLightEnabled;
        }
    }
}
