using UnityEngine;

namespace Ouroboros.Common.Utils
{
    public class TransformDebug : MonoBehaviour
    {
        [SerializeField] private Color color = Color.red;
        [SerializeField] private float size = 1f;

        private void OnDrawGizmos()
        {
            var origColor = Gizmos.color;
            var originalMatrix = Gizmos.matrix;

            Gizmos.color = color;
            Gizmos.matrix = transform.localToWorldMatrix;

            Gizmos.DrawWireCube(Vector3.zero, new Vector3(size, size, size));

            Gizmos.matrix = originalMatrix;
            Gizmos.color = origColor;
        }
    }
}