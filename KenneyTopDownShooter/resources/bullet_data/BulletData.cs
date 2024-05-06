using Godot;

namespace KenneyTopDownShooter.resources.bullet_data;
/// <summary>
/// 子弹数据
/// </summary>
public partial class BulletData : Resource
{
    [Export] public float Speed { get; set; } = 10;
    [Export] public int Damage { get; set; } = 10;
}