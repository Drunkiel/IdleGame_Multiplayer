using Cinemachine;
using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;

    public Transform targetObject;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    private void Awake()
    {
        instance = this;
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
