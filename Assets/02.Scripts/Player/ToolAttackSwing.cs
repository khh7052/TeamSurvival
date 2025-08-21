using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolAttackSwing : MonoBehaviour
{
    public Transform target;
    public Vector3 swingEuler = new Vector3(-20f, 10f, 0f);
    public Vector3 swingOffset = new Vector3(0.01f, -0.01f, 0.02f);

    [Header("타이밍")]
    public float inTime = 0.08f;
    public float outTime = 0.12f;

    [Header("보간 곡선")]
    public AnimationCurve inCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public AnimationCurve outCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    public bool resetToBaseOnDisable = true;

    Transform tr;
    Vector3 basePos;
    Quaternion baseRot;
    Coroutine playing;

    void Awake()
    {
        tr = target ? target : transform;
        basePos = tr.localPosition;
        baseRot = tr.localRotation;
    }

    void OnEnable()
    {
        tr = target ? target : transform;
        basePos = tr.localPosition;
        baseRot = tr.localRotation;
    }


    void OnDisable()
    {
        if (resetToBaseOnDisable && tr != null)
        {
            if (playing != null) StopCoroutine(playing);
            tr.localPosition = basePos;
            tr.localRotation = baseRot;
            playing = null;
        }
    }

    void OnDestroy()
    {
        if (playing != null) StopCoroutine(playing);
        playing = null;
    }

    public void CaptureAsBasePose()
    {
        if (tr == null) tr = target ? target : transform;
        basePos = tr.localPosition;
        baseRot = tr.localRotation;
    }

    public void SetProfile(Vector3 euler, Vector3 offset, float inT, float outT)
    {
        swingEuler = euler;
        swingOffset = offset;
        inTime = inT;
        outTime = outT;
        CaptureAsBasePose();
    }

    public void Play(float intensity = 1f)
    {
        if (!isActiveAndEnabled) return;

        if (tr == null) tr = target ? target : transform;
        if (playing != null) StopCoroutine(playing);
        playing = StartCoroutine(CoSwing(intensity));
    }

    IEnumerator CoSwing(float intensity)
    {
        if (tr == null || !this || !isActiveAndEnabled) { playing = null; yield break; }

        var posA = basePos;
        var rotA = baseRot;

        var rotB = rotA * Quaternion.Euler(swingEuler * intensity);
        var posB = posA + swingOffset * intensity; // 상한 클램프 제거

        // 진입
        float t = 0f, durIn = Mathf.Max(inTime, 0.0001f);
        while (t < 1f)
        {
            if (tr == null || !this || !isActiveAndEnabled) { playing = null; yield break; }

            t += Time.deltaTime / durIn;
            float e = inCurve.Evaluate(Mathf.Clamp01(t));
            tr.localRotation = Quaternion.Slerp(rotA, rotB, e);
            tr.localPosition = Vector3.Lerp(posA, posB, e);
            yield return null;
        }

        // 복귀
        t = 0f; float durOut = Mathf.Max(outTime, 0.0001f);
        while (t < 1f)
        {
            if (tr == null || !this || !isActiveAndEnabled) { playing = null; yield break; }

            t += Time.deltaTime / durOut;
            float e = outCurve.Evaluate(Mathf.Clamp01(t));
            tr.localRotation = Quaternion.Slerp(rotB, rotA, e);
            tr.localPosition = Vector3.Lerp(posB, posA, e);
            yield return null;
        }

        if (tr != null)
        {
            tr.localRotation = rotA;
            tr.localPosition = posA;
        }
        playing = null;
    }
}
