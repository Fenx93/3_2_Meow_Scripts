using UnityEngine;

public class AudioController : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource, sfxSource, extraSfxSource, extraSfxSource2;
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

    public void PlaySFX(AudioClip clip)
    {
        if (!sfxSource.isPlaying)
        {
            sfxSource.clip = clip;
            sfxSource.Play();
        }
        else if(!extraSfxSource.isPlaying)
        {
            extraSfxSource.clip = clip;
            extraSfxSource.Play();
        }
        else
        {
            extraSfxSource2.clip = clip;
            extraSfxSource.Play();
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
        sfxSource.loop = true;
        PlaySFX(rewardWheelSpinningSound);
    }
    public void PlayRewardWheelStopSpinningSound()
    {
        sfxSource.loop = false;
        sfxSource.Stop();
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
