using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using TMPro;
using Steamworks;
using StarterAssets;



public class LobbyUI : MonoBehaviour
{
    public VerticalLayoutGroup lobbyName;
    public VerticalLayoutGroup lobbyPlayerCount;
    LobbyManager lobbyManager;
    public GameObject lobbyListPrefab;
    public int currentPage = 1;


    [SerializeField] private TMP_InputField chatInput;
    [SerializeField] private Transform chatBoxContent;
    [SerializeField] private GameObject chatLine;
    private bool isTyping = false;


    // Start is called before the first frame update
    void Start()
    {
        lobbyManager = LobbyManager.Instance();
        for (int index = 0; index < lobbyName.transform.childCount; index++)
        {
            Transform g = lobbyName.transform.GetChild(index);
            g.GetComponent<Button>().onClick.AddListener(() => lobbyManager.SelectLobby(g.GetSiblingIndex() * currentPage));
    
        }
        SteamMatchmaking.OnChatMessage += (lobby, member, message) =>
        {
            var newLine = Instantiate(chatLine);
            newLine.GetComponent<TextMeshProUGUI>().text = member.Name + ":" + message;
            newLine.transform.SetParent(chatBoxContent,false);
            Debug.Log($"{member}: {message}");
        };

    }

    // Update is called once per frame
    void Update()
    {
        Chat();
    }

    private void Chat()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject != chatInput.gameObject)
            {
                chatInput.Select();
                chatInput.ActivateInputField();
                isTyping = true;
            }
            else
            {
                SendMessageUI();
            }
        }
       
    }

    async public void RefreshLobbyList()
    {
        var handle = lobbyManager.RefreshLobbyList();
        var button = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        button.interactable = false;
        while (handle.IsCompleted != true)
        {
            await Task.Yield();
        }
        button.interactable = true;

        RectTransform parent = lobbyName.GetComponent<RectTransform>();
        for (int index = 0; index < lobbyName.transform.childCount; index++)
        {
            Transform g = lobbyName.transform.GetChild(index);
            Transform t = lobbyPlayerCount.transform.GetChild(index);
            t.gameObject.SetActive(false);
            g.gameObject.SetActive(false);
            if (index < lobbyManager.lobbyList.Count)
            {
                g.gameObject.SetActive(true);
                t.gameObject.SetActive(true);
                foreach (KeyValuePair<string, string> kvp in lobbyManager.lobbyList[index].Data)
                {
                    g.GetComponentInChildren<Text>().text = kvp.Value;
                }
                   
                t.GetComponentInChildren<TextMeshProUGUI>().text = lobbyManager.lobbyList[index].MemberCount + "/"+ lobbyManager.lobbyList[index].MaxMembers;
            }
        }
    }


    async public void CreateLobbyUI()
    {
        var button = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        button.interactable = false;
        var createLobby = await lobbyManager.CreateLobby();
        while (createLobby != true)
        {
            await Task.Yield();
        }
        button.interactable = true;

    }

    async public void JoinLobbyUI()
    {
        var button = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        button.interactable = false;
        var joinLobby = await lobbyManager.JoinLobby();
        while (joinLobby != true)
        {
            await Task.Yield();
        }
        button.interactable = true;
    }

    public void SendMessageUI()
    {
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
        chatInput.DeactivateInputField();
        lobbyManager.LobbySendMessage(chatInput.text);
        chatInput.text = "";
        isTyping = false;
    }


}
