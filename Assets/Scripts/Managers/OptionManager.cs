using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionManager : MonoBehaviour
{
    private OptionManager Instance;
    private AudioSource music;
    private AudioSource effects;


    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(transform.root);
        }
        else
        {
            Destroy(gameObject);
        }

        music = FindObjectOfType<SoundManager>().ingameMusic;
        effects = FindObjectOfType<SoundManager>().explosionSFX;
        if (!PlayerPrefs.HasKey("Effects"))
        {
            PlayerPrefs.SetInt("Effects", 1);
        }
        else
        {
            effects.enabled = PlayerPrefs.GetInt("Effects") == 1;
        }

        if (!PlayerPrefs.HasKey("Music"))
        {
            PlayerPrefs.SetInt("Music", 1);
        }
        else
        {
            music.enabled = PlayerPrefs.GetInt("Music") == 1;
        }
    }

    public void TurnEffects(bool flag)
    {
        effects.enabled = flag;
        PlayerPrefs.SetInt("Effects", flag ? 1 : 0);
    }

    public void TurnMusic(bool flag)
    { 
        music.enabled = flag;
        PlayerPrefs.SetInt("Music", flag ? 1 : 0);
    }
}
