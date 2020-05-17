using HarmonyLib;
using ModLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace ArmyFormationsMadeEasy
{
    public class SubModule : MBSubModuleBase
    {
        public static readonly string ModuleFolderName = "ArmyFormationsMadeEasy";

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            try
            {
                FileDatabase.Initialise(ModuleFolderName);

                var harmony = new Harmony("mod.bannerlord.mipen");
                harmony.PatchAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error Initialising Army Formations Made Easy:\n\n{ex.ToStringFull()}");
            }
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            base.OnGameStart(game, gameStarterObject);

            // AddModels(gameStarterObject as CampaignGameStarter);
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
