using Cinemachine;
using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;

    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    public Transform playerTransform;
    [SerializeField] private float yFollowThreshold = 2f;
    [SerializeField] private float cameraFollowSpeed = 5f;

    private Transform cameraTransform;

    private void Awake()
    {
        instance = this;
        cameraTransform = virtualCamera.transform;
    }

    private void LateUpdate()
    {
        if (playerTransform == null) 
            return;

        Vector3 cameraPos = cameraTransform.position;
        float yDistance = Mathf.Abs(playerTransform.position.y - cameraPos.y);

        //If disstance grater then move camera
        if (yDistance > yFollowThreshold)
        {
            float targetY = Mathf.Lerp(cameraPos.y, playerTransform.position.y, Time.deltaTime * cameraFollowSpeed);
            cameraTransform.position = new Vector3(cameraPos.x, targetY, cameraPos.z);
        }
    }

    public void ResetZoom()
    {
        virtualCamera.m_Lens.OrthographicSize = 5;
    }

    public IEnumerator ZoomTo(int number, float timeToEnd)
    {
        float startValue = virtualCamera.m_Lens.OrthographicSize;
        float time = 0f;

        while (time < timeToEnd)
        {
            time += Time.deltaTime;
            virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(startValue, number, time / timeToEnd);
            yield return null;
        }
    }
}
