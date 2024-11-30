using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LineGenerator : MonoBehaviour
{
    [SerializeField] protected LineRenderer line; // Line Renderer
    
    void OnValidate() { Generate(); }
    void Start() { Generate(); }

    protected abstract void Generate(); // 라인 생성
}
