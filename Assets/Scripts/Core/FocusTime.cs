using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FocusTime : MonoBehaviour
{
    [Header("Time Settings")]
    public float slowTimeScale = 0F;
    public float transitionSpeed = 0F;

    [Header("Post Processing Settings")]
    public Volume volume;

    // --- PRIVATE REFERENCES --- //
    private PlayerMovement movement;
    private PlayerGrapple grapple;
    private PlayerInputs controls;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        grapple = GetComponent<PlayerGrapple>();
        controls = new PlayerInputs();
    }

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    private void Update()
    {
        bool isInputting = controls.Player.Move.ReadValue<Vector2>().sqrMagnitude > 0.1F;
        bool isJumping = controls.Player.Jump.IsPressed();
        bool isGrappling = grapple.IsGrappleState();

        if (isInputting || isJumping || isGrappling)
        {
            Time.timeScale = Mathf.MoveTowards(Time.timeScale, 1F, Time.unscaledDeltaTime * transitionSpeed * 2F);
        }
        else
        {
            Time.timeScale = Mathf.MoveTowards(Time.timeScale, slowTimeScale, Time.unscaledDeltaTime * transitionSpeed);
        }

        Time.fixedDeltaTime = 0.02F * Time.timeScale;
        Time.timeScale = Mathf.Clamp(Time.timeScale, slowTimeScale, 1F);

        if (volume != null && volume.profile.TryGet<Vignette>(out var vignette))
        {
            vignette.intensity.value = Mathf.Lerp(0.5F, 0.2F, Time.timeScale);
        }

        if (volume != null && volume.profile.TryGet<ChromaticAberration>(out var chromatic))
        {
            chromatic.intensity.value = Mathf.Lerp(1F, 0F, Time.timeScale);
        }
    }
}