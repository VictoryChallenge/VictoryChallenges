using Photon.Pun;
using System.IO;
using System.Linq;
using Photon.Realtime;
using UnityEngine;

namespace VictoryChallenge.KJ.Manager
{
    public class PlayerManager : MonoBehaviour
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

        void CreateController()
        {
            Transform spawnPoint = Spawn.SpawnManager.Instance.GetSpawnPoint();
            controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), spawnPoint.position, spawnPoint.rotation, 0, new object[] { pv.ViewID });
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
