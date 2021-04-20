using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameTime : MonoBehaviour
{
    public float space = 10;
    public float height = 60;
    public int maxFrames = 360;
    public float targetFps = 60;

    public Color lineColor = new Color(0.2f, 1, 0.2f);
    public Color maxColor = new Color(1, 0.4f, 0.1f);

    public float width = 2;

    public List<float> frameTimes;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Update()
    {
        frameTimes.Add(Time.deltaTime);

        if (frameTimes.Count > maxFrames)
            frameTimes.RemoveAt(0);
    }

    // Update is called once per frame
    void OnGUI()
    {
        float min = 1 / targetFps;
        /*frameTimes.ForEach((float v) => { 
            if (v < min)
            {
                min = v;
            }
        });*/

        var targetY = height;
        DrawLine(new Vector2(0, targetY), new Vector2(frameTimes.Count * width, targetY), 1, maxColor);

        var lastPos = new Vector2(0, frameTimes[0] / min * height);
        for (int i = 1; i < frameTimes.Count; i++)
        {
            var newPos = new Vector2(i * width, frameTimes[i] / min * height);
            DrawLine(lastPos, newPos, width, lineColor);
            lastPos = newPos;
        }
    }

    Texture2D lineTex;

    private void DrawLine(Vector2 pointA, Vector2 pointB, float width, Color color)
    {
        var matrix = GUI.matrix;

        if (!lineTex)
        {
            lineTex = new Texture2D(1, 1);
            lineTex.SetPixel(0, 0, Color.white);
            lineTex.Apply();
        }

        // Store current GUI color, so we can switch it back later,
        // and set the GUI color to the color parameter
        var savedColor = GUI.color;
        GUI.color = color;

        // Determine the angle of the line.
        var angle = Vector3.Angle(pointB - pointA, Vector2.right);

        // Vector3.Angle always returns a positive number.
        // If pointB is above pointA, then angle needs to be negative.
        if (pointA.y > pointB.y) { angle = -angle; }

        // Use ScaleAroundPivot to adjust the size of the line.
        // We could do this when we draw the texture, but by scaling it here we can use
        //  non-integer values for the width and length (such as sub 1 pixel widths).
        // Note that the pivot point is at +.5 from pointA.y, this is so that the width of the line
        //  is centered on the origin at pointA.
        GUIUtility.ScaleAroundPivot(new Vector2((pointB - pointA).magnitude, width), new Vector2(pointA.x, pointA.y + 0.5f));

        // Set the rotation for the line.
        //  The angle was calculated with pointA as the origin.
        GUIUtility.RotateAroundPivot(angle, pointA);

        // Finally, draw the actual line.
        // We're really only drawing a 1x1 texture from pointA.
        // The matrix operations done with ScaleAroundPivot and RotateAroundPivot will make this
        //  render with the proper width, length, and angle.
        GUI.DrawTexture(new Rect(pointA.x, pointA.y, 1, 1), lineTex);

        // We're done.  Restore the GUI matrix and GUI color to whatever they were before.
        GUI.matrix = matrix;
        GUI.color = savedColor;
    }
}
