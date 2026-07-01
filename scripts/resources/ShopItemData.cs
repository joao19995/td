using Godot;

[GlobalClass]
public partial class ShopItemData : Resource
{
    [Export] public string ItemId { get; set; } = "";
    [Export] public string ItemName { get; set; } = "";
    [Export] public string Description { get; set; } = "";
    [Export] public int Cost { get; set; } = 0;
    [Export] public float DamageBonusPercent { get; set; } = 0f;
    [Export] public float FireRateBonusPercent { get; set; } = 0f;
    [Export] public float RangeBonusPercent { get; set; } = 0f;
    [Export] public float HeavyDamageBonusPercent { get; set; } = 0f;
    [Export] public float FirstPurchaseDiscountPercent { get; set; } = 0f;

    [Export] public Texture2D Icon { get; set; }
}
