using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting;

namespace Pun_Tanks
{
    public class CS_GameplayManager : MonoBehaviour
    {

        [SerializeField] GameObject dpsPrefab,tankyPrefab,healerPrefab;
        [SerializeField] Transform[] spawningPositions;
        [SerializeField] CS_PunTanksNetworkManager networkManager;

        // Start is called before the first frame update
        void Start()
        {

            Transform pos = spawningPositions[(PhotonNetwork.LocalPlayer.ActorNumber - 1)%4];

            switch(CustomProps.GetTankType(PhotonNetwork.LocalPlayer))
            {
                case TankType.DPS:
                     PhotonNetwork.Instantiate(dpsPrefab.name, pos.position, pos.rotation);
                    break;
                case TankType.Tanky:
                     PhotonNetwork.Instantiate(tankyPrefab.name, pos.position, pos.rotation);
                    break;
                case TankType.Healer:
                    PhotonNetwork.Instantiate(healerPrefab.name, pos.position, pos.rotation);
                    break;
            }
        }



    }
}
