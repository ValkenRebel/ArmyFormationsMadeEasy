using ArmyFormationsMadeEasy.CampaignBehaviors;
using HarmonyLib;
using System;
using TaleWorlds.Core;

namespace ArmyFormationsMadeEasy.Patches
{
    /*[HarmonyPatch(typeof (BasicCharacterObject), "get_DefaultFormationGroup")]
    [HarmonyPatch(new Type[] {})]
    public class get_DefaultFormationGroupPatch
    {
        private static bool Prefix(BasicCharacterObject __instance, ref int __result)
        {
            if (SavedFormationClassesBehavior.Instance.formation_class_map == null || 
               !SavedFormationClassesBehavior.Instance.formation_class_map.ContainsKey(__instance) || 
              (!SavedFormationClassesBehavior.Instance.game_started || 
               !__instance.IsHero))
            {
                return true;
            }

            __result = (int)SavedFormationClassesBehavior.Instance.formation_class_map[__instance];
            return false;
        }
    }*/
}
