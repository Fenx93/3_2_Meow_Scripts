using UnityEngine;

public class AudioController : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource, sfxSource, extraSfxSource, extraSfxSource2;
    [SerializeField] private AudioClip buttonClickSound,
        countdownBeepSound, meowSound,
        hitSound, actionNeglectedSound,
        rewardWheelSpinningSound, rewardWheelSpinningStopSound,
        winningSound, losingSound, celebrationSound;

    private AudioSource loopedAudioSource = null;
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
        if (!sfxSource.isPlaying)
        {
            sfxSource.clip = clip;
            sfxSource.loop = playWithLoop;
            if (playWithLoop)
                loopedAudioSource = sfxSource;
            sfxSource.Play();
        }
        else if(!extraSfxSource.isPlaying)
        {
            extraSfxSource.clip = clip;
            extraSfxSource.loop = playWithLoop;
            if (playWithLoop)
                loopedAudioSource = sfxSource;
            extraSfxSource.Play();
        }
        else
        {
            extraSfxSource2.clip = clip;
            extraSfxSource2.loop = playWithLoop;
            if (playWithLoop)
                loopedAudioSource = sfxSource;
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
        PlaySFX(rewardWheelSpinningSound, true);
    }
    public void PlayRewardWheelStopSpinningSound()
    {
        loopedAudioSource.loop = false;
        loopedAudioSource.Stop();
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
