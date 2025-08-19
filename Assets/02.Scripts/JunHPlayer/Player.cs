using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    EntityModel model;
    PlayerView view;
    public PlayerController controller;
    public PlayerCondition condition;
    EquipSystem equip;
    public ItemData itemData;
    public Action addItem;

    public Transform dropPosition;

    private void Awake()
    {
        model = GetComponent<EntityModel>();
        view = GetComponent<PlayerView>();
        controller = GetComponent<PlayerController>();
        equip = GetComponent<EquipSystem>();
        view.Initialize(model);
        condition = GetComponent<PlayerCondition>();
    }
}
