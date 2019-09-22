using UnityEngine;


/* Taken from https://forum.unity.com/threads/need-rigidbody2d-addexplosionforce.212173/#post-1426983 */
public static class Rigidbody2DExtension
{
    public static void AddExplosionForce(this Rigidbody2D body, float explosionForce, Vector3 explosionPosition, float explosionRadius)
    {
        var dir = (body.transform.position - explosionPosition);
        float wearoff = 1 - (dir.magnitude / explosionRadius);
        body.AddForce(dir.normalized * explosionForce * wearoff, ForceMode2D.Impulse);
    }
}
