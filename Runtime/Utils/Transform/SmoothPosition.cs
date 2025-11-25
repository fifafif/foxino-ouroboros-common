using UnityEngine;

public class SmoothPosition : MonoBehaviour
{
    public int BufferSize = 10;
    public float Speed = 1f;

    private Vector3 lastPosition;
    private Vector3 velocity;

    private Vector3[] positions;
    private int currentIndex;

    private void Awake()
    {
        positions = new Vector3[BufferSize];
    }

    private void OnEnable()
    {
        transform.position = lastPosition = transform.parent.position;

        for (var i = 0; i < positions.Length; ++i)
        {
            positions[i] = lastPosition;
        }

        currentIndex = 0;
    }

    private void Update()
    {
        var p = transform.parent.position;
        var t = lastPosition;

        var ci = currentIndex;

        positions[currentIndex] = p;

        var avg = Vector3.zero;

        for (var i = 0; i < positions.Length; ++i, --ci)
        {
            if (ci < 0)
            {
                ci = positions.Length - 1;
            }

            avg += positions[ci];
        }

        avg /= BufferSize;

        ++currentIndex;
        if (currentIndex >= positions.Length)
        {
            currentIndex = 0;
        }

        var v = Vector3.SmoothDamp(t, avg, ref velocity, Speed);
        //var v = Vector3.Lerp(t, p, Speed * Time.deltaTime);

        //Debug.Log($"velocity: {velocity}");

        transform.position = lastPosition = v;
    }
}
