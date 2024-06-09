using UnityEngine;

namespace VictoryChallenge.KJ.Name
{
    public class Billboard : MonoBehaviour
    {
        UnityEngine.Camera cam;     // Ä«¸Þ¶ó

        void Update()
        {
            if (cam == null)
                cam = FindObjectOfType<UnityEngine.Camera>();
            if (cam == null)
                return;

            transform.LookAt(cam.transform);
            transform.Rotate(Vector3.up * 180);
        }
    }

}