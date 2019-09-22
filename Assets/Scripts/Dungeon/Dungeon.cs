using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FloorName {
    GARDEN, KITCHEN, WORKSHOP
}

public class Dungeon : MonoBehaviour
{
    public PlayerController playerPrefab;
    public List<Room> rooms;

    public FloorName floor;

    PlayerController player;
    // Start is called before the first frame update
    void Start()
    {
        PlayerController player = Instantiate<PlayerController>(playerPrefab);
        Room startRoom = rooms[0];
        Camera.main.GetComponent<CameraController>().target = startRoom.transform;
        player.transform.position = startRoom.transform.position;
        startRoom.Load(player);
    }
}
