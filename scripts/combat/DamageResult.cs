public readonly struct DamageResult
{
    public readonly float RequestedDamage;
    public readonly float ActualDamage;
    public readonly bool WasKilled;
    public readonly bool IsOverkill;

    public DamageResult(float requested, float actual, bool wasKilled)
    {
        RequestedDamage = requested;
        ActualDamage = actual;
        WasKilled = wasKilled;
        IsOverkill = requested > actual;
    }
}
