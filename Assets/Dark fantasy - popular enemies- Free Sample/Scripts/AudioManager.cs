using UnityEngine;

/// <summary>
/// Покладіть на окремий GameObject у сцені рівня (і за бажанням у Menu).
/// У Inspector призначте всі AudioClip вручну.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Clips")]
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private AudioClip attackClip;
    [SerializeField] private AudioClip coinClip;
    [SerializeField] private AudioClip damageClip;
    [SerializeField] private AudioClip enemyDeathClip;
    [SerializeField] private AudioClip failClip;
    [SerializeField] private AudioClip healClip;

    [Header("Settings")]
    [SerializeField] private float musicVolume = 0.35f;
    [SerializeField] private float sfxVolume = 0.8f;
    [SerializeField] private bool playMusicOnStart = true;

    private AudioSource musicSource;
    private AudioSource sfxSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.volume = musicVolume;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.loop = false;
        sfxSource.playOnAwake = false;
        sfxSource.volume = sfxVolume;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Start()
    {
        if (playMusicOnStart)
        {
            PlayBackgroundMusic();
        }
    }

    public void PlayBackgroundMusic()
    {
        if (backgroundMusic == null || musicSource == null)
        {
            return;
        }

        musicSource.clip = backgroundMusic;
        musicSource.Play();
    }

    public void StopBackgroundMusic()
    {
        musicSource?.Stop();
    }

    public void PlayAttack() => PlaySfx(attackClip);
    public void PlayCoin() => PlaySfx(coinClip);
    public void PlayDamage() => PlaySfx(damageClip);
    public void PlayEnemyDeath() => PlaySfx(enemyDeathClip);
    public void PlayFail() => PlaySfx(failClip);
    public void PlayHeal() => PlaySfx(healClip);

    private void PlaySfx(AudioClip clip)
    {
        if (clip == null || sfxSource == null)
        {
            return;
        }

        sfxSource.PlayOneShot(clip, sfxVolume);
    }
}
