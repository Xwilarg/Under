using UnityEngine;

namespace VarVarGamejam.Player.Behaviour
{
    public class TopDownBehaviour : IPlayerBehaviour
    {
        private Vector2 _mov;

        public Vector2 Movement => _mov;

        public void OnKeyboardInput(Vector2 input)
        {
            _mov = input;
        }

        public void OnMouseMove(Vector2 mousePos)
        { }
    }
}