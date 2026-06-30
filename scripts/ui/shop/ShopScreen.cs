using Godot;
using System.Collections.Generic;

public partial class ShopScreen : Control
{
    private VBoxContainer _itemsContainer;
    private Label _moneyLabel;
    private Button _leaveButton;

    public override void _Ready()
    {
        _moneyLabel = GetNode<Label>("VBox/MoneyLabel");
        _itemsContainer = GetNode<VBoxContainer>("VBox/ItemsContainer");
        _leaveButton = GetNode<Button>("VBox/LeaveButton");

        _leaveButton.Pressed += OnLeavePressed;
        UpdateMoney();
        BuildItemList();
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

    private void UpdateMoney()
    {
        _moneyLabel.Text = $"Gold: {EconomyManager.Instance.CurrentMoney}";
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

    private void OnLeavePressed()
    {
        UIManager.Instance.PopScreen();
        LevelManager.Instance.LoadRandomLevel();
    }
}
