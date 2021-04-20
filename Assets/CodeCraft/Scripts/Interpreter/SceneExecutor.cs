using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Programmer;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using CodeEditor;
using ObjectEditor;

namespace Interpreter
{
	public class ObjectCollision
	{
		public CodeExecutor Object;
		public CodeExecutor Hit;
        // Normal, Touch position, etc

        public ObjectCollision(CodeExecutor o, CodeExecutor hit)
        {
	        Object = o;
	        Hit = hit;
        }
	}

	public class BroadcastProgram
	{
		public CodeExecutor codeExec;
		public Program[] program;

		public BroadcastProgram(CodeExecutor codeExec, Program[] program)
		{
			this.codeExec = codeExec;
			this.program = program;
		}
	}

    public class SceneExecutor : MonoBehaviour
    {
	    public static SceneExecutor Instance;

        // Hierarchy
        public Transform scene;
        public Transform objects;

        // Objects
        public GameObject editorCamera;
        public ObjectEdit objEditor;
        public GameObject[] editorObjects;
        public GameObject[] sceneObjects;
        public Saving saveObject;
        public PlayInputs playInputs;

        // Gameplay
        bool running = false;
        public bool Running => running;

        // Start / Update
        public List<CodeExecutor>
	        StartCodeExecutors = new List<CodeExecutor>(),
	        UpdateCodeExecutors = new List<CodeExecutor>(),
	        FixedUpdateCodeExecutors = new List<CodeExecutor>();

        // Broadcast
        public Dictionary<string, List<BroadcastProgram>> BroadcastEvents = new Dictionary<string, List<BroadcastProgram>>();

        public HashSet<CodeExecutor> DeletedObjects = new HashSet<CodeExecutor>();

        // Physics is done on CodeExecutors now
        //public List<ObjectCollision> CollisionStart = new List<ObjectCollision>();
        //public List<ObjectCollision> CollisionEnd = new List<ObjectCollision>();
        //public CodeExecutor collisionHit;

        public void Awake()
        {
	        Instance = this;
        }

        public void Broadcast(string name)
        {
	        if (BroadcastEvents.TryGetValue(name, out var events))
	        {
		        for (int i = 0; i < events.Count; i++)
		        {
			        var l = events[i].program.Length;
			        for (int j = 0; j < l; j++)
			        {
				        events[i].program[j].Execute(events[i].codeExec);
			        }
		        }
	        }
        }

        public void AddBroadcast(string name, BroadcastProgram program)
        {
	        List<BroadcastProgram> list;
	        if (BroadcastEvents.TryGetValue(name, out list))
	        {
		        list.Add(program);
	        }
	        else
	        {
		        list = new List<BroadcastProgram>();
		        list.Add(program);
		        BroadcastEvents.Add(name, list);
            }
            
        }

        public CodeExecutor CreateObject(SelectableObject selectable, bool addStart, IObject obj)
        {
	        //if (obj == null)
		    //    obj = selectable.GetComponent<IObject>();

	        var t = selectable.transform;

	        var clone = obj.ConvertToObject();
	        clone.parent = scene;
	        clone.position = t.position;
	        clone.rotation = t.rotation;
	        clone.localScale = t.localScale;
	        clone.name = selectable.name;

	        var exec = clone.gameObject.AddComponent<CodeExecutor>();
	        exec.original = selectable;
	        //exec.isClone = originalObject != null;
	        exec.isClone = false;

	        if (obj is MeshObject mesh)
	        {
		        exec.color = new Vector3(mesh.color.r, mesh.color.g, mesh.color.b);
		        exec.transparency = 1 - mesh.color.a;
                // Todo: change in Clone script too
	        }
            else if (obj is LightObject light)
	        {
		        exec.color = new Vector3(light.color.r, light.color.g, light.color.b);
	        }

	        // If it has code (and not just empty created script)
            if (selectable.Script != null && selectable.Script.beginEvent.NextConnection != null)
	        {
		        var events = exec.CreateCode(selectable.Script.beginEvent);

		        if (addStart && events.Start)
			        StartCodeExecutors.Add(exec);

		        if (events.Update)
			        UpdateCodeExecutors.Add(exec);

		        if (events.FixedUpdate)
			        FixedUpdateCodeExecutors.Add(exec);
	        }

            exec.RefreshColor();

            return exec;
        }

        public async Task<CodeExecutor> DelayedCreateObject(CodeExecutor original, SelectableObject selectable)
        {
            var obj = selectable.GetComponent<IObject>();
            if (obj.cloneCount >= 25)
	            await Task.Delay(1);

            if (!running) return null;

            var exec = Instantiate(original, scene); //CreateObject(selectable, false, obj, selectable.GetComponent<CodeExecutor>());

            if (obj.lastCloneTime == Time.unscaledTime)
            {
	            obj.cloneCount++;
            }
            else
            {
	            obj.lastCloneTime = Time.unscaledTime;
	            obj.cloneCount = 1;
            }

            return exec;
        }

        public void StartRun()
        {
            if (!running)
            {
	            Time.timeScale = 1;
	            Time.fixedDeltaTime = 1f / 60f;

                saveObject.AutoSave();

	            PlayOutput.Instance.Reset();

	            // Disable Editor Movement Axes
                objEditor.selected = null;
	            objEditor.moveObject.gameObject.SetActive(false);
	            objEditor.rotObject.gameObject.SetActive(false);
	            objEditor.scaleObject.gameObject.SetActive(false);

                ranStart = false;
                StartCodeExecutors.Clear();
                UpdateCodeExecutors.Clear();
                BroadcastEvents.Clear();
                FixedUpdateCodeExecutors.Clear();

                DeletedObjects.Clear();

                //CollisionEnd.Clear();
                //CollisionStart.Clear();

                running = true;

                bool createdCamera = false;

                var sceneiObjects = objects.GetComponentsInChildren<IObject>();
                Array.Sort(sceneiObjects, (obj1, obj2) =>
                {
	                return obj1.Index.CompareTo(obj2.Index);
                });

                foreach (var sceneObject in sceneiObjects)
                {
	                var comp = ((MonoBehaviour)sceneObject).transform.GetComponent<SelectableObject>();
                    if (comp.TryGetComponent<IObject>(out var obj))
                    {
	                    var clone = CreateObject(comp, true, obj);

                        if (!createdCamera)
							createdCamera |= clone.GetComponent<Camera>();
                    }
                    else
                    {
                        print($"No object found in {comp.transform.name}");
                    }
                }

                editorCamera.SetActive(!createdCamera);
                foreach (var obj in editorObjects)
                {
                    obj.SetActive(false);
                }

                foreach (var obj in sceneObjects)
                {
                    obj.SetActive(true);
                }
            }
        }

        public void StopRun()
        {
            if (running)
            {
	            Time.timeScale = 1;
	            Time.fixedDeltaTime = 1f / 60f;

                running = false;

                foreach (Transform obj in scene)
                {
                    Destroy(obj.gameObject);
                }

                editorCamera.SetActive(true);
                foreach (var obj in editorObjects)
                {
                    obj.SetActive(true);
                }

                foreach (var obj in sceneObjects)
                {
                    obj.SetActive(false);
                }

                BroadcastEvents.Clear();
            }
        }

        private bool ranStart = false;
        void Update()
        {
	        if (running)
	        {
                playInputs.UpdateInputs();

		        if (ranStart)
		        {
			        for (int i = 0; i < UpdateCodeExecutors.Count; i++)
			        {
				        UpdateCodeExecutors[i].RunUpdate();
			        }
                }
		        else
		        {
			        ranStart = true;

			        for (int i = 0; i < StartCodeExecutors.Count; i++)
			        {
				        StartCodeExecutors[i].RunStart();
			        }
                }

                PurgeDeleted();
	        }
        }

        void FixedUpdate()
        {
	        if (running)
	        {
		        if (ranStart)
		        {
			        for (int i = 0; i < FixedUpdateCodeExecutors.Count; i++)
			        {
				        FixedUpdateCodeExecutors[i].RunPhysics();
			        }
                }

                PurgeDeleted();
            }
        }

        public void PurgeDeleted()
        {
	        if (DeletedObjects.Count > 0)
	        {
		        UpdateCodeExecutors.RemoveAll(o => DeletedObjects.Contains(o));
		        FixedUpdateCodeExecutors.RemoveAll(o => DeletedObjects.Contains(o));

                foreach (var b in BroadcastEvents) 
	                b.Value.RemoveAll(o => DeletedObjects.Contains(o.codeExec));

                foreach (var obj in DeletedObjects)
                {
	                if (obj != null)
	                {
		                obj.RunDeleted();
		                Destroy(obj.gameObject);
	                }
                }

                DeletedObjects.Clear();
	        }
        }

        public void Delete(CodeExecutor obj)
        {
            DeletedObjects.Add(obj);
        }
    }
}