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
        _itemsContainer = GetNode<VBoxContainer>("VBox/ItemsContainer");
        _equipContainer = GetNode<VBoxContainer>("VBox/EquipContainer");
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

    private List<ShopItemData> LoadItems()
    {
        var items = new List<ShopItemData>();
        var patterns = new string[]
        {
            "res://resources/run_data/ShopItems.tres",
            "res://resources/run_data/ShopItem2.tres",
        };
        foreach (var path in patterns)
        {
            var item = GD.Load<ShopItemData>(path);
            if (item != null)
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
            buyBtn.Pressed += () => OnBuyItem(item, buyBtn);
            hbox.AddChild(label);
            hbox.AddChild(buyBtn);
            _itemsContainer.AddChild(hbox);
        }
    }

    private void OnBuyItem(ShopItemData item, Button btn)
    {
        if (!EconomyManager.Instance.CanAfford(item.Cost)) return;

        EconomyManager.Instance.SpendMoney(item.Cost);
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
    }

    private void BuildEquipList()
    {
        var path = "res://resources/equip_data/";
        var ids = new[] { "heavy_barrel", "overdrive_coils", "precision_lens" };
        foreach (var id in ids)
        {
            var equip = GD.Load<EquipData>($"{path}{id}.tres");
            if (equip == null) continue;

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
