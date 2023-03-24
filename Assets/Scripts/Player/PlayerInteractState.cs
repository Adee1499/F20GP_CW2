using System;
using System.Collections;
using UnityEngine;

public class PlayerInteractState : PlayerBaseState
{
    private enum InteractionType {
        Default,
        ItemPickUp,
        ChestOpen,
        Merchant
    }

    InteractionType _interactionType;
    bool _isInteracting;
    GameObject _merchant;

    public PlayerInteractState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) 
    : base(currentContext, playerStateFactory) {}

    public override void EnterState() 
    {
        Debug.Log("Entered interaction state");
        _interactionType = GetInteractionType();
        _isInteracting = true;
        switch (_interactionType) {
            case InteractionType.Merchant:
                Ctx.StartCoroutine(MerchantInteraction());
                break;
            case InteractionType.ItemPickUp:
                Ctx.StartCoroutine(PickUpAnimation());
                break;
        }
    }

    public override void UpdateState() 
    {
        if (!_isInteracting) {
            CheckSwitchStates();
        }
    }

    public override void ExitState() 
    {
        _interactionType = InteractionType.Default;
        _isInteracting = false;
        _merchant = null;
    }

    public override void InitializeSubState() {}

    public override void CheckSwitchStates() 
    {
        SwitchState(Factory.Idle());
    }

    InteractionType GetInteractionType()
    {
        // Check for merchant in interaction range
        Collider[] collidersWithinRange = Physics.OverlapSphere(Ctx.transform.position, Ctx.InteractionRange);
        foreach(Collider collider in collidersWithinRange) {
            if (/*collider.CompareTag("Merchant")*/ false) {
                _merchant = collider.gameObject;
                return InteractionType.Merchant;
            }
        }
        return InteractionType.ItemPickUp;
    }

    IEnumerator PickUpAnimation() 
    {
        Ctx.Animator.SetTrigger(Ctx.AnimPickUpHash);
        Collider[] lootWithinRange = Physics.OverlapSphere(Ctx.transform.position, Ctx.InteractionRange);

        bool shiftHeld = Input.GetKey("left shift");

        foreach(Collider collider in lootWithinRange){
            if(collider.CompareTag("Equipment")){
                EquipmentItem itemRef = collider.gameObject.GetComponent<Loot>().objRef;
                Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
                RaycastHit hit;

                if(shiftHeld){
                    InventoryUI.Instance.inventory.AddItem(itemRef, 1);
                    collider.gameObject.GetComponent<Loot>().destroyObject();
                }
                else if(Physics.Raycast (ray, out hit)){
                    if(hit.transform.position == collider.gameObject.transform.position){
                        InventoryUI.Instance.inventory.AddItem(itemRef, 1);
                        collider.gameObject.GetComponent<Loot>().destroyObject();
                    }
                }
            }
        }


        Debug.Log("Pick up");
        yield return new WaitForSeconds(0.5f);
        _isInteracting = false;
    }

    IEnumerator MerchantInteraction()
    {
        yield return new WaitForSeconds(0.01f);
        InventoryUI.Instance.UI_Merchant.SetActive(true);

        _isInteracting = false;
    }
}
