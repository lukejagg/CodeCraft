using System.Collections;
using System.Collections.Generic;
using ObjectEditor;
using UnityEngine;

namespace Programmer
{
	public class PlayInputs : MonoBehaviour
	{
		// WASD, Mouse, Left, Right, Space, E
		public static Vector3 MoveDirection;
		public static Vector3 CameraMovement;
		public static bool LeftClick, RightClick, JumpClick, ActionClick;
		public static bool PrevLeftClick, PrevRightClick, PrevJumpClick, PrevActionClick;

		public bool lockCursor = true;

		public bool useGUI;
		public GameObject playInputObjects;

		public Transform playCanvas;
		public Joystick moveJoystick;
		public RectTransform lookArea;
		public int moveAreaTouchId = -1;
		public int leftId, rightId, jumpId, actionId;
		public RectTransform leftClickButton, rightClickButton, jumpButton, actionButton;

		// Initialize useGUI
		void Awake()
		{
#if UNITY_IOS
			useGUI = true;
#else
			useGUI = false;
#endif
		}

		// Initialize GUI
		void Start()
		{
			if (useGUI)
			{
				moveJoystick.Initialize();
			}

			playInputObjects.SetActive(useGUI);
		}

		void OnEnable()
		{
			lockCursor = true;
		}

		bool Click(RectTransform t, Touch p, int index)
		{
			if (p.phase == TouchPhase.Ended) return false;
			if (p.fingerId == index) return true;
			return p.phase == TouchPhase.Began && RectTransformUtility.RectangleContainsScreenPoint(t, p.position);
		}

		// Update Inputs
		public void UpdateInputs()
		{
			MoveDirection = Vector3.zero;
			CameraMovement = Vector3.zero;

			if (useGUI)
			{
				// Movement
				Vector2 pos;
				bool beganTouch = false;
				bool touchedMove = false, touchedLook = false;
				foreach (var touch in Input.touches)
				{
					if (!touchedMove && moveJoystick.GetTouch(touch, playCanvas, out pos))
					{
						touchedMove = true;
						MoveDirection = new Vector2(pos.x, pos.y);

						if (touch.phase == TouchPhase.Began) beganTouch = true;
					}

					if (!touchedLook)
					{
						if (touch.fingerId == moveAreaTouchId ||
						    (touch.phase == TouchPhase.Began &&
						     RectTransformUtility.RectangleContainsScreenPoint(lookArea, touch.position)))
						{
							var delta = touch.deltaPosition;

							moveAreaTouchId = touch.fingerId;
							CameraMovement = delta / 25 / playCanvas.localScale.x * Settings.Sensitivity;
							touchedLook = true;
						}
					}
				}

				if (!touchedMove) moveJoystick.Reset();
				if (!touchedLook) moveAreaTouchId = -1;

				PrevLeftClick = LeftClick;
				PrevRightClick = RightClick;
				PrevJumpClick = JumpClick;
				PrevActionClick = ActionClick;

				// Buttons
				LeftClick = false;
				RightClick = false;
				JumpClick = false;
				ActionClick = false;

				foreach (var touch in Input.touches)
				{
					if (Click(leftClickButton, touch, leftId))
					{
						leftId = touch.fingerId;
						LeftClick = true;
					}

					if (Click(rightClickButton, touch, rightId))
					{
						rightId = touch.fingerId;
						RightClick = true;
					}

					if (Click(jumpButton, touch, jumpId))
					{
						jumpId = touch.fingerId;
						JumpClick = true;
					}

					if (Click(actionButton, touch, actionId))
					{
						actionId = touch.fingerId;
						ActionClick = true;
					}
				}

				if (!LeftClick) leftId = -1;
				if (!RightClick) rightId = -1;
				if (!JumpClick) jumpId = -1;
				if (!ActionClick) actionId = -1;
			}
			else
			{
				if (Input.GetKeyDown(KeyCode.Escape))
					lockCursor = !lockCursor;

				if (lockCursor)
				{
					Cursor.lockState = CursorLockMode.Locked;
				}
				else
				{
					Cursor.lockState = CursorLockMode.None;
				}

				MoveDirection = new Vector2(KBMInput(KeyCode.D, KeyCode.A), KBMInput(KeyCode.W, KeyCode.S));
				CameraMovement = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * Settings.Sensitivity;

				PrevLeftClick = LeftClick;
				PrevRightClick = RightClick;
				PrevJumpClick = JumpClick;
				PrevActionClick = ActionClick;

				LeftClick = Input.GetKey(KeyCode.Mouse0);
				RightClick = Input.GetKey(KeyCode.Mouse1);
				JumpClick = Input.GetKey(KeyCode.Space);
				ActionClick = Input.GetKey(KeyCode.E);
			}

			if (MoveDirection.sqrMagnitude > 1) MoveDirection.Normalize();
		}

		float KBMInput(KeyCode positive, KeyCode negative)
		{
			return (Input.GetKey(positive) ? 1 : 0) - (Input.GetKey(negative) ? 1 : 0);
		}
	}
}