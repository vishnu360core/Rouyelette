using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

using DataCollector;

public class ClientManager : MonoBehaviour
{

    [SerializeField] List<Client> clients = new List<Client>(); 

    [SerializeField] List<PlayerData> client_datas = new List<PlayerData>();
    [SerializeField] List<Chip> chipPrefabs  = new List<Chip>();

    [Header("Chip Settings:")]
    [Range(0, 10f)]
    [SerializeField] float speed;


    public string JsonData = string.Empty;


    private void Start()
    {
        client_datas.Clear();
    }

    public void AddClient(string id,string json)
    {
        PlayerData clientPlayerData = new PlayerData
        {
            id = id,
            bets = new List<Bet>(),
            amount = 100
        };

        Debug.Log("jSON >>>>>>" +json);

        string jsonString = string.Empty;

        if (json == string.Empty)
        {
            List<PlayerData> playerDatas = new List<PlayerData>();

            playerDatas.Add(clientPlayerData);

            PlayerDataList playerDataList = new PlayerDataList();

            playerDataList.playerDatas = playerDatas;

            jsonString = JsonUtility.ToJson(playerDataList);
            Debug.Log("Player json :  " + jsonString + playerDatas.Count);

            StartCoroutine(Network.Instance.SaveToNet(jsonString));
        }
        else
        {
            PlayerDataList playerDataList = JsonUtility.FromJson<PlayerDataList>(json);

            List<PlayerData> playerDatas = playerDataList.playerDatas;

            playerDatas.Add(clientPlayerData);

            playerDataList.playerDatas = playerDatas;

            jsonString = JsonUtility.ToJson(playerDataList);
            Debug.Log("Updated Player json :  " + jsonString);

            StartCoroutine(Network.Instance.SaveToNet(jsonString));
        }


        //if (playerData == null)
        //{
        //    clientPlayerData = new PlayerData
        //    {
        //        id = id,
        //        bets = new List<Bet>(),
        //        amount = 100
        //    };

        //    string jsonString = JsonUtility.ToJson(clientPlayerData);
        //    Debug.Log("Player json :  " + jsonString);

        //    StartCoroutine(Network.Instance.SaveToNet(jsonString));
        //}
        //else
        //{
        //    Debug.Log("Player data is not null");

        //    clientPlayerData = playerData;

        //    foreach (Client clientPlayer in clients)
        //    {
        //        if (clientPlayer.taken)
        //            continue;

        //        Debug.Log("Client player adding ........");

        //        clientPlayer.taken = true;
        //        clientPlayer.PlayerData = playerData;

        //        ClientChipAction(clientPlayer.PlayerData.bets, clientPlayer._chipTransform);
        //    }
        //}

        Debug.Log("Client added !!!!");
        client_datas.Add(clientPlayerData);
    }


    public void UpdateClient(PlayerData playerData , string playerJson)
    {
        Debug.Log("Json for bet  >>>>>>>>>" + playerJson + ">>>>" + playerData.id);

        PlayerDataList playerDataList = JsonUtility.FromJson<PlayerDataList>(playerJson);
        List<PlayerData> playerDatas = playerDataList.playerDatas;

        foreach(PlayerData player in playerDatas) 
        {
            Debug.LogWarning("id >>>>" + player.id);

            if(playerData.id == player.id)
            {
                player.bets = playerData.bets;
                player.amount = playerData.amount;


                playerDataList.playerDatas = playerDatas;

                string jsonString = JsonUtility.ToJson(playerDataList);
                Debug.Log("Updated Player json after bets :  " + jsonString);

                StartCoroutine(Network.Instance.SaveToNet(jsonString));

                break;
            }
        }
    }

    #region CHIP_ACTIONS


    public void ClientBetDetect(string json)
    {
        PlayerDataList playerDataList = JsonUtility.FromJson<PlayerDataList>(json);
        List<PlayerData> playerDatas = playerDataList.playerDatas;

        Client clientPlayer = clients[0];


        Debug.Log("Client player updating ........" + json);

            for (int i = 0; i < playerDatas.Count; i++)
            {
                if (Network.Instance.Id != playerDatas[i].id)
                {
                   clientPlayer.PlayerData = playerDatas[i];
                   ClientChipAction(clientPlayer.PlayerData.bets, clientPlayer._chipTransform);
                }
            }

    }

    void ClientChipAction(List<Bet> bets,Transform chipStart)
    {
        if(bets.Count <= 0)
            return;

        foreach(Bet bet in bets) 
        {
            Slot.BoardSlotMethod method = bet.type;


            GameObject chip = ChipGeneration(bet.betAmount);
            chip.transform.position = chipStart.position;

            Transform chipDestination = GetSlotPosition(method);

            Debug.Log("Chip Destination >>>" + chipDestination);

            float duration = 10 - speed;


            if(chipDestination != null)
            {
                Debug.Log("Successful destination !!!!");
                
                Vector3 targetPosition = new Vector3(chipDestination.position.x,chip.transform.position.y,chipDestination.position.z);

                chip.transform.DOMove(targetPosition, duration);
            }
        }

    }


    Transform GetSlotPosition(Slot.BoardSlotMethod method)
    {

        foreach (Slot obj in GameObject.FindObjectsOfType<Slot>())
        {
            Slot slot = obj.GetComponent<Slot>();

            if (slot.Type == Slot.SlotType.wheel)
                continue;

            if(slot.SlotMethod == method)
                return obj.transform;        
        }

        return null;
    }


    GameObject ChipGeneration(int amount)
    {
        switch(amount) 
        {
            case 50:
                return Instantiate(chipPrefabs[3].gameObject);
        }


        return null;
    }
    #endregion

}


[System.Serializable]
public class PlayerDataList
{
    public List<PlayerData> playerDatas;   
}

