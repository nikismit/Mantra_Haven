//------------------------------------------
//            Tasharen Network
// Copyright © 2012-2014 Tasharen Entertainment
//------------------------------------------

using UnityEngine;
using TNet;

/// <summary>
/// This simple script shows how to change the color of an object on all connected clients.
/// You can see it used in Example 1.
/// </summary>

[RequireComponent(typeof(TNObject))]
public class ColoredObject : MonoBehaviour
{
	/// <summary>
	/// This function is called by the server when one of the players sends an RFC call.
	/// </summary>

	[RFC] void OnColor (Color c)
	{
		GetComponent<Renderer>().material.color = c;
	}

	/// <summary>
	/// Clicking on the object should change its color.
	/// </summary>

    void Update()
    {
        if (Input.GetKey("up"))
        {
            Color color = Color.red;

            if (GetComponent<Renderer>().material.color == Color.red) color = Color.green;
            else if (GetComponent<Renderer>().material.color == Color.green) color = Color.blue;

            TNObject tno = GetComponent<TNObject>();
            tno.Send("OnColor", Target.AllSaved, color);
        }
    }

	void OnClick ()
	{
        Debug.Log("test");
		Color color = Color.red;

		if		(GetComponent<Renderer>().material.color == Color.red)	 color = Color.green;
		else if (GetComponent<Renderer>().material.color == Color.green) color = Color.blue;

		TNObject tno = GetComponent<TNObject>();
		tno.Send("OnColor", Target.AllSaved, color);
	}
}