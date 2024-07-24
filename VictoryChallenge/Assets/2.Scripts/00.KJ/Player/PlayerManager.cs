using Photon.Pun;
using System.IO;
using System.Linq;
using Photon.Realtime;
using UnityEngine;
using VictoryChallenge.KJ.Spawn;
using VictoryChallenge.KJ.Auth;
using VictoryChallenge.KJ.Database;
using CharacterController = VictoryChallenge.Controllers.Player.CharacterController;
using UnityEngine.SceneManagement;
using VictoryChallenge.Scripts.HS;
using VictoryChallenge.KJ.Rank;

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
            /*FirebaseAuth.DefaultInstance.CurrentUser.UserId;*/
            string userId = UIDHelper.GenerateShortUID(RestAPIAuth.Instance.UserId);
            //Debug.Log("userId : " + userId);
            controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerWithCam"), /*spawnPoint.position, spawnPoint.rotation,*/transform.position, transform.rotation, 0, new object[] { userId });
            controller.GetComponentInChildren<CharacterController>().shortUID = userId;
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
