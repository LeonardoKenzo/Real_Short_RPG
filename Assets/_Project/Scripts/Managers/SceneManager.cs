using UnityEngine;

public class SceneManager : MonoBehaviour
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
