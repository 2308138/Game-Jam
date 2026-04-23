using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerGrapple : MonoBehaviour
{
    [Header("Grapple Settings")]
    public float maxDistance = 0F;
    public float pullSpeed = 0F;
    public LayerMask grappleLayer;
    private bool isGrappling = false;
    private Vector2 targetPoint;

    [Header("Cooldown Settings")]
    public float grappleCooldown = 0F;
    private float cooldownTimer = 0F;
    private bool isOnCooldown = false;

    [Header("Death Settings")]
    public GameObject deathPrefab;
    public GameObject enemyDeathPrefab;

    // --- PRIVATE REFERENCES --- //
    private LineRenderer lr;
    private Rigidbody2D rb;
    private PlayerInputs controls;
    private PlayerEvolution evo;
    private TrailRenderer tr;

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
        rb = GetComponent<Rigidbody2D>();
        controls = new PlayerInputs();
        evo = GetComponent<PlayerEvolution>();
        tr = GetComponent<TrailRenderer>();

        controls.Player.Grapple.started += ctx => StartGrapple();
        controls.Player.Grapple.canceled += ctx => StopGrapple();
    }

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    private void StartGrapple()
    {
        if (isGrappling || isOnCooldown) return;

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = (mousePos - (Vector2)transform.position).normalized;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, maxDistance, grappleLayer);

        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Enemy") || hit.collider.CompareTag("Orb"))
            {
                targetPoint = (Vector2)hit.collider.transform.position + (dir * 0.5F);
            }
            else
            {
                targetPoint = hit.point;
            }

            isGrappling = true;
            lr.enabled = true;

            GetComponent<PlayerMovement>().ToggleMovement(false);
        }
    }

    private void StopGrapple()
    {
        if (!isGrappling) return;

        isGrappling = false;
        lr.enabled = false;
        isOnCooldown = true;
        cooldownTimer = grappleCooldown;

        Vector2 flingDir = (targetPoint - (Vector2)transform.position).normalized;
        rb.linearVelocity = flingDir * (pullSpeed * 0.8F);

        GetComponent<PlayerMovement>().ToggleMovement(true);
    }

    public void ResetGrapple()
    {
        isOnCooldown = false;
        cooldownTimer = 0F;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 10F);
    }

    private void Update()
    {
        if (isDead) return;

        if (isGrappling)
        {
            rb.linearVelocity = Vector2.zero;

            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, targetPoint);

            transform.position = Vector2.MoveTowards(transform.position, targetPoint, pullSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, targetPoint) < 0.5F)
            {
                StopGrapple();
            }
        }

        else if (isOnCooldown)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0F)
            {
                isOnCooldown = false;
            }
        }

        tr.emitting = isGrappling || GetComponent<PlayerMovement>().isDashing;
    }

    private void OnTriggerEnter2D (Collider2D col)
    {
        if (isGrappling && col.CompareTag("Enemy") || col.CompareTag("Orb"))
        {
            evo.GainBiomass(1);
            if (enemyDeathPrefab != null)
            {
                Instantiate(enemyDeathPrefab, col.transform.position, Quaternion.identity);        
            }

            Destroy(col.gameObject);

            isGrappling = false;
            lr.enabled = false;

            ResetGrapple();
            GetComponent<PlayerMovement>().ToggleMovement(true);
        }
        else if (col.CompareTag("Bullet"))
        {
            Die();
        }
        
    }

    private void Die()
    {
        if (isDead) return;

        StartCoroutine(DeathSequence());
    }

    private bool isDead = false;

    private IEnumerator DeathSequence()
    {
        isDead = true;
        Debug.Log("Monster has been slain!");

        if (deathPrefab != null)
        {
            Instantiate(deathPrefab, transform.position, Quaternion.identity);
        }

        Time.timeScale = 0F;
        var impulse = GetComponent<Unity.Cinemachine.CinemachineImpulseSource>();
        if (impulse != null)
        {
            impulse.GenerateImpulse(Vector3.one * 2F);
        }

        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = Color.red;
        }

        yield return new WaitForSecondsRealtime(0.5F);
        Time.timeScale = 1F;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public bool IsGrappleState()
    {
        return isGrappling;
    }
}