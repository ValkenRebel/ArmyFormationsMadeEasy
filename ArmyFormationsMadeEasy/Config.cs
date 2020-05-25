using ArmyFormationsMadeEasy.Utilities;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;

namespace ArmyFormationsMadeEasy
{
    [XmlRoot("ArmyFormationsMadeEasyConfig")]
    public class Config
    {
        public static ModuleInfo ModInfo => Instance.InstanceModInfo;

        [XmlElement("IncreaseUnitSpeedLimitKey")]
        public int InstanceIncreaseUnitSpeedLimitKey { get; set; } = (int)InputKey.Equals;
        public static InputKey ResetUnitSpeedLimitKey => (InputKey)Instance.InstanceIncreaseUnitSpeedLimitKey;
        [XmlElement("ReduceUnitSpeedLimitKey")]
        public int InstanceReduceUnitSpeedLimitKey { get; set; } = (int)InputKey.Minus;
        public static InputKey ReduceUnitSpeedLimitKey => (InputKey)Instance.InstanceReduceUnitSpeedLimitKey;

        [XmlElement("AdvanceSelectedKey")]
        public int InstanceAdvanceSelectedKey { get; set; } = (int)InputKey.F9;
        public static InputKey AdvanceSelectedKey => (InputKey)Instance.InstanceAdvanceSelectedKey;
        [XmlElement("FallbackSelectedKey")]
        public int InstanceFallbackSelectedKey { get; set; } = (int)InputKey.F10;
        public static InputKey FallbackSelectedKey => (InputKey)Instance.InstanceFallbackSelectedKey;

        [XmlElement("CustomFormation1Key")]
        public int InstanceCustomFormation1Key { get; set; } = (int)InputKey.F11;
        public static InputKey CustomFormation1Key => (InputKey)Instance.InstanceCustomFormation1Key;
        [XmlElement("CustomFormation2Key")]
        public int InstanceCustomFormation2Key { get; set; } = (int)InputKey.F12;
        public static InputKey CustomFormation2Key => (InputKey)Instance.InstanceCustomFormation2Key;
        [XmlElement("CustomFormation3Key")]
        public int InstanceCustomFormation3Key { get; set; } = (int)InputKey.Numpad5;
        public static InputKey CustomFormation3Key => (InputKey)Instance.InstanceCustomFormation3Key;
        [XmlElement("CustomFormation4Key")]
        public int InstanceCustomFormation4Key { get; set; } = (int)InputKey.Numpad6;
        public static InputKey CustomFormation4Key => (InputKey)Instance.InstanceCustomFormation4Key;
        [XmlElement("CustomFormation5Key")]
        public int InstanceCustomFormation5Key { get; set; } = (int)InputKey.Numpad7;
        public static InputKey CustomFormation5Key => (InputKey)Instance.InstanceCustomFormation5Key;
        [XmlElement("CustomFormation6Key")]
        public int InstanceCustomFormation6Key { get; set; } = (int)InputKey.Numpad8;
        public static InputKey CustomFormation6Key => (InputKey)Instance.InstanceCustomFormation6Key;
        [XmlElement("CustomFormation7Key")]
        public int InstanceCustomFormation7Key { get; set; } = (int)InputKey.Numpad9;
        public static InputKey CustomFormation7Key => (InputKey)Instance.InstanceCustomFormation7Key;
        [XmlElement("CustomFormation8Key")]
        public int InstanceCustomFormation8Key { get; set; } = (int)InputKey.Numpad0;
        public static InputKey CustomFormation8Key => (InputKey)Instance.InstanceCustomFormation8Key;

        private static readonly string ModBasePath =
            Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "..", ".."));
        private static readonly string ConfigFilename =
            Path.Combine(ModBasePath, "ModuleData", "ArmyFormationsMadeEasyConfig.xml");

        private static Config _instance;
        private static Config Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }

                _instance = LoadConfig();
                return _instance;
            }
        }

        private ModuleInfo InstanceModInfo { get; } = new ModuleInfo();

        private Config() { }

        private static Config LoadConfig()
        {
            Config config = DeserializeConfig();
            if (config == null)
            {
                SerializeConfig();
                config = DeserializeConfig();
            }

            config.InstanceModInfo.Load(new DirectoryInfo(ModBasePath).Name);
            return config;
        }

        private static void SerializeConfig()
        {
            XmlSerialization.Serialize(ConfigFilename, new Config());
        }

        private static Config DeserializeConfig()
        {
            return XmlSerialization.Deserialize<Config>(ConfigFilename);
        }
    }
}
