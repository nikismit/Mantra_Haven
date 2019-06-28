using System.Collections;
using System.IO;

using UnityEngine;
using UnityEngine.SceneManagement;

using TNet;

[RequireComponent(typeof(TNManager))]
[RequireComponent(typeof(TNUdpLobbyClient))]
[RequireComponent(typeof(TNObject))]
public class NetworkStarter : TNBehaviour
{
	public int ServerTcpPort = 5127;

	[SerializeField]
	private string mainSceneName;

	private float timeTriggerPressed = 0;
	private float lobbyWaitTime = 1.5f;
	private bool isSessionStarted;
	//private TNObject tno;

	private AsyncOperation loadMainScene;

	// Start is called before the first frame update
	void Start()
	{
		if (Application.isPlaying)
		{
			// Start resolving IPs
			Tools.ResolveIPs(null);

			// We don't want mobile devices to dim their screen and go to sleep while the app is running
			Screen.sleepTimeout = SleepTimeout.NeverSleep;

			// Make it possible to use UDP using a random port
			TNManager.StartUDP(Random.Range(10000, 50000));
		}

		TNUdpLobbyClient.onChange += () =>
		{
			if (TNManager.isConnected == false)
			{
				JoinServer();
			}
		};
	
		StartCoroutine(LoadMainSceneRoutine(mainSceneName));
	}

    void Update()
    {
		lobbyWaitTime -= Time.deltaTime;
		if (lobbyWaitTime <= 0 && TNManager.isConnected == false)
		{
			TNet.List<ServerList.Entry> serverList = TNUdpLobbyClient.knownServers.list;

			if (serverList != null && serverList.size == 0)
			{
				StartServer();
			}

			lobbyWaitTime = 1.5f;
		}

		// When connected to the server, get input for start signal
		if ((OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger) || Input.GetKey(KeyCode.S)) && isSessionStarted == false)
			timeTriggerPressed += Time.deltaTime;
		else
			timeTriggerPressed = 0;

		if (timeTriggerPressed >= 3.0f && isSessionStarted == false)
		{
			
			//if (TNManager.isConnected == false)
			{
				StartSession();
				return;
			}
			SendBroadCast("StartSession");
		}
    }

	public void SendBroadCast(string eventName)
	{
		print("send");
		tno.Send(eventName, Target.All);
	}

	private void OnNetworkConnect(bool success, string message)
	{
		Debug.Log("connected to server");
	}

	private void JoinServer()
	{
		TNet.List<ServerList.Entry> list = TNLobbyClient.knownServers.list;
		ServerList.Entry ent = list[0];
				
		// NOTE: I am using 'internalAddress' here because I know all servers are hosted on LAN.
		// If you are hosting outside of your LAN, you should probably use 'externalAddress' instead.
		TNManager.Connect(ent.internalAddress, ent.internalAddress);
		
		if (!TNManager.mInstance.mJoining)
		{
			TNManager.mInstance.mJoining = true;
			TNManager.client.JoinChannel(1, "mantra", false, 65535, null);
		}
		tno.rebuildMethodList = true;
	}

	private void StartServer()
	{
		// Start a local server, loading the saved data if possible
		// The UDP port of the server doesn't matter much as it's optional,
		// and the clients get notified of it via Packet.ResponseSetUDP.
		int udpPort = Random.Range(10000, 40000);
		TNLobbyClient lobby = GetComponent<TNLobbyClient>();
		TNServerInstance.Type type = (lobby is TNUdpLobbyClient) ? TNServerInstance.Type.Udp : TNServerInstance.Type.Tcp;
		TNServerInstance.Start(ServerTcpPort, udpPort, lobby.remotePort, "server.dat", type);
	}

	private IEnumerator LoadMainSceneRoutine(string sceneName)
	{
		loadMainScene = SceneManager.LoadSceneAsync(sceneName);
		loadMainScene.allowSceneActivation = false;

		Debug.Log("start scene loading");
		yield return new WaitWhile(()=> loadMainScene.allowSceneActivation == false);
	}

	[RFC]
	private void StartSession()
	{
		Debug.Log("start session");
		isSessionStarted = true;
		loadMainScene.allowSceneActivation = true;
	}
}
