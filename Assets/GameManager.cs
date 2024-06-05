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

    public GameObject zenith;


    public GameObject sky1;
    public GameObject sky2;
    public GameObject sky3;

    public TextMeshProUGUI pauseText;


    public static GameManager Instance { get; private set; }

    private float highestPointReached = 0f;
    private int score = 0;
    private float survivalTime = 0f;
    private bool timeRunning = true;
    public bool gameEnded = false;

    private AudioSource backgroundAudioSource;
    private AudioSource winAudioSource;
    private AudioSource loseAudioSource;

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

        backgroundAudioSource = gameObject.AddComponent<AudioSource>();
        winAudioSource = gameObject.AddComponent<AudioSource>();
        loseAudioSource = gameObject.AddComponent<AudioSource>();

        backgroundAudioSource.clip = backgroundMusic;
        winAudioSource.clip = winSound;
        loseAudioSource.clip = loseSound;

        backgroundAudioSource.loop = true;
        winAudioSource.loop = false;
        loseAudioSource.loop = false;

        InstantiatePrefabAtHeight(303.5f);

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

        PlayBackgroundMusic();
    }


    private void FindUITextMeshes()
    {
        scoreText = GameObject.Find("ScoreText").GetComponent<TextMeshProUGUI>();
        timerText = GameObject.Find("TimerText").GetComponent<TextMeshProUGUI>();
        bestTimeText = GameObject.Find("BestTimeText").GetComponent<TextMeshProUGUI>();

        pauseText = GameObject.Find("BestTimeText").GetComponent<TextMeshProUGUI>();
        zenith = GameObject.Find("Zenith");
    }

    void TogglePause()
    {
        if (!gameEnded)
        {
            timeRunning = !timeRunning;
            if (timeRunning)
            {
                pauseText.gameObject.SetActive(false);
                Time.timeScale = 1f;
                UnPauseBackgroundMusic();
            }
            else
            {
                pauseText.gameObject.SetActive(true);
                pauseText.text = "PAUSED";

                PauseBackgroundMusic();

                Time.timeScale = 0f;
            }
        }
    }

    private void PauseBackgroundMusic()
    {
        if (backgroundAudioSource != null && backgroundAudioSource.isPlaying)
        {
            backgroundAudioSource.Pause();
        }
    }

    private void UnPauseBackgroundMusic()
    {
        if (backgroundAudioSource != null && !backgroundAudioSource.isPlaying)
        {
            backgroundAudioSource.UnPause();
        }
    }


    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Return))
        {
            TogglePause();
        }

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
        while (spawnHeight < 297.5f)
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
            UpdateSky();

            if (score >= 300 && !gameEnded)
            {
                EndGame("You Win!");
            }
        }
    }

    private void UpdateSky()
{
    if (sky1 == null || sky2 == null || sky3 == null)
    {
        sky1 = GameObject.Find("Main Camera/sky1");
        sky2 = GameObject.Find("Main Camera/sky2");
        sky3 = GameObject.Find("Main Camera/sky3");
    }

    Debug.Log($"Updating sky. Score: {score}, Skies: {sky1}, {sky2}, {sky3}");

    if (sky1 != null && sky2 != null && sky3 != null && !gameEnded)
    {
        if (score >= 200)
        {
            Debug.Log("Setting sky3 active");
            sky1.SetActive(false);
            sky2.SetActive(false);
            sky3.SetActive(true);
        }
        else if (score >= 100)
        {
            Debug.Log("Setting sky2 active");
            sky1.SetActive(false);
            sky2.SetActive(true);
            sky3.SetActive(false);
        }
        else
        {
            Debug.Log("Setting sky1 active");
            sky1.SetActive(true);
            sky2.SetActive(false);
            sky3.SetActive(false);
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

    void InstantiatePrefabAtHeight(float height)
    {
        Vector3 position = new Vector3(0, height, 0);

        GameObject instantiatedObject = Instantiate(zenith, position, Quaternion.identity);

    }


   void EndGame(string message)
{
    Debug.Log(message);
    scoreText.text = message + " Score: " + score.ToString();
    timerText.text = "Time: " + survivalTime.ToString("F2") + "s";
    CheckAndSaveBestTime();
    timeRunning = false;
    gameEnded = true;

    StopBackgroundMusic();

    if (message == "You Win!")
    {
        winAudioSource.Play();
    }
    else
    {
        loseAudioSource.Play();
    }

    Invoke("ReloadScene", 2f);
}

void ReloadScene()
{
    Time.timeScale = 1f;
    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
}

    void CheckAndSaveBestTime()
    {
        float bestTime = PlayerPrefs.GetFloat("BestTime", float.MaxValue);
        if (survivalTime < bestTime)
        {
            PlayerPrefs.SetFloat("BestTime", survivalTime);
            PlayerPrefs.Save();
            LoadAndDisplayBestTime();
        }
    }

    void LoadAndDisplayBestTime()
    {
        float bestTime = PlayerPrefs.GetFloat("BestTime", float.MaxValue);
        bestTimeText.text = bestTime == float.MaxValue ? "Best Time: --" : "Best Time: " + bestTime.ToString("F2") + "s";
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
            backgroundAudioSource.volume = 0.4f;
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
