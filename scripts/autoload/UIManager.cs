using Godot;
using System.Collections.Generic;

public partial class UIManager : Node
{
    public static UIManager Instance { get; private set; }

    [Export] public UIScreenData PauseData;
    [Export] public UIScreenData GameOverData;
    [Export] public UIScreenData VictoryData;
    [Export] public UIScreenData FightCompleteData;
    [Export] public UIScreenData ShopData;
    [Export] public UIScreenData MetaShopData;

    private readonly List<(UIScreenData data, Node instance)> _stack = new();
    private CanvasLayer _overlayLayer;

    public override void _EnterTree()
    {
        Instance = this;
    }

    public override void _Ready()
    {
        _overlayLayer = new CanvasLayer { Layer = 128 };
        AddChild(_overlayLayer);

        EventBus.Instance.GameOver += OnGameOver;
        EventBus.Instance.AllLevelsCompleted += OnAllLevelsCompleted;
        EventBus.Instance.AllWavesCompleted += OnAllWavesCompleted;
    }

    public override void _ExitTree()
    {
        if (EventBus.Instance != null)
        {
            EventBus.Instance.GameOver -= OnGameOver;
            EventBus.Instance.AllLevelsCompleted -= OnAllLevelsCompleted;
            EventBus.Instance.AllWavesCompleted -= OnAllWavesCompleted;
        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (!@event.IsActionPressed("pause_game")) return;
        if (LevelManager.Instance?.CurrentLevelNode == null) return;

        if (_stack.Count > 0 && _stack[^1].data == PauseData)
            PopScreen();
        else
            PushScreen(PauseData);
    }

    public void PushScreen(UIScreenData data)
    {
        if (data == null || string.IsNullOrEmpty(data.ScenePath))
        {
            GD.PrintErr("UIManager: cannot push null or empty screen.");
            return;
        }

        var scene = GD.Load<PackedScene>(data.ScenePath);
        if (scene == null)
        {
            GD.PrintErr($"UIManager: failed to load screen scene at '{data.ScenePath}'.");
            return;
        }

        var instance = scene.Instantiate();
        _overlayLayer.AddChild(instance);
        _stack.Add((data, instance));

        if (data.PauseGame)
            GetTree().Paused = true;
    }

    public void PopScreen()
    {
        if (_stack.Count == 0) return;

        var (data, instance) = _stack[^1];
        _stack.RemoveAt(_stack.Count - 1);
        instance.QueueFree();

        if (data.PauseGame && !_stack.Exists(p => p.data.PauseGame))
            GetTree().Paused = false;
    }

    public void PopAll()
    {
        while (_stack.Count > 0)
            PopScreen();
    }

    private void OnGameOver() => PushScreen(GameOverData);
    private void OnAllLevelsCompleted() => PushScreen(VictoryData);

    private void OnAllWavesCompleted()
    {
        if (!RunState.Instance.IsRunActive) return;

        if (RunState.Instance.IsBossFight)
        {
            RunState.Instance.EndRun();
            EventBus.Instance.EmitSignal(EventBus.SignalName.RunCompleted);
            PushScreen(VictoryData);
            return;
        }

        PushScreen(FightCompleteData);
    }
}
