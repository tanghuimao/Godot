using Godot;

namespace Adventure.script;
/// <summary>
/// 洞穴扩展脚本  实现通关
/// </summary>
public partial class Cave : World
{
    
    public Boar Boar;
    
    public  override void _Ready()
    {
        base._Ready();
        
        Boar = GetNode<Boar>("Enemy/Boar1");
        //死亡事件
        Boar.HealthComponent.Died += OnDied;
    }

    private void OnDied()
    {
        //切换通过场景
        GameGlobal.GetInstance().ChangeScene("res://Ui/GameEndScreen.tscn",null,null,null, 1D);
    }
}