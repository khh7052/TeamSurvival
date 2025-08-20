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

    // 씬이 로드될 때 BGM을 자동으로 재생
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
        sourceObject.transform.SetParent(transform, false); // AudioManager의 자식으로 설정
        audioSource = sourceObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups(soundType.ToString())[0];
        audioSource.loop = (soundType == SoundType.BGM); // SFX는 반복 재생하지 않음
        audioSource.playOnAwake = false; // 자동 재생 방지
        audioSource.volume = 1f; // 기본 볼륨 설정
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

        volume = Mathf.Clamp(volume, AudioConstants.MinVolume, AudioConstants.MaxVolume); // 볼륨 범위 제한
        string parameterName = AudioConstants.GetExposedVolumeName(volumeType);
        audioMixer.SetFloat(parameterName, Mathf.Log10(volume) * 20); // dB로 변환
    }

    public float GetVolume(VolumeType volumeType)
    {
        string parameterName = AudioConstants.GetExposedVolumeName(volumeType);
        if (audioMixer.GetFloat(parameterName, out float value))
            return Mathf.Pow(10, value / 20); // dB에서 볼륨으로 변환

        return volumeType switch {
            VolumeType.Master => AudioConstants.MasterVolume,
            VolumeType.BGM => AudioConstants.BGMVolume,
            VolumeType.SFX => AudioConstants.SFXVolume,
            _ => 1f, // 기본값
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
