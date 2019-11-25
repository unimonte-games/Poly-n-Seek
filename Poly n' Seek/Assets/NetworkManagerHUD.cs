// Decompiled with JetBrains decompiler
// Type: UnityEngine.Networking.NetworkManagerHUD
// Assembly: UnityEngine.Networking, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8B34E19C-EF53-416E-AE36-35C45BAFD2DE
// Assembly location: C:\Users\Blake\sandbox\unity\test-project\Library\UnityAssemblies\UnityEngine.Networking.dll

using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine.Networking.Match;
using UnityEngine.UI;

namespace UnityEngine.Networking
{
    /// <summary>
    ///   <para>An extension for the NetworkManager that displays a default HUD for controlling the network state of the game.</para>
    /// </summary>
    [AddComponentMenu("Network/NetworkManagerHUD")]
    [RequireComponent(typeof(NetworkManager))]
    [EditorBrowsable(EditorBrowsableState.Never)]

   
    public class NetworkManagerHUD : MonoBehaviour
    {
        public InputField Ip;
        public GameObject Obj;
        /// <summary>
        ///   <para>Whether to show the default control HUD at runtime.</para>
        /// </summary>
        [SerializeField]
        public bool showGUI = true;
        /// <summary>
        ///   <para>The NetworkManager associated with this HUD.</para>
        /// </summary>
        public NetworkManager manager;
        /// <summary>
        ///   <para>The horizontal offset in pixels to draw the HUD runtime GUI at.</para>
        /// </summary>
        [SerializeField]
        public int offsetX;
        /// <summary>
        ///   <para>The vertical offset in pixels to draw the HUD runtime GUI at.</para>
        /// </summary>
        [SerializeField]
        public int offsetY;
        private bool m_ShowServer;

        private void Awake()
        {
            this.manager = this.GetComponent<NetworkManager>();
        }
        public void EntrarServ()
        {
            this.manager.StartHost();
        }
        public void EntrarClient()
        {
            this.manager.StartClient();
        }
        private void Update()
        {
            this.manager.networkAddress = Ip.text;

            if (!this.manager.IsClientConnected())
                Obj.SetActive(true);

            if (this.manager.IsClientConnected())
                Obj.SetActive(false);

            if (!this.showGUI)
                return;
            if (!this.manager.IsClientConnected() && !NetworkServer.active && (Object)this.manager.matchMaker == (Object)null)
            {
                if (Application.platform != RuntimePlatform.WebGLPlayer)
                {
                    if (Input.GetKeyDown(KeyCode.S))
                        this.manager.StartServer();
                    if (Input.GetKeyDown(KeyCode.H))
                        this.manager.StartHost();
                }
                if (Input.GetKeyDown(KeyCode.C))
                    this.manager.StartClient();
            }
            if (!NetworkServer.active || !this.manager.IsClientConnected() || !Input.GetKeyDown(KeyCode.X))
                return;
            this.manager.StopHost();
        }

        private void OnGUI()
        {
            if (!this.showGUI)
                return;
            int num1 = 10 + this.offsetX;
            int num2 = 40 + this.offsetY;
            bool flag = this.manager.client == null || this.manager.client.connection == null || this.manager.client.connection.connectionId == -1;
            if (!this.manager.IsClientConnected() && !NetworkServer.active && (Object)this.manager.matchMaker == (Object)null)
            {
                if (flag)
                {
                    if (Application.platform != RuntimePlatform.WebGLPlayer)
                    {
                        if (GUI.Button(new Rect((float)num1, (float)num2, 200f, 20f), "LAN Host(H)"))
                            this.manager.StartHost();
                        num2 += 24;
                    }
                    if (GUI.Button(new Rect((float)num1, (float)num2, 105f, 20f), "LAN Client(C)"))
                        this.manager.StartClient();
                    this.manager.networkAddress = GUI.TextField(new Rect((float)(num1 + 100), (float)num2, 95f, 20f), this.manager.networkAddress);
                    int num3 = num2 + 24;
                    if (Application.platform == RuntimePlatform.WebGLPlayer)
                    {
                        GUI.Box(new Rect((float)num1, (float)num3, 200f, 25f), "(  WebGL cannot be server  )");
                        num2 = num3 + 24;
                    }
                    else
                    {
                        if (GUI.Button(new Rect((float)num1, (float)num3, 200f, 20f), "LAN Server Only(S)"))
                            this.manager.StartServer();
                        num2 = num3 + 24;
                    }
                }
                else
                {
                    GUI.Label(new Rect((float)num1, (float)num2, 200f, 20f), "Connecting to " + this.manager.networkAddress + ":" + (object)this.manager.networkPort + "..");
                    num2 += 24;
                    if (GUI.Button(new Rect((float)num1, (float)num2, 200f, 20f), "Cancel Connection Attempt"))
                        this.manager.StopClient();
                }
            }
            else
            {
                if (NetworkServer.active)
                {
                    string text = "Server: port=" + (object)this.manager.networkPort;
                    if (this.manager.useWebSockets)
                        text += " (Using WebSockets)";
                    GUI.Label(new Rect((float)num1, (float)num2, 300f, 20f), text);
                    num2 += 24;
                }
                if (this.manager.IsClientConnected())
                {
                    GUI.Label(new Rect((float)num1, (float)num2, 300f, 20f), "Client: address=" + this.manager.networkAddress + " port=" + (object)this.manager.networkPort);
                    num2 += 24;
                }
            }
            if (this.manager.IsClientConnected() && !ClientScene.ready)
            {
                if (GUI.Button(new Rect((float)num1, (float)num2, 200f, 20f), "Client Ready"))
                {
                    ClientScene.Ready(this.manager.client.connection);
                    if (ClientScene.localPlayers.Count == 0)
                        ClientScene.AddPlayer((short)0);
                }
                num2 += 24;
            }
            if (NetworkServer.active || this.manager.IsClientConnected())
            {
                if (GUI.Button(new Rect((float)num1, (float)num2, 200f, 20f), "Stop (X)"))
                    this.manager.StopHost();
                num2 += 24;
            }
            if (NetworkServer.active || this.manager.IsClientConnected() || !flag)
                return;
            int num4 = num2 + 10;
            int num5;
            if (Application.platform == RuntimePlatform.WebGLPlayer)
                GUI.Box(new Rect((float)(num1 - 5), (float)num4, 220f, 25f), "(WebGL cannot use Match Maker)");
            else if ((Object)this.manager.matchMaker == (Object)null)
            {
                if (GUI.Button(new Rect((float)num1, (float)num4, 200f, 20f), "Enable Match Maker (M)"))
                    this.manager.StartMatchMaker();
                num5 = num4 + 24;
            }
            else
            {
                if (this.manager.matchInfo == null)
                {
                    if (this.manager.matches == null)
                    {
                      /*  if (GUI.Button(new Rect((float)num1, (float)num4, 200f, 20f), "Create Internet Match"))
                            this.manager.matchMaker.CreateMatch(this.manager.matchName, this.manager.matchSize, true, string.Empty, new NetworkMatch.ResponseDelegate<CreateMatchResponse>(this.manager.OnMatchCreate));*/
                        int num3 = num4 + 24;
                        GUI.Label(new Rect((float)num1, (float)num3, 100f, 20f), "Room Name:");
                        this.manager.matchName = GUI.TextField(new Rect((float)(num1 + 100), (float)num3, 100f, 20f), this.manager.matchName);
                        int num6 = num3 + 24 + 10;
                        /*if (GUI.Button(new Rect((float)num1, (float)num6, 200f, 20f), "Find Internet Match"))
                            this.manager.matchMaker.ListMatches(0, 20, string.Empty, new NetworkMatch.ResponseDelegate<ListMatchResponse>(this.manager.OnMatchList));*/
                        num4 = num6 + 24;
                    }
                    else
                    {
                      /*  using (List<MatchDesc>.Enumerator enumerator = this.manager.matches.GetEnumerator())
                        {
                            while (enumerator.MoveNext())
                            {
                                MatchDesc current = enumerator.Current;
                                if (GUI.Button(new Rect((float)num1, (float)num4, 200f, 20f), "Join Match:" + current.name))
                                {
                                    this.manager.matchName = current.name;
                                    this.manager.matchSize = (uint)current.currentSize;
                                    this.manager.matchMaker.JoinMatch(current.networkId, string.Empty, new NetworkMatch.ResponseDelegate<JoinMatchResponse>(this.manager.OnMatchJoined));
                                }
                                num4 += 24;
                            }
                        }*/
                    }
                }
                if (GUI.Button(new Rect((float)num1, (float)num4, 200f, 20f), "Change MM server"))
                    this.m_ShowServer = !this.m_ShowServer;
                if (this.m_ShowServer)
                {
                    int num3 = num4 + 24;
                    if (GUI.Button(new Rect((float)num1, (float)num3, 100f, 20f), "Local"))
                    {
                        this.manager.SetMatchHost("localhost", 1337, false);
                        this.m_ShowServer = false;
                    }
                    int num6 = num3 + 24;
                    if (GUI.Button(new Rect((float)num1, (float)num6, 100f, 20f), "Internet"))
                    {
                        this.manager.SetMatchHost("mm.unet.unity3d.com", 443, true);
                        this.m_ShowServer = false;
                    }
                    num4 = num6 + 24;
                    if (GUI.Button(new Rect((float)num1, (float)num4, 100f, 20f), "Staging"))
                    {
                        this.manager.SetMatchHost("staging-mm.unet.unity3d.com", 443, true);
                        this.m_ShowServer = false;
                    }
                }
                int num7 = num4 + 24;
                GUI.Label(new Rect((float)num1, (float)num7, 300f, 20f), "MM Uri: " + (object)this.manager.matchMaker.baseUri);
                int num8 = num7 + 24;
                if (GUI.Button(new Rect((float)num1, (float)num8, 200f, 20f), "Disable Match Maker"))
                    this.manager.StopMatchMaker();
                num5 = num8 + 24;
            }
        }
    }
}