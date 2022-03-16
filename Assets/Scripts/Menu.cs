using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [Header("Menu Icons")]
    // The icons have been switched
    [SerializeField] private Sprite musicOn;
    [SerializeField] private Sprite musicOff;
    [SerializeField] private Sprite effectsOff;
    [SerializeField] private Sprite effectsOn;
    [Header("Menu Objects")]
    [SerializeField, Tooltip("The panel of the main menu")] private GameObject menu;
    [SerializeField, Tooltip("The toggle bg music button")] private Image musicIcon;
    [SerializeField, Tooltip("The toggle FX button")] private Image fxIcon;
    [SerializeField, Tooltip("The panel when dying")] private GameObject endPanel;
    [Header("The current score text fields")]
    [SerializeField] private TMP_Text currentScore;
    [SerializeField] private TMP_Text currentScoreEndPanel;
    public bool GameStarted { get; private set; }

    private void Awake()
    {
        GetComponent<GameHandler>().FinishGameEvent += ShowEndPanel;
    }
    private void OnEnable()
    {
        ChangeMusicIcon();
        ChangeEffectsIcon();
        BannerAd.BannerAdInstance.ShowBannerAd();
    }
    public void StartGame()
    {
        menu.SetActive(false);
        GameStarted = true;
        currentScore.gameObject.SetActive(true);
        BannerAd.BannerAdInstance.HideBannerAd();
    }
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void ExitApp()
    {
        Application.Quit();
    }
    private void ShowEndPanel()
    {
        endPanel.SetActive(true);
        currentScoreEndPanel.text = currentScore.text;
        currentScore.gameObject.SetActive(false);
    }
    public void ChangeMusicIcon()
    {
        musicIcon.sprite=PlayerPrefs.GetInt("MuteBG") == 1 ? musicOff : musicOn;
        BackgroundMusic.BackgroundMusicInstance.PlayMusic();
    }
    public void ToggleMusic()
    {
        PlayerPrefs.SetInt("MuteBG", PlayerPrefs.GetInt("MuteBG") == 1 ? 0 : 1);
    }
    public void ChangeEffectsIcon()
    {
        fxIcon.sprite=PlayerPrefs.GetInt("MuteFX") == 1 ? effectsOff : effectsOn;
    }
    public void ToggleEffects()
    {
        PlayerPrefs.SetInt("MuteFX", PlayerPrefs.GetInt("MuteFX") == 1 ? 0 : 1);
    }
}
