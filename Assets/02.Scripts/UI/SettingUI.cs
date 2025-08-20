using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;
using UnityEngine.AddressableAssets;

public class SettingUI : BaseUI
{
    [SerializeField] private Transform volumeSliderParent;
    [SerializeField] private AssetReference volumeSliderRef;

    protected override void Awake()
    {
        base.Awake();
        CreateVolumeSliders();
        OnReset();
    }

    async void CreateVolumeSliders()
    {
        var handle = Addressables.LoadAssetAsync<GameObject>(volumeSliderRef);
        await handle.Task;
        VolumeSlider volumeSlider = handle.Result.GetComponent<VolumeSlider>();

        if (volumeSlider == null)
        {
            Debug.LogError("VolumeSlider prefab not found.");
            return;
        }

        foreach (VolumeType volumeType in System.Enum.GetValues(typeof(VolumeType)))
        {
            VolumeSlider sliderInstance = Instantiate(volumeSlider, transform);
            sliderInstance.Initialize(volumeType, volumeSliderParent);
        }
    }

    public void OnReset()
    {
        AudioManager.Instance.ResetVolume();
        foreach (Transform child in volumeSliderParent)
        {
            VolumeSlider slider = child.GetComponent<VolumeSlider>();
            if (slider != null)
                slider.UpdateSliderValue();
        }
    }

    public void OnClose()
    {
        UIManager.Instance.CloseUI<SettingUI>();
        Cursor.lockState = CursorLockMode.Locked;
    }



}
