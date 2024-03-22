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
}
