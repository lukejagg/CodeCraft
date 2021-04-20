using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using ObjectEditor;
using UnityEngine;

namespace Programmer
{
    public interface ICode
    {
        Task<ReturnCode> Run(CodeExecutor obj);
    }
}