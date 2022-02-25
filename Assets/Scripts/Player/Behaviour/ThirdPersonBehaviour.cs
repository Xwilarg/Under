using UnityEngine;
using VarVarGamejam.SO;

namespace VarVarGamejam.Player.Behaviour
{
    public class ThirdPersonBehaviour : IPlayerBehaviour
    {
        private Transform _me, _head;
        private float _headRotation;
        private PlayerInfo _info;
        private Vector2 _groundMovement = Vector2.zero;

        public ThirdPersonBehaviour(Transform me, Transform head, PlayerInfo info)
        {
            _me = me;
            _head = head;
            _info = info;
        }

        public Vector2 Movement => _groundMovement;

        public void OnKeyboardInput(Vector2 input)
        {
            _groundMovement = input;
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
