using Godot;
using Godot.Collections;

namespace ClassicRoguelikeCourse.Resources.CharacterData.EnemyData;

/// <summary>
/// 敌人数据
/// </summary>
public partial class EnemyData : CharacterData
{
    //死亡掉落经验
    [Export]
    public float DeathDropExperience;
    //死亡掉落物品字典 Dictionary使用godot集合中的字典
    [Export]
    public Dictionary<PackedScene, float> DeathDropPickableObjects = new ();
}