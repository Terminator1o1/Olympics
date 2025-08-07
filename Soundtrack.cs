using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soundtrack : MonoBehaviour
{
    [SerializeField] AudioClip[] songs;
    [SerializeField] float coolDown = 3f;
    [SerializeField] int recentlyPlayedSize = 5;
    bool shouldPlay = true;
    Queue<AudioClip> recentlyPlayed;

    private void Start()
    {
        recentlyPlayed = new Queue<AudioClip>();
    }

    public void PlaySoundtrack()
    {
        shouldPlay = true;
        Invoke("PlayNext", coolDown);
    }

    void PlayNext()
    {
        if (shouldPlay && !GetComponent<AudioSource>().isPlaying)
        {
            int i = UnityEngine.Random.Range(0, songs.Length);
            while (recentlyPlayed.Contains(songs[i]))
            {
                i = UnityEngine.Random.Range(0, songs.Length);
            }
            GetComponent<AudioSource>().clip = songs[i];           
            GetComponent<AudioSource>().Play();
            recentlyPlayed.Enqueue(songs[i]);
            if (recentlyPlayed.Count > recentlyPlayedSize) recentlyPlayed.Dequeue();
            Invoke("PlayNext", songs[i].length + coolDown);
        }
    }

    public void StopSoundtrack()
    {
        shouldPlay = false;
        GetComponent<AudioSource>().Stop();
    }
}
