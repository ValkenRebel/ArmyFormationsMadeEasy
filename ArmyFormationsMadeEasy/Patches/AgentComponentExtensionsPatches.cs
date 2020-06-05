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
            if (agent == null || agent.State != AgentState.Active || agent.IsRetreating() || agent.Mission.CurrentState != Mission.State.Continuing)
                return;

            if (Settings.Instance.AllEnemyAllyAgentsWalk && agent.Team != agent.Mission.MainAgent?.Team)
            {
                agent.SetMaximumSpeedLimit(Settings.Instance.SpeedLimitReductionAmount, true);
            }
            else if (agent.Team == agent.Mission.MainAgent?.Team)
            {
                /*double distanceToFormationFrontAttPt = Math.Sqrt((agent.Position.X - agent.Formation.FrontAttachmentPoint.X) * (agent.Position.X - agent.Formation.FrontAttachmentPoint.X) + 
                                                                 (agent.Position.Y - agent.Formation.FrontAttachmentPoint.Y) * (agent.Position.Y - agent.Formation.FrontAttachmentPoint.Y));
                
                if (agent.Formation?.FormationIndex == FormationClass.Infantry)
                {
                    if (Settings.Instance.InfantryMaxSpeedModifier < 1)
                        if (distanceToFormationFrontAttPt < 21)
                            agent.SetMaximumSpeedLimit(Settings.Instance.InfantryMaxSpeedModifier * 2, false);
                    else if (Settings.Instance.InfantryMaxSpeedModifier >= 1)
                        agent.SetMaximumSpeedLimit(Settings.Instance.InfantryMaxSpeedModifier, true);
                }
                else if (agent.Formation?.FormationIndex == FormationClass.Ranged)
                {
                    if (Settings.Instance.RangedMaxSpeedModifier < 1)
                        if (distanceToFormationFrontAttPt < 21)
                            agent.SetMaximumSpeedLimit(Settings.Instance.RangedMaxSpeedModifier * 2, false);
                    else if (Settings.Instance.RangedMaxSpeedModifier >= 1)
                        agent.SetMaximumSpeedLimit(Settings.Instance.RangedMaxSpeedModifier, true);
                }
                else if (agent.Formation?.FormationIndex == FormationClass.Cavalry)
                {
                    if (Settings.Instance.CavalryMaxSpeedModifier < 1)
                        if (distanceToFormationFrontAttPt < 21)
                            agent.SetMaximumSpeedLimit(Settings.Instance.CavalryMaxSpeedModifier * 2, false);
                    else if (Settings.Instance.CavalryMaxSpeedModifier >= 1)
                        agent.SetMaximumSpeedLimit(Settings.Instance.CavalryMaxSpeedModifier, true);
                }
                else if (agent.Formation?.FormationIndex == FormationClass.HorseArcher)
                {
                    if (Settings.Instance.HorseArcherMaxSpeedModifier < 1)
                        if (distanceToFormationFrontAttPt < 21)
                            agent.SetMaximumSpeedLimit(Settings.Instance.HorseArcherMaxSpeedModifier * 2, false);
                    else if (Settings.Instance.HorseArcherMaxSpeedModifier >= 1)
                        agent.SetMaximumSpeedLimit(Settings.Instance.HorseArcherMaxSpeedModifier, true);
                }
                else if (agent.Formation?.FormationIndex == FormationClass.Skirmisher)
                {
                    if (Settings.Instance.SkirmisherMaxSpeedModifier < 1)
                        if (distanceToFormationFrontAttPt < 21)
                            agent.SetMaximumSpeedLimit(Settings.Instance.SkirmisherMaxSpeedModifier * 2, false);
                    else if (Settings.Instance.SkirmisherMaxSpeedModifier >= 1)
                        agent.SetMaximumSpeedLimit(Settings.Instance.SkirmisherMaxSpeedModifier, true);
                }
                else if (agent.Formation?.FormationIndex == FormationClass.HeavyInfantry)
                {
                    if (Settings.Instance.HeavyInfantryMaxSpeedModifier < 1)
                        if (distanceToFormationFrontAttPt < 21)
                            agent.SetMaximumSpeedLimit(Settings.Instance.HeavyInfantryMaxSpeedModifier * 2, false);
                    else if (Settings.Instance.HeavyInfantryMaxSpeedModifier >= 1)
                        agent.SetMaximumSpeedLimit(Settings.Instance.HeavyInfantryMaxSpeedModifier, true);
                }
                else if (agent.Formation?.FormationIndex == FormationClass.LightCavalry)
                {
                    if (Settings.Instance.LightCavalryMaxSpeedModifier < 1)
                        if (distanceToFormationFrontAttPt < 21)
                            agent.SetMaximumSpeedLimit(Settings.Instance.LightCavalryMaxSpeedModifier * 2, false);
                    else if (Settings.Instance.LightCavalryMaxSpeedModifier >= 1)
                        agent.SetMaximumSpeedLimit(Settings.Instance.LightCavalryMaxSpeedModifier, true);
                }
                else if (agent.Formation?.FormationIndex == FormationClass.HeavyCavalry)
                {
                    if (Settings.Instance.HeavyCavalryMaxSpeedModifier < 1)
                        if (distanceToFormationFrontAttPt < 21)
                            agent.SetMaximumSpeedLimit(Settings.Instance.HeavyCavalryMaxSpeedModifier * 2, false);
                    else if (Settings.Instance.HeavyCavalryMaxSpeedModifier >= 1)
                        agent.SetMaximumSpeedLimit(Settings.Instance.HeavyCavalryMaxSpeedModifier, true);
                }*/

                //agent.Formation.GetFirstUnit();
                //Vec2 middleFrontUnitPositionOffset = agent.Formation.GetMiddleFrontUnitPositionOffset();


                /*Agent medianAgent = agent.Formation.GetMedianAgent(true, true, agent.Formation.GetAveragePositionOfUnits(true, true));
                WorldPosition medianAgentOrderPosition = medianAgent.Formation.GetOrderPositionOfUnit(medianAgent);
                Vec2 medianAgentLocPos = agent.Formation.GetLocalPositionOfUnit(agent);*/


                Vec2 agentOrderPosition = agent.Formation.GetOrderPositionOfUnit(agent).AsVec2;
                float agentDistToDest = agent.Position.AsVec2.Distance(agentOrderPosition);
                if (agentDistToDest > 32 || agent.Formation.MovementOrder.OrderType == OrderType.Charge)
                    return;

                 Agent firstAgent = agent.Formation.GetFirstUnit();
                Vec2 firstAgentOrderPosition = firstAgent.Formation.GetOrderPositionOfUnit(firstAgent).AsVec2;
                float firstAgentDistToDest = firstAgent.Position.AsVec2.Distance(firstAgentOrderPosition);
                
                float slowDownModifier = 0.85f;
                if (agentDistToDest > 27)
                    slowDownModifier = 4.5f;
                else if (agentDistToDest > 22)
                    slowDownModifier = 3.5f;
                else if (agentDistToDest > 17)
                    slowDownModifier = 2.75f;
                else if (agentDistToDest > 12)
                    slowDownModifier = 2.25f;
                else if (agentDistToDest - firstAgentDistToDest > 0.5)
                    slowDownModifier = 1.0f;
                else if (agentDistToDest - firstAgentDistToDest < -0.5)
                    slowDownModifier = 0.5f;

                Console.WriteLine("Debug Line"); // For Debug Breakpoint Only
                


                if (agent.Formation?.FormationIndex == FormationClass.Infantry)
                {
                    if (Settings.Instance.InfantryMaxSpeedModifier < 1)
                        agent.SetMaximumSpeedLimit(Settings.Instance.InfantryMaxSpeedModifier * 2 * slowDownModifier, false);
                }
                else if (agent.Formation?.FormationIndex == FormationClass.Ranged)
                {
                    if (Settings.Instance.RangedMaxSpeedModifier < 1)
                        agent.SetMaximumSpeedLimit(Settings.Instance.RangedMaxSpeedModifier * 2 * slowDownModifier, false);
                }
                else if (agent.Formation?.FormationIndex == FormationClass.Cavalry)
                {
                    if (Settings.Instance.CavalryMaxSpeedModifier < 1)
                        agent.SetMaximumSpeedLimit(Settings.Instance.CavalryMaxSpeedModifier * 2 * slowDownModifier, false);
                }
                else if (agent.Formation?.FormationIndex == FormationClass.HorseArcher)
                {
                    if (Settings.Instance.HorseArcherMaxSpeedModifier < 1)
                        agent.SetMaximumSpeedLimit(Settings.Instance.HorseArcherMaxSpeedModifier * 2 * slowDownModifier, false);
                }
                else if (agent.Formation?.FormationIndex == FormationClass.Skirmisher)
                {
                    if (Settings.Instance.SkirmisherMaxSpeedModifier < 1)
                        agent.SetMaximumSpeedLimit(Settings.Instance.SkirmisherMaxSpeedModifier * 2 * slowDownModifier, false);
                }
                else if (agent.Formation?.FormationIndex == FormationClass.HeavyInfantry)
                {
                    if (Settings.Instance.HeavyInfantryMaxSpeedModifier < 1)
                        agent.SetMaximumSpeedLimit(Settings.Instance.HeavyInfantryMaxSpeedModifier * 2 * slowDownModifier, false);
                }
                else if (agent.Formation?.FormationIndex == FormationClass.LightCavalry)
                {
                    if (Settings.Instance.LightCavalryMaxSpeedModifier < 1)
                        agent.SetMaximumSpeedLimit(Settings.Instance.LightCavalryMaxSpeedModifier * 2 * slowDownModifier, false);
                }
                else if (agent.Formation?.FormationIndex == FormationClass.HeavyCavalry)
                {
                    if (Settings.Instance.HeavyCavalryMaxSpeedModifier < 1)
                        agent.SetMaximumSpeedLimit(Settings.Instance.HeavyCavalryMaxSpeedModifier * 2 * slowDownModifier, false);
                }
            }
        }

        static bool Prepare()
        {
            return Settings.Instance.SpeedLimitReductionEnabled;
        }
    }
}
