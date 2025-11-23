using UnityEngine;
using UnityEngine.InputSystem;

namespace GodinhoNelsonBennett.Lab3
{
    public class RespawnHandler : MonoBehaviour
    {
        [SerializeField] private Respawner respawner;
        [SerializeField] private GameObject playerToMove;
        private InputAction resetAction;

        public RespawnHandler(InputAction resetAction, Respawner respawner)
        {
            this.respawner = respawner;
            this.resetAction = resetAction;
            this.resetAction.performed += onSwitch;
            this.resetAction.Enable();

        }

        private void onSwitch(InputAction.CallbackContext context)
        {
            respawner.Respawn(0);
        }

        public Respawner GetRespawner()
        {
            return this.respawner;
        }
    }
}
