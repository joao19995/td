using Godot;

public partial class FightCompleteScreen : Control
{
    private Label _outcomeLabel;
    private Button _spinButton;
    private Button _rerollButton;
    private Label _goldLabel;
    private Label _livesLabel;

    private enum State { Spin, Resolve }
    private State _state = State.Spin;
    private string _pendingOutcome;

    public override void _Ready()
    {
        _goldLabel = GetNode<Label>("VBox/GoldLabel");
        _livesLabel = GetNode<Label>("VBox/LivesLabel");
        _outcomeLabel = GetNode<Label>("VBox/OutcomeLabel");
        _spinButton = GetNode<Button>("VBox/SpinButton");
        _rerollButton = GetNode<Button>("VBox/RerollButton");

        _goldLabel.Text = $"Gold: {EconomyManager.Instance.CurrentMoney}";
        _livesLabel.Text = $"Lives: {GameManager.Instance.CurrentLives}";

        _spinButton.Pressed += OnSpinPressed;
        _rerollButton.Pressed += OnRerollPressed;
        _rerollButton.Visible = false;
        GetNode<Button>("VBox/EndRunButton").Pressed += OnEndRunPressed;
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
            _pendingOutcome = "Boss";
            return;
        }

        DoSpin();
    }

    private void DoSpin()
    {
        _pendingOutcome = SlotManager.Instance.Spin();

        switch (_pendingOutcome)
        {
            case "Fight":
                ShowOutcome("Next: FIGHT");
                break;

            case "Shop":
                ShowOutcome("Next: SHOP");
                break;

            case "Treasure":
                ShowOutcome("TREASURE!");
                break;

            case "Heal":
                GameManager.Instance.Heal(SlotManager.Instance.HealAmount);
                _livesLabel.Text = $"Lives: {GameManager.Instance.CurrentLives}";
                ShowOutcome($"Healed! +{SlotManager.Instance.HealAmount} HP");
                _spinButton.Text = "Next Fight";
                _spinButton.Disabled = false;
                return;

            case "Miniboss":
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
        if (_pendingOutcome == "Boss" || _pendingOutcome == "Heal")
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
            case "Shop":
                UIManager.Instance.PushScreen(UIManager.Instance.ShopData);
                break;

            case "Treasure":
                UIManager.Instance.PushScreen(UIManager.Instance.TrinketChoiceData);
                break;

            default:
                LevelManager.Instance.LoadRandomLevel();
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
        RunState.Instance.EndRun();
        UIManager.Instance.PopScreen();
        UIManager.Instance.PopAll();
        SceneManager.Instance.LoadLevel("res://scenes/ui/screens/MainMenu.tscn",
            LevelManager.Instance.LevelContainer);
    }
}
