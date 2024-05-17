using Godot;
/// <summary>
/// 背包物品
/// </summary>
[GlobalClass]
[GodotClassName("InventoryItem")]
public partial class InventoryItem : Resource
{
    [Export] public int Id;
    [Export] public string Name;
    [Export] public Texture2D Texture;
}