using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ouroboros.Common.Utils
{
    [RequireComponent(typeof(Rigidbody))]
    public class RigidBodyDebugLine : MonoBehaviour
    {
        [SerializeField] private float widthMultiplier = 0.1f;
        [SerializeField] private float velocityScale = 1f;

        private LineRenderer line;
        private Rigidbody body;

        private void Awake()
        {
            body = GetComponent<Rigidbody>();

            line = this.GetOrAddComponent<LineRenderer>();
            line.useWorldSpace = true;
            line.widthMultiplier = widthMultiplier;
        }

        private void Update()
        {
            line.SetPosition(0, body.position);
            line.SetPosition(1, body.position + body.linearVelocity * velocityScale);
        }
    }
}