using System;
using System.Linq;
using Godot;
using Godot.Collections;

public partial class SwordAbilityController : Node2D
{
    //导入能力场景
    [Export] public PackedScene SwordAbility;
    //定时器
    private Timer _timer;
    //范围
    private const double MaxRange = 100;
    //伤害值
    private double _damage = 5D;
    //攻击间隔时间
    private double _baseWaitTime;

    public override void _Ready()
    {
        //定时器
        _timer = GetNode<Timer>("Timer");
        //获取定时器节点超时时间
        _baseWaitTime = _timer.WaitTime;
        //定时器超时连接方法
        _timer.Timeout += OnTimerTimeout;
        //全局事件链接方法
        GameEvents.AbilityUpgradeAdded += OnAbilityUpgradeAdded;

    }

    private void OnTimerTimeout(){
        if (GetTree().GetFirstNodeInGroup("player") is not Node2D player) return;
        //获取敌人
        Array<Node> enemies = GetTree().GetNodesInGroup("enemy");
        //过滤一定范围内敌人
        enemies = new Array<Node>(enemies.Where(enemy =>
        {
            var node2D = enemy as Node2D;
            return node2D!.GlobalPosition.DistanceSquaredTo(player.GlobalPosition) < Mathf.Pow(MaxRange, 2D);
        }).ToArray());
        
        if (enemies.Count == 0) return;
        
        //选择距离玩家最近敌人 按照和玩家距离排序
        enemies = new Array<Node>(enemies.OrderBy(enemy =>
        {
            var node2D = enemy as Node2D;
            return node2D!.GlobalPosition.DistanceSquaredTo(player.GlobalPosition);
        }).ToArray());

        var firstEnemy = enemies.FirstOrDefault() as Node2D;
        if (firstEnemy == null) return;

        //创建一个剑实例  转换类型 为SwordAbility
        if (SwordAbility.Instantiate() is not SwordAbility swordInstance) return;
        //将节点添加foreground_layer
        var foregroundLayer = GetTree().GetFirstNodeInGroup("foreground_layer");
        foregroundLayer.AddChild(swordInstance);
        //传递伤害值
        swordInstance.HitBoxComponent.Damage = _damage;
        //设置剑出现位置
        swordInstance.GlobalPosition = firstEnemy.GlobalPosition;
        //设置剑的旋转角度
        var enemyDirection = player.GlobalPosition - firstEnemy.GlobalPosition;
        //angle 返回向量夹角
        swordInstance.Rotation = enemyDirection.Angle();
    }

    //处理能力叠加
    private void OnAbilityUpgradeAdded(object sender, EventArgs e)
    {
        GD.Print("111111");
    }
}