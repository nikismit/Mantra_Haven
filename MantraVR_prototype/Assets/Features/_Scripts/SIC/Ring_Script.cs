using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ring_Script : MonoBehaviour {

    public bool growRing;
    public float moveSpeed = 5.0f;
    public float growSpeed = 5.0f;
	public float pitch = 1.0f;
	public Color currentColor;
	public float deathTimer = 5.0f;
	private float deathTime = 0.0f;
	public bool deathTimerStart = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (growRing == false)
        {
            transform.Translate(this.transform.forward*Time.deltaTime*moveSpeed, Space.World);
        }
        if (growRing == true)
        {
            transform.localScale += new Vector3(growSpeed, growSpeed, growSpeed) * Time.deltaTime;
        }
		pitch = Mathf.Clamp01(pitch);
		this.transform.Find("Model").GetComponent<Renderer>().material.SetColor("_TintColor", currentColor);
		if(deathTimerStart == true){
			
			deathTime += Time.deltaTime;
			if(deathTime >= 2.5f){
				currentColor.a -= Time.deltaTime;
			}
			if(deathTime >= deathTimer){
				Destroy(this.gameObject);
			}
		}
		
	}
}
