using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;

		[Header("Movement Settings")]
		public bool analogMovement;

#if !UNITY_IOS || !UNITY_ANDROID
		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;
#endif

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED

  //      public void OnMove(InputValue value)
  //      {
  //          MoveInput(value.Get<Vector2>());
  //      }

  //      public void OnLook(InputValue value)
		//{
		//	if(cursorInputForLook)
		//	{
		//		LookInput(value.Get<Vector2>());
		//	}
		//}

		//public void OnJump(InputValue value)
		//{
		//	SetJumpInputServerRpc(value.isPressed);
		//}

		//public void OnSprint(InputValue value)
		//{
		//	SetSprintInputServerRpc(value.isPressed);
		//}
#else
	// old input sys if we do decide to have it (most likely wont)...
#endif


		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		}

		[Unity.Netcode.ServerRpc]
		private void SetMoveInputServerRpc(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		}


		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		[Unity.Netcode.ServerRpc]
		public void SetJumpInputServerRpc(bool newJumpState)
		{
			jump = newJumpState;
		}

		[Unity.Netcode.ServerRpc]
		public void SetSprintInputServerRpc(bool newSprintState)
		{
			sprint = newSprintState;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}

#if !UNITY_IOS || !UNITY_ANDROID

		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}

#endif

	}
	
}