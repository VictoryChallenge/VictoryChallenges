using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VictoryChallenge.Controllers.Player;

public class Ring : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            PlayerController pc = other.gameObject.GetComponent<PlayerController>();
            if(pc != null )
            {
                GetComponent<MeshRenderer>().enabled = false;
                GetComponent<MeshCollider>().enabled = false;
                StartCoroutine(ReverseMove(pc, 2f));
                Destroy(gameObject, 2.5f);
            }
        }
    }

    private IEnumerator ReverseMove(PlayerController player, float duration)
    {
        player.isReverseKey = true;
        yield return new WaitForSeconds(duration);
        player.isReverseKey = false;
    }
}
