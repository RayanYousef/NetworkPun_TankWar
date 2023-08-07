using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Pun_Tanks
{
    public class CS_PunTanksMainMenu : MonoBehaviourPunCallbacks
    {
        [SerializeField] GameObject lobbyPanel, homePanel,preGameplayPanel;
        [SerializeField] TMP_InputField playerName,roomName;
        public InputField[] inputFields;
        [SerializeField] Button StartReadyButton;
        [SerializeField] TMP_Dropdown teamDropDown, tankTypeDropDown;
        // Start is called before the first frame update
        void Start()
        {

            lobbyPanel.SetActive(false);
            preGameplayPanel.SetActive(false);
            homePanel.SetActive(true);
            ToggleEvents(true);
        }

        private void Update()
        {
            InputFieldsTab();
        }

        void InputFieldsTab()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (playerName.isFocused)
                {
                    roomName.ActivateInputField();
                }
                else if (roomName.isFocused)
                {
                    playerName.ActivateInputField();
                }
            }
        }
        public void ToggleEvents(bool subscribe)
        {
            if (subscribe)
            {
                CS_PunTanksNetworkManager.Instance.OnJoiningRoomAction += DisableLobbyShowPreGamePlay;
                CS_PunTanksNetworkManager.Instance.OnPlayerLeftRoomAction += ChangeButtonText;
                CS_PunTanksNetworkManager.Instance.OnConnectingAction += DisableHomeShowLobby;
                CS_PunTanksNetworkManager.Instance.ReadyAction += StartGameInteractablility;
            }
            else
            {
                CS_PunTanksNetworkManager.Instance.OnJoiningRoomAction -= DisableLobbyShowPreGamePlay;
                CS_PunTanksNetworkManager.Instance.OnPlayerLeftRoomAction -= ChangeButtonText;
                CS_PunTanksNetworkManager.Instance.OnConnectingAction -= DisableHomeShowLobby;
                CS_PunTanksNetworkManager.Instance.ReadyAction -= StartGameInteractablility;

            }
        }

        #region                          Disable/Enable/Interactable

        public void DisableHomeShowLobby()
        {
            homePanel.SetActive(false); 

            lobbyPanel.SetActive(true);
        }

        public void DisableLobbyShowPreGamePlay()
        {
            ChangeButtonText();

            lobbyPanel.SetActive(false);

            preGameplayPanel.SetActive(true);
        } 

        public void StartGameInteractablility(bool state)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log("Start Game is Interactable");
                StartReadyButton.interactable = state;
            }
        }


        public void ChangeButtonText()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                StartReadyButton.interactable = false;
                StartReadyButton.GetComponentInChildren<TMP_Text>().text = "Start Game";
            }
            else StartReadyButton.GetComponentInChildren<TMP_Text>().text = " Ready ";
        }
        #endregion



        #region Buttons
        // Home Buttons
        public void OnClickedConnect()
        {
            CS_PunTanksNetworkManager.Instance.ConnectToCloud();
        }

        // Pregame Buttons
        public void OnClickStartReady()
        {
            if (CS_PunTanksNetworkManager.Instance.IsHost)
                CS_PunTanksNetworkManager.Instance.LoadGameplayScene();
            else OnClickReady();
        }
        public void OnClickReady()
        {
                CS_PunTanksNetworkManager.Instance.ReadyClicked();
        }

        // Lobby Buttons
        public void OnClickedCreateRoom()
        {
            if (string.IsNullOrEmpty(roomName.text) || string.IsNullOrEmpty(playerName.text))
                return;
            CS_PunTanksNetworkManager.Instance.CreateRoom(roomName.text);
            UpdatePlayerData();
        }

        public void OnClickedJoinRoom()
        {
            if (string.IsNullOrEmpty(roomName.text) || string.IsNullOrEmpty(playerName.text))
                return;
            CS_PunTanksNetworkManager.Instance.JoinRoom(roomName.text);
            UpdatePlayerData();
        }

        public void OnClickedJoinRandomRoom()
        {
            if (string.IsNullOrEmpty(playerName.text))
                return;
            CS_PunTanksNetworkManager.Instance.JoinRandomRoom();
            UpdatePlayerData();

        }

        public void OnClickLeaveRoom()
        {
            if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
            {
                PhotonNetwork.LeaveRoom();
            }
        }
        #endregion


        private void UpdatePlayerData()
        {
            CS_PunTanksNetworkManager.Instance.UpdatePlayerData(playerName.text,
                (Team)teamDropDown.value,(TankType)tankTypeDropDown.value);
        }

        private void OnDestroy()
        {
            ToggleEvents(false);
        }
    }
}
