using UnityEngine;
using VarVarGamejam.SO;

namespace VarVarGamejam.Tablet
{
    public class TabletManager : MonoBehaviour
    {
        public static TabletManager Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        [SerializeField]
        private GameObject _tabletObj;

        [SerializeField]
        private TabletInfo _info;

        public void Toggle()
        {
            _tabletObj.SetActive(!_tabletObj.activeInHierarchy);
        }
    }
}
