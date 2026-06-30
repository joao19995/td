using Godot;
using System.Collections.Generic;

public partial class TrinketChoiceScreen : Control
{
    private VBoxContainer _itemsContainer;
    private Label _titleLabel;

    public override void _Ready()
    {
        _titleLabel = GetNode<Label>("VBox/TitleLabel");
        _itemsContainer = GetNode<VBoxContainer>("VBox/ItemsContainer");
        BuildChoices();
    }

    private void BuildChoices()
    {
        var trinkets = LoadAllTrinkets();
        var chosen = new List<TrinketData>();

        if (trinkets.Count <= 3)
        {
            chosen.AddRange(trinkets);
        }
        else
        {
            var pool = new List<TrinketData>(trinkets);
            for (int i = 0; i < 3 && pool.Count > 0; i++)
            {
                int idx = GD.RandRange(0, pool.Count - 1);
                chosen.Add(pool[idx]);
                pool.RemoveAt(idx);
            }
        }

        foreach (var trinket in chosen)
        {
            var hbox = new HBoxContainer();
            var label = new Label();
            label.Text = $"{trinket.Name} - {trinket.Description}";
            var takeBtn = new Button();
            takeBtn.Text = "Take";
            var captured = trinket;
            takeBtn.Pressed += () => OnTakeTrinket(captured);
            hbox.AddChild(label);
            hbox.AddChild(takeBtn);
            _itemsContainer.AddChild(hbox);
        }
    }

    private static List<TrinketData> LoadAllTrinkets()
    {
        var list = new List<TrinketData>();
        var ids = new[] { "war_banner", "guardian_angel", "lucky_coin" };
        foreach (var id in ids)
        {
            var t = GD.Load<TrinketData>($"res://resources/trinket_data/{id}.tres");
            if (t != null) list.Add(t);
        }
        return list;
    }

    private void OnTakeTrinket(TrinketData trinket)
    {
        RunState.Instance.ApplyTrinket(trinket);
        LevelManager.Instance.PickRandomLevel();
        UIManager.Instance.PopScreen();
        UIManager.Instance.PushScreen(UIManager.Instance.BriefingData);
    }
}
