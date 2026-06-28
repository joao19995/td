using Godot;

[GlobalClass]
public partial class UIScreenData : Resource
{
    [Export] public string Id { get; set; } = "";
    [Export] public string ScenePath { get; set; } = "";
    [Export] public bool PauseGame { get; set; } = false;
}
