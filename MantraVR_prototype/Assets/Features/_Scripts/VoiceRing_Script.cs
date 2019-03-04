using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoundInput;

//This is to create a ring of particles around an object that responds to sound input
public class VoiceRing_Script : MonoBehaviour {

	public SoundInputController SIC; //A script pulled from a github that checks sound input and gives out some values related to it.
	public GameObject particlePrefab;
	[Range(10, 500)]
	public int amountOfParticles;
	public float particleMoveSpeed;
	[Range(0.1f, 5.0f)]
	public float waveAmp;
	[Range(0.01f, 1.0f)]
	public float waveWidth = 0.5f;


	GameObject[] particles;
	GameObject particle;

	public Color[] pitchColors;



	float pitch = 0.0f;
	float currentPitch = 0.0f;
	float volume = 0.0f;
	float currentVolume = 0.0f;
	
	//Make a sine wave based on pitch
	float SineFunction (float x, float t) {
		float y = Mathf.Sin(Mathf.PI *(pitch*0.25f) * (x + t));
		y *= waveAmp;
		return y;
	}


	private void Start()
	{
		//create a ring of particles around the object.
		particles = new GameObject[amountOfParticles];

		if(particlePrefab != null){

			for (int i = 0; i < particles.Length; i++)
			{
				particle = Instantiate(particlePrefab) as GameObject;
				particle.transform.parent = this.transform;
				particles[i] = particle;
			}

		}
	}
	
	void Update()
    {
		
		
		currentPitch = SIC.inputData.relativeFrequency;
		currentVolume = SIC.inputData.relativeAmplitude;

        pitch = Mathf.Lerp(pitch, currentPitch, Time.deltaTime*2f);
		if(currentVolume > 0){
			volume = Mathf.Lerp(volume, currentVolume, Time.deltaTime/2f);
		} else {
			volume = 1.0f;
		}

		float t = Time.time;
		
		
		Color partColor = Color.white;
		//set color of particle based on pitch
		partColor = Color.Lerp(pitchColors[0], pitchColors[1], pitch);
		
		
		//realtime particles
        for (int i = 0; i < particles.Length; i++)
        {
			//set position for each particle based on volume
			Vector3 position = this.transform.localPosition;
			Vector2 offset = new Vector2(0,0);

			offset = new Vector2(Mathf.Sin(i/(float)(particles.Length)*2f*Mathf.PI), Mathf.Cos(i/(float)(particles.Length)*2f*Mathf.PI))*(volume*waveWidth);

			position.x += offset.x;

			position.z += offset.y;

			position.y = SineFunction(i, t);

			particles[i].transform.localPosition = position;

			ParticleSystem.VelocityOverLifetimeModule veloMain = particles[i].GetComponent<ParticleSystem>().velocityOverLifetime;

			//set velocity of particle based on last position
			Vector3 velocity = Vector3.Normalize(position - this.transform.localPosition)*particleMoveSpeed;

			veloMain.x = velocity.x;
			veloMain.z = velocity.z;

			ParticleSystem.EmissionModule emis = particles[i].GetComponent<ParticleSystem>().emission;

			if(currentVolume > 0){
				emis.rateOverTime = 5;
			} else {
				emis.rateOverTime = 0;
			}

			ParticleSystem.MainModule main = particles[i].GetComponent<ParticleSystem>().main;
			
			main.startColor = partColor;

			particles[i].GetComponentInChildren<MeshRenderer>().material.SetColor("_TintColor", partColor);

		}
		
    }
}
