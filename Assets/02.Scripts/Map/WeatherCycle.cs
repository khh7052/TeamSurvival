using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public enum WeatherType
{
    Clear,
    Rain,
    Snow
}

public class WeatherCycle : MonoBehaviour
{
    [Header("Weather Particle")]
    public ParticleSystem rain;
    public ParticleSystem snow;

    [Header("Weather Chance")]
    [Range(0f, 1f)] public float rainChance = 0.3f;
    [Range(0f, 1f)] public float snowChance = 0.3f;
    public float changeInterval = 10f;

    private WeatherType currentWeather = WeatherType.Clear;
    private float timer = 0f;

    private void Update()
    {
        timer += Time.deltaTime;
       
        if(timer >= changeInterval)
        {
            ChangeWeather();
            timer = 0f;
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
                Debug.Log($"{type} ºñ³»¸°´Ù.");
                break;

            case WeatherType.Snow:
                snow.Play();
                Debug.Log($"{type} ´«³»¸°´Ù");
                break;

            case WeatherType.Clear:
                Debug.Log("¸¼À½");
                break;
        }

        currentWeather = type;
    }
}
