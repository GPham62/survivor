using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WingsMob.Survival.Utils;

public class teststuff : MonoBehaviour
{
    [SerializeField] private Transform endPos;
    [SerializeField] private Transform startPos;
    [SerializeField] private Button button; 
    [SerializeField] private float heightFriction;
    [SerializeField] private float distanceFriction;
    // Start is called before the first frame update
    void Start()
    {
        int t = 5 / 2;
        Debug.Log(t);
        button.onClick.AddListener(() => {
            StartCoroutine(BezierCurveToTarget(startPos.position, endPos.position, 5, heightFriction));
        });
    }

    private IEnumerator BezierCurveToTarget(Vector2 startPos, Vector2 endPos, float speed, float heightFriction)
    {
        Vector2 bezierPos = CalculateBezierPoint(startPos, endPos, heightFriction);
        float elapsedTime = Mathf.Max(Vector2.Distance(endPos, startPos) / speed, 1f);
        while (elapsedTime > 0)
        {
            Vector3 i1 = Vector3.Lerp(endPos, bezierPos, elapsedTime);
            Vector3 i2 = Vector3.Lerp(bezierPos, startPos, elapsedTime);
            transform.position = Vector3.Lerp(i1, i2, elapsedTime);
            elapsedTime -= Time.deltaTime;
            yield return null;
        }
        //transform.position = endPos;
    }

    private Vector3 CalculateBezierPoint(Vector2 pos1, Vector2 pos2, float friction)
    {
        Vector2 middlePointPos = Utilities.GetPointWithFriction(pos1, pos2, distanceFriction);
        float distance = Vector2.Distance(pos1, pos2) / 2 * friction;
        Vector2 result = middlePointPos + (pos1.x > pos2.x ? 1 : -1) * (Vector2.Perpendicular(pos1 - pos2).normalized * distance);
        return result;
    }
}
