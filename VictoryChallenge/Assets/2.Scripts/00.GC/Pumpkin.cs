using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VictoryChallenge.Controllers.Player;

public class Pumpkin : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            PlayerController pc = other.gameObject.GetComponent<PlayerController>();
            if (pc != null)
            {
                GetComponent<SkinnedMeshRenderer>().enabled = false;
                GetComponent<SphereCollider>().enabled = false;
                StartCoroutine(SpeedUp(pc, 10f));
                Destroy(gameObject, 2.5f);
            }
        }
    }

    private IEnumerator SpeedUp(PlayerController player, float duration)
    {
        player.speedGain += 4;
        yield return new WaitForSeconds(duration);
        player.speedGain -= 4;
    }
}
