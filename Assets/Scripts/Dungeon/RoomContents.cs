using System.Collections;
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

    public int enemyCount = 0;


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
        if (objectAddQueue == null)
        {
            objectAddQueue = new Queue<DungeonObject>();
        }
        if (objectRemoveQueue == null)
        {
            objectRemoveQueue = new Queue<DungeonObject>();
        }
        foreach (DungeonTile tile in tiles)
        {
            if (tile != null)
            {
                tile.Load();
            }
        }

        while (objectRemoveQueue.Count > 0)
        {
            DungeonObject removeObject = objectRemoveQueue.Dequeue();
            objects.Remove(removeObject);
        }
        while (objectAddQueue.Count > 0)
        {
            DungeonObject addObject = objectAddQueue.Dequeue();
            objects.Add(addObject);
        }
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
        if (objectAddQueue == null)
        {
            objectAddQueue = new Queue<DungeonObject>();
        }
        if (objectRemoveQueue == null)
        {
            objectRemoveQueue = new Queue<DungeonObject>();
        }
        foreach (DungeonTile tile in tiles)
        {
            if (tile != null)
            {
                tile.UnLoad();
            }
        }

        while (objectAddQueue.Count > 0)
        {
            DungeonObject addObject = objectAddQueue.Dequeue();
            objects.Add(addObject);
        }
        while (objectRemoveQueue.Count > 0)
        {
            DungeonObject removeObject = objectRemoveQueue.Dequeue();
            objects.Remove(removeObject);
        }
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
