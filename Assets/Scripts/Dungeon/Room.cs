using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Room : MonoBehaviour
{
    static int gridHeight = 7;
    static int gridWidth = 13;

    public float width, height;
    public int x, y;
    /* Four bits representing whether or not a door is allowed in that direction. */
    public int doorMask = 0;
    public RoomTypes types;
    public Door[] doors;

    public RoomContents contents;


    void Awake () {
        if (contents.room == null) {
            Debug.Log("bruh");
            contents = new RoomContents(this, gridWidth, gridHeight);
        } else {
            Debug.Log(contents);
        }
    }
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