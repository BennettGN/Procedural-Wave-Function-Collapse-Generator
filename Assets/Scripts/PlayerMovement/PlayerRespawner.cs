using System.Collections;
using UnityEngine;

namespace GodinhoNelsonBennett.Lab3
{
    public class Respawner : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float respawnDelay = 2f;
        [SerializeField] private float fallHeight = 3f;
        private Vector3 initialPos;
        private Quaternion initialRot;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            this.initialPos = target.position;
            this.initialRot = target.rotation;
        }

        public void Respawn(float offset)
        {
            if (offset == null) offset = 0;
            Debug.Log("Respawning player...");
            target.position = initialPos;
            target.position = new Vector3(target.position.x, target.position.y + offset, target.position.z);
            target.rotation = initialRot;
            resetRigidBody();
            Debug.Log("Player Respawned");
        }

        public void Update()
        {
            if (target.position.y < -10f)
            {
                StartCoroutine(RespawnAfterDelay(respawnDelay));
            }
        }

        private IEnumerator RespawnAfterDelay(float delay)
        {
            enabled = false;
            yield return new WaitForSeconds(delay);
            Respawn(fallHeight);
            enabled = true;
        }

        public void resetRigidBody()
        {
            Rigidbody rb = target.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }
}
