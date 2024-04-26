using Godot;

/// <summary>
/// 经验瓶
/// </summary>
public partial class ExperienceVial : Node2D
{
    private CollisionShape2D _collisionShape2D;
    private Sprite2D _sprite2D;
    
    public override void _Ready()
    {
        _collisionShape2D = GetNode<CollisionShape2D>("Area2D/CollisionShape2D");
        _sprite2D = GetNode<Sprite2D>("Sprite2D");
        
        //区域进入信号链接方法
        GetNode<Area2D>("Area2D").AreaEntered += OnAreaEntered;

    }

    //检测到区域进入使用方法
    private void OnAreaEntered(Area2D area2D)
    {
        //延迟调用
        Callable.From(DisableCollision).CallDeferred();
        //创建经验瓶播放动画
        var tween = CreateTween();
        //同时执行 
        tween.SetParallel();
        //绑定方法 绑定参数
        tween.TweenMethod(Callable.From(() => TweenConnect(0, GlobalPosition)), 0f, 1f, 0.5f)
            .SetEase(Tween.EaseType.In) //设置缓冲
            .SetTrans(Tween.TransitionType.Back); //设置过渡
        //设置对象属性 设置延迟执行
        tween.TweenProperty(_sprite2D, "scale", Vector2.Zero, 0.05).SetDelay(0.45);
        //串联执行
        tween.Chain();
        //设置回调方法
        tween.TweenCallback(Callable.From(Collect));
    }
    
    //经验瓶动画
    private void TweenConnect(float percent, Vector2 startPosition)
    {
        //获取玩家
        var player = GetTree().GetFirstNodeInGroup("player") as Node2D;
        if (player == null)
        {
            return;
        }
        //修改经验小瓶位置
        GlobalPosition = startPosition.Lerp(player.GlobalPosition, percent);
        //设置旋转
        var directFromStart = GlobalPosition - startPosition;
        //目标旋转角度
        var targetRotation = directFromStart.Angle() + Mathf.DegToRad(90);
        //改变旋转
        Rotation = (float)Mathf.LerpAngle(Rotation, targetRotation, 1 - Mathf.Exp(2 * GetProcessDeltaTime()));
    }
    
    //停止碰撞检测
    private void DisableCollision()
    {
        _collisionShape2D.Disabled = true;
    }
    
    //收集经验瓶
    private void Collect()
    {
        //通知全局事件
        GameEvents.OnExperienceVialCollected(this, 1);
        //移除当前节点
        QueueFree();
    }
}
