using Godot;
using System.Collections.Generic;

public partial class MetaShopScreen : Control
{
    private VBoxContainer _itemsContainer;
    private Label _tokenLabel;
    private Button _backButton;

    private readonly List<(MetaUpgradeData data, Button buyBtn, Label statusLabel)> _entries = new();

    public override void _Ready()
    {
        _tokenLabel = GetNode<Label>("VBox/TokenLabel");
        _itemsContainer = GetNode<VBoxContainer>("VBox/ItemsContainer");
        _backButton = GetNode<Button>("VBox/BackButton");

        _backButton.Pressed += () => UIManager.Instance.PopScreen();
        UpdateTokens();
        BuildItemList();
    }

    private void UpdateTokens()
    {
        _tokenLabel.Text = $"Tokens: {SaveManager.Instance.MetaTokens}";
    }

    private void BuildItemList()
    {
        _entries.Clear();
        var items = LoadItems();
        foreach (var item in items)
        {
            var hbox = new HBoxContainer();

            int currentLevel = SaveManager.Instance.GetMetaUpgradeLevel(item.Id);
            bool isMaxed = currentLevel >= item.MaxLevel;

            string statusText = isMaxed ? "MAX" : $"Lv.{currentLevel}/{item.MaxLevel}";
            var label = new Label();
            label.Text = $"{item.Name} ({item.Description})";
            var statusLabel = new Label();
            statusLabel.Text = statusText;

            var buyBtn = new Button();
            if (isMaxed)
            {
                buyBtn.Text = "Owned";
                buyBtn.Disabled = true;
            }
            else
            {
                int cost = item.CostTokens * (currentLevel + 1);
                buyBtn.Text = $"Buy ({cost}t)";
                buyBtn.Disabled = SaveManager.Instance.MetaTokens < cost;
            }

            var capturedItem = item;
            buyBtn.Pressed += () => OnBuyItem(capturedItem);

            hbox.AddChild(label);
            hbox.AddChild(statusLabel);
            hbox.AddChild(buyBtn);
            _itemsContainer.AddChild(hbox);

            _entries.Add((item, buyBtn, statusLabel));
        }
    }

    private void OnBuyItem(MetaUpgradeData item)
    {
        int currentLevel = SaveManager.Instance.GetMetaUpgradeLevel(item.Id);
        if (currentLevel >= item.MaxLevel) return;

        int cost = item.CostTokens * (currentLevel + 1);
        if (!SaveManager.Instance.SpendMetaTokens(cost)) return;

        int newLevel = currentLevel + 1;
        SaveManager.Instance.SetMetaUpgradeLevel(item.Id, newLevel);

        if (item.IsTowerUnlock && !string.IsNullOrEmpty(item.TowerId))
            SaveManager.Instance.UnlockTower(item.TowerId);

        UpdateTokens();
        RefreshList();
    }

    private void RefreshList()
    {
        foreach (var child in _itemsContainer.GetChildren())
            child.QueueFree();

        BuildItemList();
    }

    private static List<MetaUpgradeData> LoadItems()
    {
        var items = new List<MetaUpgradeData>();
        var paths = new string[]
        {
            "res://resources/meta_upgrade_data/UnlockIce.tres",
            "res://resources/meta_upgrade_data/UnlockPoison.tres",
            "res://resources/meta_upgrade_data/UnlockSplash.tres",
            "res://resources/meta_upgrade_data/GlobalDamage.tres",
            "res://resources/meta_upgrade_data/StartingGold.tres",
        };
        foreach (var path in paths)
        {
            var item = GD.Load<MetaUpgradeData>(path);
            if (item != null)
                items.Add(item);
        }
        return items;
    }
}
