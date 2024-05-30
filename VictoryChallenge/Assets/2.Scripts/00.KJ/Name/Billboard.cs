using UnityEngine;

namespace VictoryChallenge.KJ.Name
{
    public class Billboard : MonoBehaviour
    {
        Camera cam;     // Ä«¸Þ¶ó

        void Update()
        {
            if (cam == null)
                cam = FindObjectOfType<Camera>();
            if (cam == null)
                return;

            transform.LookAt(cam.transform);
            transform.Rotate(Vector3.up * 180);
        }
    }

}