using System;
using ClassicRoguelikeCourse.Entites;
using Godot;

namespace ClassicRoguelikeCourse.Managers.MapManager;
/// <summary>
/// 地图生成器接口
/// </summary>
public interface IMapGenerator : IEntity
{
    //玩家出生单元格
    public Vector2I GetPlayerSpawnCell();
    //敌人生成单元格
    public Vector2I GetEnemySpawnCell(); 
}