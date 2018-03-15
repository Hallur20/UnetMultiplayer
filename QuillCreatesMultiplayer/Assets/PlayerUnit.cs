using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerUnit : NetworkBehaviour {

    // Use this for initialization
    void Start() {
            playerName = "hp: " + totalHealth; //start with max health on the syncvar variable (100)
    }

    void OnCollisionEnter(Collision col) //monobehaviour method that makes the playerobject have a collision behaviour (if object touches other object)
    {

            if(col.gameObject.name == "Bullet Clone(Clone)") //if player touches bullet clone
            {
                Destroy(col.gameObject); //destroy bullet clone
                Debug.Log("you've been hit.");
                totalHealth = totalHealth - 10; //take 10 damage
                playerName = "health: " + totalHealth; //update name with healthpoints
                if (totalHealth <= 0) // if health is below or zero
                {
                CmdDestroyMyUnit(); //destroy player object
               
                }
            }
    }
    // Update is called once per frame  
    void Update()
    {
        //do code here for ALL version of this object
        if (hasAuthority == false) //if client has not been given authority from server to use this object (this is so other clients cant control your object)
        {
            return; //exit update
        } // else do code here for your local object;
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            this.transform.Translate(0, 1, 0); //go up
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            this.transform.Translate(-1, 0, 0); //go left
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            this.transform.Translate(1, 0, 0); //go right
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            this.transform.Translate(0,-1,0); //go down
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (readyToShoot == true) { //if readyToShoot is true
                Debug.Log(readyToShoot);
                CmdFire(); //use this client-to-server method
            }
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            CmdStartName("Hallur");
        }
    }
        /* if (/*some input*//*true) {
             velocity = new Vector3(1,0,0);
             CmdUpdateVelocity(velocity, transform.position);
         }
         */
        public int totalHealth = 100;

    [SyncVar(hook = "OnPlayerNameChanged")]
    public string playerName;
    public GameObject bulletPrefab;
    public Transform bulletSpawn;

    bool readyToShoot = true;

    public GameObject clone;
    
    GameObject cloneOnClients;

    [Command]
    void CmdFire()
    {
                GameObject go = Instantiate(clone); //object exists on the server
                Vector3 pos = bulletSpawn.transform.GetComponent<Transform>().position;
                pos.x = pos.x + 1; //start on +1 x away from player so player will not get hit by own bullet
                go.transform.position = pos;
                NetworkServer.Spawn(go); //object is spawned on the server
                RpcsendServerBulletToAllServers(go); //send object to all clients so they can do what they want with it
    }
    [ClientRpc]
    void RpcsendServerBulletToAllServers(GameObject go)
    {
        cloneOnClients = go;
        InvokeRepeating("LaunchProjectile", 0.0f, 0.1f); //start after 0 seconds, repeat every 0.1 second.

    }
    void LaunchProjectile()
    {
        readyToShoot = false;
        if (cloneOnClients != null) //if the server object exists
        {
            cloneOnClients.transform.Translate(1, 0, 0); //move bullet clone +1 x 
            Destroy(cloneOnClients, 1.5F); // destroy bullet clone after 1.5 seconds
        } else
        { 
            readyToShoot = true;
            CancelInvoke(); //else cancel the invoke and set readyToShoot to true.
        }
       
    }

        void OnPlayerNameChanged(string newName) //hook method for playerName
    {
        Debug.Log("changed name to: "+newName);
        playerName = newName;
        gameObject.GetComponentInChildren<TextMesh>().text = newName; //
       playerName = newName;    //ops remember hook on syncvar doesnt auto update local value
        //tip: while you're in hook, syncvar behaviour is disabled.
    }

    [Command]
    void CmdDestroyMyUnit()
    {
        Destroy(gameObject);
    }

    [Command]
    void CmdStartName(string name)
    {
        playerName = name;

    }
}
