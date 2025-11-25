using UnityEngine;

[ExecuteInEditMode]
public class PositionNoiseRandom : MonoBehaviour
{
    public float speed = 1f;
    public float frequency = 1f;
    public float strength = 1f;
    public bool playInEditMode = true;
    public Vector3 axes = Vector3.one;
    public Vector3 orig;

    public bool SetOrigNow;

    private Vector3 salt;
    private float time;

    void Start()
    {
        salt = new Vector3(
            transform.position.x + Random.Range(0f, 10f),
            transform.position.y + Random.Range(0f, 10f),
            transform.position.z + Random.Range(0f, 10f));

        salt.Scale(axes);

        orig = transform.localPosition;
    }

    void Update()
    {
        if (SetOrigNow)
        {
            SetOrigNow = false;

            orig = transform.localPosition;
        }

        if (!Application.isPlaying
            && !playInEditMode)
        {
            return;
        }

        time += Time.deltaTime * speed;

        var s = salt * time * frequency;

        var noise = new Vector3(
            Mathf.PerlinNoise(s.x, s.y) * 2 - 1f,
            Mathf.PerlinNoise(s.y, s.z) * 2 - 1f,
            Mathf.PerlinNoise(s.z, s.x) * 2 - 1f);

        noise *= strength;
        noise.Scale(axes);

        transform.localPosition = orig + noise;
	}
}
