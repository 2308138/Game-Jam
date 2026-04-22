using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float speed = 0F;
    public float lifeTime = 0F;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
        GetComponent<Rigidbody2D>().linearVelocity = transform.right * speed;
    }
}