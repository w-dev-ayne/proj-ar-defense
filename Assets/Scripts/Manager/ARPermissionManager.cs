using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.Android;

public class ARPermissionManager : MonoBehaviour
{
    private ARSession arSession;

    void Start()
    {
        arSession = GetComponent<ARSession>();

        // Android 및 iOS에서 카메라 권한 확인
        if (Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            // 권한이 이미 허용되었으면 ARSession을 바로 활성화
            EnableARSession();
        }
        else
        {
            // 권한을 요청
            Permission.RequestUserPermission(Permission.Camera);
        }
    }

    void Update()
    {
        // Android에서 권한 허용 후 AR 활성화
        if (Permission.HasUserAuthorizedPermission(Permission.Camera) && arSession.enabled == false)
        {
            EnableARSession();
        }
    }

    private void EnableARSession()
    {
        // 권한이 허용되면 ARSession을 활성화
        if (arSession != null)
        {
            // ARSession을 비활성화 후 다시 활성화
            arSession.enabled = false;
            arSession.enabled = true;
        }
    }
}