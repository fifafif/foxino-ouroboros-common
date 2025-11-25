using System;
using UnityEngine;

[ExecuteInEditMode]
public class PlaceOnMesh : MonoBehaviour
{
    public GameObject[] target = new GameObject[0];
    public GameObject parent;
    public MeshFilter meshFilter;

    public Vector3 rotationOffset;
    public Vector3 randomPosition;
    public Vector3 randomRotation;
    public float size = 1f;

    public float normalOffset = 0f;

    public int count;

    public bool PlaceNow;

	void Update ()
    {
		if (PlaceNow)
        {
            PlaceNow = false;

            Place();
        }
	}

    private void Place()
    {
        for (int i = parent.transform.childCount - 1; i >= 0; --i)
        {
            DestroyImmediate(parent.transform.GetChild(i).gameObject);
        }

        var mesh = meshFilter.sharedMesh;

        Debug.Log("vertices count: " + mesh.vertices.Length + " - " + mesh.vertexCount);

        float totalWeight = 0f;
        float[] triangleWeight = new float[mesh.triangles.Length];

        for (int i = 0; i < mesh.triangles.Length; i += 3)
        {
            float w = ShapeMath.TriangleSurface(mesh.vertices[mesh.triangles[i]], mesh.vertices[mesh.triangles[i + 1]], mesh.vertices[mesh.triangles[i + 2]]);
            triangleWeight[i / 3] = w;
            totalWeight += w;
        }

        int createdCount = 0;

        for (int i = 0; i < mesh.triangles.Length && createdCount < count; i += 3)
        {
            float proportionalWeight = count * triangleWeight[i / 3] / totalWeight;
            float countPerTriangle = (float)Math.Truncate(proportionalWeight);
            float frac = proportionalWeight - countPerTriangle;

            if (UnityEngine.Random.Range(0f, 1f) <= frac)
            {
                countPerTriangle += 1f;
            }

            for (int o = 0; o < countPerTriangle && createdCount < count; ++o)
            {
                var rndInside = Rnd.InsideTriangle(mesh.vertices[mesh.triangles[i]], mesh.vertices[mesh.triangles[i + 1]], mesh.vertices[mesh.triangles[i + 2]]);
                var normal = mesh.normals[mesh.triangles[i]] + mesh.normals[mesh.triangles[i + 1]] + mesh.normals[mesh.triangles[i + 2]];
                normal *= 0.33333f;

                normal = meshFilter.transform.rotation * normal;

                var go = GameObject.Instantiate(target[UnityEngine.Random.Range(0, target.Length)]);

                Vector3 pos = rndInside;

                pos += normal * normalOffset;
                pos += Rnd.RandomVector3(randomPosition);

                pos = meshFilter.transform.rotation * pos;

                go.transform.parent = parent.transform;
                go.transform.localScale = Vector3.one * size;
                go.transform.position = pos * meshFilter.transform.lossyScale.x + meshFilter.transform.position;

                Quaternion rot = Quaternion.LookRotation(normal);

                rot *= Quaternion.Euler(rotationOffset);

                var randRot = Quaternion.AngleAxis(UnityEngine.Random.Range(-randomRotation.x, randomRotation.x), normal);

                //var randRot = new Vector3(UnityEngine.Random.Range(-randomRotation.x, randomRotation.x), UnityEngine.Random.Range(-randomRotation.y, randomRotation.y), UnityEngine.Random.Range(-randomRotation.z, randomRotation.z));

                go.transform.rotation = randRot * rot;// Quaternion.Euler(randRot);

                ++createdCount;
            }
        }

        Debug.Log("Created! Count: " + createdCount);
    }
}
