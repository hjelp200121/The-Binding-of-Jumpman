using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class RoomEditor : MonoBehaviour
{
    static string roomPrefabPath = "Assets/Resources/Rooms/";
    static string roomResourcePath = "Rooms/";
    static string defaultRoomName = "Room-0-000";

    public DungeonTile[] tiles;

    public InputField roomNameInput;

    Room loadedRoom = null;
    DungeonTile activeTile;
    // Start is called before the first frame update
    void Awake()
    {
        activeTile = tiles[0];
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            HandleInput();
        }
    }

    public void SelectTile(Dropdown change)
    {
        int index = change.value;
        activeTile = tiles[index];
    }

    public void NewRoom()
    {
        LoadRoom(defaultRoomName);
    }

    public void LoadRoom()
    {
        LoadRoom(roomNameInput.text);
    }

    public void LoadRoom(string roomName)
    {
        /* Load the room from the path specified. */
        var room = Resources.Load<Room>(roomResourcePath + roomName);
        /* If there is no room prefab at the path, give a warning and abort. */
        if (room == null)
        {
            Debug.LogWarning("No room named \"" + roomName + "\".");
            return;
        }

        /* Delete the currently loaded room. */
        if (loadedRoom != null)
        {
            Destroy(loadedRoom.gameObject);
        }

        loadedRoom = Instantiate<Room>(room);
    }

    public void SaveRoom()
    {
        SaveRoom(roomNameInput.text);
    }

    public void SaveRoom(string roomName)
    {
        bool success = false;
        if (loadedRoom != null && roomName != "")
        {
            PrefabUtility.SaveAsPrefabAsset(loadedRoom.gameObject, roomPrefabPath + roomName + ".prefab", out success);
        }
        if (!success)
        {
            Debug.LogWarning("Could not save room.");
        }
    }

    /* Handle mouse input changing the room. */
    void HandleInput()
    {
        if (loadedRoom == null)
        {
            /* If no room is loaded, abort. */
            return;
        }
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            Debug.Log(hit.collider.name);
            if (hit.collider.tag == "Room Floor")
            {
                Vector2 tileCoordinates = loadedRoom.transform.InverseTransformPoint(hit.point);
                tileCoordinates.x = Mathf.Round(tileCoordinates.x / Room.gridWidth - Room.gridWidth);
                tileCoordinates.y = Mathf.Round(tileCoordinates.y / Room.gridHeight - Room.gridHeight);
                EditTile((int)tileCoordinates.x, (int)tileCoordinates.y);
            }
            else if (hit.collider.tag == "Room Wall")
            {
                // Edit wall.
            }
        }
    }

    void EditTile(int x, int y)
    {
        if (x < 0 || y < 0 || x >= Room.gridWidth || y >= Room.gridHeight) {
            return;
        }
        Debug.Log("Editing cell at: (" + x + ", " + y + ")");
        DungeonTile oldTile = loadedRoom.contents.GetTile(x, y);
        if (oldTile != null)
        {
            Destroy(oldTile.gameObject);
        }
        DungeonTile newTile = null;
        if (activeTile != null)
        {
            newTile = Instantiate<DungeonTile>(activeTile, loadedRoom.transform);
            newTile.x = x;
            newTile.y = y;
        }
        loadedRoom.contents.SetTile(newTile, x, y);
    }
}
