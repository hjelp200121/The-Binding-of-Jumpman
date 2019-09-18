using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : DungeonTile
{
    public override void Load () {
        gameObject.SetActive(true);
    }
    public override void UnLoad () {
        gameObject.SetActive(false);
    }

}
