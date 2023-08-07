using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using static Pun_Tanks.CustomProps;



namespace Pun_Tanks
{
    public class CS_HealingArea : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] Rigidbody rb;

        [Header("Data")]
        [SerializeField] Player bulletOwner;
        [SerializeField] Vector3 destination;
        [SerializeField] Material healingMaterial;
        [SerializeField] float speed, lifeTime;
        float timer;

        [Header("List Of Players In Area")] 
        [SerializeField] List<Player> playersInRange = new List<Player>();


        public Player BulletOwner { get => bulletOwner; set => bulletOwner = value; }
        public Vector3 Destination { get => destination; set => destination = value; }

        private void Start()
        {
            rb = GetComponentInChildren<Rigidbody>();
            Destroy(gameObject, lifeTime);

            timer = Mathf.Infinity;
        }
        private void Update()
        {
            timer += Time.deltaTime;
            if(playersInRange.Count > 0 && timer > lifeTime/50 && PhotonNetwork.IsMasterClient)
            {
                foreach(Player player in playersInRange)
                {
                    SetHealth(player, Mathf.Clamp(GetHealth(player)+GetMaxHealth(player)/50,0,GetMaxHealth(player)));
                    timer = 0;
                }
            }

            if (Vector3.Distance(transform.position, destination) > 0.5f)
                transform.position = Vector3.MoveTowards(transform.position, destination, Time.deltaTime * speed);
            else
            {
                if (transform.localScale.x != 5)

                {
                    transform.localScale = Vector3.one * 7;
                    gameObject.GetComponentInChildren<MeshRenderer>().material = healingMaterial;
                }
                float radiusFromScale = gameObject.GetComponent<SphereCollider>().radius * transform.localScale.x;
                var sphere = Physics.OverlapSphere(transform.position, radiusFromScale);
                foreach (Collider col in sphere)
                {
                    if (col.CompareTag("Player") && col.transform.parent.TryGetComponent<PhotonView>(out PhotonView player))
                        Debug.Log(GetTeam(player.Owner));
                }
            }
        }


        private void OnTriggerEnter(Collider collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                Player otherPlayer = collision.GetComponentInParent<PhotonView>().Owner;
                if(GetTeam(otherPlayer) == GetTeam(bulletOwner))
                    playersInRange.Add(otherPlayer);
            }
        }

        private void OnTriggerExit(Collider collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                Player otherPlayer = collision.GetComponentInParent<PhotonView>().Owner;
                if (GetTeam(otherPlayer) == GetTeam(bulletOwner))
                    playersInRange.Remove(otherPlayer);
            }
        }

    }
}
