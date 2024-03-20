using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Video;
using System;

public interface ChipInterface
{
    public void ChipSelecion(Chip chip);    
}

[RequireComponent(typeof(BoxCollider))]
public class Chip : MonoBehaviour
{
   
    [SerializeField] int _number;
    public int Bet=>_number;

    [SerializeField] TMP_Text numberText; 

    Vector3 _position = Vector3.zero;

    bool _selected = false;

    public ChipInterface callback;

    [SerializeField] Outline _outline;

    bool enablePlay = false;


    private void OnEnable()
    {
        numberText.text = _number.ToString();

        _position = transform.position;

        Actions.ResetAction += ResetAction;

        Actions.EnablePlay += EnablePlay;
    }

    private void EnablePlay(bool obj)
    {
       enablePlay = obj;
    }

    private void OnDestroy()
    {
        Actions.ResetAction -= ResetAction;
    }

    public void ResetAction()
    {
        transform.position = _position;

        EnableAnimation(false);
    }


    public void EnableAnimation(bool enable)
    {
       _outline.enabled = enable;   
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
        callback.ChipSelecion(this);
    }

    private void OnMouseDown()
    {
        if (!enablePlay)
            return;

       _selected = !_selected;

       ChipSelectAction(_selected);
    }
}
