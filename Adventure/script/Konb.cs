using Godot;

namespace Adventure.script;
/// <summary>
/// 屏幕摇杆
/// </summary>
public partial class Konb : TouchScreenButton
{
    //摇杆拖拽半径
    public float DragRadius = 16f;
    
    // 屏幕交互都是支持多点触碰  记录当前交互的手指点位 默认为-1 没有交互
    public int FingerIndex = -1;

    //记录摇杆初始位置
    public  Vector2 StartPosition;
    
    //记录手指触摸点位
    public  Vector2 TouchPosition;
    
    //记录按下事件名称
    public string ActionName;
    
    public override void _Ready()
    {
        //记录摇杆初始位置
        StartPosition = GlobalPosition;
    }

    /// <summary>
    /// 处理输入事件
    /// </summary>
    /// <param name="event"></param>
    public override void _Input(InputEvent @event)
    {
        switch (@event)
        {
            //屏幕点击事件
            //点按事件 并且没有交互
            case InputEventScreenTouch screenTouch :
            {
                if (screenTouch.IsPressed() && FingerIndex == -1)
                {
                    //转换为全局坐标系
                    var golbalTouchPosition = GetCanvasTransform() * screenTouch.Position;
                    //转换为局部坐标系
                    var localPos = ToLocal(golbalTouchPosition);
                    //获得按钮区域
                    var rect = new Rect2(Vector2.Zero, TextureNormal.GetSize()); //获得摇滚按钮材质大小
                    //判断是否在按钮内
                    if (rect.HasPoint(localPos))
                    {
                        //记录当前交互的手指
                        FingerIndex = screenTouch.Index;

                        //手指触摸点位（摇杆内部）
                        TouchPosition = golbalTouchPosition - GlobalPosition;
                    }
                }
                //抬起事件 并且当前交互的手指是这个
                else if (!@event.IsPressed() && screenTouch.Index == FingerIndex)
                {
                    //释放事件
                    ActionRelease(ActionName);
                    //移除交互
                    FingerIndex = -1;
                    //恢复原位
                    GlobalPosition = StartPosition;
                }
                break;
            }
            //屏幕拖拽事件
            case InputEventScreenDrag screenDrag:
                if (FingerIndex == screenDrag.Index)
                {
                    //摇杆拖动位置  全局坐标系 - 摇杆内部手指点位
                    var screenDragPosition = GetCanvasTransform() * screenDrag.Position - TouchPosition;
                    //限制拖动范围
                    var movement = (screenDragPosition - StartPosition).LimitLength(DragRadius);
                    //移动
                    GlobalPosition = movement + StartPosition;
                    
                    //控制玩家移动
                    //归一化  x 等于 1 || -1
                    movement = movement / DragRadius;
                    //水平方向
                    if (movement.X > 0)
                    {
                        ActionRelease(ActionName);
                        ActionName = "move_right";
                        ActionPress(ActionName, Mathf.Abs(movement.X));
                    }
                    else if (movement.X < 0)
                    {
                        ActionRelease(ActionName);
                        ActionName = "move_left";
                        ActionPress(ActionName, Mathf.Abs(movement.X));
                    }

                }
                break;
        }
    }

    /// <summary>
    /// 松开按钮
    /// </summary>
    /// <param name="actionName"></param>
    public void ActionRelease(string actionName)
    {
        if (string.IsNullOrEmpty(actionName)) return;
        Input.ActionRelease(actionName);
    }
    
    /// <summary>
    /// 按下按钮
    /// </summary>
    /// <param name="actionName">事件名称</param>
    /// <param name="strength">强度</param>
    public void ActionPress(string actionName, float strength)
    {
        if (string.IsNullOrEmpty(actionName)) return;
        Input.ActionPress(actionName, strength);
    }
}