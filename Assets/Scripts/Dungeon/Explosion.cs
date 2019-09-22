using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public static void Explode(DungeonEntity source, float explosionDamage, float explosionForce,
                        Vector2 explosionPosition, float explosionRadius)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(explosionPosition, explosionRadius);
        foreach (Collider2D hit in hits)
        {

            /* Get all MonoBehaviour components of the hit. */
            var scripts = hit.GetComponents<MonoBehaviour>();
            /* Select all scripts that implement IExplodable. */
            IExplodable[] interfaceScripts = (
                from a in scripts where a.GetType().GetInterfaces().Any(
                    k => k == typeof(IExplodable)) select (IExplodable)a).ToArray();
            
            foreach (IExplodable explodable in interfaceScripts)
            {
                explodable.BlowUp(source, explosionDamage);
            }

            Rigidbody2D rigidbody;
            if ((rigidbody = hit.GetComponent<Rigidbody2D>()) != null)
            {
                rigidbody.AddExplosionForce(explosionForce, explosionPosition, explosionRadius);
            }
        }
    }
}
