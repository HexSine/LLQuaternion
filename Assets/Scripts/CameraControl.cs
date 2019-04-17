using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float Speed;
    public float RotationSpeed;
    public float MaxPitch;
    // Update is called once per frame

    private Quaternion yaw = Quaternion.identity;
    private Quaternion pitch = Quaternion.identity;
    private void Start()
    {
        QuaternionHelper.DecomposeQuaternion(transform.rotation, Vector3.up, out yaw, out pitch);
    }
    void Update ()
    {
        if (Input.GetMouseButton(1))
        {
            float mousex = Input.GetAxis("Mouse X");
            float mousey = -Input.GetAxis("Mouse Y");
            yaw *= Quaternion.Euler(new Vector3(0, mousex, 0));
            pitch *= Quaternion.Euler(new Vector3(mousey, 0, 0));
            pitch = QuaternionHelper.Clamp(pitch, MaxPitch);
            transform.rotation = yaw * pitch;
        }


        Vector3 Movement = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
        {
            Movement += Vector3.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            Movement += Vector3.back;
        }
        if (Input.GetKey(KeyCode.A))
        {
            Movement += Vector3.left;
        }
        if (Input.GetKey(KeyCode.D))
        {
            Movement += Vector3.right;
        }
        if (Input.GetKey(KeyCode.E))
        {
            Movement += Vector3.up;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            Movement += Vector3.down;
        }

        transform.Translate(Movement.normalized * Speed * Time.deltaTime, Space.Self);


    }
}
