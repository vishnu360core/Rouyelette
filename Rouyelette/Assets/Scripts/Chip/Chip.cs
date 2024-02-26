using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Chip : MonoBehaviour
{
    [SerializeField] int _number;
    [SerializeField] TMP_Text numberText;

    bool _selected = false;
    private void OnEnable()
    {
        numberText.text = _number.ToString();
    }


    private void OnMouseDown()
    {   
       _selected = !_selected;

        if(!_selected)
          return;

        transform.position = new Vector3(transform.position.x, transform.position.y+2.0f,transform.position.z);
    }
}
