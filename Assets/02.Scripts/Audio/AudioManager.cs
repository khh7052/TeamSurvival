using Constants;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEditor;
using System.Threading.Tasks;

public class AudioManager : Singleton<AudioManager>, IInitializableAsync
{
    private AudioMixer audioMixer;
    private AudioSource bgmSource;
    private GameObject audioPlayerObject;

    public bool IsInitialized { get; private set; }

    public async void InitializeAsync()
    {
        var mixerHandle = Addressables.LoadAssetsAsync<AudioMixer>("AudioMixer", obj =>
        {                   
            audioMixer = obj;
            Debug.Log($"AudioMixer loaded: {audioMixer.name}");
        });

        var audioPlayerHandle = Addressables.LoadAssetsAsync<GameObject>("AudioPlayer", obj =>
        {
            audioPlayerObject = obj;
            Debug.Log($"AudioPlayerObject loaded: {audioPlayerObject.name}");
        });

        await mixerHandle.Task;
        await audioPlayerHandle.Task;

        SceneManager.sceneLoaded += OnSceneLoaded;

        CreateAudioSource(SoundType.BGM, ref bgmSource);
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);

        IsInitialized = true;
    }


    protected override void Initialize()
    {
        base.Initialize();
        InitializeAsync();
    }

    // ���� �ε�� �� BGM�� �ڵ����� ���
    private async void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SoundData bgmData = await LoadSoundData(scene.name);
        PlayBGM(bgmData);
    }


    public static Task<SoundData> LoadSoundData(string name)
        => Addressables.LoadAssetAsync<SoundData>(name).Task;

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

    public float GetVolume(VolumeType volumeType)
    {
        string parameterName = AudioConstants.GetExposedVolumeName(volumeType);
        if (audioMixer.GetFloat(parameterName, out float value))
            return Mathf.Pow(10, value / 20); // dB���� �������� ��ȯ

        return volumeType switch {
            VolumeType.Master => AudioConstants.MasterVolume,
            VolumeType.BGM => AudioConstants.BGMVolume,
            VolumeType.SFX => AudioConstants.SFXVolume,
            _ => 1f, // �⺻��
        };
    }

    public void ResetVolume()
    {
        if (audioMixer == null) return;

        SetVolume(VolumeType.Master, AudioConstants.MasterVolume);
        SetVolume(VolumeType.BGM, AudioConstants.BGMVolume);
        SetVolume(VolumeType.SFX, AudioConstants.SFXVolume);
    }

}
