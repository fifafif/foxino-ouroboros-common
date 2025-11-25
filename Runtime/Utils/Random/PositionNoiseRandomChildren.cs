using UnityEngine;

//[ExecuteInEditMode]
public class PositionNoiseRandomChildren : MonoBehaviour
{
    public float speed = 1f;
    public float frequency = 1f;
    public float strength = 1f;
    public Vector3 axes = Vector3.one;
    
    private Transform[] children;
    private Vector3[] salt;
    private Vector3[] origPos;
    private float time;

    void Start()
    {
        children = new Transform[transform.childCount];
        salt = new Vector3[transform.childCount];
        origPos = new Vector3[transform.childCount];

        for (int i = 0; i < transform.childCount; ++i)
        {
            children[i] = transform.GetChild(i);
            origPos[i] = children[i].localPosition;
            salt[i] = new Vector3(
            transform.position.x + Random.Range(0f, 10f),
            transform.position.y + Random.Range(0f, 10f),
            transform.position.z + Random.Range(0f, 10f));

            salt[i].Scale(axes);
        }
    }

    void Update()
    {
        time += Time.deltaTime * speed;

        for (int i = 0; i < children.Length; ++i)
        {
            var s = salt[i] * time * frequency;

            var noise = new Vector3(
                Mathf.PerlinNoise(s.x, s.y) * 2 - 1f,
                Mathf.PerlinNoise(s.y, s.z) * 2 - 1f,
                Mathf.PerlinNoise(s.z, s.x) * 2 - 1f);

            noise *= strength;
            noise.Scale(axes);

            children[i].localPosition = origPos[i] + noise;
        }
	}
}
