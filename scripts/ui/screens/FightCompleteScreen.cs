using Godot;

public partial class FightCompleteScreen : Control
{
    [Export] private NodePath _outcomeLabelPath = new NodePath("VBox/OutcomeLabel");
    [Export] private NodePath _spinButtonPath = new NodePath("VBox/SpinButton");
    [Export] private NodePath _rerollButtonPath = new NodePath("VBox/RerollButton");
    [Export] private NodePath _goldLabelPath = new NodePath("VBox/GoldLabel");
    [Export] private NodePath _livesLabelPath = new NodePath("VBox/LivesLabel");
    [Export] private NodePath _endRunButtonPath = new NodePath("VBox/EndRunButton");

    private Label _outcomeLabel;
    private Button _spinButton;
    private Button _rerollButton;
    private Label _goldLabel;
    private Label _livesLabel;
    private Button _endRunButton;

    private enum State { Spin, Resolve }
    private State _state = State.Spin;
    private SlotOutcome _pendingOutcome;

    public override void _Ready()
    {
        _outcomeLabel = GetNode<Label>(_outcomeLabelPath);
        _spinButton = GetNode<Button>(_spinButtonPath);
        _rerollButton = GetNode<Button>(_rerollButtonPath);
        _goldLabel = GetNode<Label>(_goldLabelPath);
        _livesLabel = GetNode<Label>(_livesLabelPath);
        _endRunButton = GetNode<Button>(_endRunButtonPath);

        _goldLabel.Text = $"Gold: {EconomyManager.Instance.CurrentMoney}";
        _livesLabel.Text = $"Lives: {GameManager.Instance.CurrentLives}";

        _spinButton.Pressed += OnSpinPressed;
        _rerollButton.Pressed += OnRerollPressed;
        _rerollButton.Visible = false;
        _endRunButton.Pressed += OnEndRunPressed;

        RunState.Instance?.SaveCurrentRun();
    }

    private void OnSpinPressed()
    {
        _spinButton.Disabled = true;

        if (_state == State.Resolve)
        {
            GD.Print($"[FightComplete] Resolving outcome: {_pendingOutcome}.");
            ResolveOutcome();
            return;
        }

        _state = State.Resolve;
        SlotManager.Instance.ResetRerolls();
        RunState.Instance.IncrementFights();
        RunState.Instance.SetMiniboss(false);

        if (RunState.Instance.FightsCompleted >= SlotManager.Instance.FightsPerRun)
        {
            GD.Print($"[FightComplete] FightsCompleted={RunState.Instance.FightsCompleted} >= FightsPerRun={SlotManager.Instance.FightsPerRun} — forcing BOSS.");
            ShowOutcome("BOSS FIGHT!");
            _spinButton.Text = "Continue";
            _spinButton.Disabled = false;
            RunState.Instance.SetBossFight(true);
            _pendingOutcome = SlotOutcome.Boss;
            return;
        }

        var result = SlotManager.Instance.Spin();
        GD.Print($"[FightComplete] Slot result: {result} (FightsCompleted={RunState.Instance.FightsCompleted}).");
        ShowOutcome(result);
    }

    private void ShowOutcome(SlotOutcome outcome)
    {
        _pendingOutcome = outcome;

        switch (_pendingOutcome)
        {
            case SlotOutcome.Fight:
                ShowOutcome("Next: FIGHT");
                break;

            case SlotOutcome.Shop:
                ShowOutcome("Next: SHOP");
                break;

            case SlotOutcome.Treasure:
                ShowOutcome("TREASURE!");
                break;

            case SlotOutcome.Heal:
                GameManager.Instance.Heal(SlotManager.Instance.HealAmount);
                _livesLabel.Text = $"Lives: {GameManager.Instance.CurrentLives}";
                ShowOutcome($"Healed! +{SlotManager.Instance.HealAmount} HP");
                _spinButton.Text = "Next Fight";
                _spinButton.Disabled = false;
                return;

            case SlotOutcome.Miniboss:
                ShowOutcome("MINIBOSS!");
                RunState.Instance.SetMiniboss(true);
                break;
        }

        _spinButton.Text = "Continue";
        _spinButton.Disabled = false;

        UpdateRerollButton();
    }

    private void OnRerollPressed()
    {
        int cost = SlotManager.Instance.GetRerollCost();
        if (!EconomyManager.Instance.CanAfford(cost)) return;

        EconomyManager.Instance.SpendMoney(cost);
        _goldLabel.Text = $"Gold: {EconomyManager.Instance.CurrentMoney}";

        SlotManager.Instance.ApplySkew(_pendingOutcome);
        ShowOutcome(SlotManager.Instance.Reroll());
    }

    private void UpdateRerollButton()
    {
        if (_pendingOutcome == SlotOutcome.Boss || _pendingOutcome == SlotOutcome.Heal)
        {
            _rerollButton.Visible = false;
            return;
        }

        int cost = SlotManager.Instance.GetRerollCost();
        _rerollButton.Text = $"Reroll ({cost}g)";
        _rerollButton.Disabled = !EconomyManager.Instance.CanAfford(cost);
        _rerollButton.Visible = true;

        _rerollButton.MouseEntered += () =>
        {
            if (_rerollButton.Disabled && !EconomyManager.Instance.CanAfford(cost))
                ShowTooltip("Not enough gold to reroll");
        };
        _rerollButton.MouseExited += HideTooltip;
    }

    private void ShowTooltip(string text)
    {
        var tooltip = GetNodeOrNull<Label>("TooltipLabel");
        if (tooltip == null)
        {
            tooltip = new Label();
            tooltip.Name = "TooltipLabel";
            tooltip.Visible = false;
            tooltip.MouseFilter = Control.MouseFilterEnum.Ignore;
            tooltip.Modulate = new Color(0.95f, 0.95f, 0.85f);
            tooltip.AutowrapMode = TextServer.AutowrapMode.Word;
            tooltip.MaxLinesVisible = 3;
            tooltip.CustomMinimumSize = new Vector2(60, 14);
            var tooltipBg = new StyleBoxFlat();
            tooltipBg.BgColor = new Color(0.05f, 0.05f, 0.08f, 0.92f);
            tooltipBg.BorderColor = new Color(0.6f, 0.6f, 0.6f);
            tooltipBg.BorderWidthLeft = 1;
            tooltipBg.BorderWidthRight = 1;
            tooltipBg.BorderWidthTop = 1;
            tooltipBg.BorderWidthBottom = 1;
            tooltipBg.ContentMarginLeft = 4;
            tooltipBg.ContentMarginRight = 4;
            tooltipBg.ContentMarginTop = 2;
            tooltipBg.ContentMarginBottom = 2;
            tooltip.AddThemeStyleboxOverride("normal", tooltipBg);
            AddChild(tooltip);
        }
        float mouseX = GetViewport().GetMousePosition().X;
        float mouseY = GetViewport().GetMousePosition().Y;
        tooltip.OffsetLeft = Mathf.Clamp(mouseX + 8, 2, 300);
        tooltip.OffsetTop = Mathf.Clamp(mouseY - tooltip.Size.Y - 4, 2, 170);
        tooltip.Text = text;
        tooltip.Show();
    }

    private void HideTooltip()
    {
        var tooltip = GetNodeOrNull<Label>("TooltipLabel");
        if (tooltip != null)
            tooltip.Hide();
    }

    private void ResolveOutcome()
    {
        RunState.Instance?.SaveCurrentRun();
        UIManager.Instance.PopScreen();

        switch (_pendingOutcome)
        {
            case SlotOutcome.Shop:
                GD.Print("[FightComplete] Outcome: Shop — pushing ShopScreen.");
                UIManager.Instance.PushScreen(UIManager.Instance.ShopData);
                break;

            case SlotOutcome.Treasure:
                GD.Print("[FightComplete] Outcome: Treasure — pushing TrinketChoiceScreen.");
                UIManager.Instance.PushScreen(UIManager.Instance.TrinketChoiceData);
                break;

            default:
                string pendingName = LevelManager.Instance.PendingLevelData?.LevelName ?? "unknown";
                GD.Print($"[FightComplete] Outcome: {_pendingOutcome} — picking random level and pushing BriefingScreen (level='{pendingName}').");
                LevelManager.Instance.PickRandomLevel();
                UIManager.Instance.PushScreen(UIManager.Instance.BriefingData);
                break;
        }
    }

    private void ShowOutcome(string text)
    {
        _outcomeLabel.Text = text;
        _outcomeLabel.Visible = true;
    }

    private static void OnEndRunPressed()
    {
        UIManager.NavigateToMainMenu();
    }
}
