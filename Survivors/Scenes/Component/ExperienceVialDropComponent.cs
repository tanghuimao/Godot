using Godot;
using System;
/// <summary>
/// 经验瓶调用组件
/// </summary>
public partial class ExperienceVialDropComponent : Node
{
    //导出场景
    [Export] public PackedScene VialScene { get; set; }
    //导出组件
    [Export] public HealthComponent HealthComponent { get; set; }
    //导出范围限制值
    [Export(PropertyHint.Range, "0,1")] 
    public float DropPercent { get; set; } = 0.5f;

    private Random _random = new Random();

    public override void _Ready()
    {
        //信号链接方法 使用时去掉EventHandler
        HealthComponent.Died += OnDied;
    }
    
    //死亡信号处理方法
    private void OnDied(object sender, EventArgs e)
    {
        if (_random.NextSingle() > DropPercent)
        {
            return;
        }

        if (VialScene == null)
        {
            return;
        }

        if (Owner is not Node2D)
        {
            return;   
        }
        //获取组件拥有节点位置 生成位置
        var spawnPosition = (Owner as Node2D).GlobalPosition;
        //创建经验瓶实例
        var vialInstance = VialScene.Instantiate() as Node2D;
        //添加经验瓶到entities_layer场景中
        var entitiesLayer = GetTree().GetFirstNodeInGroup("entities_layer");
        entitiesLayer.AddChild(vialInstance);
        //设置经验瓶位置
        if (vialInstance != null) vialInstance.GlobalPosition = spawnPosition;
    }
}
