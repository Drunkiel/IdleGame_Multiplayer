using Cinemachine;
using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;

    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    public Transform playerTransform;

    [SerializeField] private float yFollowThreshold = 2f;
    [SerializeField] private float xFollowThreshold = 2f;
    [SerializeField] private float cameraFollowSpeed = 5f;

    [SerializeField] private bool followHorizontal = true;
    [SerializeField] private bool followVertical = true;

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
        Vector3 targetPos = cameraPos;

        if (followVertical)
        {
            float yDistance = Mathf.Abs(playerTransform.position.y - cameraPos.y);
            if (yDistance > yFollowThreshold)
                targetPos.y = Mathf.Lerp(cameraPos.y, playerTransform.position.y, Time.deltaTime * cameraFollowSpeed);
        }

        if (followHorizontal)
        {
            float xDistance = Mathf.Abs(playerTransform.position.x - cameraPos.x);
            if (xDistance > xFollowThreshold)
                targetPos.x = Mathf.Lerp(cameraPos.x, playerTransform.position.x, Time.deltaTime * cameraFollowSpeed);
        }

        cameraTransform.position = targetPos;
    }

    public void Config(bool followX, bool followY)
    {
        followHorizontal = followX;
        followVertical = followY;
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
