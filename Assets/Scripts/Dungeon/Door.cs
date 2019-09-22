using UnityEngine;

public class Door : MonoBehaviour, IExplodable
{
    public SpriteRenderer spriteRenderer;
    public Sprite openSprite;
    public Sprite closedSprite;
    public Collider2D openCollider;
    public Collider2D closedCollider;

    public bool closed;
    public bool broken = false;

    public Room room;
    public Door connection;
    public Directions direction;


    public void Awake()
    {
        Open();
    }

    public virtual void Open()
    {
        closed = false;
        spriteRenderer.sprite = openSprite;
        if (openCollider != null)
        {
            openCollider.enabled = true;
        }
        if (closedCollider != null)
        {
            closedCollider.enabled = false;
        }
    }

    public virtual void Close()
    {
        if (!broken)
        {
            closed = true;
            spriteRenderer.sprite = closedSprite;
            if (openCollider != null)
            {
                openCollider.enabled = false;
            }
            if (closedCollider != null)
            {
                closedCollider.enabled = true;
            }
        }
    }

    public virtual void BlowUp(DungeonObject source, float damage)
    {
        Open();
        broken = true;
    }

    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            PlayerController player = other.GetComponent<PlayerController>();
            /* Move the player to the entrance of the other door. */
            player.transform.Translate(direction.ToVector() * 2f);
            room.UnLoad();
            connection.room.Load(player);
            Camera.main.GetComponent<CameraController>().target = connection.room.transform;

            broken = false;
        }
    }
}