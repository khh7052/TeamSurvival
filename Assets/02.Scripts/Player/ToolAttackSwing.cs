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

    Transform _t;
    Vector3 _basePos;
    Quaternion _baseRot;
    Coroutine _playing;

    void Awake()
    {
        _t = target ? target : transform;
        _basePos = _t.localPosition;
        _baseRot = _t.localRotation;
    }

    void OnEnable()
    {
        _t = target ? target : transform;
        _basePos = _t.localPosition;
        _baseRot = _t.localRotation;
    }

    void OnDisable()
    {
        if (resetToBaseOnDisable && _t != null)
        {
            if (_playing != null) StopCoroutine(_playing);
            _t.localPosition = _basePos;
            _t.localRotation = _baseRot;
            _playing = null;
        }
    }

    public void CaptureAsBasePose()
    {
        if (_t == null) _t = target ? target : transform;
        _basePos = _t.localPosition;
        _baseRot = _t.localRotation;
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
        if (_playing != null) StopCoroutine(_playing);
        _playing = StartCoroutine(CoSwing(intensity));
    }

    IEnumerator CoSwing(float intensity)
    {
        var posA = _basePos;
        var rotA = _baseRot;

        var rotB = rotA * Quaternion.Euler(swingEuler * intensity);
        var posB = posA + swingOffset * Mathf.Clamp(intensity, 0f, 10f);

        // 진입
        float t = 0f, durIn = Mathf.Max(inTime, 0.0001f);
        while (t < 1f)
        {
            t += Time.deltaTime / durIn;
            float e = inCurve.Evaluate(Mathf.Clamp01(t));
            _t.localRotation = Quaternion.Slerp(rotA, rotB, e);
            _t.localPosition = Vector3.Lerp(posA, posB, e);
            yield return null;
        }

        // 복귀
        t = 0f; float durOut = Mathf.Max(outTime, 0.0001f);
        while (t < 1f)
        {
            t += Time.deltaTime / durOut;
            float e = outCurve.Evaluate(Mathf.Clamp01(t));
            _t.localRotation = Quaternion.Slerp(rotB, rotA, e);
            _t.localPosition = Vector3.Lerp(posB, posA, e);
            yield return null;
        }

        _t.localRotation = rotA;
        _t.localPosition = posA;
        _playing = null;
    }
}
