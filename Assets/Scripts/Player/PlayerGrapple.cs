using UnityEngine;

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

    // Private References
    private LineRenderer lr;
    private Rigidbody2D rb;
    private PlayerInputs controls;
    private PlayerEvolution evo;

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
        rb = GetComponent<Rigidbody2D>();
        controls = new PlayerInputs();
        evo = GetComponent<PlayerEvolution>();

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
    }

    private void OnTriggerEnter2D (Collider2D col)
    {
        if (isGrappling && col.CompareTag("Enemy") || col.CompareTag("Orb"))
        {
            evo.GainBiomass(1);
            Destroy(col.gameObject);          
        }

        isGrappling = false;
        lr.enabled = false;

        ResetGrapple();
        GetComponent<PlayerMovement>().ToggleMovement(true);
    }
}