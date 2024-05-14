using Godot;
using System;
/// <summary>
/// 玩家状态UI
/// </summary>
public partial class UI : CanvasLayer
{
    
    private Label _laserLabel;
    private Label _grenadeLabel;
    private Control _laserControl;
    private Control _grenadeControl;
    private TextureProgressBar _textureProgressBar;

    private Color _blueColor = new("58ffff");
    private Color _redColor = new(0.9f,0,0,1);
    
    public override void _Ready()
    {
        _laserControl = GetNode<Control>("LaserControl");
        _grenadeControl = GetNode<Control>("GrenadeControl");
        _laserLabel = GetNode<Label>("LaserControl/VBoxContainer/Label");
        _grenadeLabel = GetNode<Label>("GrenadeControl/VBoxContainer/Label");
        _textureProgressBar = GetNode<TextureProgressBar>("MarginContainer/TextureProgressBar");
        Global.ChangeStateEvent += OnChangeStateEvent;
        OnChangeStateEvent();
    }
    
    public override void _ExitTree()
    {
        Global.ChangeStateEvent -= OnChangeStateEvent;
    }
    
    private void OnChangeStateEvent()
    {
        SetLaserAmount();
        SetGrenadeAmount();
        SetHealth();
    }

    /// <summary>
    /// 设置激光数量
    /// </summary>
    public void SetLaserAmount()
    {
        _laserLabel.Text = Global.LaserAmount.ToString();
        UpdateColor(Global.LaserAmount, _laserControl);
    }
    /// <summary>
    /// 设置榴弹数量
    /// </summary>
    public void SetGrenadeAmount()
    {
        _grenadeLabel.Text = Global.GrenadeAmount.ToString();
        UpdateColor(Global.GrenadeAmount, _grenadeControl);
    }
    
    /// <summary>d
    /// 设置生命值
    /// </summary>
    public void SetHealth()
    {
        _textureProgressBar.Value = Global.Health;
    }

    /// <summary>
    /// 更新颜色
    /// </summary>
    /// <param name="amount">数量</param>
    /// <param name="node">节点</param>
    /// <param name="color">颜色</param>
    public void UpdateColor(int amount, Control node)
    {
        if (amount > 0)
        {
            node.Modulate = _blueColor;
        }
        else
        {
            node.Modulate = _redColor;
        }
        
    }
}
