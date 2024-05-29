using UnityEngine;

namespace VictoryChallenge.ComponentExtensions
{
    public static class GameComponetExtensions
    {
        public static bool IsGrounded(this Component component)
        {
            return Physics.OverlapSphere(component.transform.position,
                                        0.15f,
                                        1 << LayerMask.NameToLayer("Ground"))
                                        .Length > 0;
        }
    }
}
