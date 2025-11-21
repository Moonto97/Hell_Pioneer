using UnityEngine;

public class PlayerSight : MonoBehaviour
{
    [Range(0, 360)] public float Fov = 90f;
    public int RayCount = 90;
    public float ViewDistance = 10f;
    public LayerMask LayerMask;
    private Mesh _mesh;


    private void Start()
    {
        _mesh = new Mesh();
        _mesh.name = "FOV";
        GetComponent<MeshFilter>().mesh = _mesh;
    }


    private void Update()
    {
        DrawFOV();
    }

    private void DrawFOV()
    {
        float angle = Fov * 0.5f;
        float angleIncrease = Fov / RayCount;

        Vector3[] vertices = new Vector3[RayCount + 1 + 1];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[RayCount * 3];

        vertices[0] = Vector3.zero;

        int VertexIndex = 1;
        int TriangleIndex = 0;
        for (int i = 0; i <= RayCount; i++)
        {
            Vector3 vertex;

            RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.position, GetVectorFromAngle(transform.eulerAngles.z + angle), ViewDistance, LayerMask);
            if (raycastHit2D.collider == null)
            {
                vertex = GetVectorFromAngle(angle) * ViewDistance;
            }
            else
            {
                vertex = GetVectorFromAngle(angle) * (raycastHit2D.distance / ViewDistance) * ViewDistance;
            }
            vertices[VertexIndex] = vertex;

            if (0 < i)
            {
                triangles[TriangleIndex + 0] = 0;
                triangles[TriangleIndex + 1] = VertexIndex - 1;
                triangles[TriangleIndex + 2] = VertexIndex;

                TriangleIndex += 3;
            }

            ++VertexIndex;
            angle -= angleIncrease;
        }

        _mesh.vertices = vertices;
        _mesh.uv = uv;
        _mesh.triangles = triangles;

    }

    private Vector3 GetVectorFromAngle(float angle)
    {
        float angleRad = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }

}
