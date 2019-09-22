using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IExplodable
{
    void BlowUp(DungeonObject source, float damage);
}