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
    public static Action OnInteractWithMerchant;
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
            if (collider.CompareTag("Merchant")) {
                _merchant = collider.gameObject;
                return InteractionType.Merchant;
            }
        }
        return InteractionType.ItemPickUp;
    }

    IEnumerator PickUpAnimation() 
    {
        Ctx.Animator.SetTrigger(Ctx.AnimPickUpHash);
        Debug.Log("Pick up");
        yield return new WaitForSeconds(0.5f);
        _isInteracting = false;
    }

    IEnumerator MerchantInteraction()
    {
        yield return new WaitForSeconds(0.01f);
        InventoryUI.Instance.UI_Merchant.SetActive(true);
        InventoryUI.Instance.UI_Inventory.SetActive(true);
        InventoryUI.Instance.UI_Equipment.SetActive(false);
        OnInteractWithMerchant?.Invoke();

        _isInteracting = false;
    }
}
