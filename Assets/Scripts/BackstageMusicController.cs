using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BackstageMusicController : MonoBehaviour
{
    [Header("Concert Director")]
    public ConcertDirector concertDirector;  

    private AudioSource backstageMusic;
    private bool stopped = false;

    private void Awake()
    {
        backstageMusic = GetComponent<AudioSource>();
    }

    private void Start()
    {
        if (backstageMusic != null)
        {
            backstageMusic.loop = true;
            backstageMusic.Play();
        }
    }

    private void Update()
    {
        if (stopped) return;

        if (concertDirector != null &&
            concertDirector.musicSource != null &&
            concertDirector.musicSource.isPlaying)
        {
            stopped = true;

            if (backstageMusic != null)
                backstageMusic.Stop();
        }
    }
}
