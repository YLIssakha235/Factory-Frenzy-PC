using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using StarterAssets;

public class RaceManager : MonoBehaviour
{
    [Header("Player")]
    public GameObject player;              
    [Header("UI")]
    public TextMeshProUGUI countdownText; 
    public TextMeshProUGUI timerText;      
    public GameObject endPanel;           
    public TextMeshProUGUI resultText;  
    public TextMeshProUGUI bestTimeText;   
    public GameObject getReadyBanner;     

    bool raceRunning = false;

    public bool IsRaceRunning => raceRunning;
    float startTime;

    void Start()
    {
        if (player == null) Debug.LogWarning("RaceManager: drag PlayerArmature into the Player field.");
        if (endPanel != null) endPanel.SetActive(false);
        SetText(timerText, "00:00.0");

        float savedBest = PlayerPrefs.GetFloat("BestTime", 0);
        if (savedBest > 0) SetText(bestTimeText, "BEST " + FormatTime(savedBest));
        else SetText(bestTimeText, "BEST --:--");

        GameObject finishLine = GameObject.Find("FinishLine");
        if (finishLine != null) finishLine.AddComponent<FinishLine>();
        else Debug.LogWarning("RaceManager: no FinishLine in the scene (add a Finish platform).");

        SetText(countdownText, "");
        if (getReadyBanner != null) getReadyBanner.SetActive(false);

        StartCoroutine(StartRace());
    }

    IEnumerator StartRace()
    {
        SetPlayerCanMove(false);

        LevelIntro intro = FindObjectOfType<LevelIntro>();
        if (intro != null)
        {
            yield return StartCoroutine(intro.Play());
        }

        yield return StartCoroutine(Countdown());
    }

    IEnumerator Countdown()
    {
        StartLightsFX lights = FindObjectOfType<StartLightsFX>();
        if (getReadyBanner != null) getReadyBanner.SetActive(true);

        for (int number = 3; number > 0; number--)
        {
            SetText(countdownText, number.ToString());
            if (lights != null) lights.SetCountdown(number);
            SoundFX.Play("countdown_beep");
            yield return new WaitForSeconds(1);
        }

        // GO!
        SetText(countdownText, "GO!");
        SoundFX.Play("countdown_go");
        if (getReadyBanner != null) getReadyBanner.SetActive(false);
        if (lights != null) lights.SetCountdown(0);
        SetPlayerCanMove(true);
        raceRunning = true;
        startTime = Time.time;

        yield return new WaitForSeconds(1);
        SetText(countdownText, "");
    }

    void Update()
    {
        if (raceRunning)
        {
            SetText(timerText, FormatTime(Time.time - startTime));
        }
    }

    public void PlayerFinished()
    {
        if (!raceRunning) return; 
        raceRunning = false;

        float time = Time.time - startTime;

        float bestTime = PlayerPrefs.GetFloat("BestTime", 9999);
        bool newRecord = time < bestTime;
        if (newRecord)
        {
            bestTime = time;
            PlayerPrefs.SetFloat("BestTime", bestTime);
        }

        SetPlayerCanMove(false);
        if (player != null)
        {
            BeanAnimator bean = player.GetComponentInChildren<BeanAnimator>();
            if (bean != null) bean.Celebrate();
        }
        FinishFX finish = FindObjectOfType<FinishFX>();
        if (finish != null) finish.Celebrate();
        SoundFX.Play("finish");
        if (newRecord) SoundFX.Play("new_record");

        string result = FormatTime(time);
        result += "\n<size=40%>Best " + FormatTime(bestTime) + "</size>";
        if (newRecord) result += "\n<size=40%><color=#FFC93C>NEW RECORD!</color></size>";
        SetText(resultText, result);
        SetText(bestTimeText, "BEST " + FormatTime(bestTime));
        StartCoroutine(ShowEndPanel());
    }

    IEnumerator ShowEndPanel()
    {
        yield return new WaitForSeconds(2); 
        if (endPanel != null) endPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None; 
        Cursor.visible = true;
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void SetPlayerCanMove(bool canMove)
    {
        if (player == null) return;

        ThirdPersonController controller = player.GetComponentInChildren<ThirdPersonController>();
        if (controller == null)
        {
            Debug.LogWarning("RaceManager: no ThirdPersonController found in " + player.name);
            return;
        }

        controller.enabled = canMove;
        Debug.Log("RaceManager: player can move = " + canMove);
    }

    void SetText(TextMeshProUGUI text, string value)
    {
        if (text != null) text.text = value;
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    string FormatTime(float seconds)
    {
        int minutes = (int)(seconds / 60);
        float rest = seconds % 60;
        return minutes.ToString("00") + ":" + rest.ToString("00.0");
    }
}