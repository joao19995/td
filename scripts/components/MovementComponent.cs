using Godot;

/// <summary>
/// Moves a Node2D parent along a Curve2D path at a configurable speed.
/// Emits ReachedEnd when the end of the path is reached.
/// Parent must be a Node2D.
/// </summary>
public partial class MovementComponent : Node
{
    [Signal] public delegate void ReachedEndEventHandler();

    private Curve2D _curve;
    private float _speed;
    private float _distanceTraveled;
    private float _pathLength;

    public override void _Ready()
    {
        if (GetParent() is not Node2D)
            GD.PushError($"MovementComponent: parent must be a Node2D (got {GetParent()?.GetClass()}).");
    }

    public void Initialize(Curve2D curve, float speed)
    {
        _curve = curve;
        _speed = speed;
        _pathLength = curve.GetBakedLength();
        _distanceTraveled = 0f;
        GameManager.Log($"[Movement] Initialize — speed={speed}, pathLen={_pathLength}, parent={GetParent()?.Name}");
        if (GetParent() is Node2D parent)
            parent.GlobalPosition = _curve.SampleBaked(0f);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_curve == null)
        {
            GameManager.Log($"[Movement] WARNING — _curve is null on {GetParent()?.Name}");
            return;
        }

        _distanceTraveled += _speed * (float)delta;

        if (_distanceTraveled >= _pathLength)
        {
            GameManager.Log($"[Movement] ReachedEnd — dist={_distanceTraveled}, pathLen={_pathLength}");
            EmitSignal(SignalName.ReachedEnd);
            return;
        }

        if (GetParent() is Node2D parent)
            parent.GlobalPosition = _curve.SampleBaked(_distanceTraveled);
        else
            GameManager.Log($"[Movement] WARNING — parent not Node2D on {GetParent()?.GetClass()}");
    }
}
