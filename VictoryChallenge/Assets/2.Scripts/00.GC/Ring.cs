using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ring : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            TestMove testMove = other.GetComponent<TestMove>();
            if(testMove != null )
            {
                Destroy(gameObject);
                StartCoroutine(ReverseMove(testMove, 2f));
            }
        }
    }

    private IEnumerator ReverseMove(TestMove player, float duration)
    {
        player.Reverse(true);
        yield return new WaitForSeconds(duration);
        player.Reverse(false);
    }
}
