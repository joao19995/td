using Godot;
using System.Collections.Generic;
using System.Text;

public partial class BriefingScreen : Control
{
    [Export] private NodePath _mapPreviewPath = new NodePath("VBox/TopRow/MapPreview");
    [Export] private NodePath _titleLabelPath = new NodePath("VBox/TopRow/InfoColumn/TitleLabel");
    [Export] private NodePath _goldLabelPath = new NodePath("VBox/TopRow/InfoColumn/GoldLabel");
    [Export] private NodePath _tierLabelPath = new NodePath("VBox/TopRow/InfoColumn/TierLabel");
    [Export] private NodePath _waveListLabelPath = new NodePath("VBox/WaveScroll/WaveListLabel");
    [Export] private NodePath _loadoutIconsPath = new NodePath("VBox/LoadoutRow/LoadoutIcons");
    [Export] private NodePath _loadoutRowPath = new NodePath("VBox/LoadoutRow");
    [Export] private NodePath _synergyLabelPath = new NodePath("VBox/SynergyLabel");
    [Export] private NodePath _minibossLabelPath = new NodePath("VBox/MinibossLabel");
    [Export] private NodePath _startButtonPath = new NodePath("VBox/StartButton");

    private TextureRect _mapPreview;
    private Label _titleLabel;
    private Label _goldLabel;
    private Label _tierLabel;
    private Label _waveListLabel;
    private HBoxContainer _loadoutIcons;
    private HBoxContainer _loadoutRow;
    private Label _synergyLabel;
    private Label _minibossLabel;
    private Button _startButton;

    private Dictionary<string, TowerData> _towerDataCache;

    public override void _Ready()
    {
        _mapPreview = GetNode<TextureRect>(_mapPreviewPath);
        _titleLabel = GetNode<Label>(_titleLabelPath);
        _goldLabel = GetNode<Label>(_goldLabelPath);
        _tierLabel = GetNode<Label>(_tierLabelPath);
        _waveListLabel = GetNode<Label>(_waveListLabelPath);
        _loadoutIcons = GetNode<HBoxContainer>(_loadoutIconsPath);
        _loadoutRow = GetNode<HBoxContainer>(_loadoutRowPath);
        _synergyLabel = GetNode<Label>(_synergyLabelPath);
        _minibossLabel = GetNode<Label>(_minibossLabelPath);
        _startButton = GetNode<Button>(_startButtonPath);

        var levelData = LevelManager.Instance.PendingLevelData;
        bool hasLevelData = levelData != null;

        if (hasLevelData && levelData.PreviewTexture != null)
            _mapPreview.Texture = levelData.PreviewTexture;
        else
            _mapPreview.Visible = false;

        string actSuffix = RunState.Instance.SelectedAct != null
            ? $" — {RunState.Instance.SelectedAct.ActName.ToUpper()}"
            : "";
        string title = RunState.Instance.IsBossFight
            ? $"BOSS FIGHT!{actSuffix}"
            : hasLevelData ? $"{levelData.LevelName.ToUpper()}{actSuffix}" : "MISSION";
        _titleLabel.Text = title;

        _goldLabel.Text = $"Gold: {EconomyManager.Instance.CurrentMoney}  Lives: {GameManager.Instance.CurrentLives}";

        PopulateTierLabel();
        PopulateMinibossLabel();
        PopulateWaveList(hasLevelData ? levelData : null);
        PopulateLoadout();
        PopulateSynergies();

        _startButton.Pressed += () =>
        {
            GD.Print($"[Briefing] START pressed — popping briefing, loading level '{LevelManager.Instance.PendingLevelData?.LevelName}'.");
            GD.Print($"[Briefing] IsBossFight={RunState.Instance.IsBossFight}, IsMiniboss={RunState.Instance.IsMiniboss}, FightsCompleted={RunState.Instance.FightsCompleted}.");
            UIManager.Instance.PopScreen();
            LevelManager.Instance.LoadPendingLevel();
        };

        Modulate = new Color(1, 1, 1, 0);
        var tween = CreateTween();
        tween.TweenProperty(this, "modulate", Colors.White, 0.3f);
        tween.TweenCallback(Callable.From(() =>
        {
            _startButton.Text = "START!";
            var pulse = CreateTween().SetLoops();
            pulse.TweenProperty(_startButton, "modulate", new Color(1, 1, 0.5f), 0.3f);
            pulse.TweenProperty(_startButton, "modulate", Colors.White, 0.3f);
        }));
    }

    private void PopulateTierLabel()
    {
        if (RunState.Instance.IsRunActive && !RunState.Instance.IsBossFight)
        {
            string tier = WaveGenerator.GetWaveTier(RunState.Instance.FightsCompleted, RunState.Instance.EffectiveFightsPerRun);
            string tierName = tier switch
            {
                "tier1" => "Tier 1",
                "tier2" => "Tier 2",
                "tier3" => "Tier 3",
                _ => tier
            };
            Color tierColor = tier switch
            {
                "tier1" => Colors.Green,
                "tier2" => Colors.Yellow,
                "tier3" => Colors.Red,
                _ => Colors.White
            };
            _tierLabel.Text = tierName;
            _tierLabel.Modulate = tierColor;
        }
        else
        {
            _tierLabel.Visible = false;
        }
    }

    private void PopulateMinibossLabel()
    {
        _minibossLabel.Visible = RunState.Instance.IsMiniboss;
        if (RunState.Instance.IsMiniboss)
            _minibossLabel.Text = "MINIBOSS";
    }

    private void PopulateWaveList(LevelData levelData)
    {
        var sb = new StringBuilder();

        if (RunState.Instance.IsBossFight)
        {
            var bossWave = RunState.Instance.SelectedAct?.BossWaveData ?? LevelManager.Instance.BossWaveData;
            if (bossWave?.Entries != null && bossWave.Entries.Count > 0)
            {
                var parts = new List<string>();
                foreach (var entry in bossWave.Entries)
                {
                    if (entry?.Enemy == null) continue;
                    string name = entry.Enemy.EnemyName;
                    if (string.IsNullOrEmpty(name) || name == "Enemy")
                        name = entry.Enemy.Id.Replace("_", " ").ToUpper();
                    parts.Add($"{entry.Count}x {name}");
                }
                sb.AppendLine(string.Join(", ", parts));
            }
            else
            {
                sb.AppendLine("Defeat the boss to win!");
            }
        }
        else
        {
            var waves = LevelManager.Instance.PendingRunWaves ?? levelData?.Waves;
            if (waves != null)
            {
                sb.AppendLine($"{waves.Count} waves");

                int waveNum = 1;
                foreach (var wave in waves)
                {
                    if (wave?.Entries == null || wave.Entries.Count == 0) continue;

                    var parts = new List<string>();
                    foreach (var entry in wave.Entries)
                    {
                        if (entry?.Enemy == null) continue;
                        string name = entry.Enemy.EnemyName;
                        if (string.IsNullOrEmpty(name) || name == "Enemy")
                            name = entry.Enemy.Id.Replace("_", " ").ToUpper();
                        parts.Add($"{entry.Count}x {name}");
                    }

                    string modTag = wave.Modifier != WaveModifier.None
                        ? $" · {wave.Modifier}"
                        : "";
                    sb.AppendLine($"W{waveNum}{modTag}: {string.Join(" ", parts)}");
                    waveNum++;
                }
            }
        }

        _waveListLabel.Text = sb.ToString().TrimEnd();
    }

    private void PopulateLoadout()
    {
        var selectedIds = RunState.Instance.SelectedTowerIds;
        if (selectedIds == null || selectedIds.Count == 0)
        {
            _loadoutRow.Visible = false;
            return;
        }

        LoadTowerCache();

        foreach (var id in selectedIds)
        {
            if (_towerDataCache.TryGetValue(id, out var data) && data.Sprite != null)
            {
                var icon = new TextureRect();
                icon.Texture = data.Sprite;
                icon.CustomMinimumSize = new Vector2(16, 16);
                icon.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;
                _loadoutIcons.AddChild(icon);
            }
            else
            {
                var label = new Label();
                label.Text = id.Length > 0 ? id[0].ToString().ToUpper() : "?";
                _loadoutIcons.AddChild(label);
            }
        }
    }

    private void PopulateSynergies()
    {
        var selectedIds = RunState.Instance.SelectedTowerIds;
        if (selectedIds == null || selectedIds.Count == 0)
        {
            _synergyLabel.Visible = false;
            return;
        }

        var active = GetPreviewSynergies(new List<string>(selectedIds));
        if (active.Count == 0)
        {
            _synergyLabel.Visible = false;
            return;
        }

        var names = new List<string>();
        foreach (var s in active)
            names.Add($"{s.DisplayName} (+{s.DamageBonusPercent * 100f:F0}% DMG)");
        _synergyLabel.Text = "Synergies: " + string.Join(", ", names);
    }

    private static List<SynergyData> GetPreviewSynergies(List<string> towerIds)
    {
        return SynergyPreviewHelper.GetPreviewSynergies(towerIds);
    }

    private void LoadTowerCache()
    {
        if (_towerDataCache != null) return;
        _towerDataCache = new Dictionary<string, TowerData>();

        var all = ResourceLoaderHelper.LoadFromDir<TowerData>("res://resources/tower_data/");
        foreach (var data in all)
        {
            if (!string.IsNullOrEmpty(data.Id))
                _towerDataCache[data.Id] = data;
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("pause_game"))
        {
            GetViewport().SetInputAsHandled();
            UIManager.Instance?.PopScreen();
        }
    }
}
