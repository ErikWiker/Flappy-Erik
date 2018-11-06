using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class GameManager : MonoBehaviour
{

    public delegate void GameDelegate(); // allow us to create events for other scripts to be notified
    public static event GameDelegate OnGameStarted;
    public static event GameDelegate OnGameOverConfirmed;

    public static GameManager Instance; //static accesibility reference from other scripts, they can access public members within this class (Singleton)

    public GameObject startPage;
    public GameObject gameOverPage;
    public GameObject countdownPage;
    public Text scoreText;

    enum PageState //always have a none in enums
    {
        None,
        Start,
        Countdown,
        GameOver
    }

    int score = 0;
    bool gameOver = true;

    public bool GameOver { get { return gameOver; } } //accesible but cannot be modified

    void Awake() //singlestons? destroying on load
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void OnEnable()
    {
        TapController.OnPlayerDied += OnPlayerDied;
        TapController.OnPlayerScored += OnPlayerScored;
        CountdownText.OnCountdownFinished += OnCountdownFinished;
    }

    void OnDisable()
    {
        TapController.OnPlayerDied -= OnPlayerDied; //unsubscribe, minus equals
        TapController.OnPlayerScored -= OnPlayerScored;
        CountdownText.OnCountdownFinished -= OnCountdownFinished;
    }

    void OnCountdownFinished()
    {
        SetPageState(PageState.None);
        OnGameStarted(); //event that provides... //event is sent to TapController 
        score = 0;
        gameOver = false;
    }

    void OnPlayerScored()
    {
        score++;
        scoreText.text = score.ToString(); //format the number so our text component can handle
    }

    void OnPlayerDied()
    {
        gameOver = true;
        int savedScore = PlayerPrefs.GetInt("HighScore"); //subregistery for each project that playerprefs is saved in, getting old high score for reference
        if (score > savedScore) // check if it is a highscore
        {
            PlayerPrefs.SetInt("HighScore", score);  //string values have to be exact same, this is setting highscore
        }
        SetPageState(PageState.GameOver);
    }

    void SetPageState(PageState state)
    {
        switch (state)
        {
            case PageState.None:
                startPage.SetActive(false);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(false);
                break;
            case PageState.Start:
                startPage.SetActive(true);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(false);
                break;
            case PageState.Countdown:
                startPage.SetActive(false);
                gameOverPage.SetActive(false);
                countdownPage.SetActive(true);
                break;
            case PageState.GameOver:
                startPage.SetActive(false);
                gameOverPage.SetActive(true);
                countdownPage.SetActive(false);
                break;
        }
    }

    //Button shows up when dies, activated when replay button is hit
    public void ConfirmGameOver()
    {
        SetPageState(PageState.Start);
        scoreText.text = "0";
        OnGameOverConfirmed();
    }

    //Activated when play button is hit
    public void StartGame()
    {
        SetPageState(PageState.Countdown);
    }

}