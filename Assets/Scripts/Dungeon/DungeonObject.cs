using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class DungeonObject : MonoBehaviour
{
    public Room room;

    public abstract void Load();
    public abstract void UnLoad();

    public virtual void OnRoomClear()
    {
        return;
    }
    public virtual void OnRoomBeaten()
    {
        return;
    }

    public virtual void EnableDebug()
    {
        return;
    }
    public virtual void DisableDebug()
    {
        return;
    }
}
