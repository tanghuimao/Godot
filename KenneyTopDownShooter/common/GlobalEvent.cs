using System;

namespace KenneyTopDownShooter.common;

public class GlobalEvent
{
    // 子弹发射事件
    public static event Action<BulletArgs> BulletFiredEvent;
    public static void OnBulletFiredEvent(BulletArgs args)
    {
        BulletFiredEvent?.Invoke(args);
    }
}