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
        public static bool PlayerBattleSideEnumSet { get; set; } = false;
        public static BattleSideEnum PlayerBattleSideEnum { get; set; } = BattleSideEnum.Attacker;

        public static bool CustomArmyFormationParamsSet { get; set; } = false;
        public static List<CustomArmyFormation> CustomArmyFormations { get; set; } = new List<CustomArmyFormation>();

        // Enable the Mod if it's enabled in the in-game Mod Options
        static bool Prepare()
        {
            return Settings.Instance.CustomArmyFormationsModEnabled;
        }


        // Patched 'MissionBehaviour.OnMissionTick()' (Postfix)
        static void Postfix(MissionBehaviour __instance)
        {
            // Get the Player's Team.Side
            if (!PlayerBattleSideEnumSet && __instance.Mission.MainAgent != null)
            {
                PlayerBattleSideEnum = __instance.Mission.MainAgent.Team.Side;
                PlayerBattleSideEnumSet = true;
            }

            // Fill the Custom Army Formations List - from the current Mod 'Settings' Options
            if (CustomArmyFormations.Count == 0 && __instance.Mission.MainAgent != null)
            {
                InitCustomArmyFormationsList();
            }

            // Update if Alt, Ctrl, or Shift are currently held down (Left & Right)
            UpdateAltKeyStates();

            // 'Default: F9' - Advance selected formations 'x' paces forward
            if ((Input.IsKeyPressed(Config.AdvanceSelectedKey)) && !AdvanceSelectedKeyPressed)
            {
                if (!AnyAlternateKeysHeldDown && __instance.Mission.MainAgent != null && __instance.Mission.MainAgent.Health > 0 && Settings.Instance.AdvanceTenPacesEnabled)
                {
                    SelectedFormationsAdvanceTenPaces(__instance);
                    // Success message
                    InformationManager.DisplayMessage(new InformationMessage("Selected formations advance 10 paces!"));
                }
                // Prevent repeat - until 'AdvanceSelectedKey' has been released
                AdvanceSelectedKeyPressed = true;
            }
            else if (Input.IsKeyReleased(Config.AdvanceSelectedKey) && AdvanceSelectedKeyPressed)
            {
                // Allow 'AdvanceSelectedKey' press to be registered once again
                AdvanceSelectedKeyPressed = false;
            }

            // 'Default: F10' - Fallback selected formations 'x' paces backwards
            if (Input.IsKeyPressed(Config.FallbackSelectedKey) && !FallbackSelectedKeyPressed)
            {
                if (!AnyAlternateKeysHeldDown && __instance.Mission.MainAgent != null && __instance.Mission.MainAgent.Health > 0 && Settings.Instance.FallbackTenPacesEnabled)
                {
                    SelectedFormationsFallbackTenPaces(__instance);
                    // Success message
                    InformationManager.DisplayMessage(new InformationMessage("Selected formations fallback 10 paces!"));

                }
                // Prevent repeat - until 'FallbackSelectedKey' has been released
                FallbackSelectedKeyPressed = true;
            }
            else if (Input.IsKeyReleased(Config.FallbackSelectedKey) && FallbackSelectedKeyPressed)
            {
                // Allow 'FallbackSelectedKey' press to be registered once again
                FallbackSelectedKeyPressed = false;
            }

            // 'Default: F11' - Move all units to 'Custom Army Formation 1' positions
            if (Input.IsKeyPressed(Config.CustomFormation1Key) && !CustomFormation1KeyPressed)
            {
                if (!AnyAlternateKeysHeldDown && __instance.Mission.MainAgent != null && __instance.Mission.MainAgent.Health > 0)
                {
                    // Move all units to 'Custom Army Formation 1' positions (Default: F11)
                    MoveAllToCustArmyFormPos(CustomArmyFormations[0], __instance);

                    // Display message
                    InformationManager.DisplayMessage(new InformationMessage("All units move to: 'Custom Army Formation 1'"));
                }

                // Prevent repeat - until 'CustomFormation1Key' has been released
                CustomFormation1KeyPressed = true;
            }
            else if (Input.IsKeyReleased(Config.CustomFormation1Key) && CustomFormation1KeyPressed)
            {
                // Allow 'CustomFormation1Key' press to be registered once again
                CustomFormation1KeyPressed = false;
            }

            // 'Default: F12' - Move all units to 'Custom Army Formation 2' positions
            if (Input.IsKeyPressed(Config.CustomFormation2Key) && !CustomFormation2KeyPressed)
            {
                if (!AnyAlternateKeysHeldDown && __instance.Mission.MainAgent != null && __instance.Mission.MainAgent.Health > 0)
                {
                    // Move all units to 'Custom Army Formation 2' positions (Default: F12)
                    MoveAllToCustArmyFormPos(CustomArmyFormations[1], __instance);

                    // Display message
                    InformationManager.DisplayMessage(new InformationMessage("All units move to: 'Custom Army Formation 2'"));
                }

                // Prevent repeat - until 'CustomFormation2Key' has been released
                CustomFormation2KeyPressed = true;
            }
            else if (Input.IsKeyReleased(Config.CustomFormation2Key) && CustomFormation2KeyPressed)
            {
                // Allow 'CustomFormation2Key' press to be registered once again
                CustomFormation2KeyPressed = false;
            }

            // 'Default: NumPad5' - Move all units to 'Custom Army Formation 3' positions
            if (Input.IsKeyPressed(Config.CustomFormation3Key) && !CustomFormation3KeyPressed)
            {
                if (!AnyAlternateKeysHeldDown && __instance.Mission.MainAgent != null && __instance.Mission.MainAgent.Health > 0)
                {
                    // Move all units to 'Custom Army Formation 3' positions (Default: NumPad5)
                    MoveAllToCustArmyFormPos(CustomArmyFormations[2], __instance);

                    // Display message
                    InformationManager.DisplayMessage(new InformationMessage("All units move to: 'Custom Army Formation 3'"));
                }

                // Prevent repeat - until 'CustomFormation3Key' has been released
                CustomFormation3KeyPressed = true;
            }
            else if (Input.IsKeyReleased(Config.CustomFormation3Key) && CustomFormation3KeyPressed)
            {
                // Allow 'CustomFormation3Key' press to be registered once again
                CustomFormation3KeyPressed = false;
            }

            // 'Default: NumPad6' - Move all units to 'Custom Army Formation 4' positions
            if (Input.IsKeyPressed(Config.CustomFormation4Key) && !CustomFormation4KeyPressed)
            {
                if (!AnyAlternateKeysHeldDown && __instance.Mission.MainAgent != null && __instance.Mission.MainAgent.Health > 0)
                {
                    // Move all units to 'Custom Army Formation 4' positions (Default: NumPad6)
                    MoveAllToCustArmyFormPos(CustomArmyFormations[3], __instance);

                    // Display message
                    InformationManager.DisplayMessage(new InformationMessage("All units move to: 'Custom Army Formation 4'"));
                }

                // Prevent repeat - until 'CustomFormation4Key' has been released
                CustomFormation4KeyPressed = true;
            }
            else if (Input.IsKeyReleased(Config.CustomFormation4Key) && CustomFormation4KeyPressed)
            {
                // Allow 'CustomFormation4Key' press to be registered once again
                CustomFormation4KeyPressed = false;
            }

            // 'Default: NumPad7' - Move all units to 'Custom Army Formation 5' positions
            if (Input.IsKeyPressed(Config.CustomFormation5Key) && !CustomFormation5KeyPressed)
            {
                if (!AnyAlternateKeysHeldDown && __instance.Mission.MainAgent != null && __instance.Mission.MainAgent.Health > 0)
                {
                    // Move all units to 'Custom Army Formation 5' positions (Default: NumPad7)
                    MoveAllToCustArmyFormPos(CustomArmyFormations[4], __instance);

                    // Display message
                    InformationManager.DisplayMessage(new InformationMessage("All units move to: 'Custom Army Formation 5'"));
                }

                // Prevent repeat - until 'CustomFormation5Key' has been released
                CustomFormation5KeyPressed = true;
            }
            else if (Input.IsKeyReleased(Config.CustomFormation5Key) && CustomFormation5KeyPressed)
            {
                // Allow 'CustomFormation5Key' press to be registered once again
                CustomFormation5KeyPressed = false;
            }

            // 'Default: NumPad8' - Move all units to 'Custom Army Formation 6' positions
            if (Input.IsKeyPressed(Config.CustomFormation6Key) && !CustomFormation6KeyPressed)
            {
                if (!AnyAlternateKeysHeldDown && __instance.Mission.MainAgent != null && __instance.Mission.MainAgent.Health > 0)
                {
                    // Move all units to 'Custom Army Formation 6' positions (Default: NumPad8)
                    MoveAllToCustArmyFormPos(CustomArmyFormations[5], __instance);

                    // Display message
                    InformationManager.DisplayMessage(new InformationMessage("All units move to: 'Custom Army Formation 6'"));
                }

                // Prevent repeat - until 'CustomFormation6Key' has been released
                CustomFormation6KeyPressed = true;
            }
            else if (Input.IsKeyReleased(Config.CustomFormation6Key) && CustomFormation6KeyPressed)
            {
                // Allow 'CustomFormation6Key' press to be registered once again
                CustomFormation6KeyPressed = false;
            }

            // 'Default: NumPad9' - Move all units to 'Custom Army Formation 7' positions
            if (Input.IsKeyPressed(Config.CustomFormation7Key) && !CustomFormation7KeyPressed)
            {
                if (!AnyAlternateKeysHeldDown && __instance.Mission.MainAgent != null && __instance.Mission.MainAgent.Health > 0)
                {
                    // Move all units to 'Custom Army Formation 7' positions (Default: NumPad9)
                    MoveAllToCustArmyFormPos(CustomArmyFormations[6], __instance);

                    // Display message
                    InformationManager.DisplayMessage(new InformationMessage("All units move to: 'Custom Army Formation 7'"));
                }

                // Prevent repeat - until 'CustomFormation7Key' has been released
                CustomFormation7KeyPressed = true;
            }
            else if (Input.IsKeyReleased(Config.CustomFormation7Key) && CustomFormation7KeyPressed)
            {
                // Allow 'CustomFormation7Key' press to be registered once again
                CustomFormation7KeyPressed = false;
            }

            // 'Default: NumPad0' - Move all units to 'Custom Army Formation 8' positions
            if (Input.IsKeyPressed(Config.CustomFormation8Key) && !CustomFormation8KeyPressed)
            {
                if (!AnyAlternateKeysHeldDown && __instance.Mission.MainAgent != null && __instance.Mission.MainAgent.Health > 0)
                {
                    // Move all units to 'Custom Army Formation 8' positions (Default: NumPad0)
                    MoveAllToCustArmyFormPos(CustomArmyFormations[7], __instance);

                    // Display message
                    InformationManager.DisplayMessage(new InformationMessage("All units move to: 'Custom Army Formation 8'"));
                }

                // Prevent repeat - until 'CustomFormation8Key' has been released
                CustomFormation8KeyPressed = true;
            }
            else if (Input.IsKeyReleased(Config.CustomFormation8Key) && CustomFormation8KeyPressed)
            {
                // Allow 'CustomFormation8Key' press to be registered once again
                CustomFormation8KeyPressed = false;
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


        // Advance currently selected formations ten paces forward (Cumulative)
        private static void SelectedFormationsAdvanceTenPaces(MissionBehaviour missionBehaviourInstance)
        {
            MBReadOnlyList<Formation> SelectedFormationsList = missionBehaviourInstance.Mission.PlayerTeam.PlayerOrderController.SelectedFormations;

            foreach (Formation formation in SelectedFormationsList)
            {
                WorldPosition newWorldPos;
                if (AllFormationFrontAttPtUpdated || FormationFrontAttPtUpdated[(int)formation.FormationIndex])
                {
                    newWorldPos = CalcWorldPosTenPacesFwdFrontAttPt(formation, missionBehaviourInstance);
                }
                else
                {
                    newWorldPos = CalcWorldPosRelToFormation(formation, missionBehaviourInstance, 0, 10f);
                    // Mark this formation's FrontAttachmentPoint as having been updated for the first time
                    FormationFrontAttPtUpdated[(int)formation.FormationIndex] = true;
                }
                formation.MovementOrder = MovementOrder.MovementOrderMove(newWorldPos);
            }
        }

        // Fallback currently selected formations ten paces forward (Cumulative)
        private static void SelectedFormationsFallbackTenPaces(MissionBehaviour missionBehaviourInstance)
        {
            MBReadOnlyList<Formation> SelectedFormationsList = missionBehaviourInstance.Mission.PlayerTeam.PlayerOrderController.SelectedFormations;

            foreach (Formation formation in SelectedFormationsList)
            {
                WorldPosition newWorldPos;
                if (AllFormationFrontAttPtUpdated || FormationFrontAttPtUpdated[(int)formation.FormationIndex])
                {
                    newWorldPos = CalcWorldPosFrontAttPtOffset(formation, missionBehaviourInstance, 0, -10f);
                }
                else
                {
                    newWorldPos = CalcWorldPosRelToFormation(formation, missionBehaviourInstance, 0, -10f);
                    // Mark this formation's FrontAttachmentPoint as having been updated for the first time
                    FormationFrontAttPtUpdated[(int)formation.FormationIndex] = true;
                }
                formation.MovementOrder = MovementOrder.MovementOrderMove(newWorldPos);
            }
        }

        // Calculate WorldPosition ten paces in front of FrontAttachmentPoint (Cumulative)
        private static WorldPosition CalcWorldPosTenPacesFwdFrontAttPt(Formation formation, MissionBehaviour missionBehaviourInstance)
        {
            float moveX = formation.Direction.x * 10;
            float moveY = formation.Direction.y * 10;
            WorldPosition formationCurWorldPos = formation.FrontAttachmentPoint;
            Vec3 formationNewPosition = new Vec3(formationCurWorldPos.X + moveX, formationCurWorldPos.Y + moveY);
            WorldPosition formationNewWorldPos = new WorldPosition(missionBehaviourInstance.Mission.Scene, formationNewPosition);
            return formationNewWorldPos;
        }

        // Calculate WorldPosition ten paces in front of FrontAttachmentPoint (Cumulative) with Fwd/Lateral Offset (Fwd/Back, Left/Right)
        // Used for Custom Army Formations at the beginning of a battle.
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



        // Move all units to the given Custom Army Formation positions
        private static void MoveAllToCustArmyFormPos(CustomArmyFormation customArmyFormation, MissionBehaviour missionBehaviourInstance)
        {
            List<Team> teams = (from t in missionBehaviourInstance.Mission.Teams
                                where t.Side == PlayerBattleSideEnum
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

        // (deprecated) Move all units to  the First Enabled Custom Army Formation positions (Default: F11)
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

        // (deprecated) Move all units to  the Second Enabled Custom Army Formation positions (Default: F11)
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
