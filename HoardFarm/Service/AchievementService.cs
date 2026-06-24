using System;
using System.Diagnostics.CodeAnalysis;
using ECommons.EzHookManager;
using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace HoardFarm.Service;

public unsafe class AchievementService
{
    [EzHook("C7 81 ?? ?? ?? ?? ?? ?? ?? ?? 89 91 ?? ?? ?? ?? 44 89 81")]
    public EzHook<Achievement.Delegates.ReceiveAchievementProgress> ReceiveAchievementProgressHook = null!;

    public int Progress;
    
    public AchievementService()
    {
        EzSignatureHelper.Initialize(this);
        ReceiveAchievementProgressHook.Enable();
    }
    
    public void UpdateProgress()
    {
        Achievement.Instance()->RequestAchievementProgress(ForTheHoardAchievementId);
    }
    
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    private void ReceiveAchievementProgressDetour(Achievement* achievement, uint id, uint current, uint max)
    {
        if (id == ForTheHoardAchievementId)
        {
            PluginLog.Debug($"Achievement progress: {current}/{max}");
            Progress = (int)current;
        }
        ReceiveAchievementProgressHook.Original(achievement, id, current, max);
    }
  
}
