using TMPro;
using UnityEngine;

public class PlayerGhost : MonoBehaviour
{
    public string playerId;
    public string username;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Animator anim;
    [SerializeField] private PlayerStatus currentStatus;

    [SerializeField] private Vector3 targetPosition;
    private readonly float lerpSpeed = 5f;
    private InventoryAPI _inventory;

    void Start()
    {
        targetPosition = transform.position;
        _inventory = GetComponent<InventoryAPI>();
        StartCoroutine(_inventory.GetInventoryCoroutine(playerId, GetComponent<ItemController>()));
    }

    private void Update()
    {
        float distance = Vector3.Distance(targetPosition, transform.position);
        anim.SetFloat("Movement", distance);
        if (distance > 0.05f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * lerpSpeed);
            transform.GetChild(1).localScale = new(Mathf.Sign((targetPosition - transform.position).x) * 1, 1, 1);
        }
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
