using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource ambientHum;
    public AudioSource keyBeep;

    void Start()
    {
        if (ambientHum != null)
        {
            ambientHum.loop = true;
            ambientHum.Play();
        }
    }

    public void PlayBeep()
    {
        if (keyBeep != null)
            keyBeep.Play();
    }
}