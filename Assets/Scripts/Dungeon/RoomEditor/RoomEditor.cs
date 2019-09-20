using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class RoomEditor : MonoBehaviour
{
    public enum DungeonObjectType
    {
        TILE, OBJECT
    }

    static string roomPrefabPath = "Assets/Resources/Rooms/";
    static string roomResourcePath = "Rooms/";
    static string defaultRoomName = "Room-Common-0-000";

    public DungeonTile[] tiles;
    public DungeonObject[] objects;

    public InputField roomNameInput;
    public GameObject doorGraphicPrefab;

    public GameObject[] doors;
    Room loadedRoom = null;
    DungeonTile activeTile;
    DungeonObject activeObject;

    /* Which dungeon object is currently active? A tile or a general object? */
    DungeonObjectType activeObjectType;

    // Start is called before the first frame update
    void Awake()
    {
        activeObjectType = DungeonObjectType.TILE;
        activeTile = tiles[0];
        activeObject = objects[0];
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            HandleInput();
        }
    }

    public void SetObjectMode(int type)
    {
        SetObjectMode((DungeonObjectType)type);
    }

    public void SetObjectMode(DungeonObjectType type)
    {
        activeObjectType = type;
    }

    public void SelectObject(Dropdown change)
    {
        int index = change.value;
        if (activeObjectType == DungeonObjectType.OBJECT)
        {
            activeObject = objects[index];

        }
        else if (activeObjectType == DungeonObjectType.TILE)
        {
            activeTile = tiles[index];
        }
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

        GameObject loadedRoomObject = (GameObject)PrefabUtility.InstantiatePrefab(room.gameObject);
        loadedRoom = loadedRoomObject.GetComponent<Room>();
        foreach (DungeonTile tile in loadedRoom.contents.tiles)
        {
            if (tile != null)
            {
                tile.EnableDebug();
            }
        }
        foreach (DungeonObject dungeonObject in loadedRoom.contents.objects)
        {
            dungeonObject.EnableDebug();
        }
        UpdateDoorGraphics();
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
        int layerMask = LayerMask.GetMask("Room");
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition),
                                             Vector2.zero, Mathf.Infinity, layerMask);
        if (hit.collider != null)
        {
            if (hit.collider.tag == "Room Floor")
            {
                Vector2 tileCoordinates = loadedRoom.transform.InverseTransformPoint(hit.point);
                tileCoordinates.x = Mathf.Clamp(tileCoordinates.x + Room.gridWidth / 2f,
                                                0, Room.gridWidth - 1);
                tileCoordinates.y = Mathf.Clamp(tileCoordinates.y + Room.gridHeight / 2f,
                                                0, Room.gridHeight - 1);
                /* If the active object type is a tile, edit the selected tile. */
                if (activeObjectType == DungeonObjectType.TILE)
                {
                    EditTile((int)tileCoordinates.x, (int)tileCoordinates.y);
                }
                /* If the active object type is an object, add the selected object. */
                else if (activeObjectType == DungeonObjectType.OBJECT)
                {
                    /* Only add an object on the first frame the button is pushed down. */
                    if (!Input.GetMouseButtonDown(0))
                    {
                        return;
                    }
                    AddObject((int)tileCoordinates.x, (int)tileCoordinates.y);
                }
            }
            else if (hit.collider.tag == "Room Wall")
            {
                /* Only toggle the wall on the first frame the button is pushed down. */
                if (!Input.GetMouseButtonDown(0))
                {
                    return;
                }
                Vector2 localHitPoint = loadedRoom.transform.InverseTransformPoint(hit.point);
                /* Scale the point so the width mathces the height. */
                localHitPoint.x *= loadedRoom.height;
                localHitPoint.y *= loadedRoom.width;

                Directions direction;
                if (Mathf.Abs(localHitPoint.x) > Mathf.Abs(localHitPoint.y))
                {
                    if (localHitPoint.x > 0)
                    {
                        direction = Directions.EAST;
                    }
                    else
                    {
                        direction = Directions.WEST;
                    }
                }
                else
                {
                    if (localHitPoint.y > 0)
                    {
                        direction = Directions.NORTH;
                    }
                    else
                    {
                        direction = Directions.SOUTH;
                    }
                }

                EditWall(direction);
                UpdateDoorGraphics();
            }
        }
    }

    void AddObject(int x, int y)
    {
        if (x < 0 || y < 0 || x >= Room.gridWidth || y >= Room.gridHeight)
        {
            return;
        }
        if (activeObject == null)
        {
            return;
        }
        GameObject _newObject = PrefabUtility.InstantiatePrefab(activeObject.gameObject) as GameObject;
        DungeonObject newObject = _newObject.GetComponent<DungeonObject>();
        newObject.transform.SetParent(loadedRoom.transform);

        Vector2 postion;
        postion.x = x - Room.gridWidth / 2f + .5f;
        postion.y = y - Room.gridHeight / 2f + .5f;
        newObject.transform.position = loadedRoom.transform.TransformPoint(postion);

        newObject.room = loadedRoom;
        newObject.EnableDebug();

        loadedRoom.contents.objects.Add(newObject);
    }

    void EditTile(int x, int y)
    {
        if (x < 0 || y < 0 || x >= Room.gridWidth || y >= Room.gridHeight)
        {
            return;
        }
        DungeonTile oldTile = loadedRoom.contents.GetTile(x, y);
        if (oldTile != null)
        {
            Destroy(oldTile.gameObject);
        }
        DungeonTile newTile = null;
        if (activeTile != null)
        {
            GameObject _newTile = PrefabUtility.InstantiatePrefab(activeTile.gameObject) as GameObject;
            newTile = _newTile.GetComponent<DungeonTile>();
            newTile.transform.SetParent(loadedRoom.transform);
            newTile.x = x;
            newTile.y = y;
            Vector2 postion;
            postion.x = x - Room.gridWidth / 2f + .5f;
            postion.y = y - Room.gridHeight / 2f + .5f;
            newTile.transform.position = loadedRoom.transform.TransformPoint(postion);

            newTile.room = loadedRoom;
            newTile.EnableDebug();
        }

        loadedRoom.contents.SetTile(newTile, x, y);
    }

    void EditWall(Directions direction)
    {
        /* Invert the door mask bit toggling whether or not a door can exist. */
        int bit = 1 << (int)direction;
        loadedRoom.doorMask ^= bit;
    }

    void UpdateDoorGraphics()
    {
        /* If no room is loaded, remove all door graphics. */
        if (loadedRoom == null)
        {
            for (Directions d = Directions.NORTH;
                 d <= Directions.WEST; d++)
            {
                GameObject door = doors[(int)d];
                if (door != null)
                {
                    Destroy(door);
                    doors[(int)d] = null;
                }
            }
        }
        for (Directions d = Directions.NORTH;
             d <= Directions.WEST; d++)
        {
            int mask = 1 << (int)d;
            if ((loadedRoom.doorMask & mask) != 0)
            {
                Destroy(doors[(int)d]);

                GameObject door = Instantiate<GameObject>(doorGraphicPrefab, transform);
                if (d == Directions.NORTH || d == Directions.SOUTH)
                {
                    door.transform.position = d.ToVector() * (loadedRoom.height - 1f) / 2f;
                }
                else
                {
                    door.transform.position = d.ToVector() * (loadedRoom.width - 1f) / 2f;
                }

                door.transform.position = loadedRoom.transform.TransformPoint(door.transform.position);
                door.transform.rotation = loadedRoom.transform.rotation * Quaternion.FromToRotation(Vector2.up, d.ToVector());

                doors[(int)d] = door;
            }
            else if (doors[(int)d] != null && (loadedRoom.doorMask & mask) == 0)
            {
                Destroy(doors[(int)d]);
                doors[(int)d] = null;
            }
        }
    }
}
