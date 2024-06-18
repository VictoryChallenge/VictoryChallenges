using Photon.Pun;
using System.IO;
using System.Linq;
using Photon.Realtime;
using UnityEngine;
using VictoryChallenge.Controllers.Player;
using VictoryChallenge.Camera;
using VictoryChallenge.KJ.Spawn;
using VictoryChallenge.Customize;

namespace VictoryChallenge.KJ.Manager
{
    public class PlayerManager : MonoBehaviourPun
    {
        PhotonView pv;

        GameObject controller;

        void Awake()
        {
            pv = GetComponent<PhotonView>();
        }

        void Start()
        {
            if (pv.IsMine)
            {
                CreateController();
            }
        }

        public void CreateController()
        {
            //if (controller == null)
            //{
            //    Transform spawnPoint = SpawnManager.Instance.GetSpawnPoint();
            //    controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerWithCam"), spawnPoint.position, spawnPoint.rotation, 0, new object[] { pv.ViewID });
                
            //}
            
            Transform spawnPoint = SpawnManager.Instance.GetSpawnPoint();
            if (spawnPoint == null)
            {
                return;
            }
            Debug.Log("스폰 포인트 " + spawnPoint);
            controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerWithCam"), spawnPoint.position, spawnPoint.rotation, 0, new object[] { pv.ViewID });
            controller.GetComponentInChildren<PlayerCharacterCustomized>().Load();

            controller.GetComponentInChildren<PlayerController>().GetComponent<PhotonView>().RPC("CustomDataRPC", RpcTarget.AllBuffered);
        }

        public void Die()
        {
            PhotonNetwork.Destroy(controller);
            CreateController();
        }

        public static PlayerManager Find(Player player)
        {
            return FindObjectsOfType<PlayerManager>().SingleOrDefault(x => x.pv.Owner == player);
        }
    }
}
