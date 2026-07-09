using Godot;
using System.Collections.Generic;

public partial class MetaShopScreen : Control
{
    [Export] private NodePath _tokenLabelPath = new NodePath("VBox/TokenLabel");
    [Export] private NodePath _itemsContainerPath = new NodePath("VBox/ItemsScroll/ItemsContainer");
    [Export] private NodePath _backButtonPath = new NodePath("VBox/BackButton");

    private Label _tokenLabel;
    private VBoxContainer _itemsContainer;
    private Button _backButton;

    private HBoxContainer _tabsContainer;
    private readonly List<(MetaUpgradeData data, Button buyBtn, Label statusLabel)> _allEntries = new();
    private string _activeTab = "All";

    private static readonly Dictionary<string, string[]> _tabFilters = new()
    {
        { "All", null },
        { "Towers", new[] { "Unlocks" } },
        { "Upgrades", new[] { "Upgrades" } },
        { "Boosts", new[] { "Boosts" } },
        { "Equip", new[] { "Equip" } },
    };

    private static readonly HashSet<string> _hiddenCategories = new()
    {
        "ShopItem", "StatsItem", "EconomyItem", "SpecialItem", "TrinketItem"
    };

    public override void _Ready()
    {
        _tokenLabel = GetNode<Label>(_tokenLabelPath);
        _itemsContainer = GetNode<VBoxContainer>(_itemsContainerPath);
        _backButton = GetNode<Button>(_backButtonPath);

        _tabsContainer = new HBoxContainer();
        var vbox = GetNode<VBoxContainer>("VBox");
        vbox.AddChild(_tabsContainer);
        vbox.MoveChild(_tabsContainer, vbox.GetChildCount() - 2);

        _backButton.Pressed += () => UIManager.Instance.PopScreen();
        UpdateTokens();
        BuildTabs();
        BuildItemList("All");
    }

    private void UpdateTokens()
    {
        _tokenLabel.Text = $"Tokens: {SaveManager.Instance.MetaTokens}";
    }

    private void BuildTabs()
    {
        foreach (var kvp in _tabFilters)
        {
            var btn = new Button();
            btn.Text = kvp.Key;
            btn.ToggleMode = true;
            btn.ButtonPressed = kvp.Key == "All";
            if (kvp.Key == "All")
                btn.Disabled = true;

            var capturedTab = kvp.Key;
            btn.Pressed += () => OnTabSelected(capturedTab);

            _tabsContainer.AddChild(btn);
        }
    }

    private void OnTabSelected(string tab)
    {
        _activeTab = tab;
        foreach (var child in _tabsContainer.GetChildren())
        {
            if (child is Button btn)
            {
                btn.Disabled = false;
                btn.ButtonPressed = btn.Text == tab;
                if (btn.Text == tab)
                    btn.Disabled = true;
            }
        }
        RefreshList();
    }

    private bool IsMultiLevel(MetaUpgradeData item) => item.MaxLevel > 1;

    private int GetCost(MetaUpgradeData item, int currentLevel)
    {
        return IsMultiLevel(item)
            ? item.CostTokens * (currentLevel + 1)
            : item.CostTokens;
    }

    private void BuildItemList(string tab)
    {
        _allEntries.Clear();
        var items = LoadItems();
        var filter = _tabFilters[tab];

        foreach (var item in items)
        {
            if (_hiddenCategories.Contains(item.Category))
                continue;
            if (filter != null && System.Array.IndexOf(filter, item.Category) < 0)
                continue;

            var hbox = new HBoxContainer();

            int currentLevel = SaveManager.Instance.GetMetaUpgradeLevel(item.Id);
            bool isMaxed = currentLevel >= item.MaxLevel;
            int cost = GetCost(item, currentLevel);

            string statusText = isMaxed ? "Owned" : (IsMultiLevel(item) ? $"Lv.{currentLevel}/{item.MaxLevel}" : $"Buy ({cost}t)");
            var label = new Label();
            label.Text = $"{item.Name} ({item.Description})";
            label.SizeFlagsHorizontal = Control.SizeFlags.Expand | Control.SizeFlags.Fill;
            var statusLabel = new Label();
            statusLabel.Text = statusText;

            var buyBtn = new Button();
            if (isMaxed)
            {
                buyBtn.Text = "Owned";
                buyBtn.Disabled = true;
            }
            else if ((item.Category == "Upgrades" || item.Category == "Equip")
                && !string.IsNullOrEmpty(item.TowerId)
                && !SaveManager.Instance.IsTowerUnlocked(item.TowerId))
            {
                buyBtn.Text = "Requires Tower";
                buyBtn.Disabled = true;
            }
            else
            {
                buyBtn.Text = $"Buy ({cost}t)";
                buyBtn.Disabled = SaveManager.Instance.MetaTokens < cost;
            }

            var capturedItem = item;
            buyBtn.Pressed += () => OnBuyItem(capturedItem);

            hbox.AddChild(label);
            hbox.AddChild(statusLabel);
            hbox.AddChild(buyBtn);
            _itemsContainer.AddChild(hbox);

            _allEntries.Add((item, buyBtn, statusLabel));
        }
    }

    private void OnBuyItem(MetaUpgradeData item)
    {
        int currentLevel = SaveManager.Instance.GetMetaUpgradeLevel(item.Id);
        if (currentLevel >= item.MaxLevel) return;

        int cost = GetCost(item, currentLevel);
        if (!SaveManager.Instance.SpendMetaTokens(cost)) return;

        int newLevel = currentLevel + 1;
        SaveManager.Instance.SetMetaUpgradeLevel(item.Id, newLevel);

        if (item.IsTowerUnlock && !string.IsNullOrEmpty(item.TowerId))
            SaveManager.Instance.UnlockTower(item.TowerId);

        if (item.Category == "Boosts")
            UnlockRandomFromCategory(item.Id);

        UpdateTokens();
        RefreshList();
    }

    private void UnlockRandomFromCategory(string unlockId)
    {
        string targetCat = unlockId switch
        {
            "unlock_random_shop" => "ShopItem",
            "unlock_random_stats" => "StatsItem",
            "unlock_random_economy" => "EconomyItem",
            "unlock_random_special" => "SpecialItem",
            "unlock_random_trinket" => "TrinketItem",
            _ => null
        };
        if (targetCat == null) return;

        var pool = LoadItems().FindAll(i =>
            i.Category == targetCat &&
            SaveManager.Instance.GetMetaUpgradeLevel(i.Id) < i.MaxLevel);

        if (pool.Count == 0) return;

        var chosen = pool[(int)(GD.Randi() % (uint)pool.Count)];
        SaveManager.Instance.SetMetaUpgradeLevel(chosen.Id, 1);
    }

    private void RefreshList()
    {
        foreach (var child in _itemsContainer.GetChildren())
            child.QueueFree();

        BuildItemList(_activeTab);
    }

    private static List<MetaUpgradeData> LoadItems()
    {
        var items = ResourceLoaderHelper.LoadFromDir<MetaUpgradeData>("res://resources/meta_upgrade_data/");
        items.Sort((a, b) => string.Compare(a.Id, b.Id, System.StringComparison.Ordinal));
        return items;
    }
}
