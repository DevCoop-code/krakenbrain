using System.IO;
using UnityEditor;
using UnityEngine;

namespace KrakenBrain.Editor
{
    internal class KrakenBrainSettings : ScriptableObject
    {
        private const string KrakenBrainSettingsResDir = "Assets/krakenbrain/Resources";

        private const string KrakenBrainSettingsFile = "KrakenBrainSettings";

        private const string KrakenBrainSettingsFileExtension = ".asset";

        internal static KrakenBrainSettings LoadInstance()
        {
            // Read from resources
            var instance = Resources.Load<KrakenBrainSettings>(KrakenBrainSettingsFile);

            // Create instance if null
            if (null == instance)
            {
                Directory.CreateDirectory(KrakenBrainSettingsResDir);
                instance = ScriptableObject.CreateInstance<KrakenBrainSettings>();
                string assetPath = Path.Combine(KrakenBrainSettingsResDir, KrakenBrainSettingsFile + KrakenBrainSettingsFileExtension);
                AssetDatabase.CreateAsset(instance, assetPath);
                AssetDatabase.SaveAssets();
            }

            return instance;
        }

        [SerializeField]
        private string sdWeightFileName = string.Empty;

        public string SDWeightFileName
        {
            get { return sdWeightFileName; }
            set { sdWeightFileName = value; }
        }
    }
}
