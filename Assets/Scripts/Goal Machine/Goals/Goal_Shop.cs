using UnityEngine;
using System.Collections.Generic;


public class Goal_ShopKeeping : Goal_Composite
{
    ShopManager shopManager;

    public Goal_ShopKeeping(CognitiveAgent owner)
        : base(owner) { }

    public override void Activate()
    {
        Owner.ThoughtManager.SetDialog("(SHOPKEEPING | )");
        CurrentStatus = GoalStatus.Active;
        RemoveAllSubgoals();

        shopManager = LocationManager.Instance.GetLocationMemory("Shop").GetComponent<ShopManager>();

        AddSubgoal(new Goal_ManageShop(Owner, shopManager));
        AddSubgoal(new Goal_MoveToPosition(Owner, shopManager.cTransform));
    }

    public override GoalStatus Process()
    {
        if (CurrentStatus == GoalStatus.Inactive)
            Activate();

        if (subGoals.Count > 0)
            CurrentStatus = ProcessSubgoals();

        return CurrentStatus;
    }
}

public class Goal_ManageShop : Goal_Atomic
{
    ShopManager shopManager;
    public Goal_ManageShop(CognitiveAgent owner, ShopManager shopManager)
        : base(owner)
    {
        this.shopManager = shopManager;
    }

    public override void Activate()
    {
        CurrentStatus = GoalStatus.Active;
        AddToThoughtBubble("Managing Shop");
        shopManager.OpenShop(Owner.CharacterCue);

        Owner.Transform.forward = (shopManager.eTransform.position - Owner.Transform.position).normalized;

        Owner.Animation[Owner.AnimationKeys["idle"]].speed = Owner.AnimationSpeed;
        Owner.Animation.CrossFade(Owner.AnimationKeys["idle"]);

        base.Activate();
    }

    public override void ApplyStateModVector()
    {
        Owner.startModification(Task.Shopkeeper);
    }

    public override void Reactivate()
    {
        AddToThoughtBubble("Managing Shop");

        Owner.Animation[Owner.AnimationKeys["idle"]].speed = Owner.AnimationSpeed;
        Owner.Animation.CrossFade(Owner.AnimationKeys["idle"]);
    }

    public override void Terminate()
    {
        shopManager.CloseShop();
    }

    public override GoalStatus Process()
    {
        if (CurrentStatus == GoalStatus.Inactive)
            Activate();

        return CurrentStatus;
    }
}

public class Goal_PurchaseItem : Goal_Composite
{
    ShopManager shopManager;
    string itemUID;

    public Goal_PurchaseItem(CognitiveAgent owner, string itemUID)
        : base(owner)
    {
        this.itemUID = itemUID;
    }

    public override void Activate()
    {
        AddToThoughtBubble("Buying Item");
        CurrentStatus = GoalStatus.Active;
        RemoveAllSubgoals();

        shopManager = LocationManager.Instance.GetLocationMemory("Shop").GetComponent<ShopManager>();

        AddSubgoal(new Goal_BuyItem(Owner, shopManager, itemUID));
        AddSubgoal(new Goal_MoveToPosition(Owner, shopManager.eTransform));
    }

    public override GoalStatus Process()
    {
        if (CurrentStatus == GoalStatus.Inactive)
            Activate();

        if (subGoals.Count > 0)
            CurrentStatus = ProcessSubgoals();

        return CurrentStatus;
    }
}

public class Goal_BuyItem : Goal
{
    ShopManager shopManager;
    string itemUID;

    public Goal_BuyItem(CognitiveAgent owner, ShopManager shopManager, string itemUID)
        : base(owner)
    {
        this.shopManager = shopManager;
        this.itemUID = itemUID;
    }

    public override void Activate()
    {
        CurrentStatus = GoalStatus.Active;
        AddToThoughtBubble("Buying Item");

        Owner.Transform.forward = (shopManager.cTransform.position - Owner.Transform.position).normalized;

        Owner.Animation[Owner.AnimationKeys["idle"]].speed = Owner.AnimationSpeed;
        Owner.Animation.CrossFade(Owner.AnimationKeys["idle"]);

        if (shopManager.AttemptPurchase(Owner, itemUID))
            CurrentStatus = GoalStatus.Completed;
        else
            CurrentStatus = GoalStatus.Failed;
    }

    public override void Reactivate()
    {
        AddToThoughtBubble("Buying Item");

        Owner.Animation[Owner.AnimationKeys["idle"]].speed = Owner.AnimationSpeed;
        Owner.Animation.CrossFade(Owner.AnimationKeys["idle"]);
    }

    public override GoalStatus Process()
    {
        if (CurrentStatus == GoalStatus.Inactive)
            Activate();

        return CurrentStatus;
    }
}
