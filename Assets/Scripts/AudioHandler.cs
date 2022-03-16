using UnityEngine;

public class AudioHandler : MonoBehaviour
{
    [SerializeField] private AudioSource successPlayer;
    [SerializeField] private AudioSource failPlayer;
    [SerializeField] private AudioSource gameOverPlayer;

    private void Awake()
    {
        GetComponent<GameHandler>().PlayEffectEvent += PlayEffect;
    }

    private void PlayEffect(EffectName effectName)
    {
        if (PlayerPrefs.GetInt("MuteFX") == 1)
        {
            return;
        }
        switch (effectName)
        {
            case EffectName.Success:
                PlaySuccess();
                break;
            case EffectName.Fail:
                PlayFail();
                break;
            case EffectName.EndGame:
                PlayGameOver();
                break;
        }
    }
    private void PlayGameOver()
    {
        gameOverPlayer.Play();
    }
    private void PlayFail()
    {
        failPlayer.Play();
    }

    private void PlaySuccess()
    {
        successPlayer.Play();
    }
}
public enum EffectName{
    Success, Fail, EndGame, 
}