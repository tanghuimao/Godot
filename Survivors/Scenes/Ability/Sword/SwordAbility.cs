using Godot;

[GodotClassName("SwordAbility")]
public partial class SwordAbility : Node2D
{

    public HitBoxComponent HitBoxComponent;
    
    public override void _Ready()
    {
        HitBoxComponent = GetNode<HitBoxComponent>("HitBoxComponent");
    }
}
