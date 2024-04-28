using System.Threading.Tasks;
using ClassicRoguelikeCourse.Resources.CharacterData.PlayerData;

namespace ClassicRoguelikeCourse.Entities.PickableObjects.Items;
/// <summary>
/// 复活十字
/// </summary>
public partial class ReviveCross : Item, IDeadEffectItem
{
    //服务延迟时间
    private float _reviveDelaySeconds = 2f;
    public override void Initialize()
    {
        base.Initialize();
        _description = "死亡" + _reviveDelaySeconds.ToString("0.0") + "秒后消耗本物品并复活";
    }

    public async Task DoDeadEffect()
    {
        //延迟
        await Task.Delay((int)(_reviveDelaySeconds * 1000));
        //复活
        _player.CharacterData.Health += _player.CharacterData.MaxHealth;
        //移除使用物品
        (_player.CharacterData as PlayerData).Inventory.Remove(this);
    }
}