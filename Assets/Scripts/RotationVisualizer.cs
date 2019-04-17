using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationVisualizer : MonoBehaviour {

    public Transform t1;
    public Transform t2;
    public Transform t3;
    public Transform t4;

    enum Mode
    {
        TwistSwing,
        Clamp,
        Modulo,
    }
    Mode mode = Mode.TwistSwing;
    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            mode = (Mode)(((int)mode + 1)% (int)Mode.Modulo);
        }
        switch(mode)
        {
            case Mode.TwistSwing:
                TwistSwing();
                break;
            case Mode.Clamp:
                Clamp();
                break;
        }
    }
    public void Clamp()
    {
        t2.rotation = QuaternionHelper.Clamp(t1.rotation,45);
    }
    public void TwistSwing()
    {
        Quaternion twist;
        Quaternion swing;
        QuaternionHelper.DecomposeQuaternion(t1.rotation, Vector3.up, out twist, out swing);

        t2.rotation = twist;
        t3.rotation = swing;
        t4.rotation = twist * swing;
    }
}
