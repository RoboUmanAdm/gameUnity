using System.Collections;
using UnityEngine;

public class LookToCamera : MonoBehaviour
{
   private Camera myCamera;
    void Awake()
    {
        
    }

private void LateUpdate() {
    transform.LookAt(myCamera.transform.position);
    transform.Rotate(Vector3.up*180);
}
}
