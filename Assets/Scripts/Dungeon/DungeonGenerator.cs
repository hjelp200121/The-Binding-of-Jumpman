using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    /* The root-object of the dungeon. */
    public Dungeon dungeon;
    public BranchInfo[] branchingInfo;
    [NamedArray(typeof(RoomTypes))]
    public RoomCollection[] roomPrefabs;
    [NamedArray(typeof(RoomTypes))]
    public Door[] doorPrefabs;

    /* Generate the dungeon. */
    void Awake()
    {
        List<RoomInfo> roomInfos = GenerateRoomInfo();
        List<Room> rooms = InstantiateRooms(roomInfos);
        dungeon.rooms = rooms;
    }

    public List<RoomInfo> GenerateRoomInfo()
    {
        List<RoomInfo> rooms = new List<RoomInfo>();

        /* Add the first room of the dungeon. */
        rooms.Add(new RoomInfo(0, 0, RoomTypes.START));

        /* Generate the different branches of the dungeon. */
        for (int i = 0; i < branchingInfo.Length; i++)
        {
            BranchInfo branchInfo = branchingInfo[i];
            int branchLength = Random.Range(branchInfo.minLength, branchInfo.maxLength+1);
            RoomTypes goal = branchInfo.goal;
            List<Vector2> occupied = GetOccupiedCoordinates(rooms);
            /* Choose a random room to branch off. */
            RoomInfo startRoom = rooms[Random.Range(0, rooms.Count)];
            List<RoomInfo> branch = DungeonBranch.GenerateBranch(
                new Vector2(startRoom.x, startRoom.y),
                branchLength, goal, occupied);

            /* If the branch generation failed, try again. */
            if (branch == null)
            {
                i--;
                continue;
            }

            rooms.AddRange(branch);
        }
        /* Add connections between neighboring rooms. */
        AddRoomNeighbors(rooms);

        return rooms;
    }

    public List<Room> InstantiateRooms(List<RoomInfo> roomInfos)
    {
        List<Room> rooms = new List<Room>();

        /* Instantiate all of the rooms. */
        for (int i = 0; i < roomInfos.Count; i++)
        {
            RoomInfo roomInfo = roomInfos[i];
            Room room = spawnRoom(roomInfo);

            /* Loop through the directions to add doors. */
            for (
                Directions d = Directions.NORTH;
                d <= Directions.WEST;
                d++
            )
            {
                /* Check if the room is supposed to have a neighbor in the current direction. */
                if (roomInfo.neighbors[(int)d] != null)
                {
                    int dx = room.x + (int)d.ToVector().x;
                    int dy = room.y + (int)d.ToVector().y;
                    /* See if that neighbor has been instantiated. */
                    Room neighbor = rooms.Find(r => r.x == dx && r.y == dy);
                    if (neighbor != null)
                    {
                        CreateDoorPair(room, neighbor, d);
                    }
                }
            }

            rooms.Add(room);
        }
        return rooms;
    }

    Room spawnRoom(RoomInfo roomInfo)
    {
        int doorMask = 0;
        /* Construct the door mask.
         * This is used to filter out invalid rooms. */
        for (
            Directions d = Directions.NORTH;
            d <= Directions.WEST;
            d++
        )
        {
            if (roomInfo.neighbors[(int)d] != null)
            {
                doorMask += 1 << (int)d;
            }
        }

        RoomCollection rooms = roomPrefabs[(int)roomInfo.type];
        /* Filter out all rooms that do not match the doors needed. */
        List<Room> validRooms = rooms.rooms.FindAll(r => ((r.doorMask ^ doorMask) & doorMask) == 0);
        /* Select a random room to instantiate. */
        int index = Random.Range(0, validRooms.Count);
        Room room = Instantiate<Room>(validRooms[index], dungeon.transform);
        room.x = roomInfo.x;
        room.y = roomInfo.y;
        room.type = roomInfo.type;

        /* Remove the colliders from the room where there are doors instead. */
        for (
            Directions d = Directions.NORTH;
            d <= Directions.WEST;
            d++
        )
        {
            bool noDoor = (doorMask & 1 << (int)d) == 0;
            /* If there is no door, the collider should be enabled.
             * Otherwise, it should be diabled.
             */
            room.doorwayColliders[(int)d].enabled = noDoor;
        }


        room.transform.position = new Vector2(roomInfo.x * room.width, roomInfo.y * room.height);
        room.UnLoad();

        return room;
    }

    List<Vector2> GetOccupiedCoordinates(List<RoomInfo> roomInfos)
    {
        List<Vector2> occupiedCoordinates = new List<Vector2>();
        foreach (RoomInfo roomInfo in roomInfos)
        {
            Vector2 coordinates = new Vector2(
                roomInfo.x,
                roomInfo.y
            );
            occupiedCoordinates.Add(coordinates);

            /* Special rooms should not have more than 1 adjacent room. */
            if (roomInfo.type.IsSpecial())
            {
                for (Directions d = Directions.NORTH;
                     d <= Directions.WEST;
                     d++
                )
                {
                    occupiedCoordinates.Add(coordinates + d.ToVector());
                }
            }
        }
        return occupiedCoordinates;
    }

    /* Add references to neighboring rooms. */
    void AddRoomNeighbors(List<RoomInfo> roomInfos)
    {
        foreach (RoomInfo roomInfo in roomInfos)
        {
            for (
                Directions d = Directions.NORTH;
                d <= Directions.WEST;
                d++
            )
            {
                /* If the room already has a neighbor in this direction,
                 * skip it. */
                if (roomInfo.neighbors[(int)d] != null)
                {
                    continue;
                }
                int dx = roomInfo.x + (int)d.ToVector().x;
                int dy = roomInfo.y + (int)d.ToVector().y;

                RoomInfo neighbor = roomInfos.Find(r => r.x == dx && r.y == dy);
                if (neighbor != null)
                {
                    roomInfo.neighbors[(int)d] = neighbor;
                    neighbor.neighbors[(int)d.GetOpposite()] = roomInfo;
                }
            }
        }
    }

    /* Create a pair of doors between two rooms.
     * `direction` should be the direction from `r1` to `r2`.
     */
    void CreateDoorPair(Room r1, Room r2, Directions direction)
    {
        if (r1.doors[(int)direction] != null)
        {
            /* If there already is a door in this direction, abort.
             * As doors are always created in pairs, there should never
             * be any missing door connections. */
            return;
        }
        Door doorPrefab = r1.type.IsSpecial() ? doorPrefabs[(int)r1.type] : doorPrefabs[(int)r2.type];
        Door d1 = Instantiate<Door>(doorPrefab, r1.transform);
        Door d2 = Instantiate<Door>(doorPrefab, r2.transform);
        d1.room = r1;
        d2.room = r2;
        d1.connection = d2;
        d2.connection = d1;

        r1.doors[(int)direction] = d1;
        d1.direction = direction;
        PositionDoor(d1, r1, direction);
        d1.transform.rotation = Quaternion.FromToRotation(Vector2.up, direction.ToVector());

        direction = direction.GetOpposite();

        r2.doors[(int)direction] = d2;
        d2.direction = direction;
        PositionDoor(d2, r2, direction);
        d2.transform.rotation = Quaternion.FromToRotation(Vector2.up, direction.ToVector());
    }

    /* Position the door to to match the direction it is facing. */
    void PositionDoor(Door door, Room room, Directions direction)
    {
        switch (direction)
        {
            case Directions.NORTH:
            case Directions.SOUTH:
                door.transform.position += (Vector3)direction.ToVector() * (room.height / 2f - .5f);
                break;
            case Directions.EAST:
            case Directions.WEST:
                door.transform.position += (Vector3)direction.ToVector() * (room.width / 2f - .5f);
                break;
        }
    }
}

/* Helper class for generating branches of the dungeon. */
static class DungeonBranch
{
    public static List<RoomInfo> GenerateBranch(Vector2 start, int maxRoomCount, RoomTypes goal, List<Vector2> occupied)
    {
        List<RoomInfo> rooms = new List<RoomInfo>();
        RoomInfo current = new RoomInfo((int)start.x, (int)start.y, RoomTypes.COMMON);
        RoomInfo last;
        Directions direction;

        /* Is the current room special? */
        RoomTypes roomType;
        int roomCount = 1;

        /* Generate the basic rooms. */
        while (roomCount <= maxRoomCount)
        {
            /* If it is the last room, change the room type. */
            if (roomCount == maxRoomCount)
            {
                roomType = goal;
            }
            else
            {
                roomType = RoomTypes.COMMON;
            }
            int dx = 0, dy = 0;

            /* Check if there are no valid directions for the room. */
            List<Directions> validDirections = new List<Directions>();
            for (
                Directions d = Directions.NORTH;
                d <= Directions.WEST;
                d++
            )
            {
                dx = current.x + (int)d.ToVector().x;
                dy = current.y + (int)d.ToVector().y;
                if (!rooms.Any(r => r.x == dx && r.y == dy) &&
                            !occupied.Any(r => r.x == dx && r.y == dy))
                {
                    validDirections.Add(d);
                }
            }

            /* If the room is special, check if there are any adjacent rooms to the valid direcitons. */
            if (roomType.IsSpecial())
            {
                List<Directions> notActuallyValidDirections = new List<Directions>();
                foreach (Directions validDirection in validDirections)
                {
                    for (
                        Directions d = Directions.NORTH;
                        d <= Directions.WEST;
                        d++
                    )
                    {
                        /* Do not check if a room is in the opposite direction of the valid direction.
                        * This is because it simply points to the `current` position, and will therfore always exist. */
                        if (d == validDirection.GetOpposite())
                        {
                            continue;
                        }
                        dx = current.x + (int)validDirection.ToVector().x + (int)d.ToVector().x;
                        dy = current.y + (int)validDirection.ToVector().y + (int)d.ToVector().y;
                        if (rooms.Any(r => r.x == dx && r.y == dy) ||
                                    occupied.Any(r => r.x == dx && r.y == dy))
                        {
                            /* If a room would adjacent to the special room, it is not a valid direction. */
                            notActuallyValidDirections.Add(validDirection);
                        }
                    }
                }
                validDirections = validDirections.Except(notActuallyValidDirections).ToList();
            }

            if (validDirections.Count == 0)
            {
                Debug.Log("Dungeon generation algorith, got stuck" +
                          " Aborting branch.");
                /* If no valid direction could be found, abort. */
                return null;
            }

            direction = validDirections[Random.Range(0, validDirections.Count)];
            dx = current.x + (int)direction.ToVector().x;
            dy = current.y + (int)direction.ToVector().y;

            last = current;
            current = new RoomInfo(dx, dy, roomType);
            rooms.Add(current); /* Update the room count. */
            roomCount++;
        }

        return rooms;
    }
}

[System.Serializable]
public struct BranchInfo
{
    public int minLength;
    public int maxLength;
    public RoomTypes goal;
}

public class RoomInfo
{
    public int x, y;
    public RoomTypes type;
    public RoomInfo[] neighbors;

    public RoomInfo(int x, int y, RoomTypes type)
    {
        this.x = x;
        this.y = y;
        this.type = type;
        this.neighbors = new RoomInfo[4];
    }
}
