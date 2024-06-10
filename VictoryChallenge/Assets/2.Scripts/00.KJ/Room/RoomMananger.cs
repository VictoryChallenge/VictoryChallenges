using Photon.Pun;
using UnityEngine.SceneManagement;
using VictoryChallenge.KJ.Photon;

namespace VictoryChallenge.KJ.Room
{
    public class RoomMananger : MonoBehaviourPunCallbacks
    {
        public static RoomMananger Instance;        // ΩÃ±€≈Ê

        void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
            Instance = this;
        }

        public override void OnEnable()
        {
            base.OnEnable();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (scene.buildIndex == 1 && PhotonNetwork.InRoom)
            {
                PhotonSub.Instance.AssignButtonAndText();

                if (PhotonManager.Instance != null)
                {
                    PhotonSub.Instance.OnSceneLoadedForAllPlayers();
                }
            }
        }
    }
}
