using UnityEngine;
using VarVarGamejam.SO;

namespace VarVarGamejam.Menu
{
    public class GoalManager : MonoBehaviour
    {
        public static GoalManager Instance { get; private set; }

        [SerializeField]
        private GameObject _takeHelp, _mapHelp;

        [SerializeField]
        private RectTransform _timerBar;

        [SerializeField]
        private GameInfo _info;

        [SerializeField]
        private GameObject _audioGroup;

        private float _gameTimer = -1f;

        public GameObject ObjectiveObj { private get; set; }

        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            if (_gameTimer > 0f)
            {
                _gameTimer -= Time.deltaTime;
                _timerBar.anchorMax = new Vector2(_gameTimer / _info.GameTimer, _timerBar.anchorMax.y);
                if (_gameTimer <= 0f)
                {
                    Debug.Break(); // Game lost!
                }
            }
        }

        public void ToggleTakeHelp(bool value)
        {
            _takeHelp.SetActive(value);
        }

        public void TakeObjective()
        {
            ObjectiveObj.SetActive(false);
            _gameTimer = _info.GameTimer;
            _timerBar.parent.gameObject.SetActive(true);
        }

        public void EnableMapHelp()
        {
            _mapHelp.SetActive(true);
        }

        public void EnableBGM()
        {
            _audioGroup.SetActive(true);
        }
    }
}
