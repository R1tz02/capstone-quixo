	using UnityEngine;
	using System.Collections;
	
	public class EmissionPulse : MonoBehaviour 
	{
		public float frequency = 1f;
		public float amplitude = 1f;
		public float baseMult = 0.75f;
		public Material material;
		public Color emissionColor;
		
		// Use this for initialization
		void Start () 
		{
		emissionColor = emissionColor * baseMult * amplitude;
		}
		
		// Update is called once per frame
		void Update () 
		{
			float glow = (2 + Mathf.Cos(Time.time * frequency)) * amplitude;
			material.SetColor("_EmissionColor", emissionColor * glow);
		}
	}
