using UnityEngine;

[ExecuteInEditMode]
public class RotateNoiseRandom : MonoBehaviour
{
    public float speed = 1f;
    public float noiseSpeed = 1f;
    public float scaleMin = 1f;
    public float scaleMax = 2f;
    public bool limitByScale = true;
    public bool playInEditMode = true;
    public Vector3 axes = Vector3.one;

    private Vector3 salt;

    void Start()
    {
        salt = new Vector3(
            transform.position.x + Random.Range(0f, 10f),
            transform.position.y + Random.Range(0f, 10f),
            transform.position.z + Random.Range(0f, 10f));

        salt.Scale(axes);
    }

    void Update()
    {
        if (!Application.isPlaying
            && !playInEditMode)
        {
            return;
        }

        var s = salt * Time.realtimeSinceStartup * noiseSpeed;
        float rotSpeed;

        if (limitByScale)
        {
            float scale = transform.lossyScale.x;
            rotSpeed = Mathf.Lerp(speed, 0f, Mathf.InverseLerp(scaleMin, scaleMax, scale));
        }
        else
        {
            rotSpeed = speed;
        }
        
        var rot = new Vector3(
            Mathf.PerlinNoise(s.x, s.y) - 0.5f,
            Mathf.PerlinNoise(s.y, s.z) - 0.5f,
            Mathf.PerlinNoise(s.z, s.x) - 0.5f);

        rot.Scale(axes);

        transform.Rotate(rot * rotSpeed * Time.deltaTime);
	}
}
