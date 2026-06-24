using Godot;

public partial class TowerPlacementManager : Node
{
    public static TowerPlacementManager Instance { get; private set; }

    // Loaded automatically from res://scenes/towers/Tower.tscn at startup.
    private PackedScene _genericTowerScene;

    private TowerData _selectedTowerData;
    private Node2D _previewInstance;
    private TileMapLayer _activeTileMap;

    public bool IsPlacing => _selectedTowerData != null;

    public override void _EnterTree()
    {
        Instance = this;
    }

    public override void _Ready()
    {
        _genericTowerScene = GD.Load<PackedScene>("res://scenes/towers/Tower.tscn");
        if (_genericTowerScene == null)
            GD.PrintErr("TowerPlacementManager: could not load res://scenes/towers/Tower.tscn");
    }

    public override void _Process(double delta)
    {
        if (!IsPlacing || _previewInstance == null) return;

        var mousePos = _activeTileMap.GetGlobalMousePosition();
        var cell = _activeTileMap.LocalToMap(_activeTileMap.ToLocal(mousePos));
        var snappedPos = _activeTileMap.ToGlobal(_activeTileMap.MapToLocal(cell));

        _previewInstance.GlobalPosition = snappedPos;

        bool isValid = IsCellBuildable(cell);
        UpdatePreviewColor(isValid);

        if (Input.IsActionJustPressed("place_tower") && isValid)
            ConfirmPlacement(snappedPos);

        if (Input.IsActionJustPressed("cancel_placement"))
            CancelPlacement();
    }

    public void StartPlacement(TowerData towerData, TileMapLayer tileMap)
    {
        if (_genericTowerScene == null)
        {
            GD.PrintErr("TowerPlacementManager: GenericTowerScene not loaded.");
            return;
        }

        CancelPlacement();

        _selectedTowerData = towerData;
        _activeTileMap = tileMap;

        _previewInstance = _genericTowerScene.Instantiate<Node2D>();
        _previewInstance.Modulate = new Color(1, 1, 1, 0.5f);

        if (towerData.Sprite != null)
            _previewInstance.GetNode<Sprite2D>("Sprite2D").Texture = towerData.Sprite;

        GetTree().CurrentScene.AddChild(_previewInstance);
    }

    private bool IsCellBuildable(Vector2I cell)
    {
        var data = _activeTileMap.GetCellTileData(cell);
        if (data == null) return false;

        return data.GetCustomData("tile_type").AsString() == "buildable";
    }

    private void UpdatePreviewColor(bool isValid)
    {
        _previewInstance.Modulate = isValid
            ? new Color(0, 1, 0, 0.5f)
            : new Color(1, 0, 0, 0.5f);
    }

    private void ConfirmPlacement(Vector2 position)
    {
        if (!EconomyManager.Instance.SpendMoney(_selectedTowerData.Cost))
            return;

        var tower = _genericTowerScene.Instantiate<Tower>();
        tower.GlobalPosition = position;
        GetTree().CurrentScene.AddChild(tower);
        tower.Setup(_selectedTowerData);

        CancelPlacement();
    }

    public void CancelPlacement()
    {
        if (_previewInstance != null)
        {
            _previewInstance.QueueFree();
            _previewInstance = null;
        }

        _selectedTowerData = null;
        _activeTileMap = null;
    }
}
