using UnityEngine;
using System.Collections;
using PitchDetector;
using UnityEngine.UI;
/*******************************************************
Very simple template for use pitch detector.
Mic is enabled in the Start() function
The Updte() funtion will detect the pitch and print out

Use this script as base of your development
If you wish to see a ore complex use of pitch detector
tak a look at the example folder
*******************************************************/
public class template : MonoBehaviour {
	public static template Handler;

	public Slider TimeSlider;	
	public Text noteText;							//GUI txt where the note will be printed

	private Detector pitchDetector;						//Pitch detector object

	private int minFreq, maxFreq; 						//Max and min frequencies window
	public string selectedDevice { get; private set; }	//Mic selected

	float[] data;										//Sound samples data
	public int cumulativeDetections= 5; 				//Number of consecutive detections used to determine current note
	int [] detectionsMade;								//Detections buffer
	private int maxDetectionsAllowed=150;				//Max buffer size
	private int detectionPointer=0;						//Current buffer pointer
	public int pitchTimeInterval=100; 					//Millisecons needed to detect tone
	private float refValue = 0.1f; 						// RMS value for 0 dB
	public float minVolumeDB=-17f;						//Min volume in bd needed to start detection
	
	private string currentDetectedNoteName;				//Note name in modern notation (C=Do, D=Re, etc..)
	public float CurrentNoteTimer = 0f;		//Note name in modern notation (C=Do, D=Re, etc..)
	public float Loudness = 0f;
	string currentDetectedNotestring = "";	
	string currentDetectedNotestringLetter = "";	
	public string currentDetectedNoteForTimer = "";	
	void Awake() {
		pitchDetector = new Detector();
		pitchDetector.setSampleRate(AudioSettings.outputSampleRate);
		template.Handler = this;
	}

	void Start () {
		selectedDevice = Microphone.devices[0].ToString();
		GetMicCaps();
		
		//Estimates bufer len, based on pitchTimeInterval value
		int bufferLen = (int)Mathf.Round (AudioSettings.outputSampleRate * pitchTimeInterval / 1000f);
		Debug.Log ("Buffer len: " + bufferLen);
		data = new float[bufferLen];
		
		detectionsMade = new int[maxDetectionsAllowed]; //Allocates detection buffer

		setUptMic();
	}

	// This function will detect the pitch
	void Update () {
		Loudness = 4f;

		if(Input.GetKey(KeyCode.A)){
			currentDetectedNoteForTimer="A";
			CurrentNoteTimer += Time.deltaTime;
			TimeSlider.value = CurrentNoteTimer;
			return;
		}
		if(Input.GetKey(KeyCode.B)){
			currentDetectedNoteForTimer="B";
			CurrentNoteTimer += Time.deltaTime;
			TimeSlider.value = CurrentNoteTimer;
			return;
		}
		if(Input.GetKey(KeyCode.C)){
			currentDetectedNoteForTimer="C";
			CurrentNoteTimer += Time.deltaTime;
			TimeSlider.value = CurrentNoteTimer;
			return;
		}
		if(Input.GetKey(KeyCode.D)){
			currentDetectedNoteForTimer="D";
			CurrentNoteTimer += Time.deltaTime;
			TimeSlider.value = CurrentNoteTimer;
			return;
		}
		if(Input.GetKey(KeyCode.E)){
			currentDetectedNoteForTimer="E";
			CurrentNoteTimer += Time.deltaTime;
			TimeSlider.value = CurrentNoteTimer;
			return;
		}
		if(Input.GetKey(KeyCode.F)){
			currentDetectedNoteForTimer="F";
			CurrentNoteTimer += Time.deltaTime;
			TimeSlider.value = CurrentNoteTimer;
			return;
		}
		if(Input.GetKey(KeyCode.G)){
			currentDetectedNoteForTimer="G";
			CurrentNoteTimer += Time.deltaTime;
			TimeSlider.value = CurrentNoteTimer;
			return;
		}

		if (currentDetectedNoteForTimer == currentDetectedNotestringLetter && currentDetectedNotestringLetter != "") {
			CurrentNoteTimer += Time.deltaTime;
			TimeSlider.value = CurrentNoteTimer;
		} else {
			CurrentNoteTimer = 0f;
			TimeSlider.value = CurrentNoteTimer;
			currentDetectedNoteForTimer = currentDetectedNotestringLetter;
		}


		GetComponent<AudioSource>().GetOutputData(data,0);
		float sum = 0f;
		for(int i=0; i<data.Length; i++)
			sum += data[i]*data[i];
		float rmsValue = Mathf.Sqrt(sum/data.Length);
		float dbValue = 20f*Mathf.Log10(rmsValue/refValue);

		Loudness = sum;
		if(dbValue<minVolumeDB) {
			//Sound too low
			noteText.text="Note: <<";
			currentDetectedNotestringLetter = "";
			Loudness = -1f;
			return;
		}

		//PITCH DTECTED!!
		pitchDetector.DetectPitch (data);
		int midiant = pitchDetector.lastMidiNote ();
		int midi = findMode ();
		currentDetectedNotestring = pitchDetector.midiNoteToString (midi);
		if (currentDetectedNotestring != null) {
			if (currentDetectedNotestring.Contains ("A")) {
				currentDetectedNotestringLetter = "A";
			} else if (currentDetectedNotestring.Contains ("B")) {
				currentDetectedNotestringLetter = "B";
			} else if (currentDetectedNotestring.Contains ("C")) {
				currentDetectedNotestringLetter = "C";
			} else if (currentDetectedNotestring.Contains ("D")) {
				currentDetectedNotestringLetter = "D";
			} else if (currentDetectedNotestring.Contains ("E")) {
				currentDetectedNotestringLetter = "E";
			} else if (currentDetectedNotestring.Contains ("F")) {
				currentDetectedNotestringLetter = "F";
			} else if (currentDetectedNotestring.Contains ("G")) {
				currentDetectedNotestringLetter = "G";
			} else {
				currentDetectedNotestringLetter = "H";
			}

		}
		noteText.text="Note: "+currentDetectedNotestring;
		detectionsMade [detectionPointer++] = midiant;
		detectionPointer %= cumulativeDetections;
	
	}

	void setUptMic() {
		//GetComponent<AudioSource>().volume = 0f;
		GetComponent<AudioSource>().clip = null;
		GetComponent<AudioSource>().loop = true; // Set the AudioClip to loop
		GetComponent<AudioSource>().mute = false; // Mute the sound, we don't want the player to hear it
		StartMicrophone();
	}
	
	public void GetMicCaps () {
		Microphone.GetDeviceCaps(selectedDevice, out minFreq, out maxFreq);//Gets the frequency of the device
		if ((minFreq + maxFreq) == 0)
			maxFreq = 44100;
	}
	
	public void StartMicrophone () {
		GetComponent<AudioSource>().clip = Microphone.Start(selectedDevice, true, 10, maxFreq);//Starts recording
		while (!(Microphone.GetPosition(selectedDevice) > 0)){} // Wait until the recording has started
		GetComponent<AudioSource>().Play(); // Play the audio source!
	}
	
	public void StopMicrophone () {
		GetComponent<AudioSource>().Stop();//Stops the audio
		Microphone.End(selectedDevice);//Stops the recording of the device	
	}
	
	int repetitions(int element) {
		int rep = 0;
		int tester=detectionsMade [element];
		for (int i=0; i<cumulativeDetections; i++) {
			if(detectionsMade [i]==tester)
				rep++;
		}
		return rep;
	}
	
	public int findMode() {
		cumulativeDetections = (cumulativeDetections >= maxDetectionsAllowed) ? maxDetectionsAllowed : cumulativeDetections;
		int moda = 0;
		int veces = 0;
		for (int i=0; i<cumulativeDetections; i++) {
			if(repetitions(i)>veces)
				moda=detectionsMade [i];
		}
		return moda;
	}
}
