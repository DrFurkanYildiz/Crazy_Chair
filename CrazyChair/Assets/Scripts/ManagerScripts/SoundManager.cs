using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    AudioSource mainAudioSource;
    AudioSource effectAudioSource;

    public AudioClip turningEffect;
    public AudioClip coinCollectEffect;
    public AudioClip coinTurnEffect;
    public AudioClip brokenChairEffect;
    public AudioClip[] guestFallEffect;

    public AudioClip LevelUpEffect;
    public AudioClip buttonClickEffect;
    public AudioClip buttonSelectEffect;
    public AudioClip toggleClickEffect;
    public AudioClip swipeEffect;

    private void Awake()
    {
        Instance = this;

        mainAudioSource = transform.Find("MainAudioSource").GetComponent<AudioSource>();
        effectAudioSource = transform.Find("EffectAudioSource").GetComponent<AudioSource>();
    }
    public void EffectOneShot(AudioClip clip)
    {
        effectAudioSource.PlayOneShot(clip);
    }

    public void MuteMainSound() => mainAudioSource.mute = true;
    public void UnmuteMainSound() => mainAudioSource.mute = false;
    public void MuteEffectSound() => effectAudioSource.mute = true;
    public void UnmuteEffectSound() => effectAudioSource.mute = false;

    public void LevelUpSound() => EffectOneShot(LevelUpEffect);
    public void ClickSound() => EffectOneShot(buttonClickEffect);
    public void SelectSound() => EffectOneShot(buttonSelectEffect);
    public void ToggleSound() => EffectOneShot(toggleClickEffect);
    public void SwipeSound() => EffectOneShot(swipeEffect);
}
