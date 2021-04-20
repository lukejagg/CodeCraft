using System;
using Interpreter;
using Programmer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ObjectEditor
{
	public class HasEvents
	{
		public bool Start = false, Update = false, FixedUpdate = false;
	}

    public class CodeExecutor : MonoBehaviour
    {
	    public bool isClone = false;
	    public SelectableObject original;
	    public CodeExecutor lastClone;

        // Index of the variables in Variables
	    public Dictionary<string, object> Variables;

        public Program[] startProgram;
        public Program[] updateProgram;
        public Program[] fixedUpdateProgram;

		public CodeExecutor lastHit;
        public Program[] collisionEnterPrograms;
        public Program[] collisionEndPrograms;

        public Program[] deletedPrograms;

        public Dictionary<string, Program[]> broadcastPrograms;
		// Todo: When adding Program[], add to SceneExecutor.CreateObject too

		public Vector3 color;
		public float transparency = 0;

		public void RefreshColor()
		{
			var colorColor = new Color(color.x, color.y, color.z, 1 - transparency);

			if (TryGetComponent<MeshRenderer>(out var m))
			{
				m.material.color = colorColor;
			}
			else if (TryGetComponent<Light>(out var l))
			{
				l.color = colorColor;
				// Transparency sets shadowStrength?

				// Todo: also change in SceneExecutor.CreateObject
			}
		}

		public void SetColor(Vector3 c)
		{
			color = c;
			RefreshColor();
		}

		public HasEvents CreateCode(B_BeginEvent startEvent)
        {
            var currentEvent = startEvent.NextConnection;
            if (currentEvent == null) return new HasEvents();

	        // Initialize Variables
	        var variables = new List<CachedValue>();
            Variables = new Dictionary<string, object>();
            while (currentEvent != null)
            {
	            if (currentEvent is B_VariableEvent variableEvent)
	            {
		            var name = variableEvent.variableName.text;
                    Variables.Add(name, null);
		            variables.Add(new CachedValue(0f));
	            }

	            currentEvent = currentEvent.NextConnection;
            }

			// Initialize Events after (because some code requires variables)
			var start = new List<Program>();
	        var update = new List<Program>();
	        var collisionEnter = new List<Program>();
	        var collisionEnd = new List<Program>();
	        var fixedUpdate = new List<Program>();
	        var deleted = new List<Program>();

			var broadcastPrograms = new Dictionary<string, List<Program>>();

			var events = new HasEvents();

	        currentEvent = startEvent.NextConnection;
	        while (currentEvent != null)
	        {
				// Todo: Convert to hash map for O(1)?
		        if (currentEvent is B_StartEvent)
		        {
                    start.Add(currentEvent.ConvertToCode());
                    events.Start = true;
		        }
                else if (currentEvent is B_UpdateEvent)
		        {
                    update.Add(currentEvent.ConvertToCode());
                    events.Update = true;
		        }
				else if (currentEvent is B_Received b)
		        {
			        var bp = currentEvent.ConvertToCode();

			        if (broadcastPrograms.TryGetValue(b.broadcastName.text, out var list))
			        {
						list.Add(bp);
			        }
			        else
			        {
				        var l = new List<Program>();
						l.Add(bp);
						broadcastPrograms.Add(b.broadcastName.text, l);
			        }
		        }
				else if (currentEvent is B_TouchBeganEvent)
		        {
					collisionEnter.Add(currentEvent.ConvertToCode());
		        }
		        else if (currentEvent is B_TouchEndedEvent)
		        {
			        collisionEnd.Add(currentEvent.ConvertToCode());
		        }
		        else if (currentEvent is B_PhysicsUpdateEvent)
		        {
			        fixedUpdate.Add(currentEvent.ConvertToCode());
			        events.FixedUpdate = true;
				}
				else if (currentEvent is B_Deleted)
		        {
					deleted.Add(currentEvent.ConvertToCode());
		        }

				currentEvent = currentEvent.NextConnection;
	        }
            //updateProgram = startEvent.NextConnection?.ConvertToCode();

            startProgram = start.ToArray();
            updateProgram = update.ToArray();
            fixedUpdateProgram = fixedUpdate.ToArray();

            collisionEnterPrograms = collisionEnter.ToArray();
            collisionEndPrograms = collisionEnd.ToArray();

            deletedPrograms = deleted.ToArray();

			this.broadcastPrograms = new Dictionary<string, Program[]>(broadcastPrograms.Count);
			foreach (var bp in broadcastPrograms)
				this.broadcastPrograms.Add(bp.Key, bp.Value.ToArray());

			foreach (var bps in this.broadcastPrograms)
            {
				SceneExecutor.Instance.AddBroadcast(bps.Key, new BroadcastProgram(this, bps.Value));
			}

            return events;
        }

        public void RunStart()
        {
	        for (int i = 0; i < startProgram.Length; i++)
	        {
		        startProgram[i].Execute(this);
	        }
        }

        // Update is called once per frame
        public void RunUpdate()
        {
	        for (int j = 0; j < updateProgram.Length; j++)
	        {
		        updateProgram[j].Execute(this);
	        }
        }

        public void RunPhysics()
        {
	        for (int j = 0; j < fixedUpdateProgram.Length; j++)
	        {
		        fixedUpdateProgram[j].Execute(this);
	        }
        }

        public void RunDeleted()
        {
	        if (deletedPrograms == null) return;

	        for (int j = 0; j < deletedPrograms.Length; j++)
	        {
		        deletedPrograms[j].Execute(this);
	        }
        }


		// Rigidbody stuff
		public new Rigidbody rigidbody;

        public void ToggleRigidbody(bool on)
        {
	        if (on)
	        {
		        if (rigidbody == null)
			        rigidbody = gameObject.AddComponent<Rigidbody>();
	        }
	        else
	        {
		        if (rigidbody != null)
			        Destroy(rigidbody);
            }
        }

        public override string ToString()
        {
	        return gameObject.name;
        }

		// Collision Events
		void OnCollisionEnter(Collision c)
		{
			if (collisionEnterPrograms == null || collisionEnterPrograms.Length == 0) return;

			lastHit = c.transform.GetComponent<CodeExecutor>();
			//SceneExecutor.Instance.CollisionStart.Add(new ObjectCollision(this, c.transform.GetComponent<CodeExecutor>()));
			for (int j = 0; j < collisionEnterPrograms.Length; j++)
			{
				collisionEnterPrograms[j].Execute(this);
			}
		}

		void OnCollisionExit(Collision c)
		{
			if (collisionEndPrograms == null || collisionEndPrograms.Length == 0) return;


			lastHit = c.transform.GetComponent<CodeExecutor>();

			for (int j = 0; j < collisionEndPrograms.Length; j++)
			{
				collisionEndPrograms[j].Execute(this);
			}
		}
	}
}