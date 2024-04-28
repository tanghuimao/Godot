using ClassicRoguelikeCourse.Entites.Characters;
using ClassicRoguelikeCourse.Resources.CharacterData.EnemyData;
using Godot;

namespace ClassicRoguelikeCourse.Component.DropDownComponent;
/// <summary>
/// 掉落组件
/// </summary>
public partial class DropDownComponent : Node, IComponent, ILateUpdateComponent
{
    //掉落物品容器
    private Node _pickableObjectContainer;
    
    public void Initialize()
    {
        _pickableObjectContainer = GetTree().CurrentScene.GetNode<Node>("%PickableObjectContainer");
    }

    public void Update(double delta)
    {
    }

    public void LateUpdate()
    {
        TryDropPickableObject();
    }

    /// <summary>
    /// 尝试掉落物品
    /// </summary>
    private void TryDropPickableObject()
    {
        // 获取拥有者
        var owner = GetOwner<Character>();
        // 不是敌人 或者 未死亡
        if (owner is not Enemy || !owner.IsDead) return;
        // 遍历可掉落物品
        foreach (var element in (owner.CharacterData as EnemyData).DeathDropPickableObjects)
        {
            // 获取掉落概率
            var dropProbability = element.Value;
            // 随机数大于掉落概率
            if (GD.RandRange(0f,1f) > dropProbability) continue;
            // 实例化物品
            var pickableObject = element.Key.Instantiate<PickableObject>();
            // 添加物品
            _pickableObjectContainer.AddChild(pickableObject);
            // 设置位置
            pickableObject.GlobalPosition = owner.GlobalPosition;
            // 初始化
            pickableObject.Initialize();
            break;
        }
    }
}