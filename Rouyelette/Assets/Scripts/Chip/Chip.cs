using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    private void OnEnable()
    {
        numberText.text = _number.ToString();

        _position = transform.position;

        Actions.ResetAction += ResetAction;
    }

    private void OnDestroy()
    {
        Actions.ResetAction -= ResetAction;
    }

    public void ResetAction()
    {

        transform.position = _position;
    }


    void ChipSelectAction(bool select)
    {
        if (!select)
        {
           ResetAction();
            return;
        }

        transform.position = new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z);

        callback.ChipSelecion(this);
    }

    private void OnMouseDown()
    {   
       _selected = !_selected;

       ChipSelectAction(_selected);
    }
}
