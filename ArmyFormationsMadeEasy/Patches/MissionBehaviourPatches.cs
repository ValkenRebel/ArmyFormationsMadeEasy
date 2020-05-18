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
        public static bool F9KeyPressed { get; set; } = false;
        public static bool F10KeyPressed { get; set; } = false;
        public static bool F11KeyPressed { get; set; } = false;
        public static bool F12KeyPressed { get; set; } = false;
        public static bool AllFormationFrontAttPtUpdated { get; set; } = false;
        public static bool[] FormationFrontAttPtUpdated { get; set; } = new bool[8] { false, false, false, false, false, false, false, false };
        public static bool PlayerBattleSideEnumSet { get; set; } = false;
        public static BattleSideEnum PlayerBattleSideEnum { get; set; } = BattleSideEnum.Attacker;

        public static bool CustomArmyFormationParamsSet { get; set; } = false;
        public static List<CustomArmyFormation> CustomArmyFormations { get; set; } = new List<CustomArmyFormation>();

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

            // 'F9' - Advance selected formations ten paces forward
            if ((Input.IsKeyPressed(InputKey.F9)) && !F9KeyPressed)
            {
                if (__instance.Mission.MainAgent != null && __instance.Mission.MainAgent.Health > 0 && Settings.Instance.AdvanceTenPacesEnabled)
                {
                    SelectedFormationsAdvanceTenPaces(__instance);
                    // Success message
                    InformationManager.DisplayMessage(new InformationMessage("Selected formations advance 10 paces!"));
                }
                // Prevent repeat - until 'F9' key has been released
                F9KeyPressed = true;
            }
            else if (Input.IsKeyReleased(InputKey.F9) && F9KeyPressed)
            {
                // Allow 'F9' key press to be registered once again
                F9KeyPressed = false;
            }

            // 'F10' - Fallback selected formations ten paces
            if (Input.IsKeyPressed(InputKey.F10) && !F10KeyPressed)
            {
                if (__instance.Mission.MainAgent != null && __instance.Mission.MainAgent.Health > 0 && Settings.Instance.FallbackTenPacesEnabled)
                {
                    SelectedFormationsFallbackTenPaces(__instance);
                    // Success message
                    InformationManager.DisplayMessage(new InformationMessage("Selected formations fallback 10 paces!"));

                }
                // Prevent repeat - until 'F10' key has been released
                F10KeyPressed = true;
            }
            else if (Input.IsKeyReleased(InputKey.F10) && F10KeyPressed)
            {
                // Allow 'F10' key press to be registered once again
                F10KeyPressed = false;
            }

            // 'F11' - Move all units to the First 'Enabled' Custom Army Formation positions
            if (Input.IsKeyPressed(InputKey.F11) && !F11KeyPressed)
            {
                if (__instance.Mission.MainAgent != null && __instance.Mission.MainAgent.Health > 0)
                {
                    // Move all units to the First 'Enabled' Custom Army Formation positions (Default: F11)
                    MoveAllToFirstEnabledCustArmyFormPos(__instance);
                }

                // Prevent repeat - until 'F11' key has been released
                F11KeyPressed = true;
            }
            else if (Input.IsKeyReleased(InputKey.F11) && F11KeyPressed)
            {
                // Allow 'F11' key press to be registered once again
                F11KeyPressed = false;
            }

            // 'F12' - Move all units to the Second 'Enabled' Custom Army Formation positions
            if (Input.IsKeyPressed(InputKey.F12) && !F12KeyPressed)
            {
                if (__instance.Mission.MainAgent != null && __instance.Mission.MainAgent.Health > 0)
                {
                    // Move all units to the Second 'Enabled' Custom Army Formation positions (Default: F12)
                    MoveAllToSecondEnabledCustArmyFormPos(__instance);
                }

                // Prevent repeat - until 'F12' key has been released
                F12KeyPressed = true;
            }
            else if (Input.IsKeyReleased(InputKey.F12) && F12KeyPressed)
            {
                // Allow 'F12' key press to be registered once again
                F12KeyPressed = false;
            }
        }



        // Enable the Mod if it's enabled in the in-game Mod Options
        static bool Prepare()
        {
            return Settings.Instance.CustomArmyFormationsModEnabled;
        }

        // Fill the CustomArmyFormations List
        public static void InitCustomArmyFormationsList()
        {
            CustomArmyFormation customArmyFormation00 = new CustomArmyFormation(0,
                                                                                Settings.Instance.CustomArmyFormation00Enabled, 
                                                                                Settings.Instance.CustomArmyFormation00InfantryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation00RangedArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation00RangedStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation00RangedStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation00CavalryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation00CavalryStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation00CavalryStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation00HorseArcherArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation00HorseArcherStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation00HorseArcherStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation00SkirmisherArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation00SkirmisherStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation00SkirmisherStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation00HeavyInfantryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation00HeavyInfantryStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation00HeavyInfantryStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation00LightCavalryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation00LightCavalryStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation00LightCavalryStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation00HeavyCavalryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation00HeavyCavalryStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation00HeavyCavalryStartPosFwdOffset
                                                                                );
            CustomArmyFormations.Add(customArmyFormation00);

            CustomArmyFormation customArmyFormation01 = new CustomArmyFormation(1,
                                                                                Settings.Instance.CustomArmyFormation01Enabled,
                                                                                Settings.Instance.CustomArmyFormation01InfantryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation01RangedArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation01RangedStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation01RangedStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation01CavalryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation01CavalryStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation01CavalryStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation01HorseArcherArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation01HorseArcherStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation01HorseArcherStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation01SkirmisherArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation01SkirmisherStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation01SkirmisherStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation01HeavyInfantryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation01HeavyInfantryStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation01HeavyInfantryStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation01LightCavalryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation01LightCavalryStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation01LightCavalryStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation01HeavyCavalryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation01HeavyCavalryStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation01HeavyCavalryStartPosFwdOffset
                                                                                );
            CustomArmyFormations.Add(customArmyFormation01);

            CustomArmyFormation customArmyFormation02 = new CustomArmyFormation(2,
                                                                                Settings.Instance.CustomArmyFormation02Enabled,
                                                                                Settings.Instance.CustomArmyFormation02InfantryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation02RangedArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation02RangedStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation02RangedStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation02CavalryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation02CavalryStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation02CavalryStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation02HorseArcherArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation02HorseArcherStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation02HorseArcherStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation02SkirmisherArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation02SkirmisherStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation02SkirmisherStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation02HeavyInfantryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation02HeavyInfantryStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation02HeavyInfantryStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation02LightCavalryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation02LightCavalryStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation02LightCavalryStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation02HeavyCavalryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation02HeavyCavalryStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation02HeavyCavalryStartPosFwdOffset
                                                                                );
            CustomArmyFormations.Add(customArmyFormation02);

            CustomArmyFormation customArmyFormation03 = new CustomArmyFormation(3,
                                                                                Settings.Instance.CustomArmyFormation03Enabled,
                                                                                Settings.Instance.CustomArmyFormation03InfantryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation03RangedArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation03RangedStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation03RangedStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation03CavalryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation03CavalryStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation03CavalryStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation03HorseArcherArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation03HorseArcherStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation03HorseArcherStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation03SkirmisherArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation03SkirmisherStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation03SkirmisherStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation03HeavyInfantryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation03HeavyInfantryStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation03HeavyInfantryStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation03LightCavalryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation03LightCavalryStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation03LightCavalryStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation03HeavyCavalryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation03HeavyCavalryStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation03HeavyCavalryStartPosFwdOffset
                                                                                );
            CustomArmyFormations.Add(customArmyFormation03);

            CustomArmyFormation customArmyFormation04 = new CustomArmyFormation(4,
                                                                                Settings.Instance.CustomArmyFormation04Enabled,
                                                                                Settings.Instance.CustomArmyFormation04InfantryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation04RangedArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation04RangedStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation04RangedStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation04CavalryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation04CavalryStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation04CavalryStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation04HorseArcherArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation04HorseArcherStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation04HorseArcherStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation04SkirmisherArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation04SkirmisherStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation04SkirmisherStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation04HeavyInfantryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation04HeavyInfantryStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation04HeavyInfantryStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation04LightCavalryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation04LightCavalryStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation04LightCavalryStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation04HeavyCavalryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation04HeavyCavalryStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation04HeavyCavalryStartPosFwdOffset
                                                                                );
            CustomArmyFormations.Add(customArmyFormation04);

            CustomArmyFormation customArmyFormation05 = new CustomArmyFormation(5,
                                                                                Settings.Instance.CustomArmyFormation05Enabled,
                                                                                Settings.Instance.CustomArmyFormation05InfantryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation05RangedArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation05RangedStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation05RangedStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation05CavalryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation05CavalryStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation05CavalryStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation05HorseArcherArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation05HorseArcherStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation05HorseArcherStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation05SkirmisherArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation05SkirmisherStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation05SkirmisherStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation05HeavyInfantryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation05HeavyInfantryStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation05HeavyInfantryStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation05LightCavalryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation05LightCavalryStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation05LightCavalryStartPosFwdOffset,
                                                                                Settings.Instance.CustomArmyFormation05HeavyCavalryArrangementOrder,
                                                                                Settings.Instance.CustomArmyFormation05HeavyCavalryStartPosLateralOffset,
                                                                                Settings.Instance.CustomArmyFormation05HeavyCavalryStartPosFwdOffset
                                                                                );
            CustomArmyFormations.Add(customArmyFormation05);
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
                                // Set the formation's ArrangementOrder if enabled in settings (-1 = disabled)
                                formation.ArrangementOrder = customArmyFormation.InfantryArrangementOrderNum == -1 ? formation.ArrangementOrder : customArmyFormation.InfantryArrangementOrder;
                                
                                // Give Move order to MainFormation to halt in current position.
                                WorldPosition newWorldPos = CalcWorldPosRelToFormation(MainFormation, missionBehaviourInstance, 0, 0);
                                formation.MovementOrder = MovementOrder.MovementOrderMove(newWorldPos);
                            }
                            else if (formation.FormationIndex == FormationClass.Ranged)
                            {
                                // Set the formation's ArrangementOrder if enabled in settings (-1 = disabled)
                                formation.ArrangementOrder = customArmyFormation.RangedArrangementOrderNum == -1 ? formation.ArrangementOrder : customArmyFormation.RangedArrangementOrder;

                                // Advance Ranged Formation given paces forward of MainFormation's new position.
                                WorldPosition newWorldPos = CalcWorldPosRelToFormation(MainFormation, missionBehaviourInstance, customArmyFormation.RangedStartPosLateralOffset, customArmyFormation.RangedStartPosFwdOffset);
                                formation.MovementOrder = MovementOrder.MovementOrderMove(newWorldPos);
                            }
                            else if (formation.FormationIndex == FormationClass.Cavalry)
                            {
                                // Set the formation's ArrangementOrder if enabled in settings (-1 = disabled)
                                formation.ArrangementOrder = customArmyFormation.CavalryArrangementOrderNum == -1 ? formation.ArrangementOrder : customArmyFormation.CavalryArrangementOrder;

                                // Advance Cavalry Formation given paces left of MainFormation's new position.
                                WorldPosition newWorldPos = CalcWorldPosRelToFormation(MainFormation, missionBehaviourInstance, customArmyFormation.CavalryStartPosLateralOffset, customArmyFormation.CavalryStartPosFwdOffset);
                                formation.MovementOrder = MovementOrder.MovementOrderMove(newWorldPos);
                            }
                            else if (formation.FormationIndex == FormationClass.HorseArcher)
                            {
                                // Set the formation's ArrangementOrder if enabled in settings (-1 = disabled)
                                formation.ArrangementOrder = customArmyFormation.HorseArcherArrangementOrderNum == -1 ? formation.ArrangementOrder : customArmyFormation.HorseArcherArrangementOrder;

                                // Advance HorseArcher Formation given paces right of MainFormation's new position.
                                WorldPosition newWorldPos = CalcWorldPosRelToFormation(MainFormation, missionBehaviourInstance, customArmyFormation.HorseArcherStartPosLateralOffset, customArmyFormation.HorseArcherStartPosFwdOffset);
                                formation.MovementOrder = MovementOrder.MovementOrderMove(newWorldPos);
                            }
                            else if (formation.FormationIndex == FormationClass.Skirmisher)
                            {
                                // Set the formation's ArrangementOrder if enabled in settings (-1 = disabled)
                                formation.ArrangementOrder = customArmyFormation.SkirmisherArrangementOrderNum == -1 ? formation.ArrangementOrder : customArmyFormation.SkirmisherArrangementOrder;

                                // Advance Skirmisher Formation given paces behind MainFormation's new position.
                                WorldPosition newWorldPos = CalcWorldPosRelToFormation(MainFormation, missionBehaviourInstance, customArmyFormation.SkirmisherStartPosLateralOffset, customArmyFormation.SkirmisherStartPosFwdOffset);
                                formation.MovementOrder = MovementOrder.MovementOrderMove(newWorldPos);
                            }
                            else if (formation.FormationIndex == FormationClass.HeavyInfantry)
                            {
                                // Set the formation's ArrangementOrder if enabled in settings (-1 = disabled)
                                formation.ArrangementOrder = customArmyFormation.HeavyInfantryArrangementOrderNum == -1 ? formation.ArrangementOrder : customArmyFormation.HeavyInfantryArrangementOrder;

                                // Advance HeavyInfantry Formation given paces behind MainFormation's new position.
                                WorldPosition newWorldPos = CalcWorldPosRelToFormation(MainFormation, missionBehaviourInstance, customArmyFormation.HeavyInfantryStartPosLateralOffset, customArmyFormation.HeavyInfantryStartPosFwdOffset);
                                formation.MovementOrder = MovementOrder.MovementOrderMove(newWorldPos);
                            }
                            else if (formation.FormationIndex == FormationClass.LightCavalry)
                            {
                                // Set the formation's ArrangementOrder if enabled in settings (-1 = disabled)
                                formation.ArrangementOrder = customArmyFormation.LightCavalryArrangementOrderNum == -1 ? formation.ArrangementOrder : customArmyFormation.LightCavalryArrangementOrder;

                                // Advance LightCavalry Formation given paces behind/left MainFormation's new position.
                                WorldPosition newWorldPos = CalcWorldPosRelToFormation(MainFormation, missionBehaviourInstance, customArmyFormation.LightCavalryStartPosLateralOffset, customArmyFormation.LightCavalryStartPosFwdOffset);
                                formation.MovementOrder = MovementOrder.MovementOrderMove(newWorldPos);
                            }
                            else if (formation.FormationIndex == FormationClass.HeavyCavalry)
                            {
                                // Set the formation's ArrangementOrder if enabled in settings (-1 = disabled)
                                formation.ArrangementOrder = customArmyFormation.HeavyCavalryArrangementOrderNum == -1 ? formation.ArrangementOrder : customArmyFormation.HeavyCavalryArrangementOrder;

                                // Advance HeavyCavalry Formation given paces behind/right MainFormation's new position.
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

        // Move all units to  the First Enabled Custom Army Formation positions (Default: F11)
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

        // Move all units to  the Second Enabled Custom Army Formation positions (Default: F11)
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
    }

    public class CustomArmyFormation
    {
        public int CustomArmyFormationIndex { get; set; } = 0;
        public bool Enabled { get; set; } = false;
        public int InfantryArrangementOrderNum { get; set; } = -1;
        public ArrangementOrder InfantryArrangementOrder { get; set; } = ArrangementOrder.ArrangementOrderLine;
        public int RangedArrangementOrderNum { get; set; } = -1;
        public ArrangementOrder RangedArrangementOrder { get; set; } = ArrangementOrder.ArrangementOrderLine;
        public float RangedStartPosLateralOffset { get; set; } = 0f;
        public float RangedStartPosFwdOffset { get; set; } = 0f;
        public int CavalryArrangementOrderNum { get; set; } = -1;
        public ArrangementOrder CavalryArrangementOrder { get; set; } = ArrangementOrder.ArrangementOrderLine;
        public float CavalryStartPosLateralOffset { get; set; } = 0f;
        public float CavalryStartPosFwdOffset { get; set; } = 0f;
        public int HorseArcherArrangementOrderNum { get; set; } = -1;
        public ArrangementOrder HorseArcherArrangementOrder { get; set; } = ArrangementOrder.ArrangementOrderLine;
        public float HorseArcherStartPosLateralOffset { get; set; } = 0f;
        public float HorseArcherStartPosFwdOffset { get; set; } = 0f;
        public int SkirmisherArrangementOrderNum { get; set; } = -1;
        public ArrangementOrder SkirmisherArrangementOrder { get; set; } = ArrangementOrder.ArrangementOrderLine;
        public float SkirmisherStartPosLateralOffset { get; set; } = 0f;
        public float SkirmisherStartPosFwdOffset { get; set; } = 0f;
        public int HeavyInfantryArrangementOrderNum { get; set; } = -1;
        public ArrangementOrder HeavyInfantryArrangementOrder { get; set; } = ArrangementOrder.ArrangementOrderLine;
        public float HeavyInfantryStartPosLateralOffset { get; set; } = 0f;
        public float HeavyInfantryStartPosFwdOffset { get; set; } = 0f;
        public int LightCavalryArrangementOrderNum { get; set; } = -1;
        public ArrangementOrder LightCavalryArrangementOrder { get; set; } = ArrangementOrder.ArrangementOrderLine;
        public float LightCavalryStartPosLateralOffset { get; set; } = 0f;
        public float LightCavalryStartPosFwdOffset { get; set; } = 0f;
        public int HeavyCavalryArrangementOrderNum { get; set; } = -1;
        public ArrangementOrder HeavyCavalryArrangementOrder { get; set; } = ArrangementOrder.ArrangementOrderLine;
        public float HeavyCavalryStartPosLateralOffset { get; set; } = 0f;
        public float HeavyCavalryStartPosFwdOffset { get; set; } = 0f;

        public CustomArmyFormation( int customArmyFormationIndex,
                                    bool enabled,
                                    int infantryArrangementOrderNum,
                                    int rangedArrangementOrderNum, float rangedStartPosLateralOffset, float rangedStartPosFwdOffset,
                                    int cavalryArrangementOrderNum, float cavalryStartPosLateralOffset, float cavalryStartPosFwdOffset,
                                    int horseArcherArrangementOrderNum, float horseArcherStartPosLateralOffset, float horseArcherStartPosFwdOffset,
                                    int skirmisherArrangementOrderNum, float skirmisherStartPosLateralOffset, float skirmisherStartPosFwdOffset,
                                    int heavyInfantryArrangementOrderNum, float heavyInfantryStartPosLateralOffset, float heavyInfantryStartPosFwdOffset,
                                    int lightCavalryArrangementOrderNum, float lightCavalryStartPosLateralOffset, float lightCavalryStartPosFwdOffset,
                                    int heavyCavalryArrangementOrderNum, float heavyCavalryStartPosLateralOffset, float heavyCavalryStartPosFwdOffset)
        {
            Helpers helpers = new Helpers();
            CustomArmyFormationIndex = customArmyFormationIndex;
            Enabled = enabled;
            InfantryArrangementOrderNum = infantryArrangementOrderNum;
            InfantryArrangementOrder = helpers.GetArrangementOrder(infantryArrangementOrderNum);
            RangedArrangementOrderNum = rangedArrangementOrderNum;
            RangedArrangementOrder = helpers.GetArrangementOrder(rangedArrangementOrderNum);
            RangedStartPosLateralOffset = rangedStartPosLateralOffset;
            RangedStartPosFwdOffset = rangedStartPosFwdOffset;
            CavalryArrangementOrderNum = cavalryArrangementOrderNum;
            CavalryArrangementOrder = helpers.GetArrangementOrder(cavalryArrangementOrderNum);
            CavalryStartPosLateralOffset = cavalryStartPosLateralOffset;
            CavalryStartPosFwdOffset = cavalryStartPosFwdOffset;
            HorseArcherArrangementOrderNum = horseArcherArrangementOrderNum;
            HorseArcherArrangementOrder = helpers.GetArrangementOrder(horseArcherArrangementOrderNum);
            HorseArcherStartPosLateralOffset = horseArcherStartPosLateralOffset;
            HorseArcherStartPosFwdOffset = horseArcherStartPosFwdOffset;
            SkirmisherArrangementOrderNum = skirmisherArrangementOrderNum;
            SkirmisherArrangementOrder = helpers.GetArrangementOrder(skirmisherArrangementOrderNum);
            SkirmisherStartPosLateralOffset = skirmisherStartPosLateralOffset;
            SkirmisherStartPosFwdOffset = skirmisherStartPosFwdOffset;
            HeavyInfantryArrangementOrderNum = heavyInfantryArrangementOrderNum;
            HeavyInfantryArrangementOrder = helpers.GetArrangementOrder(heavyInfantryArrangementOrderNum);
            HeavyInfantryStartPosLateralOffset = heavyInfantryStartPosLateralOffset;
            HeavyInfantryStartPosFwdOffset = heavyInfantryStartPosFwdOffset;
            LightCavalryArrangementOrderNum = lightCavalryArrangementOrderNum;
            LightCavalryArrangementOrder = helpers.GetArrangementOrder(lightCavalryArrangementOrderNum);
            LightCavalryStartPosLateralOffset = lightCavalryStartPosLateralOffset;
            LightCavalryStartPosFwdOffset = lightCavalryStartPosFwdOffset;
            HeavyCavalryArrangementOrderNum = heavyCavalryArrangementOrderNum;
            HeavyCavalryArrangementOrder = helpers.GetArrangementOrder(heavyCavalryArrangementOrderNum);
            HeavyCavalryStartPosLateralOffset = heavyCavalryStartPosLateralOffset;
            HeavyCavalryStartPosFwdOffset = heavyCavalryStartPosFwdOffset;
        }
    }
}
