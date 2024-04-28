namespace ClassicRoguelikeCourse.Entities.PickableObjects.Items;
/// <summary>
/// 视力宝珠
/// </summary>
public partial class SightBall : Item, IImmediateEffectItem
{
    //视野增加量
    private int _sightIncrement = 3;
    public override void Initialize()
    {
        base.Initialize();
        _description = "拾取后立即增加" + _sightIncrement + "点视野";
    }

    public void DoImmediateEffect()
    {
        _player.CharacterData.Sight += _sightIncrement;
    }

    public void UnDoImmediateEffect()
    {
        _player.CharacterData.Sight -= _sightIncrement;
    }
}