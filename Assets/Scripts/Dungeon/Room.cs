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
    public float rewardChance;
    public DungeonEntity[] rewards;

    public SpriteRenderer[] wallDecorationsRenderers;
    /* The four colliders to use when no door is at the wall. */
    public Collider2D[] doorwayColliders;
    public Door[] doors;

    public RoomContents contents;

    public PlayerController player;
    public bool cleared;
    public bool discovered;
    public bool beaten;


    void Awake()
    {
        cleared = false;
        discovered = false;
        beaten = true;
        if (contents.room == null)
        {
            contents = new RoomContents(this, gridWidth, gridHeight);
        }
    }

    public void Load(PlayerController player)
    {
        /* Apply random decorations. */
        if (!discovered && wallDecorationsRenderers.Length > 0)
        {
            foreach (SpriteRenderer wallDec in wallDecorationsRenderers)
            {
                wallDec.flipX = Random.Range(0, 2) == 1;
                wallDec.flipY = Random.Range(0, 2) == 1;
            }
        }

        gameObject.SetActive(true);
        this.player = player;
        player.currentRoom = this;
        contents.Load();
        UpdateCleared();
        discovered = true;
    }

    public void UnLoad()
    {
        contents.UnLoad();
        if (player != null)
        {
            player.currentRoom = null;
        }
        this.player = null;
        gameObject.SetActive(false);
    }

    public void UpdateCleared()
    {
        if (contents.enemyCount > 0)
        {
            beaten = false;
            cleared = false;
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
            if (!cleared)
            {
                OnClear();
                cleared = true;
            }
            if (!beaten)
            {
                OnBeaten();
                beaten = true;
            }
            foreach (Door door in doors)
            {
                if (door != null)
                {
                    door.Open();
                }
            }

        }
    }
    public void Enter(PlayerController player)
    {
        Load(player);
        player.OnRoomEnter(this);
    }

    public void Leave(PlayerController player)
    {
        player.OnRoomLeave(this);
        UnLoad();
    }

    void OnBeaten()
    {
        player.OnRoomBeaten();
        foreach (DungeonObject dungeonObject in contents.objects)
        {
            dungeonObject.OnRoomBeaten();
        }
        foreach (DungeonTile dungeonTile in contents.tiles)
        {
            if (dungeonTile != null)
            {
                dungeonTile.OnRoomBeaten();
            }
        }

        if (Random.Range(0f, 1f) <= rewardChance)
        {
            int x, y;
            x = gridWidth / 2;
            y = gridHeight / 2;

            while (contents.GetTile(x, y) != null) {
                Directions rand = (Directions)Random.Range(0, 5);
                if ((x += (int)rand.ToVector().x) >= gridWidth || x < 0)
                {
                    x -= (int)rand.ToVector().x;
                }
                if ((y += (int)rand.ToVector().y) >= gridHeight || y < 0)
                {
                    y -= (int)rand.ToVector().y;
                }
            }

            DungeonEntity rewardPrefab = rewards[Random.Range(0, rewards.Length)];
            DungeonEntity reward = Instantiate<DungeonEntity>(rewardPrefab, transform);
            Vector2 pos = new Vector2(x - gridWidth / 2, y - gridHeight / 2);
            reward.transform.position = transform.TransformPoint(pos);

            reward.room = this;

            contents.objectAddQueue.Enqueue(reward);
        }
    }

    void OnClear()
    {
        player.OnRoomClear();
        foreach (DungeonObject dungeonObject in contents.objects)
        {
            dungeonObject.OnRoomClear();
        }
        foreach (DungeonTile dungeonTile in contents.tiles)
        {
            if (dungeonTile != null)
            {
                dungeonTile.OnRoomClear();
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