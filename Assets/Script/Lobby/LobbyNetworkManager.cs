using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using TMPro;


public class LobbyNetworkManager : MonoBehaviourPunCallbacks
{
    public static LobbyNetworkManager Instance;
    [SerializeField] private TMP_Text waitBattleText;
   private void Awake() {
        Instance = this;
    }

     private void Start()
     {
        PhotonNetwork.ConnectUsingSettings();
        WindowsManager.Layout.OpenLayout("Loading");
     }
     public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        WindowsManager.Layout.OpenLayout("MainMenu");
       }
    public void ToBattleButon()
    {
        WindowsManager.Layout.OpenLayout("AutomaticBatle");
        PhotonNetwork.JoinRandomRoom();
     }
    //      public override void OnJoinedRoom()
    // {
    //     PhotonNetwork.LoadLevel("Battle");
    // }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        if (returnCode == (short)ErrorCode.NoRandomMatchFound)
         {      
            waitBattleText.text = "No rooms available, creating a new one...";
            CreateNewRoom();
        }
    }
    private string RoomNameGenerator()
    {
        short codeLength = 12;
        string roomCode = null;
        for (short i=0; i < codeLength; i++)
        {
            char symbol = (char)Random.Range(65, 91);
            roomCode += symbol;
        }
        return roomCode;
    }
    private void CreateNewRoom()
    {
      
        RoomOptions currentRoom = new RoomOptions();
       currentRoom.IsOpen= true;
       currentRoom.MaxPlayers=2;
       PhotonNetwork.CreateRoom(RoomNameGenerator(), currentRoom);
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        if (returnCode == (short)ErrorCode.GameIdAlreadyExists)
        {
            CreateNewRoom();
        }
    }
    public override void OnCreatedRoom()
    {
        waitBattleText.text = "Waiting for another player to join...";
    }
    public override void OnJoinedRoom()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        if(PhotonNetwork.IsMasterClient)
        {
            return;
        }
        waitBattleText.text = "Another player has joined, starting the battle...";
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (!PhotonNetwork.IsMasterClient)return;
        Room currentRoom = PhotonNetwork.CurrentRoom;
        currentRoom.IsOpen = false;
        waitBattleText.text = "Get Ready";
        Invoke("LoadingGameMap", 3f);
    }
    private void LoadingGameMap()
    {
        PhotonNetwork.LoadLevel(1);
    }
    public void StopFindingBattle()
    {
        PhotonNetwork.LeaveRoom();
    }
    public override void OnLeftRoom()
    {
        WindowsManager.Layout.OpenLayout("MainMenu");
    }
}
