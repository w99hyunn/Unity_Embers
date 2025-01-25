
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
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
			_ = ScrollInput(value.Get<Vector2>());
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
		
		private async Awaitable ScrollInput(Vector2 scrollDelta)
		{
			await Awaitable.NextFrameAsync(); //InputSystem 경고 방지를 위한 1프레임대기
			if (IsPointerOverUI()) return; // UI 위라면 스크롤 입력 무시

			Vector2 scrollInput = scrollDelta;
			AdjustShoulderOffset(scrollInput.y);
		}
		
		private bool IsPointerOverUI()
		{
			// EventSystem을 사용하여 포인터가 UI 위에 있는지 확인
			return EventSystem.current != null
			       && EventSystem.current.IsPointerOverGameObject(); // UI요소를 감지하는 함수
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