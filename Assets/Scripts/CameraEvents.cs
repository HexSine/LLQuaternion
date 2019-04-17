using UnityEngine;
using UnityEngine.Events;
public class CameraEvents : MonoBehaviour
{
    public static UnityEvent OnPostRenderEvent = new UnityEvent();

    private void OnPostRender()
    {
        OnPostRenderEvent.Invoke();
    }
}