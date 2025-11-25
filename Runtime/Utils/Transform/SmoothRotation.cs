using UnityEngine;

public class SmoothRotation : MonoBehaviour
{
    public float Speed = 1f;

    private Quaternion lastRotation;
    private float velocity;
    private Transform parent;

    private void Awake()
    {
        lastRotation = transform.rotation;
        parent = transform.parent;
    }

    private void OnEnable()
    {
        if (parent == null) return;

        transform.rotation = lastRotation = parent.rotation;
        velocity = 0f;
    }

    private void Update()
    {
        if (parent == null) return;

        var p = parent.rotation.eulerAngles;
        var t = lastRotation.eulerAngles;

        var x = Mathf.SmoothDampAngle(t.x, p.x, ref velocity, Speed);
        var y = Mathf.SmoothDampAngle(t.y, p.y, ref velocity, Speed);
        var z = Mathf.SmoothDampAngle(t.z, p.z, ref velocity, Speed);

        transform.rotation = lastRotation = Quaternion.Euler(x, y, z).normalized;
    }
}
