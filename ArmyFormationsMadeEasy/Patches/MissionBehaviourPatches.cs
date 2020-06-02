using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace ArmyFormationsMadeEasy.Patches
{
    [HarmonyPatch(typeof(MissionBehaviour), "OnMissionTick")]
    public class OnMissionTickPatch
    {
        public static bool AnyAlternateKeysHeldDown { get; set; } = false;
        public static bool LAltKeyPressed { get; set; } = false;
        public static bool LCtrlKeyPressed { get; set; } = false;
        public static bool LShiftKeyPressed { get; set; } = false;
        public static bool RAltKeyPressed { get; set; } = false;
        public static bool RCtrlKeyPressed { get; set; } = false;
        public static bool RShiftKeyPressed { get; set; } = false;
        public static bool EpicBattlesAIToggleKeyPressed { get; set; } = false;
        public static bool ResetUnitSpeedLimitKeyPressed { get; set; } = false;
        public static bool ReduceUnitSpeedLimitKeyPressed { get; set; } = false;
        public static bool AdvanceSelectedKeyPressed { get; set; } = false;
        public static bool FallbackSelectedKeyPressed { get; set; } = false;
        public static bool CustomFormation1KeyPressed { get; set; } = false;
        public static bool CustomFormation2KeyPressed { get; set; } = false;
        public static bool CustomFormation3KeyPressed { get; set; } = false;
        public static bool CustomFormation4KeyPressed { get; set; } = false;
        public static bool CustomFormation5KeyPressed { get; set; } = false;
        public static bool CustomFormation6KeyPressed { get; set; } = false;
        public static bool CustomFormation7KeyPressed { get; set; } = false;
        public static bool CustomFormation8KeyPressed { get; set; } = false;

        public static bool AllFormationFrontAttPtUpdated { get; set; } = false;
        public static bool[] FormationFrontAttPtUpdated { get; set; } = new bool[8] { false, false, false, false, false, false, false, false };

        public static bool CustomArmyFormationParamsSet { get; set; } = false;
        public static List<CustomArmyFormation> CustomArmyFormations { get; set; } = new List<CustomArmyFormation>();

        private static Helpers _helpersInstance = new Helpers();


        #region Epic Battle AI
        public static bool EpicBattleAIActive { get; set; } = false;
        public static bool EpicBattleAIPhase1Active { get; set; } = false;
        public static bool EpicBattleAIPhase1Completed { get; set; } = false;
        public static Formation EnemyMainFormation { get; set; } = null;
        public static WorldPosition EpicBattleAIPhase1DestinationPos { get; set; } = new WorldPosition();
        public static float Phase1DestinationOffsetDistance { get; set; } = 175f;
        public static float EpicBattleAIDelayPhase2Seconds { get; set; } = 30f;
        public static MissionTime EpicBattleAIPhase2DelayTime;
        public static bool EpicBattleAIPhase2Active { get; set; } = false;
        public static bool EpicBattleAIPhase2Completed { get; set; } = false;
        public static float Phase2DestinationOffsetDistance { get; set; } = 40f;
        public static float EpicBattleAIDelayPhase3Seconds { get; set; } = 30f;
        public static MissionTime EpicBattleAIPhase3DelayTime;
        public static bool EpicBattleAIPhase3Active { get; set; } = false;
        public static bool EpicBattleAIPhase3Completed { get; set; } = false;
        public static float Phase3DestinationOffsetDistance { get; set; } = 100f;
        public static float EpicBattleAIDelayPhase4Seconds { get; set; } = 100f;
        public static MissionTime EpicBattleAIPhase4DelayTime;
        public static bool EpicBattleAIPhase4Active { get; set; } = false;
        public static bool EpicBattleAIPhase4Completed { get; set; } = false;
        public static float EpicBattleAIDelayPhase5Seconds { get; set; } = 30f;
        public static MissionTime EpicBattleAIPhase5DelayTime;
        public static bool EpicBattleAIPhase5Active { get; set; } = false;
        public static bool EpicBattleAIPhase5Completed { get; set; } = false;
        public static bool EpicBattleAIFlee { get; set; } = false;
        public static bool EpicBattleAIIsRetreating { get; set; } = false;
        public static bool EpicBattleAIIsFightingOn { get; set; } = false;

        #endregion


        // Enable the Mod if it's enabled in the in-game Mod Options
        static bool Prepare()
        {
            return Settings.Instance.CustomArmyFormationsModEnabled;
        }


        // Patched 'MissionBehaviour.OnMissionTick()' (Postfix)
        static void Postfix(MissionBehaviour __instance)
        {
            // Fill the Custom Army Formations List - from the current Mod 'Settings' Options
            if (CustomArmyFormations.Count == 0 && __instance.Mission.MainAgent != null)
            {
                InitCustomArmyFormationsList();
            }

            // Reset Enemy/Ally max speed limit at start of each battle
            if (__instance.Mission.TimeSpeedTimerElapsedTime <= 0.1 && Settings.Instance.AllEnemyAllyAgentsWalk)
            {
                Settings.Instance.AllEnemyAllyAgentsWalk = false;
            }

            // Update if Alt, Ctrl, or Shift are currently held down (Left & Right)
            UpdateAltKeyStates();

            // Handle user inputs
            ManageKeyInputs(__instance);


            // Epic Battle AI Tick
            if (__instance.Mission.TimeSpeedTimerElapsedTime <= 0.1 && EpicBattleAIActive)
            {
                // Deactivate and Reset Epic Battle AI at start of each battle
                EpicBattleAIActive = false;
                EpicBattleAIReset();
            }
            else if (Settings.Instance.AllowEpicBattleAI && EpicBattleAIActive)
            {
                EpicBattleAITick(__instance);
            }
            else
            {
                EpicBattleAIReset();
            }

        }


        // Fill the CustomArmyFormations List from the current 'Mod Options' settings
        private static void InitCustomArmyFormationsList()
        {
            CustomArmyFormation customArmyFormation00 = new CustomArmyFormation(0,
                                                                                Settings.Instance.CustomArmyFormation00Enabled, 
                                                                                Settings.Instance.CustomArmyFormation00InfantryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation00InfantryFormOrder,
                                                                                Settings.Instance.CustomArmyFormation00RangedArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation00RangedFormOrder,
                                                                                Settings.Instance.CustomArmyFormation00RangedStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation00RangedStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation00CavalryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation00CavalryFormOrder,
                                                                                Settings.Instance.CustomArmyFormation00CavalryStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation00CavalryStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation00HorseArcherArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation00HorseArcherFormOrder,
                                                                                Settings.Instance.CustomArmyFormation00HorseArcherStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation00HorseArcherStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation00SkirmisherArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation00SkirmisherFormOrder,
                                                                                Settings.Instance.CustomArmyFormation00SkirmisherStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation00SkirmisherStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation00HeavyInfantryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation00HeavyInfantryFormOrder,
                                                                                Settings.Instance.CustomArmyFormation00HeavyInfantryStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation00HeavyInfantryStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation00LightCavalryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation00LightCavalryFormOrder,
                                                                                Settings.Instance.CustomArmyFormation00LightCavalryStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation00LightCavalryStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation00HeavyCavalryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation00HeavyCavalryFormOrder,
                                                                                Settings.Instance.CustomArmyFormation00HeavyCavalryStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation00HeavyCavalryStartPosFwdOffset
                                                                                );
            CustomArmyFormations.Add(customArmyFormation00);

            CustomArmyFormation customArmyFormation01 = new CustomArmyFormation(1,
                                                                                Settings.Instance.CustomArmyFormation01Enabled,
                                                                                Settings.Instance.CustomArmyFormation01InfantryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation01InfantryFormOrder,
                                                                                Settings.Instance.CustomArmyFormation01RangedArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation01RangedFormOrder,
                                                                                Settings.Instance.CustomArmyFormation01RangedStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation01RangedStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation01CavalryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation01CavalryFormOrder,
                                                                                Settings.Instance.CustomArmyFormation01CavalryStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation01CavalryStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation01HorseArcherArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation01HorseArcherFormOrder,
                                                                                Settings.Instance.CustomArmyFormation01HorseArcherStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation01HorseArcherStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation01SkirmisherArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation01SkirmisherFormOrder,
                                                                                Settings.Instance.CustomArmyFormation01SkirmisherStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation01SkirmisherStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation01HeavyInfantryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation01HeavyInfantryFormOrder,
                                                                                Settings.Instance.CustomArmyFormation01HeavyInfantryStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation01HeavyInfantryStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation01LightCavalryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation01LightCavalryFormOrder,
                                                                                Settings.Instance.CustomArmyFormation01LightCavalryStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation01LightCavalryStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation01HeavyCavalryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation01HeavyCavalryFormOrder,
                                                                                Settings.Instance.CustomArmyFormation01HeavyCavalryStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation01HeavyCavalryStartPosFwdOffset
                                                                                );
            CustomArmyFormations.Add(customArmyFormation01);

            CustomArmyFormation customArmyFormation02 = new CustomArmyFormation(2,
                                                                                Settings.Instance.CustomArmyFormation02Enabled,
                                                                                Settings.Instance.CustomArmyFormation02InfantryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation02InfantryFormOrder,
                                                                                Settings.Instance.CustomArmyFormation02RangedArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation02RangedFormOrder,
                                                                                Settings.Instance.CustomArmyFormation02RangedStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation02RangedStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation02CavalryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation02CavalryFormOrder,
                                                                                Settings.Instance.CustomArmyFormation02CavalryStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation02CavalryStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation02HorseArcherArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation02HorseArcherFormOrder,
                                                                                Settings.Instance.CustomArmyFormation02HorseArcherStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation02HorseArcherStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation02SkirmisherArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation02SkirmisherFormOrder,
                                                                                Settings.Instance.CustomArmyFormation02SkirmisherStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation02SkirmisherStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation02HeavyInfantryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation02HeavyInfantryFormOrder,
                                                                                Settings.Instance.CustomArmyFormation02HeavyInfantryStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation02HeavyInfantryStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation02LightCavalryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation02LightCavalryFormOrder,
                                                                                Settings.Instance.CustomArmyFormation02LightCavalryStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation02LightCavalryStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation02HeavyCavalryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation02HeavyCavalryFormOrder,
                                                                                Settings.Instance.CustomArmyFormation02HeavyCavalryStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation02HeavyCavalryStartPosFwdOffset
                                                                                );
            CustomArmyFormations.Add(customArmyFormation02);

            CustomArmyFormation customArmyFormation03 = new CustomArmyFormation(3,
                                                                                Settings.Instance.CustomArmyFormation03Enabled,
                                                                                Settings.Instance.CustomArmyFormation03InfantryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation03InfantryFormOrder,
                                                                                Settings.Instance.CustomArmyFormation03RangedArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation03RangedFormOrder,
                                                                                Settings.Instance.CustomArmyFormation03RangedStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation03RangedStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation03CavalryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation03CavalryFormOrder,
                                                                                Settings.Instance.CustomArmyFormation03CavalryStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation03CavalryStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation03HorseArcherArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation03HorseArcherFormOrder,
                                                                                Settings.Instance.CustomArmyFormation03HorseArcherStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation03HorseArcherStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation03SkirmisherArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation03SkirmisherFormOrder,
                                                                                Settings.Instance.CustomArmyFormation03SkirmisherStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation03SkirmisherStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation03HeavyInfantryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation03HeavyInfantryFormOrder,
                                                                                Settings.Instance.CustomArmyFormation03HeavyInfantryStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation03HeavyInfantryStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation03LightCavalryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation03LightCavalryFormOrder,
                                                                                Settings.Instance.CustomArmyFormation03LightCavalryStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation03LightCavalryStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation03HeavyCavalryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation03HeavyCavalryFormOrder,
                                                                                Settings.Instance.CustomArmyFormation03HeavyCavalryStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation03HeavyCavalryStartPosFwdOffset
                                                                                );
            CustomArmyFormations.Add(customArmyFormation03);

            CustomArmyFormation customArmyFormation04 = new CustomArmyFormation(4,
                                                                                Settings.Instance.CustomArmyFormation04Enabled,
                                                                                Settings.Instance.CustomArmyFormation04InfantryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation04InfantryFormOrder,
                                                                                Settings.Instance.CustomArmyFormation04RangedArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation04RangedFormOrder,
                                                                                Settings.Instance.CustomArmyFormation04RangedStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation04RangedStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation04CavalryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation04CavalryFormOrder,
                                                                                Settings.Instance.CustomArmyFormation04CavalryStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation04CavalryStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation04HorseArcherArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation04HorseArcherFormOrder,
                                                                                Settings.Instance.CustomArmyFormation04HorseArcherStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation04HorseArcherStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation04SkirmisherArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation04SkirmisherFormOrder,
                                                                                Settings.Instance.CustomArmyFormation04SkirmisherStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation04SkirmisherStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation04HeavyInfantryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation04HeavyInfantryFormOrder,
                                                                                Settings.Instance.CustomArmyFormation04HeavyInfantryStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation04HeavyInfantryStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation04LightCavalryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation04LightCavalryFormOrder,
                                                                                Settings.Instance.CustomArmyFormation04LightCavalryStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation04LightCavalryStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation04HeavyCavalryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation04HeavyCavalryFormOrder,
                                                                                Settings.Instance.CustomArmyFormation04HeavyCavalryStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation04HeavyCavalryStartPosFwdOffset
                                                                                );
            CustomArmyFormations.Add(customArmyFormation04);

            CustomArmyFormation customArmyFormation05 = new CustomArmyFormation(5,
                                                                                Settings.Instance.CustomArmyFormation05Enabled,
                                                                                Settings.Instance.CustomArmyFormation05InfantryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation05InfantryFormOrder,
                                                                                Settings.Instance.CustomArmyFormation05RangedArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation05RangedFormOrder,
                                                                                Settings.Instance.CustomArmyFormation05RangedStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation05RangedStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation05CavalryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation05CavalryFormOrder,
                                                                                Settings.Instance.CustomArmyFormation05CavalryStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation05CavalryStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation05HorseArcherArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation05HorseArcherFormOrder,
                                                                                Settings.Instance.CustomArmyFormation05HorseArcherStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation05HorseArcherStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation05SkirmisherArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation05SkirmisherFormOrder,
                                                                                Settings.Instance.CustomArmyFormation05SkirmisherStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation05SkirmisherStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation05HeavyInfantryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation05HeavyInfantryFormOrder,
                                                                                Settings.Instance.CustomArmyFormation05HeavyInfantryStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation05HeavyInfantryStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation05LightCavalryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation05LightCavalryFormOrder,
                                                                                Settings.Instance.CustomArmyFormation05LightCavalryStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation05LightCavalryStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation05HeavyCavalryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation05HeavyCavalryFormOrder,
                                                                                Settings.Instance.CustomArmyFormation05HeavyCavalryStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation05HeavyCavalryStartPosFwdOffset
                                                                                );
            CustomArmyFormations.Add(customArmyFormation05);

            CustomArmyFormation customArmyFormation06 = new CustomArmyFormation(6,
                                                                                Settings.Instance.CustomArmyFormation06Enabled,
                                                                                Settings.Instance.CustomArmyFormation06InfantryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation06InfantryFormOrder,
                                                                                Settings.Instance.CustomArmyFormation06RangedArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation06RangedFormOrder,
                                                                                Settings.Instance.CustomArmyFormation06RangedStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation06RangedStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation06CavalryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation06CavalryFormOrder,
                                                                                Settings.Instance.CustomArmyFormation06CavalryStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation06CavalryStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation06HorseArcherArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation06HorseArcherFormOrder,
                                                                                Settings.Instance.CustomArmyFormation06HorseArcherStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation06HorseArcherStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation06SkirmisherArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation06SkirmisherFormOrder,
                                                                                Settings.Instance.CustomArmyFormation06SkirmisherStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation06SkirmisherStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation06HeavyInfantryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation06HeavyInfantryFormOrder,
                                                                                Settings.Instance.CustomArmyFormation06HeavyInfantryStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation06HeavyInfantryStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation06LightCavalryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation06LightCavalryFormOrder,
                                                                                Settings.Instance.CustomArmyFormation06LightCavalryStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation06LightCavalryStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation06HeavyCavalryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation06HeavyCavalryFormOrder,
                                                                                Settings.Instance.CustomArmyFormation06HeavyCavalryStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation06HeavyCavalryStartPosFwdOffset
                                                                                );
            CustomArmyFormations.Add(customArmyFormation06);

            CustomArmyFormation customArmyFormation07 = new CustomArmyFormation(7,
                                                                                Settings.Instance.CustomArmyFormation07Enabled,
                                                                                Settings.Instance.CustomArmyFormation07InfantryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation07InfantryFormOrder,
                                                                                Settings.Instance.CustomArmyFormation07RangedArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation07RangedFormOrder,
                                                                                Settings.Instance.CustomArmyFormation07RangedStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation07RangedStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation07CavalryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation07CavalryFormOrder,
                                                                                Settings.Instance.CustomArmyFormation07CavalryStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation07CavalryStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation07HorseArcherArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation07HorseArcherFormOrder,
                                                                                Settings.Instance.CustomArmyFormation07HorseArcherStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation07HorseArcherStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation07SkirmisherArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation07SkirmisherFormOrder,
                                                                                Settings.Instance.CustomArmyFormation07SkirmisherStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation07SkirmisherStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation07HeavyInfantryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation07HeavyInfantryFormOrder,
                                                                                Settings.Instance.CustomArmyFormation07HeavyInfantryStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation07HeavyInfantryStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation07LightCavalryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation07LightCavalryFormOrder,
                                                                                Settings.Instance.CustomArmyFormation07LightCavalryStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation07LightCavalryStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation07HeavyCavalryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation07HeavyCavalryFormOrder,
                                                                                Settings.Instance.CustomArmyFormation07HeavyCavalryStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation07HeavyCavalryStartPosFwdOffset
                                                                                );
            CustomArmyFormations.Add(customArmyFormation07);

            CustomArmyFormation customArmyFormation08 = new CustomArmyFormation(8,
                                                                                true,   //Settings.Instance.CustomArmyFormation07Enabled,
                                                                                5,      //Settings.Instance.CustomArmyFormation07InfantryArrangementOrder,
                                                                                -2,     //Settings.Instance.CustomArmyFormation07InfantryFormOrder,
                                                                                3,      //Settings.Instance.CustomArmyFormation07RangedArrangementOrder,
                                                                                -2,     //Settings.Instance.CustomArmyFormation07RangedFormOrder,
                                                                                0,      //Settings.Instance.CustomArmyFormation07RangedStartPosLateralOffset,
                                                                                20,    //Settings.Instance.CustomArmyFormation07RangedStartPosFwdOffset,
                                                                                -1,     //Settings.Instance.CustomArmyFormation07CavalryArrangementOrder,
                                                                                0,      //Settings.Instance.CustomArmyFormation07CavalryFormOrder,
                                                                                -60,     //Settings.Instance.CustomArmyFormation07CavalryStartPosLateralOffset,
                                                                                0,      //Settings.Instance.CustomArmyFormation07CavalryStartPosFwdOffset,
                                                                                -1,     //Settings.Instance.CustomArmyFormation07HorseArcherArrangementOrder,
                                                                                0,      //Settings.Instance.CustomArmyFormation07HorseArcherFormOrder,
                                                                                60,    //Settings.Instance.CustomArmyFormation07HorseArcherStartPosLateralOffset,
                                                                                0,      //Settings.Instance.CustomArmyFormation07HorseArcherStartPosFwdOffset,
                                                                                5,      //Settings.Instance.CustomArmyFormation07SkirmisherArrangementOrder,
                                                                                -2,     //Settings.Instance.CustomArmyFormation07SkirmisherFormOrder,
                                                                                0,      //Settings.Instance.CustomArmyFormation07SkirmisherStartPosLateralOffset,
                                                                                -10,     //Settings.Instance.CustomArmyFormation07SkirmisherStartPosFwdOffset,
                                                                                5,      //Settings.Instance.CustomArmyFormation07HeavyInfantryArrangementOrder,
                                                                                -2,     //Settings.Instance.CustomArmyFormation07HeavyInfantryFormOrder,
                                                                                0,      //Settings.Instance.CustomArmyFormation07HeavyInfantryStartPosLateralOffset,
                                                                                10,    //Settings.Instance.CustomArmyFormation07HeavyInfantryStartPosFwdOffset,
                                                                                5,      //Settings.Instance.CustomArmyFormation07LightCavalryArrangementOrder,
                                                                                -2,     //Settings.Instance.CustomArmyFormation07LightCavalryFormOrder,
                                                                                0,      //Settings.Instance.CustomArmyFormation07LightCavalryStartPosLateralOffset,
                                                                                25,    //Settings.Instance.CustomArmyFormation07LightCavalryStartPosFwdOffset,
                                                                                5,      //Settings.Instance.CustomArmyFormation07HeavyCavalryArrangementOrder,
                                                                                -2,     //Settings.Instance.CustomArmyFormation07HeavyCavalryFormOrder,
                                                                                0,      //Settings.Instance.CustomArmyFormation07HeavyCavalryStartPosLateralOffset,
                                                                                35     //Settings.Instance.CustomArmyFormation07HeavyCavalryStartPosFwdOffset
                                                                                );
            CustomArmyFormations.Add(customArmyFormation08);
        }

        // Update Settings.Instance - with the CustomArmyFormations List's current settings
        private static void UpdateCustomArmyFormationSettings()
        {
            foreach (CustomArmyFormation custArmyFormation in CustomArmyFormations)
            {
                if (custArmyFormation.CustomArmyFormationIndex == 0)
                {
                    Settings.Instance.CustomArmyFormation00InfantryArrangementOrder = custArmyFormation.InfantryArrangementOrderNum;
                    Settings.Instance.CustomArmyFormation00InfantryFormOrder = custArmyFormation.InfantryFormOrderNum;
                    Settings.Instance.CustomArmyFormation00RangedArrangementOrder = custArmyFormation.RangedArrangementOrderNum;
                    Settings.Instance.CustomArmyFormation00RangedFormOrder = custArmyFormation.RangedFormOrderNum;
                    Settings.Instance.CustomArmyFormation00RangedStartPosLateralOffset = custArmyFormation.RangedStartPosLateralOffset;
                    Settings.Instance.CustomArmyFormation00RangedStartPosFwdOffset = custArmyFormation.RangedStartPosFwdOffset;
                    Settings.Instance.CustomArmyFormation00CavalryArrangementOrder = custArmyFormation.CavalryArrangementOrderNum;
                    Settings.Instance.CustomArmyFormation00CavalryFormOrder = custArmyFormation.CavalryFormOrderNum;
                    Settings.Instance.CustomArmyFormation00CavalryStartPosLateralOffset = custArmyFormation.CavalryStartPosLateralOffset;
                    Settings.Instance.CustomArmyFormation00CavalryStartPosFwdOffset = custArmyFormation.CavalryStartPosFwdOffset;
                    Settings.Instance.CustomArmyFormation00HorseArcherArrangementOrder = custArmyFormation.HorseArcherArrangementOrderNum;
                    Settings.Instance.CustomArmyFormation00HorseArcherFormOrder = custArmyFormation.HorseArcherFormOrderNum;
                    Settings.Instance.CustomArmyFormation00HorseArcherStartPosLateralOffset = custArmyFormation.HorseArcherStartPosLateralOffset;
                    Settings.Instance.CustomArmyFormation00HorseArcherStartPosFwdOffset = custArmyFormation.HorseArcherStartPosFwdOffset;
                    Settings.Instance.CustomArmyFormation00SkirmisherArrangementOrder = custArmyFormation.SkirmisherArrangementOrderNum;
                    Settings.Instance.CustomArmyFormation00SkirmisherFormOrder = custArmyFormation.SkirmisherFormOrderNum;
                    Settings.Instance.CustomArmyFormation00SkirmisherStartPosLateralOffset = custArmyFormation.SkirmisherStartPosLateralOffset;
                    Settings.Instance.CustomArmyFormation00SkirmisherStartPosFwdOffset = custArmyFormation.SkirmisherStartPosFwdOffset;
                    Settings.Instance.CustomArmyFormation00HeavyInfantryArrangementOrder = custArmyFormation.HeavyInfantryArrangementOrderNum;
                    Settings.Instance.CustomArmyFormation00HeavyInfantryFormOrder = custArmyFormation.HeavyInfantryFormOrderNum;
                    Settings.Instance.CustomArmyFormation00HeavyInfantryStartPosLateralOffset = custArmyFormation.HeavyInfantryStartPosLateralOffset;
                    Settings.Instance.CustomArmyFormation00HeavyInfantryStartPosFwdOffset = custArmyFormation.HeavyInfantryStartPosFwdOffset;
                    Settings.Instance.CustomArmyFormation00LightCavalryArrangementOrder = custArmyFormation.LightCavalryArrangementOrderNum;
                    Settings.Instance.CustomArmyFormation00LightCavalryFormOrder = custArmyFormation.LightCavalryFormOrderNum;
                    Settings.Instance.CustomArmyFormation00LightCavalryStartPosLateralOffset = custArmyFormation.LightCavalryStartPosLateralOffset;
                    Settings.Instance.CustomArmyFormation00LightCavalryStartPosFwdOffset = custArmyFormation.LightCavalryStartPosFwdOffset;
                    Settings.Instance.CustomArmyFormation00HeavyCavalryArrangementOrder = custArmyFormation.HeavyCavalryArrangementOrderNum;
                    Settings.Instance.CustomArmyFormation00HeavyCavalryFormOrder = custArmyFormation.HeavyCavalryFormOrderNum;
                    Settings.Instance.CustomArmyFormation00HeavyCavalryStartPosLateralOffset = custArmyFormation.HeavyCavalryStartPosLateralOffset;
                    Settings.Instance.CustomArmyFormation00HeavyCavalryStartPosFwdOffset = custArmyFormation.HeavyCavalryStartPosFwdOffset;
                }
                else if (custArmyFormation.CustomArmyFormationIndex == 1)
                {
                    Settings.Instance.CustomArmyFormation01InfantryArrangementOrder = custArmyFormation.InfantryArrangementOrderNum;
                    Settings.Instance.CustomArmyFormation01InfantryFormOrder = custArmyFormation.InfantryFormOrderNum;
                    Settings.Instance.CustomArmyFormation01RangedArrangementOrder = custArmyFormation.RangedArrangementOrderNum;
                    Settings.Instance.CustomArmyFormation01RangedFormOrder = custArmyFormation.RangedFormOrderNum;
                    Settings.Instance.CustomArmyFormation01RangedStartPosLateralOffset = custArmyFormation.RangedStartPosLateralOffset;
                    Settings.Instance.CustomArmyFormation01RangedStartPosFwdOffset = custArmyFormation.RangedStartPosFwdOffset;
                    Settings.Instance.CustomArmyFormation01CavalryArrangementOrder = custArmyFormation.CavalryArrangementOrderNum;
                    Settings.Instance.CustomArmyFormation01CavalryFormOrder = custArmyFormation.CavalryFormOrderNum;
                    Settings.Instance.CustomArmyFormation01CavalryStartPosLateralOffset = custArmyFormation.CavalryStartPosLateralOffset;
                    Settings.Instance.CustomArmyFormation01CavalryStartPosFwdOffset = custArmyFormation.CavalryStartPosFwdOffset;
                    Settings.Instance.CustomArmyFormation01HorseArcherArrangementOrder = custArmyFormation.HorseArcherArrangementOrderNum;
                    Settings.Instance.CustomArmyFormation01HorseArcherFormOrder = custArmyFormation.HorseArcherFormOrderNum;
                    Settings.Instance.CustomArmyFormation01HorseArcherStartPosLateralOffset = custArmyFormation.HorseArcherStartPosLateralOffset;
                    Settings.Instance.CustomArmyFormation01HorseArcherStartPosFwdOffset = custArmyFormation.HorseArcherStartPosFwdOffset;
                    Settings.Instance.CustomArmyFormation01SkirmisherArrangementOrder = custArmyFormation.SkirmisherArrangementOrderNum;
                    Settings.Instance.CustomArmyFormation01SkirmisherFormOrder = custArmyFormation.SkirmisherFormOrderNum;
                    Settings.Instance.CustomArmyFormation01SkirmisherStartPosLateralOffset = custArmyFormation.SkirmisherStartPosLateralOffset;
                    Settings.Instance.CustomArmyFormation01SkirmisherStartPosFwdOffset = custArmyFormation.SkirmisherStartPosFwdOffset;
                    Settings.Instance.CustomArmyFormation01HeavyInfantryArrangementOrder = custArmyFormation.HeavyInfantryArrangementOrderNum;
                    Settings.Instance.CustomArmyFormation01HeavyInfantryFormOrder = custArmyFormation.HeavyInfantryFormOrderNum;
                    Settings.Instance.CustomArmyFormation01HeavyInfantryStartPosLateralOffset = custArmyFormation.HeavyInfantryStartPosLateralOffset;
                    Settings.Instance.CustomArmyFormation01HeavyInfantryStartPosFwdOffset = custArmyFormation.HeavyInfantryStartPosFwdOffset;
                    Settings.Instance.CustomArmyFormation01LightCavalryArrangementOrder = custArmyFormation.LightCavalryArrangementOrderNum;
                    Settings.Instance.CustomArmyFormation01LightCavalryFormOrder = custArmyFormation.LightCavalryFormOrderNum;
                    Settings.Instance.CustomArmyFormation01LightCavalryStartPosLateralOffset = custArmyFormation.LightCavalryStartPosLateralOffset;
                    Settings.Instance.CustomArmyFormation01LightCavalryStartPosFwdOffset = custArmyFormation.LightCavalryStartPosFwdOffset;
                    Settings.Instance.CustomArmyFormation01HeavyCavalryArrangementOrder = custArmyFormation.HeavyCavalryArrangementOrderNum;
                    Settings.Instance.CustomArmyFormation01HeavyCavalryFormOrder = custArmyFormation.HeavyCavalryFormOrderNum;
                    Settings.Instance.CustomArmyFormation01HeavyCavalryStartPosLateralOffset = custArmyFormation.HeavyCavalryStartPosLateralOffset;
                    Settings.Instance.CustomArmyFormation01HeavyCavalryStartPosFwdOffset = custArmyFormation.HeavyCavalryStartPosFwdOffset;
                }
                else if (custArmyFormation.CustomArmyFormationIndex == 2)
                {
                    Settings.Instance.CustomArmyFormation02InfantryArrangementOrder = custArmyFormation.InfantryArrangementOrderNum;
                    Settings.Instance.CustomArmyFormation02InfantryFormOrder = custArmyFormation.InfantryFormOrderNum;
                    Settings.Instance.CustomArmyFormation02RangedArrangementOrder = custArmyFormation.RangedArrangementOrderNum;
                    Settings.Instance.CustomArmyFormation02RangedFormOrder = custArmyFormation.RangedFormOrderNum;
                    Settings.Instance.CustomArmyFormation02RangedStartPosLateralOffset = custArmyFormation.RangedStartPosLateralOffset;
                    Settings.Instance.CustomArmyFormation02RangedStartPosFwdOffset = custArmyFormation.RangedStartPosFwdOffset;
                    Settings.Instance.CustomArmyFormation02CavalryArrangementOrder = custArmyFormation.CavalryArrangementOrderNum;
                    Settings.Instance.CustomArmyFormation02CavalryFormOrder = custArmyFormation.CavalryFormOrderNum;
                    Settings.Instance.CustomArmyFormation02CavalryStartPosLateralOffset = custArmyFormation.CavalryStartPosLateralOffset;
                    Settings.Instance.CustomArmyFormation02CavalryStartPosFwdOffset = custArmyFormation.CavalryStartPosFwdOffset;
                    Settings.Instance.CustomArmyFormation02HorseArcherArrangementOrder = custArmyFormation.HorseArcherArrangementOrderNum;
                    Settings.Instance.CustomArmyFormation02HorseArcherFormOrder = custArmyFormation.HorseArcherFormOrderNum;
                    Settings.Instance.CustomArmyFormation02HorseArcherStartPosLateralOffset = custArmyFormation.HorseArcherStartPosLateralOffset;
                    Settings.Instance.CustomArmyFormation02HorseArcherStartPosFwdOffset = custArmyFormation.HorseArcherStartPosFwdOffset;
                    Settings.Instance.CustomArmyFormation02SkirmisherArrangementOrder = custArmyFormation.SkirmisherArrangementOrderNum;
                    Settings.Instance.CustomArmyFormation02SkirmisherFormOrder = custArmyFormation.SkirmisherFormOrderNum;
                    Settings.Instance.CustomArmyFormation02SkirmisherStartPosLateralOffset = custArmyFormation.SkirmisherStartPosLateralOffset;
                    Settings.Instance.CustomArmyFormation02SkirmisherStartPosFwdOffset = custArmyFormation.SkirmisherStartPosFwdOffset;
                    Settings.Instance.CustomArmyFormation02HeavyInfantryArrangementOrder = custArmyFormation.HeavyInfantryArrangementOrderNum;
                    Settings.Instance.CustomArmyFormation02HeavyInfantryFormOrder = custArmyFormation.HeavyInfantryFormOrderNum;
                    Settings.Instance.CustomArmyFormation02HeavyInfantryStartPosLateralOffset = custArmyFormation.HeavyInfantryStartPosLateralOffset;
                    Settings.Instance.CustomArmyFormation02HeavyInfantryStartPosFwdOffset = custArmyFormation.HeavyInfantryStartPosFwdOffset;
                    Settings.Instance.CustomArmyFormation02LightCavalryArrangementOrder = custArmyFormation.LightCavalryArrangementOrderNum;
                    Settings.Instance.CustomArmyFormation02LightCavalryFormOrder = custArmyFormation.LightCavalryFormOrderNum;
                    Settings.Instance.CustomArmyFormation02LightCavalryStartPosLateralOffset = custArmyFormation.LightCavalryStartPosLateralOffset;
                    Settings.Instance.CustomArmyFormation02LightCavalryStartPosFwdOffset = custArmyFormation.LightCavalryStartPosFwdOffset;
                    Settings.Instance.CustomArmyFormation02HeavyCavalryArrangementOrder = custArmyFormation.HeavyCavalryArrangementOrderNum;
                    Settings.Instance.CustomArmyFormation02HeavyCavalryFormOrder = custArmyFormation.HeavyCavalryFormOrderNum;
                    Settings.Instance.CustomArmyFormation02HeavyCavalryStartPosLateralOffset = custArmyFormation.HeavyCavalryStartPosLateralOffset;
                    Settings.Instance.CustomArmyFormation02HeavyCavalryStartPosFwdOffset = custArmyFormation.HeavyCavalryStartPosFwdOffset;
                }
                else if (custArmyFormation.CustomArmyFormationIndex == 3)
                {
                    Settings.Instance.CustomArmyFormation03InfantryArrangementOrder = custArmyFormation.InfantryArrangementOrderNum;
                    Settings.Instance.CustomArmyFormation03InfantryFormOrder = custArmyFormation.InfantryFormOrderNum;
                    Settings.Instance.CustomArmyFormation03RangedArrangementOrder = custArmyFormation.RangedArrangementOrderNum;
                    Settings.Instance.CustomArmyFormation03RangedFormOrder = custArmyFormation.RangedFormOrderNum;
                    Settings.Instance.CustomArmyFormation03RangedStartPosLateralOffset = custArmyFormation.RangedStartPosLateralOffset;
                    Settings.Instance.CustomArmyFormation03RangedStartPosFwdOffset = custArmyFormation.RangedStartPosFwdOffset;
                    Settings.Instance.CustomArmyFormation03CavalryArrangementOrder = custArmyFormation.CavalryArrangementOrderNum;
                    Settings.Instance.CustomArmyFormation03CavalryFormOrder = custArmyFormation.CavalryFormOrderNum;
                    Settings.Instance.CustomArmyFormation03CavalryStartPosLateralOffset = custArmyFormation.CavalryStartPosLateralOffset;
                    Settings.Instance.CustomArmyFormation03CavalryStartPosFwdOffset = custArmyFormation.CavalryStartPosFwdOffset;
                    Settings.Instance.CustomArmyFormation03HorseArcherArrangementOrder = custArmyFormation.HorseArcherArrangementOrderNum;
                    Settings.Instance.CustomArmyFormation03HorseArcherFormOrder = custArmyFormation.HorseArcherFormOrderNum;
                    Settings.Instance.CustomArmyFormation03HorseArcherStartPosLateralOffset = custArmyFormation.HorseArcherStartPosLateralOffset;
                    Settings.Instance.CustomArmyFormation03HorseArcherStartPosFwdOffset = custArmyFormation.HorseArcherStartPosFwdOffset;
                    Settings.Instance.CustomArmyFormation03SkirmisherArrangementOrder = custArmyFormation.SkirmisherArrangementOrderNum;
                    Settings.Instance.CustomArmyFormation03SkirmisherFormOrder = custArmyFormation.SkirmisherFormOrderNum;
                    Settings.Instance.CustomArmyFormation03SkirmisherStartPosLateralOffset = custArmyFormation.SkirmisherStartPosLateralOffset;
                    Settings.Instance.CustomArmyFormation03SkirmisherStartPosFwdOffset = custArmyFormation.SkirmisherStartPosFwdOffset;
                    Settings.Instance.CustomArmyFormation03HeavyInfantryArrangementOrder = custArmyFormation.HeavyInfantryArrangementOrderNum;
                    Settings.Instance.CustomArmyFormation03HeavyInfantryFormOrder = custArmyFormation.HeavyInfantryFormOrderNum;
                    Settings.Instance.CustomArmyFormation03HeavyInfantryStartPosLateralOffset = custArmyFormation.HeavyInfantryStartPosLateralOffset;
                    Settings.Instance.CustomArmyFormation03HeavyInfantryStartPosFwdOffset = custArmyFormation.HeavyInfantryStartPosFwdOffset;
                    Settings.Instance.CustomArmyFormation03LightCavalryArrangementOrder = custArmyFormation.LightCavalryArrangementOrderNum;
                    Settings.Instance.CustomArmyFormation03LightCavalryFormOrder = custArmyFormation.LightCavalryFormOrderNum;
                    Settings.Instance.CustomArmyFormation03LightCavalryStartPosLateralOffset = custArmyFormation.LightCavalryStartPosLateralOffset;
                    Settings.Instance.CustomArmyFormation03LightCavalryStartPosFwdOffset = custArmyFormation.LightCavalryStartPosFwdOffset;
                    Settings.Instance.CustomArmyFormation03HeavyCavalryArrangementOrder = custArmyFormation.HeavyCavalryArrangementOrderNum;
                    Settings.Instance.CustomArmyFormation03HeavyCavalryFormOrder = custArmyFormation.HeavyCavalryFormOrderNum;
                    Settings.Instance.CustomArmyFormation03HeavyCavalryStartPosLateralOffset = custArmyFormation.HeavyCavalryStartPosLateralOffset;
                    Settings.Instance.CustomArmyFormation03HeavyCavalryStartPosFwdOffset = custArmyFormation.HeavyCavalryStartPosFwdOffset;
                }
                else if (custArmyFormation.CustomArmyFormationIndex == 4)
                {
                    Settings.Instance.CustomArmyFormation04InfantryArrangementOrder = custArmyFormation.InfantryArrangementOrderNum;
                    Settings.Instance.CustomArmyFormation04InfantryFormOrder = custArmyFormation.InfantryFormOrderNum;
                    Settings.Instance.CustomArmyFormation04RangedArrangementOrder = custArmyFormation.RangedArrangementOrderNum;
                    Settings.Instance.CustomArmyFormation04RangedFormOrder = custArmyFormation.RangedFormOrderNum;
                    Settings.Instance.CustomArmyFormation04RangedStartPosLateralOffset = custArmyFormation.RangedStartPosLateralOffset;
                    Settings.Instance.CustomArmyFormation04RangedStartPosFwdOffset = custArmyFormation.RangedStartPosFwdOffset;
                    Settings.Instance.CustomArmyFormation04CavalryArrangementOrder = custArmyFormation.CavalryArrangementOrderNum;
                    Settings.Instance.CustomArmyFormation04CavalryFormOrder = custArmyFormation.CavalryFormOrderNum;
                    Settings.Instance.CustomArmyFormation04CavalryStartPosLateralOffset = custArmyFormation.CavalryStartPosLateralOffset;
                    Settings.Instance.CustomArmyFormation04CavalryStartPosFwdOffset = custArmyFormation.CavalryStartPosFwdOffset;
                    Settings.Instance.CustomArmyFormation04HorseArcherArrangementOrder = custArmyFormation.HorseArcherArrangementOrderNum;
                    Settings.Instance.CustomArmyFormation04HorseArcherFormOrder = custArmyFormation.HorseArcherFormOrderNum;
                    Settings.Instance.CustomArmyFormation04HorseArcherStartPosLateralOffset = custArmyFormation.HorseArcherStartPosLateralOffset;
                    Settings.Instance.CustomArmyFormation04HorseArcherStartPosFwdOffset = custArmyFormation.HorseArcherStartPosFwdOffset;
                    Settings.Instance.CustomArmyFormation04SkirmisherArrangementOrder = custArmyFormation.SkirmisherArrangementOrderNum;
                    Settings.Instance.CustomArmyFormation04SkirmisherFormOrder = custArmyFormation.SkirmisherFormOrderNum;
                    Settings.Instance.CustomArmyFormation04SkirmisherStartPosLateralOffset = custArmyFormation.SkirmisherStartPosLateralOffset;
                    Settings.Instance.CustomArmyFormation04SkirmisherStartPosFwdOffset = custArmyFormation.SkirmisherStartPosFwdOffset;
                    Settings.Instance.CustomArmyFormation04HeavyInfantryArrangementOrder = custArmyFormation.HeavyInfantryArrangementOrderNum;
                    Settings.Instance.CustomArmyFormation04HeavyInfantryFormOrder = custArmyFormation.HeavyInfantryFormOrderNum;
                    Settings.Instance.CustomArmyFormation04HeavyInfantryStartPosLateralOffset = custArmyFormation.HeavyInfantryStartPosLateralOffset;
                    Settings.Instance.CustomArmyFormation04HeavyInfantryStartPosFwdOffset = custArmyFormation.HeavyInfantryStartPosFwdOffset;
                    Settings.Instance.CustomArmyFormation04LightCavalryArrangementOrder = custArmyFormation.LightCavalryArrangementOrderNum;
                    Settings.Instance.CustomArmyFormation04LightCavalryFormOrder = custArmyFormation.LightCavalryFormOrderNum;
                    Settings.Instance.CustomArmyFormation04LightCavalryStartPosLateralOffset = custArmyFormation.LightCavalryStartPosLateralOffset;
                    Settings.Instance.CustomArmyFormation04LightCavalryStartPosFwdOffset = custArmyFormation.LightCavalryStartPosFwdOffset;
                    Settings.Instance.CustomArmyFormation04HeavyCavalryArrangementOrder = custArmyFormation.HeavyCavalryArrangementOrderNum;
                    Settings.Instance.CustomArmyFormation04HeavyCavalryFormOrder = custArmyFormation.HeavyCavalryFormOrderNum;
                    Settings.Instance.CustomArmyFormation04HeavyCavalryStartPosLateralOffset = custArmyFormation.HeavyCavalryStartPosLateralOffset;
                    Settings.Instance.CustomArmyFormation04HeavyCavalryStartPosFwdOffset = custArmyFormation.HeavyCavalryStartPosFwdOffset;
                }
                else if (custArmyFormation.CustomArmyFormationIndex == 5)
                {
                    Settings.Instance.CustomArmyFormation05InfantryArrangementOrder = custArmyFormation.InfantryArrangementOrderNum;
                    Settings.Instance.CustomArmyFormation05InfantryFormOrder = custArmyFormation.InfantryFormOrderNum;
                    Settings.Instance.CustomArmyFormation05RangedArrangementOrder = custArmyFormation.RangedArrangementOrderNum;
                    Settings.Instance.CustomArmyFormation05RangedFormOrder = custArmyFormation.RangedFormOrderNum;
                    Settings.Instance.CustomArmyFormation05RangedStartPosLateralOffset = custArmyFormation.RangedStartPosLateralOffset;
                    Settings.Instance.CustomArmyFormation05RangedStartPosFwdOffset = custArmyFormation.RangedStartPosFwdOffset;
                    Settings.Instance.CustomArmyFormation05CavalryArrangementOrder = custArmyFormation.CavalryArrangementOrderNum;
                    Settings.Instance.CustomArmyFormation05CavalryFormOrder = custArmyFormation.CavalryFormOrderNum;
                    Settings.Instance.CustomArmyFormation05CavalryStartPosLateralOffset = custArmyFormation.CavalryStartPosLateralOffset;
                    Settings.Instance.CustomArmyFormation05CavalryStartPosFwdOffset = custArmyFormation.CavalryStartPosFwdOffset;
                    Settings.Instance.CustomArmyFormation05HorseArcherArrangementOrder = custArmyFormation.HorseArcherArrangementOrderNum;
                    Settings.Instance.CustomArmyFormation05HorseArcherFormOrder = custArmyFormation.HorseArcherFormOrderNum;
                    Settings.Instance.CustomArmyFormation05HorseArcherStartPosLateralOffset = custArmyFormation.HorseArcherStartPosLateralOffset;
                    Settings.Instance.CustomArmyFormation05HorseArcherStartPosFwdOffset = custArmyFormation.HorseArcherStartPosFwdOffset;
                    Settings.Instance.CustomArmyFormation05SkirmisherArrangementOrder = custArmyFormation.SkirmisherArrangementOrderNum;
                    Settings.Instance.CustomArmyFormation05SkirmisherFormOrder = custArmyFormation.SkirmisherFormOrderNum;
                    Settings.Instance.CustomArmyFormation05SkirmisherStartPosLateralOffset = custArmyFormation.SkirmisherStartPosLateralOffset;
                    Settings.Instance.CustomArmyFormation05SkirmisherStartPosFwdOffset = custArmyFormation.SkirmisherStartPosFwdOffset;
                    Settings.Instance.CustomArmyFormation05HeavyInfantryArrangementOrder = custArmyFormation.HeavyInfantryArrangementOrderNum;
                    Settings.Instance.CustomArmyFormation05HeavyInfantryFormOrder = custArmyFormation.HeavyInfantryFormOrderNum;
                    Settings.Instance.CustomArmyFormation05HeavyInfantryStartPosLateralOffset = custArmyFormation.HeavyInfantryStartPosLateralOffset;
                    Settings.Instance.CustomArmyFormation05HeavyInfantryStartPosFwdOffset = custArmyFormation.HeavyInfantryStartPosFwdOffset;
                    Settings.Instance.CustomArmyFormation05LightCavalryArrangementOrder = custArmyFormation.LightCavalryArrangementOrderNum;
                    Settings.Instance.CustomArmyFormation05LightCavalryFormOrder = custArmyFormation.LightCavalryFormOrderNum;
                    Settings.Instance.CustomArmyFormation05LightCavalryStartPosLateralOffset = custArmyFormation.LightCavalryStartPosLateralOffset;
                    Settings.Instance.CustomArmyFormation05LightCavalryStartPosFwdOffset = custArmyFormation.LightCavalryStartPosFwdOffset;
                    Settings.Instance.CustomArmyFormation05HeavyCavalryArrangementOrder = custArmyFormation.HeavyCavalryArrangementOrderNum;
                    Settings.Instance.CustomArmyFormation05HeavyCavalryFormOrder = custArmyFormation.HeavyCavalryFormOrderNum;
                    Settings.Instance.CustomArmyFormation05HeavyCavalryStartPosLateralOffset = custArmyFormation.HeavyCavalryStartPosLateralOffset;
                    Settings.Instance.CustomArmyFormation05HeavyCavalryStartPosFwdOffset = custArmyFormation.HeavyCavalryStartPosFwdOffset;
                }
                else if (custArmyFormation.CustomArmyFormationIndex == 6)
                {
                    Settings.Instance.CustomArmyFormation06InfantryArrangementOrder = custArmyFormation.InfantryArrangementOrderNum;
                    Settings.Instance.CustomArmyFormation06InfantryFormOrder = custArmyFormation.InfantryFormOrderNum;
                    Settings.Instance.CustomArmyFormation06RangedArrangementOrder = custArmyFormation.RangedArrangementOrderNum;
                    Settings.Instance.CustomArmyFormation06RangedFormOrder = custArmyFormation.RangedFormOrderNum;
                    Settings.Instance.CustomArmyFormation06RangedStartPosLateralOffset = custArmyFormation.RangedStartPosLateralOffset;
                    Settings.Instance.CustomArmyFormation06RangedStartPosFwdOffset = custArmyFormation.RangedStartPosFwdOffset;
                    Settings.Instance.CustomArmyFormation06CavalryArrangementOrder = custArmyFormation.CavalryArrangementOrderNum;
                    Settings.Instance.CustomArmyFormation06CavalryFormOrder = custArmyFormation.CavalryFormOrderNum;
                    Settings.Instance.CustomArmyFormation06CavalryStartPosLateralOffset = custArmyFormation.CavalryStartPosLateralOffset;
                    Settings.Instance.CustomArmyFormation06CavalryStartPosFwdOffset = custArmyFormation.CavalryStartPosFwdOffset;
                    Settings.Instance.CustomArmyFormation06HorseArcherArrangementOrder = custArmyFormation.HorseArcherArrangementOrderNum;
                    Settings.Instance.CustomArmyFormation06HorseArcherFormOrder = custArmyFormation.HorseArcherFormOrderNum;
                    Settings.Instance.CustomArmyFormation06HorseArcherStartPosLateralOffset = custArmyFormation.HorseArcherStartPosLateralOffset;
                    Settings.Instance.CustomArmyFormation06HorseArcherStartPosFwdOffset = custArmyFormation.HorseArcherStartPosFwdOffset;
                    Settings.Instance.CustomArmyFormation06SkirmisherArrangementOrder = custArmyFormation.SkirmisherArrangementOrderNum;
                    Settings.Instance.CustomArmyFormation06SkirmisherFormOrder = custArmyFormation.SkirmisherFormOrderNum;
                    Settings.Instance.CustomArmyFormation06SkirmisherStartPosLateralOffset = custArmyFormation.SkirmisherStartPosLateralOffset;
                    Settings.Instance.CustomArmyFormation06SkirmisherStartPosFwdOffset = custArmyFormation.SkirmisherStartPosFwdOffset;
                    Settings.Instance.CustomArmyFormation06HeavyInfantryArrangementOrder = custArmyFormation.HeavyInfantryArrangementOrderNum;
                    Settings.Instance.CustomArmyFormation06HeavyInfantryFormOrder = custArmyFormation.HeavyInfantryFormOrderNum;
                    Settings.Instance.CustomArmyFormation06HeavyInfantryStartPosLateralOffset = custArmyFormation.HeavyInfantryStartPosLateralOffset;
                    Settings.Instance.CustomArmyFormation06HeavyInfantryStartPosFwdOffset = custArmyFormation.HeavyInfantryStartPosFwdOffset;
                    Settings.Instance.CustomArmyFormation06LightCavalryArrangementOrder = custArmyFormation.LightCavalryArrangementOrderNum;
                    Settings.Instance.CustomArmyFormation06LightCavalryFormOrder = custArmyFormation.LightCavalryFormOrderNum;
                    Settings.Instance.CustomArmyFormation06LightCavalryStartPosLateralOffset = custArmyFormation.LightCavalryStartPosLateralOffset;
                    Settings.Instance.CustomArmyFormation06LightCavalryStartPosFwdOffset = custArmyFormation.LightCavalryStartPosFwdOffset;
                    Settings.Instance.CustomArmyFormation06HeavyCavalryArrangementOrder = custArmyFormation.HeavyCavalryArrangementOrderNum;
                    Settings.Instance.CustomArmyFormation06HeavyCavalryFormOrder = custArmyFormation.HeavyCavalryFormOrderNum;
                    Settings.Instance.CustomArmyFormation06HeavyCavalryStartPosLateralOffset = custArmyFormation.HeavyCavalryStartPosLateralOffset;
                    Settings.Instance.CustomArmyFormation06HeavyCavalryStartPosFwdOffset = custArmyFormation.HeavyCavalryStartPosFwdOffset;
                }
                else if (custArmyFormation.CustomArmyFormationIndex == 7)
                {
                    Settings.Instance.CustomArmyFormation07InfantryArrangementOrder = custArmyFormation.InfantryArrangementOrderNum;
                    Settings.Instance.CustomArmyFormation07InfantryFormOrder = custArmyFormation.InfantryFormOrderNum;
                    Settings.Instance.CustomArmyFormation07RangedArrangementOrder = custArmyFormation.RangedArrangementOrderNum;
                    Settings.Instance.CustomArmyFormation07RangedFormOrder = custArmyFormation.RangedFormOrderNum;
                    Settings.Instance.CustomArmyFormation07RangedStartPosLateralOffset = custArmyFormation.RangedStartPosLateralOffset;
                    Settings.Instance.CustomArmyFormation07RangedStartPosFwdOffset = custArmyFormation.RangedStartPosFwdOffset;
                    Settings.Instance.CustomArmyFormation07CavalryArrangementOrder = custArmyFormation.CavalryArrangementOrderNum;
                    Settings.Instance.CustomArmyFormation07CavalryFormOrder = custArmyFormation.CavalryFormOrderNum;
                    Settings.Instance.CustomArmyFormation07CavalryStartPosLateralOffset = custArmyFormation.CavalryStartPosLateralOffset;
                    Settings.Instance.CustomArmyFormation07CavalryStartPosFwdOffset = custArmyFormation.CavalryStartPosFwdOffset;
                    Settings.Instance.CustomArmyFormation07HorseArcherArrangementOrder = custArmyFormation.HorseArcherArrangementOrderNum;
                    Settings.Instance.CustomArmyFormation07HorseArcherFormOrder = custArmyFormation.HorseArcherFormOrderNum;
                    Settings.Instance.CustomArmyFormation07HorseArcherStartPosLateralOffset = custArmyFormation.HorseArcherStartPosLateralOffset;
                    Settings.Instance.CustomArmyFormation07HorseArcherStartPosFwdOffset = custArmyFormation.HorseArcherStartPosFwdOffset;
                    Settings.Instance.CustomArmyFormation07SkirmisherArrangementOrder = custArmyFormation.SkirmisherArrangementOrderNum;
                    Settings.Instance.CustomArmyFormation07SkirmisherFormOrder = custArmyFormation.SkirmisherFormOrderNum;
                    Settings.Instance.CustomArmyFormation07SkirmisherStartPosLateralOffset = custArmyFormation.SkirmisherStartPosLateralOffset;
                    Settings.Instance.CustomArmyFormation07SkirmisherStartPosFwdOffset = custArmyFormation.SkirmisherStartPosFwdOffset;
                    Settings.Instance.CustomArmyFormation07HeavyInfantryArrangementOrder = custArmyFormation.HeavyInfantryArrangementOrderNum;
                    Settings.Instance.CustomArmyFormation07HeavyInfantryFormOrder = custArmyFormation.HeavyInfantryFormOrderNum;
                    Settings.Instance.CustomArmyFormation07HeavyInfantryStartPosLateralOffset = custArmyFormation.HeavyInfantryStartPosLateralOffset;
                    Settings.Instance.CustomArmyFormation07HeavyInfantryStartPosFwdOffset = custArmyFormation.HeavyInfantryStartPosFwdOffset;
                    Settings.Instance.CustomArmyFormation07LightCavalryArrangementOrder = custArmyFormation.LightCavalryArrangementOrderNum;
                    Settings.Instance.CustomArmyFormation07LightCavalryFormOrder = custArmyFormation.LightCavalryFormOrderNum;
                    Settings.Instance.CustomArmyFormation07LightCavalryStartPosLateralOffset = custArmyFormation.LightCavalryStartPosLateralOffset;
                    Settings.Instance.CustomArmyFormation07LightCavalryStartPosFwdOffset = custArmyFormation.LightCavalryStartPosFwdOffset;
                    Settings.Instance.CustomArmyFormation07HeavyCavalryArrangementOrder = custArmyFormation.HeavyCavalryArrangementOrderNum;
                    Settings.Instance.CustomArmyFormation07HeavyCavalryFormOrder = custArmyFormation.HeavyCavalryFormOrderNum;
                    Settings.Instance.CustomArmyFormation07HeavyCavalryStartPosLateralOffset = custArmyFormation.HeavyCavalryStartPosLateralOffset;
                    Settings.Instance.CustomArmyFormation07HeavyCavalryStartPosFwdOffset = custArmyFormation.HeavyCavalryStartPosFwdOffset;
                }
            }
        }

        // Update if Alt, Ctrl, or Shift are currently held down (Left & Right)
        private static void UpdateAltKeyStates()
        {
            // 'LAlt' - Check for key held down
            if ((Input.IsKeyPressed(InputKey.LeftAlt)) && !LAltKeyPressed)
            {
                // Prevent repeat - until 'LAlt' key has been released
                LAltKeyPressed = true;
            }
            else if (Input.IsKeyReleased(InputKey.LeftAlt) && LAltKeyPressed)
            {
                // Allow 'LAlt' key press to be registered once again
                LAltKeyPressed = false;
            }

            // 'LCtrl' - Check for key held down
            if ((Input.IsKeyPressed(InputKey.LeftControl)) && !LCtrlKeyPressed)
            {
                // Prevent repeat - until 'LCtrl' key has been released
                LCtrlKeyPressed = true;
            }
            else if (Input.IsKeyReleased(InputKey.LeftControl) && LCtrlKeyPressed)
            {
                // Allow 'LCtrl' key press to be registered once again
                LCtrlKeyPressed = false;
            }

            // 'LShift' - Check for key held down
            if ((Input.IsKeyPressed(InputKey.LeftShift)) && !LShiftKeyPressed)
            {
                // Prevent repeat - until 'LShift' key has been released
                LShiftKeyPressed = true;
            }
            else if (Input.IsKeyReleased(InputKey.LeftShift) && LShiftKeyPressed)
            {
                // Allow 'LShift' key press to be registered once again
                LShiftKeyPressed = false;
            }

            // 'RAlt' - Check for key held down
            if ((Input.IsKeyPressed(InputKey.RightAlt)) && !RAltKeyPressed)
            {
                // Prevent repeat - until 'RAlt' key has been released
                RAltKeyPressed = true;
            }
            else if (Input.IsKeyReleased(InputKey.RightAlt) && RAltKeyPressed)
            {
                // Allow 'RAlt' key press to be registered once again
                RAltKeyPressed = false;
            }

            // 'RCtrl' - Check for key held down
            if ((Input.IsKeyPressed(InputKey.RightControl)) && !RCtrlKeyPressed)
            {
                // Prevent repeat - until 'RCtrl' key has been released
                RCtrlKeyPressed = true;
            }
            else if (Input.IsKeyReleased(InputKey.RightControl) && RCtrlKeyPressed)
            {
                // Allow 'RCtrl' key press to be registered once again
                RCtrlKeyPressed = false;
            }

            // 'RShift' - Check for key held down
            if ((Input.IsKeyPressed(InputKey.RightShift)) && !RShiftKeyPressed)
            {
                // Prevent repeat - until 'RShift' key has been released
                RShiftKeyPressed = true;
            }
            else if (Input.IsKeyReleased(InputKey.RightShift) && RShiftKeyPressed)
            {
                // Allow 'RShift' key press to be registered once again
                RShiftKeyPressed = false;
            }

            // Check if any of the Alternate keys are currently pressed - Alt, Ctrl, Shift (Left & Right)
            if (LAltKeyPressed || LCtrlKeyPressed || LShiftKeyPressed || RAltKeyPressed || RCtrlKeyPressed || RShiftKeyPressed)
            {
                AnyAlternateKeysHeldDown = true;
            }
            else
            {
                AnyAlternateKeysHeldDown = false;
            }
        }

        // Key Input Manager - handle user inputs
        private static void ManageKeyInputs(MissionBehaviour __instance)
        {
            // 'Default: PageUp' - Toggle Epic Battle AI (Custom Enemy AI) active/deactive.
            if (Settings.Instance.AllowEpicBattleAI && Input.IsKeyPressed(Config.EpicBattlesAIToggleKey) && !EpicBattlesAIToggleKeyPressed)
            {
                if (!AnyAlternateKeysHeldDown && __instance.Mission.MainAgent != null)
                {
                    // Toggle Epic Battle AI
                    EpicBattleAIActive = !EpicBattleAIActive;

                    // Display message
                    if (EpicBattleAIActive)
                    {
                        InformationManager.DisplayMessage(new InformationMessage("Epic Battle AI - Enabled"));
                    }
                    else
                    {
                        InformationManager.DisplayMessage(new InformationMessage("Epic Battle AI - Disabled"));
                    }
                }

                // Prevent repeat - until 'EpicBattlesAIToggleKey' has been released
                EpicBattlesAIToggleKeyPressed = true;
            }
            else if (Settings.Instance.AllowEpicBattleAI && Input.IsKeyReleased(Config.EpicBattlesAIToggleKey) && EpicBattlesAIToggleKeyPressed)
            {
                // Allow 'EpicBattlesAIToggleKey' press to be registered once again
                EpicBattlesAIToggleKeyPressed = false;
            }

            // 'Default: equals(=)' - Reset selected units MaxSpeedLimit back to 100%.
            if (Settings.Instance.SpeedLimitReductionEnabled && Input.IsKeyPressed(Config.ResetUnitSpeedLimitKey) && !ResetUnitSpeedLimitKeyPressed)
            {
                if (!AnyAlternateKeysHeldDown && __instance.Mission.MainAgent != null && __instance.Mission.MainAgent.Health > 0)
                {
                    ModifySelectedMaxSpeedLimit(__instance, 1);

                    // Success message
                    InformationManager.DisplayMessage(new InformationMessage("Selected formations: Quick March!"));
                }
                else if ((LCtrlKeyPressed || RCtrlKeyPressed) && __instance.Mission.MainAgent != null && __instance.Mission.MainAgent.Health > 0)
                {
                    Settings.Instance.AllEnemyAllyAgentsWalk = false;

                    // Display message
                    InformationManager.DisplayMessage(new InformationMessage("All enemy units can now charge.'"));
                }
                // Prevent repeat - until 'ResetUnitSpeedLimitKey' has been released
                ResetUnitSpeedLimitKeyPressed = true;
            }
            else if (Settings.Instance.SpeedLimitReductionEnabled && Input.IsKeyReleased(Config.ResetUnitSpeedLimitKey) && ResetUnitSpeedLimitKeyPressed)
            {
                // Allow 'ResetUnitSpeedLimitKey' press to be registered once again
                ResetUnitSpeedLimitKeyPressed = false;
            }

            // 'Default: minus(-)' - Reduce (multiply) selected units MaxSpeedLimit by the amount set in 'Mod Options' settings.
            if (Settings.Instance.SpeedLimitReductionEnabled && Input.IsKeyPressed(Config.ReduceUnitSpeedLimitKey) && !ReduceUnitSpeedLimitKeyPressed)
            {
                if (!AnyAlternateKeysHeldDown && __instance.Mission.MainAgent != null && __instance.Mission.MainAgent.Health > 0)
                {
                    ModifySelectedMaxSpeedLimit(__instance, Settings.Instance.SpeedLimitReductionAmount);

                    // Success message
                    InformationManager.DisplayMessage(new InformationMessage("Selected formations: Slow March!"));
                }
                else if ((LCtrlKeyPressed || RCtrlKeyPressed) && __instance.Mission.MainAgent != null && __instance.Mission.MainAgent.Health > 0)
                {
                    Settings.Instance.AllEnemyAllyAgentsWalk = true;

                    // Display message
                    InformationManager.DisplayMessage(new InformationMessage("All enemy units must walk, unless routed.'"));
                }
                // Prevent repeat - until 'ReduceUnitSpeedLimitKey' has been released
                ReduceUnitSpeedLimitKeyPressed = true;
            }
            else if (Settings.Instance.SpeedLimitReductionEnabled && Input.IsKeyReleased(Config.ReduceUnitSpeedLimitKey) && ReduceUnitSpeedLimitKeyPressed)
            {
                // Allow 'ReduceUnitSpeedLimitKey' press to be registered once again
                ReduceUnitSpeedLimitKeyPressed = false;
            }


            // 'Default: F9' - Advance selected formations 'y' paces forward
            if (Settings.Instance.AdvanceYPacesEnabled && Input.IsKeyPressed(Config.AdvanceSelectedKey) && !AdvanceSelectedKeyPressed)
            {
                if (!AnyAlternateKeysHeldDown && __instance.Mission.MainAgent != null && __instance.Mission.MainAgent.Health > 0)
                {
                    if (CustomFormation1KeyPressed)
                    {
                        // Move selected units to 'Custom Army Formation 1' positions, but shift forward 'y' paces
                        MoveSelectedToCustArmyFormPos(CustomArmyFormations[0], __instance, 0, Settings.Instance.AdvanceYPaces);
                    }
                    else if (CustomFormation2KeyPressed)
                    {
                        // Move selected units to 'Custom Army Formation 2' positions, but shift forward 'y' paces
                        MoveSelectedToCustArmyFormPos(CustomArmyFormations[1], __instance, 0, Settings.Instance.AdvanceYPaces);
                    }
                    else if (CustomFormation3KeyPressed)
                    {
                        // Move selected units to 'Custom Army Formation 3' positions, but shift forward 'y' paces
                        MoveSelectedToCustArmyFormPos(CustomArmyFormations[2], __instance, 0, Settings.Instance.AdvanceYPaces);
                    }
                    else if (CustomFormation4KeyPressed)
                    {
                        // Move selected units to 'Custom Army Formation 4' positions, but shift forward 'y' paces
                        MoveSelectedToCustArmyFormPos(CustomArmyFormations[3], __instance, 0, Settings.Instance.AdvanceYPaces);
                    }
                    else if (CustomFormation5KeyPressed)
                    {
                        // Move selected units to 'Custom Army Formation 5' positions, but shift forward 'y' paces
                        MoveSelectedToCustArmyFormPos(CustomArmyFormations[4], __instance, 0, Settings.Instance.AdvanceYPaces);
                    }
                    else if (CustomFormation6KeyPressed)
                    {
                        // Move selected units to 'Custom Army Formation 6' positions, but shift forward 'y' paces
                        MoveSelectedToCustArmyFormPos(CustomArmyFormations[5], __instance, 0, Settings.Instance.AdvanceYPaces);
                    }
                    else if (CustomFormation7KeyPressed)
                    {
                        // Move selected units to 'Custom Army Formation 7' positions, but shift forward 'y' paces
                        MoveSelectedToCustArmyFormPos(CustomArmyFormations[6], __instance, 0, Settings.Instance.AdvanceYPaces);
                    }
                    else if (CustomFormation8KeyPressed)
                    {
                        // Move selected units to 'Custom Army Formation 8' positions, but shift forward 'y' paces
                        MoveSelectedToCustArmyFormPos(CustomArmyFormations[7], __instance, 0, Settings.Instance.AdvanceYPaces);
                    }
                    else // Advance Selected formations Y paces
                    {
                        SelectedFormationsAdvanceYPaces(__instance);
                    }

                    // Success message
                    int y = Settings.Instance.AdvanceYPaces;
                    InformationManager.DisplayMessage(new InformationMessage("Selected formations: Advance " + y + " paces!"));
                }
                // Prevent repeat - until 'AdvanceSelectedKey' has been released
                AdvanceSelectedKeyPressed = true;
            }
            else if (Settings.Instance.AdvanceYPacesEnabled && Input.IsKeyReleased(Config.AdvanceSelectedKey) && AdvanceSelectedKeyPressed)
            {
                // Allow 'AdvanceSelectedKey' press to be registered once again
                AdvanceSelectedKeyPressed = false;
            }

            // 'Default: F10' - Fallback selected formations 'y' paces backwards
            if (Settings.Instance.FallbackYPacesEnabled && Input.IsKeyPressed(Config.FallbackSelectedKey) && !FallbackSelectedKeyPressed)
            {
                if (!AnyAlternateKeysHeldDown && __instance.Mission.MainAgent != null && __instance.Mission.MainAgent.Health > 0)
                {
                    if (CustomFormation1KeyPressed)
                    {
                        // Move selected units to 'Custom Army Formation 1' positions, but shift backward 'y' paces
                        MoveSelectedToCustArmyFormPos(CustomArmyFormations[0], __instance, 0, Settings.Instance.FallbackYPaces);
                    }
                    else if (CustomFormation2KeyPressed)
                    {
                        // Move selected units to 'Custom Army Formation 2' positions, but shift backward 'y' paces
                        MoveSelectedToCustArmyFormPos(CustomArmyFormations[1], __instance, 0, Settings.Instance.FallbackYPaces);
                    }
                    else if (CustomFormation3KeyPressed)
                    {
                        // Move selected units to 'Custom Army Formation 3' positions, but shift backward 'y' paces
                        MoveSelectedToCustArmyFormPos(CustomArmyFormations[2], __instance, 0, Settings.Instance.FallbackYPaces);
                    }
                    else if (CustomFormation4KeyPressed)
                    {
                        // Move selected units to 'Custom Army Formation 4' positions, but shift backward 'y' paces
                        MoveSelectedToCustArmyFormPos(CustomArmyFormations[3], __instance, 0, Settings.Instance.FallbackYPaces);
                    }
                    else if (CustomFormation5KeyPressed)
                    {
                        // Move selected units to 'Custom Army Formation 5' positions, but shift backward 'y' paces
                        MoveSelectedToCustArmyFormPos(CustomArmyFormations[4], __instance, 0, Settings.Instance.FallbackYPaces);
                    }
                    else if (CustomFormation6KeyPressed)
                    {
                        // Move selected units to 'Custom Army Formation 6' positions, but shift backward 'y' paces
                        MoveSelectedToCustArmyFormPos(CustomArmyFormations[5], __instance, 0, Settings.Instance.FallbackYPaces);
                    }
                    else if (CustomFormation7KeyPressed)
                    {
                        // Move selected units to 'Custom Army Formation 7' positions, but shift backward 'y' paces
                        MoveSelectedToCustArmyFormPos(CustomArmyFormations[6], __instance, 0, Settings.Instance.FallbackYPaces);
                    }
                    else if (CustomFormation8KeyPressed)
                    {
                        // Move selected units to 'Custom Army Formation 8' positions, but shift backward 'y' paces
                        MoveSelectedToCustArmyFormPos(CustomArmyFormations[7], __instance, 0, Settings.Instance.FallbackYPaces);
                    }
                    else // Fallback Selected formations Y paces
                    {
                        SelectedFormationsFallbackYPaces(__instance);
                    }

                    // Success message
                    int y = Math.Abs(Settings.Instance.FallbackYPaces);
                    InformationManager.DisplayMessage(new InformationMessage("Selected formations: Fallback " + y + " paces!"));

                }
                // Prevent repeat - until 'FallbackSelectedKey' has been released
                FallbackSelectedKeyPressed = true;
            }
            else if (Settings.Instance.FallbackYPacesEnabled && Input.IsKeyReleased(Config.FallbackSelectedKey) && FallbackSelectedKeyPressed)
            {
                // Allow 'FallbackSelectedKey' press to be registered once again
                FallbackSelectedKeyPressed = false;
            }

            // 'Default: F11' - Move selected units to 'Custom Army Formation 1' positions
            if (Settings.Instance.CustomArmyFormation00Enabled && Input.IsKeyPressed(Config.CustomFormation1Key) && !CustomFormation1KeyPressed)
            {
                if (!AnyAlternateKeysHeldDown && __instance.Mission.MainAgent != null && __instance.Mission.MainAgent.Health > 0)
                {
                    // Move selected units to 'Custom Army Formation 1' positions (Default: F11)
                    MoveSelectedToCustArmyFormPos(CustomArmyFormations[0], __instance, 0, 0);

                    // Display message
                    InformationManager.DisplayMessage(new InformationMessage("Selected units move to: 'Custom Army Formation 1'"));
                }
                else if ((LCtrlKeyPressed || RCtrlKeyPressed) && __instance.Mission.MainAgent != null && __instance.Mission.MainAgent.Health > 0)
                {
                    // Save all formation's current position/arrangement to 'Custom Army Formation 1' positions
                    SaveAllToCustArmyForm(CustomArmyFormations[0], __instance);

                    // Display message
                    InformationManager.DisplayMessage(new InformationMessage("Current positions/arrangements saved to: 'Custom Army Formation 1'"));
                }

                // Prevent repeat - until 'CustomFormation1Key' has been released
                CustomFormation1KeyPressed = true;
            }
            else if (Settings.Instance.CustomArmyFormation00Enabled && Input.IsKeyReleased(Config.CustomFormation1Key) && CustomFormation1KeyPressed)
            {
                // Allow 'CustomFormation1Key' press to be registered once again
                CustomFormation1KeyPressed = false;
            }

            // 'Default: F12' - Move Selected units to 'Custom Army Formation 2' positions
            if (Settings.Instance.CustomArmyFormation01Enabled && Input.IsKeyPressed(Config.CustomFormation2Key) && !CustomFormation2KeyPressed)
            {
                if (!AnyAlternateKeysHeldDown && __instance.Mission.MainAgent != null && __instance.Mission.MainAgent.Health > 0)
                {
                    // Move Selected units to 'Custom Army Formation 2' positions (Default: F12)
                    MoveSelectedToCustArmyFormPos(CustomArmyFormations[1], __instance, 0, 0);

                    // Display message
                    InformationManager.DisplayMessage(new InformationMessage("Selected units move to: 'Custom Army Formation 2'"));
                }
                else if ((LCtrlKeyPressed || RCtrlKeyPressed) && __instance.Mission.MainAgent != null && __instance.Mission.MainAgent.Health > 0)
                {
                    // Save all formation's current position/arrangement to 'Custom Army Formation 2' positions
                    SaveAllToCustArmyForm(CustomArmyFormations[1], __instance);

                    // Display message
                    InformationManager.DisplayMessage(new InformationMessage("Current positions/arrangements saved to: 'Custom Army Formation 2'"));
                }

                // Prevent repeat - until 'CustomFormation2Key' has been released
                CustomFormation2KeyPressed = true;
            }
            else if (Settings.Instance.CustomArmyFormation01Enabled && Input.IsKeyReleased(Config.CustomFormation2Key) && CustomFormation2KeyPressed)
            {
                // Allow 'CustomFormation2Key' press to be registered once again
                CustomFormation2KeyPressed = false;
            }

            // 'Default: NumPad5' - Move Selected units to 'Custom Army Formation 3' positions
            if (Settings.Instance.CustomArmyFormation02Enabled && Input.IsKeyPressed(Config.CustomFormation3Key) && !CustomFormation3KeyPressed)
            {
                if (!AnyAlternateKeysHeldDown && __instance.Mission.MainAgent != null && __instance.Mission.MainAgent.Health > 0)
                {
                    // Move Selected units to 'Custom Army Formation 3' positions (Default: NumPad5)
                    MoveSelectedToCustArmyFormPos(CustomArmyFormations[2], __instance, 0, 0);

                    // Display message
                    InformationManager.DisplayMessage(new InformationMessage("Selected units move to: 'Custom Army Formation 3'"));
                }
                else if ((LCtrlKeyPressed || RCtrlKeyPressed) && __instance.Mission.MainAgent != null && __instance.Mission.MainAgent.Health > 0)
                {
                    // Save all formation's current position/arrangement to 'Custom Army Formation 3' positions
                    SaveAllToCustArmyForm(CustomArmyFormations[2], __instance);

                    // Display message
                    InformationManager.DisplayMessage(new InformationMessage("Current positions/arrangements saved to: 'Custom Army Formation 3'"));
                }

                // Prevent repeat - until 'CustomFormation3Key' has been released
                CustomFormation3KeyPressed = true;
            }
            else if (Settings.Instance.CustomArmyFormation02Enabled && Input.IsKeyReleased(Config.CustomFormation3Key) && CustomFormation3KeyPressed)
            {
                // Allow 'CustomFormation3Key' press to be registered once again
                CustomFormation3KeyPressed = false;
            }

            // 'Default: NumPad6' - Move Selected units to 'Custom Army Formation 4' positions
            if (Settings.Instance.CustomArmyFormation03Enabled && Input.IsKeyPressed(Config.CustomFormation4Key) && !CustomFormation4KeyPressed)
            {
                if (!AnyAlternateKeysHeldDown && __instance.Mission.MainAgent != null && __instance.Mission.MainAgent.Health > 0)
                {
                    // Move Selected units to 'Custom Army Formation 4' positions (Default: NumPad6)
                    MoveSelectedToCustArmyFormPos(CustomArmyFormations[3], __instance, 0, 0);

                    // Display message
                    InformationManager.DisplayMessage(new InformationMessage("Selected units move to: 'Custom Army Formation 4'"));
                }
                else if ((LCtrlKeyPressed || RCtrlKeyPressed) && __instance.Mission.MainAgent != null && __instance.Mission.MainAgent.Health > 0)
                {
                    // Save all formation's current position/arrangement to 'Custom Army Formation 4' positions
                    SaveAllToCustArmyForm(CustomArmyFormations[3], __instance);

                    // Display message
                    InformationManager.DisplayMessage(new InformationMessage("Current positions/arrangements saved to: 'Custom Army Formation 4'"));
                }

                // Prevent repeat - until 'CustomFormation4Key' has been released
                CustomFormation4KeyPressed = true;
            }
            else if (Settings.Instance.CustomArmyFormation03Enabled && Input.IsKeyReleased(Config.CustomFormation4Key) && CustomFormation4KeyPressed)
            {
                // Allow 'CustomFormation4Key' press to be registered once again
                CustomFormation4KeyPressed = false;
            }

            // 'Default: NumPad7' - Move Selected units to 'Custom Army Formation 5' positions
            if (Settings.Instance.CustomArmyFormation04Enabled && Input.IsKeyPressed(Config.CustomFormation5Key) && !CustomFormation5KeyPressed)
            {
                if (!AnyAlternateKeysHeldDown && __instance.Mission.MainAgent != null && __instance.Mission.MainAgent.Health > 0)
                {
                    // Move Selected units to 'Custom Army Formation 5' positions (Default: NumPad7)
                    MoveSelectedToCustArmyFormPos(CustomArmyFormations[4], __instance, 0, 0);

                    // Display message
                    InformationManager.DisplayMessage(new InformationMessage("Selected units move to: 'Custom Army Formation 5'"));
                }
                else if ((LCtrlKeyPressed || RCtrlKeyPressed) && __instance.Mission.MainAgent != null && __instance.Mission.MainAgent.Health > 0)
                {
                    // Save all formation's current position/arrangement to 'Custom Army Formation 5' positions
                    SaveAllToCustArmyForm(CustomArmyFormations[4], __instance);

                    // Display message
                    InformationManager.DisplayMessage(new InformationMessage("Current positions/arrangements saved to: 'Custom Army Formation 5'"));
                }

                // Prevent repeat - until 'CustomFormation5Key' has been released
                CustomFormation5KeyPressed = true;
            }
            else if (Settings.Instance.CustomArmyFormation04Enabled && Input.IsKeyReleased(Config.CustomFormation5Key) && CustomFormation5KeyPressed)
            {
                // Allow 'CustomFormation5Key' press to be registered once again
                CustomFormation5KeyPressed = false;
            }

            // 'Default: NumPad8' - Move Selected units to 'Custom Army Formation 6' positions
            if (Settings.Instance.CustomArmyFormation05Enabled && Input.IsKeyPressed(Config.CustomFormation6Key) && !CustomFormation6KeyPressed)
            {
                if (!AnyAlternateKeysHeldDown && __instance.Mission.MainAgent != null && __instance.Mission.MainAgent.Health > 0)
                {
                    // Move Selected units to 'Custom Army Formation 6' positions (Default: NumPad8)
                    MoveSelectedToCustArmyFormPos(CustomArmyFormations[5], __instance, 0, 0);

                    // Display message
                    InformationManager.DisplayMessage(new InformationMessage("Selected units move to: 'Custom Army Formation 6'"));
                }
                else if ((LCtrlKeyPressed || RCtrlKeyPressed) && __instance.Mission.MainAgent != null && __instance.Mission.MainAgent.Health > 0)
                {
                    // Save all formation's current position/arrangement to 'Custom Army Formation 6' positions
                    SaveAllToCustArmyForm(CustomArmyFormations[5], __instance);

                    // Display message
                    InformationManager.DisplayMessage(new InformationMessage("Current positions/arrangements saved to: 'Custom Army Formation 6'"));
                }

                // Prevent repeat - until 'CustomFormation6Key' has been released
                CustomFormation6KeyPressed = true;
            }
            else if (Settings.Instance.CustomArmyFormation05Enabled && Input.IsKeyReleased(Config.CustomFormation6Key) && CustomFormation6KeyPressed)
            {
                // Allow 'CustomFormation6Key' press to be registered once again
                CustomFormation6KeyPressed = false;
            }

            // 'Default: NumPad9' - Move Selected units to 'Custom Army Formation 7' positions
            if (Settings.Instance.CustomArmyFormation06Enabled && Input.IsKeyPressed(Config.CustomFormation7Key) && !CustomFormation7KeyPressed)
            {
                if (!AnyAlternateKeysHeldDown && __instance.Mission.MainAgent != null && __instance.Mission.MainAgent.Health > 0)
                {
                    // Move Selected units to 'Custom Army Formation 7' positions (Default: NumPad9)
                    MoveSelectedToCustArmyFormPos(CustomArmyFormations[6], __instance, 0, 0);

                    // Display message
                    InformationManager.DisplayMessage(new InformationMessage("Selected units move to: 'Custom Army Formation 7'"));
                }
                else if ((LCtrlKeyPressed || RCtrlKeyPressed) && __instance.Mission.MainAgent != null && __instance.Mission.MainAgent.Health > 0)
                {
                    // Save all formation's current position/arrangement to 'Custom Army Formation 7' positions
                    SaveAllToCustArmyForm(CustomArmyFormations[6], __instance);

                    // Display message
                    InformationManager.DisplayMessage(new InformationMessage("Current positions/arrangements saved to: 'Custom Army Formation 7'"));
                }

                // Prevent repeat - until 'CustomFormation7Key' has been released
                CustomFormation7KeyPressed = true;
            }
            else if (Settings.Instance.CustomArmyFormation06Enabled && Input.IsKeyReleased(Config.CustomFormation7Key) && CustomFormation7KeyPressed)
            {
                // Allow 'CustomFormation7Key' press to be registered once again
                CustomFormation7KeyPressed = false;
            }

            // 'Default: NumPad0' - Move Selected units to 'Custom Army Formation 8' positions
            if (Settings.Instance.CustomArmyFormation07Enabled && Input.IsKeyPressed(Config.CustomFormation8Key) && !CustomFormation8KeyPressed)
            {
                if (!AnyAlternateKeysHeldDown && __instance.Mission.MainAgent != null && __instance.Mission.MainAgent.Health > 0)
                {
                    // Move Selected units to 'Custom Army Formation 8' positions (Default: NumPad0)
                    MoveSelectedToCustArmyFormPos(CustomArmyFormations[7], __instance, 0, 0);

                    // Display message
                    InformationManager.DisplayMessage(new InformationMessage("Selected units move to: 'Custom Army Formation 8'"));
                }
                else if ((LCtrlKeyPressed || RCtrlKeyPressed) && __instance.Mission.MainAgent != null && __instance.Mission.MainAgent.Health > 0)
                {
                    // Save all formation's current position/arrangement to 'Custom Army Formation 8' positions
                    SaveAllToCustArmyForm(CustomArmyFormations[7], __instance);

                    // Display message
                    InformationManager.DisplayMessage(new InformationMessage("Current positions/arrangements saved to: 'Custom Army Formation 8'"));
                }

                // Prevent repeat - until 'CustomFormation8Key' has been released
                CustomFormation8KeyPressed = true;
            }
            else if (Settings.Instance.CustomArmyFormation07Enabled && Input.IsKeyReleased(Config.CustomFormation8Key) && CustomFormation8KeyPressed)
            {
                // Allow 'CustomFormation8Key' press to be registered once again
                CustomFormation8KeyPressed = false;
            }
        }

        // Advance currently selected formations Y paces forward (Cumulative)
        private static void FormationsAdvanceYPaces(MissionBehaviour missionBehaviourInstance, IEnumerable<Formation> formations, float distance)
        {
            foreach (Formation formation in formations)
            {
                WorldPosition newWorldPos;
                if (AllFormationFrontAttPtUpdated || FormationFrontAttPtUpdated[(int)formation.FormationIndex])
                {
                    newWorldPos = CalcWorldPosFrontAttPtOffset(formation, missionBehaviourInstance, 0, distance);
                }
                else
                {
                    newWorldPos = CalcWorldPosRelToFormation(formation, missionBehaviourInstance, 0, distance);
                    // Mark this formation's FrontAttachmentPoint as having been updated for the first time, if on player's team
                    if (formation.Team.Side == missionBehaviourInstance.Mission.MainAgent.Team.Side) 
                        FormationFrontAttPtUpdated[(int)formation.FormationIndex] = true;
                }
                formation.MovementOrder = MovementOrder.MovementOrderMove(newWorldPos);
            }
        }

        // Advance currently selected formations Y paces forward (Cumulative)
        private static void SelectedFormationsAdvanceYPaces(MissionBehaviour missionBehaviourInstance)
        {
            MBReadOnlyList<Formation> SelectedFormationsList = missionBehaviourInstance.Mission.PlayerTeam.PlayerOrderController.SelectedFormations;

            FormationsAdvanceYPaces(missionBehaviourInstance, SelectedFormationsList, Settings.Instance.AdvanceYPaces);
        }

        // Fallback currently selected formations Y paces backward (Cumulative)
        private static void SelectedFormationsFallbackYPaces(MissionBehaviour missionBehaviourInstance)
        {
            MBReadOnlyList<Formation> SelectedFormationsList = missionBehaviourInstance.Mission.PlayerTeam.PlayerOrderController.SelectedFormations;

            FormationsAdvanceYPaces(missionBehaviourInstance, SelectedFormationsList, Settings.Instance.FallbackYPaces);
        }

        // Calculate WorldPosition of the formation's FrontAttachmentPoint (Cumulative) with Fwd/Lateral Offset (Fwd/Back, Left/Right)
        private static WorldPosition CalcWorldPosFrontAttPtOffset(Formation formation, MissionBehaviour missionBehaviourInstance, float latOffset, float fwdOffset)
        {
            float moveX = formation.Direction.x * fwdOffset;
            float moveY = formation.Direction.y * fwdOffset;
            float offsetX = formation.Direction.y * latOffset;
            float offsetY = -formation.Direction.x * latOffset;
            WorldPosition formationCurWorldPos = formation.FrontAttachmentPoint;
            Vec3 formationNewPosition = new Vec3(formationCurWorldPos.X + moveX + offsetX, formationCurWorldPos.Y + moveY + offsetY);
            WorldPosition formationNewWorldPos = new WorldPosition(missionBehaviourInstance.Mission.Scene, formationNewPosition);
            return formationNewWorldPos;
        }

        // Calculate WorldPosition relative to the Formation's current position/faceing (Not Cumulative)
        private static WorldPosition CalcWorldPosRelToFormation(Formation formation, MissionBehaviour missionBehaviourInstance, float latDistance, float fwdDistance)
        {
            float fwdX = formation.Direction.x * fwdDistance;
            float fwdY = formation.Direction.y * fwdDistance;
            float latX = formation.Direction.y * latDistance;
            float latY = -formation.Direction.x * latDistance;
            Vec2 formationCurPos = formation.CurrentPosition;
            Vec3 relPosition = new Vec3(formationCurPos.X + fwdX + latX, formationCurPos.Y + fwdY + latY);
            WorldPosition relWorldPos = new WorldPosition(missionBehaviourInstance.Mission.Scene, relPosition);
            return relWorldPos;
        }


        // Reduce Selected units Max Speed Limit
        private static void ModifySelectedMaxSpeedLimit(MissionBehaviour missionBehaviourInstance, float modifierSpeedLimitMax)
        {
            MBReadOnlyList<Formation> SelectedFormationsList = missionBehaviourInstance.Mission.PlayerTeam.PlayerOrderController.SelectedFormations;

            foreach (Formation formation in SelectedFormationsList)
            {
                if (formation.FormationIndex == FormationClass.Infantry)
                {
                    Settings.Instance.InfantryMaxSpeedModifier = modifierSpeedLimitMax;
                }
                else if (formation.FormationIndex == FormationClass.Ranged)
                {
                    Settings.Instance.RangedMaxSpeedModifier = modifierSpeedLimitMax;
                }
                else if (formation.FormationIndex == FormationClass.Cavalry)
                {
                    Settings.Instance.CavalryMaxSpeedModifier = modifierSpeedLimitMax;
                }
                else if (formation.FormationIndex == FormationClass.HorseArcher)
                {
                    Settings.Instance.HorseArcherMaxSpeedModifier = modifierSpeedLimitMax;
                }
                else if (formation.FormationIndex == FormationClass.Skirmisher)
                {
                    Settings.Instance.SkirmisherMaxSpeedModifier = modifierSpeedLimitMax;
                }
                else if (formation.FormationIndex == FormationClass.HeavyInfantry)
                {
                    Settings.Instance.HeavyInfantryMaxSpeedModifier = modifierSpeedLimitMax;
                }
                else if (formation.FormationIndex == FormationClass.LightCavalry)
                {
                    Settings.Instance.LightCavalryMaxSpeedModifier = modifierSpeedLimitMax;
                }
                else if (formation.FormationIndex == FormationClass.HeavyCavalry)
                {
                    Settings.Instance.HeavyCavalryMaxSpeedModifier = modifierSpeedLimitMax;
                }
            }
        }
        
        // Move selected units to the given Custom Army Formation positions
        private static void MoveSelectedToCustArmyFormPos(CustomArmyFormation customArmyFormation, MissionBehaviour missionBehaviourInstance, float extraLatOffset, float extraFwdOffset)
        {
            MBReadOnlyList<Formation> SelectedFormationsList = missionBehaviourInstance.Mission.PlayerTeam.PlayerOrderController.SelectedFormations;

            List<Team> teams = (from t in missionBehaviourInstance.Mission.Teams
                                where t.Side == missionBehaviourInstance.Mission.MainAgent.Team.Side
                                select t).ToList();
            if (teams != null && teams.Count > 0)
            {
                foreach (var team in teams)
                {
                    // Set the main formation to act as leader of Army Formation
                    Formation MainFormation = null;
                    // Priorities all FormationIndexes in order, except Ranged & Cavalry.
                    foreach (Formation formation in team.Formations)
                    {
                        if (formation.FormationIndex != FormationClass.Ranged && formation.FormationIndex != FormationClass.Cavalry)
                        {
                            MainFormation = formation;
                            break;
                        }
                    }
                    // Priorities Ranged, then Cavalry last.
                    if (MainFormation == null)
                    {
                        foreach (Formation formation in team.Formations)
                        {
                            MainFormation = formation;
                            break;
                        }
                    }
                    
                    // Move selected units to the given Custom Army Formation positions
                    if (MainFormation != null)
                    {
                        foreach (Formation formation in team.Formations)
                        {
                            // Halt the MainFormation & move each formation in Army to a position relative to MainFormation's new position (default Infantry)
                            if (formation == MainFormation)
                            {
                                // Set the formation's ArrangementOrder if enabled in settings (-1 = disabled) - 'ShieldWall', 'Loose', etc.
                                formation.ArrangementOrder = customArmyFormation.InfantryArrangementOrderNum == -1 ? formation.ArrangementOrder : customArmyFormation.InfantryArrangementOrder;

                                // Set the formation's FormOrder (preset/custom front width) if enabled in settings (0 = disabled)
                                formation.FormOrder = customArmyFormation.InfantryFormOrderNum == 0 ? formation.FormOrder : customArmyFormation.InfantryFormOrder;

                                // Give Move order to MainFormation to halt in its current position.
                                WorldPosition newWorldPos = CalcWorldPosRelToFormation(MainFormation, missionBehaviourInstance, extraLatOffset, extraFwdOffset);
                                formation.MovementOrder = MovementOrder.MovementOrderMove(newWorldPos);
                            }
                            else if (SelectedFormationsList.Contains(formation))
                            {
                                if (formation.FormationIndex == FormationClass.Ranged)
                                {
                                    // Set the formation's ArrangementOrder if enabled in settings (-1 = disabled) - 'ShieldWall', 'Loose', etc.
                                    formation.ArrangementOrder = customArmyFormation.RangedArrangementOrderNum == -1 ? formation.ArrangementOrder : customArmyFormation.RangedArrangementOrder;

                                    // Set the formation's FormOrder (preset/custom front width) if enabled in settings (0 = disabled)
                                    formation.FormOrder = customArmyFormation.RangedFormOrderNum == 0 ? formation.FormOrder : customArmyFormation.RangedFormOrder;

                                    // Move Ranged Formation given paces right/forward of MainFormation's new position.
                                    WorldPosition newWorldPos = CalcWorldPosRelToFormation(MainFormation, missionBehaviourInstance, customArmyFormation.RangedStartPosLateralOffset + extraLatOffset, customArmyFormation.RangedStartPosFwdOffset + extraFwdOffset);
                                    formation.MovementOrder = MovementOrder.MovementOrderMove(newWorldPos);
                                }
                                else if (formation.FormationIndex == FormationClass.Cavalry)
                                {
                                    // Set the formation's ArrangementOrder if enabled in settings (-1 = disabled) - 'ShieldWall', 'Loose', etc.
                                    formation.ArrangementOrder = customArmyFormation.CavalryArrangementOrderNum == -1 ? formation.ArrangementOrder : customArmyFormation.CavalryArrangementOrder;

                                    // Set the formation's FormOrder (preset/custom front width) if enabled in settings (0 = disabled)
                                    formation.FormOrder = customArmyFormation.CavalryFormOrderNum == 0 ? formation.FormOrder : customArmyFormation.CavalryFormOrder;

                                    // Move Cavalry Formation given paces right/forward of MainFormation's new position.
                                    WorldPosition newWorldPos = CalcWorldPosRelToFormation(MainFormation, missionBehaviourInstance, customArmyFormation.CavalryStartPosLateralOffset + extraLatOffset, customArmyFormation.CavalryStartPosFwdOffset + extraFwdOffset);
                                    formation.MovementOrder = MovementOrder.MovementOrderMove(newWorldPos);
                                }
                                else if (formation.FormationIndex == FormationClass.HorseArcher)
                                {
                                    // Set the formation's ArrangementOrder if enabled in settings (-1 = disabled) - 'ShieldWall', 'Loose', etc.
                                    formation.ArrangementOrder = customArmyFormation.HorseArcherArrangementOrderNum == -1 ? formation.ArrangementOrder : customArmyFormation.HorseArcherArrangementOrder;

                                    // Set the formation's FormOrder (preset/custom front width) if enabled in settings (0 = disabled)
                                    formation.FormOrder = customArmyFormation.HorseArcherFormOrderNum == 0 ? formation.FormOrder : customArmyFormation.HorseArcherFormOrder;

                                    // Move HorseArcher Formation given paces right/forward of MainFormation's new position.
                                    WorldPosition newWorldPos = CalcWorldPosRelToFormation(MainFormation, missionBehaviourInstance, customArmyFormation.HorseArcherStartPosLateralOffset + extraLatOffset, customArmyFormation.HorseArcherStartPosFwdOffset + extraFwdOffset);
                                    formation.MovementOrder = MovementOrder.MovementOrderMove(newWorldPos);
                                }
                                else if (formation.FormationIndex == FormationClass.Skirmisher)
                                {
                                    // Set the formation's ArrangementOrder if enabled in settings (-1 = disabled) - 'ShieldWall', 'Loose', etc.
                                    formation.ArrangementOrder = customArmyFormation.SkirmisherArrangementOrderNum == -1 ? formation.ArrangementOrder : customArmyFormation.SkirmisherArrangementOrder;

                                    // Set the formation's FormOrder (preset/custom front width) if enabled in settings (0 = disabled)
                                    formation.FormOrder = customArmyFormation.SkirmisherFormOrderNum == 0 ? formation.FormOrder : customArmyFormation.SkirmisherFormOrder;

                                    // Move Skirmisher Formation given paces right/forward of MainFormation's new position.
                                    WorldPosition newWorldPos = CalcWorldPosRelToFormation(MainFormation, missionBehaviourInstance, customArmyFormation.SkirmisherStartPosLateralOffset + extraLatOffset, customArmyFormation.SkirmisherStartPosFwdOffset + extraFwdOffset);
                                    formation.MovementOrder = MovementOrder.MovementOrderMove(newWorldPos);
                                }
                                else if (formation.FormationIndex == FormationClass.HeavyInfantry)
                                {
                                    // Set the formation's ArrangementOrder if enabled in settings (-1 = disabled) - 'ShieldWall', 'Loose', etc.
                                    formation.ArrangementOrder = customArmyFormation.HeavyInfantryArrangementOrderNum == -1 ? formation.ArrangementOrder : customArmyFormation.HeavyInfantryArrangementOrder;

                                    // Set the formation's FormOrder (preset/custom front width) if enabled in settings (0 = disabled)
                                    formation.FormOrder = customArmyFormation.HeavyInfantryFormOrderNum == 0 ? formation.FormOrder : customArmyFormation.HeavyInfantryFormOrder;

                                    // Move HeavyInfantry Formation given paces right/forward of MainFormation's new position.
                                    WorldPosition newWorldPos = CalcWorldPosRelToFormation(MainFormation, missionBehaviourInstance, customArmyFormation.HeavyInfantryStartPosLateralOffset + extraLatOffset, customArmyFormation.HeavyInfantryStartPosFwdOffset + extraFwdOffset);
                                    formation.MovementOrder = MovementOrder.MovementOrderMove(newWorldPos);
                                }
                                else if (formation.FormationIndex == FormationClass.LightCavalry)
                                {
                                    // Set the formation's ArrangementOrder if enabled in settings (-1 = disabled) - 'ShieldWall', 'Loose', etc.
                                    formation.ArrangementOrder = customArmyFormation.LightCavalryArrangementOrderNum == -1 ? formation.ArrangementOrder : customArmyFormation.LightCavalryArrangementOrder;

                                    // Set the formation's FormOrder (preset/custom front width) if enabled in settings (0 = disabled)
                                    formation.FormOrder = customArmyFormation.LightCavalryFormOrderNum == 0 ? formation.FormOrder : customArmyFormation.LightCavalryFormOrder;

                                    // Move LightCavalry Formation given paces right/forward of MainFormation's new position.
                                    WorldPosition newWorldPos = CalcWorldPosRelToFormation(MainFormation, missionBehaviourInstance, customArmyFormation.LightCavalryStartPosLateralOffset + extraLatOffset, customArmyFormation.LightCavalryStartPosFwdOffset + extraFwdOffset);
                                    formation.MovementOrder = MovementOrder.MovementOrderMove(newWorldPos);
                                }
                                else if (formation.FormationIndex == FormationClass.HeavyCavalry)
                                {
                                    // Set the formation's ArrangementOrder if enabled in settings (-1 = disabled) - 'ShieldWall', 'Loose', etc.
                                    formation.ArrangementOrder = customArmyFormation.HeavyCavalryArrangementOrderNum == -1 ? formation.ArrangementOrder : customArmyFormation.HeavyCavalryArrangementOrder;

                                    // Set the formation's FormOrder (preset/custom front width) if enabled in settings (0 = disabled)
                                    formation.FormOrder = customArmyFormation.HeavyCavalryFormOrderNum == 0 ? formation.FormOrder : customArmyFormation.HeavyCavalryFormOrder;

                                    // Move HeavyCavalry Formation given paces right/forward of MainFormation's new position.
                                    WorldPosition newWorldPos = CalcWorldPosRelToFormation(MainFormation, missionBehaviourInstance, customArmyFormation.HeavyCavalryStartPosLateralOffset + extraLatOffset, customArmyFormation.HeavyCavalryStartPosFwdOffset + extraFwdOffset);
                                    formation.MovementOrder = MovementOrder.MovementOrderMove(newWorldPos);
                                }
                            }
                            
                        }
                    }
                }
            }

            // TO-DO: The above hasn't updated All Default Formation.FrontAttachmentPoint
            AllFormationFrontAttPtUpdated = true;
        }
        
        // Save all formation's curent position(relative to Infantry (I)) and other settings - to the given Custom Army Formation's 'Mod Options'
        private static void SaveAllToCustArmyForm(CustomArmyFormation customArmyFormation, MissionBehaviour missionBehaviourInstance)
        {
            if (missionBehaviourInstance.Mission.MainAgent != null)
            {
                // Get the main formation - acts as leader of Army Formation
                Formation MainFormation = missionBehaviourInstance.Mission.MainAgent.Team.Formations.Count() > 0 ? missionBehaviourInstance.Mission.MainAgent.Team.Formations.First() : null;
                if (MainFormation != null)
                {
                    foreach (Formation formation in missionBehaviourInstance.Mission.MainAgent.Team.Formations)
                    {
                        // Save Infantry (I) Settings
                        if (formation == MainFormation)
                        {
                            // Update ArrangementOrder - 'ShieldWall', 'Loose', etc.
                            customArmyFormation.InfantryArrangementOrderNum = (int)formation.ArrangementOrder.OrderEnum;
                            customArmyFormation.InfantryArrangementOrder = _helpersInstance.GetArrangementOrder(customArmyFormation.InfantryArrangementOrderNum);

                            // Update FormOrder (preset/custom front width)
                            customArmyFormation.InfantryFormOrderNum = _helpersInstance.GetCurFormOrderNum(formation);
                            customArmyFormation.InfantryFormOrder = _helpersInstance.GetFormOrder(customArmyFormation.InfantryFormOrderNum);
                        }
                        else if (formation.FormationIndex == FormationClass.Ranged)
                        {
                            // Update ArrangementOrder - 'ShieldWall', 'Loose', etc.
                            customArmyFormation.RangedArrangementOrderNum = (int)formation.ArrangementOrder.OrderEnum;
                            customArmyFormation.RangedArrangementOrder = _helpersInstance.GetArrangementOrder(customArmyFormation.RangedArrangementOrderNum);

                            // Update FormOrder (preset/custom front width)
                            customArmyFormation.RangedFormOrderNum = _helpersInstance.GetCurFormOrderNum(formation);
                            customArmyFormation.RangedFormOrder = _helpersInstance.GetFormOrder(customArmyFormation.RangedFormOrderNum);

                            // Update Lateral/Forward position relative to MainFormation's position.
                            customArmyFormation.RangedStartPosLateralOffset = GetCurLatFwdPosRelToMainFormation(MainFormation, formation).X;
                            customArmyFormation.RangedStartPosFwdOffset = GetCurLatFwdPosRelToMainFormation(MainFormation, formation).Y;
                        }
                        else if (formation.FormationIndex == FormationClass.Cavalry)
                        {
                            // Update ArrangementOrder - 'ShieldWall', 'Loose', etc.
                            customArmyFormation.CavalryArrangementOrderNum = (int)formation.ArrangementOrder.OrderEnum;
                            customArmyFormation.CavalryArrangementOrder = _helpersInstance.GetArrangementOrder(customArmyFormation.CavalryArrangementOrderNum);

                            // Update FormOrder (preset/custom front width)
                            customArmyFormation.CavalryFormOrderNum = _helpersInstance.GetCurFormOrderNum(formation);
                            customArmyFormation.CavalryFormOrder = _helpersInstance.GetFormOrder(customArmyFormation.CavalryFormOrderNum);

                            // Update Lateral/Forward position relative to MainFormation's position.
                            customArmyFormation.CavalryStartPosLateralOffset = GetCurLatFwdPosRelToMainFormation(MainFormation, formation).X;
                            customArmyFormation.CavalryStartPosFwdOffset = GetCurLatFwdPosRelToMainFormation(MainFormation, formation).Y;
                        }
                        else if (formation.FormationIndex == FormationClass.HorseArcher)
                        {
                            // Update ArrangementOrder - 'ShieldWall', 'Loose', etc.
                            customArmyFormation.HorseArcherArrangementOrderNum = (int)formation.ArrangementOrder.OrderEnum;
                            customArmyFormation.HorseArcherArrangementOrder = _helpersInstance.GetArrangementOrder(customArmyFormation.HorseArcherArrangementOrderNum);

                            // Update FormOrder (preset/custom front width)
                            customArmyFormation.HorseArcherFormOrderNum = _helpersInstance.GetCurFormOrderNum(formation);
                            customArmyFormation.HorseArcherFormOrder = _helpersInstance.GetFormOrder(customArmyFormation.HorseArcherFormOrderNum);

                            // Update Lateral/Forward position relative to MainFormation's position.
                            customArmyFormation.HorseArcherStartPosLateralOffset = GetCurLatFwdPosRelToMainFormation(MainFormation, formation).X;
                            customArmyFormation.HorseArcherStartPosFwdOffset = GetCurLatFwdPosRelToMainFormation(MainFormation, formation).Y;
                        }
                        else if (formation.FormationIndex == FormationClass.Skirmisher)
                        {
                            // Update ArrangementOrder - 'ShieldWall', 'Loose', etc.
                            customArmyFormation.SkirmisherArrangementOrderNum = (int)formation.ArrangementOrder.OrderEnum;
                            customArmyFormation.SkirmisherArrangementOrder = _helpersInstance.GetArrangementOrder(customArmyFormation.SkirmisherArrangementOrderNum);

                            // Update FormOrder (preset/custom front width)
                            customArmyFormation.SkirmisherFormOrderNum = _helpersInstance.GetCurFormOrderNum(formation);
                            customArmyFormation.SkirmisherFormOrder = _helpersInstance.GetFormOrder(customArmyFormation.SkirmisherFormOrderNum);

                            // Update Lateral/Forward position relative to MainFormation's position.
                            customArmyFormation.SkirmisherStartPosLateralOffset = GetCurLatFwdPosRelToMainFormation(MainFormation, formation).X;
                            customArmyFormation.SkirmisherStartPosFwdOffset = GetCurLatFwdPosRelToMainFormation(MainFormation, formation).Y;
                        }
                        else if (formation.FormationIndex == FormationClass.HeavyInfantry)
                        {
                            // Update ArrangementOrder - 'ShieldWall', 'Loose', etc.
                            customArmyFormation.HeavyInfantryArrangementOrderNum = (int)formation.ArrangementOrder.OrderEnum;
                            customArmyFormation.HeavyInfantryArrangementOrder = _helpersInstance.GetArrangementOrder(customArmyFormation.HeavyInfantryArrangementOrderNum);

                            // Update FormOrder (preset/custom front width)
                            customArmyFormation.HeavyInfantryFormOrderNum = _helpersInstance.GetCurFormOrderNum(formation);
                            customArmyFormation.HeavyInfantryFormOrder = _helpersInstance.GetFormOrder(customArmyFormation.HeavyInfantryFormOrderNum);

                            // Update Lateral/Forward position relative to MainFormation's position.
                            customArmyFormation.HeavyInfantryStartPosLateralOffset = GetCurLatFwdPosRelToMainFormation(MainFormation, formation).X;
                            customArmyFormation.HeavyInfantryStartPosFwdOffset = GetCurLatFwdPosRelToMainFormation(MainFormation, formation).Y;
                        }
                        else if (formation.FormationIndex == FormationClass.LightCavalry)
                        {
                            // Update ArrangementOrder - 'ShieldWall', 'Loose', etc.
                            customArmyFormation.LightCavalryArrangementOrderNum = (int)formation.ArrangementOrder.OrderEnum;
                            customArmyFormation.LightCavalryArrangementOrder = _helpersInstance.GetArrangementOrder(customArmyFormation.LightCavalryArrangementOrderNum);

                            // Update FormOrder (preset/custom front width)
                            customArmyFormation.LightCavalryFormOrderNum = _helpersInstance.GetCurFormOrderNum(formation);
                            customArmyFormation.LightCavalryFormOrder = _helpersInstance.GetFormOrder(customArmyFormation.LightCavalryFormOrderNum);

                            // Update Lateral/Forward position relative to MainFormation's position.
                            customArmyFormation.LightCavalryStartPosLateralOffset = GetCurLatFwdPosRelToMainFormation(MainFormation, formation).X;
                            customArmyFormation.LightCavalryStartPosFwdOffset = GetCurLatFwdPosRelToMainFormation(MainFormation, formation).Y;
                        }
                        else if (formation.FormationIndex == FormationClass.HeavyCavalry)
                        {
                            // Update ArrangementOrder - 'ShieldWall', 'Loose', etc.
                            customArmyFormation.HeavyCavalryArrangementOrderNum = (int)formation.ArrangementOrder.OrderEnum;
                            customArmyFormation.HeavyCavalryArrangementOrder = _helpersInstance.GetArrangementOrder(customArmyFormation.HeavyCavalryArrangementOrderNum);

                            // Update FormOrder (preset/custom front width)
                            customArmyFormation.HeavyCavalryFormOrderNum = _helpersInstance.GetCurFormOrderNum(formation);
                            customArmyFormation.HeavyCavalryFormOrder = _helpersInstance.GetFormOrder(customArmyFormation.HeavyCavalryFormOrderNum);

                            // Update Lateral/Forward position relative to MainFormation's position.
                            customArmyFormation.HeavyCavalryStartPosLateralOffset = GetCurLatFwdPosRelToMainFormation(MainFormation, formation).X;
                            customArmyFormation.HeavyCavalryStartPosFwdOffset = GetCurLatFwdPosRelToMainFormation(MainFormation, formation).Y;
                        }
                    }

                    // Update Settings.Instance - with the CustomArmyFormation's new settings
                    UpdateCustomArmyFormationSettings();
                }
            }
        }

        // WIP - EpicBattlesAI
        private static void EpicBattleAITick(MissionBehaviour __instance)
        {
            // PHASE 1 - Move Enemy formations to position 125m in front of Player's team
            if (!EpicBattleAIPhase1Active && !EpicBattleAIPhase1Completed)
            {
                EpicBattleAIPhase1Active = true;
                MoveAllEnemyToFrontOfPlayerTeam(__instance, CustomArmyFormations[8], 0, Phase1DestinationOffsetDistance);
            }
            else if (EpicBattleAIPhase1Active && !EpicBattleAIPhase1Completed)
            {
                // Check if the Enemy Main Formation has reached Phase 1 destination
                float xDelta = EnemyMainFormation.CurrentPosition.x - EpicBattleAIPhase1DestinationPos.X;
                float yDelta = EnemyMainFormation.CurrentPosition.y - EpicBattleAIPhase1DestinationPos.Y;
                if (xDelta > -10 && xDelta < 10 && yDelta > -10 && yDelta < 10)
                {
                    EpicBattleAIPhase1Completed = true;
                    // Delay Phase 2
                    EpicBattleAIPhase2DelayTime = MissionTime.SecondsFromNow((float)_helpersInstance.GetRandomDouble(EpicBattleAIDelayPhase2Seconds, EpicBattleAIDelayPhase2Seconds + 15));
                    //Debug Message
                    InformationManager.DisplayMessage(new InformationMessage("Epic Battle AI - Phase 1 - Destination Reached"));
                }
            }
            // PHASE 2 - Delay
            else if (EpicBattleAIPhase1Active && EpicBattleAIPhase1Completed && EpicBattleAIPhase2DelayTime.IsPast)
            {
                EpicBattleAIPhase1Active = false;
            }
            // PHASE 2 - Advance enemy Ranged (II). Return HorseArcher (IV) units back to native AI
            else if (!EpicBattleAIPhase1Active && EpicBattleAIPhase1Completed && !EpicBattleAIPhase2Active && !EpicBattleAIPhase2Completed)
            {
                EpicBattleAIPhase2Active = true;
                FormationsAdvanceYPaces(__instance, GetAllEnemyFormationsOfType(__instance, FormationClass.Ranged), Phase2DestinationOffsetDistance);
                ReturnEnemyFormationsToAI(__instance, GetAllEnemyFormationsOfType(__instance, FormationClass.HorseArcher));
                //Debug Message
                InformationManager.DisplayMessage(new InformationMessage("Epic Battle AI - Phase 2 - Ranged Advance!"));
            }
            else if (EpicBattleAIPhase2Active && !EpicBattleAIPhase2Completed)
            {
                EpicBattleAIPhase2Completed = true;
                // Delay Phase 3
                EpicBattleAIPhase3DelayTime = MissionTime.SecondsFromNow((float)_helpersInstance.GetRandomDouble(EpicBattleAIDelayPhase3Seconds, EpicBattleAIDelayPhase3Seconds + 60));
            }
            // PHASE 3 - Delay
            else if (EpicBattleAIPhase2Active && EpicBattleAIPhase2Completed && EpicBattleAIPhase3DelayTime.IsPast)
            {
                EpicBattleAIPhase2Active = false;
            }
            // PHASE 3 - Advance enemy Infantry (I). Return Cavalry enemy units back to native AI
            else if (!EpicBattleAIPhase2Active && EpicBattleAIPhase2Completed && !EpicBattleAIPhase3Active && !EpicBattleAIPhase3Completed)
            {
                EpicBattleAIPhase3Active = true;
                FormationsAdvanceYPaces(__instance, GetAllEnemyFormationsOfType(__instance, FormationClass.Infantry), Phase3DestinationOffsetDistance);

                Settings.Instance.AllEnemyAllyAgentsWalk = true;

                ReturnEnemyFormationsToAI(__instance, GetAllEnemyFormationsOfType(__instance, FormationClass.Cavalry));
                
                // Debug Message
                InformationManager.DisplayMessage(new InformationMessage("Epic Battle AI - Phase 3 - Infantry/Cavalry Advance!"));
            }
            else if (EpicBattleAIPhase3Active && !EpicBattleAIPhase3Completed)
            {
                EpicBattleAIPhase3Completed = true;
                // Delay Phase 4
                EpicBattleAIPhase4DelayTime = MissionTime.SecondsFromNow((float)_helpersInstance.GetRandomDouble(EpicBattleAIDelayPhase4Seconds, EpicBattleAIDelayPhase4Seconds + 30));
            }
            // PHASE 4 - Delay
            else if (EpicBattleAIPhase3Active && EpicBattleAIPhase3Completed && EpicBattleAIPhase4DelayTime.IsPast)
            {
                EpicBattleAIPhase3Active = false;
            }
            // PHASE 4 - Charge enemy Infantry (I). 
            else if (!EpicBattleAIPhase3Active && EpicBattleAIPhase3Completed && !EpicBattleAIPhase4Active && !EpicBattleAIPhase4Completed)
            {
                EpicBattleAIPhase4Active = true;
                
                foreach (Formation formation in GetAllEnemyFormationsOfType(__instance, FormationClass.Infantry))
                {
                    formation.AI.SetBehaviorWeight<BehaviorCharge>(100f);
                    formation.IsAIControlled = true;
                }
                Settings.Instance.AllEnemyAllyAgentsWalk = false;

                // Debug Message
                InformationManager.DisplayMessage(new InformationMessage("Epic Battle AI - Phase 4 - Infantry/Cavalry Charge!"));
            }
            else if (EpicBattleAIPhase4Active && !EpicBattleAIPhase4Completed)
            {
                EpicBattleAIPhase4Completed = true;

                // Delay Phase 5
                EpicBattleAIPhase5DelayTime = MissionTime.SecondsFromNow((float)_helpersInstance.GetRandomDouble(EpicBattleAIDelayPhase5Seconds, EpicBattleAIDelayPhase5Seconds + 10));
            }
            // PHASE 5 - Delay
            else if (EpicBattleAIPhase4Active && EpicBattleAIPhase4Completed && EpicBattleAIPhase5DelayTime.IsPast)
            {
                EpicBattleAIPhase4Active = false;
            }
            // PHASE 5 - Assess casualties and flee if casualties are too heavy (Random chance 50/50). 
            else if (!EpicBattleAIPhase4Active && EpicBattleAIPhase4Completed && !EpicBattleAIPhase5Active && !EpicBattleAIPhase5Completed)
            {
                EpicBattleAIPhase5Active = true;

                // Decide if they should flee after sustaining heavy casualties
                EpicBattleAIFlee = _helpersInstance.GetRandomInt(0, 4) == 0 ? false : true;

                // Debug Message
                InformationManager.DisplayMessage(new InformationMessage("Epic Battle AI - Phase 5 - Assessing Casualties"));
            }
            else if (EpicBattleAIPhase5Active && !EpicBattleAIPhase5Completed)
            {
                if (EnemyMainFormation.QuerySystem.CasualtyRatio < 0.40)
                {
                    if (EpicBattleAIFlee && EnemyMainFormation.Team.QuerySystem.OverallPowerRatio < __instance.Mission.MainAgent.Team.QuerySystem.OverallPowerRatio)
                    {
                        // Display Message
                        if (!EpicBattleAIIsRetreating)
                        {
                            InformationManager.DisplayMessage(new InformationMessage("Epic Battle AI - Phase 5 - Heavy Casualties - Retreat!"));

                            foreach (Agent agent in EnemyMainFormation.Team.TeamAgents)
                            {
                                agent.Retreat();
                            }

                            EpicBattleAIIsRetreating = true;
                        }
                    }
                    else
                    {
                        // Display Message
                        if (!EpicBattleAIIsFightingOn)
                        {
                            InformationManager.DisplayMessage(new InformationMessage("Epic Battle AI - Phase 5 - Heavy Casualties - Hold the line!"));
                            EpicBattleAIIsFightingOn = true;
                        }
                    }

                    //EpicBattleAIPhase5Completed = true;

                    // Deactivate EpicBattleAI
                    // EpicBattleAIActive = false;
                }
            }
        }

        // Reset Epic Battle Bools
        private static void EpicBattleAIReset()
        {
            EpicBattleAIPhase1Active = false;
            EpicBattleAIPhase1Completed = false;
            EpicBattleAIPhase2Active = false;
            EpicBattleAIPhase2Completed = false;
            EpicBattleAIPhase3Active = false;
            EpicBattleAIPhase3Completed = false;
            EpicBattleAIPhase4Active = false;
            EpicBattleAIPhase4Completed = false;
            EpicBattleAIPhase5Active = false;
            EpicBattleAIPhase5Completed = false;
        }

        // Return given Enemy Formations back to AI
        private static void ReturnEnemyFormationsToAI(MissionBehaviour missionBehaviourInstance, IEnumerable<Formation> formations)
        {
            foreach (Formation formation in formations)
            {
                // Turn ON Formation AI
                formation.IsAIControlled = true;
            }
        }

        // Return All Enemy Formations back to AI
        private static void ReturnAllEnemyFormationsToAI(MissionBehaviour missionBehaviourInstance)
        {
            List<Team> teams = (from t in missionBehaviourInstance.Mission.Teams
                                where t.Side != missionBehaviourInstance.Mission.MainAgent.Team.Side
                                select t).ToList();
            if (teams != null && teams.Count > 0)
            {
                foreach (var team in teams)
                {
                    ReturnEnemyFormationsToAI(missionBehaviourInstance, team.Formations);
                }
            }
        }
        
        // Move Enemy Formations to Custom Army Formation positions offset from the Players Team
        private static void MoveAllEnemyToFrontOfPlayerTeam(MissionBehaviour missionBehaviourInstance, CustomArmyFormation customArmyFormation, float extraLatOffset, float extraFwdOffset)
        {
            List<Team> teams = (from t in missionBehaviourInstance.Mission.Teams
                                where t.Side != missionBehaviourInstance.Mission.MainAgent.Team.Side
                                select t).ToList();
            if (teams != null && teams.Count > 0)
            {
                // Get the MainAgent's main formation to act as target of the Enemy Army Formation
                Formation PlayerMainFormation = null;
                // Priorities all FormationIndexes in order, except Ranged & Cavalry.
                foreach (Formation formation in missionBehaviourInstance.Mission.MainAgent.Team.Formations)
                {
                    if (formation.FormationIndex != FormationClass.Ranged && formation.FormationIndex != FormationClass.Cavalry)
                    {
                        PlayerMainFormation = formation;
                        break;
                    }
                }
                // Priorities Ranged, then Cavalry last.
                if (PlayerMainFormation == null)
                {
                    foreach (Formation formation in missionBehaviourInstance.Mission.MainAgent.Team.Formations)
                    {
                        PlayerMainFormation = formation;
                        break;
                    }
                }

                // Move enemy units to the given Custom Army Formation positions
                if (PlayerMainFormation != null)
                {
                    foreach (var team in teams)
                    {
                        // Set the main formation to act as leader of Army Formation
                        // Priorities all FormationIndexes in order, except Ranged & Cavalry.
                        foreach (Formation formation in team.Formations)
                        {
                            if (formation.FormationIndex != FormationClass.Ranged && formation.FormationIndex != FormationClass.Cavalry)
                            {
                                EnemyMainFormation = formation;
                                break;
                            }
                        }
                        // Priorities Ranged, then Cavalry last.
                        if (EnemyMainFormation == null)
                        {
                            foreach (Formation formation in team.Formations)
                            {
                                EnemyMainFormation = formation;
                                break;
                            }
                        }

                        // Move All Enemy units to the given Custom Army Formation positions
                        if (EnemyMainFormation != null)
                        {
                            foreach (Formation formation in team.Formations)
                            {
                                // Turn OFF Formation AI
                                formation.IsAIControlled = false;

                                // Face player's team
                                formation.FacingOrder = FacingOrder.FacingOrderLookAtEnemy;

                                // Move the EnemyMainFormation & move each formation in Enemy Army to a position relative to EnemyMainFormation's new position (default Infantry)
                                if (formation == EnemyMainFormation)
                                {
                                    // Set the formation's ArrangementOrder if enabled in settings (-1 = disabled) - 'ShieldWall', 'Loose', etc.
                                    formation.ArrangementOrder = customArmyFormation.InfantryArrangementOrderNum == -1 ? formation.ArrangementOrder : customArmyFormation.InfantryArrangementOrder;

                                    // Set the formation's FormOrder (preset/custom front width) if enabled in settings (0 = disabled)
                                    formation.FormOrder = customArmyFormation.InfantryFormOrderNum == 0 ? formation.FormOrder : customArmyFormation.InfantryFormOrder;

                                    // Give move order to EnemyMainFormation to move 100 meters in front of PlayerMainFormation current position.
                                    WorldPosition newWorldPos = CalcWorldPosRelToFormation(PlayerMainFormation, missionBehaviourInstance, extraLatOffset, extraFwdOffset);
                                    formation.MovementOrder = MovementOrder.MovementOrderMove(newWorldPos);
                                    EpicBattleAIPhase1DestinationPos = newWorldPos;
                                }
                                else if (formation.FormationIndex == FormationClass.Ranged)
                                {
                                    // Set the formation's ArrangementOrder if enabled in settings (-1 = disabled) - 'ShieldWall', 'Loose', etc.
                                    formation.ArrangementOrder = customArmyFormation.RangedArrangementOrderNum == -1 ? formation.ArrangementOrder : customArmyFormation.RangedArrangementOrder;

                                    // Set the formation's FormOrder (preset/custom front width) if enabled in settings (0 = disabled)
                                    formation.FormOrder = customArmyFormation.RangedFormOrderNum == 0 ? formation.FormOrder : customArmyFormation.RangedFormOrder;

                                    // Move Ranged Formation given paces right/forward of MainFormation's new position.
                                    WorldPosition newWorldPos = CalcWorldPosRelToFormation(PlayerMainFormation, missionBehaviourInstance, customArmyFormation.RangedStartPosLateralOffset + extraLatOffset, customArmyFormation.RangedStartPosFwdOffset + extraFwdOffset);
                                    formation.MovementOrder = MovementOrder.MovementOrderMove(newWorldPos);
                                }
                                else if (formation.FormationIndex == FormationClass.Cavalry)
                                {
                                    // Set the formation's ArrangementOrder if enabled in settings (-1 = disabled) - 'ShieldWall', 'Loose', etc.
                                    formation.ArrangementOrder = customArmyFormation.CavalryArrangementOrderNum == -1 ? formation.ArrangementOrder : customArmyFormation.CavalryArrangementOrder;

                                    // Set the formation's FormOrder (preset/custom front width) if enabled in settings (0 = disabled)
                                    formation.FormOrder = customArmyFormation.CavalryFormOrderNum == 0 ? formation.FormOrder : customArmyFormation.CavalryFormOrder;

                                    // Move Cavalry Formation given paces right/forward of MainFormation's new position.
                                    WorldPosition newWorldPos = CalcWorldPosRelToFormation(PlayerMainFormation, missionBehaviourInstance, customArmyFormation.CavalryStartPosLateralOffset + extraLatOffset, customArmyFormation.CavalryStartPosFwdOffset + extraFwdOffset);
                                    formation.MovementOrder = MovementOrder.MovementOrderMove(newWorldPos);
                                }
                                else if (formation.FormationIndex == FormationClass.HorseArcher)
                                {
                                    // Set the formation's ArrangementOrder if enabled in settings (-1 = disabled) - 'ShieldWall', 'Loose', etc.
                                    formation.ArrangementOrder = customArmyFormation.HorseArcherArrangementOrderNum == -1 ? formation.ArrangementOrder : customArmyFormation.HorseArcherArrangementOrder;

                                    // Set the formation's FormOrder (preset/custom front width) if enabled in settings (0 = disabled)
                                    formation.FormOrder = customArmyFormation.HorseArcherFormOrderNum == 0 ? formation.FormOrder : customArmyFormation.HorseArcherFormOrder;

                                    // Move HorseArcher Formation given paces right/forward of MainFormation's new position.
                                    WorldPosition newWorldPos = CalcWorldPosRelToFormation(PlayerMainFormation, missionBehaviourInstance, customArmyFormation.HorseArcherStartPosLateralOffset + extraLatOffset, customArmyFormation.HorseArcherStartPosFwdOffset + extraFwdOffset);
                                    formation.MovementOrder = MovementOrder.MovementOrderMove(newWorldPos);
                                }
                                else if (formation.FormationIndex == FormationClass.Skirmisher)
                                {
                                    // Set the formation's ArrangementOrder if enabled in settings (-1 = disabled) - 'ShieldWall', 'Loose', etc.
                                    formation.ArrangementOrder = customArmyFormation.SkirmisherArrangementOrderNum == -1 ? formation.ArrangementOrder : customArmyFormation.SkirmisherArrangementOrder;

                                    // Set the formation's FormOrder (preset/custom front width) if enabled in settings (0 = disabled)
                                    formation.FormOrder = customArmyFormation.SkirmisherFormOrderNum == 0 ? formation.FormOrder : customArmyFormation.SkirmisherFormOrder;

                                    // Move Skirmisher Formation given paces right/forward of MainFormation's new position.
                                    WorldPosition newWorldPos = CalcWorldPosRelToFormation(PlayerMainFormation, missionBehaviourInstance, customArmyFormation.SkirmisherStartPosLateralOffset + extraLatOffset, customArmyFormation.SkirmisherStartPosFwdOffset + extraFwdOffset);
                                    formation.MovementOrder = MovementOrder.MovementOrderMove(newWorldPos);
                                }
                                else if (formation.FormationIndex == FormationClass.HeavyInfantry)
                                {
                                    // Set the formation's ArrangementOrder if enabled in settings (-1 = disabled) - 'ShieldWall', 'Loose', etc.
                                    formation.ArrangementOrder = customArmyFormation.HeavyInfantryArrangementOrderNum == -1 ? formation.ArrangementOrder : customArmyFormation.HeavyInfantryArrangementOrder;

                                    // Set the formation's FormOrder (preset/custom front width) if enabled in settings (0 = disabled)
                                    formation.FormOrder = customArmyFormation.HeavyInfantryFormOrderNum == 0 ? formation.FormOrder : customArmyFormation.HeavyInfantryFormOrder;

                                    // Move HeavyInfantry Formation given paces right/forward of MainFormation's new position.
                                    WorldPosition newWorldPos = CalcWorldPosRelToFormation(PlayerMainFormation, missionBehaviourInstance, customArmyFormation.HeavyInfantryStartPosLateralOffset + extraLatOffset, customArmyFormation.HeavyInfantryStartPosFwdOffset + extraFwdOffset);
                                    formation.MovementOrder = MovementOrder.MovementOrderMove(newWorldPos);
                                }
                                else if (formation.FormationIndex == FormationClass.LightCavalry)
                                {
                                    // Set the formation's ArrangementOrder if enabled in settings (-1 = disabled) - 'ShieldWall', 'Loose', etc.
                                    formation.ArrangementOrder = customArmyFormation.LightCavalryArrangementOrderNum == -1 ? formation.ArrangementOrder : customArmyFormation.LightCavalryArrangementOrder;

                                    // Set the formation's FormOrder (preset/custom front width) if enabled in settings (0 = disabled)
                                    formation.FormOrder = customArmyFormation.LightCavalryFormOrderNum == 0 ? formation.FormOrder : customArmyFormation.LightCavalryFormOrder;

                                    // Move LightCavalry Formation given paces right/forward of MainFormation's new position.
                                    WorldPosition newWorldPos = CalcWorldPosRelToFormation(PlayerMainFormation, missionBehaviourInstance, customArmyFormation.LightCavalryStartPosLateralOffset + extraLatOffset, customArmyFormation.LightCavalryStartPosFwdOffset + extraFwdOffset);
                                    formation.MovementOrder = MovementOrder.MovementOrderMove(newWorldPos);
                                }
                                else if (formation.FormationIndex == FormationClass.HeavyCavalry)
                                {
                                    // Set the formation's ArrangementOrder if enabled in settings (-1 = disabled) - 'ShieldWall', 'Loose', etc.
                                    formation.ArrangementOrder = customArmyFormation.HeavyCavalryArrangementOrderNum == -1 ? formation.ArrangementOrder : customArmyFormation.HeavyCavalryArrangementOrder;

                                    // Set the formation's FormOrder (preset/custom front width) if enabled in settings (0 = disabled)
                                    formation.FormOrder = customArmyFormation.HeavyCavalryFormOrderNum == 0 ? formation.FormOrder : customArmyFormation.HeavyCavalryFormOrder;

                                    // Move HeavyCavalry Formation given paces right/forward of MainFormation's new position.
                                    WorldPosition newWorldPos = CalcWorldPosRelToFormation(PlayerMainFormation, missionBehaviourInstance, customArmyFormation.HeavyCavalryStartPosLateralOffset + extraLatOffset, customArmyFormation.HeavyCavalryStartPosFwdOffset + extraFwdOffset);
                                    formation.MovementOrder = MovementOrder.MovementOrderMove(newWorldPos);
                                }

                            }
                        }
                    }
                }
            }
        }

        // Get all enemy formations of given type
        private static List<Formation> GetAllEnemyFormationsOfType(MissionBehaviour missionBehaviourInstance, FormationClass formationClass)
        {
            List<Formation> formations = new List<Formation>();

            List<Team> teams = (from t in missionBehaviourInstance.Mission.Teams
                                where t.Side != missionBehaviourInstance.Mission.MainAgent.Team.Side
                                select t).ToList();
            if (teams != null && teams.Count > 0)
            {
                foreach (var team in teams)
                {
                    foreach (Formation formation in team.Formations)
                    {
                        if (formation.FormationIndex == formationClass)
                        {
                            formations.Add(formation);
                        }
                    }
                }
            }

            return formations; 
        }

        // Get Lateral and Forward offsets of formation relative to MainFormation (I)
        private static Vec2 GetCurLatFwdPosRelToMainFormation(Formation mainFormation, Formation formation)
        {
            double formationPosX = formation.CurrentPosition.X;
            double formationPosY = formation.CurrentPosition.Y;
            double mainFormationPosX = mainFormation.CurrentPosition.X;
            double mainFormationPosY = mainFormation.CurrentPosition.Y;
            double alpha = Math.Atan2(mainFormation.Direction.Y, mainFormation.Direction.X);// - (90 * Math.PI / 180);

            //step 1: 
            formationPosX -= mainFormationPosX;
            formationPosY -= mainFormationPosY;

            // step 2
            double xRot = formationPosX * Math.Sin(alpha) - formationPosY * Math.Cos(alpha);
            double yRot = formationPosX * Math.Cos(alpha) + formationPosY * Math.Sin(alpha);

            return new Vec2((float)xRot, (float)yRot);
        }



        // (deprecated) Move all units to the given Custom Army Formation positions
        private static void MoveAllToCustArmyFormPos(CustomArmyFormation customArmyFormation, MissionBehaviour missionBehaviourInstance)
        {
            List<Team> teams = (from t in missionBehaviourInstance.Mission.Teams
                                where t.Side == missionBehaviourInstance.Mission.MainAgent.Team.Side
                                select t).ToList();
            if (teams != null && teams.Count > 0)
            {
                foreach (var team in teams)
                {
                    // Set the main formation to act as leader of Army Formation
                    Formation MainFormation = team.Formations.Count() > 0 ? team.Formations.First() : null;
                    if (MainFormation != null)
                    {
                        foreach (Formation formation in team.Formations)
                        {
                            // Halt the MainFormation & move each formation in Army to a position relative to MainFormation's new position (default Infantry)
                            if (formation == team.Formations.First())
                            {
                                // Set the formation's ArrangementOrder if enabled in settings (-1 = disabled) - 'ShieldWall', 'Loose', etc.
                                formation.ArrangementOrder = customArmyFormation.InfantryArrangementOrderNum == -1 ? formation.ArrangementOrder : customArmyFormation.InfantryArrangementOrder;

                                // Set the formation's FormOrder (preset/custom front width) if enabled in settings (0 = disabled)
                                formation.FormOrder = customArmyFormation.InfantryFormOrderNum == 0 ? formation.FormOrder : customArmyFormation.InfantryFormOrder;

                                // Give Move order to MainFormation to halt in its current position.
                                WorldPosition newWorldPos = CalcWorldPosRelToFormation(MainFormation, missionBehaviourInstance, 0, 0);
                                formation.MovementOrder = MovementOrder.MovementOrderMove(newWorldPos);
                            }
                            else if (formation.FormationIndex == FormationClass.Ranged)
                            {
                                // Set the formation's ArrangementOrder if enabled in settings (-1 = disabled) - 'ShieldWall', 'Loose', etc.
                                formation.ArrangementOrder = customArmyFormation.RangedArrangementOrderNum == -1 ? formation.ArrangementOrder : customArmyFormation.RangedArrangementOrder;

                                // Set the formation's FormOrder (preset/custom front width) if enabled in settings (0 = disabled)
                                formation.FormOrder = customArmyFormation.RangedFormOrderNum == 0 ? formation.FormOrder : customArmyFormation.RangedFormOrder;

                                // Advance Ranged Formation given paces right/forward of MainFormation's new position.
                                WorldPosition newWorldPos = CalcWorldPosRelToFormation(MainFormation, missionBehaviourInstance, customArmyFormation.RangedStartPosLateralOffset, customArmyFormation.RangedStartPosFwdOffset);
                                formation.MovementOrder = MovementOrder.MovementOrderMove(newWorldPos);
                            }
                            else if (formation.FormationIndex == FormationClass.Cavalry)
                            {
                                // Set the formation's ArrangementOrder if enabled in settings (-1 = disabled) - 'ShieldWall', 'Loose', etc.
                                formation.ArrangementOrder = customArmyFormation.CavalryArrangementOrderNum == -1 ? formation.ArrangementOrder : customArmyFormation.CavalryArrangementOrder;

                                // Set the formation's FormOrder (preset/custom front width) if enabled in settings (0 = disabled)
                                formation.FormOrder = customArmyFormation.CavalryFormOrderNum == 0 ? formation.FormOrder : customArmyFormation.CavalryFormOrder;

                                // Advance Cavalry Formation given paces right/forward of MainFormation's new position.
                                WorldPosition newWorldPos = CalcWorldPosRelToFormation(MainFormation, missionBehaviourInstance, customArmyFormation.CavalryStartPosLateralOffset, customArmyFormation.CavalryStartPosFwdOffset);
                                formation.MovementOrder = MovementOrder.MovementOrderMove(newWorldPos);
                            }
                            else if (formation.FormationIndex == FormationClass.HorseArcher)
                            {
                                // Set the formation's ArrangementOrder if enabled in settings (-1 = disabled) - 'ShieldWall', 'Loose', etc.
                                formation.ArrangementOrder = customArmyFormation.HorseArcherArrangementOrderNum == -1 ? formation.ArrangementOrder : customArmyFormation.HorseArcherArrangementOrder;

                                // Set the formation's FormOrder (preset/custom front width) if enabled in settings (0 = disabled)
                                formation.FormOrder = customArmyFormation.HorseArcherFormOrderNum == 0 ? formation.FormOrder : customArmyFormation.HorseArcherFormOrder;

                                // Advance HorseArcher Formation given paces right/forward of MainFormation's new position.
                                WorldPosition newWorldPos = CalcWorldPosRelToFormation(MainFormation, missionBehaviourInstance, customArmyFormation.HorseArcherStartPosLateralOffset, customArmyFormation.HorseArcherStartPosFwdOffset);
                                formation.MovementOrder = MovementOrder.MovementOrderMove(newWorldPos);
                            }
                            else if (formation.FormationIndex == FormationClass.Skirmisher)
                            {
                                // Set the formation's ArrangementOrder if enabled in settings (-1 = disabled) - 'ShieldWall', 'Loose', etc.
                                formation.ArrangementOrder = customArmyFormation.SkirmisherArrangementOrderNum == -1 ? formation.ArrangementOrder : customArmyFormation.SkirmisherArrangementOrder;

                                // Set the formation's FormOrder (preset/custom front width) if enabled in settings (0 = disabled)
                                formation.FormOrder = customArmyFormation.SkirmisherFormOrderNum == 0 ? formation.FormOrder : customArmyFormation.SkirmisherFormOrder;

                                // Advance Skirmisher Formation given paces right/forward of MainFormation's new position.
                                WorldPosition newWorldPos = CalcWorldPosRelToFormation(MainFormation, missionBehaviourInstance, customArmyFormation.SkirmisherStartPosLateralOffset, customArmyFormation.SkirmisherStartPosFwdOffset);
                                formation.MovementOrder = MovementOrder.MovementOrderMove(newWorldPos);
                            }
                            else if (formation.FormationIndex == FormationClass.HeavyInfantry)
                            {
                                // Set the formation's ArrangementOrder if enabled in settings (-1 = disabled) - 'ShieldWall', 'Loose', etc.
                                formation.ArrangementOrder = customArmyFormation.HeavyInfantryArrangementOrderNum == -1 ? formation.ArrangementOrder : customArmyFormation.HeavyInfantryArrangementOrder;

                                // Set the formation's FormOrder (preset/custom front width) if enabled in settings (0 = disabled)
                                formation.FormOrder = customArmyFormation.HeavyInfantryFormOrderNum == 0 ? formation.FormOrder : customArmyFormation.HeavyInfantryFormOrder;

                                // Advance HeavyInfantry Formation given paces right/forward of MainFormation's new position.
                                WorldPosition newWorldPos = CalcWorldPosRelToFormation(MainFormation, missionBehaviourInstance, customArmyFormation.HeavyInfantryStartPosLateralOffset, customArmyFormation.HeavyInfantryStartPosFwdOffset);
                                formation.MovementOrder = MovementOrder.MovementOrderMove(newWorldPos);
                            }
                            else if (formation.FormationIndex == FormationClass.LightCavalry)
                            {
                                // Set the formation's ArrangementOrder if enabled in settings (-1 = disabled) - 'ShieldWall', 'Loose', etc.
                                formation.ArrangementOrder = customArmyFormation.LightCavalryArrangementOrderNum == -1 ? formation.ArrangementOrder : customArmyFormation.LightCavalryArrangementOrder;

                                // Set the formation's FormOrder (preset/custom front width) if enabled in settings (0 = disabled)
                                formation.FormOrder = customArmyFormation.LightCavalryFormOrderNum == 0 ? formation.FormOrder : customArmyFormation.LightCavalryFormOrder;

                                // Advance LightCavalry Formation given paces right/forward of MainFormation's new position.
                                WorldPosition newWorldPos = CalcWorldPosRelToFormation(MainFormation, missionBehaviourInstance, customArmyFormation.LightCavalryStartPosLateralOffset, customArmyFormation.LightCavalryStartPosFwdOffset);
                                formation.MovementOrder = MovementOrder.MovementOrderMove(newWorldPos);
                            }
                            else if (formation.FormationIndex == FormationClass.HeavyCavalry)
                            {
                                // Set the formation's ArrangementOrder if enabled in settings (-1 = disabled) - 'ShieldWall', 'Loose', etc.
                                formation.ArrangementOrder = customArmyFormation.HeavyCavalryArrangementOrderNum == -1 ? formation.ArrangementOrder : customArmyFormation.HeavyCavalryArrangementOrder;

                                // Set the formation's FormOrder (preset/custom front width) if enabled in settings (0 = disabled)
                                formation.FormOrder = customArmyFormation.HeavyCavalryFormOrderNum == 0 ? formation.FormOrder : customArmyFormation.HeavyCavalryFormOrder;

                                // Advance HeavyCavalry Formation given paces right/forward of MainFormation's new position.
                                WorldPosition newWorldPos = CalcWorldPosRelToFormation(MainFormation, missionBehaviourInstance, customArmyFormation.HeavyCavalryStartPosLateralOffset, customArmyFormation.HeavyCavalryStartPosFwdOffset);
                                formation.MovementOrder = MovementOrder.MovementOrderMove(newWorldPos);
                            }
                        }
                    }
                }
            }

            // The above has updated All Default Formation.FrontAttachmentPoint
            AllFormationFrontAttPtUpdated = true;
        }

        // (deprecated) Move all units to  the First Enabled Custom Army Formation positions
        private static void MoveAllToFirstEnabledCustArmyFormPos(MissionBehaviour missionBehaviourInstance)
        {
            List<CustomArmyFormation> enabledCustomArmyFormations = (from t in CustomArmyFormations
                                                                     where t.Enabled == true
                                                                     select t).ToList();
            // Get First Enabled Custom Army Formation
            if (enabledCustomArmyFormations.Count > 0)
            {
                MoveAllToCustArmyFormPos(enabledCustomArmyFormations[0], missionBehaviourInstance);
                // Success message
                int customArmyFormationNumber = enabledCustomArmyFormations[0].CustomArmyFormationIndex + 1;
                InformationManager.DisplayMessage(new InformationMessage("All units move to: 'Custom Army Formation " + customArmyFormationNumber + "' (F11)"));
            }
        }

        // (deprecated) Move all units to  the Second Enabled Custom Army Formation positions
        private static void MoveAllToSecondEnabledCustArmyFormPos(MissionBehaviour missionBehaviourInstance)
        {
            List<CustomArmyFormation> enabledCustomArmyFormations = (from t in CustomArmyFormations
                                                                     where t.Enabled == true
                                                                     select t).ToList();
            // Get Second Enabled Custom Army Formation
            if (enabledCustomArmyFormations.Count > 1)
            {
                MoveAllToCustArmyFormPos(enabledCustomArmyFormations[1], missionBehaviourInstance);
                // Success message
                int customArmyFormationNumber = enabledCustomArmyFormations[1].CustomArmyFormationIndex + 1;
                InformationManager.DisplayMessage(new InformationMessage("All units move to: 'Custom Army Formation " + customArmyFormationNumber + "' (F12)"));
            }

        }
        
    }

    public class Helpers
    {
        private static Random _random = new Random();
        public double GetRandomDouble(double minimum, double maximum)
        {
            return _random.NextDouble() * (maximum - minimum) + minimum;
        }

        public int GetRandomInt(int minimum, int maximum)
        {
            int randomInt = _random.Next(minimum, maximum);
            return randomInt;
        }

        public ArrangementOrder GetArrangementOrder(int index)
        {
            switch (index)
            {
                case 0:
                    return ArrangementOrder.ArrangementOrderCircle;
                case 1:
                    return ArrangementOrder.ArrangementOrderColumn;
                case 2:
                    return ArrangementOrder.ArrangementOrderLine;
                case 3:
                    return ArrangementOrder.ArrangementOrderLoose;
                case 4:
                    return ArrangementOrder.ArrangementOrderScatter;
                case 5:
                    return ArrangementOrder.ArrangementOrderShieldWall;
                case 6:
                    return ArrangementOrder.ArrangementOrderSkein;
                case 7:
                    return ArrangementOrder.ArrangementOrderSquare;
                default:
                    return ArrangementOrder.ArrangementOrderLine;
            }
        }

        public FormOrder GetFormOrder(int index)
        {
            if (index == -3)
            {
                return FormOrder.FormOrderWider;
            }
            else if (index == -2)
            {
                return FormOrder.FormOrderWide;
            }
            else if (index == -1)
            {
                return FormOrder.FormOrderDeep;
            }
            else if (index > 0)
            {
                return FormOrder.FormOrderCustom(index);
            }
            else
            {
                return FormOrder.FormOrderWide;
            }
        }

        // Get the converted FormOrder number from the given formation - (Formation Width)
        public int GetCurFormOrderNum(Formation formation)
        {
            return (int)formation.FormOrder.OrderType == 29 ? -3 : // 'FormWider' = 'PresetWider' in UI
                   (int)formation.FormOrder.OrderType == 28 ? -2 : // 'FormWide'  = 'PresetWide' in UI
                   (int)formation.FormOrder.OrderType == 27 ? -1 : // 'FormDeep'  = 'PresetDeep' in UI
                   (int)formation.Width;                           // 'FormCustom' = 'CustomWidth' in UI
        }
    }

    public class CustomArmyFormation
    {
        public int CustomArmyFormationIndex { get; set; } = 0;
        public bool Enabled { get; set; } = false;
        public int InfantryArrangementOrderNum { get; set; } = -1;
        public ArrangementOrder InfantryArrangementOrder { get; set; } = ArrangementOrder.ArrangementOrderLine;
        public int InfantryFormOrderNum { get; set; } = 0;
        public FormOrder InfantryFormOrder { get; set; } = FormOrder.FormOrderWide;
        public int RangedArrangementOrderNum { get; set; } = -1;
        public ArrangementOrder RangedArrangementOrder { get; set; } = ArrangementOrder.ArrangementOrderLine;
        public int RangedFormOrderNum { get; set; } = 0;
        public FormOrder RangedFormOrder { get; set; } = FormOrder.FormOrderWide;
        public float RangedStartPosLateralOffset { get; set; } = 0f;
        public float RangedStartPosFwdOffset { get; set; } = 0f;
        public int CavalryArrangementOrderNum { get; set; } = -1;
        public ArrangementOrder CavalryArrangementOrder { get; set; } = ArrangementOrder.ArrangementOrderLine;
        public int CavalryFormOrderNum { get; set; } = 0;
        public FormOrder CavalryFormOrder { get; set; } = FormOrder.FormOrderWide;
        public float CavalryStartPosLateralOffset { get; set; } = 0f;
        public float CavalryStartPosFwdOffset { get; set; } = 0f;
        public int HorseArcherArrangementOrderNum { get; set; } = -1;
        public ArrangementOrder HorseArcherArrangementOrder { get; set; } = ArrangementOrder.ArrangementOrderLine;
        public int HorseArcherFormOrderNum { get; set; } = 0;
        public FormOrder HorseArcherFormOrder { get; set; } = FormOrder.FormOrderWide;
        public float HorseArcherStartPosLateralOffset { get; set; } = 0f;
        public float HorseArcherStartPosFwdOffset { get; set; } = 0f;
        public int SkirmisherArrangementOrderNum { get; set; } = -1;
        public ArrangementOrder SkirmisherArrangementOrder { get; set; } = ArrangementOrder.ArrangementOrderLine;
        public int SkirmisherFormOrderNum { get; set; } = 0;
        public FormOrder SkirmisherFormOrder { get; set; } = FormOrder.FormOrderWide;
        public float SkirmisherStartPosLateralOffset { get; set; } = 0f;
        public float SkirmisherStartPosFwdOffset { get; set; } = 0f;
        public int HeavyInfantryArrangementOrderNum { get; set; } = -1;
        public ArrangementOrder HeavyInfantryArrangementOrder { get; set; } = ArrangementOrder.ArrangementOrderLine;
        public int HeavyInfantryFormOrderNum { get; set; } = 0;
        public FormOrder HeavyInfantryFormOrder { get; set; } = FormOrder.FormOrderWide;
        public float HeavyInfantryStartPosLateralOffset { get; set; } = 0f;
        public float HeavyInfantryStartPosFwdOffset { get; set; } = 0f;
        public int LightCavalryArrangementOrderNum { get; set; } = -1;
        public ArrangementOrder LightCavalryArrangementOrder { get; set; } = ArrangementOrder.ArrangementOrderLine;
        public int LightCavalryFormOrderNum { get; set; } = 0;
        public FormOrder LightCavalryFormOrder { get; set; } = FormOrder.FormOrderWide;
        public float LightCavalryStartPosLateralOffset { get; set; } = 0f;
        public float LightCavalryStartPosFwdOffset { get; set; } = 0f;
        public int HeavyCavalryArrangementOrderNum { get; set; } = -1;
        public ArrangementOrder HeavyCavalryArrangementOrder { get; set; } = ArrangementOrder.ArrangementOrderLine;
        public int HeavyCavalryFormOrderNum { get; set; } = 0;
        public FormOrder HeavyCavalryFormOrder { get; set; } = FormOrder.FormOrderWide;
        public float HeavyCavalryStartPosLateralOffset { get; set; } = 0f;
        public float HeavyCavalryStartPosFwdOffset { get; set; } = 0f;

        public CustomArmyFormation( int customArmyFormationIndex,
                                    bool enabled,
                                    int infantryArrangementOrderNum, int infantryFormOrderNum,
                                    int rangedArrangementOrderNum, int rangedFormOrderNum, float rangedStartPosLateralOffset, float rangedStartPosFwdOffset,
                                    int cavalryArrangementOrderNum, int cavalryFormOrderNum, float cavalryStartPosLateralOffset, float cavalryStartPosFwdOffset,
                                    int horseArcherArrangementOrderNum, int horseArcherFormOrderNum, float horseArcherStartPosLateralOffset, float horseArcherStartPosFwdOffset,
                                    int skirmisherArrangementOrderNum, int skirmisherFormOrderNum, float skirmisherStartPosLateralOffset, float skirmisherStartPosFwdOffset,
                                    int heavyInfantryArrangementOrderNum, int heavyInfantryFormOrderNum, float heavyInfantryStartPosLateralOffset, float heavyInfantryStartPosFwdOffset,
                                    int lightCavalryArrangementOrderNum, int lightCavalryFormOrderNum, float lightCavalryStartPosLateralOffset, float lightCavalryStartPosFwdOffset,
                                    int heavyCavalryArrangementOrderNum, int heavyCavalryFormOrderNum, float heavyCavalryStartPosLateralOffset, float heavyCavalryStartPosFwdOffset)
        {
            Helpers helpers = new Helpers();
            CustomArmyFormationIndex = customArmyFormationIndex;
            Enabled = enabled;
            InfantryArrangementOrderNum = infantryArrangementOrderNum;
            InfantryArrangementOrder = helpers.GetArrangementOrder(infantryArrangementOrderNum);
            InfantryFormOrderNum = infantryFormOrderNum;
            InfantryFormOrder = helpers.GetFormOrder(infantryFormOrderNum);
            RangedArrangementOrderNum = rangedArrangementOrderNum;
            RangedArrangementOrder = helpers.GetArrangementOrder(rangedArrangementOrderNum);
            RangedFormOrderNum = rangedFormOrderNum;
            RangedFormOrder = helpers.GetFormOrder(rangedFormOrderNum);
            RangedStartPosLateralOffset = rangedStartPosLateralOffset;
            RangedStartPosFwdOffset = rangedStartPosFwdOffset;
            CavalryArrangementOrderNum = cavalryArrangementOrderNum;
            CavalryArrangementOrder = helpers.GetArrangementOrder(cavalryArrangementOrderNum);
            CavalryFormOrderNum = cavalryFormOrderNum;
            CavalryFormOrder = helpers.GetFormOrder(cavalryFormOrderNum);
            CavalryStartPosLateralOffset = cavalryStartPosLateralOffset;
            CavalryStartPosFwdOffset = cavalryStartPosFwdOffset;
            HorseArcherArrangementOrderNum = horseArcherArrangementOrderNum;
            HorseArcherArrangementOrder = helpers.GetArrangementOrder(horseArcherArrangementOrderNum);
            HorseArcherFormOrderNum = horseArcherFormOrderNum;
            HorseArcherFormOrder = helpers.GetFormOrder(horseArcherFormOrderNum);
            HorseArcherStartPosLateralOffset = horseArcherStartPosLateralOffset;
            HorseArcherStartPosFwdOffset = horseArcherStartPosFwdOffset;
            SkirmisherArrangementOrderNum = skirmisherArrangementOrderNum;
            SkirmisherArrangementOrder = helpers.GetArrangementOrder(skirmisherArrangementOrderNum);
            SkirmisherFormOrderNum = skirmisherFormOrderNum;
            SkirmisherFormOrder = helpers.GetFormOrder(skirmisherFormOrderNum);
            SkirmisherStartPosLateralOffset = skirmisherStartPosLateralOffset;
            SkirmisherStartPosFwdOffset = skirmisherStartPosFwdOffset;
            HeavyInfantryArrangementOrderNum = heavyInfantryArrangementOrderNum;
            HeavyInfantryArrangementOrder = helpers.GetArrangementOrder(heavyInfantryArrangementOrderNum);
            HeavyInfantryFormOrderNum = heavyInfantryFormOrderNum;
            HeavyInfantryFormOrder = helpers.GetFormOrder(heavyInfantryFormOrderNum);
            HeavyInfantryStartPosLateralOffset = heavyInfantryStartPosLateralOffset;
            HeavyInfantryStartPosFwdOffset = heavyInfantryStartPosFwdOffset;
            LightCavalryArrangementOrderNum = lightCavalryArrangementOrderNum;
            LightCavalryArrangementOrder = helpers.GetArrangementOrder(lightCavalryArrangementOrderNum);
            LightCavalryFormOrderNum = lightCavalryFormOrderNum;
            LightCavalryFormOrder = helpers.GetFormOrder(lightCavalryFormOrderNum);
            LightCavalryStartPosLateralOffset = lightCavalryStartPosLateralOffset;
            LightCavalryStartPosFwdOffset = lightCavalryStartPosFwdOffset;
            HeavyCavalryArrangementOrderNum = heavyCavalryArrangementOrderNum;
            HeavyCavalryArrangementOrder = helpers.GetArrangementOrder(heavyCavalryArrangementOrderNum);
            HeavyCavalryFormOrderNum = heavyCavalryFormOrderNum;
            HeavyCavalryFormOrder = helpers.GetFormOrder(heavyCavalryFormOrderNum);
            HeavyCavalryStartPosLateralOffset = heavyCavalryStartPosLateralOffset;
            HeavyCavalryStartPosFwdOffset = heavyCavalryStartPosFwdOffset;
        }
    }
}
