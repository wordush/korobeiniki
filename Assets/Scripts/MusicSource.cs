using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MusicSource : MonoBehaviour
{
    private AudioSource _source;
    [FormerlySerializedAs("Music")] public AudioClip[] music;

    public static bool initialize;

    public static float PerfSoundVolume;

    private void Awake()
    {
        _source = GetComponent<AudioSource>();
        if (!initialize)
        {
            _source.clip = music[Random.Range(0, music.Length)];
            _source.Play();
            DontDestroyOnLoad(this);
            initialize = true;
        }
    }

    private void Update()
    {
        if (!_source.isPlaying)
        {
            _source.clip = music[(int)Random.Range(0, music.Length)];
            _source.Play();
        }
    }

    public void SetMusicVolume(float vol)
    {
        _source.volume = vol;
    }

    public void SetPerfVolume(float vol)
    {
        PerfSoundVolume = vol;
    }

}
