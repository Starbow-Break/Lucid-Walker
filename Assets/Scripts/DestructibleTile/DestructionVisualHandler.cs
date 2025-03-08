using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructionVisualHandler : MonoBehaviour
{
    // SET ON CREATION
    public Sprite tileSprite;
    public Vector2 forcePosition;
    public PhysicsMaterial2D physicsMaterial;
    // ADJUSTED IN INSPECTOR
    public float forceSpeed;

    // REFRENCES
    [SerializeField] private GameObject prefab;
    [SerializeField] private GameObject parentPrefab;
    [SerializeField] private List<Sprite> spriteMasks;

    private void Start()
    {
        BreakSpriteToMasks();
    }

    private void BreakSpriteToMasks()
    {
        List<GameObject> newObjects = new List<GameObject>();

        // GameObject parent = Instantiate(parentPrefab, transform.position, Quaternion.identity);
        GameObject parent = ObjectPooler.Instance.GetPooledObject(parentPrefab, transform.position, Quaternion.identity);

        for (int i = 0; i < spriteMasks.Count; i++)
        {
            // GameObject newGO = Instantiate(prefab, transform.position + new Vector3(0.5f, 0.5f, 0), Quaternion.identity);
            GameObject newGO = ObjectPooler.Instance.GetPooledObject(prefab, transform.position + new Vector3(0.5f, 0.5f, 0), Quaternion.identity);
            newObjects.Add(newGO);
        }

        SpriteUpdater.UpdateSpiteObject(parent, newObjects, spriteMasks, tileSprite, physicsMaterial);

        foreach (var rb in newObjects)
        {
            Vector3 forceDirection = transform.position - (Vector3)forcePosition;
            rb.GetComponent<Rigidbody2D>().velocity = forceDirection * forceSpeed;
        }
        // Destroy(gameObject);
        ObjectPooler.Instance.ReturnPooledObject(gameObject);
    }
}
