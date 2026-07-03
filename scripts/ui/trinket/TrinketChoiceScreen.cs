using Godot;
using System.Collections.Generic;

public partial class TrinketChoiceScreen : Control
{
    private Label _titleLabel;
    private HBoxContainer _cardsContainer;
    private Button _skipButton;

    private static readonly Color CommonBorder = new Color(0.6f, 0.6f, 0.6f);
    private static readonly Color RareBorder = new Color(1f, 0.84f, 0f);

    public override void _Ready()
    {
        _titleLabel = GetNode<Label>("VBox/TitleLabel");
        _cardsContainer = GetNode<HBoxContainer>("VBox/CardsContainer");

        _skipButton = new Button();
        _skipButton.Text = "Skip";
        _skipButton.Pressed += OnSkip;
        var vbox = _cardsContainer.GetParent<VBoxContainer>();
        vbox.AddChild(_skipButton);

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

        for (int i = 0; i < chosen.Count; i++)
        {
            var trinket = chosen[i];
            SaveManager.Instance?.MarkDiscovered($"trinket_{trinket.Id}");

            var card = BuildCard(trinket);
            _cardsContainer.AddChild(card);
            card.Modulate = new Color(1, 1, 1, 0);

            var tween = CreateTween();
            tween.TweenInterval(i * 0.15f);
            tween.TweenProperty(card, "modulate", Colors.White, 0.25f).SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
        }

        if (chosen.Count == 0)
        {
            _titleLabel.Text = "NO TRINKETS AVAILABLE";
            _skipButton.Text = "Continue";
        }
    }

    private PanelContainer BuildCard(TrinketData trinket)
    {
        var panel = new PanelContainer();
        panel.CustomMinimumSize = new Vector2(80, 100);
        panel.SizeFlagsHorizontal = Control.SizeFlags.Expand | Control.SizeFlags.Fill;

        var borderColor = trinket.Rarity == TrinketRarity.Rare ? RareBorder : CommonBorder;
        var theme = new Theme();
        var styleBox = new StyleBoxFlat();
        styleBox.BgColor = new Color(0.15f, 0.15f, 0.15f);
        styleBox.BorderColor = borderColor;
        styleBox.BorderWidthLeft = 2;
        styleBox.BorderWidthRight = 2;
        styleBox.BorderWidthTop = 2;
        styleBox.BorderWidthBottom = 2;
        styleBox.CornerRadiusTopLeft = 4;
        styleBox.CornerRadiusTopRight = 4;
        styleBox.CornerRadiusBottomLeft = 4;
        styleBox.CornerRadiusBottomRight = 4;
        panel.AddThemeStyleboxOverride("panel", styleBox);

        var vbox = new VBoxContainer();
        vbox.Alignment = BoxContainer.AlignmentMode.Center;

        var iconRect = new TextureRect();
        iconRect.Texture = trinket.Icon;
        iconRect.CustomMinimumSize = new Vector2(32, 32);
        iconRect.StretchMode = TextureRect.StretchModeEnum.KeepCentered;
        iconRect.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
        iconRect.SizeFlagsHorizontal = Control.SizeFlags.ShrinkCenter;
        vbox.AddChild(iconRect);

        var nameLabel = new Label();
        nameLabel.Text = trinket.Name;
        nameLabel.HorizontalAlignment = HorizontalAlignment.Center;
        nameLabel.AutowrapMode = TextServer.AutowrapMode.Word;
        nameLabel.MaxLinesVisible = 2;
        vbox.AddChild(nameLabel);

        var descLabel = new Label();
        descLabel.Text = trinket.Description;
        descLabel.HorizontalAlignment = HorizontalAlignment.Center;
        descLabel.Modulate = new Color(0.8f, 0.8f, 0.8f);
        descLabel.AutowrapMode = TextServer.AutowrapMode.Word;
        descLabel.MaxLinesVisible = 2;
        vbox.AddChild(descLabel);

        var takeBtn = new Button();
        takeBtn.Text = "Take";
        takeBtn.SizeFlagsHorizontal = Control.SizeFlags.ShrinkCenter;
        var captured = trinket;
        takeBtn.Pressed += () => OnTakeTrinket(captured);
        vbox.AddChild(takeBtn);

        panel.AddChild(vbox);

        panel.MouseEntered += () =>
        {
            panel.Modulate = new Color(1.1f, 1.1f, 1.1f);
        };
        panel.MouseExited += () =>
        {
            panel.Modulate = Colors.White;
        };

        return panel;
    }

    private static List<TrinketData> LoadAllTrinkets()
    {
        var list = new List<TrinketData>();
        var dir = DirAccess.Open("res://resources/trinket_data/");
        if (dir == null) return list;

        var alreadyApplied = RunState.Instance?.AppliedTrinketIds;

        foreach (var file in dir.GetFiles())
        {
            if (!file.EndsWith(".tres") && !file.EndsWith(".res"))
                continue;
            var t = ResourceLoader.Load<TrinketData>("res://resources/trinket_data/" + file, "", ResourceLoader.CacheMode.Replace);
            if (t != null && (alreadyApplied == null || !alreadyApplied.Contains(t.Id)))
                list.Add(t);
        }

        return list;
    }

    private void OnTakeTrinket(TrinketData trinket)
    {
        var tween = CreateTween();
        tween.TweenProperty(this, "modulate", new Color(1, 1, 1, 0), 0.2f);
        tween.TweenCallback(Callable.From(() =>
        {
            RunState.Instance.ApplyTrinket(trinket);
            LevelManager.Instance.PickRandomLevel();
            UIManager.Instance.PopScreen();
            UIManager.Instance.PushScreen(UIManager.Instance.BriefingData);
        }));
    }

    private void OnSkip()
    {
        var tween = CreateTween();
        tween.TweenProperty(this, "modulate", new Color(1, 1, 1, 0), 0.2f);
        tween.TweenCallback(Callable.From(() =>
        {
            LevelManager.Instance.PickRandomLevel();
            UIManager.Instance.PopScreen();
            UIManager.Instance.PushScreen(UIManager.Instance.BriefingData);
        }));
    }
}
