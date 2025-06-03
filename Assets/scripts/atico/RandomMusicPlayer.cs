using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMusicPlayer : MonoBehaviour
{
    [Header("Agrega aquí tus canciones")]
    public List<AudioClip> musicClips = new List<AudioClip>();

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.loop = false; // No queremos que se repita la misma canción
        PlayRandomSong();
    }

    void Update()
    {
        if (!audioSource.isPlaying)
        {
            PlayRandomSong();
        }
    }

    void PlayRandomSong()
    {
        if (musicClips.Count == 0)
        {
            Debug.LogWarning("No hay canciones en la lista.");
            return;
        }

        int randomIndex = Random.Range(0, musicClips.Count);
        audioSource.clip = musicClips[randomIndex];
        audioSource.Play();
    }
}
