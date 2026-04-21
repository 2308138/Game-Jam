using UnityEngine;

public class PlayerEvolution : MonoBehaviour
{
    [Header("Player Stats Settings")]
    public int currentBiomass = 0;
    public int biomassNeeded = 0;
    public int currentLevel = 0;

    [Header("Growth Multiplier Settings")]
    public float speedMultiplier = 0F;
    public float distanceMultiplier = 0F;
    public float pullMultiplier = 0F;

    // Private References
    private PlayerMovement movement;
    private PlayerGrapple grapple;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        grapple = GetComponent<PlayerGrapple>();
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
        currentLevel++;
        currentBiomass = 0;
        biomassNeeded = Mathf.RoundToInt(biomassNeeded * 1.5F);

        movement.moveSpeed *= speedMultiplier;
        grapple.maxDistance *= distanceMultiplier;
        grapple.pullSpeed *= pullMultiplier;

        Debug.Log($"EVOLVED! Level: {currentLevel}. Speed: {movement.moveSpeed}. Reach: {grapple.maxDistance}. Grapple Speed: {grapple.pullSpeed}.");
    }
}