using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class PlayerEvolution : MonoBehaviour
{
    [Header("Public References")]
    public CinemachineCamera virtualCamera;

    [Header("Player Stats Settings")]
    public int currentBiomass = 0;
    public int biomassNeeded = 0;
    public int currentLevel = 0;
    public int maxLevel = 0;

    [Header("Growth Multiplier Settings")]
    public float speedMultiplier = 0F;
    public float distanceMultiplier = 0F;
    public float pullMultiplier = 0F;

    [Header("Evolution Settings")]
    public Color baseLevelColor = Color.white;
    public Color maxLevelColor = Color.white;
    public float baseOrthoSize = 0F;
    public float zoomPerLevel = 0F;
    private Color originalColor;

    // --- PRIVATE REFERENCES --- //
    private PlayerMovement movement;
    private PlayerGrapple grapple;
    private LineRenderer lr;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        grapple = GetComponent<PlayerGrapple>();
        lr = GetComponent<LineRenderer>();
    }

    public void GainBiomass(int amount)
    {
        currentBiomass += amount;
        if (currentBiomass >=  biomassNeeded)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        if (currentLevel < maxLevel)
        {
            currentLevel++;
            currentBiomass = 0;
            biomassNeeded = Mathf.RoundToInt(biomassNeeded * 1.5F);

            movement.moveSpeed *= speedMultiplier;
            grapple.maxDistance *= distanceMultiplier;
            grapple.pullSpeed *= pullMultiplier;

            float t = (float)(currentLevel - 1) / 9F;
            Color newColor = Color.Lerp(baseLevelColor, maxLevelColor, t);
            lr.startColor = newColor;
            lr.endColor = newColor;

            if (virtualCamera != null)
            {
                virtualCamera.Lens.OrthographicSize = baseOrthoSize + (currentLevel * zoomPerLevel);
            }

            StartCoroutine(EvolutionDelay());
            StartCoroutine(EvolutionFlash());

            Debug.Log($"EVOLVED! Level: {currentLevel}. Speed: {movement.moveSpeed}. Reach: {grapple.maxDistance}. Grapple Speed: {grapple.pullSpeed}.");
        }
    }

    private IEnumerator EvolutionFlash()
    {
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        originalColor = sr.color;

        sr.color = Color.white;

        yield return new WaitForSeconds(0.1F);
        sr.color = originalColor;
    }

    private IEnumerator EvolutionDelay()
    {
        Time.timeScale = 0F;

        yield return new WaitForSeconds(0.05F);
        Time.timeScale = 1F;
    }
}