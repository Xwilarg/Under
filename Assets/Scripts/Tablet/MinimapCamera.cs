using UnityEngine;
using VarVarGamejam.Map;

namespace VarVarGamejam.Tablet
{
    public class MinimapCamera : MonoBehaviour
    {
        [SerializeField]
        private Light _globalLight;

        public Light PlayerLight { set; private get; }

        private bool _isMainLightEnabled;

        private void OnPreCull()
        {
            PlayerLight.enabled = false;

            _isMainLightEnabled = _globalLight.enabled;
            _globalLight.enabled = true;
            MapGeneration.Instance.DisplayWallsTexts(false);
        }

        private void OnPreRender()
        {
            PlayerLight.enabled = false;
            _globalLight.enabled = true;
            MapGeneration.Instance.DisplayWallsTexts(false);
        }

        private void OnPostRender()
        {
            PlayerLight.enabled = true;
            _globalLight.enabled = _isMainLightEnabled;
            MapGeneration.Instance.DisplayWallsTextsDefault();
        }
    }
}
