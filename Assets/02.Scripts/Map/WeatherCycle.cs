using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherCycle : MonoBehaviour
{
    [Header("Weather Prefabs")] //°¢ ÇÁ¸®ÆÕ¿¡ Trigger ³Ö¾î¼­ ´êÀ¸¸é Ã¼¿Â ¶³¾îÁö°Ôµµ ¼³Á¤ °¡´É
    public GameObject rainPrefab;
    public GameObject snowPrefab;

    [Header("Weather Chance")]
    [Range(0f, 1f)] public float rainChance; //ºñ¿Ã È®·ü
    [Range(0f, 1f)] public float snowChance; //´«¿Ã È®·ü
    public float changeInterval = 60f; //º¯°æ ÅÒ

    [Header("Spawn Setting")]
    public Vector3 spawnCenter;
    public float spawnHeight;
    public float spawnRangeX;
    public float spawnRangeZ;
    public float spawnInterval;

    private float spawnTimer = 0f;
    private float weatherTimer = 0f;

    private enum WeatherType { Clear, Rain, Snow }
    private WeatherType currentWeatherType = WeatherType.Clear;

    private void Start()
    {
        ChangeWeather();
    }

    private void Update()
    {
        weatherTimer += Time.deltaTime;
        if (weatherTimer >= changeInterval)
        {
            weatherTimer = 0f;
            ChangeWeather();
        }

        spawnTimer += Time.deltaTime;
        if(spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;
            SpawnWeatherPrefab();
        }
    }
    private void ChangeWeather()
    {
        float ran = Random.value;

        if(ran < rainChance)
        {
            currentWeatherType = WeatherType.Rain;
            Debug.Log("ºñ°¡ ³»¸°´Ù.");
        }
        else if(ran < rainChance + snowChance)
        {
            currentWeatherType = WeatherType.Snow;
            Debug.Log("´«ÀÌ ³»¸°´Ù.");
        }
        else
        {
            currentWeatherType = WeatherType.Clear;
            Debug.Log("ÇÏ´ÃÀÌ ¸¼´Ù");
        }     
    }

    private void SpawnWeatherPrefab()
    {
        Vector3 spawnPos = spawnCenter + new Vector3(
            Random.Range(-spawnRangeX, spawnRangeX),
            spawnHeight,
            Random.Range(-spawnRangeZ, spawnRangeZ));

        switch(currentWeatherType)
        {
            case WeatherType.Rain:
                Instantiate(rainPrefab, spawnPos, Quaternion.identity);
                break;
            case WeatherType.Snow:
                Instantiate(snowPrefab, spawnPos, Quaternion.identity);
                break;
            case WeatherType.Clear:
                break;


        }
    }
}

