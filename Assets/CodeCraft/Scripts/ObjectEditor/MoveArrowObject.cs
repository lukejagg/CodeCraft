using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ObjectEditor
{
    public class MoveArrowObject : MonoBehaviour
    {
        public Transform upCyl, upCone, downCyl, downCone;
        const float cyl = 0.6f;
        const float cone = 1.3f;

        public void SetPos(float size)
        {
            upCyl.localPosition = Vector3.up * (size + cyl);
            upCone.localPosition = Vector3.up * (size + cone);
            downCyl.localPosition = Vector3.up * (-size -cyl);
            downCone.localPosition = Vector3.up * (-size -cone);
        }
    }
}