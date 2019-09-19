using UnityEngine;

public class Door : MonoBehaviour
{
    public Room room;
    public Door connection;
    public Directions direction;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player") {
            PlayerController player = other.GetComponent<PlayerController>();
            /* Move the player to the entrance of the other door. */
            player.transform.Translate(direction.ToVector() * 3f);
            connection.room.Load(player);
            Camera.main.GetComponent<CameraController>().target = connection.room.transform;
            room.UnLoad();
        }
    }
}