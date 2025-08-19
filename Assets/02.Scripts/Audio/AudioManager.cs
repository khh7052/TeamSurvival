using Constants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : Singleton<AudioManager>
{
    private AudioMixer audioMixer;
    private AudioSource bgmSource;
    private GameObject audioPlayerObject;

    protected override void Initialize()
    {
        base.Initialize();
        SceneManager.sceneLoaded += OnSceneLoaded;

        audioMixer = Resources.Load<AudioMixer>("Audio/AudioMixer");
        audioPlayerObject = Resources.Load<GameObject>("Audio/AudioPlayer");

        if (audioMixer == null)
        {
            Debug.LogError("AudioMixer not found in Resources folder.");
            return;
        }

        CreateAudioSource(SoundType.BGM, ref bgmSource);
    }

    // ���� �ε�� �� BGM�� �ڵ����� ���
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SoundData bgmData = Resources.Load<SoundData>("Audio/BGM/" + scene.name);

        if (bgmData != null)
            PlayBGM(bgmData);
    }

    private void CreateAudioSource(SoundType soundType, ref AudioSource audioSource)
    {
        GameObject sourceObject = new(soundType.ToString());
        sourceObject.transform.SetParent(transform, false); // AudioManager�� �ڽ����� ����
        audioSource = sourceObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups(soundType.ToString())[0];
        audioSource.loop = (soundType == SoundType.BGM); // SFX�� �ݺ� ������� ����
        audioSource.playOnAwake = false; // �ڵ� ��� ����
        audioSource.volume = 1f; // �⺻ ���� ����
    }

    public void PlayBGM(SoundData soundData)
    {
        if (soundData == null || soundData.clips.Length == 0) return;

        AudioClip clip = soundData.clips[Random.Range(0, soundData.clips.Length)];
        PlayBGM(clip, soundData.volume);
    }

    public void PlayBGM(AudioClip clip, float volume = 1f)
    {
        if(clip == null) return;

        if (bgmSource.isPlaying)
            bgmSource.Stop();

        bgmSource.clip = clip;
        bgmSource.volume = volume;
        bgmSource.Play();
    }

    public void PlaySFX(SoundData soundData, Vector3 pos)
    {
        if (soundData == null || soundData.clips.Length == 0) return;

        GameObject go = ObjectPoolingManager.Instance.Get(audioPlayerObject, pos);
        AudioPlayer audioPlayer = go.GetComponent<AudioPlayer>();
        audioPlayer.PlaySFX(soundData);
    }

    public void SetVolume(VolumeType volumeType, float volume)
    {
        if (audioMixer == null) return;

        volume = Mathf.Clamp(volume, AudioConstants.MinVolume, AudioConstants.MaxVolume); // ���� ���� ����
        string parameterName = AudioConstants.GetExposedVolumeName(volumeType);
        audioMixer.SetFloat(parameterName, Mathf.Log10(volume) * 20); // dB�� ��ȯ
    }

    public void ResetVolume()
    {
        if (audioMixer == null) return;

        SetVolume(VolumeType.Master, AudioConstants.MasterVolume);
        SetVolume(VolumeType.BGM, AudioConstants.BGMVolume);
        SetVolume(VolumeType.SFX, AudioConstants.SFXVolume);
    }

}
