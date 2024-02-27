using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;


public class BoardManager : MonoBehaviour,ChipInterface
{
    [Header("UI Settings:")]
    [SerializeField] TMP_Text _amountText;

    int amount;

    [Header("Chips:")]
    [SerializeField] List<Chip> chips = new List<Chip>();

    #region BETS

    bool _isChipSelected = false;
    int  _currentbetAmount;

    Chip _currentChip;
    Slot _currentSlot;
 
    List<GameObject> chipObjects = new List<GameObject>();

    [SerializeField] List<Bet> bets = new List<Bet>();

    #endregion

    private void Start()
    {
        Actions.BoardHoverAction += HoverBoardSlotAction;
        Actions.BoardSlotAction += SelectBoardSlotAction;

        for (int i = 0; i < chips.Count; i++)
            chips[i].callback = this;

        amount = 100;

       _amountText.text = "Amount:" + amount.ToString();
    }

    /// <summary>
    /// Reset the board
    /// </summary>
    public void ResetAction()
    {
        _isChipSelected = false;
        _currentbetAmount = 0;

        bets.Clear();
    }

    /// <summary>
    /// Action implemented on slot select 
    /// </summary>
    /// <param name="slot"></param>
    private void SelectBoardSlotAction(Slot slot)
    {
        if (!_isChipSelected)
            return;

     
        if (slot.BoardSlottype == Slot.BoardSlotType.integer)
        {
            if (amount >= _currentChip.Bet)
            {
                if(bets.Count > 0) 
                {


                   if( bets.Find(x => x.betNumber == slot.SlotNumber) != null)
                   {
                        bets.Find(x => x.betNumber == slot.SlotNumber).betAmount += _currentChip.Bet;
                   }
                   else
                   {
                        Bet bet = new Bet
                        {
                            betAmount = _currentChip.Bet,
                            betNumber = slot.SlotNumber,
                            type = Slot.BoardSlotMethod.NULL
                        };

                        bets.Add(bet);
                   }
                   
                }
                else
                {
                    Bet bet = new Bet
                    {
                        betAmount = _currentChip.Bet,
                        betNumber = slot.SlotNumber,
                        type = Slot.BoardSlotMethod.NULL
                    };


                    bets.Add(bet);
                }

               
            }


            _amountText.text = "Amount:" + amount.ToString();
            return;
        }


        Slot.BoardSlotMethod method = slot.SlotMethod; 

        if (bets.Count > 0)
        {
            if (bets.Find(x => x.type == method) != null)
            {
                bets.Find(x => x.type == method).betAmount += _currentChip.Bet;
            }
            else
            {
                Bet bet = new Bet
                {
                    betAmount = _currentChip.Bet,
                    betNumber = -1,
                    type = method
                };

                bets.Add(bet);
            }

        }
        else
        {
            Bet bet = new Bet
            {
                betAmount = _currentChip.Bet,
                betNumber = -1,
                type = method
            };

            bets.Add(bet);
        }

        _currentbetAmount += _currentChip.Bet;
        amount -= _currentChip.Bet;

        AddChipAction(slot);
    }

    /// <summary>
    /// AddAction the chip to the slot
    /// </summary>
    /// <param name="slot"></param>
    void AddChipAction(Slot slot)
    {
        if (_currentSlot != slot)
        {
            _currentSlot = slot;
            chipObjects.Clear();
        }

        GameObject go = Instantiate(_currentChip.gameObject);

        if (go.GetComponent<Chip>() != null)
        {
            Chip chip = go.GetComponent<Chip>();
            Destroy(chip);

            BoxCollider boxCollider = go.GetComponent<BoxCollider>();
            Destroy(boxCollider);
        }

        go.transform.SetParent(slot.ChipTransform, false);
        go.transform.localScale = Vector3.one;

        if (chipObjects.Count == 0)
            go.transform.localPosition = Vector3.zero;
        else
        {
            int floor = chipObjects.Count;

            go.transform.localPosition = new Vector3(0, floor * 0.01f, 0);
        }

        go.transform.localRotation = Quaternion.identity;

        chipObjects.Add(go);
    }


    /// <summary>
    /// Hover over slot Action
    /// </summary>
    /// <param name="slotMethod"></param>
    void HoverBoardSlotAction(Slot.BoardSlotMethod slotMethod = Slot.BoardSlotMethod.NULL)
    {
        Actions.EnableHoverAction(true, slotMethod);
    }


    /// <summary>
    /// Action implemented on chip select
    /// </summary>
    /// <param name="chip"></param>
    public void ChipSelecion(Chip chip)
    {
        Actions.ResetHoverAction();

        _isChipSelected = true;

        _currentbetAmount = chip.Bet;
        _currentChip = chip;

        for (int i = 0; i < chips.Count; ++i)
        {
            if (chips[i] != chip)
                chips[i].ResetAction();
        }
    }
}
