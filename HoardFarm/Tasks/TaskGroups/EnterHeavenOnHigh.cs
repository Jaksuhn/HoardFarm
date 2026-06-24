using System.Collections;
using ECommons.Automation;
using ECommons.GameHelpers;
using ECommons.Throttlers;
using FFXIVClientStructs.FFXIV.Component.GUI;
using HoardFarm.Tasks.Base;

#pragma warning disable CS8974 // Converting method group to non-delegate type

namespace HoardFarm.Tasks.TaskGroups;

public class EnterHeavenOnHigh : IBaseTaskGroup
{
    
    public unsafe ArrayList GetTaskList()
    {
        var result = new ArrayList();
        TryGetAddonByName<AtkUnitBase>("DeepDungeonMenu", out var menu);
        TryGetAddonByName<AtkUnitBase>("DeepDungeonSaveData", out var save);

        result.Add(WaitForYesAlreadyDisabledTask);
        if (menu == null && save == null)
        {
            result.Add(new InteractObjectTask(KyuseiDataId));
        }

        if ((menu == null && save == null) || (menu != null && save == null))
        {
            result.Add(EnterDuty);
        }

        result.Add(SelectSave);
        result.Add(ConfirmPartyKo);
        result.Add(ConfirmDuty);
        result.Add(WaitTillDutyReady);
        return result;
    }
    
    private static unsafe bool? EnterDuty()
    {
        if (TryGetAddonByName<AtkUnitBase>("DeepDungeonMenu", out var menu) && IsAddonReady(menu))
        {
            Callback.Fire(menu, true, 0);
            return true;
        }

        return false;
    }
    
    private static unsafe bool? SelectSave()
    {
        if (TryGetAddonByName<AtkUnitBase>("DeepDungeonSaveData", out var menu) && IsAddonReady(menu))
        {
            Callback.Fire(menu, true, Config.HoardModeSave);
            return true;
        }

        return false;
    }
    
    private static unsafe bool? ConfirmPartyKo()
    {
        if (TryGetAddonByName<AtkUnitBase>("ContentsFinderConfirm", out var confirm) && confirm->IsVisible)
            return true;

        var addon = FindSelectYesNo(ConfirmPartyKoMessageId);
        if (addon != null && IsAddonReady(addon) && EzThrottler.Throttle("SelectYesnoTask" + ConfirmPartyKoMessageId))
        {
            Callback.Fire(addon, true, 0);
            return true;
        }

        return false;
    }

    private static unsafe bool? ConfirmDuty()
    {
        if (TryGetAddonByName<AtkUnitBase>("ContentsFinderConfirm", out var menu) && IsAddonReady(menu))
        {
            Callback.Fire(menu, true, 8);
            return true;
        }

        return false;
    }
    
    private static bool? WaitTillDutyReady()
    {
        if (InHoH && Player.Interactable && NotBusy())
        {
            EnqueueWaitImmediate(1000);
            return true;
        }

        return false;
    }

}
