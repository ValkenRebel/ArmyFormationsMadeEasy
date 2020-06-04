using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.SaveSystem;

namespace ArmyFormationsMadeEasy.CampaignBehaviors
{
    internal class SavedFormationClassesBehavior : CampaignBehaviorBase
    {
        public static readonly SavedFormationClassesBehavior Instance = new SavedFormationClassesBehavior();
        public bool game_started;
        public Dictionary<BasicCharacterObject, FormationClass> formation_class_map;

        public override void RegisterEvents()
        {
        }

        public override void SyncData(IDataStore dataStore)
        {
            if (!game_started)
            formation_class_map = new Dictionary<BasicCharacterObject, FormationClass>();

            dataStore.SyncData("formation_class_map", ref formation_class_map);
        }

        public class SavedFormationClassesSaveableDefiner : SaveableTypeDefiner
        {
            public SavedFormationClassesSaveableDefiner()
            : base(56345726)
            {
            }

            protected override void DefineEnumTypes()
            {
                AddEnumDefinition(typeof (FormationClass), 56345727);
            }

            protected override void DefineContainerDefinitions()
            {
                ConstructContainerDefinition(typeof (Dictionary<BasicCharacterObject, FormationClass>));
            }
        }
    }
}
