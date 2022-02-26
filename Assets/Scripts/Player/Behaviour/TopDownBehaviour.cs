using UnityEngine;

namespace VarVarGamejam.Player.Behaviour
{
    public class TopDownBehaviour : IPlayerBehaviour
    {
        private Transform _body;
        private Light _mainLight;
        public Vector2 Movement { private set; get; }
        public GameObject TargetCamera { private set; get; }

        public TopDownBehaviour(Transform body, GameObject camera)
        {
            _body = body;
            TargetCamera = camera;
            _mainLight = GameObject.FindGameObjectWithTag("MainLight").GetComponent<Light>();
            _mainLight.enabled = false;
        }

        public void Enable()
        {
            _mainLight.enabled = true;
        }

        public void Disable()
        {
            _mainLight.enabled = false;
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