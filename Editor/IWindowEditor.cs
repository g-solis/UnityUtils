using UnityEngine;
using UnityEditor;

namespace Utils
{
    [CustomEditor(typeof(IWindow))]
    public class ObjectBuilderEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            IWindow window = (IWindow)target;

            GUI.enabled = Application.isPlaying;

            if(GUILayout.Button("Open Window"))
                window.Open();

            if(GUILayout.Button("Close Window"))
                window.Close();

            if(GUILayout.Button("Click Modal"))
                window.ModalClick();

            GUI.enabled = true;

            DrawDefaultInspector();
        }
    }
}