using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class SteamManager : Singleton<SteamManager>
{
    [SerializeField] private static uint gameAppId = 480;

    private bool connectedToSteam = false;
    public string PlayerName { get; set; }
    [SerializeField] private SteamId UserSteamId;
    [SerializeField] private SteamId TargetSteamId;

    protected override void Awake()
    {
        base.Awake();
        try
        {
            Steamworks.SteamClient.Init(gameAppId);
            if (!SteamClient.IsValid)
            {
                Debug.LogError("Steam client not valid");
                throw new System.Exception();
            }
            PlayerName = SteamClient.Name;
            UserSteamId = SteamClient.SteamId;
            connectedToSteam = true;
            Debug.Log("Steam initialized: " + PlayerName);

            Steamworks.Dispatch.OnDebugCallback = (type, str, server) =>
            {
                Debug.Log($"[Callback {type} {(server ? "server" : "client")}]");
                Debug.Log(str);
            };
            Steamworks.Dispatch.OnException = (e) =>
            {
                Debug.LogError(e.Message);
                Debug.LogError(e.StackTrace);
            };
        }
        catch (System.Exception)
        {
            Debug.LogError("Error connecting to Steam");
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
       
    }

    private void OnDisable()
    {
        Steamworks.SteamClient.Shutdown();
    }
    public bool ConnectedToSteam()
    {
        return connectedToSteam;
    }

    //public void CreateServer()
    //{
    //    var serverInit = new SteamServerInit()
    //    {
    //        GamePort = 28015,
    //        Secure = true,
    //        QueryPort = 28016
    //    };

    //    try
    //    {
    //        Steamworks.SteamServer.Init(480, serverInit);
    //    }
    //    catch (System.Exception)
    //    {
    //        Debug.Assert(false, "Couldn't init for some reason");
    //    }
    //}

    //public void P2PSendMsg(byte[] data,SteamId targetSteamID)
    //{
    //    var sent = SteamNetworking.SendP2PPacket(targetSteamID, data);
    //    Debug.Log("msgsent:" + sent);
    //}

    public void P2PAcceptMsg(SteamId steamId)
    {
        
        SteamNetworking.OnP2PSessionRequest = (steamId) =>
        {
            SteamNetworking.AcceptP2PSessionWithUser(steamId);
        };
    }

    //public void P2PReadMsg()
    //{
    //    while (SteamNetworking.IsP2PPacketAvailable())
    //    {
    //        var packet = SteamNetworking.ReadP2PPacket();
    //        if(packet.HasValue)
    //        {
    //            //èàóù
    //        }
    //    }
    //}


}
