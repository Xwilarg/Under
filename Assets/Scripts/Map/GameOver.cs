using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VarVarGamejam.SO;

namespace VarVarGamejam.Map
{
    public class GameOver : MonoBehaviour
    {
        [SerializeField]
        private GameInfo _info;

        private Image _fade;

        private void Start()
        {
            _fade = GetComponent<Image>();
        }

        private void Update()
        {
            var nextAlpha = _fade.color.a + Time.deltaTime * _info.GameoverFadeTime;
            if (nextAlpha >= 1f)
            {
                SceneManager.LoadScene("Menu");
            }
            else
            {
                _fade.color = new Color(_fade.color.r, _fade.color.g, _fade.color.b, nextAlpha);
            }
        }
    }
}
