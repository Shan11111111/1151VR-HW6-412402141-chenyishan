
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Director : MonoBehaviour
{
    [Header("Game Settings")]
    public float gameDuration = 60f;
    public int appleScore = 10;
    public int bombPenalty = 20;

    [Header("UI References")]
    public TMP_Text scoreText;
    public TMP_Text timerText;
    public TMP_Text resultText;

    [Header("Panels")]
    public GameObject startPanel;
    public GameObject resultPanel;
    public GameObject hudPanel;

    [Header("Buttons")]
    public Button startButton;
    public Button restartButton;

    [Header("References")]
    public ItemFactory factory;

    [Header("Ending Images")]
    public Image backgroundImage;
    public Sprite failSprite;
    public Sprite keepTryingSprite;
    public Sprite goodSprite;
    public Sprite excellentSprite;

    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("Background Music")]
    public AudioClip menuBgmClip;
    public AudioClip gameBgmClip;
    public AudioClip endingBgmClip;

    [Header("Sound Effects")]
    public AudioClip catchAppleClip;
    public AudioClip catchBombClip;
    public AudioClip startClip;
    public AudioClip gameOverClip;
    public AudioClip restartClip;

    private float remainingTime;
    private int score = 0;
    private bool gameEnded = false;
    private bool gameStarted = false;
    private bool isRestarting = false;

    private AudioClip defaultBgmClip;

    public bool IsGameEnded
    {
        get { return gameEnded; }
    }

    public bool IsGameStarted
    {
        get { return gameStarted; }
    }

    public bool CanPlay
    {
        get { return gameStarted && !gameEnded; }
    }

    void Start()
    {
        Time.timeScale = 1f;

        remainingTime = gameDuration;
        score = 0;
        gameEnded = false;
        gameStarted = false;
        isRestarting = false;

        // 開始前不生成物品
        if (factory != null)
        {
            factory.enabled = false;
        }

        // 顯示開始畫面
        if (startPanel != null)
        {
            startPanel.SetActive(true);
        }

        // 隱藏結局畫面
        if (resultPanel != null)
        {
            resultPanel.SetActive(false);
        }

        // 隱藏分數及時間
        if (hudPanel != null)
        {
            hudPanel.SetActive(false);
        }

        // 隱藏結局插畫
        if (backgroundImage != null)
        {
            backgroundImage.gameObject.SetActive(false);
        }

        // 開始按鈕
        if (startButton != null)
        {
            startButton.onClick.RemoveListener(StartGame);
            startButton.onClick.AddListener(StartGame);
            startButton.interactable = true;
        }

        // 重新開始按鈕
        if (restartButton != null)
        {
            restartButton.onClick.RemoveListener(RestartGame);
            restartButton.onClick.AddListener(RestartGame);
            restartButton.interactable = true;
        }

        // 設定背景音樂
        if (bgmSource != null)
        {
            defaultBgmClip = bgmSource.clip;
            bgmSource.playOnAwake = false;
            bgmSource.loop = true;
            bgmSource.spatialBlend = 0f;
        }

        // 設定音效播放器
        if (sfxSource != null)
        {
            sfxSource.playOnAwake = false;
            sfxSource.loop = false;
            sfxSource.spatialBlend = 0f;
        }

        // 開始畫面播放背景音樂
        PlayBGM(menuBgmClip);

        UpdateUI();
    }

    void Update()
    {
        if (!CanPlay)
        {
            return;
        }

        remainingTime -= Time.deltaTime;

        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            EndGame();
        }

        UpdateUI();
    }

    public void StartGame()
    {
        if (gameStarted || gameEnded)
        {
            return;
        }

        Time.timeScale = 1f;

        gameStarted = true;
        gameEnded = false;
        remainingTime = gameDuration;
        score = 0;

        // 隱藏開始畫面
        if (startPanel != null)
        {
            startPanel.SetActive(false);
        }

        // 隱藏結局畫面
        if (resultPanel != null)
        {
            resultPanel.SetActive(false);
        }

        // 隱藏結局圖片
        if (backgroundImage != null)
        {
            backgroundImage.gameObject.SetActive(false);
        }

        // 顯示分數及倒數
        if (hudPanel != null)
        {
            hudPanel.SetActive(true);
        }

        // 啟動 Factory
        if (factory != null)
        {
            factory.enabled = true;
        }

        // 遊戲背景音樂
        PlayBGM(gameBgmClip);

        // 開始音效
        PlaySFX(startClip);

        UpdateUI();

        Debug.Log("Game Started!");
    }

    public void CatchItem(string itemTag)
    {
        if (!CanPlay)
        {
            return;
        }

        if (itemTag == "Apple")
        {
            score += appleScore;
            PlaySFX(catchAppleClip);
        }
        else if (itemTag == "Bomb")
        {
            score -= bombPenalty;
            PlaySFX(catchBombClip);
        }
        else
        {
            return;
        }

        UpdateUI();

        // 負分立即結束
        if (score < 0)
        {
            EndGame();
        }
    }

    // 一般音效
    void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    // 給 FallingItem 使用的落地音效
    public void PlayGroundSound(AudioClip clip, float volume)
    {
        if (!CanPlay)
        {
            return;
        }

        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(
                clip,
                Mathf.Clamp01(volume)
            );
        }
    }

    // 控制各階段背景音樂
    void PlayBGM(AudioClip selectedClip)
    {
        if (bgmSource == null)
        {
            return;
        }

        AudioClip clip = selectedClip;

        if (clip == null)
        {
            clip = defaultBgmClip;
        }

        if (clip == null)
        {
            Debug.LogWarning("尚未設定背景音樂！");
            return;
        }

        bgmSource.loop = true;

        if (bgmSource.clip == clip &&
            bgmSource.isPlaying)
        {
            return;
        }

        bgmSource.Stop();
        bgmSource.clip = clip;
        bgmSource.Play();
    }

    void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }

        if (timerText != null)
        {
            timerText.text =
                "Time: " + Mathf.CeilToInt(remainingTime);
        }
    }

    void EndGame()
    {
        if (gameEnded)
        {
            return;
        }

        gameEnded = true;

        // 停止生成
        if (factory != null)
        {
            factory.enabled = false;
        }

        // 隱藏開始畫面
        if (startPanel != null)
        {
            startPanel.SetActive(false);
        }

        // 隱藏 HUD
        if (hudPanel != null)
        {
            hudPanel.SetActive(false);
        }

        string resultTitle;
        Sprite selectedSprite;

        // 負分立即失敗
        if (score < 0)
        {
            resultTitle = "GAME OVER";
            selectedSprite = failSprite;
        }
        // 0～29 分
        else if (score < 30)
        {
            resultTitle = "再接再厲！";
            selectedSprite = keepTryingSprite;
        }
        // 30～99 分
        else if (score < 100)
        {
            resultTitle = "很棒～！";
            selectedSprite = goodSprite;
        }
        // 100 分以上
        else
        {
            resultTitle = "超級棒～！";
            selectedSprite = excellentSprite;
        }

        // 切換結局插畫
        if (backgroundImage != null)
        {
            if (selectedSprite != null)
            {
                backgroundImage.sprite = selectedSprite;
                backgroundImage.color = Color.white;
                backgroundImage.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogWarning("結局圖片尚未設定！");
            }
        }

        // 結局只顯示兩行
        if (resultText != null)
        {
            resultText.text =
                resultTitle + "\n" +
                "Final Score: " + score;
        }

        // 顯示結局畫面
        if (resultPanel != null)
        {
            resultPanel.SetActive(true);
        }

        // 停止場上剩餘的物品
        FallingItem[] items =
            FindObjectsByType<FallingItem>(
                FindObjectsSortMode.None
            );

        foreach (FallingItem item in items)
        {
            if (item != null)
            {
                Rigidbody itemRb =
                    item.GetComponent<Rigidbody>();

                if (itemRb != null)
                {
                    itemRb.linearVelocity = Vector3.zero;
                    itemRb.angularVelocity = Vector3.zero;
                    itemRb.isKinematic = true;
                }

                item.enabled = false;
            }
        }

        // 播放結局音樂
        PlayBGM(endingBgmClip);

        // 播放結束音效
        PlaySFX(gameOverClip);

        Debug.Log(
            "遊戲結束：" + resultTitle +
            "，最終分數：" + score
        );
    }

    // 重新開始按鈕
    public void RestartGame()
    {
        if (isRestarting)
        {
            return;
        }

        isRestarting = true;

        if (restartButton != null)
        {
            restartButton.interactable = false;
        }

        StartCoroutine(RestartWithSound());
    }

    // 播放音效後重新載入場景
    private IEnumerator RestartWithSound()
    {
        Time.timeScale = 1f;

        AudioClip clip = restartClip;

        // 沒設定專用音效，就使用開始音效
        if (clip == null)
        {
            clip = startClip;
        }

        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);

            // 等待音效播放完，避免被切場景中斷
            yield return new WaitForSecondsRealtime(
                clip.length
            );
        }

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().name
        );
    }
}
