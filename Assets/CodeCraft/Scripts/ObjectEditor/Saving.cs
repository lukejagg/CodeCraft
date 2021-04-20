using Interpreter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using System.Text;
using CodeEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace ObjectEditor
{
    public enum ObjectType
    {
        Mesh = 0,
        Light = 1,
        Camera = 2,
    }

    public class Saving : MonoBehaviour
    {
#if UNITY_WEBGL
	    public static string SavePath => "Saves/";
	    public static string AutoSavePath => "AutoSaves/";
	    public static string DeletePath => "Deleted/";
#else
        public static string SavePath => Application.persistentDataPath + "/Saves/";
        public static string AutoSavePath => Application.persistentDataPath + "/AutoSaves/";
        public static string DeletePath => Application.persistentDataPath + "/Deleted/";
#endif

        // https://en.wikipedia.org/wiki/Private_Use_Areas
        public const char PropertySplit = '\uFFFF';
        public const char TransformObjectSplit = '\uF8FD';
        public const char ObjectCodeSplit = '\uF8FF';
        public const char CodeSplit = '\uF8FE';
        public const char CodeValueBegin = '\uF8FD'; // Change to unicode
        public const char CodeValueEnd = '\uF8FC'; // Change to unicode
        public const char CodeScopeBegin = '\uF8FB'; // beginning of scope
        public const char CodeScopeEnd = '\uF8FA'; // end of scope
        public const char CodeValueQuote = '\uF8EF'; // Used for separating code type from value e.g. N:&69
        public const char VariableSplit = '\uF8EE'; // For the variable event
        public const char FunctionSplit = '\uF8ED'; // For functions saving

        public Editor Editor;
        public CodeView codeView;

        public Transform[] ObjectIdPrefabs;
        public Transform[] EventPrefabArray;
        public Transform[] FunctionPrefabArray;
        public Transform[] ValuePrefabArray;
        Dictionary<string, Transform> CodeIdPrefabs;

        public Transform objects;
        public Transform scripts;

        public Text saveText;

        private const string defaultSave =
            "H4sIAAAAAAAACzN4v3+/c2lSKpAy0NE11DHTMwUzQdAQyDLUMQTT7xfvNQBJ6JmbmpgbmutgMgzxSoON2M/LZQSyLzE3tSgRyDDS0QXJ65joGeG01cxAzwAIwJpBQi6ZRanJJZn5eYk5Cj6Z6RklYI1GeiaWIAByP0g51DEG5oYGZhZgY+E8VOORbYNoArnEEORXM7ClAN5vNkQhAQAA";
        //"H4sIAAAAAAAACzN4v3+/c2lSKpAy0AFCGK1jCGQZ6hiC6feL94Il9MxNTcwNzXWwMJD0v1+8n5cLxAhPTUmHGaxriMtkQxJNNgK5ODE3tSgRIgNUqKNrBiRxWWBmoGcABGDNICGXzKLU5JLM/LzEHAWfzPSMErBGEx2oFw3MDQ3MLMAGwXmYLoZxIa4H2W0I0m+GsCa4IL8Ebr6ukZ4p0AoQBdZgYmxsYGgE8iaMpWugZ2QK9g1mwBO2LCA/M68EyTdGxPvGiJAFALEorL0jAgAA";

        HashSet<int> usedIndices = new HashSet<int>();

        // .ccs = Code Craft Save
        public static string GetAutoSavePath(string name) => AutoSavePath + name + ".ccs";
        public static string GetSavePath(string name) => SavePath + name + ".ccs";

        void OnApplicationPause(bool paused)
        {
            AutoSave();
        }

        void OnApplicationQuit()
        {
	        AutoSave();
        }

        public static string Export(string name)
        {
	        var path = GetSavePath(name);
	        var data = Saving.ReadSave(path); //File.ReadAllText(path));

	        return $"Name:{name}/{data}\nhttp://codecraft3d.com/play/";
        }

        public static string FixName(string str)
        {
	        if (str.Length == 0) return "Unnamed";

	        str = str.Replace("\\", "").Replace("/", "");
	        return str.Substring(0, str.Length > 20 ? 20 : str.Length);
        }

        public void AutoSave()
        {
	        if (SceneData.IsTutorial) return; // Dont autosave if tutorial.

	        if (SceneData.Name == null || SceneData.Name.Length == 0) SceneData.Name = FindUnusedName("Unnamed");

	        var save = GetSaveString();

            var savePath = GetSavePath(SceneData.Name);
	        if (!SaveExists(savePath)) // If a save doesn't exist, create one
	        {
		        //File.WriteAllText(savePath, save);
		        Saving.WriteSave(savePath, save);
                Debug.Log("No save found, wrote save");
	        }

	        if (SceneData.AllowAutosave)
	        {
		        var autoSavePath = GetAutoSavePath(SceneData.Name);
		        //File.WriteAllText(autoSavePath, save);
                Saving.WriteSave(autoSavePath, save);
		        Debug.Log($"Autosaved {SceneData.Name}");
	        }
        }

        public static void WriteSave(string path, string save)
        {
#if UNITY_WEBGL
            PlayerPrefs.SetString(path, save);

            var folderIndex = path.IndexOf("/");
            var folder = path.Substring(0, folderIndex + 1);
            var name = path.Substring(folderIndex + 1, path.Length - folderIndex - 1);

            AddSaveName(folder, name);
#else
			File.WriteAllText(path, save);
#endif
        }

        public static bool SaveExists(string path)
        {
#if UNITY_WEBGL
	        return PlayerPrefs.HasKey(path);
#else
            return File.Exists(path);
#endif
        }

        public static string ReadSave(string path)
        {
#if UNITY_WEBGL
	        return PlayerPrefs.GetString(path);
#else
			return File.ReadAllText(path);
#endif
        }

        public static string GetDeletedName(string str)
        {
	        int index = 0;
	        var dashIndex = str.LastIndexOf('-');
	        if (dashIndex != -1)
	        {
		        var num = str.Substring(dashIndex);
		        if (int.TryParse(num, out var res))
		        {
			        // The file is named {Name}-(number) (eg. File-2)
			        str = str.Substring(0, dashIndex);
			        index = res;
		        }
	        }

	        var name = "";
	        while (true)
	        {
		        name = index > 0 ? $"{str.Substring(0, str.Length > 22 ? 22 : str.Length)}-{index}" : str;
		        if (!SaveExists(Saving.DeletePath + name + ".ccs")) break;

		        index++;
	        }

	        return name;
        }

        public static string FindUnusedName(string str)
        {
	        int index = 0;
            var dashIndex = str.LastIndexOf('-');
	        if (dashIndex != -1)
	        {
		        var num = str.Substring(dashIndex);
		        if (int.TryParse(num, out var res))
		        {
                    // The file is named {Name}-(number) (eg. File-2)
                    str = str.Substring(0, dashIndex);
                    index = res;
		        }
	        }

	        var name = "";
	        while (true)
	        {
		        name = index > 0 ? $"{str.Substring(0, str.Length > 22 ? 22 : str.Length)}-{index}" : str;
		        if (!SaveExists(Saving.GetSavePath(name))) break;

                index++;
	        }

	        return name;
        }

        public void Save()
        {
	        if (SceneData.Name == null || SceneData.Name.Length == 0) SceneData.Name = FindUnusedName(SceneData.IsTutorial ? "Tutorial" : "Unnamed");

            var save = GetSaveString();
	        var savePath = Saving.GetSavePath(SceneData.Name);
		    //File.WriteAllText(savePath, save);
            Saving.WriteSave(savePath, save);

		    SceneData.AllowAutosave = true;
		    Debug.Log($"Saved {SceneData.Name}");

		    StopAllCoroutines();
            StartCoroutine(ChangedSaveText());
        }

        IEnumerator ChangedSaveText()
        {
	        saveText.text = "Saved";
	        saveText.color = Color.green;
            yield return new WaitForSeconds(0.6f);
            saveText.text = "Save";
            saveText.color = Color.white;
        }

        public void ToMenu()
        {
            AutoSave();
            SceneManager.LoadScene("Menu");
        }

        public static string SaveFloat(float f)
        {
            return f.ToString();
            //return f.ToString("0.000f");
        }

        public static string SaveString(string str)
        {
            // Get rid of special saving characters
            return str.Replace(PropertySplit.ToString(), "").Replace(TransformObjectSplit.ToString(), "").Replace(ObjectCodeSplit.ToString(), "").Replace(CodeSplit.ToString(), "");
        }

        public static string SaveTransform(Transform t)
        {
	        var obj = t.GetComponent<IObject>();
            string str = ((int)obj.Id).ToString() + PropertySplit;
            str += SaveString(t.name) + PropertySplit;
            str += $"{SaveFloat(t.localPosition.x)},{SaveFloat(t.localPosition.y)},{SaveFloat(t.localPosition.z)}" + PropertySplit;
            str += $"{SaveFloat(t.localRotation.x)},{SaveFloat(t.localRotation.y)},{SaveFloat(t.localRotation.z)},{SaveFloat(t.localRotation.w)}" + PropertySplit;
            str += $"{SaveFloat(t.localScale.x)},{SaveFloat(t.localScale.y)},{SaveFloat(t.localScale.z)}" + PropertySplit;
            str += $"{obj.Index}" + PropertySplit;
            return str;
        }

        public IObject LoadTransform(string str)
        {
            var props = str.Split(PropertySplit);

            var obj = Instantiate(ObjectIdPrefabs[int.Parse(props[0])], objects);
            obj.name = props[1];

            var pos = props[2].Split(',');
            obj.localPosition = new Vector3(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2]));

            var rot = props[3].Split(',');
            obj.localRotation = new Quaternion(float.Parse(rot[0]), float.Parse(rot[1]), float.Parse(rot[2]), float.Parse(rot[3]));

            var scale = props[4].Split(',');
            obj.localScale = new Vector3(float.Parse(scale[0]), float.Parse(scale[1]), float.Parse(scale[2]));

            // Transform stuff ^^

            var iobj = obj.GetComponent<IObject>();

            // Index is new part of save: if index is not included, then don't set index
            if (props.Length >= 6 && props[5].Length > 0)
            {
	            var index = props[5];
	            iobj.Index = int.Parse(index);
            }

            return iobj;
        }

        // Start is called before the first frame update
        void Start()
        {
	        SceneData.LoadedScene = false;
            if (SceneData.Data != null)
				Load(SceneData.Data);

            SceneData.LoadedScene = true;
        }

        private static bool initialized = false;
        public static void Initialize()
        {
	        if (!initialized)
	        {
		        initialized = true;

#if UNITY_WEBGL
		        SaveFolders.Add(SavePath, new HashSet<string>(PlayerPrefsX.GetStringArray(SavePath, "", 0)));

		        SaveFolders.Add(AutoSavePath, new HashSet<string>(PlayerPrefsX.GetStringArray(AutoSavePath, "", 0)));

		        SaveFolders.Add(DeletePath, new HashSet<string>(PlayerPrefsX.GetStringArray(DeletePath, "", 0)));
#else
                CreateDirectory(Saving.SavePath);
		        CreateDirectory(Saving.AutoSavePath);
		        CreateDirectory(Saving.DeletePath);
#endif
            }
        }

        static void CreateDirectory(string path)
        {
	        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        }

#if UNITY_WEBGL
        public static Dictionary<string, HashSet<string>> SaveFolders = new Dictionary<string, HashSet<string>>(4);
#endif

	    public static void AddSaveName(string folder, string name)
	    {
#if UNITY_WEBGL
		    SaveFolders[folder].Add(name);
		    var saves = SaveFolders[folder].ToArray();
            Array.Sort(saves);
            PlayerPrefsX.SetStringArray(folder, saves);
#endif
        }

	    public static void RemoveSaveName(string folder, string name)
	    {
		    name += ".ccs";
#if UNITY_WEBGL
		    SaveFolders[folder].Remove(name);
		    var saves = SaveFolders[folder].ToArray();
		    Array.Sort(saves);
		    PlayerPrefsX.SetStringArray(folder, saves);
#endif
	    }

        private void Awake()
        {
	        CodeView.Instance = codeView;

            CodeIdPrefabs = new Dictionary<string, Transform>();

            foreach (var t in EventPrefabArray)
	            CodeIdPrefabs.Add(t.GetComponent<ICodeType>().SaveId, t);
            foreach (var t in FunctionPrefabArray)
	            CodeIdPrefabs.Add(t.GetComponent<ICodeType>().SaveId, t);
            foreach (var t in ValuePrefabArray)
	            CodeIdPrefabs.Add(t.GetComponent<ICodeType>().SaveId, t);
        }

        string GetSaveString()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var save = new StringBuilder();

            foreach (Transform obj in objects)
            {
                if (obj.TryGetComponent<IObject>(out var comp)) {
                    // + ObjectCodeSplit + code.Serialize()
                    var code = obj.GetComponent<SelectableObject>().Script;
                    var scriptSave = "";
                    if (code != null)
                    {
                        scriptSave = code.Save();
                        //print(scriptSave);
                    }
                    save.AppendLine(SaveTransform(obj) + TransformObjectSplit + comp.Serialize() + ObjectCodeSplit + scriptSave);
                }
            }

            var str = save.ToString().TrimEnd('\n');
            //print(str);

            // COMPRESS
            byte[] inputBytes = Encoding.UTF8.GetBytes(str);
            var outputStream = new MemoryStream();
            using (var gZipStream = new GZipStream(outputStream, CompressionMode.Compress))
                gZipStream.Write(inputBytes, 0, inputBytes.Length);

            var outputBytes = outputStream.ToArray();
            outputStream.Close();

            var outputbase64 = Convert.ToBase64String(outputBytes);

#if UNITY_EDITOR
            print(outputbase64);
            print($"{outputbase64.Length}, {outputbase64.Length * 1f / str.Length}");
#endif

            Debug.LogWarning($"Saved! Took {stopwatch.Elapsed}");

            return outputbase64;
        }

        void Load(string save)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            byte[] inputBytes = Convert.FromBase64String(save);

            using (var inputStream = new MemoryStream(inputBytes))
            using (var gZipStream = new GZipStream(inputStream, CompressionMode.Decompress))
            using (var streamReader = new StreamReader(gZipStream))
            {
                save = streamReader.ReadToEnd();
            }

            foreach (Transform t in this.objects)
            {
                Destroy(t.gameObject);
            }

            foreach (Transform t in scripts)
            {
                Destroy(t.gameObject);
            }

            var objects = save.Split('\n');
#if UNITY_EDITOR
            Debug.Log(save);
#endif
            Debug.Log($"Loading {objects.Length} objects");
            if (save.Length == 0) return;

            foreach(var obj in objects)
            {
                var objectCode = obj.Split(ObjectCodeSplit);

                var transformComp = objectCode[0].Split(TransformObjectSplit);
                var t = transformComp[0]; // Transform save
                var comp = transformComp[1]; // Component save

                var created = LoadTransform(t);
                created.Deserialize(comp.Split(PropertySplit));
                created.UpdateProperties();

                var code = objectCode.Length > 1 ? objectCode[1] : "";
                LoadCode((created as MonoBehaviour).GetComponent<SelectableObject>(), new SubstringOptimizedString(code));
            }

            Debug.LogWarning($"Loaded! Took {stopwatch.Elapsed} to load {objects.Length} objects.");

            if (SceneData.IsTutorial)
            {
	            Editor.OpenScript(this.objects.GetChild(0).GetComponent<SelectableObject>());
            }

            // Check for clones of indexes of object
            foreach (Transform c in transform)
            {
	            var obj = c.GetComponent<IObject>();
	            AddObjectIndex(obj);
            }
        }

        private int addIndex = 1;
        public void AddObjectIndex(IObject obj)
        {
	        if (obj.Index <= 0) obj.Index = addIndex;

	        while (usedIndices.Contains(obj.Index))
	        {
		        obj.Index++;
	        }

	        usedIndices.Add(obj.Index);

	        if (obj.Index >= addIndex)
	        {
		        addIndex = obj.Index + 1;
	        }
        }

        // Load Events
        public void LoadCode(SelectableObject obj, SubstringOptimizedString code)
        {
	        var eventScope = code.IndexOf(CodeScopeBegin);
	        ICompilerEvent lastEventObject = null;

            // CREATE CODE IF THERE IS CODE
	        if (eventScope != -1)
	        {
		        //print(code);

                Editor.OpenScript(obj);
                Editor.CloseEditor();

                lastEventObject = obj.Script.beginEvent;
	        }

	        while (eventScope != -1)
	        {
		        // Find where event {} ends
		        var scopeAmount = 1;
		        var eventScopeEnd = -1;
		        for (int i = eventScope + 1; i < code.Length; i++)
		        {
			        if (code[i] == CodeScopeBegin)
				        scopeAmount++;
			        else if (code[i] == CodeScopeEnd)
			        {
				        scopeAmount--;

				        if (scopeAmount == 0)
				        {
					        eventScopeEnd = i;
					        break;
				        }
			        }
		        }

		        // Check if code contains equal amount of {}
		        if (eventScopeEnd == -1)
		        {
			        Debug.LogWarning($"{obj} code was corrupted.");
			        return;
		        }

		        // Create Event
		        var eventName = code.Substring(0, eventScope);

                // Is it a Variable Event
		        var varSplit = eventName.IndexOf(VariableSplit);
		        if (varSplit != -1)
		        {
			        eventName = eventName.Substring(0, varSplit);
		        }

		        var eventObject = Instantiate(CodeIdPrefabs[eventName.ToString()], obj.Script.transform);
                var currentEvent = eventObject.GetComponent<ICompilerEvent>();

                // Event has custom saving
                if (varSplit != -1)
                {
	                var str = eventName.ToString();
	                if (str == "VarEvent")
	                {
		                B_VariableEvent variable = currentEvent as B_VariableEvent;
		                variable.variableName.text = code.Substring(varSplit + 1, eventScope - varSplit - 1).ToString();
	                }
                    else if (str == "Receive")
	                {
		                B_Received variable = currentEvent as B_Received;
		                variable.broadcastName.text = code.Substring(varSplit + 1, eventScope - varSplit - 1).ToString();
                    }
                }

                // Event Offset
		        var offset = eventObject.gameObject.AddComponent<BlockOffset>();
                currentEvent.Offset = offset;
                offset.Connection = (lastEventObject as MonoBehaviour).transform.Find("EventConnection");
                offset.Connect();
                //offset.Connect();

                // Connect event to previous event
                lastEventObject.NextConnection = currentEvent;




                // FUNCTIONS INSIDE OF CODE
                var subcode = code.Substring(eventScope + 1, eventScopeEnd - eventScope - 1);
                LoadFunctions(subcode, currentEvent, obj);



                // Update Size
		        //currentEvent.UpdateSize(0);

                // Next Loop:
                code = code.Substring(eventScopeEnd + 1);
                eventScope = code.IndexOf(CodeScopeBegin);
                 
                lastEventObject = currentEvent;
	        }
        }

        void LoadFunctions(SubstringOptimizedString subcode, ICompilerEvent currentEvent, SelectableObject obj, ICompilerCode currentCode = null, int codeId = 0)
        {
	        var functionScope = subcode.IndexOf(CodeValueBegin);
	        ICompilerCode lastFunctionObject = null;

	        while (functionScope != -1)
	        {
		        var valueScopeAmount = 1;
		        var valueEndIndex = -1;

		        for (int i = functionScope + 1; i < subcode.Length; i++)
		        {
			        if (subcode[i] == CodeValueBegin)
				        valueScopeAmount++;
			        else if (subcode[i] == CodeValueEnd)
			        {
				        valueScopeAmount--;

				        if (valueScopeAmount == 0)
				        {
					        if (subcode.Length > i + 1 && subcode[i + 1] == CodeValueBegin)
					        {
                                // There is still another ()
                                // Eg: (Val1)(Val2)
					        }
					        else // if it is the end
					        {
						        valueEndIndex = i;
						        break;
					        }
				        }
			        }
		        }

		        // Check if code contains equal amount of {}
		        if (valueEndIndex == -1)
		        {
			        Debug.LogWarning($"{currentEvent as MonoBehaviour} code was corrupted.");
			        return;
		        }

		        // Create Function
		        var functionName = subcode.Substring(0, functionScope).ToString();
		        //print(functionName);

                // Broadcast function is only (currently) function with no values, but InputField
                var functionValueSplit = functionName.IndexOf(FunctionSplit);
                string functionValue = "";
                if (functionValueSplit != -1)
                {
	                functionValue = functionName.Substring(functionValueSplit + 1);
	                functionName = functionName.Substring(0, functionValueSplit);
                }

#if UNITY_EDITOR
                if (!CodeIdPrefabs.ContainsKey(functionName)) Debug.LogError($"Doesn't Contain {functionName}");
#endif

                var functionObject = Instantiate(CodeIdPrefabs[functionName], obj.Script.transform);
		        var currentFunction = functionObject.GetComponent<ICompilerCode>();

		        if (functionValueSplit != -1)
		        {
			        if (currentFunction is B_Broadcast bb)
			        {
                        bb.broadcastName.text = functionValue;
                    }
                    else if (currentFunction is B_Comment cc)
			        {
				        cc.inputField.text = functionValue;
			        }
		        }

		        // Event Offset
		        var offset = functionObject.gameObject.AddComponent<BlockOffset>();
		        currentFunction.Offset = offset;

		        if (lastFunctionObject != null)
		        {
			        lastFunctionObject.CodeConnections[0] = currentFunction;
                    offset.Connection = (lastFunctionObject as MonoBehaviour).transform.Find("CodeConnection");
		        }
		        else
		        {
			        if (currentEvent != null)
			        {
				        currentEvent.CodeConnections[0] = currentFunction;
				        offset.Connection = (currentEvent as MonoBehaviour).transform.Find("Scope")
					        .Find("CodeConnection");
			        }
			        else
			        {
				        currentCode.CodeConnections[codeId] = currentFunction;
				        if (codeId == 1)
				        {
					        offset.Connection = (currentCode as MonoBehaviour).transform.Find("Scope")
						        .Find("CodeConnection");
				        }
				        else
				        {
					        offset.Connection = (currentCode as MonoBehaviour).transform.Find($"Scope{codeId}")
						        .Find("CodeConnection");
                        }
			        }
		        }

		        offset.Connect();

                // Todo: Implement () value creation (LoadValues)
	            var valueString = subcode.Substring(functionScope /*+ 1*/, valueEndIndex - functionScope + 1/*- 1*/);
	            if (valueString.Length > 0)
	            {
		            LoadValueScopes(valueString, currentFunction);
	            }

	            // Todo: Implement {}{} function searches (if, if else) (recursion)
                // Function Scopes
                var innerScope = LoadFunctionScopes(subcode, currentFunction, obj, valueEndIndex);
                // Connect innerScope.Variable1? to mainfunction

                // Next Loop:
                if (innerScope != -1)
                {
	                subcode = subcode.Substring(innerScope + 2);
                }
                else
                {
	                subcode = subcode.Substring(valueEndIndex + 1);
                }

                // Next Loop:
                functionScope = subcode.IndexOf(CodeValueBegin);

		        lastFunctionObject = currentFunction;
            }
        }

        // Subcode and string index end
        int LoadFunctionScopes(SubstringOptimizedString subcode, ICompilerCode currentFunction, SelectableObject obj, int valueEndIndex)
        {
	        //print(subcode);

            // CodeConnections: scopeIndex + 1
	        int scopeIndex = 0;
	        var totalEndIndex = -1;

	        var scopeBeginIndex = subcode.Length > valueEndIndex + 1 ? (subcode[valueEndIndex + 1] == CodeScopeBegin
		        ? valueEndIndex + 1
		        : -1) : -1;

            while (scopeBeginIndex != -1)
	        {
		        if (scopeIndex != 0)
		        {
			        scopeBeginIndex = subcode.Length > 0 ? (subcode[0] == CodeScopeBegin
				        ? 0
				        : -1) : -1;
                }

		        if (scopeBeginIndex != -1)
		        {
			        var scopeEndIndex = -1;
			        var scopeAmount = 1;

			        for (int i = scopeBeginIndex + 1; i < subcode.Length; i++)
			        {
				        if (subcode[i] == CodeScopeBegin)
					        scopeAmount++;
				        else if (subcode[i] == CodeScopeEnd)
				        {
					        scopeAmount--;

					        if (scopeAmount == 0)
					        {
						        scopeEndIndex = i;
						        break;
					        }
				        }
			        }

			        var scopeCode = subcode.Substring(scopeBeginIndex + 1, scopeEndIndex - scopeBeginIndex - 1);
                    //print(scopeCode);

                    LoadFunctions(scopeCode, null, obj, currentFunction, scopeIndex + 1);

                    totalEndIndex += scopeEndIndex;

			        subcode = subcode.Substring(scopeEndIndex + 1);

			        scopeIndex++;
                }
		        else
		        {
			        break;
		        }
	        }

            if (totalEndIndex != -1)
            {
	            return totalEndIndex + scopeIndex - 1;
            }

            return -1;
        }

        void LoadValues(SubstringOptimizedString subcode, ICompilerValues currentCode, int value = 0)
        {
	        var valueIndex = -1;
	        var scopeIndex = subcode.Length;
	        // Type:&"Text"(sub values)
            // This reads until (
	        for (int i = 0; i < subcode.Length; i++)
	        {
		        if (subcode[i] == CodeValueQuote)
		        {
			        valueIndex = i;
		        }
                else if (subcode[i] == CodeValueBegin)
		        {
			        scopeIndex = i;
			        break;
		        }
	        }

	        var valueName = valueIndex == -1 ? subcode.Substring(0, scopeIndex) : subcode.Substring(0, valueIndex);

            var valueParent = currentCode.Transform.Find("Block")
	            .Find($"Value{(value == 0 ? "" : (value + 1).ToString())}");
            var valueObject = Instantiate(CodeIdPrefabs[valueName.ToString()], valueParent);
            var currentValue = valueObject.GetComponent<ICompilerValue>();
            currentCode.Values[value] = currentValue;

            var loadedValue = valueIndex == -1 ? "" : subcode.Substring(valueIndex + 1, scopeIndex - valueIndex - 1).ToString();
            currentValue.LoadValue(loadedValue);

            // Event Offset
            var offset = valueObject.gameObject.AddComponent<ValueOffset>();
            (currentValue as IUseOffset).Offset = offset;
            offset.Connection = valueParent;

            if (scopeIndex < subcode.Length)
            {
	            var newCode = subcode.Substring(scopeIndex);
                LoadValueScopes(newCode, currentValue);
            }
        }

        void LoadValueScopes(SubstringOptimizedString subcode, ICompilerValues currentCode)
        {
	        int currentSubValue = 0;

	        var scopeBeginIndex = 0;

	        while (scopeBeginIndex != -1)
	        {
		        if (currentSubValue != 0)
		        {
			        scopeBeginIndex = subcode.Length > 0 && subcode[0] == CodeValueBegin ? 0 : -1;
		        }

		        if (scopeBeginIndex != -1)
		        {
			        var scopeEndIndex = -1;
			        var scopeAmount = 1;

			        for (int i = scopeBeginIndex + 1; i < subcode.Length; i++)
			        {
				        if (subcode[i] == CodeValueBegin)
					        scopeAmount++;
				        else if (subcode[i] == CodeValueEnd)
				        {
					        scopeAmount--;

					        if (scopeAmount == 0)
					        {
						        scopeEndIndex = i;
						        break;
					        }
				        }
			        }

			        var scopeCode = subcode.Substring(scopeBeginIndex + 1, scopeEndIndex - scopeBeginIndex - 1);
			        //print(scopeCode);

			        if (scopeCode.Length > 0)
			        {
				        LoadValues(scopeCode, currentCode, currentSubValue);
			        }

			        subcode = subcode.Substring(scopeEndIndex + 1);

			        currentSubValue++;
		        }
		        else
		        {
			        break;
		        }
	        }
        }

        /*
        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.P)) 
                Load(GetSaveString());
        }
        */
    }
}