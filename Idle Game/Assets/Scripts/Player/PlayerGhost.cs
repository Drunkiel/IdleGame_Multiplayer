using TMPro;
using UnityEngine;

public class PlayerGhost : MonoBehaviour
{
    public string playerId;
    public string username;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private PlayerStatus currentStatus;

    [SerializeField] private Vector3 targetPosition;
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
        nameText.text = username;
    }

    public void SetPosition(Vector3 pos, bool force = false)
    {
        targetPosition = pos;

        if (force)
            transform.position = pos - new Vector3(Mathf.Sign(pos.x) * 0.05f, 0f, 0f);
    }
}
