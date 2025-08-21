using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDeathBehavior
{
    public EntityModel model;
    PlayerView view;
    public PlayerController controller;
    public PlayerCondition condition;
    public EquipSystem equip;
    public ItemData itemData;
    public Action addItem { get; set; }
    public PlayerInventory inventory;
    public AnimationHandler animationHandler;

    public Transform dropPosition;

    private void Awake()
    {
        model = GetComponent<EntityModel>();
        view = GetComponent<PlayerView>();
        controller = GetComponent<PlayerController>();
        equip = GetComponent<EquipSystem>();
        view.Initialize();
        condition = GetComponent<PlayerCondition>();
        inventory = GetComponent<PlayerInventory>();
        animationHandler = GetComponent<AnimationHandler>();

        GameManager.player = this;


    }

    public void Die()
    {
        throw new NotImplementedException();
    }
}
