using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSpawner : MonoBehaviour
{
    //���� ���� / ��������
    //Resource���� ���ļ� ����
    //�ʹ� ���۽����� ���� => �þ߹ۿ� ������ ���� or �Ʒ����� ���� �ھƳ��� ��������
    //Player ��ġ���� Spawn�� ��� ���ϼ� ����

    [Header("Spawn Setting")]
    public int resourceID; //Factor���� ������ Data ID => addressable���� ResourceŸ�Ը� �����ü� ������ �ʿ�X
    public float spawnInterval = 3f; //�����ֱ�
    public Vector3 spawnAreaMin; //���� �ּ� ��ǥ
    public Vector3 spawnAreaMax; //���� �ִ� ��ǥ
    public int maxSpawn = 10; //�ʿ� �����Ҽ� �ִ� �ִ� �ڿ� ��
    private int currentSpawn = 0; //���� �ʿ��ִ� �ڿ�

    private bool canSpawn = false;

    private void Start()
    {
        StartCoroutine(WaitForFactorySpawn());
    }

    IEnumerator WaitForFactorySpawn()
    {
        while(!Factory.Instance.IsInitialized || !ObjectPoolingManager.Instance.IsInitialized)  //�ʱ�ȭ������ ���
        {
            yield return null;
        }

        canSpawn = true;

        while(canSpawn)
        {
            SpawnResource();
            yield return new WaitForSeconds(spawnInterval); //�⵿ֱ�� �ݺ�
        }
    }

    void SpawnResource()
    {
        if (currentSpawn >= maxSpawn) return;

        Vector3 spawnPos = new Vector3(
            Random.Range(spawnAreaMin.x, spawnAreaMax.x),
            Random.Range(spawnAreaMin.y, spawnAreaMax.y),
            Random.Range(spawnAreaMin.z, spawnAreaMax.z)
            ); //���� ��ǥ ����

        GameObject resource = Factory.Instance.CreateByID<ItemData>(resourceID); //Factory���� ID������� Prefab ������
        if(resource != null)
        {
            resource.transform.position = spawnPos;
            currentSpawn++;
        }  
    }
}
