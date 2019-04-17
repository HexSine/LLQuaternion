using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;

public enum RotationMode
{
    EditorControl,
    Euler,
    AngleAxis,
    FromToRotation,
    LookRotation,
}
public class QuaternionSequence : MonoBehaviour {

    public Material material;
    [System.Serializable]
    public class QuaternionObject
    {
        public RotationPanel Panel;
        public Transform Target;
        public Color color;
    }
    public float Radius;
    public float LineWidth;
    public int resolution;
    public Transform Target;
    public Transform TargetEuler;
    public List<QuaternionObject> Rotations;

    static Material lineMaterial;
    private void Awake()
    {
        CameraEvents.OnPostRenderEvent.AddListener(OnPostRender);
        for(int i = 0, c = Rotations.Count; i < c; ++i)
        {
            QuaternionObject element = Rotations[i];
            element.Panel.SetSphereColor(element.color);
        }
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

    public void Update()
    {
        Quaternion result = Quaternion.identity;
        Vector3 eulerResult = Vector3.zero;
        for (int i = 0, c = Rotations.Count; i < c; ++i)
        {
            QuaternionObject element = Rotations[i];
            UpdateElement(element);
            result *= element.Target.rotation;
            eulerResult += element.Target.eulerAngles;
        }
        Target.rotation = result;
        TargetEuler.eulerAngles = eulerResult;
    }

    public void OnPostRender()
    {
        CreateLineMaterial();
        Quaternion result = Quaternion.identity;

        lineMaterial.SetPass(0);

        //Draw the Quaternion Result
        GL.PushMatrix();
        GL.MultMatrix(Matrix4x4.TRS(Target.position, Quaternion.identity, Target.lossyScale));

        GL.Begin(GL.TRIANGLE_STRIP);

        for (int i = 0, c = Rotations.Count; i < c; ++i)
        {
            QuaternionObject element = Rotations[i];


            GL.Color(element.color);
            Quaternion beforeRotation = result;
            result *= element.Target.rotation;
            DrawInterpolatedQuaternion(beforeRotation, result);
        }
        GL.End();
        GL.PopMatrix();


        //Draw the Euler Result
        GL.PushMatrix();
        GL.MultMatrix(Matrix4x4.TRS(TargetEuler.position, Quaternion.identity, Target.lossyScale));

        GL.Begin(GL.TRIANGLE_STRIP);

        Vector3 eulerResult = Vector3.zero;
        for (int i = 0, c = Rotations.Count; i < c; ++i)
        {
            QuaternionObject element = Rotations[i];


            GL.Color(element.color);
            Quaternion beforeRotation = Quaternion.Euler(eulerResult);
            eulerResult += element.Target.eulerAngles;
            DrawInterpolatedQuaternion(beforeRotation, Quaternion.Euler(eulerResult));
        }
        GL.End();
        GL.PopMatrix();

        //Draw the component results
        for (int i = 0, c = Rotations.Count; i < c; ++i)
        {
            QuaternionObject element = Rotations[i];

            GL.PushMatrix();
            GL.MultMatrix(Matrix4x4.TRS(element.Target.position, Quaternion.identity, Target.lossyScale));

            GL.Begin(GL.TRIANGLE_STRIP);

            GL.Color(element.color);
            Quaternion elementRotation;
            elementRotation = element.Target.rotation;
            DrawInterpolatedQuaternion(Quaternion.identity, elementRotation);

            GL.End();
            GL.PopMatrix();
        }
    }
    public void UpdateElement(QuaternionObject element)
    {
        Quaternion elementRotation;
        switch(element.Panel.GetMode())
        {
            case RotationMode.Euler:
                if(element.Panel.GetInverse())
                {
                    elementRotation = Quaternion.Inverse(Quaternion.Euler(element.Panel.GetVector()));
                }
                else
                {
                    elementRotation = Quaternion.Euler(element.Panel.GetVector());
                }
                element.Target.rotation = elementRotation;
                break;
            case RotationMode.AngleAxis:
                if (element.Panel.GetInverse())
                {
                    elementRotation = Quaternion.Inverse(Quaternion.AngleAxis(element.Panel.GetFloat(), element.Panel.GetVector()));
                }
                else
                {
                    elementRotation = Quaternion.AngleAxis(element.Panel.GetFloat(), element.Panel.GetVector());
                }
                element.Target.rotation = elementRotation;
                break;
            case RotationMode.LookRotation:
                if (element.Panel.GetInverse())
                {
                    elementRotation = Quaternion.Inverse(Quaternion.LookRotation(element.Panel.GetVector()));
                }
                else
                {
                    elementRotation = Quaternion.LookRotation(element.Panel.GetVector());
                }
                element.Target.rotation = elementRotation;
                element.Panel.sphere.transform.position = element.Target.position + element.Panel.GetVector();
                break;
        }
    }
    public void DrawInterpolatedQuaternion(Quaternion before, Quaternion after)
    {
        Quaternion difference = Quaternion.Inverse(before) * after;


        for(float i = 0, c = resolution; i <= c; ++i)
        {
            float t = i/c;
            float t2 = (i + 1)/c;
            Quaternion q1 = Quaternion.Slerp(before, after, t);
            Quaternion q2 = Quaternion.Slerp(before, after, t2);
            Vector3 v1 = q1 * new Vector3(0, 0, Radius);
            Vector3 v2 = q2 * new Vector3(0, 0, Radius);
            Vector3 cross = Vector3.Cross(Camera.main.transform.forward,v2 - v1).normalized * LineWidth;
            if (i == 0)
            {
                GL.Vertex(v1 + cross);
                GL.Vertex(v1 - cross);
            }
            else
            { 
                GL.Vertex(v2 + cross);
                GL.Vertex(v2 - cross);
            }
        }
    }
}
