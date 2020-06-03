using ModLib.Definitions.Attributes;
using ModLib.Definitions;
using System.Xml.Serialization;

namespace ArmyFormationsMadeEasy
{
    public class Settings : SettingsBase
    {
        public const string InstanceID = "ArmyFormationsMadeEasySettings";
        public override string ModName => "Army Formations Made Easy";
        public override string ModuleFolderName => SubModule.ModuleFolderName;

        [XmlElement]
        public override string ID { get; set; } = InstanceID;

        public static Settings Instance
        {
            get
            {
                return (Settings)SettingsDatabase.GetSettings<Settings>();
            }
        }

        #region MissionBehaviour Patches
        #region Custom Formation Settings
        [XmlElement]
        [SettingProperty("Allow 'Advance X Paces' Move Order", "Advance (Default: press F9) last selected units by X Paces.")]
        [SettingPropertyGroup("Allow 'Advance X Paces' Move Order   (Default: press F9)", true)]
        public bool AdvanceYPacesEnabled { get; set; } = true;
        [XmlElement]
        [SettingProperty("X", -20, 20, "Advance (Default: press F9) last selected units by 'X' Paces. Default: 'X'= 10")]
        [SettingPropertyGroup("Allow 'Advance X Paces' Move Order   (Default: press F9)")]
        public int AdvanceYPaces { get; set; } = 10;

        [XmlElement]
        [SettingProperty("Allow 'Fallback X Paces' Move Order", "Fallback (Default: press F10) last selected units by X Paces.")]
        [SettingPropertyGroup("Allow 'Fallback X Paces' Move Order   (Default: press F10)", true)]
        public bool FallbackYPacesEnabled { get; set; } = true;
        [XmlElement]
        [SettingProperty("X", -20, 20, "Fallback (Default: press F10) last selected units by 'X' Paces. Default: 'X'= -10")]
        [SettingPropertyGroup("Allow 'Fallback X Paces' Move Order   (Default: press F10)")]
        public int FallbackYPaces { get; set; } = -10;

        [XmlElement]
        [SettingProperty("Allow 'Reduce Unit Speed Limit'", "(Default: press minus(-)) Multiply last selected units Max Speed by input amount.")]
        [SettingPropertyGroup("Allow 'Reduce Unit Speed Limit'   (Default: press minus(-))", true)]
        public bool SpeedLimitReductionEnabled { get; set; } = true;
        [XmlElement]
        [SettingProperty("Speed Multiplier", 0, 1, "(press Ctrl + equals(=) to reset) Multiply last selected units Max Speed by input amount. Default: 'Speed Multiplier'= 0.25f")]
        [SettingPropertyGroup("Allow 'Reduce Unit Speed Limit'   (Default: press minus(-))")]
        public float SpeedLimitReductionAmount { get; set; } = 0.25f;
        [XmlElement]
        [SettingProperty("Allow 'Force Enemy/Ally To Walk'", "(Default: press Ctrl + minus(-) or Ctrl + equals(=)) Multiply Enemy/Ally units Max Speed by input amount. Default: 'Speed Multiplier'= 0.25f")]
        [SettingPropertyGroup("Allow 'Reduce Unit Speed Limit'   (Default: press minus(-))")]
        public bool AllEnemyAllyAgentsWalk { get; set; } = true;

        public float InfantryMaxSpeedModifier { get; set; } = 1;
        public float RangedMaxSpeedModifier { get; set; } = 1;
        public float CavalryMaxSpeedModifier { get; set; } = 1;
        public float HorseArcherMaxSpeedModifier { get; set; } = 1;
        public float SkirmisherMaxSpeedModifier { get; set; } = 1;
        public float HeavyInfantryMaxSpeedModifier { get; set; } = 1;
        public float LightCavalryMaxSpeedModifier { get; set; } = 1;
        public float HeavyCavalryMaxSpeedModifier { get; set; } = 1;

        [XmlElement]
        [SettingProperty("Allow 'Epic Battles AI'", 0, 1, "(Default: press PageUp to toggle On/Off) Enemy will form up 100m in front of Player's Army and begin a 'Phased Battle Plan'")]
        [SettingPropertyGroup("Allow 'Epic Battles AI'   (Default: press PageUp)", true)]
        public bool AllowEpicBattleAI { get; set; } = true;

        [XmlElement]
        [SettingProperty("Enable Custom Army Formations", "Toggle Mod ON/OFF")]
        /*[SettingPropertyGroup("Allow Custom Army Formations - press F11 or F12", true)]*/
        public bool CustomArmyFormationsModEnabled { get; set; } = true;

        [XmlElement]
        [SettingProperty("Enable Helpful Tips", "Toggle helpful in-battle tips ON/OFF")]
        public bool TipsEnabled { get; set; } = true;



        [XmlElement]
        [SettingProperty("Custom Army Formation 1   (Default: press F11)", "'3x4 Line' Formation: Left:Cavalry(III)/LightCavalry(VII). Center:Ranged(II)/Infantry(I)/Skirmisher(V)/HeavyInfantry(VI). Right:HorseArcher(IV)/HeavyCavalry(VIII).")]
        [SettingPropertyGroup("Custom Army Formation 1   (Default: press F11)", true)]
        public bool CustomArmyFormation00Enabled { get; set; } = true;
        [XmlElement]
        [SettingProperty("Unit Arrangement - Infantry (I)", -1, 7,  "-1=NotSet (default), 0=Circle, 1=Column, 2=Line, 3=Loose, 4=Scatter, 5=ShieldWall, 6=Skein, 7=Square")]
        [SettingPropertyGroup("Custom Army Formation 1   (Default: press F11)")]
        public int CustomArmyFormation00InfantryArrangementOrder { get; set; } = -1;
        [XmlElement]
        [SettingProperty("Front Width - Infantry (I)", -3, 100, "-3=WiderPreset, -2=WidePreset, -1=DeepPreset, 0=NotSet(default), 1-100=Custom Width")]
        [SettingPropertyGroup("Custom Army Formation 1   (Default: press F11)")]
        public int CustomArmyFormation00InfantryFormOrder { get; set; } = 0;
        [XmlElement]
        [SettingProperty("Unit Arrangement - Ranged (II)", -1, 7,  "-1=NotSet (default), 0=Circle, 1=Column, 2=Line, 3=Loose, 4=Scatter, 5=ShieldWall, 6=Skein, 7=Square")]
        [SettingPropertyGroup("Custom Army Formation 1   (Default: press F11)")]
        public int CustomArmyFormation00RangedArrangementOrder { get; set; } = -1;
        [XmlElement]
        [SettingProperty("Front Width - Ranged (II)", -3, 100, "-3=WiderPreset, -2=WidePreset, -1=DeepPreset, 0=NotSet(default), 1-100=Custom Width")]
        [SettingPropertyGroup("Custom Army Formation 1   (Default: press F11)")]
        public int CustomArmyFormation00RangedFormOrder { get; set; } = 0;
        [XmlElement]
        [SettingProperty("Lateral Offset - Position - Ranged (II)", -150f, 150f, "Default value is 0f (Positive Value = Offset Right relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 1   (Default: press F11)")]
        public float CustomArmyFormation00RangedStartPosLateralOffset { get; set; } = 0f;
        [XmlElement]
        [SettingProperty("Forward Offset - Position - Ranged (II)", -150f, 150f, "Default value is 10f (Positive Value = Offset Forward relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 1   (Default: press F11)")]
        public float CustomArmyFormation00RangedStartPosFwdOffset { get; set; } = 10f;
        [XmlElement]
        [SettingProperty("Unit Arrangement - Cavalry (III)", -1, 7,  "-1=NotSet (default), 0=Circle, 1=Column, 2=Line, 3=Loose, 4=Scatter, 5=ShieldWall, 6=Skein, 7=Square")]
        [SettingPropertyGroup("Custom Army Formation 1   (Default: press F11)")]
        public int CustomArmyFormation00CavalryArrangementOrder { get; set; } = -1;
        [XmlElement]
        [SettingProperty("Front Width - Cavalry (III)", -3, 100, "-3=WiderPreset, -2=WidePreset, -1=DeepPreset, 0=NotSet(default), 1-100=Custom Width")]
        [SettingPropertyGroup("Custom Army Formation 1   (Default: press F11)")]
        public int CustomArmyFormation00CavalryFormOrder { get; set; } = 0;
        [XmlElement]
        [SettingProperty("Lateral Offset - Position - Cavalry (III)", -150f, 150f, "Default value is -50f (Positive Value = Offset Right relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 1   (Default: press F11)")]
        public float CustomArmyFormation00CavalryStartPosLateralOffset { get; set; } = -50f;
        [XmlElement]
        [SettingProperty("Forward Offset - Position - Cavalry (III)", -150f, 150f, "Default value is 0f (Positive Value = Offset Forward relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 1   (Default: press F11)")]
        public float CustomArmyFormation00CavalryStartPosFwdOffset { get; set; } = 0f;
        [XmlElement]
        [SettingProperty("Unit Arrangement - HorseArcher (IV)", -1, 7,  "-1=NotSet (default), 0=Circle, 1=Column, 2=Line, 3=Loose, 4=Scatter, 5=ShieldWall, 6=Skein, 7=Square")]
        [SettingPropertyGroup("Custom Army Formation 1   (Default: press F11)")]
        public int CustomArmyFormation00HorseArcherArrangementOrder { get; set; } = -1;
        [XmlElement]
        [SettingProperty("Front Width - HorseArcher (IV)", -3, 100, "-3=WiderPreset, -2=WidePreset, -1=DeepPreset, 0=NotSet(default), 1-100=Custom Width")]
        [SettingPropertyGroup("Custom Army Formation 1   (Default: press F11)")]
        public int CustomArmyFormation00HorseArcherFormOrder { get; set; } = 0;
        [XmlElement]
        [SettingProperty("Lateral Offset - Position - HorseArcher (IV)", -150f, 150f, "Default value is 50f (Positive Value = Offset Right relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 1   (Default: press F11)")]
        public float CustomArmyFormation00HorseArcherStartPosLateralOffset { get; set; } = 50f;
        [XmlElement]
        [SettingProperty("Forward Offset - Position - HorseArcher (IV)", -150f, 150f, "Default value is 0f (Positive Value = Offset Forward relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 1   (Default: press F11)")]
        public float CustomArmyFormation00HorseArcherStartPosFwdOffset { get; set; } = 0f;
        [XmlElement]
        [SettingProperty("Unit Arrangement - Skirmisher (V)", -1, 7,  "-1=NotSet (default), 0=Circle, 1=Column, 2=Line, 3=Loose, 4=Scatter, 5=ShieldWall, 6=Skein, 7=Square")]
        [SettingPropertyGroup("Custom Army Formation 1   (Default: press F11)")]
        public int CustomArmyFormation00SkirmisherArrangementOrder { get; set; } = -1;
        [XmlElement]
        [SettingProperty("Front Width - Skirmisher (V)", -3, 100, "-3=WiderPreset, -2=WidePreset, -1=DeepPreset, 0=NotSet(default), 1-100=Custom Width")]
        [SettingPropertyGroup("Custom Army Formation 1   (Default: press F11)")]
        public int CustomArmyFormation00SkirmisherFormOrder { get; set; } = 0;
        [XmlElement]
        [SettingProperty("Lateral Offset - Position - Skirmisher (V)", -150f, 150f, "Default value is 0f (Positive Value = Offset Right relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 1   (Default: press F11)")]
        public float CustomArmyFormation00SkirmisherStartPosLateralOffset { get; set; } = 0f;
        [XmlElement]
        [SettingProperty("Forward Offset - Position - Skirmisher (V)", -150f, 150f, "Default value is -10f (Positive Value = Offset Forward relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 1   (Default: press F11)")]
        public float CustomArmyFormation00SkirmisherStartPosFwdOffset { get; set; } = -10f;
        [XmlElement]
        [SettingProperty("Unit Arrangement - HeavyInfantry (VI)", -1, 7,  "-1=NotSet (default), 0=Circle, 1=Column, 2=Line, 3=Loose, 4=Scatter, 5=ShieldWall, 6=Skein, 7=Square")]
        [SettingPropertyGroup("Custom Army Formation 1   (Default: press F11)")]
        public int CustomArmyFormation00HeavyInfantryArrangementOrder { get; set; } = -1;
        [XmlElement]
        [SettingProperty("Front Width - HeavyInfantry (VI)", -3, 100, "-3=WiderPreset, -2=WidePreset, -1=DeepPreset, 0=NotSet(default), 1-100=Custom Width")]
        [SettingPropertyGroup("Custom Army Formation 1   (Default: press F11)")]
        public int CustomArmyFormation00HeavyInfantryFormOrder { get; set; } = 0;
        [XmlElement]
        [SettingProperty("Lateral Offset - Position - HeavyInfantry (VI)", -150f, 150f, "Default value is 0f (Positive Value = Offset Right relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 1   (Default: press F11)")]
        public float CustomArmyFormation00HeavyInfantryStartPosLateralOffset { get; set; } = 0f;
        [XmlElement]
        [SettingProperty("Forward Offset - Position - HeavyInfantry (VI)", -150f, 150f, "Default value is -20f (Positive Value = Offset Forward relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 1   (Default: press F11)")]
        public float CustomArmyFormation00HeavyInfantryStartPosFwdOffset { get; set; } = -20f;
        [XmlElement]
        [SettingProperty("Unit Arrangement - LightCavalry (VII)", -1, 7,  "-1=NotSet (default), 0=Circle, 1=Column, 2=Line, 3=Loose, 4=Scatter, 5=ShieldWall, 6=Skein, 7=Square")]
        [SettingPropertyGroup("Custom Army Formation 1   (Default: press F11)")]
        public int CustomArmyFormation00LightCavalryArrangementOrder { get; set; } = -1;
        [XmlElement]
        [SettingProperty("Front Width - LightCavalry (VII)", -3, 100, "-3=WiderPreset, -2=WidePreset, -1=DeepPreset, 0=NotSet(default), 1-100=Custom Width")]
        [SettingPropertyGroup("Custom Army Formation 1   (Default: press F11)")]
        public int CustomArmyFormation00LightCavalryFormOrder { get; set; } = 0;
        [XmlElement]
        [SettingProperty("Lateral Offset - Position - LightCavalry (VII)", -150f, 150f, "Default value is -40f (Positive Value = Offset Right relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 1   (Default: press F11)")]
        public float CustomArmyFormation00LightCavalryStartPosLateralOffset { get; set; } = -40f;
        [XmlElement]
        [SettingProperty("Forward Offset - Position - LightCavalry (VII)", -150f, 150f, "Default value is -20f (Positive Value = Offset Forward relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 1   (Default: press F11)")]
        public float CustomArmyFormation00LightCavalryStartPosFwdOffset { get; set; } = -20f;
        [XmlElement]
        [SettingProperty("Unit Arrangement - HeavyCavalry (VIII)", -1, 7,  "-1=NotSet (default), 0=Circle, 1=Column, 2=Line, 3=Loose, 4=Scatter, 5=ShieldWall, 6=Skein, 7=Square")]
        [SettingPropertyGroup("Custom Army Formation 1   (Default: press F11)")]
        public int CustomArmyFormation00HeavyCavalryArrangementOrder { get; set; } = -1;
        [XmlElement]
        [SettingProperty("Front Width - HeavyCavalry (VIII)", -3, 100, "-3=WiderPreset, -2=WidePreset, -1=DeepPreset, 0=NotSet(default), 1-100=Custom Width")]
        [SettingPropertyGroup("Custom Army Formation 1   (Default: press F11)")]
        public int CustomArmyFormation00HeavyCavalryFormOrder { get; set; } = 0;
        [XmlElement]
        [SettingProperty("Lateral Offset - Position - HeavyCavalry (VIII)", -150f, 150f, "Default value is 40f (Positive Value = Offset Right relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 1   (Default: press F11)")]
        public float CustomArmyFormation00HeavyCavalryStartPosLateralOffset { get; set; } = 40f;
        [XmlElement]
        [SettingProperty("Forward Offset - Position - HeavyCavalry (VIII)", -150f, 150f, "Default value is -20f (Positive Value = Offset Forward relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 1   (Default: press F11)")]
        public float CustomArmyFormation00HeavyCavalryStartPosFwdOffset { get; set; } = -20f;



        [XmlElement]
        [SettingProperty("Custom Army Formation 2   (Default: press F12)", "'3x4 Line' Formation - Ranged(II) Third Row: Left:Cavalry(III)/LightCavalry(VII). Center:Infantry(I)/Ranged(II)/Skirmisher(V)/HeavyInfantry(VI). Right:HorseArcher(IV)/HeavyCavalry(VIII).")]
        [SettingPropertyGroup("Custom Army Formation 2   (Default: press F12)", true)]
        public bool CustomArmyFormation01Enabled { get; set; } = true;
        [XmlElement]
        [SettingProperty("Unit Arrangement - Infantry (I)", -1, 7,  "-1=NotSet (default), 0=Circle, 1=Column, 2=Line, 3=Loose, 4=Scatter, 5=ShieldWall, 6=Skein, 7=Square")]
        [SettingPropertyGroup("Custom Army Formation 2   (Default: press F12)")]
        public int CustomArmyFormation01InfantryArrangementOrder { get; set; } = -1;
        [XmlElement]
        [SettingProperty("Front Width - Infantry (I)", -3, 100, "-3=WiderPreset, -2=WidePreset, -1=DeepPreset, 0=NotSet(default), 1-100=Custom Width")]
        [SettingPropertyGroup("Custom Army Formation 2   (Default: press F12)")]
        public int CustomArmyFormation01InfantryFormOrder { get; set; } = 0;
        [XmlElement]
        [SettingProperty("Unit Arrangement - Ranged (II)", -1, 7,  "-1=NotSet (default), 0=Circle, 1=Column, 2=Line, 3=Loose, 4=Scatter, 5=ShieldWall, 6=Skein, 7=Square")]
        [SettingPropertyGroup("Custom Army Formation 2   (Default: press F12)")]
        public int CustomArmyFormation01RangedArrangementOrder { get; set; } = -1;
        [XmlElement]
        [SettingProperty("Front Width - Ranged (II)", -3, 100, "-3=WiderPreset, -2=WidePreset, -1=DeepPreset, 0=NotSet(default), 1-100=Custom Width")]
        [SettingPropertyGroup("Custom Army Formation 2   (Default: press F12)")]
        public int CustomArmyFormation01RangedFormOrder { get; set; } = 0;
        [XmlElement]
        [SettingProperty("Lateral Offset - Position - Ranged (II)", -150f, 150f, "Default value is 0f (Positive Value = Offset Right relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 2   (Default: press F12)")]
        public float CustomArmyFormation01RangedStartPosLateralOffset { get; set; } = 0f;
        [XmlElement]
        [SettingProperty("Forward Offset - Position - Ranged (II)", -150f, 150f, "Default value is -25f (Positive Value = Offset Forward relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 2   (Default: press F12)")]
        public float CustomArmyFormation01RangedStartPosFwdOffset { get; set; } = -25f;
        [XmlElement]
        [SettingProperty("Unit Arrangement - Cavalry (III)", -1, 7,  "-1=NotSet (default), 0=Circle, 1=Column, 2=Line, 3=Loose, 4=Scatter, 5=ShieldWall, 6=Skein, 7=Square")]
        [SettingPropertyGroup("Custom Army Formation 2   (Default: press F12)")]
        public int CustomArmyFormation01CavalryArrangementOrder { get; set; } = -1;
        [XmlElement]
        [SettingProperty("Front Width - Cavalry (III)", -3, 100, "-3=WiderPreset, -2=WidePreset, -1=DeepPreset, 0=NotSet(default), 1-100=Custom Width")]
        [SettingPropertyGroup("Custom Army Formation 2   (Default: press F12)")]
        public int CustomArmyFormation01CavalryFormOrder { get; set; } = 0;
        [XmlElement]
        [SettingProperty("Lateral Offset - Position - Cavalry (III)", -150f, 150f, "Default value is -85f (Positive Value = Offset Right relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 2   (Default: press F12)")]
        public float CustomArmyFormation01CavalryStartPosLateralOffset { get; set; } = -85f;
        [XmlElement]
        [SettingProperty("Forward Offset - Position - Cavalry (III)", -150f, 150f, "Default value is 10f (Positive Value = Offset Forward relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 2   (Default: press F12)")]
        public float CustomArmyFormation01CavalryStartPosFwdOffset { get; set; } = 10f;
        [XmlElement]
        [SettingProperty("Unit Arrangement - HorseArcher (IV)", -1, 7,  "-1=NotSet (default), 0=Circle, 1=Column, 2=Line, 3=Loose, 4=Scatter, 5=ShieldWall, 6=Skein, 7=Square")]
        [SettingPropertyGroup("Custom Army Formation 2   (Default: press F12)")]
        public int CustomArmyFormation01HorseArcherArrangementOrder { get; set; } = -1;
        [XmlElement]
        [SettingProperty("Front Width - HorseArcher (IV)", -3, 100, "-3=WiderPreset, -2=WidePreset, -1=DeepPreset, 0=NotSet(default), 1-100=Custom Width")]
        [SettingPropertyGroup("Custom Army Formation 2   (Default: press F12)")]
        public int CustomArmyFormation01HorseArcherFormOrder { get; set; } = 0;
        [XmlElement]
        [SettingProperty("Lateral Offset - Position - HorseArcher (IV)", -150f, 150f, "Default value is 85f (Positive Value = Offset Right relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 2   (Default: press F12)")]
        public float CustomArmyFormation01HorseArcherStartPosLateralOffset { get; set; } = 85f;
        [XmlElement]
        [SettingProperty("Forward Offset - Position - HorseArcher (IV)", -150f, 150f, "Default value is 10f (Positive Value = Offset Forward relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 2   (Default: press F12)")]
        public float CustomArmyFormation01HorseArcherStartPosFwdOffset { get; set; } = 10f;
        [XmlElement]
        [SettingProperty("Unit Arrangement - Skirmisher (V)", -1, 7,  "-1=NotSet (default), 0=Circle, 1=Column, 2=Line, 3=Loose, 4=Scatter, 5=ShieldWall, 6=Skein, 7=Square")]
        [SettingPropertyGroup("Custom Army Formation 2   (Default: press F12)")]
        public int CustomArmyFormation01SkirmisherArrangementOrder { get; set; } = -1;
        [XmlElement]
        [SettingProperty("Front Width - Skirmisher (V)", -3, 100, "-3=WiderPreset, -2=WidePreset, -1=DeepPreset, 0=NotSet(default), 1-100=Custom Width")]
        [SettingPropertyGroup("Custom Army Formation 2   (Default: press F12)")]
        public int CustomArmyFormation01SkirmisherFormOrder { get; set; } = 0;
        [XmlElement]
        [SettingProperty("Lateral Offset - Position - Skirmisher (V)", -150f, 150f, "Default value is 0f (Positive Value = Offset Right relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 2   (Default: press F12)")]
        public float CustomArmyFormation01SkirmisherStartPosLateralOffset { get; set; } = 0f;
        [XmlElement]
        [SettingProperty("Forward Offset - Position - Skirmisher (V)", -150f, 150f, "Default value is -5f (Positive Value = Offset Forward relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 2   (Default: press F12)")]
        public float CustomArmyFormation01SkirmisherStartPosFwdOffset { get; set; } = -5f;
        [XmlElement]
        [SettingProperty("Unit Arrangement - HeavyInfantry (VI)", -1, 7,  "-1=NotSet (default), 0=Circle, 1=Column, 2=Line, 3=Loose, 4=Scatter, 5=ShieldWall, 6=Skein, 7=Square")]
        [SettingPropertyGroup("Custom Army Formation 2   (Default: press F12)")]
        public int CustomArmyFormation01HeavyInfantryArrangementOrder { get; set; } = -1;
        [XmlElement]
        [SettingProperty("Front Width - HeavyInfantry (VI)", -3, 100, "-3=WiderPreset, -2=WidePreset, -1=DeepPreset, 0=NotSet(default), 1-100=Custom Width")]
        [SettingPropertyGroup("Custom Army Formation 2   (Default: press F12)")]
        public int CustomArmyFormation01HeavyInfantryFormOrder { get; set; } = 0;
        [XmlElement]
        [SettingProperty("Lateral Offset - Position - HeavyInfantry (VI)", -150f, 150f, "Default value is 0f (Positive Value = Offset Right relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 2   (Default: press F12)")]
        public float CustomArmyFormation01HeavyInfantryStartPosLateralOffset { get; set; } = 0f;
        [XmlElement]
        [SettingProperty("Forward Offset - Position - HeavyInfantry (VI)", -150f, 150f, "Default value is -15f (Positive Value = Offset Forward relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 2   (Default: press F12)")]
        public float CustomArmyFormation01HeavyInfantryStartPosFwdOffset { get; set; } = -15f;
        [XmlElement]
        [SettingProperty("Unit Arrangement - LightCavalry (VII)", -1, 7,  "-1=NotSet (default), 0=Circle, 1=Column, 2=Line, 3=Loose, 4=Scatter, 5=ShieldWall, 6=Skein, 7=Square")]
        [SettingPropertyGroup("Custom Army Formation 2   (Default: press F12)")]
        public int CustomArmyFormation01LightCavalryArrangementOrder { get; set; } = -1;
        [XmlElement]
        [SettingProperty("Front Width - LightCavalry (VII)", -3, 100, "-3=WiderPreset, -2=WidePreset, -1=DeepPreset, 0=NotSet(default), 1-100=Custom Width")]
        [SettingPropertyGroup("Custom Army Formation 2   (Default: press F12)")]
        public int CustomArmyFormation01LightCavalryFormOrder { get; set; } = 0;
        [XmlElement]
        [SettingProperty("Lateral Offset - Position - LightCavalry (VII)", -150f, 150f, "Default value is -40f (Positive Value = Offset Right relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 2   (Default: press F12)")]
        public float CustomArmyFormation01LightCavalryStartPosLateralOffset { get; set; } = -40f;
        [XmlElement]
        [SettingProperty("Forward Offset - Position - LightCavalry (VII)", -150f, 150f, "Default value is 5f (Positive Value = Offset Forward relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 2   (Default: press F12)")]
        public float CustomArmyFormation01LightCavalryStartPosFwdOffset { get; set; } = 5f;
        [XmlElement]
        [SettingProperty("Unit Arrangement - HeavyCavalry (VIII)", -1, 7,  "-1=NotSet (default), 0=Circle, 1=Column, 2=Line, 3=Loose, 4=Scatter, 5=ShieldWall, 6=Skein, 7=Square")]
        [SettingPropertyGroup("Custom Army Formation 2   (Default: press F12)")]
        public int CustomArmyFormation01HeavyCavalryArrangementOrder { get; set; } = -1;
        [XmlElement]
        [SettingProperty("Front Width - HeavyCavalry (VIII)", -3, 100, "-3=WiderPreset, -2=WidePreset, -1=DeepPreset, 0=NotSet(default), 1-100=Custom Width")]
        [SettingPropertyGroup("Custom Army Formation 2   (Default: press F12)")]
        public int CustomArmyFormation01HeavyCavalryFormOrder { get; set; } = 0;
        [XmlElement]
        [SettingProperty("Lateral Offset - Position - HeavyCavalry (VIII)", -150f, 150f, "Default value is 40f (Positive Value = Offset Right relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 2   (Default: press F12)")]
        public float CustomArmyFormation01HeavyCavalryStartPosLateralOffset { get; set; } = 40f;
        [XmlElement]
        [SettingProperty("Forward Offset - Position - HeavyCavalry (VIII)", -150f, 150f, "Default value is 5f (Positive Value = Offset Forward relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 2   (Default: press F12)")]
        public float CustomArmyFormation01HeavyCavalryStartPosFwdOffset { get; set; } = 5f;



        [XmlElement]
        [SettingProperty("Custom Army Formation 3   (Default: press NumPad5)", "'Flat Stick-Figure' Formation: Left:Cavalry(III). CenterLeft:Ranged(II)/Infantry(I)/Skirmisher(V)/LightCavalry(VII). CenterRight:Ranged(II)/HeavyInfantry(VI)/Skirmisher(V)/HeavyCavalry(VIII) Right:HorseArcher(IV).")]
        [SettingPropertyGroup("Custom Army Formation 3   (Default: press NumPad5)", true)]
        public bool CustomArmyFormation02Enabled { get; set; } = true;
        [XmlElement]
        [SettingProperty("Unit Arrangement - Infantry (I)", -1, 7,  "-1=NotSet (default), 0=Circle, 1=Column, 2=Line, 3=Loose, 4=Scatter, 5=ShieldWall, 6=Skein, 7=Square")]
        [SettingPropertyGroup("Custom Army Formation 3   (Default: press NumPad5)")]
        public int CustomArmyFormation02InfantryArrangementOrder { get; set; } = -1;
        [XmlElement]
        [SettingProperty("Front Width - Infantry (I)", -3, 100, "-3=WiderPreset, -2=WidePreset, -1=DeepPreset, 0=NotSet(default), 1-100=Custom Width")]
        [SettingPropertyGroup("Custom Army Formation 3   (Default: press NumPad5)")]
        public int CustomArmyFormation02InfantryFormOrder { get; set; } = 0;
        [XmlElement]
        [SettingProperty("Unit Arrangement - Ranged (II)", -1, 7,  "-1=NotSet (default), 0=Circle, 1=Column, 2=Line, 3=Loose, 4=Scatter, 5=ShieldWall, 6=Skein, 7=Square")]
        [SettingPropertyGroup("Custom Army Formation 3   (Default: press NumPad5)")]
        public int CustomArmyFormation02RangedArrangementOrder { get; set; } = -1;
        [XmlElement]
        [SettingProperty("Front Width - Ranged (II)", -3, 100, "-3=WiderPreset, -2=WidePreset, -1=DeepPreset, 0=NotSet(default), 1-100=Custom Width")]
        [SettingPropertyGroup("Custom Army Formation 3   (Default: press NumPad5)")]
        public int CustomArmyFormation02RangedFormOrder { get; set; } = 0;
        [XmlElement]
        [SettingProperty("Lateral Offset - Position - Ranged (II)", -150f, 150f, "Default value is 30f (Positive Value = Offset Right relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 3   (Default: press NumPad5)")]
        public float CustomArmyFormation02RangedStartPosLateralOffset { get; set; } = 30f;
        [XmlElement]
        [SettingProperty("Forward Offset - Position - Ranged (II)", -150f, 150f, "Default value is 10f (Positive Value = Offset Forward relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 3   (Default: press NumPad5)")]
        public float CustomArmyFormation02RangedStartPosFwdOffset { get; set; } = 10f;
        [XmlElement]
        [SettingProperty("Unit Arrangement - Cavalry (III)", -1, 7,  "-1=NotSet (default), 0=Circle, 1=Column, 2=Line, 3=Loose, 4=Scatter, 5=ShieldWall, 6=Skein, 7=Square")]
        [SettingPropertyGroup("Custom Army Formation 3   (Default: press NumPad5)")]
        public int CustomArmyFormation02CavalryArrangementOrder { get; set; } = -1;
        [XmlElement]
        [SettingProperty("Front Width - Cavalry (III)", -3, 100, "-3=WiderPreset, -2=WidePreset, -1=DeepPreset, 0=NotSet(default), 1-100=Custom Width")]
        [SettingPropertyGroup("Custom Army Formation 3   (Default: press NumPad5)")]
        public int CustomArmyFormation02CavalryFormOrder { get; set; } = 0;
        [XmlElement]
        [SettingProperty("Lateral Offset - Position - Cavalry (III)", -150f, 150f, "Default value is -60f (Positive Value = Offset Right relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 3   (Default: press NumPad5)")]
        public float CustomArmyFormation02CavalryStartPosLateralOffset { get; set; } = -60f;
        [XmlElement]
        [SettingProperty("Forward Offset - Position - Cavalry (III)", -150f, 150f, "Default value is 0f (Positive Value = Offset Forward relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 3   (Default: press NumPad5)")]
        public float CustomArmyFormation02CavalryStartPosFwdOffset { get; set; } = 0f;
        [XmlElement]
        [SettingProperty("Unit Arrangement - HorseArcher (IV)", -1, 7,  "-1=NotSet (default), 0=Circle, 1=Column, 2=Line, 3=Loose, 4=Scatter, 5=ShieldWall, 6=Skein, 7=Square")]
        [SettingPropertyGroup("Custom Army Formation 3   (Default: press NumPad5)")]
        public int CustomArmyFormation02HorseArcherArrangementOrder { get; set; } = -1;
        [XmlElement]
        [SettingProperty("Front Width - HorseArcher (IV)", -3, 100, "-3=WiderPreset, -2=WidePreset, -1=DeepPreset, 0=NotSet(default), 1-100=Custom Width")]
        [SettingPropertyGroup("Custom Army Formation 3   (Default: press NumPad5)")]
        public int CustomArmyFormation02HorseArcherFormOrder { get; set; } = 0;
        [XmlElement]
        [SettingProperty("Lateral Offset - Position - HorseArcher (IV)", -150f, 150f, "Default value is 90f (Positive Value = Offset Right relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 3   (Default: press NumPad5)")]
        public float CustomArmyFormation02HorseArcherStartPosLateralOffset { get; set; } = 90f;
        [XmlElement]
        [SettingProperty("Forward Offset - Position - HorseArcher (IV)", -150f, 150f, "Default value is 0f (Positive Value = Offset Forward relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 3   (Default: press NumPad5)")]
        public float CustomArmyFormation02HorseArcherStartPosFwdOffset { get; set; } = 0f;
        [XmlElement]
        [SettingProperty("Unit Arrangement - Skirmisher (V)", -1, 7,  "-1=NotSet (default), 0=Circle, 1=Column, 2=Line, 3=Loose, 4=Scatter, 5=ShieldWall, 6=Skein, 7=Square")]
        [SettingPropertyGroup("Custom Army Formation 3   (Default: press NumPad5)")]
        public int CustomArmyFormation02SkirmisherArrangementOrder { get; set; } = -1;
        [XmlElement]
        [SettingProperty("Front Width - Skirmisher (V)", -3, 100, "-3=WiderPreset, -2=WidePreset, -1=DeepPreset, 0=NotSet(default), 1-100=Custom Width")]
        [SettingPropertyGroup("Custom Army Formation 3   (Default: press NumPad5)")]
        public int CustomArmyFormation02SkirmisherFormOrder { get; set; } = 0;
        [XmlElement]
        [SettingProperty("Lateral Offset - Position - Skirmisher (V)", -150f, 150f, "Default value is 30f (Positive Value = Offset Right relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 3   (Default: press NumPad5)")]
        public float CustomArmyFormation02SkirmisherStartPosLateralOffset { get; set; } = 30f;
        [XmlElement]
        [SettingProperty("Forward Offset - Position - Skirmisher (V)", -150f, 150f, "Default value is -10f (Positive Value = Offset Forward relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 3   (Default: press NumPad5)")]
        public float CustomArmyFormation02SkirmisherStartPosFwdOffset { get; set; } = -10f;
        [XmlElement]
        [SettingProperty("Unit Arrangement - HeavyInfantry (VI)", -1, 7,  "-1=NotSet (default), 0=Circle, 1=Column, 2=Line, 3=Loose, 4=Scatter, 5=ShieldWall, 6=Skein, 7=Square")]
        [SettingPropertyGroup("Custom Army Formation 3   (Default: press NumPad5)")]
        public int CustomArmyFormation02HeavyInfantryArrangementOrder { get; set; } = -1;
        [XmlElement]
        [SettingProperty("Front Width - HeavyInfantry (VI)", -3, 100, "-3=WiderPreset, -2=WidePreset, -1=DeepPreset, 0=NotSet(default), 1-100=Custom Width")]
        [SettingPropertyGroup("Custom Army Formation 3   (Default: press NumPad5)")]
        public int CustomArmyFormation02HeavyInfantryFormOrder { get; set; } = 0;
        [XmlElement]
        [SettingProperty("Lateral Offset - Position - HeavyInfantry (VI)", -150f, 150f, "Default value is 60f (Positive Value = Offset Right relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 3   (Default: press NumPad5)")]
        public float CustomArmyFormation02HeavyInfantryStartPosLateralOffset { get; set; } = 60f;
        [XmlElement]
        [SettingProperty("Forward Offset - Position - HeavyInfantry (VI)", -150f, 150f, "Default value is 0f (Positive Value = Offset Forward relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 3   (Default: press NumPad5)")]
        public float CustomArmyFormation02HeavyInfantryStartPosFwdOffset { get; set; } = 0f;
        [XmlElement]
        [SettingProperty("Unit Arrangement - LightCavalry (VII)", -1, 7,  "-1=NotSet (default), 0=Circle, 1=Column, 2=Line, 3=Loose, 4=Scatter, 5=ShieldWall, 6=Skein, 7=Square")]
        [SettingPropertyGroup("Custom Army Formation 3   (Default: press NumPad5)")]
        public int CustomArmyFormation02LightCavalryArrangementOrder { get; set; } = -1;
        [XmlElement]
        [SettingProperty("Front Width - LightCavalry (VII)", -3, 100, "-3=WiderPreset, -2=WidePreset, -1=DeepPreset, 0=NotSet(default), 1-100=Custom Width")]
        [SettingPropertyGroup("Custom Army Formation 3   (Default: press NumPad5)")]
        public int CustomArmyFormation02LightCavalryFormOrder { get; set; } = 0;
        [XmlElement]
        [SettingProperty("Lateral Offset - Position - LightCavalry (VII)", -150f, 150f, "Default value is 0f (Positive Value = Offset Right relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 3   (Default: press NumPad5)")]
        public float CustomArmyFormation02LightCavalryStartPosLateralOffset { get; set; } = 0f;
        [XmlElement]
        [SettingProperty("Forward Offset - Position - LightCavalry (VII)", -150f, 150f, "Default value is -20f (Positive Value = Offset Forward relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 3   (Default: press NumPad5)")]
        public float CustomArmyFormation02LightCavalryStartPosFwdOffset { get; set; } = -20f;
        [XmlElement]
        [SettingProperty("Unit Arrangement - HeavyCavalry (VIII)", -1, 7,  "-1=NotSet (default), 0=Circle, 1=Column, 2=Line, 3=Loose, 4=Scatter, 5=ShieldWall, 6=Skein, 7=Square")]
        [SettingPropertyGroup("Custom Army Formation 3   (Default: press NumPad5)")]
        public int CustomArmyFormation02HeavyCavalryArrangementOrder { get; set; } = -1;
        [XmlElement]
        [SettingProperty("Front Width - HeavyCavalry (VIII)", -3, 100, "-3=WiderPreset, -2=WidePreset, -1=DeepPreset, 0=NotSet(default), 1-100=Custom Width")]
        [SettingPropertyGroup("Custom Army Formation 3   (Default: press NumPad5)")]
        public int CustomArmyFormation02HeavyCavalryFormOrder { get; set; } = 0;
        [XmlElement]
        [SettingProperty("Lateral Offset - Position - HeavyCavalry (VIII)", -150f, 150f, "Default value is 60f (Positive Value = Offset Right relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 3   (Default: press NumPad5)")]
        public float CustomArmyFormation02HeavyCavalryStartPosLateralOffset { get; set; } = 60f;
        [XmlElement]
        [SettingProperty("Forward Offset - Position - HeavyCavalry (VIII)", -150f, 150f, "Default value is -20f (Positive Value = Offset Forward relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 3   (Default: press NumPad5)")]
        public float CustomArmyFormation02HeavyCavalryStartPosFwdOffset { get; set; } = -20f;

        

        [XmlElement]
        [SettingProperty("Custom Army Formation 4   (Default: press NumPad6)", "'Flat Stick-Figure' Formation: Left:Cavalry(III). CenterLeft:Ranged(II)/Infantry(I)/Skirmisher(V)/LightCavalry(VII). CenterRight:Ranged(II)/HeavyInfantry(VI)/Skirmisher(V)/HeavyCavalry(VIII) Right:HorseArcher(IV).")]
        [SettingPropertyGroup("Custom Army Formation 4   (Default: press NumPad6)", true)]
        public bool CustomArmyFormation03Enabled { get; set; } = true;
        [XmlElement]
        [SettingProperty("Unit Arrangement - Infantry (I)", -1, 7, "-1=NotSet (default), 0=Circle, 1=Column, 2=Line, 3=Loose, 4=Scatter, 5=ShieldWall, 6=Skein, 7=Square")]
        [SettingPropertyGroup("Custom Army Formation 4   (Default: press NumPad6)")]
        public int CustomArmyFormation03InfantryArrangementOrder { get; set; } = -1;
        [XmlElement]
        [SettingProperty("Front Width - Infantry (I)", -3, 100, "-3=WiderPreset, -2=WidePreset, -1=DeepPreset, 0=NotSet(default), 1-100=Custom Width")]
        [SettingPropertyGroup("Custom Army Formation 4   (Default: press NumPad6)")]
        public int CustomArmyFormation03InfantryFormOrder { get; set; } = 0;
        [XmlElement]
        [SettingProperty("Unit Arrangement - Ranged (II)", -1, 7, "-1=NotSet (default), 0=Circle, 1=Column, 2=Line, 3=Loose, 4=Scatter, 5=ShieldWall, 6=Skein, 7=Square")]
        [SettingPropertyGroup("Custom Army Formation 4   (Default: press NumPad6)")]
        public int CustomArmyFormation03RangedArrangementOrder { get; set; } = -1;
        [XmlElement]
        [SettingProperty("Front Width - Ranged (II)", -3, 100, "-3=WiderPreset, -2=WidePreset, -1=DeepPreset, 0=NotSet(default), 1-100=Custom Width")]
        [SettingPropertyGroup("Custom Army Formation 4   (Default: press NumPad6)")]
        public int CustomArmyFormation03RangedFormOrder { get; set; } = 0;
        [XmlElement]
        [SettingProperty("Lateral Offset - Position - Ranged (II)", -150f, 150f, "Default value is 30f (Positive Value = Offset Right relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 4   (Default: press NumPad6)")]
        public float CustomArmyFormation03RangedStartPosLateralOffset { get; set; } = 30f;
        [XmlElement]
        [SettingProperty("Forward Offset - Position - Ranged (II)", -150f, 150f, "Default value is 10f (Positive Value = Offset Forward relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 4   (Default: press NumPad6)")]
        public float CustomArmyFormation03RangedStartPosFwdOffset { get; set; } = 10f;
        [XmlElement]
        [SettingProperty("Unit Arrangement - Cavalry (III)", -1, 7, "-1=NotSet (default), 0=Circle, 1=Column, 2=Line, 3=Loose, 4=Scatter, 5=ShieldWall, 6=Skein, 7=Square")]
        [SettingPropertyGroup("Custom Army Formation 4   (Default: press NumPad6)")]
        public int CustomArmyFormation03CavalryArrangementOrder { get; set; } = -1;
        [XmlElement]
        [SettingProperty("Front Width - Cavalry (III)", -3, 100, "-3=WiderPreset, -2=WidePreset, -1=DeepPreset, 0=NotSet(default), 1-100=Custom Width")]
        [SettingPropertyGroup("Custom Army Formation 4   (Default: press NumPad6)")]
        public int CustomArmyFormation03CavalryFormOrder { get; set; } = 0;
        [XmlElement]
        [SettingProperty("Lateral Offset - Position - Cavalry (III)", -150f, 150f, "Default value is -60f (Positive Value = Offset Right relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 4   (Default: press NumPad6)")]
        public float CustomArmyFormation03CavalryStartPosLateralOffset { get; set; } = -60f;
        [XmlElement]
        [SettingProperty("Forward Offset - Position - Cavalry (III)", -150f, 150f, "Default value is 0f (Positive Value = Offset Forward relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 4   (Default: press NumPad6)")]
        public float CustomArmyFormation03CavalryStartPosFwdOffset { get; set; } = 0f;
        [XmlElement]
        [SettingProperty("Unit Arrangement - HorseArcher (IV)", -1, 7, "-1=NotSet (default), 0=Circle, 1=Column, 2=Line, 3=Loose, 4=Scatter, 5=ShieldWall, 6=Skein, 7=Square")]
        [SettingPropertyGroup("Custom Army Formation 4   (Default: press NumPad6)")]
        public int CustomArmyFormation03HorseArcherArrangementOrder { get; set; } = -1;
        [XmlElement]
        [SettingProperty("Front Width - HorseArcher (IV)", -3, 100, "-3=WiderPreset, -2=WidePreset, -1=DeepPreset, 0=NotSet(default), 1-100=Custom Width")]
        [SettingPropertyGroup("Custom Army Formation 4   (Default: press NumPad6)")]
        public int CustomArmyFormation03HorseArcherFormOrder { get; set; } = 0;
        [XmlElement]
        [SettingProperty("Lateral Offset - Position - HorseArcher (IV)", -150f, 150f, "Default value is 90f (Positive Value = Offset Right relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 4   (Default: press NumPad6)")]
        public float CustomArmyFormation03HorseArcherStartPosLateralOffset { get; set; } = 90f;
        [XmlElement]
        [SettingProperty("Forward Offset - Position - HorseArcher (IV)", -150f, 150f, "Default value is 0f (Positive Value = Offset Forward relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 4   (Default: press NumPad6)")]
        public float CustomArmyFormation03HorseArcherStartPosFwdOffset { get; set; } = 0f;
        [XmlElement]
        [SettingProperty("Unit Arrangement - Skirmisher (V)", -1, 7, "-1=NotSet (default), 0=Circle, 1=Column, 2=Line, 3=Loose, 4=Scatter, 5=ShieldWall, 6=Skein, 7=Square")]
        [SettingPropertyGroup("Custom Army Formation 4   (Default: press NumPad6)")]
        public int CustomArmyFormation03SkirmisherArrangementOrder { get; set; } = -1;
        [XmlElement]
        [SettingProperty("Front Width - Skirmisher (V)", -3, 100, "-3=WiderPreset, -2=WidePreset, -1=DeepPreset, 0=NotSet(default), 1-100=Custom Width")]
        [SettingPropertyGroup("Custom Army Formation 4   (Default: press NumPad6)")]
        public int CustomArmyFormation03SkirmisherFormOrder { get; set; } = 0;
        [XmlElement]
        [SettingProperty("Lateral Offset - Position - Skirmisher (V)", -150f, 150f, "Default value is 30f (Positive Value = Offset Right relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 4   (Default: press NumPad6)")]
        public float CustomArmyFormation03SkirmisherStartPosLateralOffset { get; set; } = 30f;
        [XmlElement]
        [SettingProperty("Forward Offset - Position - Skirmisher (V)", -150f, 150f, "Default value is -10f (Positive Value = Offset Forward relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 4   (Default: press NumPad6)")]
        public float CustomArmyFormation03SkirmisherStartPosFwdOffset { get; set; } = -10f;
        [XmlElement]
        [SettingProperty("Unit Arrangement - HeavyInfantry (VI)", -1, 7, "-1=NotSet (default), 0=Circle, 1=Column, 2=Line, 3=Loose, 4=Scatter, 5=ShieldWall, 6=Skein, 7=Square")]
        [SettingPropertyGroup("Custom Army Formation 4   (Default: press NumPad6)")]
        public int CustomArmyFormation03HeavyInfantryArrangementOrder { get; set; } = -1;
        [XmlElement]
        [SettingProperty("Front Width - HeavyInfantry (VI)", -3, 100, "-3=WiderPreset, -2=WidePreset, -1=DeepPreset, 0=NotSet(default), 1-100=Custom Width")]
        [SettingPropertyGroup("Custom Army Formation 4   (Default: press NumPad6)")]
        public int CustomArmyFormation03HeavyInfantryFormOrder { get; set; } = 0;
        [XmlElement]
        [SettingProperty("Lateral Offset - Position - HeavyInfantry (VI)", -150f, 150f, "Default value is 60f (Positive Value = Offset Right relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 4   (Default: press NumPad6)")]
        public float CustomArmyFormation03HeavyInfantryStartPosLateralOffset { get; set; } = 60f;
        [XmlElement]
        [SettingProperty("Forward Offset - Position - HeavyInfantry (VI)", -150f, 150f, "Default value is 0f (Positive Value = Offset Forward relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 4   (Default: press NumPad6)")]
        public float CustomArmyFormation03HeavyInfantryStartPosFwdOffset { get; set; } = 0f;
        [XmlElement]
        [SettingProperty("Unit Arrangement - LightCavalry (VII)", -1, 7, "-1=NotSet (default), 0=Circle, 1=Column, 2=Line, 3=Loose, 4=Scatter, 5=ShieldWall, 6=Skein, 7=Square")]
        [SettingPropertyGroup("Custom Army Formation 4   (Default: press NumPad6)")]
        public int CustomArmyFormation03LightCavalryArrangementOrder { get; set; } = -1;
        [XmlElement]
        [SettingProperty("Front Width - LightCavalry (VII)", -3, 100, "-3=WiderPreset, -2=WidePreset, -1=DeepPreset, 0=NotSet(default), 1-100=Custom Width")]
        [SettingPropertyGroup("Custom Army Formation 4   (Default: press NumPad6)")]
        public int CustomArmyFormation03LightCavalryFormOrder { get; set; } = 0;
        [XmlElement]
        [SettingProperty("Lateral Offset - Position - LightCavalry (VII)", -150f, 150f, "Default value is 0f (Positive Value = Offset Right relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 4   (Default: press NumPad6)")]
        public float CustomArmyFormation03LightCavalryStartPosLateralOffset { get; set; } = 0f;
        [XmlElement]
        [SettingProperty("Forward Offset - Position - LightCavalry (VII)", -150f, 150f, "Default value is -20f (Positive Value = Offset Forward relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 4   (Default: press NumPad6)")]
        public float CustomArmyFormation03LightCavalryStartPosFwdOffset { get; set; } = -20f;
        [XmlElement]
        [SettingProperty("Unit Arrangement - HeavyCavalry (VIII)", -1, 7, "-1=NotSet (default), 0=Circle, 1=Column, 2=Line, 3=Loose, 4=Scatter, 5=ShieldWall, 6=Skein, 7=Square")]
        [SettingPropertyGroup("Custom Army Formation 4   (Default: press NumPad6)")]
        public int CustomArmyFormation03HeavyCavalryArrangementOrder { get; set; } = -1;
        [XmlElement]
        [SettingProperty("Front Width - HeavyCavalry (VIII)", -3, 100, "-3=WiderPreset, -2=WidePreset, -1=DeepPreset, 0=NotSet(default), 1-100=Custom Width")]
        [SettingPropertyGroup("Custom Army Formation 4   (Default: press NumPad6)")]
        public int CustomArmyFormation03HeavyCavalryFormOrder { get; set; } = 0;
        [XmlElement]
        [SettingProperty("Lateral Offset - Position - HeavyCavalry (VIII)", -150f, 150f, "Default value is 60f (Positive Value = Offset Right relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 4   (Default: press NumPad6)")]
        public float CustomArmyFormation03HeavyCavalryStartPosLateralOffset { get; set; } = 60f;
        [XmlElement]
        [SettingProperty("Forward Offset - Position - HeavyCavalry (VIII)", -150f, 150f, "Default value is -20f (Positive Value = Offset Forward relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 4   (Default: press NumPad6)")]
        public float CustomArmyFormation03HeavyCavalryStartPosFwdOffset { get; set; } = -20f;



        [XmlElement]
        [SettingProperty("Custom Army Formation 5   (Default: press NumPad7)", "'Flat Stick-Figure' Formation: Left:Cavalry(III). CenterLeft:Ranged(II)/Infantry(I)/Skirmisher(V)/LightCavalry(VII). CenterRight:Ranged(II)/HeavyInfantry(VI)/Skirmisher(V)/HeavyCavalry(VIII) Right:HorseArcher(IV).")]
        [SettingPropertyGroup("Custom Army Formation 5   (Default: press NumPad7)", true)]
        public bool CustomArmyFormation04Enabled { get; set; } = true;
        [XmlElement]
        [SettingProperty("Unit Arrangement - Infantry (I)", -1, 7, "-1=NotSet (default), 0=Circle, 1=Column, 2=Line, 3=Loose, 4=Scatter, 5=ShieldWall, 6=Skein, 7=Square")]
        [SettingPropertyGroup("Custom Army Formation 5   (Default: press NumPad7)")]
        public int CustomArmyFormation04InfantryArrangementOrder { get; set; } = -1;
        [XmlElement]
        [SettingProperty("Front Width - Infantry (I)", -3, 100, "-3=WiderPreset, -2=WidePreset, -1=DeepPreset, 0=NotSet(default), 1-100=Custom Width")]
        [SettingPropertyGroup("Custom Army Formation 5   (Default: press NumPad7)")]
        public int CustomArmyFormation04InfantryFormOrder { get; set; } = 0;
        [XmlElement]
        [SettingProperty("Unit Arrangement - Ranged (II)", -1, 7, "-1=NotSet (default), 0=Circle, 1=Column, 2=Line, 3=Loose, 4=Scatter, 5=ShieldWall, 6=Skein, 7=Square")]
        [SettingPropertyGroup("Custom Army Formation 5   (Default: press NumPad7)")]
        public int CustomArmyFormation04RangedArrangementOrder { get; set; } = -1;
        [XmlElement]
        [SettingProperty("Front Width - Ranged (II)", -3, 100, "-3=WiderPreset, -2=WidePreset, -1=DeepPreset, 0=NotSet(default), 1-100=Custom Width")]
        [SettingPropertyGroup("Custom Army Formation 5   (Default: press NumPad7)")]
        public int CustomArmyFormation04RangedFormOrder { get; set; } = 0;
        [XmlElement]
        [SettingProperty("Lateral Offset - Position - Ranged (II)", -150f, 150f, "Default value is 30f (Positive Value = Offset Right relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 5   (Default: press NumPad7)")]
        public float CustomArmyFormation04RangedStartPosLateralOffset { get; set; } = 30f;
        [XmlElement]
        [SettingProperty("Forward Offset - Position - Ranged (II)", -150f, 150f, "Default value is 10f (Positive Value = Offset Forward relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 5   (Default: press NumPad7)")]
        public float CustomArmyFormation04RangedStartPosFwdOffset { get; set; } = 10f;
        [XmlElement]
        [SettingProperty("Unit Arrangement - Cavalry (III)", -1, 7, "-1=NotSet (default), 0=Circle, 1=Column, 2=Line, 3=Loose, 4=Scatter, 5=ShieldWall, 6=Skein, 7=Square")]
        [SettingPropertyGroup("Custom Army Formation 5   (Default: press NumPad7)")]
        public int CustomArmyFormation04CavalryArrangementOrder { get; set; } = -1;
        [XmlElement]
        [SettingProperty("Front Width - Cavalry (III)", -3, 100, "-3=WiderPreset, -2=WidePreset, -1=DeepPreset, 0=NotSet(default), 1-100=Custom Width")]
        [SettingPropertyGroup("Custom Army Formation 5   (Default: press NumPad7)")]
        public int CustomArmyFormation04CavalryFormOrder { get; set; } = 0;
        [XmlElement]
        [SettingProperty("Lateral Offset - Position - Cavalry (III)", -150f, 150f, "Default value is -60f (Positive Value = Offset Right relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 5   (Default: press NumPad7)")]
        public float CustomArmyFormation04CavalryStartPosLateralOffset { get; set; } = -60f;
        [XmlElement]
        [SettingProperty("Forward Offset - Position - Cavalry (III)", -150f, 150f, "Default value is 0f (Positive Value = Offset Forward relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 5   (Default: press NumPad7)")]
        public float CustomArmyFormation04CavalryStartPosFwdOffset { get; set; } = 0f;
        [XmlElement]
        [SettingProperty("Unit Arrangement - HorseArcher (IV)", -1, 7, "-1=NotSet (default), 0=Circle, 1=Column, 2=Line, 3=Loose, 4=Scatter, 5=ShieldWall, 6=Skein, 7=Square")]
        [SettingPropertyGroup("Custom Army Formation 5   (Default: press NumPad7)")]
        public int CustomArmyFormation04HorseArcherArrangementOrder { get; set; } = -1;
        [XmlElement]
        [SettingProperty("Front Width - HorseArcher (IV)", -3, 100, "-3=WiderPreset, -2=WidePreset, -1=DeepPreset, 0=NotSet(default), 1-100=Custom Width")]
        [SettingPropertyGroup("Custom Army Formation 5   (Default: press NumPad7)")]
        public int CustomArmyFormation04HorseArcherFormOrder { get; set; } = 0;
        [XmlElement]
        [SettingProperty("Lateral Offset - Position - HorseArcher (IV)", -150f, 150f, "Default value is 90f (Positive Value = Offset Right relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 5   (Default: press NumPad7)")]
        public float CustomArmyFormation04HorseArcherStartPosLateralOffset { get; set; } = 90f;
        [XmlElement]
        [SettingProperty("Forward Offset - Position - HorseArcher (IV)", -150f, 150f, "Default value is 0f (Positive Value = Offset Forward relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 5   (Default: press NumPad7)")]
        public float CustomArmyFormation04HorseArcherStartPosFwdOffset { get; set; } = 0f;
        [XmlElement]
        [SettingProperty("Unit Arrangement - Skirmisher (V)", -1, 7, "-1=NotSet (default), 0=Circle, 1=Column, 2=Line, 3=Loose, 4=Scatter, 5=ShieldWall, 6=Skein, 7=Square")]
        [SettingPropertyGroup("Custom Army Formation 5   (Default: press NumPad7)")]
        public int CustomArmyFormation04SkirmisherArrangementOrder { get; set; } = -1;
        [XmlElement]
        [SettingProperty("Front Width - Skirmisher (V)", -3, 100, "-3=WiderPreset, -2=WidePreset, -1=DeepPreset, 0=NotSet(default), 1-100=Custom Width")]
        [SettingPropertyGroup("Custom Army Formation 5   (Default: press NumPad7)")]
        public int CustomArmyFormation04SkirmisherFormOrder { get; set; } = 0;
        [XmlElement]
        [SettingProperty("Lateral Offset - Position - Skirmisher (V)", -150f, 150f, "Default value is 30f (Positive Value = Offset Right relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 5   (Default: press NumPad7)")]
        public float CustomArmyFormation04SkirmisherStartPosLateralOffset { get; set; } = 30f;
        [XmlElement]
        [SettingProperty("Forward Offset - Position - Skirmisher (V)", -150f, 150f, "Default value is -10f (Positive Value = Offset Forward relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 5   (Default: press NumPad7)")]
        public float CustomArmyFormation04SkirmisherStartPosFwdOffset { get; set; } = -10f;
        [XmlElement]
        [SettingProperty("Unit Arrangement - HeavyInfantry (VI)", -1, 7, "-1=NotSet (default), 0=Circle, 1=Column, 2=Line, 3=Loose, 4=Scatter, 5=ShieldWall, 6=Skein, 7=Square")]
        [SettingPropertyGroup("Custom Army Formation 5   (Default: press NumPad7)")]
        public int CustomArmyFormation04HeavyInfantryArrangementOrder { get; set; } = -1;
        [XmlElement]
        [SettingProperty("Front Width - HeavyInfantry (VI)", -3, 100, "-3=WiderPreset, -2=WidePreset, -1=DeepPreset, 0=NotSet(default), 1-100=Custom Width")]
        [SettingPropertyGroup("Custom Army Formation 5   (Default: press NumPad7)")]
        public int CustomArmyFormation04HeavyInfantryFormOrder { get; set; } = 0;
        [XmlElement]
        [SettingProperty("Lateral Offset - Position - HeavyInfantry (VI)", -150f, 150f, "Default value is 60f (Positive Value = Offset Right relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 5   (Default: press NumPad7)")]
        public float CustomArmyFormation04HeavyInfantryStartPosLateralOffset { get; set; } = 60f;
        [XmlElement]
        [SettingProperty("Forward Offset - Position - HeavyInfantry (VI)", -150f, 150f, "Default value is 0f (Positive Value = Offset Forward relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 5   (Default: press NumPad7)")]
        public float CustomArmyFormation04HeavyInfantryStartPosFwdOffset { get; set; } = 0f;
        [XmlElement]
        [SettingProperty("Unit Arrangement - LightCavalry (VII)", -1, 7, "-1=NotSet (default), 0=Circle, 1=Column, 2=Line, 3=Loose, 4=Scatter, 5=ShieldWall, 6=Skein, 7=Square")]
        [SettingPropertyGroup("Custom Army Formation 5   (Default: press NumPad7)")]
        public int CustomArmyFormation04LightCavalryArrangementOrder { get; set; } = -1;
        [XmlElement]
        [SettingProperty("Front Width - LightCavalry (VII)", -3, 100, "-3=WiderPreset, -2=WidePreset, -1=DeepPreset, 0=NotSet(default), 1-100=Custom Width")]
        [SettingPropertyGroup("Custom Army Formation 5   (Default: press NumPad7)")]
        public int CustomArmyFormation04LightCavalryFormOrder { get; set; } = 0;
        [XmlElement]
        [SettingProperty("Lateral Offset - Position - LightCavalry (VII)", -150f, 150f, "Default value is 0f (Positive Value = Offset Right relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 5   (Default: press NumPad7)")]
        public float CustomArmyFormation04LightCavalryStartPosLateralOffset { get; set; } = 0f;
        [XmlElement]
        [SettingProperty("Forward Offset - Position - LightCavalry (VII)", -150f, 150f, "Default value is -20f (Positive Value = Offset Forward relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 5   (Default: press NumPad7)")]
        public float CustomArmyFormation04LightCavalryStartPosFwdOffset { get; set; } = -20f;
        [XmlElement]
        [SettingProperty("Unit Arrangement - HeavyCavalry (VIII)", -1, 7, "-1=NotSet (default), 0=Circle, 1=Column, 2=Line, 3=Loose, 4=Scatter, 5=ShieldWall, 6=Skein, 7=Square")]
        [SettingPropertyGroup("Custom Army Formation 5   (Default: press NumPad7)")]
        public int CustomArmyFormation04HeavyCavalryArrangementOrder { get; set; } = -1;
        [XmlElement]
        [SettingProperty("Front Width - HeavyCavalry (VIII)", -3, 100, "-3=WiderPreset, -2=WidePreset, -1=DeepPreset, 0=NotSet(default), 1-100=Custom Width")]
        [SettingPropertyGroup("Custom Army Formation 5   (Default: press NumPad7)")]
        public int CustomArmyFormation04HeavyCavalryFormOrder { get; set; } = 0;
        [XmlElement]
        [SettingProperty("Lateral Offset - Position - HeavyCavalry (VIII)", -150f, 150f, "Default value is 60f (Positive Value = Offset Right relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 5   (Default: press NumPad7)")]
        public float CustomArmyFormation04HeavyCavalryStartPosLateralOffset { get; set; } = 60f;
        [XmlElement]
        [SettingProperty("Forward Offset - Position - HeavyCavalry (VIII)", -150f, 150f, "Default value is -20f (Positive Value = Offset Forward relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 5   (Default: press NumPad7)")]
        public float CustomArmyFormation04HeavyCavalryStartPosFwdOffset { get; set; } = -20f;



        [XmlElement]
        [SettingProperty("Custom Army Formation 6   (Default: press NumPad8)", "'Checker Board' Formation: Front: Ranged(II). Left:Cavalry(III). 1stRow: Left:Skirmisher(V) Center:Infantry(I) Right:HorseArcher(IV). 2ndRow: Left:HeavyInfantry(VI) Right:LightCavalry(VII). 3rdRow: HeavyCavalry(VIII).")]
        [SettingPropertyGroup("Custom Army Formation 6   (Default: press NumPad8)", true)]
        public bool CustomArmyFormation05Enabled { get; set; } = true;
        [XmlElement]
        [SettingProperty("Unit Arrangement - Infantry (I)", -1, 7, "-1=NotSet (default), 0=Circle, 1=Column, 2=Line, 3=Loose, 4=Scatter, 5=ShieldWall, 6=Skein, 7=Square")]
        [SettingPropertyGroup("Custom Army Formation 6   (Default: press NumPad8)")]
        public int CustomArmyFormation05InfantryArrangementOrder { get; set; } = -1;
        [XmlElement]
        [SettingProperty("Front Width - Infantry (I)", -3, 100, "-3=WiderPreset, -2=WidePreset, -1=DeepPreset, 0=NotSet(default), 1-100=Custom Width")]
        [SettingPropertyGroup("Custom Army Formation 6   (Default: press NumPad8)")]
        public int CustomArmyFormation05InfantryFormOrder { get; set; } = 10;
        [XmlElement]
        [SettingProperty("Unit Arrangement - Ranged (II)", -1, 7, "-1=NotSet (default), 0=Circle, 1=Column, 2=Line, 3=Loose, 4=Scatter, 5=ShieldWall, 6=Skein, 7=Square")]
        [SettingPropertyGroup("Custom Army Formation 6   (Default: press NumPad8)")]
        public int CustomArmyFormation05RangedArrangementOrder { get; set; } = 3;
        [XmlElement]
        [SettingProperty("Front Width - Ranged (II)", -3, 100, "-3=WiderPreset, -2=WidePreset, -1=DeepPreset, 0=NotSet(default), 1-100=Custom Width")]
        [SettingPropertyGroup("Custom Army Formation 6   (Default: press NumPad8)")]
        public int CustomArmyFormation05RangedFormOrder { get; set; } = 0;
        [XmlElement]
        [SettingProperty("Lateral Offset - Position - Ranged (II)", -150f, 150f, "Default value is 0f (Positive Value = Offset Right relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 6   (Default: press NumPad8)")]
        public float CustomArmyFormation05RangedStartPosLateralOffset { get; set; } = 0;
        [XmlElement]
        [SettingProperty("Forward Offset - Position - Ranged (II)", -150f, 150f, "Default value is 10f (Positive Value = Offset Forward relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 6   (Default: press NumPad8)")]
        public float CustomArmyFormation05RangedStartPosFwdOffset { get; set; } = 10f;
        [XmlElement]
        [SettingProperty("Unit Arrangement - Cavalry (III)", -1, 7, "-1=NotSet (default), 0=Circle, 1=Column, 2=Line, 3=Loose, 4=Scatter, 5=ShieldWall, 6=Skein, 7=Square")]
        [SettingPropertyGroup("Custom Army Formation 6   (Default: press NumPad8)")]
        public int CustomArmyFormation05CavalryArrangementOrder { get; set; } = -1;
        [XmlElement]
        [SettingProperty("Front Width - Cavalry (III)", -3, 100, "-3=WiderPreset, -2=WidePreset, -1=DeepPreset, 0=NotSet(default), 1-100=Custom Width")]
        [SettingPropertyGroup("Custom Army Formation 6   (Default: press NumPad8)")]
        public int CustomArmyFormation05CavalryFormOrder { get; set; } = 0;
        [XmlElement]
        [SettingProperty("Lateral Offset - Position - Cavalry (III)", -150f, 150f, "Default value is -60f (Positive Value = Offset Right relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 6   (Default: press NumPad8)")]
        public float CustomArmyFormation05CavalryStartPosLateralOffset { get; set; } = -60f;
        [XmlElement]
        [SettingProperty("Forward Offset - Position - Cavalry (III)", -150f, 150f, "Default value is 0f (Positive Value = Offset Forward relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 6   (Default: press NumPad8)")]
        public float CustomArmyFormation05CavalryStartPosFwdOffset { get; set; } = 0f;
        [XmlElement]
        [SettingProperty("Unit Arrangement - HorseArcher (IV)", -1, 7, "-1=NotSet (default), 0=Circle, 1=Column, 2=Line, 3=Loose, 4=Scatter, 5=ShieldWall, 6=Skein, 7=Square")]
        [SettingPropertyGroup("Custom Army Formation 6   (Default: press NumPad8)")]
        public int CustomArmyFormation05HorseArcherArrangementOrder { get; set; } = -1;
        [XmlElement]
        [SettingProperty("Front Width - HorseArcher (IV)", -3, 100, "-3=WiderPreset, -2=WidePreset, -1=DeepPreset, 0=NotSet(default), 1-100=Custom Width")]
        [SettingPropertyGroup("Custom Army Formation 6   (Default: press NumPad8)")]
        public int CustomArmyFormation05HorseArcherFormOrder { get; set; } = 10;
        [XmlElement]
        [SettingProperty("Lateral Offset - Position - HorseArcher (IV)", -150f, 150f, "Default value is 20f (Positive Value = Offset Right relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 6   (Default: press NumPad8)")]
        public float CustomArmyFormation05HorseArcherStartPosLateralOffset { get; set; } = 20f;
        [XmlElement]
        [SettingProperty("Forward Offset - Position - HorseArcher (IV)", -150f, 150f, "Default value is 0f (Positive Value = Offset Forward relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 6   (Default: press NumPad8)")]
        public float CustomArmyFormation05HorseArcherStartPosFwdOffset { get; set; } = 0f;
        [XmlElement]
        [SettingProperty("Unit Arrangement - Skirmisher (V)", -1, 7, "-1=NotSet (default), 0=Circle, 1=Column, 2=Line, 3=Loose, 4=Scatter, 5=ShieldWall, 6=Skein, 7=Square")]
        [SettingPropertyGroup("Custom Army Formation 6   (Default: press NumPad8)")]
        public int CustomArmyFormation05SkirmisherArrangementOrder { get; set; } = -1;
        [XmlElement]
        [SettingProperty("Front Width - Skirmisher (V)", -3, 100, "-3=WiderPreset, -2=WidePreset, -1=DeepPreset, 0=NotSet(default), 1-100=Custom Width")]
        [SettingPropertyGroup("Custom Army Formation 6   (Default: press NumPad8)")]
        public int CustomArmyFormation05SkirmisherFormOrder { get; set; } = 10;
        [XmlElement]
        [SettingProperty("Lateral Offset - Position - Skirmisher (V)", -150f, 150f, "Default value is -20f (Positive Value = Offset Right relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 6   (Default: press NumPad8)")]
        public float CustomArmyFormation05SkirmisherStartPosLateralOffset { get; set; } = -20f;
        [XmlElement]
        [SettingProperty("Forward Offset - Position - Skirmisher (V)", -150f, 150f, "Default value is 0f (Positive Value = Offset Forward relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 6   (Default: press NumPad8)")]
        public float CustomArmyFormation05SkirmisherStartPosFwdOffset { get; set; } = 0;
        [XmlElement]
        [SettingProperty("Unit Arrangement - HeavyInfantry (VI)", -1, 7, "-1=NotSet (default), 0=Circle, 1=Column, 2=Line, 3=Loose, 4=Scatter, 5=ShieldWall, 6=Skein, 7=Square")]
        [SettingPropertyGroup("Custom Army Formation 6   (Default: press NumPad8)")]
        public int CustomArmyFormation05HeavyInfantryArrangementOrder { get; set; } = -1;
        [XmlElement]
        [SettingProperty("Front Width - HeavyInfantry (VI)", -3, 100, "-3=WiderPreset, -2=WidePreset, -1=DeepPreset, 0=NotSet(default), 1-100=Custom Width")]
        [SettingPropertyGroup("Custom Army Formation 6   (Default: press NumPad8)")]
        public int CustomArmyFormation05HeavyInfantryFormOrder { get; set; } = 10;
        [XmlElement]
        [SettingProperty("Lateral Offset - Position - HeavyInfantry (VI)", -150f, 150f, "Default value is -10f (Positive Value = Offset Right relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 6   (Default: press NumPad8)")]
        public float CustomArmyFormation05HeavyInfantryStartPosLateralOffset { get; set; } = -10f;
        [XmlElement]
        [SettingProperty("Forward Offset - Position - HeavyInfantry (VI)", -150f, 150f, "Default value is -15f (Positive Value = Offset Forward relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 6   (Default: press NumPad8)")]
        public float CustomArmyFormation05HeavyInfantryStartPosFwdOffset { get; set; } = -15f;
        [XmlElement]
        [SettingProperty("Unit Arrangement - LightCavalry (VII)", -1, 7, "-1=NotSet (default), 0=Circle, 1=Column, 2=Line, 3=Loose, 4=Scatter, 5=ShieldWall, 6=Skein, 7=Square")]
        [SettingPropertyGroup("Custom Army Formation 6   (Default: press NumPad8)")]
        public int CustomArmyFormation05LightCavalryArrangementOrder { get; set; } = -1;
        [XmlElement]
        [SettingProperty("Front Width - LightCavalry (VII)", -3, 100, "-3=WiderPreset, -2=WidePreset, -1=DeepPreset, 0=NotSet(default), 1-100=Custom Width")]
        [SettingPropertyGroup("Custom Army Formation 6   (Default: press NumPad8)")]
        public int CustomArmyFormation05LightCavalryFormOrder { get; set; } = 10;
        [XmlElement]
        [SettingProperty("Lateral Offset - Position - LightCavalry (VII)", -150f, 150f, "Default value is 10f (Positive Value = Offset Right relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 6   (Default: press NumPad8)")]
        public float CustomArmyFormation05LightCavalryStartPosLateralOffset { get; set; } = 10f;
        [XmlElement]
        [SettingProperty("Forward Offset - Position - LightCavalry (VII)", -150f, 150f, "Default value is -15f (Positive Value = Offset Forward relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 6   (Default: press NumPad8)")]
        public float CustomArmyFormation05LightCavalryStartPosFwdOffset { get; set; } = -15f;
        [XmlElement]
        [SettingProperty("Unit Arrangement - HeavyCavalry (VIII)", -1, 7, "-1=NotSet (default), 0=Circle, 1=Column, 2=Line, 3=Loose, 4=Scatter, 5=ShieldWall, 6=Skein, 7=Square")]
        [SettingPropertyGroup("Custom Army Formation 6   (Default: press NumPad8)")]
        public int CustomArmyFormation05HeavyCavalryArrangementOrder { get; set; } = -1;
        [XmlElement]
        [SettingProperty("Front Width - HeavyCavalry (VIII)", -3, 100, "-3=WiderPreset, -2=WidePreset, -1=DeepPreset, 0=NotSet(default), 1-100=Custom Width")]
        [SettingPropertyGroup("Custom Army Formation 6   (Default: press NumPad8)")]
        public int CustomArmyFormation05HeavyCavalryFormOrder { get; set; } = 40;
        [XmlElement]
        [SettingProperty("Lateral Offset - Position - HeavyCavalry (VIII)", -150f, 150f, "Default value is 0f (Positive Value = Offset Right relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 6   (Default: press NumPad8)")]
        public float CustomArmyFormation05HeavyCavalryStartPosLateralOffset { get; set; } = 0f;
        [XmlElement]
        [SettingProperty("Forward Offset - Position - HeavyCavalry (VIII)", -150f, 150f, "Default value is -30f (Positive Value = Offset Forward relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 6   (Default: press NumPad8)")]
        public float CustomArmyFormation05HeavyCavalryStartPosFwdOffset { get; set; } = -30f;



        [XmlElement]
        [SettingProperty("Custom Army Formation 7   (Default: press NumPad9)", "'Checker Board' Formation: Rear: Ranged(II). Left:Cavalry(III). 1stRow: Left:Skirmisher(V) Center:Infantry(I) Right:HorseArcher(IV). 2ndRow: Left:HeavyInfantry(VI) Right:LightCavalry(VII). 3rdRow: HeavyCavalry(VIII).")]
        [SettingPropertyGroup("Custom Army Formation 7   (Default: press NumPad9)", true)]
        public bool CustomArmyFormation06Enabled { get; set; } = true;
        [XmlElement]
        [SettingProperty("Unit Arrangement - Infantry (I)", -1, 7, "-1=NotSet (default), 0=Circle, 1=Column, 2=Line, 3=Loose, 4=Scatter, 5=ShieldWall, 6=Skein, 7=Square")]
        [SettingPropertyGroup("Custom Army Formation 7   (Default: press NumPad9)")]
        public int CustomArmyFormation06InfantryArrangementOrder { get; set; } = -1;
        [XmlElement]
        [SettingProperty("Front Width - Infantry (I)", -3, 100, "-3=WiderPreset, -2=WidePreset, -1=DeepPreset, 0=NotSet(default), 1-100=Custom Width")]
        [SettingPropertyGroup("Custom Army Formation 7   (Default: press NumPad9)")]
        public int CustomArmyFormation06InfantryFormOrder { get; set; } = 10;
        [XmlElement]
        [SettingProperty("Unit Arrangement - Ranged (II)", -1, 7, "-1=NotSet (default), 0=Circle, 1=Column, 2=Line, 3=Loose, 4=Scatter, 5=ShieldWall, 6=Skein, 7=Square")]
        [SettingPropertyGroup("Custom Army Formation 7   (Default: press NumPad9)")]
        public int CustomArmyFormation06RangedArrangementOrder { get; set; } = 3;
        [XmlElement]
        [SettingProperty("Front Width - Ranged (II)", -3, 100, "-3=WiderPreset, -2=WidePreset, -1=DeepPreset, 0=NotSet(default), 1-100=Custom Width")]
        [SettingPropertyGroup("Custom Army Formation 7   (Default: press NumPad9)")]
        public int CustomArmyFormation06RangedFormOrder { get; set; } = 0;
        [XmlElement]
        [SettingProperty("Lateral Offset - Position - Ranged (II)", -150f, 150f, "Default value is 0f (Positive Value = Offset Right relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 7   (Default: press NumPad9)")]
        public float CustomArmyFormation06RangedStartPosLateralOffset { get; set; } = 0;
        [XmlElement]
        [SettingProperty("Forward Offset - Position - Ranged (II)", -150f, 150f, "Default value is -35f (Positive Value = Offset Forward relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 7   (Default: press NumPad9)")]
        public float CustomArmyFormation06RangedStartPosFwdOffset { get; set; } = -35f;
        [XmlElement]
        [SettingProperty("Unit Arrangement - Cavalry (III)", -1, 7, "-1=NotSet (default), 0=Circle, 1=Column, 2=Line, 3=Loose, 4=Scatter, 5=ShieldWall, 6=Skein, 7=Square")]
        [SettingPropertyGroup("Custom Army Formation 7   (Default: press NumPad9)")]
        public int CustomArmyFormation06CavalryArrangementOrder { get; set; } = -1;
        [XmlElement]
        [SettingProperty("Front Width - Cavalry (III)", -3, 100, "-3=WiderPreset, -2=WidePreset, -1=DeepPreset, 0=NotSet(default), 1-100=Custom Width")]
        [SettingPropertyGroup("Custom Army Formation 7   (Default: press NumPad9)")]
        public int CustomArmyFormation06CavalryFormOrder { get; set; } = 0;
        [XmlElement]
        [SettingProperty("Lateral Offset - Position - Cavalry (III)", -150f, 150f, "Default value is -60f (Positive Value = Offset Right relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 7   (Default: press NumPad9)")]
        public float CustomArmyFormation06CavalryStartPosLateralOffset { get; set; } = -60f;
        [XmlElement]
        [SettingProperty("Forward Offset - Position - Cavalry (III)", -150f, 150f, "Default value is 0f (Positive Value = Offset Forward relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 7   (Default: press NumPad9)")]
        public float CustomArmyFormation06CavalryStartPosFwdOffset { get; set; } = 0f;
        [XmlElement]
        [SettingProperty("Unit Arrangement - HorseArcher (IV)", -1, 7, "-1=NotSet (default), 0=Circle, 1=Column, 2=Line, 3=Loose, 4=Scatter, 5=ShieldWall, 6=Skein, 7=Square")]
        [SettingPropertyGroup("Custom Army Formation 7   (Default: press NumPad9)")]
        public int CustomArmyFormation06HorseArcherArrangementOrder { get; set; } = -1;
        [XmlElement]
        [SettingProperty("Front Width - HorseArcher (IV)", -3, 100, "-3=WiderPreset, -2=WidePreset, -1=DeepPreset, 0=NotSet(default), 1-100=Custom Width")]
        [SettingPropertyGroup("Custom Army Formation 7   (Default: press NumPad9)")]
        public int CustomArmyFormation06HorseArcherFormOrder { get; set; } = 10;
        [XmlElement]
        [SettingProperty("Lateral Offset - Position - HorseArcher (IV)", -150f, 150f, "Default value is 20f (Positive Value = Offset Right relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 7   (Default: press NumPad9)")]
        public float CustomArmyFormation06HorseArcherStartPosLateralOffset { get; set; } = 20f;
        [XmlElement]
        [SettingProperty("Forward Offset - Position - HorseArcher (IV)", -150f, 150f, "Default value is 0f (Positive Value = Offset Forward relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 7   (Default: press NumPad9)")]
        public float CustomArmyFormation06HorseArcherStartPosFwdOffset { get; set; } = 0f;
        [XmlElement]
        [SettingProperty("Unit Arrangement - Skirmisher (V)", -1, 7, "-1=NotSet (default), 0=Circle, 1=Column, 2=Line, 3=Loose, 4=Scatter, 5=ShieldWall, 6=Skein, 7=Square")]
        [SettingPropertyGroup("Custom Army Formation 7   (Default: press NumPad9)")]
        public int CustomArmyFormation06SkirmisherArrangementOrder { get; set; } = -1;
        [XmlElement]
        [SettingProperty("Front Width - Skirmisher (V)", -3, 100, "-3=WiderPreset, -2=WidePreset, -1=DeepPreset, 0=NotSet(default), 1-100=Custom Width")]
        [SettingPropertyGroup("Custom Army Formation 7   (Default: press NumPad9)")]
        public int CustomArmyFormation06SkirmisherFormOrder { get; set; } = 10;
        [XmlElement]
        [SettingProperty("Lateral Offset - Position - Skirmisher (V)", -150f, 150f, "Default value is -20f (Positive Value = Offset Right relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 7   (Default: press NumPad9)")]
        public float CustomArmyFormation06SkirmisherStartPosLateralOffset { get; set; } = -20f;
        [XmlElement]
        [SettingProperty("Forward Offset - Position - Skirmisher (V)", -150f, 150f, "Default value is 0f (Positive Value = Offset Forward relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 7   (Default: press NumPad9)")]
        public float CustomArmyFormation06SkirmisherStartPosFwdOffset { get; set; } = 0;
        [XmlElement]
        [SettingProperty("Unit Arrangement - HeavyInfantry (VI)", -1, 7, "-1=NotSet (default), 0=Circle, 1=Column, 2=Line, 3=Loose, 4=Scatter, 5=ShieldWall, 6=Skein, 7=Square")]
        [SettingPropertyGroup("Custom Army Formation 7   (Default: press NumPad9)")]
        public int CustomArmyFormation06HeavyInfantryArrangementOrder { get; set; } = -1;
        [XmlElement]
        [SettingProperty("Front Width - HeavyInfantry (VI)", -3, 100, "-3=WiderPreset, -2=WidePreset, -1=DeepPreset, 0=NotSet(default), 1-100=Custom Width")]
        [SettingPropertyGroup("Custom Army Formation 7   (Default: press NumPad9)")]
        public int CustomArmyFormation06HeavyInfantryFormOrder { get; set; } = 10;
        [XmlElement]
        [SettingProperty("Lateral Offset - Position - HeavyInfantry (VI)", -150f, 150f, "Default value is -10f (Positive Value = Offset Right relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 7   (Default: press NumPad9)")]
        public float CustomArmyFormation06HeavyInfantryStartPosLateralOffset { get; set; } = -10f;
        [XmlElement]
        [SettingProperty("Forward Offset - Position - HeavyInfantry (VI)", -150f, 150f, "Default value is -15f (Positive Value = Offset Forward relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 7   (Default: press NumPad9)")]
        public float CustomArmyFormation06HeavyInfantryStartPosFwdOffset { get; set; } = -15f;
        [XmlElement]
        [SettingProperty("Unit Arrangement - LightCavalry (VII)", -1, 7, "-1=NotSet (default), 0=Circle, 1=Column, 2=Line, 3=Loose, 4=Scatter, 5=ShieldWall, 6=Skein, 7=Square")]
        [SettingPropertyGroup("Custom Army Formation 7   (Default: press NumPad9)")]
        public int CustomArmyFormation06LightCavalryArrangementOrder { get; set; } = -1;
        [XmlElement]
        [SettingProperty("Front Width - LightCavalry (VII)", -3, 100, "-3=WiderPreset, -2=WidePreset, -1=DeepPreset, 0=NotSet(default), 1-100=Custom Width")]
        [SettingPropertyGroup("Custom Army Formation 7   (Default: press NumPad9)")]
        public int CustomArmyFormation06LightCavalryFormOrder { get; set; } = 10;
        [XmlElement]
        [SettingProperty("Lateral Offset - Position - LightCavalry (VII)", -150f, 150f, "Default value is 10f (Positive Value = Offset Right relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 7   (Default: press NumPad9)")]
        public float CustomArmyFormation06LightCavalryStartPosLateralOffset { get; set; } = 10f;
        [XmlElement]
        [SettingProperty("Forward Offset - Position - LightCavalry (VII)", -150f, 150f, "Default value is -15f (Positive Value = Offset Forward relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 7   (Default: press NumPad9)")]
        public float CustomArmyFormation06LightCavalryStartPosFwdOffset { get; set; } = -15f;
        [XmlElement]
        [SettingProperty("Unit Arrangement - HeavyCavalry (VIII)", -1, 7, "-1=NotSet (default), 0=Circle, 1=Column, 2=Line, 3=Loose, 4=Scatter, 5=ShieldWall, 6=Skein, 7=Square")]
        [SettingPropertyGroup("Custom Army Formation 7   (Default: press NumPad9)")]
        public int CustomArmyFormation06HeavyCavalryArrangementOrder { get; set; } = -1;
        [XmlElement]
        [SettingProperty("Front Width - HeavyCavalry (VIII)", -3, 100, "-3=WiderPreset, -2=WidePreset, -1=DeepPreset, 0=NotSet(default), 1-100=Custom Width")]
        [SettingPropertyGroup("Custom Army Formation 7   (Default: press NumPad9)")]
        public int CustomArmyFormation06HeavyCavalryFormOrder { get; set; } = 40;
        [XmlElement]
        [SettingProperty("Lateral Offset - Position - HeavyCavalry (VIII)", -150f, 150f, "Default value is 0f (Positive Value = Offset Right relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 7   (Default: press NumPad9)")]
        public float CustomArmyFormation06HeavyCavalryStartPosLateralOffset { get; set; } = 0f;
        [XmlElement]
        [SettingProperty("Forward Offset - Position - HeavyCavalry (VIII)", -150f, 150f, "Default value is -25f (Positive Value = Offset Forward relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 7   (Default: press NumPad9)")]
        public float CustomArmyFormation06HeavyCavalryStartPosFwdOffset { get; set; } = -25f;



        [XmlElement]
        [SettingProperty("Custom Army Formation 8   (Default: press NumPad0)", "'Checker Board (2ndRow-Advance)' Formation: Rear: Ranged(II). Left:Cavalry(III). 1stRow: Left:Skirmisher(V) CenterLeft:HeavyInfantry(VI) Center:Infantry(I) CenterRight:LightCavalry(VII) Right:HorseArcher(IV). 3rdRow: HeavyCavalry(VIII).")]
        [SettingPropertyGroup("Custom Army Formation 8   (Default: press NumPad0)", true)]
        public bool CustomArmyFormation07Enabled { get; set; } = true;
        [XmlElement]
        [SettingProperty("Unit Arrangement - Infantry (I)", -1, 7, "-1=NotSet (default), 0=Circle, 1=Column, 2=Line, 3=Loose, 4=Scatter, 5=ShieldWall, 6=Skein, 7=Square")]
        [SettingPropertyGroup("Custom Army Formation 8   (Default: press NumPad0)")]
        public int CustomArmyFormation07InfantryArrangementOrder { get; set; } = -1;
        [XmlElement]
        [SettingProperty("Front Width - Infantry (I)", -3, 100, "-3=WiderPreset, -2=WidePreset, -1=DeepPreset, 0=NotSet(default), 1-100=Custom Width")]
        [SettingPropertyGroup("Custom Army Formation 8   (Default: press NumPad0)")]
        public int CustomArmyFormation07InfantryFormOrder { get; set; } = 10;
        [XmlElement]
        [SettingProperty("Unit Arrangement - Ranged (II)", -1, 7, "-1=NotSet (default), 0=Circle, 1=Column, 2=Line, 3=Loose, 4=Scatter, 5=ShieldWall, 6=Skein, 7=Square")]
        [SettingPropertyGroup("Custom Army Formation 8   (Default: press NumPad0)")]
        public int CustomArmyFormation07RangedArrangementOrder { get; set; } = 3;
        [XmlElement]
        [SettingProperty("Front Width - Ranged (II)", -3, 100, "-3=WiderPreset, -2=WidePreset, -1=DeepPreset, 0=NotSet(default), 1-100=Custom Width")]
        [SettingPropertyGroup("Custom Army Formation 8   (Default: press NumPad0)")]
        public int CustomArmyFormation07RangedFormOrder { get; set; } = 0;
        [XmlElement]
        [SettingProperty("Lateral Offset - Position - Ranged (II)", -150f, 150f, "Default value is 0f (Positive Value = Offset Right relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 8   (Default: press NumPad0)")]
        public float CustomArmyFormation07RangedStartPosLateralOffset { get; set; } = 0;
        [XmlElement]
        [SettingProperty("Forward Offset - Position - Ranged (II)", -150f, 150f, "Default value is -35f (Positive Value = Offset Forward relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 8   (Default: press NumPad0)")]
        public float CustomArmyFormation07RangedStartPosFwdOffset { get; set; } = -27.5f;
        [XmlElement]
        [SettingProperty("Unit Arrangement - Cavalry (III)", -1, 7, "-1=NotSet (default), 0=Circle, 1=Column, 2=Line, 3=Loose, 4=Scatter, 5=ShieldWall, 6=Skein, 7=Square")]
        [SettingPropertyGroup("Custom Army Formation 8   (Default: press NumPad0)")]
        public int CustomArmyFormation07CavalryArrangementOrder { get; set; } = -1;
        [XmlElement]
        [SettingProperty("Front Width - Cavalry (III)", -3, 100, "-3=WiderPreset, -2=WidePreset, -1=DeepPreset, 0=NotSet(default), 1-100=Custom Width")]
        [SettingPropertyGroup("Custom Army Formation 8   (Default: press NumPad0)")]
        public int CustomArmyFormation07CavalryFormOrder { get; set; } = 0;
        [XmlElement]
        [SettingProperty("Lateral Offset - Position - Cavalry (III)", -150f, 150f, "Default value is -60f (Positive Value = Offset Right relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 8   (Default: press NumPad0)")]
        public float CustomArmyFormation07CavalryStartPosLateralOffset { get; set; } = -60f;
        [XmlElement]
        [SettingProperty("Forward Offset - Position - Cavalry (III)", -150f, 150f, "Default value is 0f (Positive Value = Offset Forward relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 8   (Default: press NumPad0)")]
        public float CustomArmyFormation07CavalryStartPosFwdOffset { get; set; } = 0f;
        [XmlElement]
        [SettingProperty("Unit Arrangement - HorseArcher (IV)", -1, 7, "-1=NotSet (default), 0=Circle, 1=Column, 2=Line, 3=Loose, 4=Scatter, 5=ShieldWall, 6=Skein, 7=Square")]
        [SettingPropertyGroup("Custom Army Formation 8   (Default: press NumPad0)")]
        public int CustomArmyFormation07HorseArcherArrangementOrder { get; set; } = -1;
        [XmlElement]
        [SettingProperty("Front Width - HorseArcher (IV)", -3, 100, "-3=WiderPreset, -2=WidePreset, -1=DeepPreset, 0=NotSet(default), 1-100=Custom Width")]
        [SettingPropertyGroup("Custom Army Formation 8   (Default: press NumPad0)")]
        public int CustomArmyFormation07HorseArcherFormOrder { get; set; } = 10;
        [XmlElement]
        [SettingProperty("Lateral Offset - Position - HorseArcher (IV)", -150f, 150f, "Default value is 20f (Positive Value = Offset Right relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 8   (Default: press NumPad0)")]
        public float CustomArmyFormation07HorseArcherStartPosLateralOffset { get; set; } = 20f;
        [XmlElement]
        [SettingProperty("Forward Offset - Position - HorseArcher (IV)", -150f, 150f, "Default value is 0f (Positive Value = Offset Forward relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 8   (Default: press NumPad0)")]
        public float CustomArmyFormation07HorseArcherStartPosFwdOffset { get; set; } = 0f;
        [XmlElement]
        [SettingProperty("Unit Arrangement - Skirmisher (V)", -1, 7, "-1=NotSet (default), 0=Circle, 1=Column, 2=Line, 3=Loose, 4=Scatter, 5=ShieldWall, 6=Skein, 7=Square")]
        [SettingPropertyGroup("Custom Army Formation 8   (Default: press NumPad0)")]
        public int CustomArmyFormation07SkirmisherArrangementOrder { get; set; } = -1;
        [XmlElement]
        [SettingProperty("Front Width - Skirmisher (V)", -3, 100, "-3=WiderPreset, -2=WidePreset, -1=DeepPreset, 0=NotSet(default), 1-100=Custom Width")]
        [SettingPropertyGroup("Custom Army Formation 8   (Default: press NumPad0)")]
        public int CustomArmyFormation07SkirmisherFormOrder { get; set; } = 10;
        [XmlElement]
        [SettingProperty("Lateral Offset - Position - Skirmisher (V)", -150f, 150f, "Default value is -20f (Positive Value = Offset Right relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 8   (Default: press NumPad0)")]
        public float CustomArmyFormation07SkirmisherStartPosLateralOffset { get; set; } = -20f;
        [XmlElement]
        [SettingProperty("Forward Offset - Position - Skirmisher (V)", -150f, 150f, "Default value is 0f (Positive Value = Offset Forward relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 8   (Default: press NumPad0)")]
        public float CustomArmyFormation07SkirmisherStartPosFwdOffset { get; set; } = 0;
        [XmlElement]
        [SettingProperty("Unit Arrangement - HeavyInfantry (VI)", -1, 7, "-1=NotSet (default), 0=Circle, 1=Column, 2=Line, 3=Loose, 4=Scatter, 5=ShieldWall, 6=Skein, 7=Square")]
        [SettingPropertyGroup("Custom Army Formation 8   (Default: press NumPad0)")]
        public int CustomArmyFormation07HeavyInfantryArrangementOrder { get; set; } = -1;
        [XmlElement]
        [SettingProperty("Front Width - HeavyInfantry (VI)", -3, 100, "-3=WiderPreset, -2=WidePreset, -1=DeepPreset, 0=NotSet(default), 1-100=Custom Width")]
        [SettingPropertyGroup("Custom Army Formation 8   (Default: press NumPad0)")]
        public int CustomArmyFormation07HeavyInfantryFormOrder { get; set; } = 10;
        [XmlElement]
        [SettingProperty("Lateral Offset - Position - HeavyInfantry (VI)", -150f, 150f, "Default value is -10f (Positive Value = Offset Right relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 8   (Default: press NumPad0)")]
        public float CustomArmyFormation07HeavyInfantryStartPosLateralOffset { get; set; } = -10f;
        [XmlElement]
        [SettingProperty("Forward Offset - Position - HeavyInfantry (VI)", -150f, 150f, "Default value is 0f (Positive Value = Offset Forward relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 8   (Default: press NumPad0)")]
        public float CustomArmyFormation07HeavyInfantryStartPosFwdOffset { get; set; } = 0f;
        [XmlElement]
        [SettingProperty("Unit Arrangement - LightCavalry (VII)", -1, 7, "-1=NotSet (default), 0=Circle, 1=Column, 2=Line, 3=Loose, 4=Scatter, 5=ShieldWall, 6=Skein, 7=Square")]
        [SettingPropertyGroup("Custom Army Formation 8   (Default: press NumPad0)")]
        public int CustomArmyFormation07LightCavalryArrangementOrder { get; set; } = -1;
        [XmlElement]
        [SettingProperty("Front Width - LightCavalry (VII)", -3, 100, "-3=WiderPreset, -2=WidePreset, -1=DeepPreset, 0=NotSet(default), 1-100=Custom Width")]
        [SettingPropertyGroup("Custom Army Formation 8   (Default: press NumPad0)")]
        public int CustomArmyFormation07LightCavalryFormOrder { get; set; } = 10;
        [XmlElement]
        [SettingProperty("Lateral Offset - Position - LightCavalry (VII)", -150f, 150f, "Default value is 10f (Positive Value = Offset Right relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 8   (Default: press NumPad0)")]
        public float CustomArmyFormation07LightCavalryStartPosLateralOffset { get; set; } = 10f;
        [XmlElement]
        [SettingProperty("Forward Offset - Position - LightCavalry (VII)", -150f, 150f, "Default value is 0f (Positive Value = Offset Forward relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 8   (Default: press NumPad0)")]
        public float CustomArmyFormation07LightCavalryStartPosFwdOffset { get; set; } = 0f;
        [XmlElement]
        [SettingProperty("Unit Arrangement - HeavyCavalry (VIII)", -1, 7, "-1=NotSet (default), 0=Circle, 1=Column, 2=Line, 3=Loose, 4=Scatter, 5=ShieldWall, 6=Skein, 7=Square")]
        [SettingPropertyGroup("Custom Army Formation 8   (Default: press NumPad0)")]
        public int CustomArmyFormation07HeavyCavalryArrangementOrder { get; set; } = -1;
        [XmlElement]
        [SettingProperty("Front Width - HeavyCavalry (VIII)", -3, 100, "-3=WiderPreset, -2=WidePreset, -1=DeepPreset, 0=NotSet(default), 1-100=Custom Width")]
        [SettingPropertyGroup("Custom Army Formation 8   (Default: press NumPad0)")]
        public int CustomArmyFormation07HeavyCavalryFormOrder { get; set; } = 40;
        [XmlElement]
        [SettingProperty("Lateral Offset - Position - HeavyCavalry (VIII)", -150f, 150f, "Default value is 0f (Positive Value = Offset Right relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 8   (Default: press NumPad0)")]
        public float CustomArmyFormation07HeavyCavalryStartPosLateralOffset { get; set; } = 0f;
        [XmlElement]
        [SettingProperty("Forward Offset - Position - HeavyCavalry (VIII)", -150f, 150f, "Default value is -15f (Positive Value = Offset Forward relative to Infantry (I)).")]
        [SettingPropertyGroup("Custom Army Formation 8   (Default: press NumPad0)")]
        public float CustomArmyFormation07HeavyCavalryStartPosFwdOffset { get; set; } = -15f;

        #endregion
        #endregion

    }
}
