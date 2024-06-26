using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Video;
using System;

using DG.Tweening;

public interface ChipInterface
{
    public void ChipSelecion(Chip chip);    
}

public class Chip : MonoBehaviour
{
   
    [SerializeField] int _number;
    public int Bet=>_number;

    Vector3 _position = Vector3.zero;

    bool _selected = false;

    public ChipInterface callback;

    bool enablePlay = false;

    [SerializeField] RectTransform _rect;

    public enum PlayMode {play,View}

    [Space]
    [SerializeField] PlayMode playMode;

    [Header("Chip:")]
    [SerializeField] Chip _chipObject;


    private void OnEnable()
    {
        _position = transform.position;

        Actions.ResetAction += ResetAction;

        Actions.EnablePlay += EnablePlay;
    }

    private void EnablePlay(bool obj)
    {
        if (playMode != PlayMode.play)
            return;

       enablePlay = obj;
    }

    private void OnDestroy()
    {
        Actions.ResetAction -= ResetAction;
    }

    public void ResetAction()
    {
        //transform.position = _position;

        EnableAnimation(false);

    }


    public void EnableAnimation(bool enable)
    {
        if (enable)
            _rect.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.2f);
        else
            _rect.DOScale(Vector3.one, 0.2f);
    }

    void ChipSelectAction(bool select)
    {
        if (!select)
        {
           ResetAction();
            return;
        }

        EnableAnimation(true);
        // transform.position = new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z);

        Actions.BoardSelectAction();

        callback.ChipSelecion(_chipObject);
    }

    public  void Select()
    {
        if (playMode != PlayMode.play)
            return;

        if (!enablePlay)
            return;

        _selected = !_selected;

       ChipSelectAction(_selected);
    }
}
