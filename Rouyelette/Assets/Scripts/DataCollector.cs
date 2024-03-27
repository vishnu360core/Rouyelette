using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DataCollector
{
    public enum GameSwitch {on, off };


    [System.Serializable]
    public class GameData 
    {
        public  GameSwitch status;
    }

    [System.Serializable]

    public class PlayerData
    {
        public string id;
        public List<Bet> bets;

        public int amount;
    }


    [System.Serializable]
    public class JsonData
    {
        public string playerFile = string.Empty;
        public string gameFile = string.Empty;
    }
}

