using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveBomb : DungeonEntity
{
    public ExplosionEffect explosionEffectPrefab;
    public float explosionDamage;
    public float explosionRadius;
    public float explosionForce;
    public float fuseTime;
    float fuse;

    public override void Awake()
    {
        base.Awake();
        fuse = 0;
        GetComponent<Collider2D>().isTrigger = true;
    }

    public void Update()
    {
        if ((fuse += Time.deltaTime) >= fuseTime)
        {
            Explode();
            if (room != null)
            {
                room.contents.objects.Remove(this);
            }
            Destroy(gameObject);
        }
    }

    public void Explode()
    {
        Explosion.Explode(this, explosionDamage, explosionForce,
                          transform.position, explosionRadius);
        ExplosionEffect exp = Instantiate<ExplosionEffect>(explosionEffectPrefab);
        exp.transform.position = transform.position;
        exp.transform.localScale *= explosionRadius / exp.radius;
    }

    public override void Load()
    {
        gameObject.SetActive(true);
        fuse = 0;
    }
    public override void UnLoad()
    {
        gameObject.SetActive(false);
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            GetComponent<Collider2D>().isTrigger = false;
        }
    }
}
