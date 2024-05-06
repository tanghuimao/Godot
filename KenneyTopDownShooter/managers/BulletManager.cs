using Godot;
using System;
/// <summary>
/// 子弹管理器
/// </summary>
public partial class BulletManager : Node
{
    //生成子弹
    public void SpawnBullet(BulletSpawn args)
    {
        //添加子节点
        AddChild(args.Bullet);
        //设置位置
        args.Bullet.GlobalPosition = args.Position;
        //设置方向
        args.Bullet.SetDirection(args.Direction);
        //设置来源
        args.Bullet.SetOrigin(args.Character);
    }
}
/// <summary>
/// 子弹生成参数
/// </summary>
public class BulletSpawn
{   
    //生成子弹角色
    public Character Character { get; set; }
    //子弹实体
    public Bullet Bullet { get; set; }
    //位置
    public Vector2 Position { get; set; }
    //方向
    public Vector2 Direction { get; set; }
}
