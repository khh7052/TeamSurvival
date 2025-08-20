using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constants;

public class SettingUI : BaseUI
{
    [SerializeField] private Transform volumeSliderParent;

    protected override void Awake()
    {
        base.Awake();
        CreateVolumeSliders();
        OnReset();
    }

    void CreateVolumeSliders()
    {
        VolumeSlider volumeSlider = Resources.Load<VolumeSlider>("UI/VolumeSlider");
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
