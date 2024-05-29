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
    public bool gameEnded = false; // Flag to prevent further actions after game ends

    private AudioSource backgroundAudioSource; // Reference to the AudioSource for background music
    private AudioSource winAudioSource; // Reference to the AudioSource for win sound
    private AudioSource loseAudioSource; // Reference to the AudioSource for lose sound

    public AudioClip backgroundMusic;
    public AudioClip winSound;
    public AudioClip loseSound;

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

        // Create and set up the AudioSources
        backgroundAudioSource = gameObject.AddComponent<AudioSource>();
        winAudioSource = gameObject.AddComponent<AudioSource>();
        loseAudioSource = gameObject.AddComponent<AudioSource>();

        // Assign audio clips
        backgroundAudioSource.clip = backgroundMusic;
        winAudioSource.clip = winSound;
        loseAudioSource.clip = loseSound;

        // Configure audio sources
        backgroundAudioSource.loop = true; // Loop background music
        winAudioSource.loop = false; // No loop for win sound
        loseAudioSource.loop = false; // No loop for lose sound
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
        
        // Play background music
        PlayBackgroundMusic();
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
        while (spawnHeight < 1000f)
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
        if (!gameEnded && newYPosition > highestPointReached)
        {
            highestPointReached = newYPosition;
            score = Mathf.FloorToInt(highestPointReached);
            UpdateScoreText();
            if (score >= 1000 && !gameEnded)
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
        timeRunning = false; // Stop the timer
        gameEnded = true;
        
        // Stop background music
        StopBackgroundMusic();
        
        // Play win or lose sound
        if (message == "You Win!")
        {
            winAudioSource.Play();
        }
        else
        {
            loseAudioSource.Play();
        }
        
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
        
        // Restart background music
        PlayBackgroundMusic();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void PlayBackgroundMusic()
    {
        if (backgroundAudioSource != null && !backgroundAudioSource.isPlaying)
        {
            backgroundAudioSource.Play();
        }
    }

    private void StopBackgroundMusic()
    {
        if (backgroundAudioSource != null && backgroundAudioSource.isPlaying)
        {
            backgroundAudioSource.Stop();
        }
    }
}
