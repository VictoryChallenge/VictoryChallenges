using UnityEngine;

namespace VictoryChallenge.Scripts.CL
{
    public class Managers : MonoBehaviour
    {
        static Managers s_Instance;
        public static Managers Instance { get { Init(); return s_Instance; } }

        ResourceManager _resource = new ResourceManager();
        UIManager _ui = new UIManager();
        SoundManager _sound = new SoundManager();

        public static ResourceManager Resource { get { return Instance._resource; } }
        public static UIManager UI { get { return Instance._ui; } }
        public static SoundManager Sound { get { return Instance._sound; } }

        void Start()
        {
            Init();
        }

        void Update()
        {

        }

        static void Init()
        {
            if (s_Instance == null)
            {
                GameObject go = GameObject.Find("Managers");
                if (go == null)
                {
                    go = new GameObject { name = "Managers" };
                    go.AddComponent<Managers>();
                }
                DontDestroyOnLoad(go);
                s_Instance = go.GetComponent<Managers>();

                s_Instance._sound.Init();   // soundmanager¿« init»£√‚
            }
        }
    }
}