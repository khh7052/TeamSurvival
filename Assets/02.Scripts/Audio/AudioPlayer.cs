using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour
{
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySFX(SoundData soundData)
    {
        if (soundData == null || soundData.clips.Length == 0) return;

        AudioClip clip = soundData.clips[Random.Range(0, soundData.clips.Length)];
        audioSource.PlayOneShot(clip, soundData.volume);

        // 재생이 끝난 후 자동으로 비활성화
        Invoke(nameof(Disable), clip.length);
    }

    private void Disable()
        => gameObject.SetActive(false);

}
