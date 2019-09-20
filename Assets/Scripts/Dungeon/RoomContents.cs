﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoomContents
{
    public int width, height;
    [SerializeField]
    public Room room;

    public DungeonTile[] tiles;

    public List<DungeonObject> objects;

    public Queue<DungeonObject> objectAddQueue;
    public Queue<DungeonObject> objectRemoveQueue;


    public RoomContents(Room room, int width, int height)
    {
        this.room = room;
        this.width = width;
        this.height = height;
        this.tiles = new DungeonTile[width * height];
        this.objects = new List<DungeonObject>();
    }

    public DungeonTile GetTile(int x, int y)
    {
        return tiles[x + y * width];
    }

    public void SetTile(DungeonTile tile, int x, int y)
    {
        tiles[x + y * width] = tile;

    }

    public void Load()
    {
        foreach (DungeonTile tile in tiles)
        {
            if (tile != null)
            {
                tile.Load();
            }
        }

        objectAddQueue = new Queue<DungeonObject>();

        foreach (DungeonObject dungeonObject in objects)
        {
            dungeonObject.Load();
        }
        while (objectAddQueue.Count > 0)
        {
            DungeonObject addObject = objectAddQueue.Dequeue();
            objects.Add(addObject);
        }
    }

    public void UnLoad()
    {
        foreach (DungeonTile tile in tiles)
        {
            if (tile != null)
            {
                tile.UnLoad();
            }
        }

        objectRemoveQueue = new Queue<DungeonObject>();
        
        foreach (DungeonObject dungeonObject in objects)
        {
            dungeonObject.UnLoad();
        }
        while (objectRemoveQueue.Count > 0)
        {
            DungeonObject removeObject = objectRemoveQueue.Dequeue();
            objects.Remove(removeObject);
        }
    }
}
