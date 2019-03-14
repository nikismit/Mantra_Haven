using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MantraVR/BreathAudio", fileName = "BreathAudio.asset")]
public class BreatheAudio : ScriptableObject
{
	public List<BreathingRingData> breatheAudioDataList;
}