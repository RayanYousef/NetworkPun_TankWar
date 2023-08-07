using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using static  Pun_Tanks.CustomProps;


namespace Pun_Tanks
{
    /// <summary>
    ///  Photonproperties.Custom
    /// </summary>
    public class CS_PunNetworkPlayer : MonoBehaviourPun/*, IPunObservable*/
    {

        [Header("Player Data")]
        [SerializeField] LayerMask mask;
        float specialAttackCD;
        float timer;

        [Header("Player Components")]
        [SerializeField] Rigidbody rb;
        [SerializeField] CS_PunTanksNetworkManager networkManager;

        [Header("UI")]
        [SerializeField] Image hpImage;
        [SerializeField] TMP_Text textPlayerName;

        [Header("Cannon Info")]
        [SerializeField] Transform cannon;
        [SerializeField] CS_Bullet damageBullet;
        [SerializeField] CS_HealingArea heallingBullet;







        private void Awake()
        {
            networkManager = CS_PunTanksNetworkManager.Instance;
  
        }


        void Start()
        {
            rb = GetComponent<Rigidbody>();  
            textPlayerName.text = photonView.Owner.NickName;
            
            photonView.RPC(nameof(RPC_ChangeColour), RpcTarget.All);

            UpdateHealthUI();

            networkManager.OnHealthUpdated += UpdateHealthUI;

            timer = 50;
            specialAttackCD = 20;

        }

        private void OnDestroy()
        {
            networkManager.OnHealthUpdated -= UpdateHealthUI;
        }


        private void Update()
        {

            TankBehavior();


            //bool rotateRight = Input.GetKey(KeyCode.RightArrow);
            //bool rotateLeft = Input.GetKey(KeyCode.LeftArrow);
            //bool shooting = Input.GetKeyDown(KeyCode.Space);
            //if (photonView.IsMine && rotateRight)
            //{
            //    cannon.Rotate(Vector3.up, PunPlayerData.GetCannonRotationSpeed(photonView.Owner) * Time.deltaTime);

            //}
            //else if (photonView.IsMine && rotateLeft)
            //{
            //    cannon.Rotate(Vector3.up, -PunPlayerData.GetCannonRotationSpeed(photonView.Owner) * Time.deltaTime);

            //}
        }

        public void UpdateHealth(float damage)
        {
            if(GetHealth(photonView.Owner)>0)
            SetHealth(photonView.Owner,Mathf.Clamp(GetHealth(photonView.Owner) + damage,0, GetMaxHealth(photonView.Owner)));
        }

        public void UpdateHealthUI()
        {
            if (hpImage != null)
                hpImage.transform.localScale = new Vector3(GetHealth(photonView.Owner) /GetMaxHealth(photonView.Owner), 1, 1);
        }


        public void TankBehavior()
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");


            if (rb && photonView.IsMine)
                rb.AddForce(new Vector3(h, 0, v).normalized * GetMovementSpeed(photonView.Owner) * Time.deltaTime, ForceMode.VelocityChange);
            rb.velocity = new Vector3(Mathf.Clamp(rb.velocity.x, -0.1f, 0.1f), rb.velocity.y, Mathf.Clamp(rb.velocity.x, -0.1f, 0.1f));


            if (photonView.IsMine)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask) && hit.transform.name == "Target" && photonView.IsMine)
                {
                    if ((hit.point - cannon.position).magnitude > 1.5f)
                        cannon.rotation = Quaternion.LookRotation(hit.point - cannon.position);
                }


                if (Input.GetMouseButtonDown(0))
                {
                    photonView.RPC(nameof(RPC_Shoot), RpcTarget.All);
                }


                timer+=Time.deltaTime;
                if (timer > specialAttackCD && Input.GetMouseButtonDown(1) && GetTankType(photonView.Owner) == TankType.Healer)
                {
                    photonView.RPC(nameof(RPC_SpecialSkill), RpcTarget.All, hit.point);
                    timer = 0;
                }

            }

        }

        #region RPCs

        [PunRPC]
        public void RPC_Shoot()
        {
            CS_Bullet bullet = Instantiate(damageBullet, cannon.GetChild(0).position, cannon.rotation);
            bullet.BulletOwner = photonView.Owner;
            bullet.BulletType = BulletType.attack;
        }

        [PunRPC]
        public void RPC_SpecialSkill(Vector3 destination)
        {
            switch(GetTankType(photonView.Owner))
            {
                case TankType.Healer:
                    CS_HealingArea bullet = Instantiate(heallingBullet, cannon.GetChild(0).position, cannon.rotation);
                    bullet.BulletOwner = photonView.Owner;
                    bullet.Destination = destination;
                    break;
            }
  
        }


        [PunRPC]
        private void RPC_ChangeColour()
        {
            if (GetTeam(photonView.Owner) == Team.Blue)
                gameObject.GetComponentInChildren<Renderer>().material.color = Color.blue;
            else if (GetTeam(photonView.Owner) == Team.Red) gameObject.GetComponentInChildren<Renderer>().material.color = Color.red;
        }

        #endregion

        //public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        //{
        //    //stream.Serialize(ref health);
        //    UpdateHealthUI();
        //}

    }

    

}
