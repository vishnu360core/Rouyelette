using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent (typeof(MeshRenderer))]
public class Slot : MonoBehaviour
{
    [SerializeField] int number;
    public int SlotNumber => number;

    public enum ColorType {red,black,NULL};
    public enum SlotType { board, wheel }
    public enum BoardSlotType { integer, text,NULL }
    public enum BoardSlotMethod { odd, even, oneeighteen, red,black, ninteensixteen, first12, second12, third12,NULL}

    [Header("Slot Settings:")]
    [SerializeField] ColorType _colorType;
    public ColorType Colortype => _colorType;

    [SerializeField] SlotType _type;
    public SlotType Type => _type;

    [SerializeField] BoardSlotType _boardSlotType;
    public BoardSlotType BoardSlottype => _boardSlotType;

    [SerializeField] BoardSlotMethod _slotMethod;
    public BoardSlotMethod SlotMethod => _slotMethod;


    [Header("Chip placement:")]
    [SerializeField]Transform _chipTransform;
    public Transform ChipTransform => _chipTransform;


    MeshRenderer _meshRenderer;

    private void OnEnable()
    {
        if (_boardSlotType == BoardSlotType.integer)
        {
            string rollString = gameObject.name.Replace(" ", "");
            MatchCollection matches = Regex.Matches(rollString, @"\d+");
            foreach (Match match in matches)
            {
                number += int.Parse(match.Value);
            }
        }

        _meshRenderer = GetComponent<MeshRenderer>();
        _meshRenderer.enabled = false;

        Actions.EnableHoverAction += HoverEnableAction;
        Actions.ResetHoverAction += ResetAction;

        _chipTransform = this.transform.GetChild(0).transform;
    }

    /// <summary>
    /// Reset the slot
    /// </summary>
    void ResetAction()
    {
        OnHoverAction(false);
    }


    #region HOVER_ACTIONS

    void HoverEnableAction(bool onHover, BoardSlotMethod boardSlotMethod)
    {
        if (!onHover)
            return;


        switch (boardSlotMethod)
        {
            case BoardSlotMethod.red:
                OnHoverAction(_colorType == ColorType.red);
                break;

            case BoardSlotMethod.black:
                OnHoverAction(_colorType == ColorType.black);
                break;

            case  BoardSlotMethod.odd:
                if (_boardSlotType != BoardSlotType.integer)
                    return;

                OnHoverAction(number % 2 != 0);
                  break;

            case BoardSlotMethod.even:
                if (_boardSlotType != BoardSlotType.integer)
                    return;

                OnHoverAction(number % 2 == 0);
                break;

            case BoardSlotMethod.oneeighteen:
                if (_boardSlotType != BoardSlotType.integer)
                    return;

                OnHoverAction(number > 0 && number < 19);
                break;

            case BoardSlotMethod.ninteensixteen:
                if (_boardSlotType != BoardSlotType.integer)
                    return;

                OnHoverAction(number > 18 && number < 37);
                break;

            case BoardSlotMethod.first12:
                if (_boardSlotType != BoardSlotType.integer)
                    return;

                OnHoverAction(number > 0 && number < 13);
                break;

            case BoardSlotMethod.second12:
                if (_boardSlotType != BoardSlotType.integer)
                    return;

                OnHoverAction(number > 12  && number < 25);
                break;

            case BoardSlotMethod.third12:
                if (_boardSlotType != BoardSlotType.integer)
                    return;

                OnHoverAction(number > 24 && number < 37);
                break;

        }
    }

    public void OnHoverAction(bool OnHover)
    {
        if (_type == SlotType.wheel)
            return;


        _meshRenderer.enabled = OnHover;
    }

    private void OnMouseOver()
    {
       OnHoverAction(true);

       Actions.BoardHoverAction(_slotMethod);
    }

    private void OnMouseExit()
    {
        OnHoverAction(false);

        Actions.ResetHoverAction();
    }
    #endregion


    #region SELECT_ACTIONS

    private void OnMouseDown()
    {
        if (_type == SlotType.wheel)
            return;

        Actions.BoardSlotAction(this);
    }

    #endregion
}
