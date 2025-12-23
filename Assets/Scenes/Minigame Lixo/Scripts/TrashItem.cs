using UnityEngine;

public class TrashItem : MonoBehaviour
{
    private bool isTargetable = false;
    public bool isIgnored = false;
    private float destroyXPosition = -15f;
    private AlbatrossController albatross;

    void Start()
    {
        albatross = FindFirstObjectByType<AlbatrossController>();
    }

    void Update()
    {
        if (GameManagerLixo.instance == null) return;
        float speed = GameManagerLixo.instance.minigameSpeed;
        transform.Translate(Vector2.left * speed * Time.deltaTime);

        if (transform.position.x < destroyXPosition)
        {
            if (isTargetable && albatross != null)
            {
                albatross.CancelTarget();
            }
            Destroy(gameObject);
        }
    }

    void OnMouseDown()
    {
        if (isTargetable)
        {
            GameManagerLixo.instance.PlayTrashSound();

            albatross.ReturnHome();

            isTargetable = false;
            isIgnored = true;
        }
    }

    public void SetTargetable(bool status)
    {
        isTargetable = status;
    }

    public void ClearTarget()
    {
        isTargetable = false;
    }
}