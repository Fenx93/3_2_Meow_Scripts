using UnityEngine;

public class AudioController : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource, sfxSource, loopedSfxSource;
    [SerializeField] private AudioClip buttonClickSound,
        countdownBeepSound, meowSound,
        hitSound, actionNeglectedSound,
        rewardWheelSpinningSound, rewardWheelSpinningStopSound,
        winningSound, losingSound, celebrationSound;

    public static AudioController current;
    void Awake()
    {
        current = this;
    }

    public void PlayMusic(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip, bool playWithLoop = false)
    {
        if (!playWithLoop)
        {
            sfxSource.PlayOneShot(clip);
        }
        else
        {
            loopedSfxSource.clip = clip;
            loopedSfxSource.Play();
        }
    }

    #region SFX Play Sounds
    public void PlayButtonClick()
    {
        PlaySFX(buttonClickSound);
    }
    public void PlayBeepSound()
    {
        PlaySFX(countdownBeepSound);
    }
    public void PlayMeowSound()
    {
        PlaySFX(meowSound);
    }
    public void PlayHitSound()
    {
        PlaySFX(hitSound);
    }
    public void PlayActionNeglectedSound()
    {
        PlaySFX(actionNeglectedSound);
    }
    public void PlayRewardWheelSpinningSound()
    {
        loopedSfxSource.loop = true;
        PlaySFX(rewardWheelSpinningSound, true);
    }
    public void PlayRewardWheelStopSpinningSound()
    {
        loopedSfxSource.loop = false;
        loopedSfxSource.Stop();
        PlayBeepSound();
    }
    public void PlayWinningSound()
    {
        PlaySFX(winningSound);
    }
    public void PlayLosingSound()
    {
        PlaySFX(losingSound);
    }
    public void PlayCelebrationSound()
    {
        PlaySFX(celebrationSound);
    }
    #endregion
}
