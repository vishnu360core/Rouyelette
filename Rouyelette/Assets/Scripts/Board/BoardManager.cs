using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;

using DataCollector;
using UnityEngine.Playables;

public interface BoardControlInterface
{
    public void EnableSpin(bool enable);

    public void BetProducedAction(int number,Slot.ColorType colorType);
}


public class BoardManager : MonoBehaviour,ChipInterface
{
    [Header("UI Settings:")]
    [SerializeField] TMP_Text _amountText;
    [SerializeField] TMP_Text _betAmountText;
    [SerializeField] Button betButton;
    [SerializeField] Button previousButton;
    [SerializeField] Button doubleButton;
    [SerializeField] Button clearButton;

    float amount;
    float totalWinAmount;
    int lossAmount = 0;

    [Header("Chips:")]
    [SerializeField] List<Chip> chips = new List<Chip>();

    #region BETS

    bool _isChipSelected = false;
    [SerializeField] int  _currentbetAmount;

    [SerializeField]Chip _currentChip;
    Slot _currentSlot;
 
    List<GameObject> chipObjects = new List<GameObject>();

    [SerializeField] List<Bet> bets = new List<Bet>();
   

    #endregion

    public BoardControlInterface callback;

    [SerializeField] Slot _currentWheelSlot;
    public Slot CurrentWheelSlot => _currentWheelSlot;

    [SerializeField] Slot _getSlot;

    [SerializeField] bool _reachedTargetSlot = false;

    public bool _setBet = false;

    enum Result {Win,Loss };

    bool _isPreviousPress = false;

    #region PREVIOUS_DATA

    List<Bet> previousBets = new List<Bet>();
    List<GameObject> previouschipObjects = new List<GameObject>();
    List<Chip> previousChips = new List<Chip>();
    List<Slot> previousSlots = new List<Slot>();

    #endregion

    private void Start()
    {
        totalWinAmount = 0.0f;
        lossAmount = 0;

        _setBet = false;
        _isPreviousPress = false;

        Actions.BoardHoverAction += HoverBoardSlotAction;
        Actions.BoardSlotAction += SelectBoardSlotAction;

        Actions.GetWalletBalance += GetWalletBalance;

        Actions.DeductionRejected += DeductionRejected;

        for (int i = 0; i < chips.Count; i++)
            chips[i].callback = this;


        callback.EnableSpin(false);

        //Actions.OnSlotAction += WheelSlotSelectAction;
        Actions.EndedSpinAction += EndSpinWheelAction;

       // Actions.ResetAction += ResetAction;

        Actions.OnSlotAction += SlotAction;


    }

    private void DeductionRejected()
    {
        Debug.Log("Deduction camcelled");


        amount += _currentbetAmount;
        _amountText.text = "Amount: $" + amount.ToString("F2");

        ClearBets();

        Actions.EnablePlay(true);
    }

    public void ClearBets()
    {
        _currentbetAmount = 0;
        _betAmountText.text = "Total Bet Amount: " + _currentbetAmount.ToString();

        Actions.DeleteChip();
        bets.Clear();

        _setBet = false;

       // EnableBet(true);
    }


    #region BOARD_CONTROL ACTIONS
    public void ClearBetButtonAction()
    {
        if (_setBet)
            return;

       ClearBets();

       previousSlots.Clear();

        _currentSlot = null;
    }


    public void DoubleBetAction()
    {
        if (_setBet)
            return;

        if (!_isPreviousPress)
            return;


        if (_currentbetAmount * 2 > amount)
            return;

        Debug.LogWarning("pre slots >>" + previousSlots.Count);


        List<Slot> slots = new List<Slot>();

        for(int i = 0;i < previousSlots.Count;++i)
        {
            slots.Add(previousSlots[i]);
        }

        for(int i = 0;i<slots.Count;++i)
        {
            _currentChip = previousChips[i];
            SelectBoardSlotAction(slots[i]);
        }
    }

    public void EnableBet(bool enable)
    {
        Debug.LogWarning("Bet Button !!!" + enable);

        betButton.interactable = enable;
        clearButton.interactable = enable;
        previousButton.interactable = enable;
        doubleButton.interactable = enable;
    }


    public void PreviousBetButtonAction()
    {
        if (_isPreviousPress)
            return;

        if (_setBet)
            return;

        _isPreviousPress = true;

        ClearBets();

        Debug.Log("Previous Objects Count >>" + previousChips.Count + ">>> " + previousBets.Count);

        int chipIndex = 0;

        if (previousBets.Count >0)
        {

            foreach(Bet bet in previousBets)
            {
                Debug.Log("Chip >>>" + previousChips[chipIndex].GetComponent<Chip>().Bet + ">>>" + previousSlots[chipIndex]);
                _currentChip =  chips.Find(x =>x.Bet == previousChips[chipIndex].GetComponent<Chip>().Bet);

                Debug.Log("chip >>>" + _currentChip);

                Slot slot = new Slot();

                slot = previousSlots[chipIndex];

                _isChipSelected = true;
                SelectBoardSlotAction(slot);
              
                chipIndex++;
            }
        }
       else
        {
            Debug.LogError("No previous bets !!!");
        }
    }

    #endregion

    private void GetWalletBalance(float bal)
    {
        amount = bal;

       _amountText.text = "$" + amount.ToString("F2");

    }

    public void SetGetSlot(Slot slot)
    {
        _getSlot= slot;
        _currentWheelSlot = _getSlot;
    }

    /// <summary>
    /// Spin Result Action
    /// </summary>
    /// <param name="slot"></param>
    private void SlotAction(Slot slot)
    {
        if (_reachedTargetSlot)
            return;

        Debug.Log("Get Slot >>>" + slot.SlotNumber + "CurrentSlot >>>" + _getSlot.SlotNumber);

        if (_getSlot.SlotNumber == slot.SlotNumber)
        {
            Debug.LogWarning("Reached destination !!!!!!");

            slot.EnableBall(true);

            Actions.ReachedDestination();

            _reachedTargetSlot = true;

            Actions.EnableSlotSetectAction(false);
            //Actions.OnSlotAction -= SlotAction;
        }
        else
        {
            Actions.MoveTowardTarget(slot.NextSlot);
        }
    }

    /// <summary>
    /// Action implemented on spin wheel stopped spining
    /// </summary>
    void EndSpinWheelAction()
    {
        callback.BetProducedAction(_getSlot.SlotNumber, _getSlot.Colortype);
        _reachedTargetSlot = false;

       Actions.ResetAction();

        DealerMovement();

        if (bets.Count > 0)
        {
            Debug.LogWarning("Bets checking  >>>");

            foreach (Bet bet in bets)
            {

                Slot.BoardSlotMethod method = bet.type;

                switch (method)
                {
                    case Slot.BoardSlotMethod.red:

                        if (_currentWheelSlot.Colortype == Slot.ColorType.red)
                            AddAmount_OnBet(bet.betAmount, 1);
                        else
                            // ResultAction(bet.betAmount, Result.Loss);
                            lossAmount += bet.betAmount;

                        break;

                    case Slot.BoardSlotMethod.black:

                        if (_currentWheelSlot.Colortype == Slot.ColorType.black)
                            AddAmount_OnBet(bet.betAmount, 1);
                        else
                            // ResultAction(bet.betAmount, Result.Loss);
                            lossAmount += bet.betAmount;

                        break;

                    case Slot.BoardSlotMethod.odd:

                        if (_currentWheelSlot.SlotNumber % 2 != 0)
                            AddAmount_OnBet(bet.betAmount, 1);
                        else
                            lossAmount += bet.betAmount;

                        break;

                    case Slot.BoardSlotMethod.even:

                        if (_currentWheelSlot.SlotNumber % 2 == 0)
                            AddAmount_OnBet(bet.betAmount, 1);
                        else
                            lossAmount += bet.betAmount;

                        break;

                    case Slot.BoardSlotMethod.oneeighteen:

                        if (_currentWheelSlot.SlotNumber > 0 && _currentWheelSlot.SlotNumber < 19)
                            AddAmount_OnBet(bet.betAmount, 1);
                        else
                            lossAmount += bet.betAmount;

                        break;

                    case Slot.BoardSlotMethod.ninteensixteen:

                        if (_currentWheelSlot.SlotNumber > 18 && _currentWheelSlot.SlotNumber < 37)
                            AddAmount_OnBet(bet.betAmount, 1);
                        else
                            lossAmount += bet.betAmount;

                        break;


                    case Slot.BoardSlotMethod.first12:

                        if (_currentWheelSlot.SlotNumber > 0 && _currentWheelSlot.SlotNumber < 13)
                            AddAmount_OnBet(bet.betAmount, 2);
                        else
                            lossAmount += bet.betAmount;

                        break;

                    case Slot.BoardSlotMethod.second12:

                        if (_currentWheelSlot.SlotNumber > 12 && _currentWheelSlot.SlotNumber < 25)
                            AddAmount_OnBet(bet.betAmount, 2);
                        else
                            lossAmount += bet.betAmount;

                        break;

                    case Slot.BoardSlotMethod.third12:

                        if (_currentWheelSlot.SlotNumber > 24 && _currentWheelSlot.SlotNumber < 37)
                            AddAmount_OnBet(bet.betAmount, 2);
                        else
                            lossAmount += bet.betAmount;

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

                        if (bet.splitNumbers.Length == 3)
                            splitMuilplier = 11;

                        if (bet.splitNumbers.Length == 4)
                            splitMuilplier = 8;

                        Debug.Log("Split CALLED  >>> " + splitMuilplier);

                        bool win = false;

                        for (int i = 0; i < bet.splitNumbers.Length; ++i)
                        {
                            Debug.Log("Split Index >>>" + bet.splitNumbers[i]);

                            if (_currentWheelSlot.SlotNumber == bet.splitNumbers[i])
                            {
                                Debug.Log("Spit Number added with " + splitMuilplier);

                                AddAmount_OnBet(bet.betAmount, splitMuilplier);
                                win = true;
                                break;
                            }
                        }

                        if (!win)
                            lossAmount += bet.betAmount;

                        break;

                    case Slot.BoardSlotMethod.NULL:
                        if (bet.betNumber == _currentWheelSlot.SlotNumber)
                        {
                            AddAmount_OnBet(bet.betAmount, 35);
                        }
                        else
                            lossAmount += bet.betAmount;

                        break;
                }

            }

            if (totalWinAmount > 0)
                ResultAction((int)totalWinAmount, Result.Win);
            else
                ResultAction(lossAmount, Result.Loss);
        }


       ResetAction();
    }

    void DealerMovement()
    {
        int _index = _getSlot.SlotNumber;

        Transform dealerPosition = null;


        foreach(Slot obj in GameObject.FindObjectsOfType<Slot>()) 
        {
            Slot slot = obj.GetComponent<Slot>();

          
            if(slot.Type == Slot.SlotType.board && slot.SlotNumber == _index && slot.BoardSlottype == Slot.BoardSlotType.integer)
            {
                Debug.LogWarning("Dealer >>>" + slot.Type  + " " +  slot.SlotNumber);

                dealerPosition = obj.transform;
                break;
            }
        }

        Actions.DealerMoveAction(dealerPosition);
    }


    /// <summary>
    /// Result updater
    /// </summary>
    /// <param name="betamount"></param>
    /// <param name="result"></param>
    void ResultAction(int betamount,Result result)
    {
        switch(result)
        {
            case Result.Win:
                PopMessage.Instance.PopUpMessage(PopMessage.MessageType.win, "You Win :" + betamount.ToString());
                AudioManager.Instance.PlaySFX(AudioManager.SFX.win);

                HistoryController.Instance.profit = betamount;

                HistoryController.Instance.HistoryGenerateAction();

                amount += betamount;
                //_amountText.text = "Amount: $" + amount.ToString("F2");

                Network.Instance.CreditAmount((float)betamount);

                break;

            case Result.Loss:

                if (totalWinAmount > 0)
                    break;

                PopMessage.Instance.PopUpMessage(PopMessage.MessageType.lost, "You lost :" + betamount.ToString());
                AudioManager.Instance.PlaySFX(AudioManager.SFX.loss);

               // amount += betamount;
                //_amountText.text = "Amount: $" + amount.ToString("F2");

                //Network.Instance.DeductAmount((float)betamount);
                break;
        }
    }
    
    /// <summary>
    /// Check the ratio of the selected the slot range
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="difference"></param>
    /// <param name="bet"></param>
    void RatioCheckAction(int start, int end , int difference,Bet bet)
    {
        if (_currentWheelSlot.SlotNumber == 1 || _currentWheelSlot.SlotNumber == 34)
        {
            AddAmount_OnBet(bet.betAmount, 2);
            return;
        }


        bool win = false;

        for (int i = start; i <= end - difference; i += difference)
        {
            if (_currentWheelSlot.SlotNumber == i)
            {
                AddAmount_OnBet(bet.betAmount, 2);
                win = true;
            }
        }

        if (!win) 
        {
            // ResultAction(bet.betAmount, Result.Loss);
            lossAmount += bet.betAmount;

        }

    }

    void AddAmount_OnBet(int betamount, int multiplier)
    {
        //int winAmount = betamount + betamount * multiplier;

        totalWinAmount += betamount + betamount * multiplier;

       // ResultAction(winAmount, Result.Win);

        //amount += betamount + betamount * multiplier;
        //_amountText.text = "Amount:" + amount.ToString();
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
        totalWinAmount = 0;
        lossAmount = 0; 

        _currentWheelSlot = null;


        bets.Clear();

        callback.EnableSpin(false);

        _betAmountText.text = "TotalBet: " + _currentbetAmount.ToString();
    }

    /// <summary>
    /// Action implemented on slot select 
    /// </summary>
    /// <param name="slot"></param>
    private void SelectBoardSlotAction(Slot slot)
    {
         if (!_isChipSelected)
          return;

        if (amount < _currentbetAmount+1 || amount < _currentChip.Bet)
        {
            AudioManager.Instance.PlaySFX(AudioManager.SFX.error);
            return;
        }

        if (!_isPreviousPress)
        {
            previousSlots.Clear();
            previousChips.Clear();
            previouschipObjects.Clear();    

            _isPreviousPress = true;
        }

        AudioManager.Instance.PlaySFX(AudioManager.SFX.chip);

        if (slot.BoardSlottype == Slot.BoardSlotType.integer)
        {    
            if (bets.Count > 0) 
            {
                if( bets.Find(x => x.betNumber == slot.SlotNumber) != null)
                {
                    bets.Find(x => x.betNumber == slot.SlotNumber).betAmount += _currentChip.Bet;

                    Bet bet = bets.Find(x => x.betNumber == slot.SlotNumber);
                    SendBetAction(_currentChip.Bet, slot.SlotNumber);
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
                    SendBetAction(_currentChip.Bet, slot.SlotNumber);
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
                SendBetAction(_currentChip.Bet, slot.SlotNumber);
            }

            _currentbetAmount += _currentChip.Bet;
            // amount -= _currentChip.Bet;
            previousChips.Add(_currentChip);


            _betAmountText.text = "TotalBet: " + _currentbetAmount.ToString();
            // _amountText.text = "Amount: $" + amount.ToString("F2");

            previousSlots.Add(slot);
           
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

                           SendBetAction(_currentChip.Bet, -1, method, slot.SpitNumbers);
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
                    SendBetAction(_currentChip.Bet, -1, method, slot.SpitNumbers);
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
                SendBetAction(_currentChip.Bet, -1, method,slot.SpitNumbers);
            }
        }
        else
        {
            if (bets.Count > 0)
            {
                if (bets.Find(x => x.type == method) != null)
                {
                    bets.Find(x => x.type == method).betAmount += _currentChip.Bet;

                    Bet bet = bets.Find(x => x.type == method);
                    SendBetAction(_currentChip.Bet, -1, method);
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
                    SendBetAction(_currentChip.Bet, -1, method);
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
                SendBetAction(_currentChip.Bet,-1,method);
            }
        }

        _currentbetAmount += _currentChip.Bet;

        previousChips.Add(_currentChip);

       // amount -= _currentChip.Bet;

       // Actions.PlayerBets(bets,amount);

        _betAmountText.text = "TotalBet: " + _currentbetAmount.ToString();
        // _amountText.text = "Amount: $" + amount.ToString("F2");

        previousSlots.Add(slot);
        AddChipAction(slot);

        callback.EnableSpin(true);
    }


    public void SetBetAction()
    {
        if (_setBet)
        {
            return;
        }

        if (_currentbetAmount == 0)
            return;

        EnableBet(false);

        _isPreviousPress = false;
        _setBet = true;
        amount -= _currentbetAmount;
        //_amountText.text = "Amount: $" + amount.ToString("F2");

        //HistoryController.Instance.bets = _currentbetAmount;

        Debug.LogWarning("Bets Count >>>" + bets.Count);

        if (bets.Count > 0)
        {
            for (int i = 0;i<bets.Count;++i)
            {
                previousBets.Add(bets[i]);
            }


            Debug.LogWarning("Bets Count previous >>>" + previousBets.Count);
        }
        else
        {
            Debug.LogError("Bets were cleared !!!");
        }


        Debug.LogWarning("Chip Objects >>" + chipObjects.Count +">>" + previousChips.Count); 
        
        if(chipObjects.Count > 0) 
        {
            for (int i = 0; i < chipObjects.Count; ++i)
            {
                previouschipObjects.Add(chipObjects[i]);    
            }
        }
        else
        {
            Debug.LogError("bet objects were cleared!!");
        }


        Actions.EnablePlay(false);

        Network.Instance.DeductAmount(_currentbetAmount);
    }

    /// <summary>
    /// Sending the slot selection data
    /// </summary>
    /// <param name="amount"></param>
    /// <param name="number"></param>
    /// <param name="method"></param>
    /// <param name="_splitNumbers"></param>
    void SendBetAction(int amount,int number, Slot.BoardSlotMethod method =  Slot.BoardSlotMethod.NULL, int[] _splitNumbers = null)
    {
        Bet bet = new Bet
        {
            betAmount = amount,
            betNumber = number,
            splitNumbers = _splitNumbers,
            type = method
        };

        Actions.PlayerBets(bet);
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
        go.transform.localScale = Vector3.one;

        Debug.Log("Chip object 1" + go);

       
        if (go.GetComponent<Chip>() != null)
        {
            Chip chip = go.GetComponent<Chip>();
            //chip.EnableAnimation(false);


            Debug.Log("Chip object 2");
            //Material[] mats = go.GetComponent<MeshRenderer>().materials;
            //Material[] _textmats = go.transform.GetChild(0).GetComponent<MeshRenderer>().materials;


            //Array.Resize(ref mats, mats.Length - 2);
            //Array.Resize(ref _textmats, _textmats.Length - 2);

            //go.GetComponent<MeshRenderer>().materials = mats;
            //go.transform.GetChild(0).GetComponent<MeshRenderer>().materials = _textmats;

            Destroy(chip);

            //BoxCollider boxCollider = go.GetComponent<BoxCollider>();
            //Destroy(boxCollider);
        }


        Debug.Log("Chip object 3");

        //go.transform.SetParent(slot.ChipTransform, false);

        //go.transform.position = slot.ChipTransform.position;

        if (chipObjects.Count == 0)
            go.transform.position = slot.ChipTransform.position;
        else
        {
            int floor = chipObjects.Count;

            go.transform.localPosition = new Vector3(slot.ChipTransform.position.x, slot.ChipTransform.position.y + floor * 0.05f, slot.ChipTransform.position.z);
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
        chip.gameObject.transform.localScale = Vector3.one;

        Actions.ResetHoverAction();

        AudioManager.Instance.PlaySFX(AudioManager.SFX.chip);

        _isChipSelected = true;

        //_currentbetAmount = chip.Bet;
        _currentChip = chip;
        _currentChip.transform.localScale = Vector3.one;

       
        for (int i = 0; i < chips.Count; ++i)
        {
            if (chips[i] != chip)
                chips[i].ResetAction();
        }
    }
}
