using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public List<GameObject> commonPlatforms;
    public List<GameObject> uncommonPlatforms;
    public List<GameObject> rarePlatforms;
    public List<GameObject> legendaryPlatforms;

    public int platformCount = 300;
    private TextMeshProUGUI scoreText;
    private TextMeshProUGUI timerText; 
    private TextMeshProUGUI bestTimeText;

    public static GameManager Instance { get; private set; }

    private float highestPointReached = 0f;
    private int score = 0;
    private float survivalTime = 0f;
    private bool timeRunning = true; // Flag to control whether time should be updated
    private bool gameEnded = false; // Flag to prevent further actions after game ends

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        FindUITextMeshes();
        GeneratePlatforms();
        UpdateScoreText();
        UpdateTimerText();
        LoadAndDisplayBestTime();
        gameEnded = false;
        timeRunning = true;
    }

    private void FindUITextMeshes()
    {
        scoreText = GameObject.Find("ScoreText").GetComponent<TextMeshProUGUI>();
        timerText = GameObject.Find("TimerText").GetComponent<TextMeshProUGUI>();
        bestTimeText = GameObject.Find("BestTimeText").GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (timeRunning)
            UpdateSurvivalTimer();
    }

    void GeneratePlatforms()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        Vector3 spawnPosition = new Vector3();
        float spawnHeight = 0f;
        while (spawnHeight < 100f)
        {
            spawnHeight += Random.Range(0.5f, 2f);
            spawnPosition.y = spawnHeight;
            spawnPosition.x = Random.Range(-5f, 5f);

            GameObject platformPrefab;
            float randomValue = Random.value;
            if (randomValue < 0.90f) 
            {
                platformPrefab = commonPlatforms[Random.Range(0, commonPlatforms.Count)];
            }
            else if (randomValue < 0.95f) 
            {
                platformPrefab = uncommonPlatforms[Random.Range(0, uncommonPlatforms.Count)];
            }
            else if (randomValue < 0.98f)
            {
                platformPrefab = rarePlatforms[Random.Range(0, rarePlatforms.Count)];
            }
            else 
            {
                platformPrefab = legendaryPlatforms[Random.Range(0, legendaryPlatforms.Count)];
            }

            Instantiate(platformPrefab, spawnPosition, Quaternion.identity, transform);
        }
    }

    void UpdateSurvivalTimer()
    {
        survivalTime += Time.deltaTime;
        UpdateTimerText();
    }

    private void UpdateTimerText()
    {
        timerText.text = "Time: " + survivalTime.ToString("F2") + "s";
    }

    public void UpdateScore(float newYPosition)
    {
        if (newYPosition > highestPointReached)
        {
            highestPointReached = newYPosition;
            score = Mathf.FloorToInt(highestPointReached);
            UpdateScoreText();
            if (score >= 100 && !gameEnded)
            {
                EndGame("You Win!");
            }
        }
    }

    private void UpdateScoreText()
    {
        scoreText.text = "Score: " + score.ToString();
    }

    public void GameOver()
    {
        if (!gameEnded)
        {
            EndGame("Game Over!");
        }
    }

    void EndGame(string message)
    {
        Debug.Log(message);
        scoreText.text = message + " Score: " + score.ToString();
        timerText.text = "Time: " + survivalTime.ToString("F2") + "s";
        CheckAndSaveBestTime();
        timeRunning = false;
        gameEnded = true;
        Invoke("ReloadScene", 2);
    }

    void CheckAndSaveBestTime()
    {
        float bestTime = PlayerPrefs.GetFloat("BestTime", float.MaxValue); // Default to float.MaxValue if not set
        if (survivalTime < bestTime)
        {
            PlayerPrefs.SetFloat("BestTime", survivalTime);
            PlayerPrefs.Save();
            LoadAndDisplayBestTime(); // Update the best time text if the best time is updated
        }
    }

    void LoadAndDisplayBestTime()
    {
        float bestTime = PlayerPrefs.GetFloat("BestTime", float.MaxValue); // Default to float.MaxValue if not set
        bestTimeText.text = bestTime == float.MaxValue ? "Best Time: --" : "Best Time: " + bestTime.ToString("F2") + "s";
    }

    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindUITextMeshes();
        highestPointReached = 0f;
        score = 0;
        survivalTime = 0f;
        timeRunning = true;
        gameEnded = false;
        GeneratePlatforms();
        UpdateScoreText();
        UpdateTimerText();
        LoadAndDisplayBestTime();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
