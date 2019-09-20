using UnityEngine;

public class Door : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite openSprite;
    public Sprite closedSprite;
    public Collider2D openCollider;
    public Collider2D closedCollider;

    public bool closed;

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
        openCollider.enabled = true;
        closedCollider.enabled = false;
    }

    public virtual void Close()
    {
        closed = true;
        spriteRenderer.sprite = closedSprite;
        openCollider.enabled = false;
        closedCollider.enabled = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            PlayerController player = other.GetComponent<PlayerController>();
            /* Move the player to the entrance of the other door. */
            player.transform.Translate(direction.ToVector() * 2f);
            connection.room.Load(player);
            Camera.main.GetComponent<CameraController>().target = connection.room.transform;
            room.UnLoad();
        }
    }
}