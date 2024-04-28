using System.Threading.Tasks;

namespace ClassicRoguelikeCourse.Entities.PickableObjects.Items;
/// <summary>
/// 死亡触发物品
/// </summary>
public interface IDeadEffectItem
{
    /// <summary>
    /// 死亡触发物品
    /// </summary>
    /// <returns></returns>
    public Task DoDeadEffect();
}