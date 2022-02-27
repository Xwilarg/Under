using UnityEngine;
using VarVarGamejam.Map;
using VarVarGamejam.Player;
using VarVarGamejam.SO;

namespace VarVarGamejam.Menu
{
    public class GoalManager : MonoBehaviour
    {
        public static GoalManager Instance { get; private set; }

        [SerializeField]
        private GameObject _takeHelp, _nextTakeHelp, _mapHelp;

        [SerializeField]
        private RectTransform _timerBar;

        [SerializeField]
        private GameInfo _info;

        [SerializeField]
        private GameObject _audioGroup;

        [SerializeField]
        private GameOver _gameOver;

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
                    var p = PlayerController.Instance.transform.position;
                    StartCoroutine(MapGeneration.Instance.KillPlayer(new Vector2Int(Mathf.RoundToInt(p.x), Mathf.RoundToInt(p.z))));
                    PlayerController.Instance.Loose();
                    _gameOver.gameObject.SetActive(true);
                }
            }
        }

        public void ToggleTakeHelp(bool value)
        {
            _takeHelp.SetActive(value);
        }

        public void ToggleNextTakeHelp(bool value)
        {
            if (value)
            {
                ToggleTakeHelp(false);
                _nextTakeHelp.SetActive(true);
            }
            else
            {
                ToggleTakeHelp(false);
                _nextTakeHelp.SetActive(false);
            }
        }

        public void TakeObjective()
        {
            ObjectiveObj.transform.parent.GetComponent<AudioSource>().Play();
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
