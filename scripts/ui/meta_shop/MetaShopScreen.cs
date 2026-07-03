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
        string[] tabs = { "All", "Unlocks", "Stats", "Economy" };
        foreach (var tab in tabs)
        {
            var btn = new Button();
            btn.Text = tab;
            btn.ToggleMode = true;
            btn.ButtonPressed = tab == "All";
            if (tab == "All")
                btn.Disabled = true;

            var capturedTab = tab;
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

    private void BuildItemList(string categoryFilter)
    {
        _allEntries.Clear();
        var items = LoadItems();

        foreach (var item in items)
        {
            if (categoryFilter != "All" && item.Category != categoryFilter)
                continue;

            var hbox = new HBoxContainer();

            int currentLevel = SaveManager.Instance.GetMetaUpgradeLevel(item.Id);
            bool isMaxed = currentLevel >= item.MaxLevel;

            string statusText = isMaxed ? "MAX" : $"Lv.{currentLevel}/{item.MaxLevel}";
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

            _allEntries.Add((item, buyBtn, statusLabel));
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

        BuildItemList(_activeTab);
    }

    private static List<MetaUpgradeData> LoadItems()
    {
        var items = new List<MetaUpgradeData>();
        var dir = DirAccess.Open("res://resources/meta_upgrade_data/");
        if (dir == null) return items;

        foreach (var file in dir.GetFiles())
        {
            if (!file.EndsWith(".tres") && !file.EndsWith(".res"))
                continue;
            var item = ResourceLoader.Load<MetaUpgradeData>("res://resources/meta_upgrade_data/" + file, "", ResourceLoader.CacheMode.Replace);
            if (item != null)
                items.Add(item);
        }

        items.Sort((a, b) => string.Compare(a.Id, b.Id, System.StringComparison.Ordinal));
        return items;
    }
}
