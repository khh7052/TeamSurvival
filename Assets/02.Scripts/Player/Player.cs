using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    EntityModel model;
    PlayerView view;
    PlayerController controller;
    EquipSystem equip;

    private void Awake()
    {
        model = GetComponent<EntityModel>();
        view = GetComponent<PlayerView>();
        controller = GetComponent<PlayerController>();
        equip = GetComponent<EquipSystem>();
        view.Initialize(model);
    }
}
