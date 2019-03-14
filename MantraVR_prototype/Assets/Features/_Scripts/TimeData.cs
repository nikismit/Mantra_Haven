using UnityEngine;

[System.Serializable]
public struct TimeData
{
	public float seconds;
	public float minutes;

	public float TotalSeconds
	{
		get
		{
			return seconds + minutes * 60;
		}
	}

	public static TimeData operator -(TimeData a, TimeData b)
	{
		TimeData timeData = new TimeData();

		float totalSeconds = a.TotalSeconds - b.TotalSeconds;

		if (totalSeconds < 0f)
			Debug.LogWarning("Cannot have a negative value for TimeData.seconds");

		timeData.seconds = totalSeconds;

		while (timeData.seconds >= 60f)
		{
			timeData.minutes++;
			timeData.seconds -= 60f;
		}

		return timeData;
	}

	public static TimeData operator +(TimeData a, float b)
	{
		TimeData timeData = new TimeData();

		float totalSeconds = a.TotalSeconds + b;

		if (totalSeconds < 0f)
			Debug.LogWarning("Cannot have a negative value for TimeData.seconds");

		timeData.seconds = totalSeconds;

		while (timeData.seconds >= 60f)
		{
			timeData.minutes++;
			timeData.seconds -= 60f;
		}

		return timeData;
	}

	public static TimeData operator -(TimeData a, float b)
	{
		TimeData timeData = new TimeData();

		float totalSeconds = a.TotalSeconds - b;

		if (totalSeconds < 0f)
			Debug.LogWarning("Cannot have a negative value for TimeData.seconds");

		timeData.seconds = totalSeconds;

		while (timeData.seconds >= 60f)
		{
			timeData.minutes++;
			timeData.seconds -= 60f;
		}

		return timeData;
	}

	public static float operator /(float a, TimeData b)
	{
		return a / b.TotalSeconds;
	}

	public static bool operator <(float a, TimeData b)
	{
		return a < b.TotalSeconds;
	}

	public static bool operator >(float a, TimeData b)
	{
		return a > b.TotalSeconds;
	}

	public static bool operator <(TimeData a, float b)
	{
		return a.TotalSeconds < b;
	}

	public static bool operator >(TimeData a, float b)
	{
		return a.TotalSeconds > b;
	}
}