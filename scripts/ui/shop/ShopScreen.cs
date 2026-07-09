using Godot;
using System.Collections.Generic;

public partial class ShopScreen : Control
{
    [Export] private NodePath _moneyLabelPath = new NodePath("VBox/MoneyLabel");
    [Export] private NodePath _itemsContainerPath = new NodePath("VBox/ItemsScroll/ItemsContainer");
    [Export] private NodePath _equipContainerPath = new NodePath("VBox/EquipScroll/EquipContainer");
    [Export] private NodePath _leaveButtonPath = new NodePath("VBox/LeaveButton");

    private Label _moneyLabel;
    private VBoxContainer _itemsContainer;
    private VBoxContainer _equipContainer;
    private Button _leaveButton;

    public override void _Ready()
    {
        _moneyLabel = GetNode<Label>(_moneyLabelPath);
        _itemsContainer = GetNode<VBoxContainer>(_itemsContainerPath);
        _equipContainer = GetNode<VBoxContainer>(_equipContainerPath);
        _leaveButton = GetNode<Button>(_leaveButtonPath);

        _leaveButton.Pressed += OnLeavePressed;
        UpdateMoney();
        BuildItemList();
        BuildEquipList();
    }

    private void UpdateMoney()
    {
        _moneyLabel.Text = $"Gold: {EconomyManager.Instance.CurrentMoney}";
    }

    private static List<ShopItemData> LoadItems()
    {
        return ResourceLoaderHelper.LoadFromDir<ShopItemData>("res://resources/run_data/");
    }

    private void BuildItemList()
    {
        var items = LoadItems();
        foreach (var item in items)
        {
            bool alreadyOwned = RunState.Instance?.PurchasedShopItemIds.Contains(item.ItemId) == true;
            string unlockId = $"unlock_shop_{item.ItemId}";
            bool unlockOwned = SaveManager.Instance.GetMetaUpgradeLevel(unlockId) > 0;
            if (!unlockOwned) continue;

            var vbox = new VBoxContainer();
            var hbox = new HBoxContainer();
            var iconRect = new TextureRect();
            iconRect.Texture = item.Icon;
            iconRect.CustomMinimumSize = new Vector2(16, 16);
            iconRect.StretchMode = TextureRect.StretchModeEnum.Keep;
            int effectiveCost = RunState.Instance?.GetEffectiveShopCost(item.Cost) ?? item.Cost;
            var label = new Label();
            label.Text = effectiveCost < item.Cost
                ? $"{item.ItemName} ({effectiveCost}g, was {item.Cost}g)"
                : $"{item.ItemName} ({item.Cost}g)";
            var desc = new Label();
            desc.Text = item.Description;
            desc.Modulate = new Color(0.7f, 0.7f, 0.7f);
            var buyBtn = new Button();
            if (alreadyOwned)
            {
                buyBtn.Text = "✓ Owned";
                buyBtn.Disabled = true;
            }
            else
            {
                buyBtn.Text = "Buy";
                buyBtn.Disabled = !EconomyManager.Instance.CanAfford(effectiveCost);
                var captured = item;
                buyBtn.Pressed += () => OnBuyItem(captured, buyBtn);
            }
            hbox.AddChild(iconRect);
            hbox.AddChild(label);
            hbox.AddChild(buyBtn);
            vbox.AddChild(hbox);
            vbox.AddChild(desc);
            _itemsContainer.AddChild(vbox);
        }
    }

    private void OnBuyItem(ShopItemData item, Button btn)
    {
        int cost = RunState.Instance?.GetEffectiveShopCost(item.Cost) ?? item.Cost;

        bool hasPendingDiscount = RunState.Instance.FirstPurchaseDiscountPercent > 0f
            && item.FirstPurchaseDiscountPercent <= 0f;

        if (hasPendingDiscount)
            cost -= Mathf.RoundToInt(cost * RunState.Instance.FirstPurchaseDiscountPercent);

        if (!EconomyManager.Instance.CanAfford(cost)) return;

        EconomyManager.Instance.SpendMoney(cost);
        if (hasPendingDiscount)
            RunState.Instance.FirstPurchaseDiscountPercent = 0f;
        RunState.Instance?.PurchasedShopItemIds.Add(item.ItemId);
        if (item.Icon != null)
        {
            RunState.Instance?.PurchasedItemIcons.Add(item.Icon);
            RunState.Instance?.PurchasedItemNames.Add(item.ItemName);
            RunState.Instance?.PurchasedItemDescriptions.Add(item.Description);
        }
        ApplyItemEffect(item);
        UpdateMoney();
        btn.Disabled = true;
        btn.Text = "✓ Owned";

        var tween = CreateTween();
        tween.TweenProperty(btn, "modulate", new Color(0.5f, 1, 0.5f), 0.15f);
        tween.TweenProperty(btn, "modulate", Colors.White, 0.3f);
    }

    private static void ApplyItemEffect(ShopItemData item)
    {
        if (RunState.Instance == null) return;
        RunState.Instance.ShopDamageBonusPercent += item.DamageBonusPercent;
        RunState.Instance.ShopFireRateBonusPercent += item.FireRateBonusPercent;
        RunState.Instance.ShopRangeBonusPercent += item.RangeBonusPercent;
        RunState.Instance.ShopHeavyDamageBonusPercent += item.HeavyDamageBonusPercent;
        if (item.FirstPurchaseDiscountPercent > 0f)
            RunState.Instance.FirstPurchaseDiscountPercent += item.FirstPurchaseDiscountPercent;
    }

    private void BuildEquipList()
    {
        var allEquips = ResourceLoaderHelper.LoadFromDir<EquipData>("res://resources/equip_data/");

        foreach (var equip in allEquips)
        {
            bool inLoadout = RunState.Instance?.SelectedTowerIds.Contains(equip.TargetTowerId) == true;
            if (!inLoadout) continue;

            string unlockId = $"unlock_equip_{equip.Id}";
            bool unlockOwned = SaveManager.Instance.GetMetaUpgradeLevel(unlockId) > 0;
            if (!unlockOwned) continue;

            string equippedId = RunState.Instance?.GetEquippedItem(equip.TargetTowerId);
            bool alreadyEquipped = equippedId == equip.Id;

            SaveManager.Instance?.MarkDiscovered($"equip_{equip.Id}");

            var vbox = new VBoxContainer();
            var hbox = new HBoxContainer();
            var iconRect = new TextureRect();
            iconRect.Texture = equip.Icon;
            iconRect.CustomMinimumSize = new Vector2(16, 16);
            iconRect.StretchMode = TextureRect.StretchModeEnum.Keep;

            int equipEffectiveCost = RunState.Instance?.GetEffectiveShopCost(equip.Cost) ?? equip.Cost;
            var label = new Label();
            label.Text = equipEffectiveCost < equip.Cost
                ? $"{equip.Name} ({equipEffectiveCost}g, was {equip.Cost}g) [{equip.TargetTowerId.ToUpper()}]"
                : $"{equip.Name} ({equip.Cost}g) [{equip.TargetTowerId.ToUpper()}]";

            var desc = new Label();
            desc.Text = equip.Description;
            desc.Modulate = new Color(0.7f, 0.7f, 0.7f);

            var buyBtn = new Button();
            if (alreadyEquipped)
            {
                buyBtn.Text = "✓ Equipped";
                buyBtn.Disabled = true;
            }
            else if (!string.IsNullOrEmpty(equippedId))
            {
                buyBtn.Text = $"Replace ({equipEffectiveCost}g)";
                buyBtn.Disabled = !EconomyManager.Instance.CanAfford(equipEffectiveCost);
                var captured = equip;
                buyBtn.Pressed += () => OnBuyEquip(captured, buyBtn);
            }
            else
            {
                buyBtn.Text = "Buy & Equip";
                buyBtn.Disabled = !EconomyManager.Instance.CanAfford(equipEffectiveCost);
                var captured = equip;
                buyBtn.Pressed += () => OnBuyEquip(captured, buyBtn);
            }

            hbox.AddChild(iconRect);
            hbox.AddChild(label);
            hbox.AddChild(buyBtn);
            vbox.AddChild(hbox);
            vbox.AddChild(desc);
            _equipContainer.AddChild(vbox);
        }
    }

    private void OnBuyEquip(EquipData equip, Button btn)
    {
        int equipCost = RunState.Instance?.GetEffectiveShopCost(equip.Cost) ?? equip.Cost;
        if (!EconomyManager.Instance.CanAfford(equipCost)) return;

        EconomyManager.Instance.SpendMoney(equipCost);
        RunState.Instance.SetEquippedItem(equip.TargetTowerId, equip.Id);
        UpdateMoney();

        foreach (var child in _equipContainer.GetChildren())
        {
            child.QueueFree();
        }
        BuildEquipList();
    }

    private void OnLeavePressed()
    {
        RunState.Instance?.SaveCurrentRun();
        LevelManager.Instance.PickRandomLevel();
        UIManager.Instance.PopScreen();
        UIManager.Instance.PushScreen(UIManager.Instance.BriefingData);
    }
}
