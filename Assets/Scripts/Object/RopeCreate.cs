using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeCreate : MonoBehaviour
{
    public GameObject ropePrefab;
    public int ropeCnt = 30; // 기본 로프 개수
    public Rigidbody2D pointRig;
    private List<GameObject> ropeSegments = new List<GameObject>(); // 로프 오브젝트들 저장

    public void CreateRope()
    {
        ClearRope(); // 기존 로프 삭제

        for (int i = 0; i < ropeCnt; i++)
        {
            GameObject ropeSegment = Instantiate(ropePrefab, transform);
            FixedJoint2D currentJoint = ropeSegment.GetComponent<FixedJoint2D>();
            Rigidbody2D ropeRb = ropeSegment.GetComponent<Rigidbody2D>();

            // 로프 위치 설정
            ropeSegment.transform.localPosition = new Vector3(0, (i + 1) * -1f, 0);

            // 로프 조각 연결
            if (i == 0)
                currentJoint.connectedBody = pointRig;
            else
                currentJoint.connectedBody = ropeSegments[i - 1].GetComponent<Rigidbody2D>();

            if (i == ropeCnt - 1)
            {
                ropeRb.mass = 10;
                ropeSegment.GetComponent<SpriteRenderer>().enabled = false;
            }

            ropeSegments.Add(ropeSegment);
        }
    }

    public void ClearRope()
    {
        foreach (GameObject rope in ropeSegments)
        {
            Destroy(rope);
        }
        ropeSegments.Clear();
    }
}
