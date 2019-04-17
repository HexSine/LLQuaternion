using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Camera))]
public class VectorLines : MonoBehaviour
{
    public static VectorLines Instance;
    // Lists of properties for each line
    public Color lineColor;
    public int lineWidth;
    public bool drawLines = true;

    // Material and camera
    private static Material lineMaterial;
    private Camera cam;

    // List of lines (each a list of vertices) and getter/setter
    private List<List<Vector2>> linePoints;

    void Awake()
    {
        cam = Camera.main;
        Instance = this;
        CreateLineMaterial();
    }

    public void AddPoint()
    {

    }

    static void CreateLineMaterial()
    {
        if (!lineMaterial)
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn on depth writes
            lineMaterial.SetInt("_ZWrite", 1);
        }
    }
    public void InitializeLines(List<List<Vector2>> newPointsList)
    {
        // Creates new list of points
        linePoints = new List<List<Vector2>>();
        for (int i = 0; i < newPointsList.Count; ++i)
        {
            List<Vector2> newList = new List<Vector2>();
            for (int j = 0; j < newPointsList[i].Count; ++j)
            {
                newList.Add(newPointsList[i][j]);
            }
            linePoints.Add(newList);
        }
    }

    public void UpdateLines(List<List<Vector2>> updatedPoints)
    {
        // Sets all points of list to update list of points
        for (int i = 0; i < linePoints.Count; ++i)
            for (int j = 0; j < linePoints[i].Count; ++j)
                linePoints[i][j] = updatedPoints[i][j];
    }

    void OnPostRender()
    {
        // Cycles through each separate line
        for (int i = 0; i < linePoints.Count; ++i)
        {
            if (!drawLines || linePoints == null || linePoints[i].Count < 2)
                return;

            float nearClip = cam.nearClipPlane + 0.00001f;
            int end = linePoints[i].Count - 1;
            float thisWidth = 1f / Screen.width * lineWidth * 0.5f;

            lineMaterial.SetPass(0);
            GL.Color(lineColor);

            if (lineWidth == 1)
            {
                GL.Begin(GL.LINES);
                for (int j = 0; j < end; ++j)
                {
                    GL.Vertex(cam.ViewportToWorldPoint(new Vector3(linePoints[i][j].x, linePoints[i][j].y, nearClip)));
                    GL.Vertex(cam.ViewportToWorldPoint(new Vector3(linePoints[i][j + 1].x, linePoints[i][j + 1].y, nearClip)));
                }
            }
            else
            {
                GL.Begin(GL.QUADS);
                for (int j = 0; j < end; ++j)
                {
                    Vector3 perpendicular = (new Vector3(linePoints[i][j + 1].y, linePoints[i][j].x, nearClip) -
                                         new Vector3(linePoints[i][j].y, linePoints[i][j + 1].x, nearClip)).normalized * thisWidth;
                    Vector3 v1 = new Vector3(linePoints[i][j].x, linePoints[i][j].y, nearClip);
                    Vector3 v2 = new Vector3(linePoints[i][j + 1].x, linePoints[i][j + 1].y, nearClip);
                    GL.Vertex(cam.ViewportToWorldPoint(v1 - perpendicular));
                    GL.Vertex(cam.ViewportToWorldPoint(v1 + perpendicular));
                    GL.Vertex(cam.ViewportToWorldPoint(v2 + perpendicular));
                    GL.Vertex(cam.ViewportToWorldPoint(v2 - perpendicular));
                }
            }
            GL.End();
        }
    }

    void OnApplicationQuit()
    {
        DestroyImmediate(lineMaterial);
    }
}