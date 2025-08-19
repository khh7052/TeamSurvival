using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BuildObj : MonoBehaviour
{
    public EntityModel model;
    public AnimationHandler animationHandler;

    [SerializeField]
    public BuildKey key;

    public bool IsDead => model.health.CurValue <= 0;


    public void Initialize()
    {
        model = GetComponent<EntityModel>();
        animationHandler = GetComponent<AnimationHandler>();
        model.health.OnChanged -= Health_OnChanged;

        model.health.Init();

        model.health.OnChanged += Health_OnChanged;

    }

    private void Health_OnChanged()
    {
        if (IsDead)
        {
            gameObject.SetActive(false);
        }
        else
        {
            // NPC가 데미지를 받았을 때의 로직
            // (현재 데미지 받았을 때의 이벤트가 따로 없으므로 일단 OnChanged에서 처리함, 나중에 수정필요)
            animationHandler?.PlayHit();
        }
    }

    private void OnDisable()
    {
        BuildingManager.Instance?.UnregisterBuild(key);
    }
}
