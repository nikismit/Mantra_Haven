using UnityEngine;

public class particleDelayStart : MonoBehaviour 
{
	private ParticleSystem.EmissionModule emission;
	private float delay;

	void Start()
	{
		emission = GetComponent<ParticleSystem>().emission;
		delay = Random.value * 2;
		transform.localScale = Vector3.zero;
	}

	void Update () 
	{
		delay -= Time.deltaTime;
		emission.enabled = delay <= 0;
		enabled = transform.localScale.x < .98f;

		if( delay <= 0 )
			transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, Time.deltaTime * 5f);
	}
}
