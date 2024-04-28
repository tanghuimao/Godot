namespace ClassicRoguelikeCourse.Entities.PickableObjects.Items;
/// <summary>
/// 拾取物品产生作用
/// </summary>
public interface IImmediateEffectItem
{
    /// <summary>
    /// 拾取物品产生作用
    /// </summary>
    public void DoImmediateEffect();
    /// <summary>
    /// 丢弃物品产生作用
    /// </summary>
    public void UnDoImmediateEffect();
}