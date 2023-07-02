using UnityEngine;
using UnityEditor;

namespace KrakenBrain.Editor
{
    public class KrakenBrainSettingsEditor : UnityEditor.Editor
    {
        SerializedProperty _sdWeightName;

        [MenuItem("Assets/KrakenBrain Setting/Settings...")]
        public static void OpenInspector()
        {
            Selection.activeObject = KrakenBrainSettings.LoadInstance();
        }

        public void OnEnable()
        {
            _sdWeightName = serializedObject.FindProperty("sdWeightFileName");
        }

        public override void OnInspectorGUI()
        {
            // Make sure the Settings object has all recent changes.

            var settings = (KrakenBrainSettings)target;

            if (null == settings)
            {
                UnityEngine.Debug.LogError("KrakenBrainSettings is null");
                return;
            }

            EditorGUILayout.LabelField("DeepLearning Weight File", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(_sdWeightName, new GUIContent("StableDiffusion - CoreML"));
            EditorGUILayout.HelpBox("Input the Weight File in Assets/krakenbrain/Plugins/iOS", MessageType.Info);
            EditorGUI.indentLevel--;

            serializedObject.ApplyModifiedProperties();
        }
    }
}
