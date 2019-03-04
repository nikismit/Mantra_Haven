using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using SoundInput;

public class RingSpawner : MonoBehaviour {

	public SoundInputController SIC;
    public AudioMixer mainMixer;

    public bool spawnStomach;
    public GameObject StomachRing;
	public float ringGrowSpeed = 5.0f;

	public bool spawnMouth;
	public GameObject MouthRing;
	public float mouthRingSize = 3.0f;
	public float ringFlySpeed = 10.0f;

	public GameObject spawnLocation;
	public Color highPitchColor = Color.yellow;
	public Color lowPitchColor = Color.red;
	private Color currentColor;

	private float _micPitch;
    private float _micAmplitude;
	private bool _isSpeaking = false;
	[Tooltip("Amount of Rings per second to spawn")]
	public int ringSpawningSpeed = 10;
	
	
	public bool ringDeathtimer = true;
	public float ringDeathAfterSeconds = 5.0f;
	private float speakingTimer;
    private float echotimer;
    private float volume;
    public float echoTime = 1.0f;
	public float maxEchoVolumeDB = 1.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
		_micPitch =  SIC.inputData.relativeFrequency;
        _micAmplitude = SIC.inputData.amp01;

		if ((_micAmplitude > 0) && (!_isSpeaking)){
			_isSpeaking = true;
		}
		if ((_micAmplitude <= 0) && (_isSpeaking)){
			_isSpeaking = false;
            echotimer = 0.0f;
			SIC.NullifyClipData();
		}
        
        if (_isSpeaking){
			currentColor = Color.Lerp(lowPitchColor, highPitchColor, _micPitch);
			speakingTimer+=Time.deltaTime;
            echotimer += Time.deltaTime;
            
			if(speakingTimer >= 1.0f/ringSpawningSpeed){
                if (spawnStomach)
                {
                    GameObject currentRing = GameObject.Instantiate(StomachRing, spawnLocation.transform.position - (new Vector3(0, 2.5f * (1.0f - _micPitch), 0)), StomachRing.transform.rotation);
                    currentRing.GetComponent<Ring_Script>().growSpeed = ringGrowSpeed;
                    currentRing.GetComponent<Ring_Script>().pitch = _micPitch;
                    currentRing.GetComponent<Ring_Script>().currentColor = currentColor;
                    if (ringDeathtimer == true)
                    {
                        currentRing.GetComponent<Ring_Script>().deathTimer = ringDeathAfterSeconds;
                        currentRing.GetComponent<Ring_Script>().deathTimerStart = true;
                    }
                }
                if (spawnMouth)
                {
                    GameObject currentRing2 = GameObject.Instantiate(MouthRing, spawnLocation.transform.position, this.transform.rotation);
                    
                    currentRing2.transform.localScale = new Vector3(_micAmplitude * mouthRingSize, _micAmplitude * mouthRingSize, _micAmplitude * mouthRingSize);
                    currentRing2.GetComponent<Ring_Script>().moveSpeed = ringFlySpeed;
                    currentRing2.GetComponent<Ring_Script>().pitch = _micPitch;
                    currentRing2.GetComponent<Ring_Script>().currentColor = currentColor;
                    if (ringDeathtimer == true)
                    {
                        currentRing2.GetComponent<Ring_Script>().deathTimer = ringDeathAfterSeconds;
                        currentRing2.GetComponent<Ring_Script>().deathTimerStart = true;
                    }
                    //print(this.transform.position + " --> " + currentRing2.transform.position);
                }
                
                
                
				speakingTimer = 0.0f;
			}
		}
        if (echotimer >= echoTime)
        {
            volume += 80.0f * Time.deltaTime;
            volume = Mathf.Clamp(volume, -80.0f, maxEchoVolumeDB);
            mainMixer.SetFloat("MasterVolume", volume);
        } else
        {
            volume -= 80.0f * Time.deltaTime;
            volume = Mathf.Clamp(volume, -80.0f, maxEchoVolumeDB);
            mainMixer.SetFloat("MasterVolume", volume);
        }

    }
}
