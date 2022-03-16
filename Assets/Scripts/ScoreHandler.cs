using UnityEngine;
using TMPro;

public class ScoreHandler : MonoBehaviour
{
    [SerializeField] private TMP_Text currentScoreText;
    [SerializeField] private TMP_Text highScore;
    private int _currentScore;
    private const string HighScore = "HighScore";

    private void Awake()
    {
        GetComponent<GameHandler>().UpdateScoreEvent += UpdateCurrentScore;
        GetComponent<GameHandler>().FinishGameEvent += UpdateHighScore;
        highScore.text = PlayerPrefs.GetInt(HighScore, 0).ToString();
    }

    private void UpdateHighScore()
    {
        if (_currentScore > PlayerPrefs.GetInt(HighScore, 0))
        {
            PlayerPrefs.SetInt(HighScore, _currentScore);
            highScore.text = _currentScore.ToString();

        }
        BannerAd.BannerAdInstance.ShowBannerAd();
    }
    
    private void UpdateCurrentScore()
    {
        _currentScore++;
        currentScoreText.text = _currentScore.ToString();
    }

}
