using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class DealerController : MonoBehaviour
{
   [SerializeField] GameObject _dealerObject;

    [Space]
    [SerializeField] Transform _start;


    float yPos = -1;

    private void Start()
    {
        Actions.DealerMoveAction += MoveTowards;

        yPos = _dealerObject.transform.position.y;

        //MoveTowards(_start);
    }

    public void ResetAction()
    {
        MoveTowards(_start);
    }

    public void MoveTowards(Transform t)
    {
        DOTween.KillAll();

        if(t == null) return;

        Vector3 newPos = new Vector3(t.position.x, yPos, t.position.z);

        _dealerObject.transform.DOMove(newPos, 1.0f).SetDelay(1.0f).OnComplete(()=> Invoke("ResetAction",1.0f));
    }

}
