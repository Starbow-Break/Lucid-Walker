using UnityEngine;

public class BulletSpawnerTargetArea : MonoBehaviour
{
    [System.Serializable]
    public enum Coordinate {
        Local, 
        World
    }

    [SerializeField] Coordinate coordinate;
    [SerializeField] private float left;
    [SerializeField] private float right;
    [SerializeField] private float up;
    [SerializeField] private float down;

    private Vector3 min => Mathf.Min(-left, right) * Vector3.right
                + Mathf.Min(-down, up) * Vector3.up;
    private Vector3 max => Mathf.Max(-left, right) * Vector3.right
                + Mathf.Max(-down, up) * Vector3.up;

    public Vector3 GetRandomPosition() {
        float x = Random.Range(min.x, max.x);
        float y = Random.Range(min.y, max.y);
        Vector3 result = new(x, y, 0f);

        if(coordinate == Coordinate.Local)
        {
            Vector4 temp = new(result.x, result.y, result.z, 1f);
            Matrix4x4 matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            temp = matrix * temp;
            result = new(temp.x, temp.y, temp.z);
            result /= temp.w;
        }
        else
        {
            result += transform.position;
        }

        return result;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;

        if(coordinate == Coordinate.Local)
        {
            Vector3 areaCenter = (min + max) / 2;
            Vector3 areaSize = max - min;
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            Gizmos.DrawWireCube(areaCenter, areaSize);
        }
        else
        {
            Vector3 areaCenter = (min + max) / 2;
            Vector3 areaSize = max - min;
            Gizmos.DrawWireCube(transform.position + areaCenter, areaSize);
        }
    }
}
