using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VarVarGamejam.Map
{
    public class VictoryObjs : MonoBehaviour
    {
        public static VictoryObjs Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        public Image Fade;
        public TMP_Text[] Texts;
    }
}
