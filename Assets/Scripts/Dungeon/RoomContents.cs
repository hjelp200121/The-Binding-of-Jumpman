using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoomContents
{
    public int width, height;
    public Room room;

    public DungeonTile[] tiles;

    public List<DungeonEntity> entities;


    public RoomContents(Room room, int width, int height)
    {
        this.room = room;
        this.width = width;
        this.height = height;
        this.tiles = new DungeonTile[width * height];
        this.entities = new List<DungeonEntity>();
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
        foreach (DungeonEntity entity in entities) {
            entity.Load();
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
        foreach (DungeonEntity entity in entities) {
            entity.UnLoad();
        }
    }
}
