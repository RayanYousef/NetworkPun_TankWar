using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using static Pun_Tanks.CustomProps;

namespace Pun_Tanks
{

    enum BulletType
    {
        attack,superAttack
    }
    public class CS_Bullet : MonoBehaviour
    {

        [Header ("Components")]
        [SerializeField] Rigidbody rb;

        [Header("Data")]
        [SerializeField] Player bulletOwner;
        [SerializeField] BulletType thisBulletType;
        [SerializeField] Vector3 destination;
        [SerializeField] float speed;

        [Header("Sound Effects")]
        [SerializeField] AudioClip instantiationClip;


        public Player BulletOwner { get => bulletOwner; set => bulletOwner = value; }
        internal BulletType BulletType { get => thisBulletType; set => thisBulletType = value; }
        public Vector3 Destination { get => destination; set => destination = value; }

        private void Start()
        {
            rb = GetComponentInChildren<Rigidbody>();
            GetComponent<AudioSource>().PlayOneShot(instantiationClip);
            rb.velocity = transform.forward * speed;
            Destroy(gameObject, 1);
        }

        #region Trigger and Collision
        private void OnTriggerEnter(Collider other)
        {
            if (thisBulletType == BulletType.attack)
                if (other.CompareTag("Player"))
                {
                    Player otherPlayer = other.transform.parent.GetComponent<PhotonView>().Owner;
                    if (GetTeam(otherPlayer) != GetTeam(bulletOwner))
                    {
                        other.GetComponentInParent<AudioSource>().Play();

                        if (PhotonNetwork.IsMasterClient)
                            other.GetComponentInParent<CS_PunNetworkPlayer>().UpdateHealth(-GetDamage(bulletOwner));
                        Destroy(gameObject);
                    }
                    else if (GetTeam(otherPlayer) == GetTeam(bulletOwner) && GetTankType(bulletOwner) == TankType.Healer)
                    {
                        if (PhotonNetwork.IsMasterClient)
                            other.GetComponentInParent<CS_PunNetworkPlayer>().UpdateHealth(GetDamage(bulletOwner));

                    }

                }
        }
        #endregion

    }
}

