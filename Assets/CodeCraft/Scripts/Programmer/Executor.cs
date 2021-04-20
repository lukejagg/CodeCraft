using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Programmer
{
    public class Executor : MonoBehaviour
    {
        Variables localVariables;
        Program startProgram = null;
        Program updateProgram = null;

        private void Awake()
        {
            var deltaTime = new DeltaTime();

            localVariables = GetComponent<Variables>();
            //localVariables["Number"] = new PositionValue(1,2,3);

            var value = localVariables["Number"];
            /*updateProgram = new Program(new ICode[] { 
                new SetVariable(value, new Multiply(value, deltaTime)),
                new Print(value),
            });*/
        }

        private void Start()
        {

        }

        private void Update()
        {

        }
    }
}