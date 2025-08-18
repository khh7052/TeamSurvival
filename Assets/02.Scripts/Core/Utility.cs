using System.Collections.Generic;
using System.Linq;
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

    private const int MaxAttempts = 10;
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

    private static Vector3 TryGetNavMeshPosition(Vector3 startPos, System.Func<Vector3> getTargetPosition, float maxDistance)
    {
        for (int i = 0; i < MaxAttempts; i++)
        {
            Vector3 targetPos = getTargetPosition();
            if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, maxDistance, NavMesh.AllAreas))
                return hit.position;
        }
        return startPos;
    }

    public static Vector3 GetFleePositionFrom(this Transform self, Transform enemy, float minDistance, float maxDistance)
    {
        return TryGetNavMeshPosition(self.position, () =>
        {
            Vector3 fleeDir = (self.position - enemy.position).normalized;
            float fleeDist = Random.Range(minDistance, maxDistance);
            return self.position + fleeDir * fleeDist;
        }, maxDistance);
    }

    public static Vector3 GetRandomPosition(this Transform self, float minDistance, float maxDistance)
    {
        return TryGetNavMeshPosition(self.position, () =>
        {
            float dist = Random.Range(minDistance, maxDistance);
            Vector2 random2D = Random.insideUnitCircle.normalized; // 길이 1짜리 XZ 방향 벡터
            Vector3 randomDir = new Vector3(random2D.x, 0f, random2D.y) * dist; // Y축 0으로 XZ 평면 변환
            Vector3 pos = self.position + randomDir;
            pos.y = self.position.y; // Y축 고정
            return pos;
        }, maxDistance);
    }


    public static Vector3 ClampDistanceFrom(this Vector3 self, Vector3 from, float limitDistance)
        => Vector3.ClampMagnitude(self - from, limitDistance) + from;


 

    #endregion
}
