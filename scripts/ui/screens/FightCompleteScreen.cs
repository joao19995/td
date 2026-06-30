using Godot;

public partial class FightCompleteScreen : Control
{
    private Label _outcomeLabel;
    private Button _spinButton;
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

        _goldLabel.Text = $"Gold: {EconomyManager.Instance.CurrentMoney}";
        _livesLabel.Text = $"Lives: {GameManager.Instance.CurrentLives}";

        _spinButton.Pressed += OnSpinPressed;
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

        _pendingOutcome = SlotManager.Instance.Spin();

        switch (_pendingOutcome)
        {
            case "Fight":
                ShowOutcome("Next: FIGHT");
                _spinButton.Text = "Continue";
                _spinButton.Disabled = false;
                break;

            case "Shop":
                ShowOutcome("Next: SHOP");
                _spinButton.Text = "Continue";
                _spinButton.Disabled = false;
                break;

            case "Heal":
                GameManager.Instance.Heal(SlotManager.Instance.HealAmount);
                _livesLabel.Text = $"Lives: {GameManager.Instance.CurrentLives}";
                ShowOutcome($"Healed! +{SlotManager.Instance.HealAmount} HP");
                _spinButton.Text = "Next Fight";
                _spinButton.Disabled = false;
                break;

            case "Miniboss":
                ShowOutcome("MINIBOSS!");
                _spinButton.Text = "Continue";
                _spinButton.Disabled = false;
                RunState.Instance.SetMiniboss(true);
                break;
        }
    }

    private void ResolveOutcome()
    {
        UIManager.Instance.PopScreen();

        if (_pendingOutcome == "Shop")
        {
            UIManager.Instance.PushScreen(UIManager.Instance.ShopData);
        }
        else
        {
            LevelManager.Instance.LoadRandomLevel();
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
