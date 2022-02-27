using UnityEngine;
using UnityEngine.SceneManagement;
using VarVarGamejam.Player;
using VarVarGamejam.SO;

namespace VarVarGamejam.Map
{
    public class Victory : MonoBehaviour
    {
        [SerializeField]
        private GameInfo _info;

        private bool _victory;

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

            var nextAlpha = VictoryObjs.Instance.Fade.color.a + Time.deltaTime * _info.GameoverFadeTime;
            if (nextAlpha >= 1f)
            {
                SceneManager.LoadScene("Menu");
            }
            else
            {
                VictoryObjs.Instance.Fade.color = new Color(VictoryObjs.Instance.Fade.color.r, VictoryObjs.Instance.Fade.color.g, VictoryObjs.Instance.Fade.color.b, nextAlpha);
                foreach (var text in VictoryObjs.Instance.Texts)
                {
                    text.color = new Color(text.color.r, text.color.g, text.color.b, nextAlpha);
                }
            }
        }
    }
}
