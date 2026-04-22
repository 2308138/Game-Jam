using UnityEngine;

public class TurretAI : MonoBehaviour
{
    [Header("Detection Settings")]
    public float range = 0F;
    public LayerMask playerLayer;
    public LayerMask obstacleLayer;

    [Header("Firing Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 0F;
    private float nextFireTime = 0F;

    // --- PRIVATE REFERENCES --- //
    private Transform player;

    private void Start() => player = GameObject.FindGameObjectWithTag("Player").transform;

    private void Update()
    {
        float distToPlayer = Vector2.Distance(transform.position, player.position);

        if (distToPlayer <= range)
        {
            Vector2 dir = (player.position - transform.position).normalized;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, range, playerLayer | obstacleLayer);

            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0F, 0F, angle);

                if (Time.time >= nextFireTime)
                {
                    Shoot();
                    nextFireTime = Time.time + fireRate;
                }
            }
        }
    }

    private void Shoot()
    {
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }
}