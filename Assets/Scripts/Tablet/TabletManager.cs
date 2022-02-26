using UnityEngine;
using UnityEngine.UI;
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

        [SerializeField]
        private Image _batteryImage;

        private float _remainingBattery;

        private void Start()
        {
            _remainingBattery = _info.BatteryDuration;
        }

        public void Toggle()
        {
            if (_remainingBattery > 0f)
            {
                _tabletObj.SetActive(!_tabletObj.activeInHierarchy);
            }
        }

        private void Update()
        {
            if (_tabletObj.activeInHierarchy)
            {
                _remainingBattery -= Time.deltaTime;
                if (_remainingBattery <= 0f) // We are out of battery
                {
                    _tabletObj.SetActive(false);
                }
                else
                {
                    _batteryImage.sprite = _info.BatteryImages[(_info.BatteryImages.Length - 1) - Mathf.RoundToInt(_remainingBattery * (_info.BatteryImages.Length - 1) / _info.BatteryDuration)];
                }
            }
        }
    }
}
