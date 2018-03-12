using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerUnit : NetworkBehaviour {

    // Use this for initialization
    void Start() {

    }

    /* Vector3 velocity; //direction/speed
     Vector3 bestGuessPosition; // the position we think is most correct for this player,
     //if we are authority this will be same as transform.position
     float ourLatency; //updates our latency to the server, i.e. how many seconds it takes for
                       //us to send a message

     float latencySmoothingFactor = 10; //higher value = faster local position will match best guess position
     */
    // Update is called once per frame  
    void Update()
    {
        //do code here for ALL version of this object
        //transform.Translate(velocity * Time.deltaTime);
        if (hasAuthority == false)
        {
            /*bestGuessPosition = bestGuessPosition + (velocity * Time.deltaTime);
            transform.position = Vector3.Lerp(transform.position, bestGuessPosition, Time.deltaTime);*/
            return;
        } //do code here for your local object;
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
            // Create the Bullet from the Bullet Prefab


            /*  clone = (GameObject)Instantiate(
                  bulletPrefab,
                  bulletSpawn.position,
                  bulletSpawn.rotation);

              InvokeRepeating("CmdFire", 0.0f, 0.3f);
              Debug.Log("test");
              */
            CmdFire();
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            totalHealth = totalHealth - 10;
            Debug.Log("ouch! your health is now: " + totalHealth);
            if (totalHealth <= 0)
            {
                CmdDestroyMyUnit();
                Debug.Log("you're dead.");
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

    public GameObject clone;
    GameObject cloneOnServer;

    [Command]
    void CmdFire()
    {
        GameObject go = Instantiate(clone); //object exists on the server
        go.transform.position = bulletSpawn.transform.GetComponent<Transform>().position;
        NetworkServer.Spawn(go);
        cloneOnServer = go;
        
        InvokeRepeating("LaunchProjectile", 0.0f, 0.3f);
    }
    void LaunchProjectile()
    {
        cloneOnServer.transform.Translate(1,0,0);
        Destroy(cloneOnServer,2.0F);
    }
        void OnPlayerNameChanged(string newName)
    {
        Debug.Log("changed name to: "+newName);
        playerName = newName;
        gameObject.GetComponentInChildren<TextMesh>().text = newName;
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
    /*[Command]
    void CmdUpdateVelocity(Vector3 v, Vector3 p) {
        transform.position = p;
        velocity = v;

        RpcUpdatVelocity(velocity, transform.position);
    }
    [ClientRpc]
    void RpcUpdatVelocity(Vector3 v, Vector3 p)
    {
        if (hasAuthority)
        {
            //this is my own object, it's already the most accurate.
            return;
        }
        //i need to listen to server to improve latency accuracy
        velocity = v;
        bestGuessPosition = p + (v * (ourLatency));
        
    }*/
}
