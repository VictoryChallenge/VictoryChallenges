using Photon.Pun;
using System.IO;
using System.Linq;
using Photon.Realtime;
using UnityEngine;
using VictoryChallenge.Controllers.Player;
using VictoryChallenge.Camera;

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
            if (controller == null)
            {
                Transform spawnPoint = Spawn.SpawnManager.Instance.GetSpawnPoint();
                controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerWithCam"), spawnPoint.position, spawnPoint.rotation, 0, new object[] { pv.ViewID });
                controller.transform.SetParent(this.transform);
                DontDestroyOnLoad(controller);

                PhotonView controllerPv = controller.GetComponent<PhotonView>();
                if (controllerPv != null && controllerPv.IsMine)
                {
                    EnablePlayerControllers(controller);
                }
                else
                {
                    DisablePlayerControllers(controller);
                }
            }
        }

        private void EnablePlayerControllers(GameObject player)
        {
            var playerController = player.GetComponent<PlayerController>();
            player.GetComponent<PlayerController>().enabled = true;
            player.GetComponent<CameraController>().enabled = true;
            player.GetComponent<TrackFollow>().enabled = true;
        }

        private void DisablePlayerControllers(GameObject player)
        {
            player.GetComponent<PlayerController>().enabled = false;
            player.GetComponent<CameraController>().enabled = false;
            player.GetComponent<TrackFollow>().enabled = false;
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
