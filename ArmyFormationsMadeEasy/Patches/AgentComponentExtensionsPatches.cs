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

            if (Settings.Instance.AllAgentsWalk && agent.Team != agent.Mission.MainAgent?.Team)
            {
                agent.SetMaximumSpeedLimit(Settings.Instance.SpeedLimitReductionAmount, true);
            }
            else if (agent.Team == agent.Mission.MainAgent?.Team)
            {
                if (agent.Formation?.FormationIndex == FormationClass.Infantry)
                {
                    agent.SetMaximumSpeedLimit(Settings.Instance.InfantryMaxSpeedModifier, true);
                }
                else if (agent.Formation?.FormationIndex == FormationClass.Ranged)
                {
                    agent.SetMaximumSpeedLimit(Settings.Instance.RangedMaxSpeedModifier, true);
                }
                else if (agent.Formation?.FormationIndex == FormationClass.Cavalry)
                {
                    agent.SetMaximumSpeedLimit(Settings.Instance.CavalryMaxSpeedModifier, true);
                }
                else if (agent.Formation?.FormationIndex == FormationClass.HorseArcher)
                {
                    agent.SetMaximumSpeedLimit(Settings.Instance.HorseArcherMaxSpeedModifier, true);
                }
                else if (agent.Formation?.FormationIndex == FormationClass.Skirmisher)
                {
                    agent.SetMaximumSpeedLimit(Settings.Instance.SkirmisherMaxSpeedModifier, true);
                }
                else if (agent.Formation?.FormationIndex == FormationClass.HeavyInfantry)
                {
                    agent.SetMaximumSpeedLimit(Settings.Instance.HeavyInfantryMaxSpeedModifier, true);
                }
                else if (agent.Formation?.FormationIndex == FormationClass.LightCavalry)
                {
                    agent.SetMaximumSpeedLimit(Settings.Instance.LightCavalryMaxSpeedModifier, true);
                }
                else if (agent.Formation?.FormationIndex == FormationClass.HeavyCavalry)
                {
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
