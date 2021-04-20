using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using CodeEditor;
using Programmer;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ObjectEditor
{
    public enum Axis
    {
        None,
        X,
        Y,
        Z
    }

    public enum MoveType : int
    {
        None = -1,
        Move = 0,
        Rotate = 1,
        Scale = 2,
    }

    public class MoveCompared
    {
        public Axis movementAxis;
        public float dist;

        public MoveCompared(Axis movementAxis, float dist)
        {
            this.movementAxis = movementAxis;
            this.dist = dist;
        }
    }

    [Serializable]
    public class Joystick
    {
	    public static float Deadzone = 0.01f;
	    public RectTransform joystick;
	    public RectTransform center;
	    public int touchId;
	    public RectTransform initialPos;

	    public void Initialize()
	    {
		    
	    }

	    public bool GetTouch(Touch touch, Transform canvas, out Vector2 pos)
	    {
            var delta = touch.position - (Vector2)joystick.position;
            var scale = canvas.localScale.x;

            if (touch.fingerId == touchId || (touch.phase == TouchPhase.Began && delta.magnitude < joystick.sizeDelta.x / 2 * scale))
            {
	            if (touch.phase == TouchPhase.Began)
	            {
		            joystick.position = touch.position;
		            delta = Vector2.zero;
	            }

	            touchId = touch.fingerId;
                var l = (joystick.sizeDelta.x / 2 - center.sizeDelta.x / 2);
			    center.anchoredPosition = Vector3.ClampMagnitude(delta / scale, l);
			    pos = Vector2.ClampMagnitude(center.anchoredPosition / l, 1);
			    var m = (pos.magnitude - Deadzone) / (1 - Deadzone);
			    pos *= Mathf.Clamp01(m);
			    return true;
		    }

		    pos = Vector2.zero;
		    return false;
	    }

	    public void Reset()
	    {
		    touchId = -1;
		    center.anchoredPosition = Vector2.zero;
		    joystick.position = initialPos.position;
	    }
    }

    public class ObjectEdit : MonoBehaviour
    {
	    public Saving saving;
        public Camera cam;
        float camX, camY;

        [Header("Script Editor")]
        public Editor codeEditor;

        [Header("GUI Objects")]
        public bool useGUI = false;
        public GameObject guiGameObject;
        public Transform canvas;
        public Joystick moveJoystick, lookJoystick;
        public RectTransform lookArea;
        public EventSystem eventSystem;
        public GameObject addScrollView;
        public GameObject menuView;
        public GameObject settingsView;

        [Header("Settings")] 
        public InputField snapInputField;
        public Slider snapSlider;

        public InputField rotationSnapInputField;
        public Slider rotationSnapSlider;

        public InputField editorSpeedInputField;
        public Slider editorSpeedSlider;

        public Toggle localMoveToggle;

        [Header("GUI Buttons")]
        public GameObject deleteButton;
        public GameObject duplicateButton;
        public GameObject addButton;
        public Transform deleteStorage;
        public GameObject undoButton;
        public GameObject redoButton;
        public GameObject scriptButton;

        [Header("Undo/Redo")] 
        public List<UndoAction> undoActions = new List<UndoAction>();
        public List<UndoAction> redoActions = new List<UndoAction>();

        [Header("Editing Settings")]
        public bool useKBM = true;
        public float moveSpeed = 10;
        public float zoom = 0;
        public float sensitivity = 1;

        public LayerMask selectMask;
        public float positionSnap = 0.5f;
        public float rotationSnap = 15f;

        [Header("Axis Materials")]
        public Material matX;
        public Material disabledMatX;
        public Material matY;
        public Material disabledMatY;
        public Material matZ;
        public Material disabledMatZ;

        [Header("Position Objects")]
        public Transform moveObject;
        public MoveArrowObject moveObjectX;
        public MoveArrowObject moveObjectY;
        public MoveArrowObject moveObjectZ;
        Arrow moveArrowX, moveArrowY, moveArrowZ;

        [Header("Rotation Objects")]
        public Transform rotObject;
        public Transform rotObjectX;
        public Transform rotObjectY;
        public Transform rotObjectZ;

        [Header("Scale Objects")]
        public Transform scaleObject;
        public MoveArrowObject scaleObjectX;
        public MoveArrowObject scaleObjectY;
        public MoveArrowObject scaleObjectZ;

        [Header("Mouse Settings")]
        Vector3 lastMousePos;
        public float selectDist; // distance to select movement axis

        [Header("Selections")]
        public bool local = false;
        public Axis movementAxis = Axis.None;
        public MoveType movementType = MoveType.Move;
        public SelectableObject selected;
        public MeshRenderer selectedMeshRenderer;

        [Header("Axis Sorts")]
        public List<MoveCompared> axisWeights;
        public MoveCompared axisXWeight, axisYWeight, axisZWeight;

        [Header("Position Values")]
        public Vector3 originalPos;
        public Vector3 selectedPos;

        [Header("Rotation Values")]
        public Quaternion originalRot;
        public float currentRot;

        [Header("Scale Values")]
        public Vector3 originalScale;
        public bool inverseDirection;
        public float currentScale;

        [Header("Move Type")]
        public Image ChangeMoveTypeButton;
        public Sprite[] MoveTypeImages;

        //public float testVel = 40, testDamp = 0.9f;
        //Springs.Vector3Spring testSpring = new Springs.Vector3Spring(Vector3.zero, Vector3.zero, Vector3.zero);

        float ZoomMultiplier => Mathf.Exp(zoom);

        void UpdateArrow(Arrow arrow, Transform obj)
        {
            var p1 = selected.transform.position + obj.up;
            arrow.Update(cam.WorldToScreenPoint(selected.transform.position), cam.WorldToScreenPoint(p1));
        }

        void CalculateArrowVectors(Transform x, Transform y, Transform z)
        {
            UpdateArrow(moveArrowX, x);
            UpdateArrow(moveArrowY, y);
            UpdateArrow(moveArrowZ, z);
        }

        // Start is called before the first frame update
        void Start()
        {
            moveArrowX = new Arrow();
            moveArrowY = new Arrow();
            moveArrowZ = new Arrow();

            selectDist = ((Screen.width + Screen.height) / 2) / 20;

            axisWeights = new List<MoveCompared>();
            axisXWeight = new MoveCompared(Axis.X, 0);
            axisYWeight = new MoveCompared(Axis.Y, 0);
            axisZWeight = new MoveCompared(Axis.Z, 0);
            axisWeights.Add(axisXWeight);
            axisWeights.Add(axisYWeight);
            axisWeights.Add(axisZWeight);

            uiPointer = new PointerEventData(eventSystem);

            lookJoystick.Initialize();
            moveJoystick.Initialize();

#if UNITY_IOS
            useGUI = true;
#else
            useGUI = false;
#endif
        }

#if UNITY_WEBGL
	    public static bool IsMobile()
	    {
		    return SystemInfo.deviceType == DeviceType.Handheld;
	    }
#endif

        void MoveCamera(Vector3 move)
        {
            cam.transform.Translate(move * Time.deltaTime * moveSpeed * ZoomMultiplier, Space.Self);
        }

        void RotateCamera(float dx, float dy)
        {
            camX += dx * sensitivity * Settings.Sensitivity;
            camY += dy * sensitivity * Settings.Sensitivity;
            cam.transform.rotation = Quaternion.Euler(camX, camY, 0);
            camX %= 360;
            camY %= 360;
        }

        void Select(Transform t)
        {
            selected = t.GetComponent<SelectableObject>();
            selectedMeshRenderer = selected?.GetComponent<MeshRenderer>();
            UpdateSelectedVariables();
        }

        void MouseSelect()
        {
            var ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit, 1000, selectMask))
            {
                Select(hit.transform);
            }
            else
            {
                selected = null;
            }
        }

#region Position Movement
        void MovePosition(Vector3 delta, Arrow arrow, Vector3 faceVector, bool add)
        {
            var p1 = cam.WorldToScreenPoint(selected.transform.position);
            var p2 = cam.WorldToScreenPoint(selected.transform.position + (moveObject.TransformDirection(faceVector)));
            selectedPos += (add ? 1 : -1) * faceVector * Vector3.Dot(delta, arrow.vec) / Vector3.Distance(p1, p2);
        }

        void MovePosition(Vector2 delta)
        {
            switch(movementAxis)
            {
                case Axis.X:
                    MovePosition(delta, moveArrowX, Vector3.right, false);
                    break;
                case Axis.Y:
                    MovePosition(delta, moveArrowY, Vector3.up, true);
                    break;
                case Axis.Z:
                    MovePosition(delta, moveArrowZ, Vector3.forward, true);
                    break;
            }
        }
#endregion

#region Rotation Movement
        void MoveRotation(Vector2 pos, Vector2 delta, Vector3 faceVector)
        {
            var p1 = cam.WorldToScreenPoint(moveObject.position);
            var vec = pos - new Vector2(p1.x, p1.y);
            var a1 = Mathf.Rad2Deg * Mathf.Atan2(vec.y, vec.x);
            var a2 = Mathf.Rad2Deg * Mathf.Atan2(vec.y + delta.y, vec.x + delta.x);
            var dot = Vector3.Dot(cam.transform.forward, faceVector);
            currentRot += (a2 - a1) * (dot > 0 ? 1 : -1);
        }

        void MoveRotation(Vector2 pos, Vector2 delta)
        {
            switch (movementAxis)
            {
                case Axis.X:
                    MoveRotation(pos, delta, moveObject.right);
                    break;
                case Axis.Y:
                    MoveRotation(pos, delta, moveObject.up);
                    //MoveRotationY(delta);
                    break;
                case Axis.Z:
                    MoveRotation(pos, delta, moveObject.forward);
                    //MoveRotationZ(delta);
                    break;
            }
        }
#endregion

#region Scaling Movement
        void MoveScale(Vector3 delta, Arrow arrow, Vector3 faceVector, bool add)
        {
            var p1 = cam.WorldToScreenPoint(selected.transform.position);
            var p2 = cam.WorldToScreenPoint(selected.transform.position + scaleObject.TransformDirection(faceVector));
            currentScale += (add ? 1 : -1) * Vector3.Dot(delta, arrow.vec) / Vector3.Distance(p1, p2);
        }

        void MoveScale(Vector2 delta)
        {
            if (selected.Scalable)
            {
                switch (movementAxis)
                {
                    case Axis.X:
                        MoveScale(delta, moveArrowX, Vector3.right, false);
                        break;
                    case Axis.Y:
                        MoveScale(delta, moveArrowY, Vector3.up, true);
                        break;
                    case Axis.Z:
                        MoveScale(delta, moveArrowZ, Vector3.forward, true);
                        break;
                }
            }
        }
#endregion

        void UpdateSelected(Vector2 pos, Vector2 delta)
        {
            switch (movementType)
            {
                case MoveType.Move:
                    MovePosition(delta);
                    break;
                case MoveType.Rotate:
                    MoveRotation(pos, delta);
                    break;
                case MoveType.Scale:
                    MoveScale(delta);
                    break;
            }
        }

        void ChangeMovementType(MoveType moveType)
        {
            movementType = moveType;
            if (selected != null)
            {
                UpdateSelectedVariables();
            }

            ChangeMoveTypeButton.sprite = MoveTypeImages[(int)movementType];
        }

        void MoveSelected()
        {
            switch(movementType)
            {
                case MoveType.Move:
                    var roundedPos = (positionSnap > 0.001f ? RoundVector3(selectedPos) : selectedPos);
                    selected.transform.position = originalPos + (local ? originalRot * roundedPos : roundedPos);
                    break;
                case MoveType.Rotate:
                    var snapRot = rotationSnap > 0.001f ? Round(currentRot, rotationSnap) : currentRot;
                    var rot = Quaternion.identity;
                    switch (movementAxis)
                    {
                        case Axis.X:
                            rot = Quaternion.Euler(snapRot, 0, 0);
                            break;
                        case Axis.Y:
                            rot = Quaternion.Euler(0, snapRot, 0);
                            break;
                        case Axis.Z:
                            rot = Quaternion.Euler(0, 0, snapRot);
                            break;
                    }
                    selected.transform.rotation = originalRot;
                    selected.transform.Rotate(rot.eulerAngles, local ? Space.Self : Space.World);
                    break;
                case MoveType.Scale:
                    var vec = Vector3.zero;
                    switch (movementAxis)
                    {
                        case Axis.X:
                            vec = Vector3.right;
                            break;
                        case Axis.Y:
                            vec = Vector3.up;
                            break;
                        case Axis.Z:
                            vec = Vector3.forward;
                            break;
                    }

                    var roundedScale = (positionSnap > 0.001f ? Round(currentScale, positionSnap) : currentScale);
                    if (float.IsNaN(currentScale)) break;

                    selected.transform.position = originalPos + selected.transform.TransformDirection(vec) * roundedScale / 2;
                    selected.transform.localScale = originalScale + (movementAxis == Axis.X ? -1 : 1) * (inverseDirection ? 1 : -1) * vec * roundedScale;
                    selected.transform.localScale = MaxVector3(selected.transform.localScale, 0.001f);
                    break;
            }

            /*if (reset)
            {
                originalPos = selected.position;
                selectedPos = Vector3.zero;
            }*/
        }


#region Selection
        int AxisSort(MoveCompared p1, MoveCompared p2)
        {
            return p1.dist.CompareTo(p2.dist);
        }

        float PositionWeight(Vector3 pos, Arrow arrow, Transform vec)
        {
            var d = Mathf.Abs(Vector3.Dot((vec.position - cam.transform.position).normalized, vec.up));
            return arrow.GetDistance(pos) * (d + 1) + d;
        }
        void SortPositions(Vector2 pos)
        {
            axisXWeight.dist = PositionWeight(pos, moveArrowX, moveObjectX.transform);
            axisYWeight.dist = PositionWeight(pos, moveArrowY, moveObjectY.transform);
            axisZWeight.dist = PositionWeight(pos, moveArrowZ, moveObjectZ.transform);
            axisWeights[0] = axisXWeight;
            axisWeights[1] = axisYWeight;
            axisWeights[2] = axisZWeight;
            axisWeights.Sort(AxisSort);
        }

        void SortScales(Vector2 pos)
        {
            axisXWeight.dist = PositionWeight(pos, moveArrowX, scaleObjectX.transform);
            axisYWeight.dist = PositionWeight(pos, moveArrowY, scaleObjectY.transform);
            axisZWeight.dist = PositionWeight(pos, moveArrowZ, scaleObjectZ.transform);
            axisWeights[0] = axisXWeight;
            axisWeights[1] = axisYWeight;
            axisWeights[2] = axisZWeight;
            axisWeights.Sort(AxisSort);
        }

        float RotationWeight(Transform vec)
        {
            return 1 - Mathf.Abs(Vector3.Dot((selected.transform.position - cam.transform.position).normalized, vec.up));
        }
        void SortRotations()
        {
            axisXWeight.dist = RotationWeight(moveObjectX.transform);
            axisYWeight.dist = RotationWeight(moveObjectY.transform);
            axisZWeight.dist = RotationWeight(moveObjectZ.transform);
            axisWeights[0] = axisXWeight;
            axisWeights[1] = axisYWeight;
            axisWeights[2] = axisZWeight;
            axisWeights.Sort(AxisSort);
        }

        void UpdateSelectedVariables()
        {
	        if (selected != null)
	        {
		        originalPos = selected.transform.position;
		        selectedPos = Vector3.zero;

		        originalRot = selected.transform.rotation;
		        currentRot = 0;

		        originalScale = selected.transform.localScale;
		        inverseDirection = false;
		        currentScale = 0;
	        }
        }
        void UpdateAxis(Vector2 pos)
        {
            var p1 = cam.WorldToScreenPoint(selected.transform.position);
            switch (movementType)
            {
                case MoveType.Move:
                    {
                        SortPositions(pos);
                        UpdateSelectedVariables();

                        var distObj = moveObjectX;
                        var arrow = moveArrowX;
                        switch(axisWeights[0].movementAxis)
                        {
                            case Axis.Y:
                                distObj = moveObjectY;
                                arrow = moveArrowY;
                                break;
                            case Axis.Z:
                                distObj = moveObjectZ;
                                arrow = moveArrowZ;
                                break;
                        }
                        
                        var p2 = arrow.AboveLine(pos) ? cam.WorldToScreenPoint(distObj.upCone.position) : cam.WorldToScreenPoint(distObj.downCone.position);

                        if (axisWeights[0].dist < selectDist && Vector2.Distance(pos, p1) < Vector2.Distance(p1, p2) + 25)
                        {
                            movementAxis = axisWeights[0].movementAxis;
                        }
                        else
                        {
                            movementAxis = Axis.None;
                        }
                    }
                    break;
                case MoveType.Rotate:
                    {
                        SortRotations();
                        UpdateSelectedVariables();
                        if (Vector2.Distance(pos, p1) / rotObjectX.lossyScale.x < selectDist / p1.z * 15)
                        {
                            movementAxis = axisWeights[0].movementAxis;
                        }
                        else
                        {
                            movementAxis = Axis.None;
                        }
                    }
                    break;
                case MoveType.Scale:
                    {
                        SortScales(pos);
                        UpdateSelectedVariables();

                        var distObj = scaleObjectX;
                        var arrow = moveArrowX;
                        switch (axisWeights[0].movementAxis)
                        {
                            case Axis.Y:
                                distObj = scaleObjectY;
                                arrow = moveArrowY;
                                break;
                            case Axis.Z:
                                distObj = scaleObjectZ;
                                arrow = moveArrowZ;
                                break;
                        }
                        var p2 = arrow.AboveLine(pos) ? cam.WorldToScreenPoint(distObj.upCone.position) : cam.WorldToScreenPoint(distObj.downCone.position);

                        if (axisWeights[0].dist < selectDist && Vector2.Distance(pos, p1) < Vector2.Distance(p1, p2) + 25)
                        {
                            movementAxis = axisWeights[0].movementAxis;

                            switch (movementAxis)
                            {
                                case Axis.X:
                                    inverseDirection = moveArrowX.AboveLine(pos);
                                    break;
                                case Axis.Y:
                                    inverseDirection = moveArrowY.AboveLine(pos);
                                    break;
                                case Axis.Z:
                                    inverseDirection = moveArrowZ.AboveLine(pos);
                                    break;
                            }
                        }
                        else
                        {
                            movementAxis = Axis.None;
                        }
                    }
                    break;
            }

            UpdateMaterials();
        }

        void SetMaterials(Transform obj1, Transform obj2, Material enabled, Material disabled, Axis axis)
        {
            var mat = movementAxis == Axis.None ? enabled : (movementAxis == axis ? enabled : disabled);

            foreach (var o in obj1.GetComponentsInChildren<MeshRenderer>())
            {
                o.sharedMaterial = mat;
            }
            foreach (var o in obj1.GetComponentsInChildren<MeshRenderer>())
            {
                o.sharedMaterial = mat;
            }
        }
        void UpdateMaterials()
        {
            SetMaterials(moveObjectX.transform, scaleObjectX.transform, matX, disabledMatX, Axis.X);
            SetMaterials(moveObjectY.transform, scaleObjectY.transform, matY, disabledMatY, Axis.Y);
            SetMaterials(moveObjectZ.transform, scaleObjectZ.transform, matZ, disabledMatZ, Axis.Z);
        }
#endregion

        float KBMInput(KeyCode positive, KeyCode negative)
        {
            return (Input.GetKey(positive) ? 1 : 0) - (Input.GetKey(negative) ? 1 : 0);
        }

        float Round(float n, float r)
        {
            return Mathf.Round(n / r) * r;
        }

        public Vector3 RoundVector3(Vector3 vec)
        {
            vec.x = Round(vec.x, positionSnap);
            vec.y = Round(vec.y, positionSnap);
            vec.z = Round(vec.z, positionSnap);
            return vec;
        }

        Vector3 MaxVector3(Vector3 vec, float maxValue)
        {
            vec.x = Mathf.Max(vec.x, maxValue);
            vec.y = Mathf.Max(vec.y, maxValue);
            vec.z = Mathf.Max(vec.z, maxValue);
            return vec;
        }

        void UpdateAxisObjects()
        {
            moveObject.position = selected.transform.position;
            moveObject.rotation = local ? selected.transform.rotation : Quaternion.identity;
            moveObject.gameObject.SetActive(movementType == MoveType.Move);
            if (movementType == MoveType.Move)
            {
                CalculateArrowVectors(moveObjectX.transform, moveObjectY.transform, moveObjectZ.transform);

                if (local || selectedMeshRenderer == null)
                {
                    var size = selected.transform.lossyScale;
                    moveObjectX.SetPos(size.x / 2);
                    moveObjectY.SetPos(size.y / 2);
                    moveObjectZ.SetPos(size.z / 2);
                }
                else
                {
                    var size = selectedMeshRenderer.bounds;
                    moveObjectX.SetPos(size.size.x / 2);
                    moveObjectY.SetPos(size.size.y / 2);
                    moveObjectZ.SetPos(size.size.z / 2);
                }
            }

            rotObject.position = selected.transform.position;
            rotObject.rotation = local ? selected.transform.rotation : Quaternion.identity;
            rotObject.gameObject.SetActive(movementType == MoveType.Rotate);
            if (movementType == MoveType.Rotate)
            {
                if (movementAxis != Axis.None)
                {
                    rotObjectX.gameObject.SetActive(movementAxis == Axis.X);
                    rotObjectY.gameObject.SetActive(movementAxis == Axis.Y);
                    rotObjectZ.gameObject.SetActive(movementAxis == Axis.Z);
                }
                else
                {
                    SortRotations();
                    var axis = axisWeights[0].movementAxis;
                    rotObjectX.gameObject.SetActive(axis == Axis.X);
                    rotObjectY.gameObject.SetActive(axis == Axis.Y);
                    rotObjectZ.gameObject.SetActive(axis == Axis.Z);
                }

                var size = Mathf.Max(selected.transform.lossyScale.x, selected.transform.lossyScale.y, selected.transform.lossyScale.z);
                rotObject.localScale = Vector3.one * size;
            }

            scaleObject.position = selected.transform.position;
            scaleObject.rotation = selected.transform.rotation;
            scaleObject.gameObject.SetActive(movementType == MoveType.Scale);
            if (movementType == MoveType.Scale)
            {
                CalculateArrowVectors(scaleObjectX.transform, scaleObjectY.transform, scaleObjectZ.transform);

                var size = selected.transform.lossyScale;
                scaleObjectX.SetPos(size.x / 2);
                scaleObjectY.SetPos(size.y / 2);
                scaleObjectZ.SetPos(size.z / 2);
            }
        }

        List<RaycastResult> uiRaycastCache = new List<RaycastResult>(2);
        PointerEventData uiPointer;
        public bool IsPointerOverUIObject()
        {
	        uiPointer.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
	        eventSystem.RaycastAll(uiPointer, uiRaycastCache);
	        return uiRaycastCache.Count > 0;
        }

        bool nextDeltaZero = false;
        void UpdateKBM()
        {
            var delta = Input.mousePosition - lastMousePos;
            delta.z = 0;

            if (nextDeltaZero)
            {
                delta = Vector2.zero;
                nextDeltaZero = false;
            }

            var interactingWithUi = IsPointerOverUIObject();

            if (selected != null)
            {
	            if (Input.GetMouseButtonDown(0) && !interactingWithUi)
	            {
		            delta = Vector2.zero;
		            UpdateAxis(Input.mousePosition);
	            }

	            if (Input.GetMouseButton(0))
	            {
		            UpdateSelected(Input.mousePosition, delta);
		            MoveSelected();
	            }

	            if (Input.GetMouseButtonUp(0))
	            {
		            switch (movementType)
		            {
                        case MoveType.Move:
	                        var roundedPos = (positionSnap > 0.001f ? RoundVector3(selectedPos) : selectedPos);
                            if (roundedPos.sqrMagnitude > 0) AddUndo(new MoveAction(selected.transform, originalPos));
	                        break;
                        case MoveType.Rotate:
	                        var snapRot = rotationSnap > 0.001f ? Round(currentRot, rotationSnap) : currentRot;
                            if (snapRot != 0) AddUndo(new RotateAction(selected.transform, originalRot));
	                        break;
                        case MoveType.Scale:
	                        var roundedScale = (positionSnap > 0.001f ? Round(currentScale, positionSnap) : currentScale);
                            if (roundedScale != 0) AddUndo(new ScaleAction(selected.transform, originalPos, originalScale));
	                        break;
                    }

		            movementAxis = Axis.None;
		            UpdateSelectedVariables();
		            UpdateMaterials();
	            }

	            UpdateAxisObjects();
            }
            else
            {
                moveObject.gameObject.SetActive(false);
                rotObject.gameObject.SetActive(false);
                scaleObject.gameObject.SetActive(false);
            }

            if (Input.GetMouseButtonDown(0) && !interactingWithUi)
            {
                if (selected)
                {
                    if (movementAxis == Axis.None)
                    {
                        MouseSelect();
                    }
                }
                else
                {
                    MouseSelect();
                }
            }

            MoveCamera(new Vector3(KBMInput(KeyCode.D, KeyCode.A), KBMInput(KeyCode.E, KeyCode.Q), KBMInput(KeyCode.W, KeyCode.S)));

            if (!useGUI)
            {
	            if (Input.GetMouseButton(1))
	            {
		            RotateCamera(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"));
		            if (Cursor.lockState == CursorLockMode.None)
		            {
			            nextDeltaZero = true;
			            //MoveSelected(true);
		            }

		            Cursor.lockState = CursorLockMode.Locked;
		            Cursor.visible = false;

		            zoom += (Input.GetKey(KeyCode.F) ? 0 : 1) * Input.GetAxis("Mouse ScrollWheel");
	            }
                else if (Input.GetMouseButton(2))
	            {
                    var deltaMouse = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

                    cam.transform.Translate(-deltaMouse * ZoomMultiplier / 5,
			            Space.Self);
                }
	            else
	            {
		            Cursor.lockState = CursorLockMode.None;
		            Cursor.visible = true;

		            cam.transform.Translate(Vector3.forward * Input.GetAxis("Mouse ScrollWheel") * 3 * ZoomMultiplier,
			            Space.Self);
	            }
            }

            if (movementAxis == Axis.None && Input.GetKeyDown(KeyCode.L))
            {
                local = !local;
            }

            if (selected && Input.GetKey(KeyCode.F))
            {
                cam.transform.position = selected.transform.position - cam.transform.rotation * Vector3.forward * ZoomMultiplier * 5;
                zoom -= Input.GetAxis("Mouse ScrollWheel");
            }

            if (selected && Input.GetKeyDown(KeyCode.X))
            {
                selected = null;
            }

            if (selected && Input.GetKeyDown(KeyCode.C))
            {
	            Clone();
            }
            if (selected && Input.GetKeyDown(KeyCode.Delete))
            {
                Destroy(selected.gameObject);
                selected = null;
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                ChangeMovementType(MoveType.Move);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                ChangeMovementType(MoveType.Rotate);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                ChangeMovementType(MoveType.Scale);
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                OpenSelected();
            }

            lastMousePos = Input.mousePosition;
        }

        private int moveAreaTouchId = -1;
        //private Vector2 lastMoveAreaPos;

        public bool UpdateGUI()
        {
	        Vector2 pos;
	        bool beganTouch = false;
	        bool touchedMove = false, touchedLook = false;
	        foreach (var touch in Input.touches)
	        {
		        if (!touchedMove && moveJoystick.GetTouch(touch, canvas, out pos))
		        {
			        touchedMove = true;
			        MoveCamera(new Vector3(pos.x, 0, pos.y));

			        if (touch.phase == TouchPhase.Began) beganTouch = true;
                }

		        if (!touchedLook)
		        {
			        if (!addScrollView.activeSelf || touch.phase != TouchPhase.Began || touch.phase == TouchPhase.Began && !RectTransformUtility.RectangleContainsScreenPoint(addScrollView.transform as RectTransform, touch.position))
			        {
				        if (Settings.LookType == JoystickType.Joystick)
				        {
					        if (lookJoystick.GetTouch(touch, canvas, out pos))
					        {
						        touchedLook = true;
						        pos *= Time.deltaTime * 100;
						        RotateCamera(-pos.y, pos.x);

						        if (touch.phase == TouchPhase.Began) beganTouch = true;
					        }
				        }
				        else if (Settings.LookType == JoystickType.Move)
				        {
					        if (touch.fingerId == moveAreaTouchId ||
					            (touch.phase == TouchPhase.Began &&
					             RectTransformUtility.RectangleContainsScreenPoint(lookArea, touch.position)))
					        {
						        //if (touch.phase == TouchPhase.Began) lastMoveAreaPos = touch.position;
						        //var delta = touch.position - lastMoveAreaPos;
						        //lastMoveAreaPos = touch.position;
						        var delta = touch.deltaPosition;

						        moveAreaTouchId = touch.fingerId;
						        RotateCamera(-delta.y / 25f / canvas.localScale.x, delta.x / 25f / canvas.localScale.x);
						        touchedLook = true;
					        }
				        }
			        }
		        }
	        }

            if (!touchedMove) moveJoystick.Reset();
            if (!touchedLook)
            {
	            if (Settings.LookType == JoystickType.Joystick) lookJoystick.Reset();
	            else moveAreaTouchId = -1;
            }

	        return beganTouch;
        }

        public void Clone()
        {
	        var copy = Instantiate(selected.transform, selected.transform.parent).GetComponent<SelectableObject>();
	        copy.name = selected.name;
	        selected = copy;
	        UpdateSelectedVariables();

	        if (selected.Script != null)
	        {
		        var code = selected.Script.Save();
		        copy.Script = null;
		        saving.LoadCode(copy, new SubstringOptimizedString(code));
	        }

            AddUndo(new ParentAction(selected.transform, deleteStorage));

            var iobj = copy.GetComponent<IObject>();
            iobj.Index = 0;
            saving.AddObjectIndex(iobj);
        }

        public void OpenSelected()
        {
            if (selected != null)
            {
                codeEditor.OpenScript(selected);
                CodeView.Instance?.OpenEvents();
            }
        }

        public void DeleteSelected()
        {
	        var oldParent = selected.transform.parent;
            AddUndo(new ParentAction(selected.transform, oldParent)); 
            selected.transform.parent = deleteStorage;
	        selected = null;
        }

		public void AddUndo(UndoAction undo)
		{
		    if (undo != null)
		    {
			    foreach (var action in redoActions)
			    {
				    if (action is ParentAction p)
				    {
					    if (p.obj != null && p.obj.parent == deleteStorage)
					    {
						    var obj = p.obj.GetComponent<SelectableObject>();
						    if (obj.Script != null) Destroy(obj.Script.gameObject);
						    Destroy(obj.gameObject);
                        }
				    }
			    }
		        redoActions.Clear();

		        undoActions.Add(undo);
		    }
		}

		public void Undo()
		{
		    var count = undoActions.Count;
		    if (count > 0)
		    {
			    selected = null;
			    var r = undoActions[count - 1];
		        r.Undo(); // Undo the undo
		        undoActions.RemoveAt(count - 1);
		        redoActions.Add(r);
		    }
		}

		public void Redo()
		{
		    var count = redoActions.Count;
		    if (count > 0)
		    {
			    selected = null;
                var r = redoActions[count - 1];
		        r.Undo(); // Undo the undo
		        redoActions.RemoveAt(count - 1);
		        undoActions.Add(r);
		    }
		}

		public void ToggleAddView()
		{
			addScrollView.SetActive(!addScrollView.activeSelf);
		}

		public void ToggleMenuView()
		{
            menuView.SetActive(!menuView.activeSelf);
		}

		public void ChangeMoveType()
		{
            ChangeMovementType((MoveType) (((int)movementType + 1) % 3));
		}

		public void ToggleSettingsView()
		{
            settingsView.SetActive(!settingsView.activeSelf);
		}

		public void UpdateSnapSlider()
		{
			var snap = Mathf.Round(snapSlider.value * 20) / 20;

            snapInputField.text = $"{snap}";
			positionSnap = snapSlider.value;
		}

		public void UpdateSnapInputField()
		{
			if (float.TryParse(snapInputField.text, out var n))
			{
				//snapSlider.value = n;
				positionSnap = n;
			}
			else
			{
				//snapSlider.value = 0.5f;
				positionSnap = n;
            }
		}

		public void UpdateRotationSnapSlider()
		{
			var val = Mathf.Round(rotationSnapSlider.value / 5) * 5;
			rotationSnapInputField.text = $"{val}";
			rotationSnap = rotationSnapSlider.value;
		}

		public void UpdateRotationSnapInputField()
		{
			if (float.TryParse(rotationSnapInputField.text, out var n))
			{
				//rotationSnapSlider.value = n;
				rotationSnap = n;
			}
			else
			{
				//rotationSnapSlider.value = 0.5f;
				rotationSnap = n;
			}
		}

		public void UpdateEditorSpeedSlider()
		{
			var val = Mathf.Round(editorSpeedSlider.value * 10) / 10;
			editorSpeedInputField.text = $"{val}";
			zoom = editorSpeedSlider.value;
		}

		public void UpdateEditorSpeedInputField()
		{
			if (float.TryParse(editorSpeedInputField.text, out var n))
			{
				zoom = n;
			}
			else
			{
				zoom = n;
			}
		}

		public void UpdateLocalMove()
		{
			local = localMoveToggle.isOn;
		}

        private void Update()
        {
            guiGameObject.SetActive(useGUI && !codeEditor.UsingEditor);

            if (selected != null) 
                addScrollView.SetActive(false);

            if (codeEditor.UsingEditor)
            {
                if (useKBM)
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;

                    if (Input.GetKeyDown(KeyCode.Tab))
                    {
                        codeEditor.CloseEditor();
                    }
                }
            }
            else
            {
	            var beganTouch = false;

	            addButton.SetActive(selected == null);
	            deleteButton.SetActive(selected != null);
	            duplicateButton.SetActive(selected != null);

	            undoButton.SetActive(undoActions.Count > 0);
	            redoButton.SetActive(redoActions.Count > 0);

                scriptButton.SetActive(selected != null);

                if (useGUI)
	            {
                    lookJoystick.joystick.gameObject.SetActive(Settings.LookType == JoystickType.Joystick);
                    lookArea.gameObject.SetActive(Settings.LookType == JoystickType.Move); 
                    
                    beganTouch = UpdateGUI();
	            }
	            if (!beganTouch) UpdateKBM();
            }
        }
    }
}