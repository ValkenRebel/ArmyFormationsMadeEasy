using ArmyFormationsMadeEasy.CampaignBehaviors;
using HarmonyLib;
using System;
using TaleWorlds.SaveSystem;
using TaleWorlds.SaveSystem.Load;

namespace ArmyFormationsMadeEasy.Patches
{
    [HarmonyPatch(typeof (LoadContext), "Load")]
    [HarmonyPatch(new Type[] {typeof (LoadData), typeof (bool)})]
    public class LoadPatch
    {
        private static bool Prefix(bool loadAsLateInitialize)
        {
            SavedFormationClassesBehavior.Instance.game_started = false;
            return true;
        }
    }
}
