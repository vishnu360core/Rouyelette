using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

using DataCollector;
using System;
using System.Linq;

public class ClientManager : MonoBehaviour
{

    [SerializeField] List<Client> clients = new List<Client>(); 

    [SerializeField] List<Chip> chipPrefabs  = new List<Chip>();

    [Header("Chip Settings:")]
    [Range(0, 10f)]
    [SerializeField] float speed;

    public string JsonData = string.Empty;

    bool onBetUpdate = false;

    string _currentJson;

    int clientIndex;

    private void Start()
    {
        clientIndex = 0;

        Actions.DeleteClient += DeleteClientAction;
    }


    public void ResetAction(string json)
    {
        Debug.Log("Client manager is reseting !!!!!!!!!!!" + json);

        PlayerDataList playerDataList = JsonUtility.FromJson<PlayerDataList>(json);
        List<PlayerData> playerDatas = playerDataList.playerDatas;

        for(int i = 0; i < clients.Count; i++) 
        {
           if (clients[i].PlayerData != null)
           {
               clients[i].UpdateBet(0);
           }
        }

        for (int i = 0; i < playerDatas.Count; i++)
        {         
           playerDatas[i].bets.Clear();
           playerDatas[i].amount = 100;
        }

        playerDataList.playerDatas = playerDatas;

        json = JsonUtility.ToJson(playerDataList);
        Debug.Log("Resetted json >>>>" + json);

        StartCoroutine(Network.Instance.SaveToNet(json));
    }


    public void SetJson(string json)
    {
        _currentJson = json;
    }

    #region CLIENT_STATS

    private void DeleteClientAction(string id)
    {
        foreach (var client in clients)
        {
            Debug.Log("Delete id " + client.PlayerData.id + ">>>" + id);
            if(client.PlayerData.id == id)
            {
                Debug.LogWarning("Deleting ...." + id);
                client.ClearData();

                PlayerDataList playerDataList = JsonUtility.FromJson<PlayerDataList>(_currentJson);
                List<PlayerData> playerDatas = playerDataList.playerDatas;

                playerDatas.Remove(playerDatas.Find(x => x.id == id));
                playerDataList.playerDatas = playerDatas;

                string jsonString = JsonUtility.ToJson(playerDataList);
                Debug.Log("Updated json after deletion >>>" + jsonString);

                StartCoroutine(Network.Instance.SaveToNet(jsonString));
                break;
            }
        }
    }

    public void AddClient(string id, string json)
    {
        PlayerData clientPlayerData = new PlayerData
        {
            id = id,
            bets = new List<Bet>(),
            amount = 100
        };

        Debug.Log("jSON >>>>>>" + json);

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

            for (int i = 0; i < playerDataList.playerDatas.Count; i++) 
            {
                if (playerDataList.playerDatas[i].id != Network.Instance.Id)
                {
                    if (!IsIdPresentClient(playerDataList.playerDatas[i].id))
                    {
                        clients[clientIndex].PlayerData = playerDataList.playerDatas[i];
                        clients[clientIndex].EnablePlayer(true);
                        clients[clientIndex].UpdateName(playerDataList.playerDatas[i].id);
                        clients[clientIndex].UpdateBet(ReturnTotalBets(playerDataList.playerDatas[i].bets));

                        clientIndex++;

                        Debug.Log("Client added >>" + clientIndex + ">>>" + clients[clientIndex - 1].PlayerData.id);
                    }
                }
            }

            jsonString = JsonUtility.ToJson(playerDataList);
            Debug.Log("Updated Player json :  " + jsonString);

            StartCoroutine(Network.Instance.SaveToNet(jsonString));
        }

        SetJson(jsonString);

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

    }

    #endregion

    bool IsIdPresentClient(string id)
    {
        bool isIdPresent = false;

        for(int i= 0;i<clients.Count;++i)
        {
            if (clients[i].PlayerData == null)
                continue;

            if (clients[i].PlayerData.id == id)
            {
                isIdPresent = true; 
                break;
            }
        }

        return isIdPresent;
    }

    bool IsIdPresent(List<PlayerData> list, string str)
    {
        return list.Exists(obj => obj.id == str);
    }


    public void UpdateClient(string id,Bet bet,string playerJson)
    {
        Debug.Log("Json for bet  >>>>>>>>>" + playerJson + ">>>>" + id);

        onBetUpdate = true;

        PlayerDataList playerDataList = JsonUtility.FromJson<PlayerDataList>(playerJson);
        List<PlayerData> playerDatas = playerDataList.playerDatas;

        foreach(PlayerData player in playerDatas) 
        {
            Debug.LogWarning("id >>>>" + player.id);

            if(Network.Instance.Id == player.id)
            {
                Debug.Log("Bet added " + bet.betAmount + " >>> " + player.id);

                player.bets.Add(bet);
                player.amount = 100;

                playerDataList.playerDatas = playerDatas;

                string jsonString = JsonUtility.ToJson(playerDataList);
                Debug.Log("Updated Player json after bets :  " + jsonString);

                StartCoroutine(Network.Instance.SaveToNet(jsonString));

                break;
            }
        }
    }

    #region CHIP_ACTIONS

    bool BetsAreEqual(List<Bet> list1, List<Bet> list2)
    {
        // Check if the lists have different lengths
        if (list1.Count != list2.Count)
        {
            return false;
        }

        // Iterate through each element of the lists and compare them
        for (int i = 0; i < list1.Count; i++)
        {
            if (list1[i].type != list2[i].type
             || list1[i].betNumber != list2[i].betNumber 
             || list1[i].betAmount != list2[i].betAmount
             || !AreIntegerArraysEqual(list1[i].splitNumbers, list2[i].splitNumbers)
                )
                 return false;
        }

        return true;
    }

    private bool AreIntegerArraysEqual(int[] array1, int[] array2)
    {
        if (array1.Length != array2.Length)
        {
            return false;
        }

        for (int i = 0; i < array1.Length; i++)
        {
            if (array1[i] != array2[i])
            {
                return false;
            }
        }

        return true;
    }


    public void ClientBetDetect(string json)
    {
        if (onBetUpdate)
        {
            onBetUpdate = false;
            return;
        }

        PlayerDataList playerDataList = JsonUtility.FromJson<PlayerDataList>(json);
        List<PlayerData> playerDatas = playerDataList.playerDatas;

     
       Debug.Log("Client player updating ........" + json);

        for(int i=0; clients.Count > i; i++) 
        {
            Debug.Log("Client id >>>" + clients[i].PlayerData.id);
        }

        for (int i = 0; i < playerDatas.Count; i++)
        {

            if (clients[0].PlayerData != null)
            {
                Debug.Log("Get id >>>" + playerDatas[i].id + ">>>" + clients[0].PlayerData.id);

                if (playerDatas[i].id == clients[0].PlayerData.id)
                {
                    Debug.LogWarning("Chip for client");

                    if (!BetsAreEqual(playerDatas[i].bets, clients[0].PlayerData.bets))
                    {
                        Debug.LogWarning("Chip movement for client");

                        clients[0].UpdateBet(ReturnTotalBets(playerDatas[i].bets));
                        ClientChipAction(playerDatas[i].bets, clients[0]._chipTransform);

                        clients[0].PlayerData.bets = playerDatas[i].bets;
                        break;
                    }
                }
            }

            if (clients[1].PlayerData != null)
            {
                Debug.Log("Get id >>>" + playerDatas[i].id + ">>>" + clients[1].PlayerData.id);

                if (playerDatas[i].id == clients[1].PlayerData.id)
                {
                    Debug.LogWarning("Chip  for client");

                    if (!BetsAreEqual(playerDatas[i].bets, clients[1].PlayerData.bets))
                    {
                        Debug.LogWarning("Chip movement for client");

                        clients[1].UpdateBet(ReturnTotalBets(playerDatas[i].bets));
                        ClientChipAction(playerDatas[i].bets, clients[1]._chipTransform);

                        clients[1].PlayerData.bets = playerDatas[i].bets;
                        break;
                    }
                }
            }

            if (clients[2].PlayerData != null)
            {
                Debug.Log("Get id >>>" + playerDatas[i].id + ">>>" + clients[2].PlayerData.id);

                if (playerDatas[i].id == clients[2].PlayerData.id)
                {
                    Debug.LogWarning("Chip  for client");

                    if (!BetsAreEqual(playerDatas[i].bets, clients[2].PlayerData.bets))
                    {
                        Debug.LogWarning("Chip movement for client");

                        clients[2].UpdateBet(ReturnTotalBets(playerDatas[i].bets));
                        ClientChipAction(playerDatas[i].bets, clients[2]._chipTransform);

                        clients[2].PlayerData.bets = playerDatas[i].bets;
                        break;
                    }
                }
            }
        }

    }


    int ReturnTotalBets(List<Bet> bets) 
    {
        int betAmount = 0;

        for(int i = 0; i < bets.Count; i++) 
        {
            betAmount += bets[i].betAmount;
        }

        return betAmount;
    }



    void ClientChipAction(List<Bet> bets,Transform chipStart)
    {
        if(bets.Count <= 0)
            return;


        Bet bet = bets[bets.Count - 1];
       

        Debug.Log("Client bet amount  >>>>" + bet.betAmount);

        GameObject chip = ChipGeneration(bet.betAmount);
        chip.transform.position = chipStart.position;

        Debug.Log("Chip >>>>>>>>" + chip);

        Debug.Log("BET >>>>"+ bet.type +">>>>" + bet.betNumber);

        Transform chipDestination = GetSlotPosition(bet.type,bet.betNumber,bet.splitNumbers);

        Debug.Log("Chip Destination >>>" + chipDestination);

        float duration = 10 - speed;

        if (chipDestination != null)
        {
            Debug.Log("Successful destination !!!!");

            Vector3 targetPosition = new Vector3(chipDestination.position.x, chip.transform.position.y, chipDestination.position.z);

            chip.transform.DOMove(targetPosition, duration);

            AudioManager.Instance.PlaySFX(AudioManager.SFX.chip);
        }
             
    }


    Transform GetSlotPosition(Slot.BoardSlotMethod method = Slot.BoardSlotMethod.NULL,int slotnumber = -1, int[] numbers = null)
    {
        
        foreach (Slot obj in GameObject.FindObjectsOfType<Slot>())
        {
            Slot slot = obj.GetComponent<Slot>();

            if (slot.Type == Slot.SlotType.wheel)
                continue;


            switch(method)
            {
                case Slot.BoardSlotMethod.NULL:

                    if(slot.SlotNumber == slotnumber && slotnumber != -1)
                        return obj.transform;

                    break;

                case Slot.BoardSlotMethod.split:

                    if (numbers.Length>0 && slot.SlotMethod == Slot.BoardSlotMethod.split)
                    {
                        Debug.Log("Split check >>>" + numbers.Length);

                         if(AreArraysEqual(numbers, slot.SpitNumbers))
                             return obj.transform;
                    }
                    break;

                default:
                    if (slot.SlotMethod == method && method != Slot.BoardSlotMethod.NULL)
                        return obj.transform;
                    break;
            }

            //if(slot.SlotMethod == method)
            //    return obj.transform;        
        }

        return null;
    }

    bool AreArraysEqual(int[] array1, int[] array2)
    {
        if (array1 == null || array2 == null)
            return false;

        if (array1.Length != array2.Length)
            return false;

        for (int i = 0; i < array1.Length; i++)
        {
            if (array1[i] != array2[i])
                return false;
        }

        return true;
    }


    GameObject ChipGeneration(int amount)
    {
        switch(amount) 
        {
            case 1:
                return Instantiate(chipPrefabs[0].gameObject);

            case 10:
                return Instantiate(chipPrefabs[1].gameObject);

            case 20:
                return Instantiate(chipPrefabs[2].gameObject);

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

