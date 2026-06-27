using Godot;

[GlobalClass]
public partial class LevelData : Resource
{
    [Export] public string LevelName { get; set; } = "Level";
    [Export] public string ScenePath { get; set; } = "";
    [Export] public Texture2D PreviewTexture { get; set; }

    // -1 means "use the global default set on the manager"
    [Export] public int StartingMoney { get; set; } = -1;
    [Export] public int StartingLives { get; set; } = -1;
}
