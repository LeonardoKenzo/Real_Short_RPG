using UnityEngine;

public class CinematicManager : MonoBehaviour
{
    public AudioSource musicSource;

    public AudioClip backgroundMusic;

    void Start()
    {
        musicSource.clip = backgroundMusic;
        musicSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
