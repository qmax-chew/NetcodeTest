using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Steamworks;
using Steamworks.Data;


public enum LobbyPlayerLimit
{
    TwoPlayer = 2,
    FourPlayer = 4,
    EightPlayer = 8
}

public class LobbyManager : Singleton<LobbyManager>
{
    private bool lobbyDisconneted;
    public ulong lobbyId;
    public Lobby currentLobby;
    private Lobby selectedLobbyData;
    public LobbyPlayerLimit limit = LobbyPlayerLimit.FourPlayer;
    public SteamId OpponentSteamId { get; set; }
    public List<Lobby> lobbyList = new List<Lobby>();

    [SerializeField]
    private Netcode.Transports.Facepunch.FacepunchTransport facepunchTransport;

    // Start is called before the first frame update
    void Start()
    {
        SteamMatchmaking.OnLobbyCreated += OnLobbyCreatedCallback;
        SteamMatchmaking.OnLobbyMemberJoined += OnLobbyMemberJoinedCallback;
        SteamMatchmaking.OnLobbyGameCreated += OnLobbyGameCreatedCallback;



        //SteamMatchmaking.OnChatMessage += (lobby, member, message) =>
        //{
        //    Debug.Log($"{member}: {message}");
        //};
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public async Task<bool> CreateLobby()
    {
        try
        {
            var createLobby =await SteamMatchmaking.CreateLobbyAsync((int)limit);
            if (!createLobby.HasValue)
            {
                Debug.Log("Lobby created but not correctly instantiated");
                throw new System.Exception();
            }
            selectedLobbyData = createLobby.Value;
            selectedLobbyData.SetPublic();
            selectedLobbyData.SetJoinable(true);
            string lobbyName = SteamClient.Name + "'s lobby";
            selectedLobbyData.SetData("name", lobbyName);
            currentLobby = selectedLobbyData;
            Debug.Log(lobbyName + "created");

        }
        catch (System.Exception exception)
        {
            Debug.Log("Failed to create multiplayer lobby");
            Debug.Log(exception.ToString());
            return false;
        }
        return true;
    }

    public async Task<bool> JoinLobby()
    {
        try
        {
            var joinLobby = await SteamMatchmaking.JoinLobbyAsync(selectedLobbyData.Id);
            Debug.Log("joined" + selectedLobbyData.Id);

        }
        catch (System.Exception exception)
        {
            Debug.Log("Failed to join multiplayer lobby");
            Debug.Log(exception.ToString());
            return false;
        }
        return true;
    }

    void OnLobbyCreatedCallback(Result result, Lobby lobby)
    {
        // Lobby was created
        lobbyDisconneted = false;
        if (result != Result.OK)
        {
            Debug.Log("lobby creation result not ok");
            Debug.Log(result.ToString());
        }
    }

    public async Task<bool> RefreshLobbyList()
    {
        try
        {
            lobbyList.Clear();
            Lobby[] lobbies = await SteamMatchmaking.LobbyList.FilterDistanceClose().RequestAsync();
            if (lobbies != null)
            {
                foreach (Lobby lobby in lobbies)
                {
                    lobbyList.Add(lobby);
                    Debug.Log($"[{lobby.Id}] owned by {lobby.Owner} ({lobby.MemberCount}/{lobby.MaxMembers})");
                }
            }
            else
            {
                Debug.Log("No Lobby Found");
            }
            return true;
        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString());
            Debug.Log("Error fetching multiplayer lobbies");
            return true;
        }
    }

    public void SelectLobby(int no)
    {
        Debug.Log(no);
        selectedLobbyData = lobbyList[no];
    }

    //public async Task LobbyChatLog()
    //{
    //    SteamMatchmaking.OnChatMessage += (lobby, member, message) =>
    //    {
    //        Debug.Log($"{member}: {message}");
    //    };
    //}

    public async Task<bool> LobbySendMessage(string msg)
    {
        try
        {
            var send = currentLobby.SendChatString(msg);
            return true;
        }
        catch (System.Exception e)
        {
            Debug.Log(e.ToString());
            Debug.Log("Error sending message");
            return true;
        }
    }

    void OnLobbyMemberJoinedCallback(Lobby lobby, Friend friend)
    {
        Debug.Log("someone else joined lobby");
        OpponentSteamId = friend.Id;
        AcceptP2P(OpponentSteamId);
    }

    private void AcceptP2P(SteamId opponentId)
    {
        try
        {
            // For two players to send P2P packets to each other, they each must call this on the other player
            SteamNetworking.AcceptP2PSessionWithUser(opponentId);
        }
        catch
        {
            Debug.Log("Unable to accept P2P Session with user");
        }
    }

    public void StartGame()
    {
        currentLobby.SetGameServer(SteamClient.SteamId);
        Unity.Netcode.NetworkManager.Singleton.StartHost();
        
    }

    void OnLobbyGameCreatedCallback(Lobby lobby, uint ip, ushort port, SteamId steamId)
    {
        AcceptP2P(steamId);
        facepunchTransport.targetSteamId = steamId;
        GameObject.FindGameObjectWithTag("UICamera").SetActive(false);
        Unity.Netcode.NetworkManager.Singleton.StartClient();
    }
}
