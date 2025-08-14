using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    #endregion
}
