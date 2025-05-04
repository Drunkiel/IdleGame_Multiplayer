using TMPro;
using UnityEngine;

public class PlayerGhost : MonoBehaviour
{
    public string playerId;
    [SerializeField] private TMP_Text ala;
    private PlayerStatus currentStatus;

    private Vector3 targetPosition;
    private float lerpSpeed = 5f;

    void Start()
    {
        targetPosition = transform.position;
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * lerpSpeed);
        transform.GetChild(1).localScale = new(Mathf.Sign((targetPosition - transform.position).x) * 1, 1, 1);
    }

    public void SetStatus(PlayerStatus status)
    {
        currentStatus = status;
        ala.text = playerId;
    }

    public void SetPosition(Vector3 pos)
    {
        targetPosition = pos;
    }
}
