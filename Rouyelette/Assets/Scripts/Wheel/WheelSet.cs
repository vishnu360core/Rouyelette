using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class WheelSet : MonoBehaviour
{
    [SerializeField] int _setNumber;


    private void OnEnable()
    {
        string rollString = gameObject.name.Replace(" ", "");
        MatchCollection matches = Regex.Matches(rollString, @"\d+");
        foreach (Match match in matches)
        {
            _setNumber += int.Parse(match.Value);
        }
    }
}
