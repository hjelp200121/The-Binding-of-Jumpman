using UnityEngine;

public enum Directions
{
    NORTH, EAST, SOUTH, WEST
}

static class RoomDirectionsExtentionMethods
{
    public static Directions GetOpposite(this Directions direction) {
        if (direction > Directions.EAST) {
            return (Directions)((int) direction - 2);
        } else {
            return (Directions)((int) direction + 2);
        }
    }

    public static Directions Next(this Directions direction) {
        if (direction == Directions.WEST) {
            return Directions.NORTH;
        } else {
            return (Directions) (direction++);
        }
    }

    public static Directions Prev(this Directions direction) {
        if (direction == Directions.NORTH) {
            return Directions.WEST;
        } else {
            return (Directions) (direction--);
        }
    }

    public static Vector2 ToVector(this Directions direction)
    {
        switch (direction)
        {
            case Directions.NORTH: return Vector2.up;
            case Directions.EAST: return Vector2.right;
            case Directions.SOUTH: return Vector2.down;
            case Directions.WEST: return Vector2.left;
            default: return Vector2.zero;
        }
    }
}
