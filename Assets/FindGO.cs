using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindGO : MonoBehaviour
{
    public GameObject Track;
    public GameObject P1Avatar;
    public GameObject P2Avatar;
    public GameObject P1Canvas;
    public GameObject P2Canvas;

    public GameObject FinishBlip;
    public GameObject Walls;
    public GameObject MinimapTrack;
    public GameObject CenterMesh;
    public GameObject OutsideMesh;
    public GameObject PostProcessing;

    public static AudioSource audioSource;
    public static AudioSource ASshield;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        ASshield = transform.GetChild(3).GetComponent<AudioSource>();

        transform.GetChild(0).GetComponent<AudioSource>().volume = 1f * Save.ambienceVol / 10f;
        transform.GetChild(1).GetComponent<AudioSource>().volume = 0.1f * Save.sfxVol / 10f;
        transform.GetChild(2).GetComponent<AudioSource>().volume = 0.1f * Save.sfxVol / 10f;
        transform.GetChild(3).GetComponent<AudioSource>().volume = 0.3f * Save.sfxVol / 10f;
        transform.GetChild(4).GetComponent<AudioSource>().volume = 0.1f * Save.sfxVol / 10f;
        transform.GetChild(5).GetComponent<AudioSource>().volume = 1f * Save.musicVol / 10f;

    }

    public static void PlaySound(string title, float volume)
    {
        AudioClip clip = Resources.Load(title, typeof(AudioClip)) as AudioClip;
        audioSource.PlayOneShot(clip, volume * Save.sfxVol / 10f);
        
    }


}
