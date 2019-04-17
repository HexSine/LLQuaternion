using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class QuaternionHelper
{
    public static Quaternion Conjugate(Quaternion rotation)
    {
        return new Quaternion(-rotation.x, -rotation.y, -rotation.z, rotation.w);
    }
    public static Quaternion Clamp(Quaternion rotation, float MaxRotation)
    {
        float angle;
        Vector3 axis;
        rotation.ToAngleAxis(out angle, out axis);
        if (angle > 180)
        {
            angle -= 360;
        }
        angle = Mathf.Clamp(angle, -MaxRotation, MaxRotation);
        return Quaternion.AngleAxis(angle, axis);
    }
    public static void ScaleQuat(ref Quaternion rotation, float scale)
    {
        rotation.x *= scale;
        rotation.y *= scale;
        rotation.z *= scale;
        rotation.w *= scale;
    }
    public static Quaternion GetTwist(Quaternion rotation, Vector3 Direction)
    {
        Vector3 rotationAxis = new Vector3(rotation.x, rotation.y, rotation.z);
        Vector3 projected = Vector3.Project(rotationAxis, Direction);
        return new Quaternion(projected.x, projected.y, projected.z, rotation.w);
    }
    public static Quaternion GetSwing(Quaternion rotation, Vector3 Direction)
    {
        Quaternion twist = GetTwist(rotation, Direction);
        return Quaternion.Inverse(twist) * rotation;
    }
    public static void DecomposeQuaternion(Quaternion rotation, Vector3 direction, out Quaternion twist, out Quaternion swing)
    {
        twist = GetTwist(rotation, direction);
        swing = Quaternion.Inverse(twist) * rotation;
    }
    public static void ApplyRotationOverTime(ref Quaternion current, ref Quaternion rotation, float speed)
    {
        Quaternion quat = Quaternion.RotateTowards(Quaternion.identity, rotation, speed * Time.deltaTime);
        current *= quat;
        rotation = Quaternion.Inverse(quat) * rotation;
    }
}
