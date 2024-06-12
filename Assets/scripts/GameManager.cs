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

    public GameObject player;


    private bool sky1Activated = false;
    private bool sky2Activated = false;
    private bool sky3Activated = false;

    private void LockPlayerPosition()
    {
        if (player != null)
        {
            Rigidbody2D playerRigidbody = player.GetComponent<Rigidbody2D>();
            if (playerRigidbody != null)
            {
                playerRigidbody.velocity = Vector2.zero;
                playerRigidbody.constraints = RigidbodyConstraints2D.FreezePositionY;
            }

        }
    }

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

        InstantiatePrefabAtHeight(301.5f);

    }

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        FindObjects();
        GeneratePlatforms();
        UpdateScoreText();
        UpdateTimerText();
        LoadAndDisplayBestTime();
        gameEnded = false;
        sky1Activated = false;
        sky2Activated = false;
        sky3Activated = false;
        timeRunning = true;
        PlayBackgroundMusic();
        TogglePause(initPause: true);

    }


    private void FindObjects()
    {
        scoreText = GameObject.Find("ScoreText").GetComponent<TextMeshProUGUI>();
        timerText = GameObject.Find("TimerText").GetComponent<TextMeshProUGUI>();
        bestTimeText = GameObject.Find("BestTimeText").GetComponent<TextMeshProUGUI>();

        pauseText = GameObject.Find("PauseText").GetComponent<TextMeshProUGUI>();
        zenith = GameObject.Find("Zenith");
        player = GameObject.FindGameObjectsWithTag("Player")[0];
    }

    void TogglePause(bool initPause = false)
    {
        if (!gameEnded)
        {
            if (initPause)
            {
                timeRunning = false;
                pauseText.gameObject.SetActive(true);
                pauseText.text = "PRESS ENTER";
                Time.timeScale = 0f;
                PauseBackgroundMusic();
            }
            else
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
                    Time.timeScale = 0f;
                    PauseBackgroundMusic();
                }
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
            spawnHeight += Random.Range(1f, 4f);
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
        timerText.text = survivalTime.ToString("F2") + "s";
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

        if (sky1 != null && sky2 != null && sky3 != null && !gameEnded)
        {
            if (score >= 200 && !sky3Activated)
            {
                StartCoroutine(FadeOutAndActivate(sky2, 0.5f));
                StartCoroutine(FadeIn(sky3, 0.5f));
                sky3Activated = true;
            }
            else if (score >= 100 && !sky2Activated)
            {
                StartCoroutine(FadeOutAndActivate(sky1, 0.5f));
                StartCoroutine(FadeIn(sky2, 0.5f));
                sky2Activated = true;
            }
            else if (!sky1Activated)
            {
                sky1Activated = true;
            }
        }
    }

    IEnumerator<object> FadeOutAndActivate(GameObject obj, float duration)
    {
        SpriteRenderer renderer = obj.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            float startAlpha = renderer.color.a;
            float targetAlpha = 0f;
            float currentTime = 0f;

            while (currentTime < duration)
            {
                float alpha = Mathf.Lerp(startAlpha, targetAlpha, currentTime / duration);
                renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, alpha);
                currentTime += Time.deltaTime;
                yield return null;
            }

            obj.SetActive(false);
        }
    }

    IEnumerator<object> FadeIn(GameObject obj, float duration)
    {
        SpriteRenderer renderer = obj.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            float startAlpha = renderer.color.a;
            float targetAlpha = 1f;
            float currentTime = 0f;

            obj.SetActive(true);

            while (currentTime < duration)
            {
                float alpha = Mathf.Lerp(startAlpha, targetAlpha, currentTime / duration);
                renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, alpha);
                currentTime += Time.deltaTime;
                yield return null;
            }
        }
    }

    IEnumerator<object> FadeOut(GameObject obj, float duration)
    {
        SpriteRenderer renderer = obj.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            float startAlpha = renderer.color.a;
            float targetAlpha = 0f;
            float currentTime = 0f;

            while (currentTime < duration)
            {
                float alpha = Mathf.Lerp(startAlpha, targetAlpha, currentTime / duration);
                renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, alpha);
                currentTime += Time.deltaTime;
                yield return null;
            }

            obj.SetActive(false);
        }
    }
    private void UpdateScoreText()
    {
        scoreText.text = score.ToString() + "/300";
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
        LockPlayerPosition();
        scoreText.text = message;
        timerText.text = survivalTime.ToString("F2") + "s";
        timeRunning = false;
        gameEnded = true;

        StopBackgroundMusic();

        if (message == "You Win!")
        {
            winAudioSource.Play();
            SaveBestTime();
        }
        else
        {
            loseAudioSource.Play();
        }

        Invoke("ReloadScene", 4f);
    }

    void SaveBestTime()
    {
        float bestTime = PlayerPrefs.GetFloat("BestTime", float.MaxValue);
        if (survivalTime < bestTime)
        {
            PlayerPrefs.SetFloat("BestTime", survivalTime);
            PlayerPrefs.Save();
            LoadAndDisplayBestTime();
        }
    }

    void ReloadScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void LoadAndDisplayBestTime()
    {
        float bestTime = PlayerPrefs.GetFloat("BestTime", float.MaxValue);
        bestTimeText.text = bestTime == float.MaxValue ? "Best: --" : "Best: " + bestTime.ToString("F2") + "s";
    }


    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindObjects();
        highestPointReached = 0f;
        score = 0;
        survivalTime = 0f;
        timeRunning = false;
        gameEnded = false;
        sky1Activated = false;
        sky2Activated = false;
        sky3Activated = false;
        GeneratePlatforms();
        UpdateScoreText();
        UpdateTimerText();
        LoadAndDisplayBestTime();
        PlayBackgroundMusic();
        TogglePause(initPause: true);

    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void PlayBackgroundMusic()
    {
        if (backgroundAudioSource != null && !backgroundAudioSource.isPlaying)
        {
            backgroundAudioSource.volume = 0.7f;
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
