using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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

    private void Update()
    {
        Die();
    }

    public void Die()
    {
        if (model.isDie) return;

        if (model.health.CurValue <= 0f)
        {
            model.isDie = true;

            Debug.Log("ав╬З╣Ш");
            Animator animator = GetComponent<Animator>();
            if (animator != null) animator.enabled = false;

            NavMeshAgent agent = GetComponent<NavMeshAgent>();
            if (agent != null) agent.enabled = false;

            controller.canLook = false;
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.constraints = RigidbodyConstraints.None;
                rb.AddForce(transform.right * 1.5f, ForceMode.Impulse); 
                rb.AddTorque(Vector3.back * 1.5f, ForceMode.Impulse);  
            }
        }
    }
}
