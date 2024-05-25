using Cinemachine;
using UnityEngine;

public class CameraRegister : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnEnable()
    {
        CameraManager.Register(GetComponent<CinemachineVirtualCamera>());
    }

    void OnDisable()
    {
        CameraManager.Unregister(GetComponent<CinemachineVirtualCamera>());
    }
}
