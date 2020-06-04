using ArmyFormationsMadeEasy.CampaignBehaviors;
using HarmonyLib;
using ModLib;
using System;
using System.Windows.Forms;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace ArmyFormationsMadeEasy
{
    public class SubModule : MBSubModuleBase
    {
        public static readonly string ModuleFolderName = "ArmyFormationsMadeEasy";

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            // Is this wrong? Check M&B Libraries to see if it should be completely overriden
            base.OnBeforeInitialModuleScreenSetAsRoot();

            try
            {
                FileDatabase.Initialise(ModuleFolderName);

                var harmony = new Harmony("mod.valkenrebel.bannerlord.armyformationsmadeeasy");
                harmony.PatchAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error Initialising Army Formations Made Easy:\n\n{ex.ToStringFull()}");
            }
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            try
            {
                base.OnGameStart(game, gameStarterObject);
                if (!(game.GameType is Campaign))
                    return;
                this.AddBehaviors(gameStarterObject as CampaignGameStarter);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error Adding Behaviours - Army Formations Made Easy:\n\n{ex.ToStringFull()}");
            }

            // AddModels(gameStarterObject as CampaignGameStarter);
        }

        private void AddBehaviors(CampaignGameStarter gameStarterObject)
        {
            if (gameStarterObject == null)
                return;
            gameStarterObject.AddBehavior(SavedFormationClassesBehavior.Instance);
            gameStarterObject.LoadGameTexts(BasePath.Name + "Modules/ArmyFormationsMadeEasy/ModuleData/module_strings.xml");
        }

        private void AddModels(CampaignGameStarter gameStarter)
        {
            if (gameStarter != null)
            {
                /*
                 * EXAMPLES 
                 * if (Settings.Instance.TroopBattleExperienceMultiplierEnabled || Settings.Instance.ArenaHeroExperienceMultiplierEnabled || Settings.Instance.TournamentHeroExperienceMultiplierEnabled)
                    gameStarter.AddModel(new TweakedCombatXpModel());
                
                   if (Settings.Instance.AttributeFocusPointTweakEnabled)
                    gameStarter.AddModel(new TweakedCharacterDevelopmentModel());
                */
            }
        }
    }
}
