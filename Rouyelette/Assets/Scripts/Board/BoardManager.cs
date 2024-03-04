using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;

public interface BoardControlInterface
{
    public void EnableSpin(bool enable);
}



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

    public BoardControlInterface callback;

    [SerializeField] Slot _currentWheelSlot;
    public Slot CurrentWheelSlot => _currentWheelSlot;

    [SerializeField] Slot _GetSlot;

    bool _reachedTargetSlot = false;

    private void Start()
    {
        Actions.BoardHoverAction += HoverBoardSlotAction;
        Actions.BoardSlotAction += SelectBoardSlotAction;

        for (int i = 0; i < chips.Count; i++)
            chips[i].callback = this;

        amount = 100;

       _amountText.text = "Amount:" + amount.ToString();

        callback.EnableSpin(false);

        //Actions.OnSlotAction += WheelSlotSelectAction;
        // Actions.EndedSpinAction += EndSpinWheelAction;

        //  Actions.ResetAction += ResetAction;

        Actions.OnSlotAction += SlotAction;
    }

    private void SlotAction(Slot slot)
    {
        if (_reachedTargetSlot)
            return;

        Debug.Log("Get Slot >>>" + slot.SlotNumber + "CurrentSlot >>>" + _GetSlot.SlotNumber);

        if (_GetSlot.SlotNumber == slot.SlotNumber)
        {
            Debug.LogWarning("Reached destination !!!!!!");

            Actions.ReachedDestination();

            _reachedTargetSlot = true;

            Actions.EnableSlotSetectAction(false);

            WheelSlotSelectAction(slot);
            //Actions.OnSlotAction -= SlotAction;
        }
        else
        {
            Actions.MoveTowardTarget(slot.NextSlot.transform);
        }
    }

    void EndSpinWheelAction()
    {
        Actions.ResetAction();

        if (bets.Count == 0)
            return;

        foreach(Bet bet in bets)
        {
        
                Slot.BoardSlotMethod method = bet.type;

                switch (method) 
                {
                    case Slot.BoardSlotMethod.red:

                    if (_currentWheelSlot.Colortype == Slot.ColorType.red)
                        AddAmount_OnBet(bet.betAmount, 1);

                    break;

                    case Slot.BoardSlotMethod.black:

                    if (_currentWheelSlot.Colortype == Slot.ColorType.black)
                        AddAmount_OnBet(bet.betAmount, 1);

                    break;

                    case Slot.BoardSlotMethod.odd:

                    if (_currentWheelSlot.SlotNumber % 2 != 0)
                        AddAmount_OnBet(bet.betAmount, 1);

                    break;

                    case Slot.BoardSlotMethod.even:

                    if (_currentWheelSlot.SlotNumber % 2 == 0)
                        AddAmount_OnBet(bet.betAmount, 1);

                    break;

                    case Slot.BoardSlotMethod.oneeighteen:

                    if (_currentWheelSlot.SlotNumber > 0 && _currentWheelSlot.SlotNumber < 19)
                        AddAmount_OnBet(bet.betAmount, 1);

                    break;

                    case Slot.BoardSlotMethod.ninteensixteen:

                    if (_currentWheelSlot.SlotNumber > 18 && _currentWheelSlot.SlotNumber < 37)
                        AddAmount_OnBet(bet.betAmount, 1);

                    break;


                    case Slot.BoardSlotMethod.first12:

                    if (_currentWheelSlot.SlotNumber > 0 && _currentWheelSlot.SlotNumber < 13)
                        AddAmount_OnBet(bet.betAmount, 2);

                    break;

                   case Slot.BoardSlotMethod.second12:

                    if (_currentWheelSlot.SlotNumber > 12 && _currentWheelSlot.SlotNumber < 25)
                        AddAmount_OnBet(bet.betAmount, 2);

                    break;

                   case Slot.BoardSlotMethod.third12:

                    if (_currentWheelSlot.SlotNumber > 24 && _currentWheelSlot.SlotNumber < 37)
                        AddAmount_OnBet(bet.betAmount, 2);

                    break;

                    case Slot.BoardSlotMethod.FirstRow:

                    RatioCheckAction(1, 34, 3, bet);

                    break;

                     case Slot.BoardSlotMethod.SecondRow:

                    RatioCheckAction(2, 35, 3, bet);

                    break;

                    case Slot.BoardSlotMethod.ThirdRow:

                    RatioCheckAction(3, 36, 3, bet);

                    break;

                     case Slot.BoardSlotMethod.split:

                        int splitMuilplier = -1;

                        if (bet.splitNumbers.Length == 2)
                            splitMuilplier = 17;

                        if (bet.splitNumbers.Length == 4)
                            splitMuilplier = 8;

                      Debug.Log("Split CALLED  >>> " + splitMuilplier);

                          for (int i = 0;i<bet.splitNumbers.Length;++i)
                          { 
                               Debug.Log("Split Index >>>" + bet.splitNumbers[i]); 

                                if(_currentWheelSlot.SlotNumber == bet.splitNumbers[i])
                                {
                                   Debug.Log("Spit Number added with " + splitMuilplier);
                                  
                                   AddAmount_OnBet(bet.betAmount, splitMuilplier);
                                   break;
                                }
                          }

                     break;

                    case Slot.BoardSlotMethod.NULL:
                        if (bet.betNumber == _currentWheelSlot.SlotNumber)
                        {
                            AddAmount_OnBet(bet.betAmount, 35);
                        }

                    break;
                }

        }

        ResetAction();

    }
    
    void RatioCheckAction(int start, int end , int difference,Bet bet)
    {
        if (_currentWheelSlot.SlotNumber == 1 || _currentWheelSlot.SlotNumber == 34)
        {
            AddAmount_OnBet(bet.betAmount, 2);
            return;
        }


        for (int i = start; i <= end - difference; i += difference)
        {
            if (_currentWheelSlot.SlotNumber == i)
            {
                AddAmount_OnBet(bet.betAmount, 2);
            }
        }
    }

    void AddAmount_OnBet(int betamount, int multiplier)
    {
        amount += betamount + betamount * multiplier;
        _amountText.text = "Amount:" + amount.ToString();
    }


    private void WheelSlotSelectAction(Slot slot)
    {
       _currentWheelSlot = slot;
    }

    /// <summary>
    /// Reset the board
    /// </summary>
    public void ResetAction()
    {
        _isChipSelected = false;
        _currentbetAmount = 0;

        bets.Clear();

        callback.EnableSpin(false);
    }

    /// <summary>
    /// Action implemented on slot select 
    /// </summary>
    /// <param name="slot"></param>
    private void SelectBoardSlotAction(Slot slot)
    {
        if (!_isChipSelected)
            return;


        if (amount < _currentChip.Bet)
        {
            AudioManager.Instance.PlaySFX(AudioManager.SFX.error);
            return;
        }

        AudioManager.Instance.PlaySFX(AudioManager.SFX.chip);

        if (slot.BoardSlottype == Slot.BoardSlotType.integer)
        {    
            if (bets.Count > 0) 
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

            _currentbetAmount += _currentChip.Bet;
            amount -= _currentChip.Bet;

            _amountText.text = "Amount:" + amount.ToString();
            AddChipAction(slot);

            callback.EnableSpin(true);
            return;
        }


        Slot.BoardSlotMethod method = slot.SlotMethod;


        if (method == Slot.BoardSlotMethod.split)
        {
            if (bets.Count > 0)
            {
                bool isNewArray = true;

                foreach (var bet in bets)
                {
                    if (bet.splitNumbers != null)
                    {
                        if (bet.splitNumbers.SequenceEqual(slot.SpitNumbers))
                        {
                            isNewArray = false;
                            bet.betAmount += _currentChip.Bet;
                            break;
                        }
                    }
                }


                if (isNewArray)
                {
                    Bet bet = new Bet
                    {
                        betAmount = _currentChip.Bet,
                        betNumber = -1,
                        splitNumbers = slot.SpitNumbers,
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
                    splitNumbers = slot.SpitNumbers,
                    type = method
                };

                bets.Add(bet);
            }
        }
        else
        {
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
        }

        _currentbetAmount += _currentChip.Bet;
        amount -= _currentChip.Bet;

        _amountText.text = "Amount:" + amount.ToString();

        AddChipAction(slot);

        callback.EnableSpin(true);
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
        go.transform.localScale = new Vector3(0.9f,0.9f,0.9f);

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

        AudioManager.Instance.PlaySFX(AudioManager.SFX.chip);

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
