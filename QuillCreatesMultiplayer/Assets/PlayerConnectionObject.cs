using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerConnectionObject : NetworkBehaviour {

    // Use this for initialization
    void Start() {
        if (isLocalPlayer == false)
        {
            //this object belongs to another player.
            return;
        }
        //command server to spawn our unit
        CmdSpawnMyUnit();
        //totalHp = Int32.Parse(GameObject.Find("Hp").GetComponent<Text>().text);
    }

    public GameObject PlayerUnitPrefab;

    //syncvar is a variable where if value changes on server, then all clients will auto see the new change.
    [SyncVar(hook="OnPlayerNameChanged")] //hook runs a targeted function after value change. the hook function is
    //also an rpc, rcp is server sending something to the clients.
    public string PlayerName = "Anonymous";

    // Update is called once per frame
    void Update() {
        if (isLocalPlayer == false)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            CmdSpawnMyUnit();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            string n = "Player " + UnityEngine.Random.Range(1,100);
            CmdChangePlayerName(n);
        }
    }
    void OnPlayerNameChanged(string newName)
    {
        Debug.Log("hey! new name change... old was : " + PlayerName + ", new name is: " + newName);

        PlayerName = newName;    //ops remember hook on syncvar doesnt auto update local value
        //tip: while you're in hook, syncvar behaviour is disabled.

        gameObject.name = "PlayerUnit [" + newName + "]";
    }

    [Command] //commands are a way for clients to send a message to the server.
    void CmdSpawnMyUnit()
    {
        //we are guaranteed to be on the server right now.
        GameObject go = Instantiate(PlayerUnitPrefab); //object exists on the server
        NetworkServer.SpawnWithClientAuthority(go, connectionToClient); //spawn and spread gameobject all over the network.
        //also tell everyone on server that one person owns this
    }

    [Command]
    void CmdChangePlayerName(string n)
    {
        PlayerName = n; //syncvar also updates other clients with new name
    }
}
