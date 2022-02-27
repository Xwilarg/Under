using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.Menu
{
    public class Transition : MonoBehaviour
    {
        [SerializeField]
        private Image _fade;

        private bool _canFade;

        public void OnNext(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                _canFade = true;
            }
        }

        private void Update()
        {
            if (_canFade)
            {
                var alpha = _fade.color.a + Time.deltaTime * 3f;
                if (alpha >= 1f)
                {
                    SceneManager.LoadScene("Main");
                }
                else
                {
                    _fade.color = new Color(_fade.color.r, _fade.color.g, _fade.color.b, alpha);
                }
            }
        }
    }
}
