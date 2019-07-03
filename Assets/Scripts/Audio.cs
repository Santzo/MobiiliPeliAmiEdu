using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Unity.Collections;
public class Audio : MonoBehaviour
{
    [System.Serializable]
    public class Clip
    {
        public string tag;
        public AudioClip clip;
        [HideInInspector] public AudioSource source;
        public bool music;

        public Clip()
        {
            tag = "New Audio Clip";
        }
    }

    public class Dict : Dictionary<string, Clip>
    {
        public void _Add(string tag, AudioClip val, AudioSource src, bool msc)
        {
            Clip clp = new Clip();
            clp.clip = val;
            clp.source = src;
            clp.music = msc;
            Add(tag, clp);
        }
    }

    public AudioMixer master;
    public static Audio ad;
    public List<Clip> clip;
    public Dict audioLib = new Dict();

    [HideInInspector] public float masterVolume;
    [HideInInspector] public float musicVolume;
    [HideInInspector] public float soundVolume;

    private void Awake()
    {
        if (ad == null)
        {
            ad = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);


        foreach (Clip clp in clip)
        {
            AudioSource ads = gameObject.AddComponent<AudioSource>();
            ads.outputAudioMixerGroup = clp.music ? master.FindMatchingGroups("Music")[0] : master.FindMatchingGroups("Sound")[0];
            audioLib._Add(clp.tag, clp.clip, ads, clp.music);
        }

        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 100f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 100f);
        soundVolume = PlayerPrefs.GetFloat("SoundVolume", 100f);
    }
    private void Start()
    {
        master = Resources.Load("Master") as AudioMixer;
        VolumeReset("Master", masterVolume);
        VolumeReset("Music", musicVolume);
        VolumeReset("Sound", soundVolume);
    }



    public static void PlaySound(string clip, float volume = 1f, float pitch = 1f)
    {
        ad.audioLib[clip].source.pitch = pitch;
        ad.audioLib[clip].source.PlayOneShot(ad.audioLib[clip].clip, volume);
    }

    public static void PlayOnLoop(string clip, float volume = 1f, float pitch = 1f)
    {
        ad.audioLib[clip].source.clip = ad.audioLib[clip].clip;
        ad.audioLib[clip].source.pitch = pitch;
        ad.audioLib[clip].source.volume = volume;
        ad.audioLib[clip].source.loop = true;
        ad.audioLib[clip].source.Play();
    }

    public static void StopMusic(string clip)
    {
        if (ad.audioLib[clip].source.isPlaying)
            ad.audioLib[clip].source.Stop();
    }

    public static bool IsPlaying(string clip)
    {
        return ad.audioLib[clip].source.isPlaying;
    }

    public static void PauseMusic(string clip)
    {
        if (ad.audioLib[clip].source.isPlaying) ad.audioLib[clip].source.Pause();
        else ad.audioLib[clip].source.UnPause();
    }

    public static void SetVolume(string clip, float volume)
    {
        volume = Mathf.Clamp(volume, 0f, 1f);
        ad.audioLib[clip].source.volume = volume;
    }

    public static float CurrentVolume(string clip)
    {
        return ad.audioLib[clip].source.volume;
    }

    public static IEnumerator _VolumeFade(string clip, float from, float to, float time, bool pause = false, bool stop = false)
    {
        ad.audioLib[clip].source.volume = from;
        if (!ad.audioLib[clip].source.isPlaying) ad.audioLib[clip].source.Play();
        from = Mathf.Clamp(from, 0f, 1f);
        to = Mathf.Clamp(to, 0f, 1f);

        float start = Time.realtimeSinceStartup;
        float end = Time.realtimeSinceStartup + time;
        float add = (to - from) / time;
        yield return null;
        while (end > start)
        {
            float _add = Time.realtimeSinceStartup - start;
            start = Time.realtimeSinceStartup;
            ad.audioLib[clip].source.volume += _add * add;
            yield return null;
        }
        if (pause) PauseMusic(clip);
        if (!pause && stop) StopMusic(clip);
        yield return null;
    }


    public static void ResetAllMusicAndSound()
    {
        foreach (KeyValuePair<string, Audio.Clip> _clp in ad.audioLib)
        {
            StopMusic(_clp.Key);
        }
    }


    public void VolumeReset(string _name, float volume)
    {

        volume = Mathf.Clamp(volume, 0f, 100f);
        float _volume = -20f + (volume * 0.2f);
        if (volume < 50f) _volume += -49f + (volume);
        if (volume == 0f) _volume = -80f;
        if (_name == "Master")
            master.SetFloat("masterVol", _volume);
        if (_name == "Sound")
            master.SetFloat("soundVol", _volume);
        if (_name == "Music")
            master.SetFloat("musicVol", _volume);
        }
}


