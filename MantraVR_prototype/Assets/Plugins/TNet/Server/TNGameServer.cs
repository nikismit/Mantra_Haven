//---------------------------------------------
//            Tasharen Network
// Copyright © 2012-2015 Tasharen Entertainment
//---------------------------------------------

#define MULTI_THREADED

using System;
using System.IO;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.Text;

namespace TNet
{
/// <summary>
/// Game server logic. Handles new connections, RFCs, and pretty much everything else. Example usage:
/// GameServer gs = new GameServer();
/// gs.Start(5127);
/// </summary>

public class GameServer : FileServer
{
#if MULTI_THREADED
	public const bool isMultiThreaded = true;
#else
	public const bool isMultiThreaded = false;
#endif

	/// <summary>
	/// You will want to make this a unique value.
	/// </summary>

	public const ushort gameID = 1;

	public delegate void OnCustomPacket (TcpPlayer player, Buffer buffer, BinaryReader reader, Packet request, bool reliable);
	public delegate void OnPlayerAction (Player p);
	public delegate void OnShutdown ();

	/// <summary>
	/// Any packet not already handled by the server will go to this function for processing.
	/// </summary>

	public OnCustomPacket onCustomPacket;

	/// <summary>
	/// Notification triggered when a player connects and authenticates successfully.
	/// </summary>

	public OnPlayerAction onPlayerConnect;

	/// <summary>
	/// Notification triggered when a player disconnects.
	/// </summary>

	public OnPlayerAction onPlayerDisconnect;

	/// <summary>
	/// Notification triggered when the server shuts down.
	/// </summary>

	public OnShutdown onShutdown;

	/// <summary>
	/// Give your server a name.
	/// </summary>

	public string name = "Game Server";

	/// <summary>
	/// Lobby server link, if one is desired.
	/// You can use this to automatically inform a remote lobby server of any changes to this server.
	/// </summary>

	public LobbyServerLink lobbyLink;

	/// <summary>
	/// List of players in a consecutive order for each looping.
	/// </summary>

	List<TcpPlayer> mPlayers = new List<TcpPlayer>();

	/// <summary>
	/// Dictionary list of players for easy access by ID.
	/// </summary>

	Dictionary<int, TcpPlayer> mDictionaryID = new Dictionary<int, TcpPlayer>();

	/// <summary>
	/// Dictionary list of players for easy access by IPEndPoint.
	/// </summary>

	Dictionary<IPEndPoint, TcpPlayer> mDictionaryEP = new Dictionary<IPEndPoint, TcpPlayer>();

	/// <summary>
	/// List of all the active channels.
	/// </summary>

	List<Channel> mChannels = new List<Channel>();

	/// <summary>
	/// Random number generator.
	/// </summary>

	Random mRandom = new Random();

	Buffer mBuffer;
	TcpListener mListener;
	Thread mThread;
	int mListenerPort = 0;
	long mTime = 0;
	UdpProtocol mUdp = new UdpProtocol();
	bool mAllowUdp = false;
	object mLock = 0;
	DataNode mData = null;
	string mFilename = "world.dat";

	// List of admin keywords
	List<string> mAdmin = new List<string>();

	// List of banned players
	List<string> mBan = new List<string>();

	/// <summary>
	/// Add a new entry to the list. Returns 'true' if a new entry was added.
	/// </summary>

	static bool AddUnique (List<string> list, string s)
	{
		if (!string.IsNullOrEmpty(s) && !list.Contains(s))
		{
			list.Add(s);
			return true;
		}
		return false;
	}

	/// <summary>
	/// Whether the server is currently actively serving players.
	/// </summary>

#if MULTI_THREADED
	public bool isActive { get { return mThread != null; } }
#else
	public bool isActive { get { return mListener != null; } }
#endif

	/// <summary>
	/// Whether the server is listening for incoming connections.
	/// </summary>

	public bool isListening { get { return (mListener != null); } }

	/// <summary>
	/// Port used for listening to incoming connections. Set when the server is started.
	/// </summary>

	public int tcpPort { get { return (mListener != null) ? mListenerPort : 0; } }

	/// <summary>
	/// Listening port for UDP packets.
	/// </summary>

	public int udpPort { get { return mUdp.listeningPort; } }

	/// <summary>
	/// How many players are currently connected to the server.
	/// </summary>

	public int playerCount { get { return isActive ? mPlayers.size : 0; } }

	/// <summary>
	/// Start listening to incoming connections on the specified port.
	/// </summary>

	public bool Start (int tcpPort) { return Start(tcpPort, 0); }

	/// <summary>
	/// Start listening to incoming connections on the specified port.
	/// </summary>

	public bool Start (int tcpPort, int udpPort)
	{
		Stop();

		Tools.LoadList("ServerConfig/ban.txt", mBan);
		Tools.LoadList("ServerConfig/admin.txt", mAdmin);

		// Banning by IPs is pointless
		//for (int i = mBan.size; i > 0; )
		//{
		//    IPAddress ip;
		//    if (IPAddress.TryParse(mBan[--i], out ip))
		//        mBan.RemoveAt(i);
		//}

#if WINDWARD
		// Check URL: http://steamcommunity.com/profiles/76561199211637591
		AddUnique(mBan, "76561198265685624"); // Shared account, hundreds of people using it
		AddUnique(mBan, "76561198022066592"); // Hacker: spammed chat with repeated packets for 2 days
		AddUnique(mBan, "76561198046792874"); // Hacker: was doing all kinds of weird shit
		AddUnique(mBan, "76561199046841142"); // Hidden account, 2 billion gold, very fishy
		AddUnique(mBan, "76561198744124281"); // ALI213
		AddUnique(mBan, "76561200587781055"); // ALI213
		AddUnique(mBan, "76561201119545283"); // Chinese hackers
		AddUnique(mBan, "76561198311261525"); // Chinese hackers
		AddUnique(mBan, "76561197960639879"); // Using some old version
		AddUnique(mBan, "76561202232029992"); // Shared account
		AddUnique(mBan, "76561197996533125"); // Shared account
		//AddUnique(mBan, "76561199211637591"); // ALI213
		//AddUnique(mBan, "ALI213");
#endif

		try
		{
			mListenerPort = tcpPort;
			mListener = new TcpListener(IPAddress.Any, tcpPort);
			mListener.Start(50);
			//mListener.BeginAcceptSocket(OnAccept, null);
		}
		catch (System.Exception ex)
		{
			Tools.LogError(ex.Message, ex.StackTrace, true);
			return false;
		}

#if STANDALONE
		Tools.Print("Game server started on port " + tcpPort + " using protocol version " + Player.version);
#endif
		if (!mUdp.Start(udpPort))
		{
			Tools.LogError("Unable to listen to UDP port " + udpPort, null);
			Stop();
			return false;
		}

		mAllowUdp = (udpPort > 0);

		if (lobbyLink != null)
		{
			lobbyLink.Start();
			lobbyLink.SendUpdate(this);
		}

#if MULTI_THREADED
		mThread = new Thread(ThreadFunction);
		mThread.Start();
#endif
		return true;
	}

	/// <summary>
	/// Call this function when you've disabled multi-threading.
	/// </summary>

	public void Update () { if (mThread == null && mListener != null) ThreadFunction(); }

	/// <summary>
	/// Accept socket callback.
	/// </summary>

	//void OnAccept (IAsyncResult result) { AddPlayer(mListener.EndAcceptSocket(result)); }

	/// <summary>
	/// Stop listening to incoming connections and disconnect all players.
	/// </summary>

	public void Stop ()
	{
		if (onShutdown != null) onShutdown();
		if (lobbyLink != null) lobbyLink.Stop();

		mAllowUdp = false;

		// Stop the worker thread
		if (mThread != null)
		{
			mThread.Abort();
			mThread = null;
		}

		// Stop listening
		if (mListener != null)
		{
			mListener.Stop();
			mListener = null;
		}
		mUdp.Stop();

		// Remove all connected players and clear the list of channels
		for (int i = mPlayers.size; i > 0; ) RemovePlayer(mPlayers[--i]);
		mChannels.Clear();

		// Player counter should be reset
		Player.ResetPlayerCounter();
	}

	/// <summary>
	/// Stop listening to incoming connections but keep the server running.
	/// </summary>

	public void MakePrivate () { mListenerPort = 0; }

	/// <summary>
	/// Thread that will be processing incoming data.
	/// </summary>

	void ThreadFunction ()
	{
#if MULTI_THREADED
		for (; ; )
#endif
		{
			bool received = false;

			lock (mLock)
			{
				Buffer buffer;
				mTime = DateTime.UtcNow.Ticks / 10000;
				IPEndPoint ip;

				// Stop the listener if the port is 0 (MakePrivate() was called)
				if (mListenerPort == 0)
				{
					if (mListener != null)
					{
						mListener.Stop();
						mListener = null;
						if (lobbyLink != null) lobbyLink.Stop();
					}
				}
				else
				{
					// Add all pending connections
					while (mListener != null && mListener.Pending())
					{
						Socket socket = mListener.AcceptSocket();

						if (socket != null)
						{
							IPEndPoint remote = socket.RemoteEndPoint as IPEndPoint;

							if (remote == null || mBan.Contains(remote.Address.ToString()))
							{
								socket.Close();
							}
							else AddPlayer(socket);
						}
					}
				}

				// Process datagrams first
				while (mUdp.listeningPort != 0 && mUdp.ReceivePacket(out buffer, out ip))
				{
					if (buffer.size > 0)
					{
						TcpPlayer player = GetPlayer(ip);

						if (player != null)
						{
							if (!player.udpIsUsable) player.udpIsUsable = true;

							try
							{
								if (ProcessPlayerPacket(buffer, player, false))
									received = true;
							}
							catch (System.Exception ex)
							{
								Tools.LogError(ex.Message, ex.StackTrace, true);
								RemovePlayer(player);
							}
						}
						else if (buffer.size > 0)
						{
							Packet request = Packet.Empty;

							try
							{
								BinaryReader reader = buffer.BeginReading();
								request = (Packet)reader.ReadByte();

								if (request == Packet.RequestActivateUDP)
								{
									int pid = reader.ReadInt32();
									player = GetPlayer(pid);

									// This message must arrive after RequestSetUDP which sets the UDP end point.
									// We do an additional step here because in some cases UDP port can be changed
									// by the router so that it appears that packets come from a different place.
									if (player != null && player.udpEndPoint != null && player.udpEndPoint.Address == ip.Address)
									{
										player.udpEndPoint = ip;
										player.udpIsUsable = true;
										mUdp.SendEmptyPacket(player.udpEndPoint);
									}
								}
								else if (request == Packet.RequestPing)
								{
									BeginSend(Packet.ResponsePing);
									EndSend(ip);
								}
							}
							catch (System.Exception ex)
							{
								if (player != null) player.LogError(ex.Message, ex.StackTrace);
								else Tools.LogError(ex.Message, ex.StackTrace);
								RemovePlayer(player);
							}
						}
					}
					buffer.Recycle();
				}

				// Process player connections next
				for (int i = 0; i < mPlayers.size; )
				{
					TcpPlayer player = mPlayers[i];

					// Remove disconnected players
					if (!player.isSocketConnected)
					{
						RemovePlayer(player);
						continue;
					}

					// Process up to 100 packets at a time
					for (int b = 0; b < 100 && player.ReceivePacket(out buffer); ++b)
					{
						if (buffer.size > 0)
						{
#if MULTI_THREADED
							try
							{
								if (ProcessPlayerPacket(buffer, player, true))
									received = true;
							}
#if STANDALONE
							catch (System.Exception ex)
							{
								player.LogError(ex.Message, ex.StackTrace);
								RemovePlayer(player);
								buffer.Recycle();
								continue;
							}
#else
							catch (System.Exception ex)
							{
								player.LogError(ex.Message, ex.StackTrace);
								RemovePlayer(player);
							}
#endif
#else
							if (ProcessPlayerPacket(buffer, player, true))
								received = true;
#endif
						}
						buffer.Recycle();
					}

					// Time out -- disconnect this player
					if (player.stage == TcpProtocol.Stage.Connected)
					{
						// If the player doesn't send any packets in a while, disconnect him
						if (player.timeoutTime > 0 && player.lastReceivedTime + player.timeoutTime < mTime)
						{
							RemovePlayer(player);
							continue;
						}
					}
					else if (player.lastReceivedTime + 2000 < mTime)
					{
						RemovePlayer(player);
						continue;
					}
					++i;
				}
			}
#if MULTI_THREADED
			if (!received) Thread.Sleep(1);
#endif
		}
	}

	/// <summary>
	/// Add a new player entry.
	/// </summary>

	TcpPlayer AddPlayer (Socket socket)
	{
		TcpPlayer player = new TcpPlayer();
		player.id = 0;
		player.name = "Guest";
		player.StartReceiving(socket);
		mPlayers.Add(player);
		return player;
	}

	/// <summary>
	/// Remove the specified player.
	/// </summary>

	void RemovePlayer (TcpPlayer p)
	{
		if (p != null)
		{
#if STANDALONE
			if (p.id != 0) Tools.Log(p.name + " (" + p.address + "): Disconnected [" + p.id + "]");
#endif
			SendLeaveChannel(p, false);

			p.Release();
			mPlayers.Remove(p);

			if (p.udpEndPoint != null)
			{
				mDictionaryEP.Remove(p.udpEndPoint);
				p.udpEndPoint = null;
				p.udpIsUsable = false;
			}

			if (p.id != 0)
			{
				if (mDictionaryID.Remove(p.id))
				{
					if (lobbyLink != null) lobbyLink.SendUpdate(this);
					if (onPlayerDisconnect != null) onPlayerDisconnect(p);

					BinaryWriter writer = BeginSend(Packet.PlayerDisconnected);
					writer.Write(p.id);
					writer.Write(p.name);
					EndSend(true, p);
				}
				p.id = 0;
			}
		}
	}

	/// <summary>
	/// Retrieve a player by their ID.
	/// </summary>

	TcpPlayer GetPlayer (int id)
	{
		TcpPlayer p = null;
		mDictionaryID.TryGetValue(id, out p);
		return p;
	}

	/// <summary>
	/// Retrieve a player by their name.
	/// </summary>

	TcpPlayer GetPlayer (string name)
	{
		if (!string.IsNullOrEmpty(name))
		{
			// Exact name match
			for (int i = 0; i < mPlayers.size; ++i)
			{
				if (mPlayers[i].name == name)
					return mPlayers[i];
			}

			// Partial name match
			for (int i = 0; i < mPlayers.size; ++i)
			{
				if (mPlayers[i].name.IndexOf(name, StringComparison.CurrentCultureIgnoreCase) != -1)
					return mPlayers[i];
			}

			// Alias match
			for (int i = 0; i < mPlayers.size; ++i)
			{
				TcpPlayer p = mPlayers[i];
				if (p.HasAlias(name)) return p;
			}
		}
		return null;
	}

	/// <summary>
	/// Retrieve a player by their UDP end point.
	/// </summary>

	TcpPlayer GetPlayer (IPEndPoint ip)
	{
		TcpPlayer p = null;
		mDictionaryEP.TryGetValue(ip, out p);
		return p;
	}

	/// <summary>
	/// Change the player's UDP end point and update the local dictionary.
	/// </summary>

	void SetPlayerUdpEndPoint (TcpPlayer player, IPEndPoint udp)
	{
		if (player.udpEndPoint != null) mDictionaryEP.Remove(player.udpEndPoint);
		player.udpEndPoint = udp;
		player.udpIsUsable = false;
		if (udp != null) mDictionaryEP[udp] = player;
	}

	/// <summary>
	/// Create a new channel (or return an existing one).
	/// </summary>

	Channel CreateChannel (int channelID, out bool isNew)
	{
		Channel channel;

		for (int i = 0; i < mChannels.size; ++i)
		{
			channel = mChannels[i];
			
			if (channel.id == channelID)
			{
				isNew = false;
				if (channel.closed) return null;
				return channel;
			}
		}

		channel = new Channel();
		channel.id = channelID;
		mChannels.Add(channel);
		isNew = true;
		return channel;
	}

	/// <summary>
	/// Check to see if the specified channel exists.
	/// </summary>

	bool ChannelExists (int id)
	{
		for (int i = 0; i < mChannels.size; ++i) if (mChannels[i].id == id) return true;
		return false;
	}

	/// <summary>
	/// Start the sending process.
	/// </summary>

	BinaryWriter BeginSend (Packet type)
	{
		mBuffer = Buffer.Create();
		BinaryWriter writer = mBuffer.BeginPacket(type);
		return writer;
	}

	/// <summary>
	/// Send the outgoing buffer to the specified remote destination.
	/// </summary>

	void EndSend (IPEndPoint ip)
	{
		mBuffer.EndPacket();
		mUdp.Send(mBuffer, ip);
		mBuffer.Recycle();
		mBuffer = null;
	}

	/// <summary>
	/// Send the outgoing buffer to the specified player.
	/// </summary>

	void EndSend (bool reliable, TcpPlayer player)
	{
		mBuffer.EndPacket();
		if (mBuffer.size > 1024) reliable = true;

		if (reliable || !player.udpIsUsable || player.udpEndPoint == null || !mAllowUdp)
		{
			player.SendTcpPacket(mBuffer);
		}
		else mUdp.Send(mBuffer, player.udpEndPoint);
		
		mBuffer.Recycle();
		mBuffer = null;
	}

	/// <summary>
	/// Send the outgoing buffer to all players in the specified channel.
	/// </summary>

	void EndSend (bool reliable, Channel channel, TcpPlayer exclude)
	{
		mBuffer.EndPacket();
		if (mBuffer.size > 1024) reliable = true;

		for (int i = 0; i < channel.players.size; ++i)
		{
			TcpPlayer player = channel.players[i];
			
			if (player.stage == TcpProtocol.Stage.Connected && player != exclude)
			{
				if (reliable || !player.udpIsUsable || player.udpEndPoint == null || !mAllowUdp)
				{
					player.SendTcpPacket(mBuffer);
				}
				else mUdp.Send(mBuffer, player.udpEndPoint);
			}
		}

		mBuffer.Recycle();
		mBuffer = null;
	}

	/// <summary>
	/// Send the outgoing buffer to all connected players.
	/// </summary>

	void EndSend (bool reliable)
	{
		mBuffer.EndPacket();
		if (mBuffer.size > 1024) reliable = true;

		for (int i = 0; i < mChannels.size; ++i)
		{
			Channel channel = mChannels[i];

			for (int b = 0; b < channel.players.size; ++b)
			{
				TcpPlayer player = channel.players[b];
				
				if (player.stage == TcpProtocol.Stage.Connected)
				{
					if (reliable || !player.udpIsUsable || player.udpEndPoint == null || !mAllowUdp)
					{
						player.SendTcpPacket(mBuffer);
					}
					else mUdp.Send(mBuffer, player.udpEndPoint);
				}
			}
		}
		mBuffer.Recycle();
		mBuffer = null;
	}

	/// <summary>
	/// Send the outgoing buffer to all players in the specified channel.
	/// </summary>

	void SendToChannel (bool reliable, Channel channel, Buffer buffer)
	{
		mBuffer.MarkAsUsed();
		if (mBuffer.size > 1024) reliable = true;

		for (int i = 0; i < channel.players.size; ++i)
		{
			TcpPlayer player = channel.players[i];
			
			if (player.stage == TcpProtocol.Stage.Connected)
			{
				if (reliable || !player.udpIsUsable || player.udpEndPoint == null || !mAllowUdp)
				{
					player.SendTcpPacket(mBuffer);
				}
				else mUdp.Send(mBuffer, player.udpEndPoint);
			}
		}
		mBuffer.Recycle();
	}

	/// <summary>
	/// Have the specified player assume control of the channel.
	/// </summary>

	void SendSetHost (TcpPlayer player)
	{
		if (player.channel != null && player.channel.host != player)
		{
			player.channel.host = player;
			BinaryWriter writer = BeginSend(Packet.ResponseSetHost);
			writer.Write(player.id);
			EndSend(true, player.channel, null);
		}
	}

	// Temporary buffer used in SendLeaveChannel below
	List<uint> mTemp = new List<uint>();

	/// <summary>
	/// Leave the channel the player is in.
	/// </summary>

	void SendLeaveChannel (TcpPlayer player, bool notify)
	{
		Channel ch = player.channel;

		if (ch != null)
		{
			// Remove this player from the channel
			ch.RemovePlayer(player, mTemp);
			player.channel = null;

			// Are there other players left?
			if (ch.players.size > 0)
			{
				BinaryWriter writer;

				// Inform the other players that the player's objects should be destroyed
				if (mTemp.size > 0)
				{
					writer = BeginSend(Packet.ResponseDestroy);
					writer.Write((ushort)mTemp.size);
					for (int i = 0; i < mTemp.size; ++i) writer.Write(mTemp[i]);
					EndSend(true, ch, null);
				}

				// If this player was the host, choose a new host
				if (ch.host == null) SendSetHost(ch.players[0]);

				// Inform everyone of this player leaving the channel
				writer = BeginSend(Packet.ResponsePlayerLeft);
				writer.Write(player.id);
				EndSend(true, ch, null);
			}
			else if (!ch.persistent)
			{
				// No other players left -- delete this channel
				mChannels.Remove(ch);
			}

			// Notify the player that they have left the channel
			if (notify && player.isConnected)
			{
				BeginSend(Packet.ResponseLeaveChannel);
				EndSend(true, player);
			}
		}
	}

	/// <summary>
	/// Join the specified channel.
	/// </summary>

	void SendJoinChannel (TcpPlayer player, Channel channel)
	{
		if (player.channel == channel) return;

		// Ensure the player has left the channel
		if (player.channel != null)
		{
			SendLeaveChannel(player, true);
		}
		else if (mData != null)
		{
			// Send the server data the first time
			player.BeginSend(Packet.RequestSetServerOption).Write(mData);
			player.EndSend();
		}

		// Set the player's channel
		player.channel = channel;

		// Everything else gets sent to the player, so it's faster to do it all at once
		player.FinishJoiningChannel();

		// Inform the channel that a new player is joining
		BinaryWriter writer = BeginSend(Packet.ResponsePlayerJoined);
		{
			writer.Write(player.id);
			writer.Write(string.IsNullOrEmpty(player.name) ? "Guest" : player.name);
#if STANDALONE
			if (player.data == null) writer.Write((byte)0);
			else writer.Write((byte[])player.data);
#else
			writer.WriteObject(player.data);
#endif
		}
		EndSend(true, channel, null);

		// Add this player to the channel now that the joining process is complete
		channel.players.Add(player);
	}

	/// <summary>
	/// Extra verification steps, if necessary.
	/// </summary>

	protected virtual bool Verify (BinaryReader reader) { return true; }

	/// <summary>
	/// Receive and process a single incoming packet.
	/// Returns 'true' if a packet was received, 'false' otherwise.
	/// </summary>

	bool ProcessPlayerPacket (Buffer buffer, TcpPlayer player, bool reliable)
	{
		// If the player has not yet been verified, the first packet must be an ID request
		if (player.stage == TcpProtocol.Stage.Verifying)
		{
			BinaryReader rd = player.VerifyRequestID(buffer, true);

			if (rd != null)
			{
				if (player.isAdmin || !mBan.Contains(player.name))
				{
					mDictionaryID.Add(player.id, player);

					if (mData != null)
					{
						player.BeginSend(Packet.ResponseServerOptions).Write(mData);
						player.EndSend();
					}

					if (lobbyLink != null) lobbyLink.SendUpdate(this);
					if (onPlayerConnect != null) onPlayerConnect(player);

					BinaryWriter writer = BeginSend(Packet.PlayerConnected);
					writer.Write(player.id);
					writer.Write(player.name);
					EndSend(true, player);
					return true;
				}
				else
				{
					player.Log("User is banned");
					RemovePlayer(player);
					return false;
				}
			}

			Tools.Print(player.address + " has failed the verification step");
			RemovePlayer(player);
			return false;
		}

		BinaryReader reader = buffer.BeginReading();
		Packet request = (Packet)reader.ReadByte();

//		if (request != Packet.RequestPing) Tools.Print("Server: " + request + " " + buffer.position + " " + buffer.size);

		switch (request)
		{
			case Packet.Empty:
			{
				break;
			}
			case Packet.Error:
			{
				player.LogError(reader.ReadString());
				break;
			}
			case Packet.Disconnect:
			{
				RemovePlayer(player);
				break;
			}
			case Packet.RequestPing:
			{
				// Respond with a ping back
				BeginSend(Packet.ResponsePing);
				EndSend(true, player);
				break;
			}
			case Packet.RequestSetUDP:
			{
				int port = reader.ReadUInt16();

				if (port != 0 && mUdp.isActive)
				{
					IPAddress ip = new IPAddress(player.tcpEndPoint.Address.GetAddressBytes());
					SetPlayerUdpEndPoint(player, new IPEndPoint(ip, port));
				}
				else SetPlayerUdpEndPoint(player, null);

				// Let the player know if we are hosting an active UDP connection
				ushort udp = mUdp.isActive ? (ushort)mUdp.listeningPort : (ushort)0;
				BeginSend(Packet.ResponseSetUDP).Write(udp);
				EndSend(true, player);

				// Send an empty packet to the target player to open up UDP for communication
				if (player.udpEndPoint != null) mUdp.SendEmptyPacket(player.udpEndPoint);
				break;
			}
			case Packet.RequestActivateUDP:
			{
				player.udpIsUsable = true;
				if (player.udpEndPoint != null) mUdp.SendEmptyPacket(player.udpEndPoint);
				break;
			}
			case Packet.RequestJoinChannel:
			{
				// Join the specified channel
				int		channelID	= reader.ReadInt32();
				string	pass		= reader.ReadString();
				string	levelName	= reader.ReadString();
				bool	persist		= reader.ReadBoolean();
				ushort	playerLimit = reader.ReadUInt16();

#if STANDALONE && WINDWARD
				if (player.aliases == null || player.aliases.size == 0)
				{
					Ban(player, player);
					return false;
				}
#endif
				// Join a random existing channel or create a new one
				if (channelID == -2)
				{
					bool randomLevel = string.IsNullOrEmpty(levelName);
					channelID = -1;

					for (int i = 0; i < mChannels.size; ++i)
					{
						Channel ch = mChannels[i];

						if (ch.isOpen && (randomLevel || levelName.Equals(ch.level)) &&
							(string.IsNullOrEmpty(ch.password) || (ch.password == pass)))
						{
							channelID = ch.id;
							break;
						}
					}

					// If no level name has been specified and no channels were found, we're done
					if (randomLevel && channelID == -1)
					{
						BinaryWriter writer = BeginSend(Packet.ResponseJoinChannel);
						writer.Write(false);
						writer.Write("No suitable channels found");
						EndSend(true, player);
						break;
					}
				}

				// Join a random new channel
				if (channelID == -1)
				{
					channelID = 10001 + mRandom.Next(100000000);

					for (int i = 0; i < 1000; ++i)
					{
						if (!ChannelExists(channelID)) break;
						channelID = 10001 + mRandom.Next(100000000);
					}
				}

				if (player.channel == null || player.channel.id != channelID)
				{
					bool isNew;
					Channel channel = CreateChannel(channelID, out isNew);

					if (channel == null || !channel.isOpen)
					{
						BinaryWriter writer = BeginSend(Packet.ResponseJoinChannel);
						writer.Write(false);
						writer.Write("The requested channel is closed");
						EndSend(true, player);
					}
					else if (isNew)
					{
						channel.password = pass;
						channel.persistent = persist;
						channel.level = levelName;
						channel.playerLimit = playerLimit;

						SendJoinChannel(player, channel);
					}
					else if (string.IsNullOrEmpty(channel.password) || (channel.password == pass))
					{
						SendJoinChannel(player, channel);
					}
					else
					{
						BinaryWriter writer = BeginSend(Packet.ResponseJoinChannel);
						writer.Write(false);
						writer.Write("Wrong password");
						EndSend(true, player);
					}
				}
				break;
			}
			case Packet.RequestSetName:
			{
				// Change the player's name
				player.name = reader.ReadString();

				if (mBan.Contains(player.name))
				{
					player.Log("FAILED a ban check: " + player.name);
					RemovePlayer(player);
					break;
				}

				BinaryWriter writer = BeginSend(Packet.ResponseRenamePlayer);
				writer.Write(player.id);
				writer.Write(player.name);

				if (player.channel != null)
				{
					EndSend(true, player.channel, null);
				}
				else
				{
					EndSend(true, player);
				}
				break;
			}
			case Packet.SyncPlayerData:
			{
				// 4 bytes for size, 1 byte for ID
				int origin = buffer.position - 5;

				// Find the player
				TcpPlayer target = GetPlayer(reader.ReadInt32());
				if (target == null) break;

				// Read the player's custom data
				if (buffer.size > 1)
				{
#if STANDALONE
					target.data = reader.ReadBytes(buffer.size);
#else
					target.data = reader.ReadObject();
#endif
				}
				else target.data = null;

				if (target.channel != null)
				{
					// We want to forward the packet as-is
					buffer.position = origin;

					// Forward the packet to everyone except the sender
					for (int i = 0; i < target.channel.players.size; ++i)
					{
						TcpPlayer tp = target.channel.players[i];

						if (tp != player)
						{
							if (reliable || !tp.udpIsUsable || tp.udpEndPoint == null || !mAllowUdp)
							{
								tp.SendTcpPacket(buffer);
							}
							else mUdp.Send(buffer, tp.udpEndPoint);
						}
					}
				}
				else if (target != player)
				{
					buffer.position = origin;
					target.SendTcpPacket(buffer);
				}
				break;
			}
			case Packet.RequestSaveFile:
			{
				try
				{
					string fileName = reader.ReadString();
					byte[] data = reader.ReadBytes(reader.ReadInt32());
					
					if (!string.IsNullOrEmpty(fileName))
					{
						if (SaveFile(fileName, data))
						{
							player.Log("Saved " + fileName);
						}
						else player.LogError("Unable to save " + fileName);
					}
				}
				catch (Exception ex)
				{
					player.LogError(ex.Message, ex.StackTrace);
					RemovePlayer(player);
				}
				break;
			}
			case Packet.RequestLoadFile:
			{
				string fn = reader.ReadString();
				byte[] data = LoadFile(fn);

				BinaryWriter writer = BeginSend(Packet.ResponseLoadFile);
				writer.Write(fn);

				if (data != null)
				{
					writer.Write(data.Length);
					writer.Write(data);
				}
				else writer.Write(0);

				EndSend(true, player);
				break;
			}
			case Packet.RequestDeleteFile:
			{
				string fileName = reader.ReadString();

				if (!string.IsNullOrEmpty(fileName))
				{
					if (DeleteFile(fileName))
					{
						player.Log("Deleted " + fileName);
					}
					else player.LogError("Unable to delete " + fileName);
				}
				break;
			}
			case Packet.RequestNoDelay:
			{
				player.noDelay = reader.ReadBoolean();
				break;
			}
			case Packet.RequestChannelList:
			{
				BinaryWriter writer = BeginSend(Packet.ResponseChannelList);

				int count = 0;
				for (int i = 0; i < mChannels.size; ++i)
					if (!mChannels[i].closed) ++count;

				writer.Write(count);

				for (int i = 0; i < mChannels.size; ++i)
				{
					Channel ch = mChannels[i];

					if (!ch.closed)
					{
						writer.Write(ch.id);
						writer.Write((ushort)ch.players.size);
						writer.Write(ch.playerLimit);
						writer.Write(!string.IsNullOrEmpty(ch.password));
						writer.Write(ch.persistent);
						writer.Write(ch.level);
						writer.Write(ch.data);
					}
				}
				EndSend(true, player);
				break;
			}
			case Packet.ServerLog:
			{
#if UNITY_EDITOR
				reader.ReadString();
#else
				string s = reader.ReadString();
				player.Log(s);
 #if WINDWARD
				if (s.Contains("has entered region"))
					Ban(player, player);
 #endif
#endif
				break;
			}
			case Packet.RequestSetTimeout:
			{
				// The passed value is in seconds, but the stored value is in milliseconds (to avoid a math operation)
				player.timeoutTime = reader.ReadInt32() * 1000;
				break;
			}
			case Packet.ForwardToPlayer:
			{
				// Forward this packet to the specified player
				TcpPlayer target = GetPlayer(reader.ReadInt32());

				if (target != null && target.isConnected)
				{
					// Reset the position back to the beginning (4 bytes for size, 1 byte for ID, 4 bytes for player)
					buffer.position = buffer.position - 9;
					target.SendTcpPacket(buffer);
				}
				break;
			}
			case Packet.ForwardByName:
			{
				int start = buffer.position - 5;
				string name = reader.ReadString();
				TcpPlayer target = GetPlayer(name);

				if (target != null && target.isConnected)
				{
					buffer.position = start;
					target.SendTcpPacket(buffer);
				}
				else if (reliable)
				{
					BeginSend(Packet.ForwardTargetNotFound).Write(name);
					EndSend(true, player);
				}
				break;
			}
			case Packet.BroadcastAdmin:
			case Packet.Broadcast:
			{
				//Tools.Print("Broadcast: " + player.name + ", " + player.address);

				if (player.nextBroadcast < mTime)
				{
					player.nextBroadcast = mTime + 500;
					player.broadcastCount = 0;
				}
				else if (++player.broadcastCount > 5)
				{
					player.Log("SPAM filter trigger! " + (player.channel != null ? player.channel.id : -1));
					RemovePlayer(player);
					break;
				}
				else if (player.broadcastCount > 2)
				{
					player.Log("Possible spam! " + (player.channel != null ? player.channel.id : -1));
				}

				// 4 bytes for size, 1 byte for ID
				buffer.position = buffer.position - 5;

				// Forward the packet to everyone connected to the server
				for (int i = 0; i < mPlayers.size; ++i)
				{
					TcpPlayer tp = mPlayers[i];
					if (!tp.isConnected) continue;
					if (request == Packet.BroadcastAdmin && !tp.isAdmin) continue;

					if (reliable || !tp.udpIsUsable || tp.udpEndPoint == null || !mAllowUdp)
					{
						tp.SendTcpPacket(buffer);
					}
					else mUdp.Send(buffer, tp.udpEndPoint);
				}
				break;
			}
			case Packet.RequestVerifyAdmin:
			{
				string pass = reader.ReadString();

				if (!string.IsNullOrEmpty(pass) && mAdmin.Contains(pass))
				{
					if (!player.isAdmin)
					{
						player.isAdmin = true;
						player.Log("Admin verified");

						player.BeginSend(Packet.ResponseVerifyAdmin).Write(player.id);
						player.EndSend();
					}
				}
				else
				{
					player.LogError("Tried to authenticate as admin and failed (" + pass + ")");
					RemovePlayer(player);
				}
				break;
			}
			case Packet.RequestCreateAdmin:
			{
				string s = reader.ReadString();

				if (player.isAdmin)
				{
					if (!mAdmin.Contains(s)) mAdmin.Add(s);
					player.Log("Added an admin (" + s + ")");
					Tools.SaveList("ServerConfig/admin.txt", mAdmin);
				}
				else
				{
					player.LogError("Tried to add an admin (" + s + ") and failed");
					RemovePlayer(player);
				}
				break;
			}
			case Packet.RequestRemoveAdmin:
			{
				string s = reader.ReadString();

				// First administrator can't be removed
				if (player.isAdmin && (mAdmin.size == 0 || mAdmin[0] != s))
				{
					mAdmin.Remove(s);
					player.Log("Removed an admin (" + s + ")");
					Tools.SaveList("ServerConfig/admin.txt", mAdmin);
				}
				else
				{
					player.LogError("Tried to remove an admin (" + s + ") without authorization", null);
					RemovePlayer(player);
				}
				break;
			}
			case Packet.RequestSetAlias:
			{
				string s = reader.ReadString();
				if (!SetAlias(player, s)) break;

				if (mAdmin.Contains(s))
				{
					player.isAdmin = true;
					player.Log("Admin verified");

					player.BeginSend(Packet.ResponseVerifyAdmin).Write(player.id);
					player.EndSend();
				}
#if WINDWARD
				if (player.aliases != null && player.aliases.size > 3) RemovePlayer(player);
#endif
				break;
			}
			case Packet.RequestUnban:
			{
				string s = reader.ReadString();

				if (player.isAdmin)
				{
					mBan.Remove(s);
					Tools.SaveList("ServerConfig/ban.txt", mBan);
					player.Log("Removed an banned keyword (" + s + ")");
				}
				else
				{
					player.LogError("Tried to unban (" + s + ") without authorization", null);
					RemovePlayer(player);
				}
				break;
			}
			case Packet.RequestLogPlayers:
			{
				if (player.isAdmin)
				{
					for (int i = 0; i < mPlayers.size; ++i)
					{
						TcpPlayer p = mPlayers[i];
						if (p.isAdmin) p.Log(p.channel.id + " ADMIN");
						p.Log(p.channel.id.ToString());
					}
				}
				break;
			}
			case Packet.RequestSetBanList:
			{
				string s = reader.ReadString();

				if (player.isAdmin)
				{
					if (!string.IsNullOrEmpty(s))
					{
						string[] lines = s.Split('\n');
						mBan.Clear();
						for (int i = 0; i < lines.Length; ++i) mBan.Add(lines[i]);
					}
					else mBan.Clear();
				}
				else
				{
					player.LogError("Tried to set the ban list without authorization", null);
					RemovePlayer(player);
				}
				break;
			}
			case Packet.RequestReloadServerData:
			{
				if (player.isAdmin)
				{
					Tools.LoadList("ServerConfig/ban.txt", mBan);
					Tools.LoadList("ServerConfig/admin.txt", mAdmin);
					LoadData();

					if (mData == null) mData = new DataNode("Version", Player.version);

					Buffer buff = Buffer.Create();
					buff.BeginPacket(Packet.ResponseServerOptions).Write(mData);
					buff.EndPacket();

					// Forward the packet to everyone connected to the server
					for (int i = 0; i < mPlayers.size; ++i)
					{
						TcpPlayer tp = mPlayers[i];
						tp.SendTcpPacket(buff);
					}
					buff.Recycle();
				}
				else
				{
					player.LogError("Tried to request reloaded server data without authorization", null);
					RemovePlayer(player);
				}
				break;
			}
			case Packet.RequestSetServerOption:
			{
				if (player.isAdmin)
				{
					if (mData == null) mData = new DataNode("Version", Player.version);

					DataNode child = reader.ReadDataNode();

					if (child.value == null && child.children.size == 0)
					{
						mData.RemoveChild(child.name);
						child = mData;
					}
					else child = mData.ReplaceChild(child);

					Buffer buff = Buffer.Create();
					buff.BeginPacket(Packet.ResponseSetServerOption).Write(child);
					buff.EndPacket();

					// Forward the packet to everyone connected to the server
					for (int i = 0; i < mPlayers.size; ++i)
					{
						TcpPlayer tp = mPlayers[i];
						tp.SendTcpPacket(buff);
					}
					buff.Recycle();
				}
				else
				{
					player.LogError("Tried to set the ban server data without authorization", null);
					RemovePlayer(player);
				}
				break;
			}
			case Packet.RequestKick:
			{
				int id = reader.ReadInt32();
				string s = (id != 0) ? null : reader.ReadString();
				TcpPlayer other = (id != 0) ? GetPlayer(id) : GetPlayer(s);

				if (player.isAdmin || other == player)
				{
					if (other != null)
					{
						player.Log("Kicked " + other.name + " (" + other.address + ")");
						RemovePlayer(other);
					}
				}
				else
				{
					player.LogError("Tried to kick " + (other != null ? other.name : s) + " without authorization", null);
					RemovePlayer(player);
				}
				break;
			}
			case Packet.RequestBan:
			{
				int id = reader.ReadInt32();
				string s = (id != 0) ? null : reader.ReadString();
				TcpPlayer other = (id != 0) ? GetPlayer(id) : GetPlayer(s);

				bool playerBan = (other == player && mData != null && mData.GetChild<bool>("playersCanBan"));

				if (player.isAdmin || playerBan)
				{
					if (other != null)
					{
						Ban(player, other);
					}
					else if (id == 0)
					{
						player.Log("BANNED " + s);
						string banText = "// [" + s + "] banned by [" + player.name + "]- " + (player.aliases != null &&
							player.aliases.size > 0 ? player.aliases[0] : player.address);
						AddUnique(mBan, banText);
						AddUnique(mBan, s);
						Tools.SaveList("ServerConfig/ban.txt", mBan);
					}
				}
				else if (!playerBan)
				{
					// Do nothing -- players can't ban other players, even themselves for security reasons
				}
				else
				{
					player.LogError("Tried to ban " + (other != null ? other.name : s) + " without authorization", null);
					RemovePlayer(player);
				}
				break;
			}
			case Packet.RequestGetFileList:
			{
				string original = reader.ReadString();
				string path = Tools.FindDirectory(original, player.isAdmin);

				BinaryWriter writer = player.BeginSend(Packet.ResponseGetFileList);
				writer.Write(original);

				if (!string.IsNullOrEmpty(path))
				{
					string[] files = Tools.GetFiles(path);
					writer.Write(files.Length);
					for (int i = 0, imax = files.Length; i < imax; ++i)
						writer.Write(files[i]);
				}
				else
				{
					writer.Write(0);
				}
				player.EndSend();
				break;
			}
			case Packet.RequestLockChannel:
			{
				bool locked = reader.ReadBoolean();

				if (player.isAdmin)
				{
					if (player.channel != null)
					{
						player.channel.locked = locked;
						BeginSend(Packet.ResponseLockChannel).Write(locked);
						EndSend(true);
					}
				}
				else
				{
					player.LogError("RequestLockChannel(" + locked + ") without authorization", null);
					RemovePlayer(player);
				}
				break;
			}
			default:
			{
				if (player.channel != null && (int)request < (int)Packet.UserPacket)
				{
					// Other packets can only be processed while in a channel
					if (request >= Packet.ForwardToAll && request <= Packet.ForwardToPlayerSaved)
					{
						ProcessForwardPacket(player, buffer, reader, request, reliable);
					}
					else
					{
						ProcessChannelPacket(player, buffer, reader, request);
					}
				}
				else if (onCustomPacket != null)
				{
					onCustomPacket(player, buffer, reader, request, reliable);
				}
				break;
			}
		}
		return true;
	}

	/// <summary>
	/// Set an alias and check it against the ban list.
	/// </summary>

	bool SetAlias (TcpPlayer player, string s)
	{
		if (mBan.Contains(s))
		{
			player.Log("FAILED a ban check: " + s);
			RemovePlayer(player);
			return false;
		}
		else
		{
			player.Log("Passed a ban check: " + s);
			if (player.aliases == null) player.aliases = new List<string>();
			AddUnique(player.aliases, s);
			return true;
		}
	}

	/// <summary>
	/// Ban the specified player.
	/// </summary>

	void Ban (TcpPlayer player, TcpPlayer other)
	{
		player.Log("BANNED " + other.name + " (" + (other.aliases != null &&
			other.aliases.size > 0 ? other.aliases[0] : other.address) + ")");

		// Just to show the name of the player
		string banText = "// [" + other.name + "]";

		if (player != other)
		{
			banText += " banned by [" + player.name + "]- " + (other.aliases != null &&
				player.aliases.size > 0 ? player.aliases[0] : player.address);
		}

		AddUnique(mBan, banText);
		AddUnique(mBan, other.tcpEndPoint.Address.ToString());

		if (other.aliases != null)
			for (int i = 0; i < other.aliases.size; ++i)
				AddUnique(mBan, other.aliases[i]);

		Tools.SaveList("ServerConfig/ban.txt", mBan);
		RemovePlayer(other);
	}

	/// <summary>
	/// Process a packet that's meant to be forwarded.
	/// </summary>

	void ProcessForwardPacket (TcpPlayer player, Buffer buffer, BinaryReader reader, Packet request, bool reliable)
	{
		// We can't send unreliable packets if UDP is not active
		if (!mUdp.isActive || buffer.size > 1024) reliable = true;

		switch (request)
		{
			case Packet.ForwardToHost:
			{
				// Reset the position back to the beginning (4 bytes for size, 1 byte for ID)
				buffer.position = buffer.position - 5;

				// Forward the packet to the channel's host
				if (reliable || !player.udpIsUsable || player.channel.host.udpEndPoint == null || !mAllowUdp)
				{
					player.channel.host.SendTcpPacket(buffer);
				}
				else mUdp.Send(buffer, player.channel.host.udpEndPoint);
				break;
			}
			case Packet.ForwardToPlayerSaved:
			{
				if (player.channel.locked && !player.isAdmin)
				{
					player.LogError("Tried to call a persistent RFC while the channel is locked", null);
					RemovePlayer(player);
					break;
				}

				// 4 bytes for size, 1 byte for ID
				int origin = buffer.position - 5;

				// Figure out who the intended recipient is
				TcpPlayer targetPlayer = GetPlayer(reader.ReadInt32());

				// Save this function call
				uint target = reader.ReadUInt32();
				string funcName = ((target & 0xFF) == 0) ? reader.ReadString() : null;
				buffer.position = origin;
				player.channel.CreateRFC(target, funcName, buffer);

				// Forward the packet to the target player
				if (targetPlayer != null && targetPlayer.isConnected)
				{
					if (reliable || !targetPlayer.udpIsUsable || targetPlayer.udpEndPoint == null || !mAllowUdp)
					{
						targetPlayer.SendTcpPacket(buffer);
					}
					else mUdp.Send(buffer, targetPlayer.udpEndPoint);
				}
				break;
			}
			default:
			{
				// We want to exclude the player if the request was to forward to others
				TcpPlayer exclude = (
					request == Packet.ForwardToOthers ||
					request == Packet.ForwardToOthersSaved) ? player : null;

				// 4 bytes for size, 1 byte for ID
				int origin = buffer.position - 5;

				// If the request should be saved, let's do so
				if (request == Packet.ForwardToAllSaved || request == Packet.ForwardToOthersSaved)
				{
					if (player.channel.locked && !player.isAdmin)
					{
						player.LogError("Tried to call a persistent RFC while the channel is locked", null);
						RemovePlayer(player);
						break;
					}

					uint target = reader.ReadUInt32();
					string funcName = ((target & 0xFF) == 0) ? reader.ReadString() : null;
					buffer.position = origin;
					player.channel.CreateRFC(target, funcName, buffer);
				}
				else buffer.position = origin;

				// Forward the packet to everyone except the sender
				for (int i = 0; i < player.channel.players.size; ++i)
				{
					TcpPlayer tp = player.channel.players[i];
					
					if (tp != exclude)
					{
						if (reliable || !tp.udpIsUsable || tp.udpEndPoint == null || !mAllowUdp)
						{
							tp.SendTcpPacket(buffer);
						}
						else mUdp.Send(buffer, tp.udpEndPoint);
					}
				}
				break;
			}
		}
	}

	/// <summary>
	/// Process a packet from the player.
	/// </summary>

	void ProcessChannelPacket (TcpPlayer player, Buffer buffer, BinaryReader reader, Packet request)
	{
		switch (request)
		{
			case Packet.RequestCreate:
			{
				if (!player.channel.locked)
				{
					// Create a new object
					ushort objectIndex = reader.ReadUInt16();
					byte type = reader.ReadByte();
					uint uniqueID = 0;

					if (type != 0)
					{
						uniqueID = --player.channel.objectCounter;

						// 1-32767 is reserved for existing scene objects.
						// 32768 - 16777215 is for dynamically created objects.
						if (uniqueID < 32768)
						{
							player.channel.objectCounter = 0xFFFFFF;
							uniqueID = 0xFFFFFF;
						}

						Channel.CreatedObject obj = new Channel.CreatedObject();
						obj.playerID = player.id;
						obj.objectIndex = objectIndex;
						obj.objectID = uniqueID;
						obj.type = type;

						if (buffer.size > 0)
						{
							obj.buffer = buffer;
							buffer.MarkAsUsed();
						}
						player.channel.created.Add(obj);
					}

					// Inform the channel
					BinaryWriter writer = BeginSend(Packet.ResponseCreate);
					writer.Write(player.id);
					writer.Write(objectIndex);
					writer.Write(uniqueID);
					if (buffer.size > 0) writer.Write(buffer.buffer, buffer.position, buffer.size);
					EndSend(true, player.channel, null);
				}
				else
				{
					player.LogError("Tried to create an object in a locked channel", null);
					RemovePlayer(player);
				}
				break;
			}
			case Packet.RequestDestroy:
			{
				if (!player.channel.locked)
				{
					// Destroy the specified network object
					uint uniqueID = reader.ReadUInt32();

					// Remove this object
					if (player.channel.DestroyObject(uniqueID))
					{
						// Inform all players in the channel that the object should be destroyed
						BinaryWriter writer = BeginSend(Packet.ResponseDestroy);
						writer.Write((ushort)1);
						writer.Write(uniqueID);
						EndSend(true, player.channel, null);
					}
				}
				else
				{
					player.LogError("Tried to destroy an object in a locked channel", null);
					RemovePlayer(player);
				}
				break;
			}
			case Packet.RequestLoadLevel:
			{
#if WINDWARD
				if (player.isAdmin)
#else
				if (!player.channel.locked)
#endif
				{
					// Change the currently loaded level
					if (player.channel.host == player)
					{
						player.channel.Reset();
						player.channel.level = reader.ReadString();

						BinaryWriter writer = BeginSend(Packet.ResponseLoadLevel);
						writer.Write(string.IsNullOrEmpty(player.channel.level) ? "" : player.channel.level);
						EndSend(true, player.channel, null);
					}
				}
				else
				{
					player.LogError("Tried to load a new level while not authorized", null);
					RemovePlayer(player);
				}
				break;
			}
			case Packet.RequestSetHost:
			{
				// Transfer the host state from one player to another
				if (player.channel.host == player)
				{
					TcpPlayer newHost = GetPlayer(reader.ReadInt32());
					if (newHost != null && newHost.channel == player.channel) SendSetHost(newHost);
				}
				break;
			}
			case Packet.RequestLeaveChannel:
			{
				SendLeaveChannel(player, true);
				break;
			}
			case Packet.RequestCloseChannel:
			{
				if (player.channel != null)
				{
					if (player.isAdmin)
					{
						player.Log("Closing channel " + player.channel.id);
						player.channel.persistent = false;
						player.channel.closed = true;
					}
					else
					{
						player.LogError("Tried to call a close channel " + player.channel.id + " while not authorized", null);
						RemovePlayer(player);
					}
				}
				break;
			}
			case Packet.RequestDeleteChannel:
			{
				if (player.isAdmin)
				{
					int id = reader.ReadInt32();
					bool dc = reader.ReadBoolean();

					player.Log("Deleting channel " + id);

					for (int i = 0; i < mChannels.size; ++i)
					{
						Channel ch = mChannels[i];

						if (ch != null && ch.id == id)
						{
							for (int b = ch.players.size; b > 0; )
							{
								TcpPlayer p = ch.players[--b];

								if (p != null)
								{
									if (dc) RemovePlayer(p);
									else SendLeaveChannel(p, true);
								}
							}

							ch.persistent = false;
							ch.closed = true;
							ch.Reset();

							mChannels.RemoveAt(i);
							break;
						}
					}
				}
				else
				{
					player.LogError("Tried to call a delete a channel while not authorized", null);
					RemovePlayer(player);
				}
				break;
			}
			case Packet.RequestSetPlayerLimit:
			{
#if WINDWARD
				if (player.isAdmin)
				{
					player.channel.playerLimit = reader.ReadUInt16();
				}
				else
				{
					player.LogError("Tried to set a player limit while not authorized", null);
					RemovePlayer(player);
				}
#else
				player.channel.playerLimit = reader.ReadUInt16();
#endif
				break;
			}
			case Packet.RequestRemoveRFC:
			{
				if (player.isAdmin || !player.channel.locked)
				{
					uint id = reader.ReadUInt32();
					string funcName = ((id & 0xFF) == 0) ? reader.ReadString() : null;
					player.channel.DeleteRFC(id, funcName);
				}
				else
				{
					player.LogError("Tried to remove an RFC while not authorized", null);
					RemovePlayer(player);
				}
				break;
			}
			case Packet.RequestSetChannelData:
			{
				if (player.isAdmin || !player.channel.locked)
				{
					player.channel.data = reader.ReadString();
					BinaryWriter writer = BeginSend(Packet.ResponseSetChannelData);
					writer.Write(player.channel.data);
					EndSend(true, player.channel, null);
				}
				else
				{
					player.LogError("Tried to set channel data in a locked channel", null);
					RemovePlayer(player);
				}
				break;
			}
		}
	}

#if !UNITY_WEBPLAYER && !UNITY_FLASH
	// Cached to reduce memory allocation
	MemoryStream mWriteStream = null;
	BinaryWriter mWriter = null;
#endif

	/// <summary>
	/// Save the server's current state into the specified file so it can be easily restored later.
	/// </summary>

	public void SaveTo (string fileName)
	{
		mFilename = fileName;

#if !UNITY_WEBPLAYER && !UNITY_FLASH
		if (mListener == null) return;

		Tools.SaveList("ServerConfig/ban.txt", mBan);
		Tools.SaveList("ServerConfig/admin.txt", mAdmin);

		if (mWriteStream == null)
		{
			mWriteStream = new MemoryStream();
			mWriter = new BinaryWriter(mWriteStream);
		}
		else
		{
			mWriter.Seek(0, SeekOrigin.Begin);
			mWriteStream.SetLength(0);
		}

		lock (mLock)
		{
			mWriter.Write(0);
			int count = 0;

			for (int i = 0; i < mChannels.size; ++i)
			{
				Channel ch = mChannels[i];

				if (!ch.closed && ch.persistent && ch.hasData)
				{
					mWriter.Write(ch.id);
					ch.SaveTo(mWriter);
					++count;
				}
			}

			if (count > 0)
			{
				mWriteStream.Seek(0, SeekOrigin.Begin);
				mWriter.Write(count);
			}
		}

		Tools.WriteFile(fileName, mWriteStream);
		SaveData();
#endif
	}

	/// <summary>
	/// Save the server's human-readable data.
	/// </summary>

	void SaveData ()
	{
		if (!string.IsNullOrEmpty(mFilename))
		{
			try
			{
				mData.Write(mFilename + ".config", DataNode.SaveType.Text, true);
			}
			catch (Exception) { }
		}
	}

	/// <summary>
	/// Load the server's human-readable data.
	/// </summary>

	void LoadData ()
	{
		if (!string.IsNullOrEmpty(mFilename))
		{
			try
			{
				byte[] data = File.ReadAllBytes(mFilename + ".config");
				mData = DataNode.Read(data, DataNode.SaveType.Text);
			}
			catch (Exception) { mData = null; }
		}
	}

	/// <summary>
	/// Load a previously saved server from the specified file.
	/// </summary>

	public bool LoadFrom (string fileName)
	{
		mFilename = fileName;

#if UNITY_WEBPLAYER || UNITY_FLASH
		// There is no file access in the web player.
		return false;
#else
		LoadData();

		byte[] bytes = Tools.ReadFile(fileName);
		if (bytes == null) return false;

		MemoryStream stream = new MemoryStream(bytes);

		lock (mLock)
		{
			try
			{
				BinaryReader reader = new BinaryReader(stream);

				int channels = reader.ReadInt32();

				for (int i = 0; i < channels; ++i)
				{
					int chID = reader.ReadInt32();
					bool isNew;
					Channel ch = CreateChannel(chID, out isNew);
					if (isNew) ch.LoadFrom(reader);
				}
			}
			catch (System.Exception ex)
			{
				Tools.LogError("Loading from " + fileName + ": " + ex.Message, ex.StackTrace);
				return false;
			}
		}
		return true;
#endif
	}
}
}
