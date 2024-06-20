using Photon.Pun;
using UnityEngine;

namespace VictoryChallenge.KJ.Photon
{
    public class ObstacleManager : MonoBehaviourPun
    {
        private GameObject[] _obstacles;

        void Start()
        {
            int obstacleLayer = LayerMask.NameToLayer("Obstacle");
            _obstacles = FindObjectsOfType<GameObject>();

            _obstacles = System.Array.FindAll(_obstacles, go => go.layer == obstacleLayer);

            if (PhotonNetwork.IsMasterClient)
            {
                SyncObstacleState();
            }
        }

        [PunRPC]
        void SyncObstacleState()
        {
            foreach (GameObject obstacle in _obstacles)
            {
                PhotonView photonView = obstacle.GetComponent<PhotonView>();

                if (photonView != null && photonView.IsMine)
                {
                    Animator animator = obstacle.GetComponent<Animator>();

                    if (animator != null)
                    {
                        foreach (AnimatorControllerParameter parameter in animator.parameters)
                        {
                            if (parameter.type == AnimatorControllerParameterType.Bool)
                            {
                                bool value = animator.GetBool(parameter.nameHash);
                                photonView.RPC("SetAnimationBool", RpcTarget.AllBuffered, parameter.nameHash, value);
                            }
                        }
                    }
                }
            }
        }
    }

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
