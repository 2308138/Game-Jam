using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class ArenaManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject enemyPrefab;
    public GameObject warningPrefab;
    public BoxCollider2D spawnArea;
    public float spawnInterval = 0F;
    public float warningDuration = 0F;

    [Header("Survival Settings")]
    public float timeToSurvive = 0F;
    private float currentTime = 0F;
    private bool isGameOver = false;

    [Header("UI Settings")]
    public TextMeshProUGUI timerText;

    private void Start()
    {
        currentTime = timeToSurvive;
        InvokeRepeating(nameof(StartSpawnSequence), 2F, spawnInterval);
    }

    private void Update()
    {
        if (isGameOver) return;

        currentTime -= Time.deltaTime;
        if (timerText != null)
        {
            timerText.text = Mathf.CeilToInt(currentTime).ToString();
        }

        if (currentTime <= 0)
        {
            Win();
        }
    }

    private void StartSpawnSequence()
    {
        if (isGameOver) return;

        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        Vector2 spawnPos = GetRandomPointInCollider(spawnArea);
        GameObject warning = Instantiate(warningPrefab, spawnPos, Quaternion.identity);

        yield return new WaitForSeconds(warningDuration);
        Destroy(warning);
        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
    }

    private Vector2 GetRandomPointInCollider(BoxCollider2D col)
    {
        Bounds bounds = col.bounds;
        return new Vector2(Random.Range(bounds.min.x, bounds.max.x), Random.Range(bounds.min.y, bounds.max.y));
    }

    private void Win()
    {
        isGameOver = true;
        CancelInvoke(nameof(StartSpawnSequence));
        Debug.Log("Arena Cleared!");

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject e in enemies) Destroy(e);
    }
}