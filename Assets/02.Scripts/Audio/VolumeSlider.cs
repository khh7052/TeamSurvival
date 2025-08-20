using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Constants;

public class VolumeSlider : MonoBehaviour
{
    private VolumeType volumeType;
    private TMP_Text label;
    private Slider slider;

    public void Initialize(VolumeType volumeType, Transform parent)
    {
        transform.SetParent(parent, false);

        this.volumeType = volumeType;
        name = $"{volumeType}_VolumeSlider";

        label = GetComponentInChildren<TMP_Text>();
        slider = GetComponentInChildren<Slider>();
        slider.onValueChanged.AddListener(OnSliderValueChanged);

        UpdateSliderValue();
        UpdateLabel();
    }

    private void OnSliderValueChanged(float value)
        => AudioManager.Instance.SetVolume(volumeType, value);

    public void UpdateSliderValue()
        => slider.value = AudioManager.Instance.GetVolume(volumeType);

    private void UpdateLabel()
        => label.text = volumeType.ToString();



}
