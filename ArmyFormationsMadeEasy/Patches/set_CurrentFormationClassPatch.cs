using ArmyFormationsMadeEasy.CampaignBehaviors;
using HarmonyLib;
using System;
using TaleWorlds.Core;

namespace ArmyFormationsMadeEasy.Patches
{
    /*[HarmonyPatch(typeof (BasicCharacterObject), "set_CurrentFormationClass")]
    [HarmonyPatch(new Type[] {typeof (FormationClass)})]
    public class set_CurrentFormationClassPatch
    {
        private static bool Prefix(BasicCharacterObject __instance, FormationClass value)
        {
            if (!SavedFormationClassesBehavior.Instance.game_started || SavedFormationClassesBehavior.Instance.formation_class_map == null)
                return true;

            SavedFormationClassesBehavior.Instance.formation_class_map[__instance] = value;
            return false;
        }
    }*/
}
