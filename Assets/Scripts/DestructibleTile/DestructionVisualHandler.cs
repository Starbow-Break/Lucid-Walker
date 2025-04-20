using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.ShaderGraph.Internal;
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
    [SerializeField] private float parentDuration = 5.0f;

    public List<Sprite> SpriteMasks { get => spriteMasks; set => spriteMasks = value; }

    private GameObject spawnedParent = null;

    private bool isWork = false;

    private void OnEnable()
    {
        isWork = false;
    }

    private void Update()
    {
        if(!isWork)
        {
            isWork = true;
            BreakSpriteToMasks();
        }
    }

    private void OnDisable()
    {
        if(spawnedParent != null)
        {
            ObjectPooler.Instance.ReturnPooledObject(spawnedParent);
            spawnedParent = null;
        }
    }

    private void BreakSpriteToMasks()
    {
        List<GameObject> newObjects = new List<GameObject>();

        // GameObject parent = Instantiate(parentPrefab, transform.position, Quaternion.identity);
        spawnedParent = ObjectPooler.Instance.GetPooledObject(parentPrefab, transform.position, Quaternion.identity);

        for (int i = 0; i < spriteMasks.Count; i++)
        {
            // GameObject newGO = Instantiate(prefab, transform.position + new Vector3(0.5f, 0.5f, 0), Quaternion.identity);
            float angle = Random.Range(0f, 360f);
            Quaternion rotation = Quaternion.Euler(0f, 0f, angle);
            GameObject newGO = ObjectPooler.Instance.GetPooledObject(prefab, transform.position + new Vector3(0.5f, 0.5f, 0), rotation);
            newObjects.Add(newGO);
        }

        SpriteUpdater.UpdateSpiteObject(spawnedParent, newObjects, spriteMasks, tileSprite, physicsMaterial);

        foreach (var rb in newObjects)
        {
            Vector3 forceDirection = transform.position - (Vector3)forcePosition;
            rb.GetComponent<Rigidbody2D>().velocity = forceDirection * forceSpeed;
        }
        // Destroy(gameObject);
        StartCoroutine(ReturnSequence());
    }

    private IEnumerator ReturnSequence()
    {
        yield return new WaitForSeconds(parentDuration);
        ObjectPooler.Instance.ReturnPooledObject(gameObject);
    }
}
