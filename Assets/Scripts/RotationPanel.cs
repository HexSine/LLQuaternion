using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RotationPanel : MonoBehaviour
{
    public TMPro.TMP_Dropdown RotationMode;
    public MeshRenderer sphere;
    public Text vecx;
    public Text vecy;
    public Text vecz;
    public Text floatVal;
    public Toggle Inverse;
    public void Start()
    {
        UpdateMode();
    }
    public void SetSphereColor(Color color)
    {
        sphere.material.color = color;
    }
    public void UpdateMode()
    {
        switch(GetMode())
        {
            case global::RotationMode.EditorControl:
                SetVecActive(false);
                SetFloatActive(false);
                SetSphereActive(false);
                break;
            case global::RotationMode.Euler:
                SetVecActive(true);
                SetFloatActive(false);
                SetSphereActive(false);
                break;
            case global::RotationMode.AngleAxis:
                SetVecActive(true);
                SetFloatActive(true);
                SetSphereActive(false);
                break;
            case global::RotationMode.LookRotation:
                SetVecActive(true);
                SetFloatActive(false);
                SetSphereActive(true);
                break;
            case global::RotationMode.FromToRotation:
                SetVecActive(false);
                SetFloatActive(false);
                SetSphereActive(false);
                break;
        }
    }
    private void SetVecActive(bool active)
    {
        vecx.transform.parent.gameObject.SetActive(active);
        vecy.transform.parent.gameObject.SetActive(active);
        vecz.transform.parent.gameObject.SetActive(active);
    }
    private void SetSphereActive(bool active)
    {
        sphere.gameObject.SetActive(active);
    }
    private void SetFloatActive(bool active)
    {
        floatVal.transform.parent.gameObject.SetActive(active);
    }
    public RotationMode GetMode()
    {
        return (RotationMode)RotationMode.value;
    }
    public bool GetInverse()
    {
        return Inverse.isOn;
    }
    public Vector3 GetVector()
    {
        Vector3 ret = Vector3.zero;
        if(!float.TryParse(vecx.text, out ret.x))
        {
            vecx.text = 0.ToString();
        }
        if (!float.TryParse(vecy.text, out ret.y))
        {
            vecy.text = 0.ToString();
        }
        if (!float.TryParse(vecz.text, out ret.z))
        {
            vecz.text = 0.ToString();
        }
        return ret;


    }
    public float GetFloat()
    {
        float ret = 0;
        if (!float.TryParse(floatVal.text, out ret))
        {
            floatVal.text = 0.ToString();
        }
        return ret;
    }
}
