using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireFlicker : MonoBehaviour {

    public float baseStart = 0.0f; // start 
    public float amplitude = 1.0f; // amplitude of the wave
    public float phase = 0.0f; // start point inside on wave cycle
    public float frequency = 0.5f; // cycle frequency per second

    // Keep a copy of the original color
    private Color originalColor; 
    private Light light;

    // Store the original color
    void Start () {   
        light = GetComponent<Light>(); 
        originalColor = light.color;
    }

    void Update () 
    {  
        light.color = originalColor * (EvalWave());
    }

    float EvalWave () 
	{ 
        float x = (Time.time + phase) * frequency;
        float y ;
        x = x - Mathf.Floor(x); // normalized value (0..1)
        y = 2f - (Random.value) * 0.75f;
        return (y * amplitude) + baseStart;    
    }
}
