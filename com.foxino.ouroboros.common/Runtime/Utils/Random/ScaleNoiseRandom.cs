using UnityEngine;

[ExecuteInEditMode]
public class ScaleNoiseRandom : MonoBehaviour
{
    public float speed = 1f;
    public float noiseSpeed = 1f;
    public float strength = 1f;
    public bool playInEditMode = true;
    public Vector3 axes = Vector3.one;

    private Vector3 salt;
    private Vector3 baseScale;
    private float time;

    void Start()
    {
        salt = new Vector3(
            transform.position.x + Random.Range(0f, 10f),
            transform.position.y + Random.Range(0f, 10f),
            transform.position.z + Random.Range(0f, 10f));

        salt.Scale(axes);

        baseScale = transform.localScale;
    }

    void Update()
    {
        if (!Application.isPlaying
            && !playInEditMode)
        {
            return;
        }

        time += Time.deltaTime;

        var s = salt * time * noiseSpeed;

        var noise = new Vector3(
            Mathf.PerlinNoise(s.x, s.y),
            Mathf.PerlinNoise(s.y, s.z),
            Mathf.PerlinNoise(s.z, s.x));

        noise *= strength;

        noise += Vector3.one;
        noise.Scale(axes);

        var scale = baseScale;
        scale.Scale(noise);

        transform.localScale = scale;
	}
}
