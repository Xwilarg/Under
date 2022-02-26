using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using VarVarGamejam.Effect;
using VarVarGamejam.Map;
using VarVarGamejam.Menu;
using VarVarGamejam.Player.Behaviour;
using VarVarGamejam.SO;

namespace VarVarGamejam.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private PlayerInfo _info;
        [SerializeField]
        private Transform _head, _body;
        [SerializeField]
        private GameObject _tpsCamera, _topDownCamera;
        [SerializeField]
        private Shake _cameraShake;
        [SerializeField]
        private Light _torchlight;

        private List<AudioClip> _footstepsWalk, _footstepsRun;

        private AudioSource _audioSource;
		private CharacterController _controller;
        private bool _isSprinting;
        private float _verticalSpeed;
        private float _footstepDelay;

        private IPlayerBehaviour _playerBehaviour;
        private IPlayerBehaviour _tpsControls, _topDownControls;

        private bool _isGoalInHands;
        private bool _isNearGoal;

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            _controller = GetComponent<CharacterController>();
            Cursor.lockState = CursorLockMode.Locked;

            _footstepsWalk = _info.FootstepsWalk.ToList();
            _footstepsRun = _info.FootstepsRun.ToList();

            _tpsControls = new ThirdPersonBehaviour(transform, _head, _info, _tpsCamera);
            _topDownControls = new TopDownBehaviour(_body, _topDownCamera);

            MinimapCamera.Instance.PlayerLight = _torchlight;

            SwitchProfile(_topDownControls);
        }

        private void FixedUpdate()
		{
            var pos = _playerBehaviour.Movement;
			Vector3 desiredMove = transform.forward * pos.y + transform.right * pos.x;

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
				_verticalSpeed += Physics.gravity.y * _info.GravityMultiplicator;
				moveDir.y += _verticalSpeed;
			}

            var p = transform.position;
			_controller.Move(moveDir);

            // Footsteps
            _footstepDelay -= Vector3.SqrMagnitude(p - transform.position);
            if (_footstepDelay < 0f)
            {
                var target = _isSprinting ? _footstepsRun : _footstepsWalk;
                var clipIndex = Random.Range(1, target.Count);
                var clip = target[clipIndex];
                target.RemoveAt(clipIndex);
                target.Insert(0, clip);

                _audioSource.PlayOneShot(clip);
                _footstepDelay += _info.FootstepDelay * (_isSprinting ? _info.FootstepDelayRunMultiplier : 1f);
            }

            // Check if the player is going backward
            var intPos = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));
            if (MapGeneration.Instance.IsGoingBackward(intPos))
            {
                StartCoroutine(MapGeneration.Instance.Regenerate(intPos));
            }
        }

        private void TogglePossibleGoalTake(bool value)
        {
            _isNearGoal = value;
            GoalManager.Instance.ToggleTakeHelp(value);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Goal"))
            {
                if (_playerBehaviour == _topDownControls)
                {
                    // We are near goal, switch to TPS camera
                    SwitchProfile(_tpsControls);

                    // Now that the user, we introduce the notion that he can't go back
                    MapGeneration.Instance.EnableBackwardPrevention();
                    TogglePossibleGoalTake(true);
                }
                else if (!_isGoalInHands)
                {
                    TogglePossibleGoalTake(true);

                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Goal") && _isNearGoal)
            {
                TogglePossibleGoalTake(false);
            }
        }

        private void SwitchProfile(IPlayerBehaviour target)
        {
            _playerBehaviour?.Disable();

            _tpsCamera.SetActive(false);
            _topDownCamera.SetActive(false);
            target.TargetCamera.SetActive(true);
            _playerBehaviour = target;

            _playerBehaviour.Enable();
        }

		public void OnMovement(InputAction.CallbackContext value)
        {
            _playerBehaviour.OnKeyboardInput(value.ReadValue<Vector2>().normalized);
        }

        public void OnLook(InputAction.CallbackContext value)
        {
            var rot = value.ReadValue<Vector2>();
            _playerBehaviour.OnMouseMove(rot);
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

        public void ChangeView(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                SwitchProfile(_playerBehaviour == _tpsControls ? _topDownControls : _tpsControls);
            }
        }

        public void OnAction(InputAction.CallbackContext value)
        {
            if (value.performed && !_isGoalInHands && _isNearGoal)
            {
                TogglePossibleGoalTake(false);
                _isGoalInHands = true;
                GoalManager.Instance.TakeObjective();
                _cameraShake.Launch(.25f, .25f);
            }
        }
    }
}
