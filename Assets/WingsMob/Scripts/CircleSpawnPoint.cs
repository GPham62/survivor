using System.Collections.Generic;
using UnityEngine;
using WingsMob.Survival.Utils;

public class CircleSpawnPoint : MonoBehaviour
{
    public Transform RadiusPoint;

    public List<Vector3> GeneratePositions(int amount, float maxAngle)
    {
        float radius = Vector2.Distance(transform.position, RadiusPoint.position);

        float sign = transform.up.x > 0f ? 1f : -1f;
        float offsetAngle = Vector2.Angle(Vector2.up, transform.up) * sign;
        float angle = -(float)maxAngle / 2 + offsetAngle; // start angle

        float angleDelta = (float)maxAngle / (amount - 1);

        Vector2 offsetPosition = transform.position;

        List<Vector3> positions = new List<Vector3>();
        while (amount > 0)
        {
            positions.Add(Utilities.CircleXY(radius, angle) + offsetPosition);
            angle += angleDelta;

            amount--;
        }

        return positions;
    }
}
