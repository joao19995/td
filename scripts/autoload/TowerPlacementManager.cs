using Godot;

public partial class TowerPlacementManager : Node
{
    public static TowerPlacementManager Instance { get; private set; }

    [Export] public PackedScene GenericTowerScene;

    private TowerData _selectedTowerData;
    private Node2D _previewInstance;
    private TileMapLayer _activeTileMap;
    private readonly System.Collections.Generic.HashSet<Vector2I> _occupiedCells = new();
    private readonly System.Collections.Generic.HashSet<string> _placedTowerIds = new();
    public System.Collections.Generic.IEnumerable<string> PlacedTowerIds => _placedTowerIds;
    public bool IsPlacing => _selectedTowerData != null;

    public override void _EnterTree()
    {
        Instance = this;
    }

    public override void _Ready()
    {
        SceneManager.Instance.LevelLoaded += OnLevelLoaded;
    }

    public override void _ExitTree()
    {
        if (SceneManager.Instance != null)
            SceneManager.Instance.LevelLoaded -= OnLevelLoaded;
    }

    public bool IsTowerTypePlaced(string towerId) => _placedTowerIds.Contains(towerId);

    private void OnLevelLoaded(Node _)
    {
        _occupiedCells.Clear();
        _placedTowerIds.Clear();
        CancelPlacement();
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
            ConfirmPlacement(cell, snappedPos);

        if (Input.IsActionJustPressed("cancel_placement"))
            CancelPlacement();
    }

    public void StartPlacement(TowerData towerData, TileMapLayer tileMap)
    {
        if (GenericTowerScene == null)
        {
            GD.PrintErr("TowerPlacementManager: GenericTowerScene not assigned.");
            return;
        }

        if (_placedTowerIds.Contains(towerData.Id)) return;

        if (TowerSelectionManager.Instance != null)
            TowerSelectionManager.Instance.Deselect();

        CancelPlacement();

        _selectedTowerData = towerData;
        _activeTileMap = tileMap;

        _previewInstance = GenericTowerScene.Instantiate<Node2D>();
        _previewInstance.Modulate = new Color(1, 1, 1, 0.5f);

        if (towerData.Sprite != null)
            _previewInstance.GetNode<Sprite2D>("Sprite2D").Texture = towerData.Sprite;

        GetTowersContainer().AddChild(_previewInstance);
    }

    private bool IsCellBuildable(Vector2I cell)
    {
        if (_occupiedCells.Contains(cell)) return false;

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

    private void ConfirmPlacement(Vector2I cell, Vector2 position)
    {
        if (!EconomyManager.Instance.SpendMoney(_selectedTowerData.Cost))
            return;

        _occupiedCells.Add(cell);
        _placedTowerIds.Add(_selectedTowerData.Id);

        EventBus.Instance?.EmitSignal(EventBus.SignalName.TowerPlaced, _selectedTowerData.Cost);

        var tower = TowerFactory.Create(GenericTowerScene, _selectedTowerData, position);
        var towerId = _selectedTowerData.Id;
        tower.TreeExited += () => { _occupiedCells.Remove(cell); _placedTowerIds.Remove(towerId); SynergyManager.Instance?.OnTowerRemoved(); };
        GetTowersContainer().AddChild(tower);

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

    private Node GetTowersContainer()
    {
        if (LevelManager.Instance.CurrentLevelNode is BaseLevel level && level.TowersContainer != null)
            return level.TowersContainer;

        GD.PushWarning("TowerPlacementManager: TowersContainer not found, falling back to level root.");
        return LevelManager.Instance.CurrentLevelNode;
    }
}
