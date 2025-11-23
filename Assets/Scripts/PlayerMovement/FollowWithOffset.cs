using UnityEngine;

namespace GodinhoNelsonBennett.Lab3
{
    public class FollowWithOffset : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset;
        private void Update()
        {
            transform.position = target.position + offset;
        }
    }
}

