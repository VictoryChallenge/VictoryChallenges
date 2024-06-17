using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 원래 ithappy 이었으나 스크립트 똑같은거 있어서 일단 2붙임
namespace ithappy2
{
    public class Rnd_Animation : MonoBehaviour
    {

        Animator anim;
        float offsetAnim;

        [SerializeField] string titleAnim;


        // Start is called before the first frame update
        void Start()
        {
            anim = GetComponent<Animator>();
            offsetAnim = Random.Range(0f, 1f);


            anim.Play(titleAnim, 0, offsetAnim);
        }
    }
}
