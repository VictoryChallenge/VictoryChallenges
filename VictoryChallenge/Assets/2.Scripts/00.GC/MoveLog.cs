using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MoveLog : MonoBehaviour
{
    public Ease ease;

    private void Start()
    {
        transform.DOMoveZ(172f, 10f).SetEase(ease).SetLoops(-1, LoopType.Yoyo);
        
        
    }
}
