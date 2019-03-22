using UnityEngine;

public class BreathingRingController : MonoBehaviour
{
	public BreatheAudio breatheAudio;
	public BreathingRing breathingRing;
	public Vector3 defaultStartScale;
	public Vector3 defaultEndScale;

	public bool canAddTime = false;
	public KeyCode addTimeKey = KeyCode.Space;

	private TimeData _currentTime = new TimeData();

	private int _breathingIndex = -1;
	private int _elementIndex = 0;

	private void Start()
	{
		if (canAddTime)
			breatheAudio.breatheAudioDataList.Clear();
	}

	private void Update()
	{
		if (canAddTime)
		{
			AddTimeUpdate();
		}
		else
		{
			ExecuteTimeUpdate();
		}

		// Update Timer
		_currentTime.Seconds += Time.deltaTime;

		if (_currentTime.Seconds >= 60)
		{
			_currentTime.Seconds -= 60;
			_currentTime.Minutes++;
		}
	}

	private void AddTimeUpdate()
	{
		if (Input.GetKeyDown(addTimeKey))
		{
			if (_breathingIndex > 3 || _breathingIndex == -1)
			{
				breatheAudio.breatheAudioDataList.Add(new BreathingRingData());
				_breathingIndex = 0;
			}

			BreathingRingData breathingRingData = new BreathingRingData();
			int breatheAudioDataListIndex = breatheAudio.breatheAudioDataList.Count - 1;
			switch (_breathingIndex)
			{
				case 0:
					breatheAudio.breatheAudioDataList[breatheAudioDataListIndex].inhaleStartTime = _currentTime;
					breathingRingData.inhaleStartTime = _currentTime;
					breathingRingData.inhaleEndTime = _currentTime + 5;
					break;
				case 1:
					breatheAudio.breatheAudioDataList[breatheAudioDataListIndex].inhaleEndTime = _currentTime;
					breathingRingData.inhaleEndTime = _currentTime;
					breathingRingData.inhaleStartTime = _currentTime - 5;
					break;
				case 2:
					breatheAudio.breatheAudioDataList[breatheAudioDataListIndex].exhaleStartTime = _currentTime;
					breathingRingData.exhaleStartTime = _currentTime;
					breathingRingData.exhaleEndTime = _currentTime + 5;
					breathingRingData.inhaleStartTime = _currentTime - 10;
					breathingRingData.inhaleEndTime = _currentTime - 5;
					break;
				case 3:
					breatheAudio.breatheAudioDataList[breatheAudioDataListIndex].exhaleEndTime = _currentTime;
					breathingRingData.exhaleStartTime = _currentTime;
					breathingRingData.exhaleEndTime = _currentTime - 5;
					break;
			}
			breatheAudio.breatheAudioDataList[breatheAudioDataListIndex].startScale = defaultStartScale;
			breatheAudio.breatheAudioDataList[breatheAudioDataListIndex].endScale = defaultEndScale;
			breathingRingData.startScale = defaultStartScale;
			breathingRingData.endScale = defaultEndScale;

			breathingRing.SetData(breathingRingData);
			if (_breathingIndex == 0)
				breathingRing.Unpause(true);
			if (_breathingIndex == 2)
				breathingRing.Unpause(false);
			if (_breathingIndex == 1 || _breathingIndex == 3)
				breathingRing.Pause();

			_breathingIndex++;
		}
	}

	private void ExecuteTimeUpdate()
	{
		if (_elementIndex >= breatheAudio.breatheAudioDataList.Count)
			return;

		if (_breathingIndex == -1)
			_breathingIndex = 0;

		float timeToExecute = 0;
		BreathingRingData breathingRingData = breatheAudio.breatheAudioDataList[_elementIndex];
		switch (_breathingIndex)
		{
			case 0:
				timeToExecute = breatheAudio.breatheAudioDataList[_elementIndex].inhaleStartTime.TotalSeconds;
				break;
			case 1:
				timeToExecute = breatheAudio.breatheAudioDataList[_elementIndex].inhaleEndTime.TotalSeconds;
				break;
			case 2:
				timeToExecute = breatheAudio.breatheAudioDataList[_elementIndex].exhaleStartTime.TotalSeconds;
				break;
			case 3:
				timeToExecute = breatheAudio.breatheAudioDataList[_elementIndex].exhaleEndTime.TotalSeconds;
				break;
		}

		if (_currentTime.TotalSeconds >= timeToExecute)
		{
			breathingRing.SetData(breathingRingData);
			breathingRing.Unpause();

			_breathingIndex++;

			if (_breathingIndex > 3)
			{
				_elementIndex++;
				_breathingIndex = 0;
			}
		}
	}
}