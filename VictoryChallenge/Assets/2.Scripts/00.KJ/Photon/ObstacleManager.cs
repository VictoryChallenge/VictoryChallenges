using Photon.Pun;
using UnityEngine;

namespace VictoryChallenge.KJ.Photon
{ 

    public class Obstcle : MonoBehaviourPun
    {
        private Animator _animator;

        void Start()
        {
            _animator = GetComponent<Animator>();
        }

        [PunRPC]
        public void SetAnimationBool(int parameterHash, bool value)
        {
            _animator.SetBool(parameterHash, value);
        }
    }
}
