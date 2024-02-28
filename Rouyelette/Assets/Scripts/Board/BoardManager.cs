using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

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

    private void Start()
    {
        Actions.BoardHoverAction += HoverBoardSlotAction;
        Actions.BoardSlotAction += SelectBoardSlotAction;

        for (int i = 0; i < chips.Count; i++)
            chips[i].callback = this;

        amount = 100;

       _amountText.text = "Amount:" + amount.ToString();

        callback.EnableSpin(false);

        Actions.OnSlotAction += WheelSlotSelectAction;
        Actions.EndedSpinAction += EndSpinWheelAction;

      //  Actions.ResetAction += ResetAction;
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



                    case Slot.BoardSlotMethod.NULL:
                        if (bet.betNumber == _currentWheelSlot.SlotNumber)
                        {
                            amount += bet.betAmount + bet.betAmount * 35;
                            _amountText.text = "Amount:" + amount.ToString();
                            return;
                        }
                    break;
                }

        }

        ResetAction();

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
