using Godot;

public partial class TowerPlacementManager : Node
{
    public static TowerPlacementManager Instance { get; private set; }

    private PackedScene _selectedTowerScene;
    private Node2D _previewInstance;
    private TileMapLayer _activeTileMap;
    private int _towerCost;

    public bool IsPlacing => _selectedTowerScene != null;

    public override void _EnterTree()
    {
        Instance = this;
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
        {
            ConfirmPlacement(snappedPos);
        }

        if (Input.IsActionJustPressed("cancel_placement"))
        {
            CancelPlacement();
        }
    }

    public void StartPlacement(PackedScene towerScene, TileMapLayer tileMap)
    {
        CancelPlacement();

        _selectedTowerScene = towerScene;
        _activeTileMap = tileMap;

        _previewInstance = towerScene.Instantiate<Node2D>();
        _previewInstance.Modulate = new Color(1, 1, 1, 0.5f);
        GetTree().CurrentScene.AddChild(_previewInstance);
    }

    private bool IsCellBuildable(Vector2I cell)
    {
        var data = _activeTileMap.GetCellTileData(cell);
        if (data == null) return false;

        var tileType = data.GetCustomData("tile_type").AsString();
        return tileType == "buildable";
    }

    private void UpdatePreviewColor(bool isValid)
    {
        _previewInstance.Modulate = isValid
            ? new Color(0, 1, 0, 0.5f)
            : new Color(1, 0, 0, 0.5f);
    }

private void ConfirmPlacement(Vector2 position)
{
    if (!EconomyManager.Instance.SpendMoney(_towerCost))
    {
        return; // sem dinheiro suficiente, não coloca
    }

    var tower = _selectedTowerScene.Instantiate<Node2D>();
    tower.GlobalPosition = position;
    GetTree().CurrentScene.AddChild(tower);

    CancelPlacement();
}

    public void CancelPlacement()
    {
        if (_previewInstance != null)
        {
            _previewInstance.QueueFree();
            _previewInstance = null;
        }

        _selectedTowerScene = null;
        _activeTileMap = null;
    }
}