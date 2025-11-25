using UnityEngine;
using Ouroboros.Common.Utils.Transforms;

[ExecuteInEditMode]
public class RandomRotateArraySeqments : MonoBehaviour
{
    public ArraySegmentTransform[] Segments;

    public float speed = 1f;
    public float frequency = 1f;
    public float strength = 1f;
    public Vector3 axes = Vector3.one;
    public bool CenterPosition;

    public bool SaveOrigsNow;
    public bool UpdateInEditMode;

    private Transform[] children;
    private Vector3[] salt;
    private Vector3[] origPos;
    private Quaternion[] origRot;
    private float time;

    void Start()
    {
        SaveOrigs();
    }

    private void SaveOrigs()
    {
        Segments = GetComponentsInChildren<ArraySegmentTransform>();

        children = new Transform[transform.childCount];
        salt = new Vector3[transform.childCount];
        origPos = new Vector3[transform.childCount];
        origRot = new Quaternion[transform.childCount];

        for (int i = 0; i < transform.childCount; ++i)
        {
            children[i] = transform.GetChild(i);
            origPos[i] = children[i].localPosition;
            origRot[i] = children[i].localRotation;
            salt[i] = new Vector3(
            transform.position.x + Random.Range(0f, 10f),
            transform.position.y + Random.Range(0f, 10f),
            transform.position.z + Random.Range(0f, 10f));

            salt[i].Scale(axes);
        }
    }

    void Update()
    {
        if (SaveOrigsNow)
        {
            SaveOrigsNow = false;

            SaveOrigs();
        }

        if (!Application.isPlaying
            && !UpdateInEditMode)
        {
            return;
        }


        Vector3 posCenter = Vector3.zero;

        if (CenterPosition)
        {
            posCenter = (Segments[0].transform.localPosition - Segments[Segments.Length - 1].transform.localPosition) * 0.5f;

            Segments[0].transform.localPosition = posCenter;
        }

        time += Time.deltaTime * speed;

        for (int i = 0; i < Segments.Length; ++i)
        {
            var s = salt[i] * time * frequency;

            var noise = new Vector3(
                Mathf.PerlinNoise(s.x, s.y) * 2 - 1f,
                Mathf.PerlinNoise(s.y, s.z) * 2 - 1f,
                Mathf.PerlinNoise(s.z, s.x) * 2 - 1f);

            noise *= strength;
            noise.Scale(axes);

            if (i > 0)
            {
                Segments[i].transform.MatchOther(Segments[i - 1].EndPoint);
            }

            var rot = Quaternion.Euler(noise);

            Segments[i].transform.localRotation = rot * origRot[i];
        }
    }
}
