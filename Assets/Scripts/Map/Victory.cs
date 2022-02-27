using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VarVarGamejam.Player;
using VarVarGamejam.SO;

namespace VarVarGamejam.Map
{
    public class Victory : MonoBehaviour
    {
        [SerializeField]
        private GameInfo _info;

        [SerializeField]
        private TMP_Text[] _text;

        private bool _victory;

        private Image _fade;

        private void Start()
        {
            _fade = GetComponent<Image>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                var player = other.GetComponent<PlayerController>();
                if (player.HaveGoalInHands)
                {
                    player.Loose();
                    _victory = true;
                }
            }
        }

        private void Update()
        {
            if (!_victory)
            {
                return;
            }

            var nextAlpha = _fade.color.a + Time.deltaTime * _info.GameoverFadeTime;
            if (nextAlpha >= 1f)
            {
                SceneManager.LoadScene("Menu");
            }
            else
            {
                _fade.color = new Color(_fade.color.r, _fade.color.g, _fade.color.b, nextAlpha);
                foreach (var text in _text)
                {
                    text.color = new Color(text.color.r, text.color.g, text.color.b, nextAlpha);
                }
            }
        }
    }
}
