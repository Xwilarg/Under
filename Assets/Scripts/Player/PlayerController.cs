using UnityEngine;
using UnityEngine.InputSystem;
using VarVarGamejam.SO;

namespace VarVarGamejam.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private PlayerInfo _info;
        [SerializeField]
        private Transform _head;

        private float _headRotation;
        private Vector2 _groundMovement = Vector2.zero;
		private CharacterController _controller;
        private bool _isSprinting;
        private float _verticalSpeed;

        private void Start()
        {
            _controller = GetComponent<CharacterController>();
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void FixedUpdate()
		{
			Vector3 desiredMove = transform.forward * _groundMovement.y + transform.right * _groundMovement.x;

			// Get a normal for the surface that is being touched to move along it
			Physics.SphereCast(transform.position, _controller.radius, Vector3.down, out RaycastHit hitInfo,
							   _controller.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
			desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

			Vector3 moveDir = Vector3.zero;
			moveDir.x = desiredMove.x * _info.ForceMultiplier * (_isSprinting ? _info.SpeedRunningMultiplicator : 1f);
			moveDir.z = desiredMove.z * _info.ForceMultiplier * (_isSprinting ? _info.SpeedRunningMultiplicator : 1f);

			if (_controller.isGrounded && _verticalSpeed < 0f) // We are on the ground and not jumping
			{
				moveDir.y = -.1f; // Stick to the ground
			}
			else
			{
				// We are currently jumping, reduce our jump velocity by gravity and apply it
				_verticalSpeed += Physics.gravity.y;
				moveDir.y += _verticalSpeed;
			}
			_controller.Move(moveDir);
		}

		public void OnMovement(InputAction.CallbackContext value)
        {
            _groundMovement = value.ReadValue<Vector2>().normalized;
        }

        public void OnLook(InputAction.CallbackContext value)
        {
            var rot = value.ReadValue<Vector2>();

            transform.rotation *= Quaternion.AngleAxis(rot.x * _info.HorizontalLookMultiplier, Vector3.up);

            _headRotation -= rot.y * _info.VerticalLookMultiplier; // Vertical look is inverted by default, hence the -=

            _headRotation = Mathf.Clamp(_headRotation, -89, 89);
            _head.transform.localRotation = Quaternion.AngleAxis(_headRotation, Vector3.right);
        }

        public void OnJump(InputAction.CallbackContext value)
        {
            if (_controller.isGrounded)
            {
                _verticalSpeed = _info.JumpForce;
            }
        }

        public void OnSprint(InputAction.CallbackContext value)
        {
            _isSprinting = value.ReadValueAsButton();
        }
    }
}
