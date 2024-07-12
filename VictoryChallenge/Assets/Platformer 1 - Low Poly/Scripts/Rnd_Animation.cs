using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ithappy
{
    public class Rnd_Animation : MonoBehaviour
    {

        public Animator anim;
        float offsetAnim;

        [SerializeField] string titleAnim;


        // Start is called before the first frame update
        void Start()
        {
            anim = GetComponent<Animator>();
        }

        public void Active()
        {
            anim.SetBool("isActive", true);
        }
    }
}
