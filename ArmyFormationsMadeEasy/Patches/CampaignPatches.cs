using ArmyFormationsMadeEasy.CampaignBehaviors;
using HarmonyLib;
using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace ArmyFormationsMadeEasy.Patches
{
    [HarmonyPatch(typeof (Campaign), "OnSessionStart")]
    [HarmonyPatch(new Type[] {typeof (CampaignGameStarter)})]
    public class OnSessionStartPatch
    {
        private static void Postfix()
        {
            if (SavedFormationClassesBehavior.Instance.formation_class_map == null)
            { 
                SavedFormationClassesBehavior.Instance.formation_class_map = new Dictionary<BasicCharacterObject, FormationClass>();
            }

            SavedFormationClassesBehavior.Instance.game_started = true;
        }
    }
}
