using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;



public class History : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField] TMP_Text _idText;
    [SerializeField] TMP_Text _betsText;
    [SerializeField] TMP_Text _profitText;
    [SerializeField] TMP_Text _blockText;
    [SerializeField] TMP_Text _transactionText;

    public void IntialiseValues(int id,int bets,int profit,int blockNumber,string hash)
    {
         _idText.text = id.ToString();
        _betsText.text = bets.ToString();
        _profitText.text = profit.ToString();
        _blockText.text = blockNumber.ToString();
        _transactionText.text = hash.ToString();
    }
}
