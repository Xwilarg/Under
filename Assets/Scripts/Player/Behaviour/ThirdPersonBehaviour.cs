using UnityEngine;
using VarVarGamejam.SO;

namespace VarVarGamejam.Player.Behaviour
{
    public class ThirdPersonBehaviour : IPlayerBehaviour
    {
        private Transform _me, _head;
        private float _headRotation;
        private PlayerInfo _info;

        public Vector2 Movement { private set; get; }

        public GameObject TargetCamera { private set; get; }

        public ThirdPersonBehaviour(Transform me, Transform head, PlayerInfo info, GameObject camera)
        {
            _me = me;
            _head = head;
            _info = info;
            TargetCamera = camera;
        }

        public void Enable()
        { }

        public void Disable()
        { }

        public void OnKeyboardInput(Vector2 input)
        {
            Movement = input;
        }

        public void OnMouseMove(Vector2 mousePos)
        {
            _me.rotation *= Quaternion.AngleAxis(mousePos.x * _info.HorizontalLookMultiplier, Vector3.up);

            _headRotation -= mousePos.y * _info.VerticalLookMultiplier; // Vertical look is inverted by default, hence the -=

            _headRotation = Mathf.Clamp(_headRotation, -89, 89);
            _head.transform.localRotation = Quaternion.AngleAxis(_headRotation, Vector3.right);
        }
    }
}
