using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetVolume : MonoBehaviour
{
    public AudioSource music;

    // Start is called before the first frame update
    void Start()
    {
        music.volume = VolumeControl.VOLUME;
    }

    // Update is called once per frame
    void Update()
    {
        if(music.volume != VolumeControl.VOLUME) music.volume = VolumeControl.VOLUME;
    }
}
