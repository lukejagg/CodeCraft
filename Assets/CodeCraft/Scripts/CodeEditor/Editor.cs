using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Interpreter;
using System.Threading.Tasks;
using ObjectEditor;
using UnityEngine.SceneManagement;

namespace CodeEditor
{
    public class Editor : MonoBehaviour
    {
        public bool UsingEditor = false;
        public GameObject[] EditorObjects;
        public GameObject[] SceneObjects;
        public Transform scriptPrefab;
        public Transform scripts;
        public Toggle disableMovement;

        public bool UsingKBM = true;
        [SerializeField] private Canvas canvas = null;
        [SerializeField] private GraphicRaycaster graphicRaycaster = null;
        [SerializeField] private EventSystem eventSystem = null;
        [SerializeField] private Transform currentScriptObject = null;
        [SerializeField] private ScrollRect blockPrefabsScroll = null, codeViewScroll = null;
        [SerializeField] private RectTransform codeScrollContent = null;
        [SerializeField] public B_BeginEvent beginEvent = null;
        [SerializeField] private RectTransform deletePosition = null;

        [SerializeField] private Transform currentClonedObject = null;
        [SerializeField] private CodeType clonedObjectType = CodeType.Event;

        // Movement variables
        [SerializeField] private bool createdObject = true;
        [SerializeField] private ICodeType currentClonedType = null;
        //[SerializeField] private Transform codeOldParent = null;
        [SerializeField] object previousConnection = null;

        private HashSet<string> connectionTags;
        private HashSet<string> codeTags;
        private HashSet<string> prefabTags;
        private List<RaycastResult> raycastResult;

        [SerializeField] private AudioSource clickAudioSource;

        // Code outside of the main code
        public RectTransform otherCodeTransform;

        // Start is called before the first frame update
        void Start()
        {
            connectionTags = new HashSet<string>();
            connectionTags.Add("Value");
            connectionTags.Add("Event");
            connectionTags.Add("Function");

            codeTags = new HashSet<string>();
            codeTags.Add("Code");

            prefabTags = new HashSet<string>();
            prefabTags.Add("Prefab");

            raycastResult = new List<RaycastResult>();
        }


        #region UI Raycasting
        string GetTagFromType(CodeType type)
        {
            switch(type)
            {
                case CodeType.Event:
                    return "Event";
                case CodeType.Function:
                    return "Function";
                case CodeType.Value:
                    return "Value";
            }
            return "";
        }

        Transform CodeRaycast(Vector2 pos, string tag)
        {
            raycastResult.Clear();

            var eventData = new PointerEventData(eventSystem);
            eventData.position = pos;
            graphicRaycaster.Raycast(eventData, raycastResult);

            raycastResult.RemoveAll((res) => { return !tag.Equals(res.gameObject.tag) || (currentClonedObject != null && res.gameObject.transform.IsChildOf(currentClonedObject)); });

            if (raycastResult.Count > 0)
            {
                return raycastResult[0].gameObject.transform;
            }
            return null;
        }

        Transform CodeRaycast(Vector2 pos, HashSet<string> tags)
        {
            raycastResult.Clear();

            var eventData = new PointerEventData(eventSystem);
            eventData.position = pos;
            graphicRaycaster.Raycast(eventData, raycastResult);

            raycastResult.RemoveAll((res) => { return !tags.Contains(res.gameObject.tag) || (currentClonedObject != null && res.gameObject.transform.IsChildOf(currentClonedObject)); });

            if (raycastResult.Count > 0)
            {
                return raycastResult[0].gameObject.transform;
            }
            return null;
        }
        #endregion

        void PlaceCode(Transform hit)
        {
            bool delete = true;

            if (hit)
            {
                switch (clonedObjectType)
                {
                    case CodeType.Event:
                        var currentEvent = currentClonedObject.GetComponentInParent<ICompilerEvent>();
                        var eventConnection = hit.GetComponentInParent<ICompilerEvent>();
                        if (currentEvent != null && eventConnection != null && !hit.IsChildOf(otherCodeTransform))
                        {
                            var nextConnection = eventConnection.NextConnection;
                            if (nextConnection != null)
                            {
                                nextConnection.Offset.Connection = currentEvent.NextConnectionObject;
                            }
                            eventConnection.NextConnection = currentEvent;

                            currentEvent.NextConnection = nextConnection;

                            var offset = currentClonedObject.gameObject.AddComponent<BlockOffset>();
                            offset.Connection = hit;
                            currentEvent.Offset = offset;

                            if (hit.IsChildOf(otherCodeTransform))
                            {
	                            currentClonedObject.SetParent(otherCodeTransform, false);
	                            currentClonedObject.localScale = Vector3.one;
                                //ReconnectOtherCode();
                            }
                            else
                            {
	                            currentClonedObject.SetParent(currentScriptObject, false);
	                            //ReconnectAll();
                            }
                            ReconnectAll();

                            delete = false;
                            currentClonedObject = null;
                        }
                        else if (hit.IsChildOf(otherCodeTransform))
                        {
	                        var iUseOffset = currentClonedObject.GetComponent<IUseOffset>();
	                        var boffset = currentClonedObject.gameObject.AddComponent<BlockOffset>();
	                        boffset.Connection = null;
	                        iUseOffset.Offset = boffset;

	                        currentClonedObject.SetParent(otherCodeTransform);
	                        currentClonedObject.localScale = Vector3.one;
                            currentClonedObject = null;
                        }

                        break;



                    case CodeType.Function:
                        var currentCode = currentClonedObject.GetComponentInParent<ICompilerCode>();
                        var codeConnection = hit.GetComponentInParent<ICompilerCodeConnection>();

                        if (currentCode != null && codeConnection != null)
                        {
                            var con = codeConnection.GetCodeConnection(hit);

                            if (con.HasValue)
                            {
                                var codeCon = con.Value;

                                var nextConnection = codeCon.code;

                                if (nextConnection != null)
                                {
                                    nextConnection.Offset.Connection = currentCode.CodeConnectionObjects[0];
                                }
                                codeConnection.CodeConnections[codeCon.codeIndex] = currentCode;

                                currentCode.CodeConnections[0] = nextConnection;

                                var offset = currentClonedObject.gameObject.AddComponent<BlockOffset>();
                                offset.Connection = hit;
                                currentCode.Offset = offset;

                                if (hit.IsChildOf(otherCodeTransform))
                                {
	                                currentClonedObject.SetParent(otherCodeTransform, false);
	                                //ReconnectOtherCode();
                                }
                                else
                                {
	                                currentClonedObject.SetParent(currentScriptObject, false);
	                                //ReconnectAll();
                                }
                                ReconnectAll();

                                delete = false;
                                currentClonedObject = null;
                            }
                        }
                        break;



                    case CodeType.Value:
                        var currentValue = currentClonedObject.GetComponentInParent<ICompilerValue>();
                        var valueConnection = hit.GetComponentInParent<ICompilerValues>();

                        if (currentValue != null && valueConnection != null)
                        {
                            var con = valueConnection.GetValue(hit);

                            if (con.HasValue)
                            {
                                var val = con.Value;

                                var v0 = valueConnection.Values[val.valueIndex];
                                if (v0 != null)
                                {
                                    //Destroy(v0.transform.gameObject);
                                    //valueConnection.Values[0] = null;
                                }
                                else
                                {

                                    valueConnection.Values[val.valueIndex] = currentValue;

                                    var offset = currentClonedObject.gameObject.AddComponent<ValueOffset>();
                                    offset.Connection = hit;
                                    currentValue.Offset = offset;
                                    currentClonedObject.SetParent(hit, false);
                                    CodeConnect(valueConnection.Transform.GetComponentInParent<ICompilerCode>());

                                    delete = false;
                                    currentClonedObject = null;
                                }

                                if (hit != null && hit.IsChildOf(otherCodeTransform))
									ReconnectOtherCode();
                            }
                        }
                        break;
                }
            }
            else if (!CanDelete())
            {
	            var iUseOffset = currentClonedObject.GetComponent<IUseOffset>();
                switch (clonedObjectType)
	            {
                    case CodeType.Event:
                    case CodeType.Function:
	                    var boffset = currentClonedObject.gameObject.AddComponent<BlockOffset>();
			            boffset.Connection = null;
	                    iUseOffset.Offset = boffset;
	                    break;
                    case CodeType.Value:
	                    var offset = currentClonedObject.gameObject.AddComponent<ValueOffset>();
	                    offset.Connection = null;
	                    iUseOffset.Offset = offset;
	                    break;
                }

	            currentClonedObject.SetParent(otherCodeTransform);
	            currentClonedObject.localScale = Vector3.one;
                currentClonedObject = null;
            }

            // Don't delete, just place
            if (delete && false || CanDelete())
            {
	            Destroy(currentClonedObject.gameObject);
                currentClonedObject = null;
            }
        }

        struct CodeCon
        {
            public ICompilerCodeConnection con;
            public int index;

            public CodeCon(ICompilerCodeConnection con, int index)
            {
                this.con = con;
                this.index = index;
            }
        }

        struct ValueCon
        {
            public ICompilerValues con;
            public int index;
            public Transform lastCon;

            public ValueCon(ICompilerValues con, int index, Transform lastParent)
            {
                this.con = con;
                this.index = index;
                this.lastCon = lastParent;
            }
        }

        void DisconnectPreviousCode(bool reattachAfter = false)
        {
            switch (clonedObjectType)
            {
                case CodeType.Event:
                    var even = currentClonedObject.GetComponent<ICompilerEvent>();
                    var prevCon1 = even.Offset.Connection;

                    if (prevCon1 != null)
                    {
	                    var prevEvent = prevCon1.GetComponentInParent<ICompilerEvent>();

	                    var next = even.NextConnection;
	                    if (next != null)
	                    {
		                    next.Offset.Connection = prevEvent.NextConnectionObject;
	                    }

	                    prevEvent.NextConnection = next;

	                    previousConnection = prevEvent;
                    }
                    else
                    {
	                    previousConnection = null;
                    }

                    even.Offset.Connection = null;
                    break;

                case CodeType.Function:
                    var func = currentClonedObject.GetComponent<ICompilerCode>();
                    var prevCon = func.Offset.Connection;

                    if (prevCon != null)
                    {
	                    var prevCode = prevCon.GetComponentInParent<ICompilerCodeConnection>();
	                    var prevConIndex = prevCode.GetCodeConnection(func.Offset.Connection).Value.codeIndex;
	                    prevCode.CodeConnections[prevConIndex] = null;
	                    previousConnection = new CodeCon(prevCode, prevConIndex);


	                    if (reattachAfter)
	                    {
		                    var nextCode = func.CodeConnections[0];

		                    if (nextCode != null)
		                    {
			                    nextCode.Offset.Connection = prevCode.CodeConnectionObjects[prevConIndex];
			                    prevCode.CodeConnections[prevConIndex] = nextCode;
			                    func.CodeConnections[0] = null;
			                    // Attach uses code connection for dragged block
		                    }
	                    }
                    }
                    else
                    {
	                    previousConnection = null;
                    }

                    func.Offset.Connection = null;
                    break;

                case CodeType.Value:
                    var val = currentClonedObject.GetComponent<ICompilerValue>();
                    var prevValCon = val.Offset.Connection;
                    if (prevValCon != null)
                    {
	                    var prevVal = prevValCon.GetComponentInParent<ICompilerValues>();
	                    var prevValIndex = prevVal.GetValue(val.Offset.Connection).Value.valueIndex;
	                    prevVal.Values[prevValIndex] = null;
	                    previousConnection = new ValueCon(prevVal, prevValIndex, val.Offset.Connection);
                    }
                    else
                    {
	                    previousConnection = null;
                    }

                    val.Offset.Connection = null;
                    break;
            }
        }

        bool CanDelete()
        {
            // position.x might not be 0 because of safe area
            return Input.mousePosition.x < deletePosition.sizeDelta.x * deletePosition.lossyScale.x + deletePosition.position.x;
        }

        void CodeDelete(ICompilerCodeConnection code)
        {
	        if (code.CodeConnections == null) return;

            foreach (var c in code.CodeConnections)
            {
                if (c != null)
                {
                    CodeDelete(c);
                }
            }

            var v = code as ICompilerCode;
            if (v != null) 
                ValueDelete(v);
        }

        void ValueDelete(ICompilerValues value)
        {
            foreach (var v in value.Values)
            {
                if (v != null)
                {
                    ValueDelete(v);
                }
            }

            Destroy(value.Transform.gameObject);
        }

        void MoveCode(Transform hit, bool left = true)
        {
	        var canDropCode = previousConnection == null || 
		        (left ? ((byte) Settings.CodeDropType & 0b01) == 1 : ((byte) Settings.CodeDropType & 0b10) == 1);

	        if (CanDelete())
            {
                switch (clonedObjectType)
                {
                    case CodeType.Event:
                        CodeDelete(currentClonedObject.GetComponent<ICompilerEvent>());
                        Destroy(currentClonedObject.gameObject);
                        break;
                    case CodeType.Function:
                        CodeDelete(currentClonedObject.GetComponent<ICompilerCode>());
                        break;
                    case CodeType.Value:
                        ValueDelete(currentClonedObject.GetComponent<ICompilerValues>());
                        break;
                }

                currentClonedObject = null;

                //clickAudioSource.Play();
            }
            else
            {
	            switch (clonedObjectType)
                {
                    case CodeType.Event:
                        var curEvent = currentClonedObject.GetComponent<ICompilerEvent>();
                        if (hit && !hit.IsChildOf(otherCodeTransform))
                        {
                            var oldEvent = hit.GetComponentInParent<ICompilerEvent>();

                            ICompilerEvent nextCon = oldEvent.NextConnection;
                            oldEvent.NextConnection = curEvent;
                            curEvent.NextConnection = nextCon;

                            curEvent.Offset.Connection = oldEvent.NextConnectionObject;
                            if (curEvent.NextConnection != null)
                            {
                                nextCon.Offset.Connection = curEvent.NextConnectionObject;
                            }

                            currentClonedObject.SetParent(currentScriptObject, false);
                            currentClonedObject = null;
                            ReconnectAll();
                        }
                        else
                        {
	                        if (!canDropCode)
	                        {
		                        var prevConEvent = (ICompilerEvent) previousConnection;
		                        ICompilerEvent nextCon = prevConEvent.NextConnection;
		                        prevConEvent.NextConnection = curEvent;
		                        curEvent.NextConnection = nextCon;

		                        curEvent.Offset.Connection = prevConEvent.NextConnectionObject;
		                        if (curEvent.NextConnection != null)
		                        {
			                        nextCon.Offset.Connection = curEvent.NextConnectionObject;
		                        }

		                        currentClonedObject.SetParent(currentScriptObject, false);
		                        currentClonedObject = null;
		                        ReconnectAll();
	                        }
	                        else
	                        {
		                        currentClonedObject.SetParent(otherCodeTransform);
                                currentClonedObject = null;
		                        ReconnectOtherCode();
                            }
                        }

                        break;



                    case CodeType.Function:
                        var func = currentClonedObject.GetComponent<ICompilerCode>();

                        var lastCodeInMove = func;
                        var lastFunc = func.CodeConnections[0];

                        while (lastFunc != null)
                        {
                            lastCodeInMove = lastFunc;
                            lastFunc = lastFunc.CodeConnections[0];
                        }

                        if (hit)
                        {
                            var connection = hit.GetComponentInParent<ICompilerCodeConnection>();

                            if (connection != null && func != null)
                            {
                                var con = connection.GetCodeConnection(hit).Value;

                                ICompilerCode afterCode = connection.CodeConnections[con.codeIndex];
                                if (afterCode != null)
                                {
                                    lastCodeInMove.CodeConnections[0] = afterCode;
                                    afterCode.Offset.Connection = lastCodeInMove.CodeConnectionObjects[0];
                                }
                                connection.CodeConnections[con.codeIndex] = func;
                                func.Offset.Connection = connection.CodeConnectionObjects[con.codeIndex];

                                currentClonedObject.SetParent(currentScriptObject, false);
                                currentClonedObject = null;
                                ReconnectAll();
                            }
                        }
                        else
                        {
	                        if (!canDropCode)
	                        {
		                        var prevConCode = (CodeCon) previousConnection;
		                        ICompilerCode afterCode = prevConCode.con.CodeConnections[prevConCode.index];
		                        if (afterCode != null)
		                        {
			                        lastCodeInMove.CodeConnections[0] = afterCode;
			                        afterCode.Offset.Connection = lastCodeInMove.CodeConnectionObjects[0];
		                        }

		                        prevConCode.con.CodeConnections[prevConCode.index] = func;
		                        func.Offset.Connection = prevConCode.con.CodeConnectionObjects[prevConCode.index];

		                        currentClonedObject.SetParent(currentScriptObject, false);
		                        currentClonedObject = null;
		                        ReconnectAll();
	                        }
	                        else
	                        {
		                        currentClonedObject.SetParent(otherCodeTransform);
                                currentClonedObject = null;
                                ReconnectOtherCode();
                            }
                        }

                        break;



                    case CodeType.Value:
                        var val = currentClonedObject.GetComponent<ICompilerValue>();
                        var connectPrev = true;

                        if (hit)
                        {
                            var connection = hit.GetComponentInParent<ICompilerValues>();

                            if (connection != null && val != null)
                            {
                                var con = connection.GetValue(hit).Value;

                                ICompilerValue oldValue = connection.Values[con.valueIndex];
                                if (oldValue != null)
                                {
                                    connectPrev = true;
                                }
                                else
                                {
                                    connection.Values[con.valueIndex] = val;
                                    val.Offset.Connection = con.connectObject;

                                    currentClonedObject.SetParent(hit, false);
                                    currentClonedObject = null;
                                    ReconnectAll();

                                    connectPrev = false;
                                }
                            }
                        }

                        if (connectPrev)
                        {
	                        if (!canDropCode)
	                        {
		                        var prevCodeCon = (ValueCon) previousConnection;
		                        prevCodeCon.con.Values[prevCodeCon.index] = val;
		                        val.Offset.Connection = prevCodeCon.lastCon;

		                        currentClonedObject.SetParent(prevCodeCon.lastCon, false);
		                        currentClonedObject = null;
		                        ReconnectAll();
	                        }
	                        else
	                        {
                                currentClonedObject.SetParent(otherCodeTransform);
		                        currentClonedObject = null;
	                        }
                        }

                        ReconnectOtherCode();

                        break;
                }

	            //clickAudioSource.Play();
            }

	        //if (hit != null && hit.IsChildOf(otherCodeTransform))
				//ReconnectOtherCode();
        }

        void SetParent(ICompilerCodeConnection original, Transform parent)
        {
            (original as MonoBehaviour).transform.SetParent(parent, true);
            foreach (var c in original.CodeConnections)
            {
                if (c != null)
                {
                    SetParent(c, parent);
                }
            }
        }

        void SelectCodeBlock(Vector2 pos, bool reattachAfter = false)
        {
            if (currentClonedObject == null)
            {
                var obj = CodeRaycast(pos, prefabTags);
                var comp = obj == null ? null : obj.GetComponentInParent<HierarchyObject>();
                
                if (comp != null)
                {
                    currentClonedObject = Instantiate(comp.codePrefab, canvas.transform);
                    clonedObjectType = comp.codeType;
                    createdObject = true;
                }
                else
                {
                    var codeObj = CodeRaycast(pos, codeTags);
                    var codeComp = codeObj == null ? null : codeObj.GetComponentInParent<ICodeType>();

                    if (codeComp != null)
                    {
                        currentClonedObject = (codeComp as MonoBehaviour).transform;
                        currentClonedType = codeComp;
                        createdObject = false;
                        clonedObjectType = codeComp.CodeType;

                        //codeOldParent = currentClonedObject.parent;
                        var updateOtherCode = currentClonedObject.IsChildOf(otherCodeTransform);
                        if (updateOtherCode)
                        {
	                        currentClonedObject.SetParent(canvas.transform, false);

	                        DisconnectPreviousCode(reattachAfter);
	                        ReconnectOtherCode();
                        }
                        else
                        {
	                        currentClonedObject.SetParent(canvas.transform, false);

	                        DisconnectPreviousCode(reattachAfter);
	                        ReconnectAll();
                        }
                    }
                }

                if (currentClonedObject != null) 
	                currentClonedObject.localScale = CodeView.Instance.rectTransform.localScale;
            }
        }

        void ReleaseCodeBlock(Vector2 pos, bool left = true)
        {
            if (currentClonedObject != null)
            {
	            currentClonedObject.localScale = Vector3.one;

                if (createdObject)
                {
                    var hit = CodeRaycast(pos, GetTagFromType(clonedObjectType));
                    PlaceCode(hit);
                    //MoveCode(hit, left);
                }
                else
                {
                    var hit = CodeRaycast(pos, GetTagFromType(clonedObjectType));
                    MoveCode(hit, left);
                }
            }
        }

        private const float scrollThreshold = 100, scrollSpeed = 30;
        void UpdateSelected(Vector2 pos)
        {
            blockPrefabsScroll.enabled = true;
            codeViewScroll.enabled = true;
            if (currentClonedObject != null)
            {
                // Todo: Move canvas with mouse position
                // Input.mousePosition.x < deletePosition.sizeDelta.x * deletePosition.lossyScale.x + deletePosition.position.x
                // codeScrollContent

                var scale = canvas.transform.localScale.x;
                var yMoveDown = (Input.mousePosition.y - (Screen.height / 2f - Screen.safeArea.height / 2f)) / scale;
                var yMoveUp = ((Screen.height / 2f + Screen.safeArea.height / 2f) - Input.mousePosition.y) / scale;

                var xMoveLeft = (Input.mousePosition.x - (deletePosition.sizeDelta.x * deletePosition.lossyScale.x + deletePosition.position.x)) / scale + scrollThreshold;
                var xMoveRight = ((Screen.width / 2f + Screen.safeArea.width / 2f) - Input.mousePosition.x) / scale;

                if (yMoveDown < scrollThreshold)
	                codeScrollContent.anchoredPosition -= Vector2.down * (scrollThreshold - yMoveDown) * Time.deltaTime * scrollSpeed;

                if (yMoveUp < scrollThreshold)
                {
	                codeScrollContent.anchoredPosition -=
		                Vector2.up * (scrollThreshold - yMoveUp) * Time.deltaTime * scrollSpeed;

	                if (codeScrollContent.anchoredPosition.y < 0)
		                codeScrollContent.anchoredPosition = new Vector2(codeScrollContent.anchoredPosition.x, 0);
                }

                if (xMoveLeft < scrollThreshold)
                {
	                codeScrollContent.anchoredPosition -=
		                Vector2.left * (scrollThreshold - xMoveLeft) * Time.deltaTime * scrollSpeed;

	                if (codeScrollContent.anchoredPosition.x > 0)
		                codeScrollContent.anchoredPosition = new Vector2(0, codeScrollContent.anchoredPosition.y);
                }

                //Debug.Log(codeScrollContent.anchoredPosition);

                if (xMoveRight < scrollThreshold)
	                codeScrollContent.anchoredPosition -= Vector2.right * (scrollThreshold - xMoveRight) * Time.deltaTime * scrollSpeed;

                currentClonedObject.position = pos;

                if (!createdObject)
                {
                    switch (clonedObjectType)
                    {
                        case CodeType.Event:
                            RecursiveCodeConnect((currentClonedType as ICompilerEvent).CodeConnections?[0]);
                            break;
                        case CodeType.Function:
                            MoveCodeConnect(currentClonedType as ICompilerCode);
                            break;
                        case CodeType.Value:
                            RecursiveValueConnect(currentClonedType as ICompilerValue, false);
                            break;
                    }
                }

                blockPrefabsScroll.enabled = !createdObject;
                codeViewScroll.enabled = createdObject;
            }
        }

        // Update is called once per frame
        void Update()
        {
            foreach (var g in EditorObjects)
            {
                g.SetActive(UsingEditor);
            }

            foreach (var g in SceneObjects)
            {
                g.SetActive(!UsingEditor);
            }

            if (UsingEditor)
            {
                if (UsingKBM)
                {
	                if (Input.GetKeyDown(KeyCode.LeftAlt))
	                {
		                disableMovement.isOn = !disableMovement.isOn;
	                }

                    // Move with left click, shift to change value
#if UNITY_IOS 
					if (Input.GetMouseButtonDown(0) && !disableMovement.isOn) SelectCodeBlock(Input.mousePosition);
#else
	                if (Input.GetMouseButtonDown(0) && !disableMovement.isOn) SelectCodeBlock(Input.mousePosition);
#endif
	                if (Input.GetMouseButtonDown(1)) SelectCodeBlock(Input.mousePosition, true);

                    if (Input.GetMouseButtonDown(0) && disableMovement.isOn)
	                {
		                var obj = CodeRaycast(Input.mousePosition, prefabTags);
		                var comp = obj == null ? null : obj.GetComponentInParent<HierarchyObject>();
		                if (comp != null)
		                {
			                if (comp.Description.Length > 0)
			                {
                                CodePopup.Instance.MakePopup(comp.Description, Input.mousePosition);
			                }
		                }
                    }

                    if (Input.GetMouseButtonUp(0))
	                    ReleaseCodeBlock(Input.mousePosition);

                    if (Input.GetMouseButtonUp(1))
	                    ReleaseCodeBlock(Input.mousePosition, false);

                    UpdateSelected(Input.mousePosition);
                }
            }
        }

        void RecursiveValueConnect(ICompilerValue value, bool connectInitial)
        {
            if (connectInitial)
            {
                value.Offset.Connect();
            }
            else
            {
                value.Transform.SetAsLastSibling();
            }

            foreach (var v in value.Values)
            {
                if (v != null)
                {
                    RecursiveValueConnect(v, true);
                }
            }
        }

        void MoveCodeConnect(ICompilerCode code)
        {
            if (code != null)
            {
                foreach (var v in code.Values)
                {
                    if (v != null)
                    {
                        RecursiveValueConnect(v, true);
                    }
                }

                for (int i = 1; i < code.CodeConnections.Length; i++)
                {
                    RecursiveCodeConnect(code.CodeConnections[i]);
                }

                RecursiveCodeConnect(code.CodeConnections?[0]);
            }
        }

        void RecursiveCodeConnect(ICompilerCode code)
        {
            while (code != null)
            {
                code.Offset.Connect();

                foreach (var v in code.Values)
                {
                    if (v != null)
                    {
                        RecursiveValueConnect(v, true);
                    }
                }

                for (int i = 1; i < code.CodeConnections.Length; i++)
                {
                    RecursiveCodeConnect(code.CodeConnections[i]);
                }
                code = code.CodeConnections?[0];
            }
        }

        void CodeConnect(ICompilerCode code)
        {
            if (code != null)
            {
                code.UpdateSize(-1);
                code.Offset.Connect();

                foreach (var v in code.Values)
                {
                    if (v != null)
                    {
                        RecursiveValueConnect(v, true);
                    }
                }
            }
        }

        public void ReconnectAll()
        {
            var totalHeight = 900f;
            var nextEvent = beginEvent.NextConnection;
            while (nextEvent != null)
            {
                var nextCode = nextEvent.CodeConnections?[0];
                while (nextCode != null)
                {
                    nextCode.UpdateSize(0);
                    nextCode = nextCode.CodeConnections?[0];
                }

                nextEvent.UpdateSize(0);
                totalHeight += nextEvent.Height + 80;

                nextEvent = nextEvent.NextConnection;
            }
            codeScrollContent.sizeDelta = new Vector2(codeScrollContent.sizeDelta.x, totalHeight);

            nextEvent = beginEvent.NextConnection;
            while (nextEvent != null)
            {
                nextEvent.Offset.Connect();

                if (nextEvent.CodeConnectionObjects?[0] != null)
					RecursiveCodeConnect(nextEvent.CodeConnections[0]);

                nextEvent = nextEvent.NextConnection;
            }

            otherCodeTransform.SetAsLastSibling();

            ReconnectOtherCode();
        }

        List<Transform> otherCodeOffsets = new List<Transform>(16);
        public void ReconnectOtherCode()
        {
	        otherCodeOffsets.Clear();

	        foreach (Transform t in otherCodeTransform)
	        {
		        if (t.TryGetComponent<IUseOffset>(out var o))
		        {
			        t.localScale = Vector3.one;
			        if (o.Offset.Connection == null)
			        {
				        otherCodeOffsets.Add(t);
			        }
		        }
	        }

	        foreach (var t in otherCodeOffsets)
	        {
		        if (t.TryGetComponent<ICompilerCodeConnection>(out var c))
		        {
			        if (c is ICompilerEvent codeEvent)
			        {
				        var nextCode = codeEvent.CodeConnections?[0];
				        while (nextCode != null)
				        {
					        nextCode.UpdateSize(0);
					        nextCode = nextCode.CodeConnections?[0];
				        }

				        codeEvent.UpdateSize(0);

                        if (codeEvent.CodeConnections?[0]  != null)
                            RecursiveCodeConnect(codeEvent.CodeConnections?[0]);
                    }
                    else if (c is ICompilerCode codeCode)
			        {
				        var nextCode = codeCode.CodeConnections?[0];
				        while (nextCode != null)
				        {
					        nextCode.UpdateSize(0);
					        nextCode = nextCode.CodeConnections?[0];
				        }

				        codeCode.UpdateSize(0);

					    MoveCodeConnect(codeCode);
                    }
                }
                else if (t.TryGetComponent<ICompilerValue>(out var v))
		        {
                    v.UpdateSize(0);
                    RecursiveValueConnect(v, false);
                }
	        }
        }

        public void OpenScript(SelectableObject obj)
        {
            if (currentScriptObject)
            {
                CloseEditor();
            }
            UsingEditor = true;

            if (obj.Script != null)
            {
                currentScriptObject = obj.Script.transform;
                beginEvent = obj.Script.beginEvent;

                currentScriptObject.SetParent(codeScrollContent, false);
                currentScriptObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(8, -8);
                currentScriptObject.SetAsFirstSibling();
            }
            else
            {
                var newScript = Instantiate(scriptPrefab, codeScrollContent);
                var r = newScript.GetComponent<RectTransform>();
                r.anchoredPosition = new Vector2(8, -8);
                r.SetAsFirstSibling();

                var script = newScript.GetComponent<SavedScript>();
                script.obj = obj;
                obj.Script = script;

                currentScriptObject = newScript;
                beginEvent = script.beginEvent;
            }

            codeScrollContent.anchoredPosition = Vector2.zero;

            otherCodeTransform = (RectTransform)obj.Script.transform.Find("OtherCode");

            // Todo: Check if size needs to be updated first
            ReconnectAll();
        }

        public async void CloseEditor()
        {
            // Leave scene if it's a tutorial (and it's loaded)
	        //if (SceneData.IsTutorial && SceneData.LoadedScene)
	        //{
		    //    SceneManager.LoadScene("Menu");
		    //    return;
	        //}

	        UsingEditor = !UsingEditor;

	        if (currentClonedObject != null)
	        {
		        ReleaseCodeBlock(Input.mousePosition);
		        await Task.Delay(0);
	        }

            currentScriptObject.SetParent(scripts, false);
            currentScriptObject = null;
            beginEvent = null;

        }
    }
}