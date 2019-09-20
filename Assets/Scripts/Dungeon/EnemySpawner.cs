using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class EnemySpawner : DungeonObject
{
    public Enemy enemyPrefab;

    SpriteRenderer debugRenderer;

    void Awake()
    {
        debugRenderer = GetComponent<SpriteRenderer>();
        if (enemyPrefab != null) {
            debugRenderer.sprite = enemyPrefab.renderer.sprite;
        }
        debugRenderer.enabled = false;
    }

    public void SpawnEnemy() {
        Enemy enemy = Instantiate<Enemy>(enemyPrefab, room.transform);
        enemy.transform.position = transform.position;
        room.contents.objects.Add(enemy);
    }

    public override void Load()
    {
        SpawnEnemy();
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
