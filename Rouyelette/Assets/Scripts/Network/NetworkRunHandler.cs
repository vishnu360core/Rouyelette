using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using System.Threading.Tasks;

public class NetworkRunHandler : MonoBehaviour
{
    [SerializeField] NetworkRunner runnerPrefab;
    
    NetworkRunner runner;

    // Start is called before the first frame update
    void Start()
    {
        runner = Instantiate(runnerPrefab);
        runner.name = "NetworkRunner";

        var clientTask = IntializeNetworkRunner(runner, GameMode.AutoHostOrClient, NetAddress.Any(), SceneRef.FromIndex(0) , null);

        Debug.Log("Server Network Started .");

    }

    protected virtual Task IntializeNetworkRunner(NetworkRunner runner, GameMode gameMode, NetAddress netAddress, SceneRef scene, Action<NetworkRunner> intialised)
    {
        var sceneManager = runner.GetComponents(typeof(MonoBehaviour)).OfType<INetworkSceneManager>().FirstOrDefault();

        if (sceneManager != null)
        {
            sceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>();
        }

        runner.ProvideInput = true;


        return runner.StartGame(new StartGameArgs 
        { 
          GameMode = gameMode,
          Address = netAddress,
          Scene = scene,
          SessionName = "TestRoom",
          HostMigrationResume = intialised,
          SceneManager = sceneManager 
        });
    }

}
 