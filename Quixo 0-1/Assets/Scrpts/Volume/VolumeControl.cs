using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{

    public static float VOLUME = .25f;

    public Slider VolumeSlider;

    private void Start()
    {
        VolumeSlider.value = VOLUME;
    }

    public void modifyVolume()
    { 
        VOLUME = VolumeSlider.value;
    }

}
