using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class DungeonObject : MonoBehaviour
{
    public Room room;

    public abstract void Load();
    public abstract void UnLoad();
}
