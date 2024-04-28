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
    public Dictionary<PackedScene, float> DeathDropPickableObjects = new ()
    {
        {GD.Load<PackedScene>("res://Entities/PickableObjects/Items/SmallPotion.tscn"), 0.5f}, //小血瓶
        {GD.Load<PackedScene>("res://Entities/PickableObjects/Items/BigPotion.tscn"), 0.5f}, //大血瓶
        {GD.Load<PackedScene>("res://Entities/PickableObjects/Items/SightBall.tscn"), 0.5f}, //视力宝珠
        {GD.Load<PackedScene>("res://Entities/PickableObjects/Items/MapScroll.tscn"), 0.5f}, //地图卷轴
        {GD.Load<PackedScene>("res://Entities/PickableObjects/Items/ReviveCross.tscn"), 0.5f}, //复活十字
        {GD.Load<PackedScene>("res://Entities/PickableObjects/Equipments/Sword.tscn"), 0.5f}, //剑
        {GD.Load<PackedScene>("res://Entities/PickableObjects/Equipments/Mace.tscn"), 0.5f}, //锤
        {GD.Load<PackedScene>("res://Entities/PickableObjects/Equipments/Shield.tscn"), 0.5f}, //盾
        {GD.Load<PackedScene>("res://Entities/PickableObjects/Equipments/Armour.tscn"), 0.5f}, //盔甲
        {GD.Load<PackedScene>("res://Entities/PickableObjects/Equipments/Ring.tscn"), 0.5f}, //戒指
        {GD.Load<PackedScene>("res://Entities/PickableObjects/Equipments/Necklace.tscn"), 0.5f}, //项链
    };   
}