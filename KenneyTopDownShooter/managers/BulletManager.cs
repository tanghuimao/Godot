using Godot;
using System;
/// <summary>
/// 子弹管理器
/// </summary>
public partial class BulletManager : Node
{
    //生成子弹
    public void SpawnBullet(BulletArgs args)
    {
        //添加子节点
        AddChild(args.Bullet);
        //设置位置
        args.Bullet.GlobalPosition = args.Position;
        //设置方向
        args.Bullet.SetDirection(args.Direction);
    }
}
/// <summary>
/// 子弹参数
/// </summary>
public class BulletArgs
{   
    //子弹实体
    public Bullet Bullet { get; set; }
    //位置
    public Vector2 Position { get; set; }
    //方向
    public Vector2 Direction { get; set; }
}
