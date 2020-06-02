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
    [HarmonyPatch(typeof(AgentComponentExtensions), "UpdateFormationMovement")]
    public class UpdateFormationMovementPatch
    {
        static void Postfix(Agent agent)
        {
            if (agent == null || agent.State != AgentState.Active || agent.IsRetreating())
                return;

            if (Settings.Instance.AllEnemyAllyAgentsWalk && agent.Team != agent.Mission.MainAgent?.Team)
            {
                agent.SetMaximumSpeedLimit(Settings.Instance.SpeedLimitReductionAmount, true);
            }
            else if (agent.Team == agent.Mission.MainAgent?.Team)
            {
                if (agent.Formation?.FormationIndex == FormationClass.Infantry)
                {
                    if (Settings.Instance.InfantryMaxSpeedModifier < 1)
                        agent.SetMaximumSpeedLimit(Settings.Instance.InfantryMaxSpeedModifier * 2, false);
                    else if (Settings.Instance.InfantryMaxSpeedModifier >= 1)
                        agent.SetMaximumSpeedLimit(Settings.Instance.InfantryMaxSpeedModifier, true);
                }
                else if (agent.Formation?.FormationIndex == FormationClass.Ranged)
                {
                    if (Settings.Instance.RangedMaxSpeedModifier < 1)
                        agent.SetMaximumSpeedLimit(Settings.Instance.RangedMaxSpeedModifier * 2, false);
                    else if (Settings.Instance.RangedMaxSpeedModifier >= 1)
                        agent.SetMaximumSpeedLimit(Settings.Instance.RangedMaxSpeedModifier, true);
                }
                else if (agent.Formation?.FormationIndex == FormationClass.Cavalry)
                {
                    if (Settings.Instance.CavalryMaxSpeedModifier < 1)
                        agent.SetMaximumSpeedLimit(Settings.Instance.CavalryMaxSpeedModifier * 2, false);
                    else if (Settings.Instance.CavalryMaxSpeedModifier >= 1)
                        agent.SetMaximumSpeedLimit(Settings.Instance.CavalryMaxSpeedModifier, true);
                }
                else if (agent.Formation?.FormationIndex == FormationClass.HorseArcher)
                {
                    if (Settings.Instance.HorseArcherMaxSpeedModifier < 1)
                        agent.SetMaximumSpeedLimit(Settings.Instance.HorseArcherMaxSpeedModifier * 2, false);
                    else if (Settings.Instance.HorseArcherMaxSpeedModifier >= 1)
                        agent.SetMaximumSpeedLimit(Settings.Instance.HorseArcherMaxSpeedModifier, true);
                }
                else if (agent.Formation?.FormationIndex == FormationClass.Skirmisher)
                {
                    if (Settings.Instance.SkirmisherMaxSpeedModifier < 1)
                        agent.SetMaximumSpeedLimit(Settings.Instance.SkirmisherMaxSpeedModifier * 2, false);
                    else if (Settings.Instance.SkirmisherMaxSpeedModifier >= 1)
                        agent.SetMaximumSpeedLimit(Settings.Instance.SkirmisherMaxSpeedModifier, true);
                }
                else if (agent.Formation?.FormationIndex == FormationClass.HeavyInfantry)
                {
                    if (Settings.Instance.HeavyInfantryMaxSpeedModifier < 1)
                        agent.SetMaximumSpeedLimit(Settings.Instance.HeavyInfantryMaxSpeedModifier * 2, false);
                    else if (Settings.Instance.HeavyInfantryMaxSpeedModifier >= 1)
                        agent.SetMaximumSpeedLimit(Settings.Instance.HeavyInfantryMaxSpeedModifier, true);
                }
                else if (agent.Formation?.FormationIndex == FormationClass.LightCavalry)
                {
                    if (Settings.Instance.LightCavalryMaxSpeedModifier < 1)
                        agent.SetMaximumSpeedLimit(Settings.Instance.LightCavalryMaxSpeedModifier * 2, false);
                    else if (Settings.Instance.LightCavalryMaxSpeedModifier >= 1)
                        agent.SetMaximumSpeedLimit(Settings.Instance.LightCavalryMaxSpeedModifier, true);
                }
                else if (agent.Formation?.FormationIndex == FormationClass.HeavyCavalry)
                {
                    if (Settings.Instance.HeavyCavalryMaxSpeedModifier < 1)
                        agent.SetMaximumSpeedLimit(Settings.Instance.HeavyCavalryMaxSpeedModifier * 2, false);
                    else if (Settings.Instance.HeavyCavalryMaxSpeedModifier >= 1)
                        agent.SetMaximumSpeedLimit(Settings.Instance.HeavyCavalryMaxSpeedModifier, true);
                }
            }
        }

        static bool Prepare()
        {
            return Settings.Instance.SpeedLimitReductionEnabled;
        }
    }
}
