using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public BikeController bike;


    public AudioClip chainLoop;
    public AudioClip skid;
    public AudioClip ambience;
    public AudioClip crash;
    public AudioClip spinOut;
    public AudioClip honk;
    public AudioClip pedal;
    public AudioClip music;


    public float chainVolume = 0.5f;
    public float skidVolume = 0.7f;
    public float ambienceVolume = 0.3f;
    public float sfxVolume = 1f;
    public float honkVolume = 0.6f;
    public float musicVolume = 0.4f;


    public float minPitch = 0.8f;   // chain at a standstill
    public float maxPitch = 1.8f;   // chain right before spinning out
    public float skidPitch = 1f;    // lower this if the skid sounds too high


    public float minHonkWait = 6f;
    public float maxHonkWait = 15f;

    AudioSource chainSource;
    AudioSource skidSource;
    AudioSource ambienceSource;
    AudioSource musicSource;
    AudioSource sfxSource;
    AudioSource honkSource;
    bool wasDead;
    bool wasSkidding;
    float nextHonk;

    void Awake()
    {
        Instance = this;

        // loops start silent and fade in when needed
        chainSource = MakeSource(chainLoop, true, 0f);
        ambienceSource = MakeSource(ambience, true, 0f);
        musicSource = MakeSource(music, true, 0f);

        // one-shot sources need full volume, or one-shots play silently
        skidSource = MakeSource(null, false, 1f);
        sfxSource = MakeSource(null, false, 1f);
        honkSource = MakeSource(null, false, 1f);

        nextHonk = Random.Range(minHonkWait, maxHonkWait);
    }

    AudioSource MakeSource(AudioClip clip, bool loop, float volume)
    {
        AudioSource s = gameObject.AddComponent<AudioSource>();
        s.clip = clip;
        s.loop = loop;
        s.playOnAwake = false;
        s.volume = volume;
        if (clip && loop) s.Play();
        return s;
    }

    void Update()
    {
        // music and city sounds play the whole time, title screen included
        musicSource.volume = musicVolume;
        ambienceSource.volume = ambienceVolume;

        bool riding = bike.started && !bike.dead;

        // chain gets higher the faster you go, silent in a skid since the back wheel locks
        chainSource.pitch = Mathf.Lerp(minPitch, maxPitch, bike.SpeedPercent());
        float chainTarget = riding && !bike.skidding ? chainVolume : 0f;
        chainSource.volume = Mathf.MoveTowards(chainSource.volume, chainTarget, 8f * Time.deltaTime);

        // skid: play the sound once each time a new skid starts
        bool skidNow = riding && bike.skidding;
        if (skidNow && !wasSkidding && skid)
        {
            skidSource.pitch = skidPitch;
            skidSource.PlayOneShot(skid, skidVolume);
        }
        wasSkidding = skidNow;

        // play the death sound once, the moment you die
        if (bike.dead && !wasDead)
        {
            AudioClip clip = (bike.speed >= bike.maxSpeed && spinOut) ? spinOut : crash;
            if (clip) sfxSource.PlayOneShot(clip, sfxVolume);
        }
        wasDead = bike.dead;

        // a quiet honk somewhere in the city every so often
        if (riding)
        {
            nextHonk -= Time.deltaTime;
            if (nextHonk <= 0f)
            {
                Honk(0.35f);
                nextHonk = Random.Range(minHonkWait, maxHonkWait);
            }
        }
    }

    // loudness: 1 = right next to you, lower = far away
    public void Honk(float loudness = 1f)
    {
        if (!honk) return;
        honkSource.pitch = Random.Range(0.9f, 1.1f); // so honks don't all sound identical
        honkSource.PlayOneShot(honk, honkVolume * loudness);
    }

    // called from BikeController when you pedal
    public void PlayPedal()
    {
        if (pedal) sfxSource.PlayOneShot(pedal, sfxVolume * 0.5f);
    }
}