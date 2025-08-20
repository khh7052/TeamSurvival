using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeatherType
{
    Clear,
    Rain,
    Snow
}

public class WeatherCycle : MonoBehaviour
{
    public static WeatherCycle Instance; //�׽�Ʈ�� �̱���

    [Header("Weather Particle")]
    public ParticleSystem rain;
    public ParticleSystem snow;

    [Header("Weather Chance")]
    [Range(0f, 1f)] public float rainChance = 0.3f; //�� ���� Ȯ��
    [Range(0f, 1f)] public float snowChance = 0.3f; //���� ���� Ȯ��
    public float changeInterval = 10f;

    private WeatherType currentWeather = WeatherType.Clear;
    private float timer = 0f;

    private List<IWeatherObserver> observers = new();

    private void Awake() => Instance = this; //�׽�Ʈ

    public void RegisterObserver(IWeatherObserver observer)
    {
        if (!observers.Contains(observer))
        {
            observers.Add(observer);
        }
    }

    public void RemoveObserver(IWeatherObserver observer)
    {
        observers.Remove(observer);
    }

    private void NotifyObservers()
    {
        foreach (var observer in observers)
        {
            observer.OnWeatherChanged(currentWeather);
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= changeInterval)
        {
            ChangeWeather(); //���� ���� ����
            timer = 0f; //Ÿ�̸� �ʱ�ȭ
        }
    }

    private void ChangeWeather()
    {
        float ran = Random.value;

        if (ran < rainChance)
        {
            SetWeather(WeatherType.Rain);
        }
        else if (ran < rainChance + snowChance)
        {
            SetWeather(WeatherType.Snow);
        }
        else
        {
            SetWeather(WeatherType.Clear);
        }
    }

    private void SetWeather(WeatherType type)
    {
        if (currentWeather == type) return;

        rain.Stop();
        snow.Stop();

        switch (type)
        {
            case WeatherType.Rain:
                rain.Play();
                Debug.Log($"{type} �񳻸���.");
                break;

            case WeatherType.Snow:
                snow.Play();
                Debug.Log($"{type} ��������");
                break;

            case WeatherType.Clear:
                Debug.Log("����");
                break;
        }

        currentWeather = type;
        NotifyObservers();
    }
}

