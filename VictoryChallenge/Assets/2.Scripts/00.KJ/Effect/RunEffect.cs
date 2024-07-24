using UnityEngine;

namespace VictoryChallenge.KJ.Effect
{
    public class RunEffect : MonoBehaviour
    {
        public GameObject runEffectobj;

        void Update()
        {

            if (Input.GetKey(KeyCode.LeftShift))
            {
                runEffectobj.SetActive(true);
            }
            else
            {
                runEffectobj.SetActive(false);
            }
        }
    }
}
