using UnityEngine;

namespace VarVarGamejam.Player.Behaviour
{
    public class TopDownBehaviour : IPlayerBehaviour
    {
        private Transform _body;
        private GameObject _mainLight;
        public Vector2 Movement { private set; get; }
        public GameObject TargetCamera { private set; get; }

        public TopDownBehaviour(Transform body, GameObject camera)
        {
            _body = body;
            TargetCamera = camera;
            _mainLight = GameObject.FindGameObjectWithTag("MainLight");
            _mainLight.SetActive(false);
        }

        public void Enable()
        {
            _mainLight.SetActive(true);
        }

        public void Disable()
        {
            _mainLight.SetActive(false);
            _body.rotation = Quaternion.identity;
        }

        public void OnKeyboardInput(Vector2 input)
        {
            Movement = input;
            if (input.magnitude > 0f)
            {
                _body.rotation = Quaternion.LookRotation(new Vector3(input.x, 0f, input.y));
            }
        }

        public void OnMouseMove(Vector2 mousePos)
        { }
    }
}