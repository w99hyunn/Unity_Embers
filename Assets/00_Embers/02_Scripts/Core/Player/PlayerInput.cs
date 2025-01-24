
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NOLDA
{
	public class PlayerInput : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;
		
		[Header("Cinemachine Settings")]
		public CinemachineThirdPersonFollow thirdPersonFollow;
		public float scrollSpeed = 0.5f;
		public float minZ = 1f;
		public float maxZ = 8f;
		
		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			LookInput(value.Get<Vector2>());
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}

		public void OnScroll(InputValue value)
		{
			Vector2 scrollInput = value.Get<Vector2>();
			AdjustShoulderOffset(scrollInput.y);
		}
		
		private void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		private void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		private void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		private void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}
		
		private void AdjustShoulderOffset(float scrollInput)
		{
			if (thirdPersonFollow != null && scrollInput != 0)
			{
				float currentDistance = thirdPersonFollow.CameraDistance;
				currentDistance += -scrollInput * scrollSpeed;
				currentDistance = Mathf.Clamp(currentDistance, minZ, maxZ);

				thirdPersonFollow.CameraDistance = currentDistance;
			}
		}
	}
	
}