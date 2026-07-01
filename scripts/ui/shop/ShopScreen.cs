using Godot;
using System.Collections.Generic;

public partial class ShopScreen : Control
{
    private VBoxContainer _itemsContainer;
    private VBoxContainer _equipContainer;
    private Label _moneyLabel;
    private Button _leaveButton;

    public override void _Ready()
    {
        _moneyLabel = GetNode<Label>("VBox/MoneyLabel");
        _itemsContainer = GetNode<VBoxContainer>("VBox/ItemsScroll/ItemsContainer");
        _equipContainer = GetNode<VBoxContainer>("VBox/EquipScroll/EquipContainer");
        _leaveButton = GetNode<Button>("VBox/LeaveButton");

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
        var items = new List<ShopItemData>();
        var dir = DirAccess.Open("res://resources/run_data/");
        if (dir == null) return items;

        foreach (var file in dir.GetFiles())
        {
            if (!file.EndsWith(".tres") && !file.EndsWith(".res"))
                continue;
            var res = ResourceLoader.Load<Resource>("res://resources/run_data/" + file, "", ResourceLoader.CacheMode.Replace);
            if (res is ShopItemData item)
                items.Add(item);
        }

        return items;
    }

    private void BuildItemList()
    {
        var items = LoadItems();
        foreach (var item in items)
        {
            var hbox = new HBoxContainer();
            var label = new Label();
            label.Text = $"{item.ItemName} ({item.Cost}g)";
            var buyBtn = new Button();
            buyBtn.Text = "Buy";
            buyBtn.Disabled = !EconomyManager.Instance.CanAfford(item.Cost);
            buyBtn.Pressed += () => OnBuyItem(item, buyBtn);
            hbox.AddChild(label);
            hbox.AddChild(buyBtn);
            _itemsContainer.AddChild(hbox);
        }
    }

    private void OnBuyItem(ShopItemData item, Button btn)
    {
        int cost = item.Cost;

        bool hasPendingDiscount = RunState.Instance.FirstPurchaseDiscountPercent > 0f
            && item.FirstPurchaseDiscountPercent <= 0f;

        if (hasPendingDiscount)
            cost -= Mathf.RoundToInt(cost * RunState.Instance.FirstPurchaseDiscountPercent);

        if (!EconomyManager.Instance.CanAfford(cost)) return;

        EconomyManager.Instance.SpendMoney(cost);
        if (hasPendingDiscount)
            RunState.Instance.FirstPurchaseDiscountPercent = 0f;
        ApplyItemEffect(item);
        UpdateMoney();
        btn.Disabled = true;
        btn.Text = "Owned";
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
        var dir = DirAccess.Open("res://resources/equip_data/");
        if (dir == null) return;

        foreach (var file in dir.GetFiles())
        {
            if (!file.EndsWith(".tres") && !file.EndsWith(".res"))
                continue;

            var equip = ResourceLoader.Load<EquipData>("res://resources/equip_data/" + file, "", ResourceLoader.CacheMode.Replace);
            if (equip == null) continue;

            SaveManager.Instance?.MarkDiscovered($"equip_{equip.Id}");

            bool inLoadout = RunState.Instance?.SelectedTowerIds.Contains(equip.TargetTowerId) == true;
            string equippedId = RunState.Instance?.GetEquippedItem(equip.TargetTowerId);
            bool alreadyEquipped = equippedId == equip.Id;

            var hbox = new HBoxContainer();
            var label = new Label();
            label.Text = inLoadout
                ? $"{equip.Name} ({equip.Cost}g) [{equip.TargetTowerId.ToUpper()}]"
                : $"{equip.Name} (LOCKED - {equip.TargetTowerId} not in loadout)";

            var buyBtn = new Button();
            if (!inLoadout)
            {
                buyBtn.Text = "-";
                buyBtn.Disabled = true;
            }
            else if (alreadyEquipped)
            {
                buyBtn.Text = "Equipped";
                buyBtn.Disabled = true;
            }
            else
            {
                buyBtn.Text = "Buy & Equip";
                buyBtn.Disabled = !EconomyManager.Instance.CanAfford(equip.Cost);
                var capturedEquip = equip;
                buyBtn.Pressed += () => OnBuyEquip(capturedEquip, buyBtn);
            }

            hbox.AddChild(label);
            hbox.AddChild(buyBtn);
            _equipContainer.AddChild(hbox);
        }
    }

    private void OnBuyEquip(EquipData equip, Button btn)
    {
        if (!EconomyManager.Instance.CanAfford(equip.Cost)) return;

        EconomyManager.Instance.SpendMoney(equip.Cost);
        RunState.Instance.SetEquippedItem(equip.TargetTowerId, equip.Id);
        UpdateMoney();
        btn.Text = "Equipped";
        btn.Disabled = true;
    }

    private void OnLeavePressed()
    {
        LevelManager.Instance.PickRandomLevel();
        UIManager.Instance.PopScreen();
        UIManager.Instance.PushScreen(UIManager.Instance.BriefingData);
    }
}
