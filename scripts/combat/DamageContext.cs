public readonly struct DamageContext
{
    public readonly float Amount;
    public readonly string SourceTowerId;
    public readonly DamageType Type;
    public readonly bool WasCrit;
    public readonly bool WasExecute;

    public DamageContext(float amount, string sourceTowerId, DamageType type, bool wasCrit = false, bool wasExecute = false)
    {
        Amount = amount;
        SourceTowerId = sourceTowerId;
        Type = type;
        WasCrit = wasCrit;
        WasExecute = wasExecute;
    }
}
