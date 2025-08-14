using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public static class Utility
{
    #region List Extensions

    public static void Shuffle<T>(this IList<T> list)
    {
        if (list == null || list.Count == 0) return;

        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }

    #endregion


    #region Component Extensions

    public static void SetActive(this Component self, bool active)
    {
        if (self == null) return;

        self.gameObject.SetActive(active);
    }

    public static bool IsActive(this Component self)
    {
        if (self == null) return false;

        return self.gameObject.activeInHierarchy;
    }


    public static Vector3 GetPosition(this GameObject self)
    {
        if (self == null) return Vector3.zero;

        return self.transform.position;
    }

    public static void SetPosition(this GameObject self, Vector3 pos)
    {
        if (self == null) return;
        self.transform.position = pos;
    }

    public static void LookTarget(this Transform self, Transform target, float lookSpeed)
    {
        if (target == null) return;

        Vector3 direction = target.position - self.position;
        direction.y = 0;
        direction.Normalize();

        Quaternion lookRotation = Quaternion.LookRotation(direction);

        if (Quaternion.Angle(self.rotation, lookRotation) >= 1f)
            self.rotation = Quaternion.Slerp(self.rotation, lookRotation, Time.deltaTime * lookSpeed);
    }

    public static bool IsTargetInDistance(this Transform self, Transform target, float distance)
        => Vector3.Distance(self.position, target.position) < distance;

    // 적으로부터 도망가는 위치를 계산하는 메서드
    public static Vector3 GetFleePositionFrom(this Transform self, Transform enemy, float minDistance, float maxDistance)
    {
        Vector3 fleeDirection = (self.position - enemy.position).normalized;
        float fleeDistance = Random.Range(minDistance, maxDistance);
        Vector3 fleePosition = self.position + fleeDirection * fleeDistance;

        if (NavMesh.SamplePosition(fleePosition, out NavMeshHit hit, maxDistance, NavMesh.AllAreas))
            return hit.position;
        else
        {
            int i = 0;
            while (i < 10) // 최대 10번 시도
            {
                fleeDistance = Random.Range(minDistance, maxDistance);
                fleePosition = self.position + fleeDirection * fleeDistance;

                if (NavMesh.SamplePosition(fleePosition, out hit, maxDistance, NavMesh.AllAreas))
                    return hit.position;

                i++;
            }
        }

        return self.position;
    }

    public static Vector3 ClampDistanceFrom(this Vector3 self, Vector3 from, float limitDistance)
        => Vector3.ClampMagnitude(self - from, limitDistance) + from;



    #endregion
}
