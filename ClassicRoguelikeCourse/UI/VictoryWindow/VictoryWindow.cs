using ClassicRoguelikeCourse.Entities.Characters.Enemies;
using Godot;

namespace ClassicRoguelikeCourse.UI.VictoryWindow;
/// <summary>
/// 胜利窗口
/// </summary>
public partial class VictoryWindow : CanvasLayer, IUi
{
    public void Initialize()
    {
        var enemyContainer = GetTree().CurrentScene.GetNode<Node>("%EnemyContainer");

        foreach (var child in enemyContainer.GetChildren())
        {
            if (child is not Enemy enemy) continue;
            if (enemy.CharacterData.Name == "骷髅王")
            {
                // 骷髅王死亡时显示胜利窗口
                enemy.SkeletonKingDead += () => Visible = true;
            }
        }
    }

    public void Update(double delta)
    {
    }
}