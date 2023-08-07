using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace Pun_Tanks
{
    public  enum Team { Red, Blue }
    public  enum TankType { DPS, Tanky, Healer }

    public static  class CustomProps
    {

        //keys
        static string
            keyID="ID",
            keyName = "Name",
            keyteam = "PlayerTeam",
            keyTankType = "Tank Type",
            keyMaxHealth = "Max Health",
            keyHealth = "Health",
            keyMovementSpeed = "Movement Speed",
            keyCannonRotationspeed = "Cannon Rotation Speed",
            keyDamage = "Damage",
            keyReady = "Ready",
            keyCurrentSceneIndex = "CurrentScene";


        public static  string KeyID { get => keyID;}
        public static  string KeyName { get => keyName;}
        public static  string Keyteam { get => keyteam;}
        public static  string KeyTankType { get => keyTankType;}
        public static  string KeyMaxHealth { get => keyMaxHealth;}
        public static  string KeyHealth { get => keyHealth;}
        public static  string KeyMovementSpeed { get => keyMovementSpeed;}
        public static  string KeyCannonRotationspeed { get => keyCannonRotationspeed;}
        public static  string KeyDamage { get => keyDamage;}
        public static string KeyReady { get => keyReady; }
        public static string KeyCurrentSceneIndex { get => keyCurrentSceneIndex;}


        #region Player Initialization
        public static void InitializePlayerCustomProperties( int id = 0, string name = null, Team team = 0, TankType tankType = 0 )
        {

            Hashtable customProperties = new Hashtable();

            customProperties.Add(KeyID, id);
            customProperties.Add(keyName, name);
            customProperties.Add(keyteam, team);
            customProperties.Add(keyTankType, tankType);

            float maxHealth = 100f, health = 100f, movementSpeed = 100f, cannonRotationSpeed = 50f, damage = 10f;

            switch (tankType)
            {
                case TankType.DPS:
                    maxHealth = 150f;
                    movementSpeed = 400f;
                    damage = 15f;

                    break;
                case TankType.Tanky:
                    maxHealth = 600f;
                    movementSpeed = 100f;
                    damage = 5f;

                    break;
                case TankType.Healer:
                    maxHealth = 150f;
                    movementSpeed = 400f;
                    damage = 5f;

                    break;
            }

            health = maxHealth;

            customProperties.Add(keyMaxHealth, maxHealth);
            customProperties.Add(keyHealth, health);
            customProperties.Add(keyMovementSpeed, movementSpeed);
            customProperties.Add(keyCannonRotationspeed, cannonRotationSpeed);
            customProperties.Add(keyDamage, damage);

            SetCustomProperties(PhotonNetwork.LocalPlayer, customProperties);

        } 
        #endregion

        #region Get and Set Player Data

        // Player ID
        public static int GetID(Player player)
        {
            object obj;
            if (player.CustomProperties.TryGetValue(KeyID, out obj))
            {
                return (int)obj;
            }
            return 0;
        }

        public static void SetID(Player player,int neWID)
        {
            Hashtable newProperty = new Hashtable();

            newProperty[KeyID] = neWID;

            SetCustomProperties(player, newProperty);
        }


        // Player Scene
        public static int GetSceneIndex(Player player)
        {
            object obj;
            if (player.CustomProperties.TryGetValue(KeyCurrentSceneIndex, out obj))
            {
                return (int)obj;
            }
            return 0;
        }

        public static void SetSceneIndex(Player player, int neWID)
        {
            Hashtable newProperty = new Hashtable();

            newProperty[KeyCurrentSceneIndex] = neWID;

            SetCustomProperties(player, newProperty);
        }

        // Name 
        public static string GetName(Player player)
        {
            object obj;
            if (player.CustomProperties.TryGetValue(keyName, out obj))
            {
                return (string)obj;
            }
            return null;
        }

        public static void SetName(Player player, string newName)
        {
            Hashtable newProperty = new Hashtable();

            newProperty[keyName] = newName;

            SetCustomProperties(player, newProperty);
        }


        // Team
        public static Team GetTeam(Player player)
        {
            object obj;
            if (player.CustomProperties.TryGetValue(keyteam, out obj))
            {
                return (Team)obj;
            }
            return 0;
        }

        public static void SetTeam(Player player, Team team)
        {
            Hashtable newProperty = new Hashtable();

            newProperty[keyteam] = team;

            SetCustomProperties(player, newProperty);

        }


        // Tank Type
        public static TankType GetTankType(Player player)
        {
            object obj;
            if (player.CustomProperties.TryGetValue(keyTankType, out obj))
            {
                return (TankType)obj;
            }
            return 0;

        }
        public static void SetTankType(Player player, TankType tanktype)
        {
            Hashtable newProperty = new Hashtable();

            newProperty[keyTankType] = tanktype;

            SetCustomProperties(player, newProperty);

        }

        // Max Health
        public static float GetMaxHealth(Player player)
        {
            object obj;
            if (player.CustomProperties.TryGetValue(keyMaxHealth, out obj))
            {
                return (float)obj;
            }
            return 0;
        }

        public static void SetMaxHealth(Player player, float maxHealth)
        {
            Hashtable newProperty = new Hashtable();

            newProperty[keyMaxHealth] = maxHealth;

            SetCustomProperties(player, newProperty);

        }

        // Current Health
        public static float GetHealth(Player player)
        {
            object obj;
            if (player.CustomProperties.TryGetValue(keyHealth, out obj))
            {
                return (float)obj;
            }
            return 0;
        }

        public static void SetHealth(Player player, float newHealthValue)
        {
            Hashtable newProperty = new Hashtable();

            newProperty[keyHealth] = newHealthValue;

            SetCustomProperties(player, newProperty);

        }


        // Movement Speed
        public static float GetMovementSpeed(Player player)
        {
            object obj;
            if (player.CustomProperties.TryGetValue(keyMovementSpeed, out obj))
            {
                return (float)obj;
            }
            return 0;
        }

        public static void SetMovementSpeed(Player player, float movementSpeed)
        {
            Hashtable newProperty = new Hashtable();

            newProperty[keyMovementSpeed] = movementSpeed;

            SetCustomProperties(player, newProperty);

        }

        // Cannon Rotation Speed
        public static float GetCannonRotationSpeed(Player player)
        {
            object obj;
            if (player.CustomProperties.TryGetValue(keyCannonRotationspeed, out obj))
            {
                return (float)obj;
            }
            return 0;
        }

        public static void SetCannonRotationSpeed(Player player, float cannonRotationSpeed)
        {
            Hashtable newProperty = new Hashtable();

            newProperty[keyCannonRotationspeed] = cannonRotationSpeed;

            SetCustomProperties(player, newProperty);

        }

        // Damage
        public static float GetDamage(Player player)
        {
            object obj;
            if (player.CustomProperties.TryGetValue(keyDamage, out obj))
            {
                return (float)obj;
            }
            return 0;
        }

        public static void SetDamage(Player player,float damage)
        {
            Hashtable newProperty = new Hashtable();

            newProperty[keyDamage] = damage;

            SetCustomProperties(player, newProperty);
        }


        // Set Data
        public static void SetCustomProperties(Player player,Hashtable newProp)
        {
            player.SetCustomProperties(newProp);
        }


        #endregion

    }
}
