using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace DS.Windows
{
    public class DSEditorWindow : EditorWindow
    {

        [MenuItem("Window/Dialogue Editor")]
        public static void OpenWindow()
        {
            var window = GetWindow<DSEditorWindow>();
            window.titleContent = new GUIContent("Dialogue Editor");
        }

        private void OnEnable()
        {
            AddGraphView();
            AddStyles();
        }

        private void AddGraphView()
        {
            DSGraphView graphView = new DSGraphView();
            graphView.StretchToParentSize();
            rootVisualElement.Add(graphView);
        }
        private void AddStyles()
        {
            StyleSheet styleSheet = (StyleSheet)EditorGUIUtility.Load("DialogueSystem/DialogueVariables.uss");
            rootVisualElement.styleSheets.Add(styleSheet);
        }
    }
}