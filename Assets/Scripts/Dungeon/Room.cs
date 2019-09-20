using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Room : MonoBehaviour
{
    public static int gridHeight = 7;
    public static int gridWidth = 13;

    public float width, height;
    public float wallThickness = 1f;
    public int x, y;
    /* Four bits representing whether or not a door is allowed in that direction. */
    public int doorMask = 0;
    public RoomTypes type;
    /* The four colliders to use when no door is at the wall. */
    public Collider2D[] doorwayColliders;
    public Door[] doors;

    public RoomContents contents;

    public PlayerController player;


    void Awake()
    {
        if (contents.room == null)
        {
            contents = new RoomContents(this, gridWidth, gridHeight);
        }
    }

    public void Load(PlayerController player)
    {
        gameObject.SetActive(true);
        this.player = player;
        contents.Load();
    }

    public void UnLoad()
    {
        contents.UnLoad();
        this.player = null;
        gameObject.SetActive(false);
    }

    public void UpdateDoors()
    {
        if (contents.enemyCount > 0)
        {
            foreach (Door door in doors)
            {
                if (door != null)
                {
                    door.Close();
                }
            }
        }
        else
        {
            foreach (Door door in doors)
            {
                if (door != null)
                {
                    door.Open();
                }
            }

        }
    }
}

/* A collection of rooms. Used by the dungeon generator. */
[System.Serializable]
public class RoomCollection
{
    public List<Room> rooms;
}

public enum RoomTypes
{
    START, COMMON, TREASURE, BOSS
}

static class RoomTypesExtentionMethods
{
    public static bool IsSpecial(this RoomTypes type)
    {
        return type == RoomTypes.TREASURE ||
               type == RoomTypes.BOSS;
    }
}