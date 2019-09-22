using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(SpriteRenderer))]
public class TrapDoor : DungeonTile
{
    public string nextScene;
    public Collider2D openCollider;
    public Sprite openSprite;
    public Sprite closedSprite;

    SpriteRenderer spriteRenderer;

    public void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = closedSprite;
        openCollider.enabled = false;
    }

    public virtual void Open()
    {
        spriteRenderer.sprite = openSprite;
        openCollider.enabled = true;
    }

    public override void OnRoomClear()
    {
        Open();
    }

    public override void Load()
    {
        gameObject.SetActive(true);
    }
    public override void UnLoad()
    {
        gameObject.SetActive(false);
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            room.UnLoad();
            SceneManager.LoadScene(nextScene);
        }
    }
}
