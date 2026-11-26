using UnityEngine;
using Photon.Pun;

public class ClientSideCamera : MonoBehaviourPunCallbacks
{
    void Start()
    {
        if (photonView.IsMine)
        {
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                ConfigureCamera(mainCamera);
            }
            else
            {
                Debug.LogWarning("Main camera not found.");
            }
        }
    }

    void ConfigureCamera(Camera camera)
    {
        // 필요한 카메라 설정 조정
        camera.useOcclusionCulling = false;

        camera.cullingMask = -1;
        camera.clearFlags = CameraClearFlags.Skybox;
        camera.nearClipPlane = 0.1f; 
        camera.farClipPlane = 1000f;

        Debug.Log("Camera configured for local player.");
    }
}