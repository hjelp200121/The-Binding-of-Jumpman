using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class EnemySpawner : DungeonObject
{
    public Enemy enemyPrefab;

    SpriteRenderer debugRenderer;
    Sprite oldDebugSprite;

    void Awake()
    {
        debugRenderer = GetComponent<SpriteRenderer>();
        UpdateDebugSprite();
        DisableDebug();
    }

    void Update()
    {
        if (debugRenderer.enabled)
        {
            if (oldDebugSprite != debugRenderer.sprite)
            {
                UpdateDebugSprite();
            }
        }
    }

    void UpdateDebugSprite()
    {
        if (enemyPrefab != null)
        {
            debugRenderer.sprite = enemyPrefab.GetComponent<SpriteRenderer>().sprite;
            oldDebugSprite = debugRenderer.sprite;
        }
        else
        {
            oldDebugSprite = null;
        }
    }

    public void SpawnEnemy()
    {
        Enemy enemy = Instantiate<Enemy>(enemyPrefab, room.transform);
        enemy.room = room;
        enemy.transform.position = transform.position;
        enemy.Load();
        room.contents.objectAddQueue.Enqueue(enemy);
        room.contents.enemyCount++;
        room.UpdateCleared();
    }

    public override void Load()
    {
        if (!room.discovered || !room.beaten)
        {
            SpawnEnemy();
        }
        gameObject.SetActive(true);
    }
    public override void UnLoad()
    {
        gameObject.SetActive(false);
    }

    public override void EnableDebug()
    {
        debugRenderer.enabled = true;
    }

    public override void DisableDebug()
    {
        debugRenderer.enabled = false;
    }
}
