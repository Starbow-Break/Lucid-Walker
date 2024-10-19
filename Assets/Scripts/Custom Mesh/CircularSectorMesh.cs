using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class CircularSectorMesh : MonoBehaviour
{
    [Range(0.0f, 360.0f)]
    [SerializeField] float angle;
    [SerializeField] float radius;
    [SerializeField] int pieces;

    Mesh mesh;
    Vector3[] vertices;
    Vector2[] uv;
    int[] triangles;

    // check properties are valid
    // if valid draw mesh
    void OnValidate() {
        if(mesh == null) return;

        if(radius > 0 && pieces > 0) {
            Generate();
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        Generate();
    }

    //----------------------- Setter Function -----------------------
    public void SetRadius(float newRadius)
    {
        if(newRadius > 0)
        {
            radius = newRadius;
        }
    }

    public void SetAngle(float newAngle)
    {
        if(newAngle >= 0)
        {
            angle = newAngle;
        }
    }
    //----------------------- Setter Function -----------------------

    // Generate Mesh
    public void Generate()
    {
        SetMeshData(radius, pieces);
        createMesh();
    }

    // set mesh data
    void SetMeshData(float radius, int pieces)
    {
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        vertices = new Vector3[pieces + 2]; // vertex array
        uv = new Vector2[vertices.Length];
        triangles = new int[3 * pieces]; // index array

        vertices[0] = Vector3.zero;
        for(int i = 0; i <= pieces; i++) {
            vertices[i + 1] = new(
                -Mathf.Sin(Mathf.Deg2Rad * (angle / pieces * i - angle / 2)), 
                Mathf.Cos(Mathf.Deg2Rad * (angle / pieces * i - angle / 2))
            );

            vertices[i + 1] *= radius;
        }

        for(int i = 0; i < pieces; i++) {
            triangles[3 * i] = 0;
            triangles[3 * i + 1] = i + 2;
            triangles[3 * i + 2] =  i + 1;
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }

    // create mesh
    void createMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        
        Destroy(GetComponent<PolygonCollider2D>());
        PolygonCollider2D collider = gameObject.AddComponent<PolygonCollider2D>();

        collider.points = new Vector2[vertices.Length];
        List<Vector2> path = new List<Vector2>();

        for(int i = 0; i < vertices.Length; i++) {
            collider.points[i] = new(vertices[i].x, vertices[i].y);
            path.Add(new(vertices[i].x, vertices[i].y));
        }
        collider.SetPath(0, path);
        collider.isTrigger = true;
    }
}
