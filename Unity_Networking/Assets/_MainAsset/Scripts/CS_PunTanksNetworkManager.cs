using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using System;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using static Pun_Tanks.CustomProps;

namespace Pun_Tanks
{
    public class CS_PunTanksNetworkManager : MonoBehaviourPunCallbacks
    {
        [Header("Instance Data")]
        static CS_PunTanksNetworkManager instance;
        public static CS_PunTanksNetworkManager Instance => instance;
        public bool IsHost => PhotonNetwork.IsMasterClient;

        //public bool gameStarted = false;

        [Header("Log Data")]
        [SerializeField] TMP_Text logText;

        // events
        public Action OnConnectingAction, OnJoiningRoomAction, OnPlayerLeftRoomAction, OnHealthUpdated;
        public Action<bool> ReadyAction;


        public void Awake()
        {
            ClearLog();
            if (!instance)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);    
            }
        }

        #region Property Update Functions

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
        {
            base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);

            //Debug.Log("Connected and In Room:" + PhotonNetwork.IsConnected +" : In Room:"+ PhotonNetwork.InRoom);
            //Debug.Log(PhotonNetwork.LocalPlayer.NickName);

            if (changedProps.ContainsKey(KeyReady))
                CheckReadyState();

            if (changedProps.ContainsKey(KeyHealth) && changedProps.TryGetValue(KeyHealth,out var oHealth))
            {
                OnHealthUpdated?.Invoke();
                if ((float)oHealth == 0)
                {
                    float health = (float)oHealth;
                    if (health == 0 && targetPlayer.IsLocal/* && PhotonNetwork.IsMasterClient==false*/)
                    {
                        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
                        {
                            PhotonNetwork.LeaveRoom();
                        }
                    }
                }
                #region Test
                //else if(health == 0 && targetPlayer.IsLocal && PhotonNetwork.IsMasterClient == true)
                //{
                //    foreach(Player player in PhotonNetwork.PlayerListOthers)
                //    {
                //        PhotonNetwork.SetMasterClient(player);
                //    }

                //    if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
                //    {
                //        PhotonNetwork.LeaveRoom();
                //    }


                //    gameStarted = false;
                //} 
                #endregion

            }



        }



        private void CheckReadyState()
        {
            bool allReady = true;
            foreach (var player in PhotonNetwork.PlayerList)
            {
                if (player.CustomProperties.TryGetValue(KeyReady, out object ready) && !player.IsMasterClient)
                    if ((bool)ready == false) allReady = false;
            }

            if (PhotonNetwork.PlayerList.Length < 2) allReady = false;

            ReadyAction?.Invoke(allReady);
        }

        public void UpdatePlayerData(string name, Team team, TankType tankType)
        {
            PhotonNetwork.NickName = name;
            InitializePlayerCustomProperties(PhotonNetwork.LocalPlayer.ActorNumber, name, team, tankType);
        }

        #endregion


        #region On Clicked Functions
        // Home
        public void ConnectToCloud()
        {
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.GameVersion = "v0.1";
            PhotonNetwork.ConnectUsingSettings();
            Log("Connecting...");
        }

        // Ready Screen
        public void ReadyClicked()
        {
            Hashtable isReady = new Hashtable();
            isReady.Add("Ready", true);
            PhotonNetwork.LocalPlayer.SetCustomProperties(isReady);
        }

        // Gameplay
        public void LoadGameplayScene()
        {
            PhotonNetwork.LoadLevel(1);
            SetSceneIndex(PhotonNetwork.LocalPlayer, 1);


        }

        #endregion

        #region Room Functions
        public void CreateRoom(string roomName)
        {
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = 4;
            roomOptions.BroadcastPropsChangeToAll = true;
            
            PhotonNetwork.CreateRoom(roomName, roomOptions);
            Log("Creating room " + roomName);

            SetSceneIndex(PhotonNetwork.LocalPlayer, 0);


        }

        public void JoinRoom(string roomName)
        {
            if(roomName!=null)
            PhotonNetwork.JoinRoom(roomName);
            Log("Joining room" + roomName);



        }

        public void JoinRandomRoom()
        {
            PhotonNetwork.JoinRandomRoom();
            Log("Joining random room");

        } 

        #endregion

        #region override functions
        public override void OnConnectedToMaster()
        {
            base.OnConnectedToMaster();
            Log("Connected to photon master.");
            OnConnectingAction?.Invoke();
        }

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();

            OnJoiningRoomAction?.Invoke();
            Log("Joined: " + PhotonNetwork.CurrentRoom.Name +" room");

            foreach (Player p in PhotonNetwork.PlayerListOthers)
            {
                Log( p.NickName + " is in room");
            }



            // Setting Ready = false
            Hashtable isReady = new Hashtable();
            isReady.Add(KeyReady, false);
            PhotonNetwork.LocalPlayer.SetCustomProperties(isReady);


            // Player Joined Roomz
                GameObject readyUI = PhotonNetwork.Instantiate("ReadyUI", Vector3.zero, Quaternion.identity);

        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            base.OnPlayerEnteredRoom(newPlayer);
            Log(newPlayer.NickName+" has entered the room!");

        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Debug.Log(otherPlayer.NickName + " Has left the room. (sad)");
            CheckReadyState();
            OnPlayerLeftRoomAction?.Invoke();

        }

        public override void OnLeftRoom()
        {
            base.OnLeftRoom();
            PhotonNetwork.LoadLevel(0);

        }
        #endregion

        #region Log Region
        public void Log(string message)
        {
            logText.text = $" - {message}\n{logText.text}";
        }

        public void ClearLog()
        {
            logText.text = string.Empty;
        } 
        #endregion
    }
}
