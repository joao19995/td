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
    }

    private void OnSpinPressed()
    {
        _spinButton.Disabled = true;

        if (_state == State.Resolve)
        {
            ResolveOutcome();
            return;
        }

        _state = State.Resolve;
        SlotManager.Instance.ResetRerolls();
        RunState.Instance.IncrementFights();
        RunState.Instance.SetMiniboss(false);

        if (RunState.Instance.FightsCompleted >= SlotManager.Instance.FightsPerRun)
        {
            ShowOutcome("BOSS FIGHT!");
            _spinButton.Text = "Continue";
            _spinButton.Disabled = false;
            RunState.Instance.SetBossFight(true);
            _pendingOutcome = SlotOutcome.Boss;
            return;
        }

        DoSpin();
    }

    private void DoSpin()
    {
        _pendingOutcome = SlotManager.Instance.Spin();

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
        DoSpin();
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
    }

    private void ResolveOutcome()
    {
        UIManager.Instance.PopScreen();

        switch (_pendingOutcome)
        {
            case SlotOutcome.Shop:
                UIManager.Instance.PushScreen(UIManager.Instance.ShopData);
                break;

            case SlotOutcome.Treasure:
                UIManager.Instance.PushScreen(UIManager.Instance.TrinketChoiceData);
                break;

            default:
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
