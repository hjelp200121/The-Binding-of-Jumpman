﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Rock : DungeonTile, IExplodable
{
    public Sprite[] sprites;
    SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Sprite randSprite = sprites[Random.Range(0, sprites.Length)];
        spriteRenderer.sprite = randSprite;
    }

    public void BlowUp(DungeonObject source, float damage)
    {
        room.contents.SetTile(null, x, y);
        Destroy(gameObject);
    }

    public override void Load()
    {
        gameObject.SetActive(true);
    }
    public override void UnLoad()
    {
        gameObject.SetActive(false);
    }

}
