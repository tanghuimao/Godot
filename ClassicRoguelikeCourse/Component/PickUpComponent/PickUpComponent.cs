using System;
using ClassicRoguelikeCourse.Entites.Characters;
using ClassicRoguelikeCourse.Entites.Characters.Player;
using ClassicRoguelikeCourse.Managers;
using ClassicRoguelikeCourse.Resources.CharacterData.PlayerData;
using Godot;
using Godot.Collections;

namespace ClassicRoguelikeCourse.Component.PickUpComponent;
/// <summary>
/// 拾取组件
/// </summary>
public partial class PickUpComponent : Node, IComponent
{
    //输入处理
    private InputHandler _inputHandler;
    //拾取对象容器
    private Node _pickableObjectContainer;
    //是否拾取
    private bool _isPickUp = false;
    public void Initialize()
    {
        _inputHandler = GetTree().CurrentScene.GetNode<InputHandler>("%InputHandler");
        _pickableObjectContainer = GetTree().CurrentScene.GetNode<Node>("%PickableObjectContainer");
        //绑定事件
        _inputHandler.PickupInputEvent += OnPickupInputEvent;
    }

    public void Update(double delta)
    {
        if (!_isPickUp) return;
        //尝试拾取物品
        TryPickUpPickableObjects();
        _isPickUp = false;
    }
    /// <summary>
    /// 拾取事件
    /// </summary>
    private void OnPickupInputEvent()
    {
        _isPickUp = true;
    }
    /// <summary>
    /// 尝试拾取物品
    /// </summary>
    private void TryPickUpPickableObjects()
    {
        var owner = GetOwner<Character>();
        if (owner is not Player) return;
        //获取物理空间
        var space = owner.GetWorld2D().DirectSpaceState;
        //创建物理查询参数
        var parameters = new PhysicsPointQueryParameters2D
        {
            Position = owner.GlobalPosition, //拾取位置
            CollideWithAreas = true, //拾取区域
            CollideWithBodies = false, //拾取物体
            CollisionMask = (uint)PhysicsLayer.PickableObject, //拾取对象
            Exclude = new Array<Rid> // 排除区域
            {
                owner.GetNode<Area2D>("Area2D").GetRid()
            }
        };
        //查询
        var results = space.IntersectPoint(parameters);
        //没有可拾取对象
        if (results.Count == 0) return;
        //遍历结果
        foreach (var result in results)
        {
            //获取拾取对象
            var pickableObject = result["collider"].As<Area2D>().Owner as PickableObject;
            //拾取
            PickUp(owner, pickableObject);
        }
    }
    /// <summary>
    /// 拾取物品
    /// </summary>
    /// <param name="character">拾取对象</param>
    /// <param name="pickableObject">物品或者装备</param>
    private void PickUp(Character character, PickableObject pickableObject)
    {
        //获取玩家
        var player = character as Player;
        //添加物品
        (player.CharacterData as PlayerData).Inventory.Add(pickableObject);
        //隐藏物品
        pickableObject.Visible = false;
        //移除物品
        _pickableObjectContainer.RemoveChild(pickableObject);
    }
}