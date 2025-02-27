//---------------------------------------------
//            Tasharen Network
// Copyright © 2012-2015 Tasharen Entertainment
//---------------------------------------------

using UnityEngine;
using UnityEngine.SceneManagement;
using TNet;
using UnityTools = TNet.UnityTools;

/// <summary>
/// Extremely simplified "join a server" functionality. Attaching this script will
/// make it possible to automatically join a remote server when the game starts.
/// It's best to place this script in a clean scene with a message that displays
/// a "Connecting, please wait..." message.
/// </summary>

public class TNAutoJoin : MonoBehaviour
{
	static public TNAutoJoin instance;

	public string serverAddress = "127.0.0.1";
	public int serverPort = 5127;
	
	public string firstLevel = "Example 1";
	public int channelID = 1;
	public bool persistent = false;
	public string disconnectLevel;

	public bool allowUDP = true;
	public bool connectOnStart = true;
	public string successFunctionName;
	public string failureFunctionName;

	public byte attempts = 0;

	private float pressed;

	/// <summary>
	/// Set the instance so this script can be easily found.
	/// </summary>

	void Awake ()
	{
		pressed = 1;

		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
	}

	/// <summary>
	/// Connect to the server if requested.
	/// </summary>

	void Start () { if (connectOnStart) Connect(); }

	/// <summary>
	/// custom code: skip connect en move to firstlevel
	/// </summary>
	void Update() 
	{ 
		if (Input.GetKey(KeyCode.Escape)) 
		{ 
			pressed -= Time.deltaTime; 
			
			if (pressed <= 0) 
				SceneManager.LoadScene(firstLevel); 
		}
	}

	/// <summary>
	/// disconnect from the server to prevent dead users in serverlist
	/// </summary>

	void OnDestroy() { if (TNManager.isConnected ) TNManager.Disconnect(); }

	/// <summary>
	/// Connect to the server.
	/// </summary>

	public void Connect ()
	{
		// We don't want mobile devices to dim their screen and go to sleep while the app is running
		Screen.sleepTimeout = SleepTimeout.NeverSleep;		
		// Connect to the remote server
		TNManager.Connect(serverAddress, serverPort);
	}

	/// <summary>
	/// On success -- join a channel.
	/// </summary>

	void OnNetworkConnect (bool result, string message)
	{
		if (result)
		{
			// Make it possible to use UDP using a random port
			if (allowUDP) TNManager.StartUDP(Random.Range(10000, 50000));
			TNManager.JoinChannel(channelID, firstLevel, persistent, 10000, null);
		}
		else if (++attempts <= 15)
		{
			// try to connect again
			Connect();
		}
		else if (!string.IsNullOrEmpty(failureFunctionName))
		{
			// Broadcast failure message
			UnityTools.Broadcast(failureFunctionName, message);
		}
		else 
			Debug.LogError(message);
	}

	/// <summary>
	/// Disconnected? Go back to the menu.
	/// </summary>

	void OnNetworkDisconnect ()
	{
		if (!string.IsNullOrEmpty(disconnectLevel) && Application.loadedLevelName != disconnectLevel)
			Application.LoadLevel(disconnectLevel);
	}

	/// <summary>
	/// Joined a channel (or failed to).
	/// </summary>

	void OnNetworkJoinChannel (bool result, string message)
	{
		if (result)
		{
			if (!string.IsNullOrEmpty(successFunctionName))
			{
				UnityTools.Broadcast(successFunctionName);
			}
		}
		else
		{
			if (!string.IsNullOrEmpty(failureFunctionName))
			{
				UnityTools.Broadcast(failureFunctionName, message);
			}
			else Debug.LogError(message);

			TNManager.Disconnect();
		}
	}
}
