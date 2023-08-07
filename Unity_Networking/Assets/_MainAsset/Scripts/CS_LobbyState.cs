using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Pun_Tanks.CustomProps;



namespace Pun_Tanks
{
    public class CS_LobbyState : MonoBehaviourPun/*, IPunObservable*/
    {
        public Image ReadyImage;
        public TMP_Text PlayerName;

        //public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        //{
        //    //stream.Serialize(ref health);


        //}


        private void Start()
        {
            if (SceneManager.GetSceneByBuildIndex(0) != SceneManager.GetActiveScene())
            {
                Destroy(gameObject);
                return;
            }

            transform.parent = GameObject.Find("Players").transform;
            transform.localScale = Vector3.one;

            if (photonView.Owner.IsMasterClient==false)
            GetComponentInChildren<CS_LobbyState>().PlayerName.text = photonView.Owner.NickName;
            else GetComponentInChildren<CS_LobbyState>().PlayerName.text = "(Host) " + photonView.Owner.NickName;

            CS_PunTanksNetworkManager.Instance.ReadyAction += UpdateReadyUI;
            CS_PunTanksNetworkManager.Instance.OnJoiningRoomAction += ReArrangeSiblings;
            CS_PunTanksNetworkManager.Instance.OnPlayerLeftRoomAction += ReArrangeSiblings;


            UpdateReadyUI();
            ReArrangeSiblings();

        }

        private void OnDisable()
        {
            CS_PunTanksNetworkManager.Instance.ReadyAction -= UpdateReadyUI;
            CS_PunTanksNetworkManager.Instance.OnJoiningRoomAction -= ReArrangeSiblings;
            CS_PunTanksNetworkManager.Instance.OnPlayerLeftRoomAction -= ReArrangeSiblings;

        }

        private void OnDestroy()
        {
            CS_PunTanksNetworkManager.Instance.ReadyAction -= UpdateReadyUI;
            CS_PunTanksNetworkManager.Instance.OnJoiningRoomAction -= ReArrangeSiblings;
            CS_PunTanksNetworkManager.Instance.OnPlayerLeftRoomAction -= ReArrangeSiblings;

        }



        public void UpdateReadyUI(bool actionState = false)
        {

            if (photonView.Owner.IsMasterClient)
            {
                ReadyImage.color = Color.cyan;
                return;
            }

            if (photonView.Owner.CustomProperties.TryGetValue(CustomProps.KeyReady,out var obj))
            {
                if ((bool)obj == false)
                    ReadyImage.color = Color.red;
                else ReadyImage.color = Color.green;
            }
        }

        public void ReArrangeSiblings()
        {

            if (photonView.Owner.IsMasterClient == false)
                GetComponentInChildren<CS_LobbyState>().PlayerName.text = photonView.Owner.NickName;
            else GetComponentInChildren<CS_LobbyState>().PlayerName.text = "(Host) " + photonView.Owner.NickName;

            GameObject parentObject = GameObject.Find("Players");
            RectTransform[] childObjects = parentObject.GetComponentsInChildren<RectTransform>();

            for(int i = 1; i<childObjects.Length-1; i++)            
            {
                if (childObjects[i].TryGetComponent<PhotonView>(out PhotonView outObject))
                {
                    childObjects[i].SetSiblingIndex(outObject.Owner.ActorNumber);
                    if(outObject.Owner.IsMasterClient)
                        childObjects[i].SetAsFirstSibling();
                    else
                        childObjects[i].SetSiblingIndex(outObject.Owner.ActorNumber);

                }
                
            }
        }

    }





}