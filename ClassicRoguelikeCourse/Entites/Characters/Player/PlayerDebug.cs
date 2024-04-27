using ClassicRoguelikeCourse.Resources.CharacterData.PlayerData;
using Godot;

namespace ClassicRoguelikeCourse.Entites.Characters.Player;
/// <summary>
/// 玩家调试信息
/// </summary>
public partial class PlayerDebug : Node
{
    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("show_player_info"))
        {
            var player = GetTree().CurrentScene.GetNode<Player>("%Player");
            var data = player.CharacterData as PlayerData;
            GD.Print("----------------------------------");
            GD.Print($"名称: {player.CharacterData.Name}");
            GD.Print($"等级: {data.Level}");
            GD.Print($"经验: {data.Experience}");
            GD.Print($"经验增长率: {data.LevelUpExperienceThresholdIncrementRate}");
            GD.Print($"升级属性点: {data.BaseAttributePointsGainPerLevelUp}");
            GD.Print($"视野: {player.CharacterData.Sight}");
            GD.Print($"力量: {player.CharacterData.Strength}");
            GD.Print($"体质: {player.CharacterData.Constitution}");
            GD.Print($"敏捷: {player.CharacterData.Agility}");
            GD.Print($"生命值: {player.CharacterData.Health}");
            GD.Print($"最大生命值: {player.CharacterData.MaxHealth}");
            GD.Print($"攻击力: {player.CharacterData.Attack}");
            GD.Print($"防御力: {player.CharacterData.Defend}");
            GD.Print($"闪避: {player.CharacterData.Dodge}");
            GD.Print($"暴击: {player.CharacterData.CriticalChance}");
            GD.Print("----------------------------------");
        }
    }
}