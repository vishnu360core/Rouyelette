using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(BoxCollider))]
public class Chip : MonoBehaviour
{
    [SerializeField] int _number;
    [SerializeField] TMP_Text numberText;

    Vector3 _position = Vector3.zero;
    bool _selected = false;


    private void OnEnable()
    {
        numberText.text = _number.ToString();

        _position = transform.position;

         

    }


    void ChipSelectAction(bool select)
    {
        if (!select)
        {
            transform.position = _position;
            return;
        }

        transform.position = new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z);
    }

    private void OnMouseDown()
    {   
       _selected = !_selected;

       ChipSelectAction(_selected);
    }
}
