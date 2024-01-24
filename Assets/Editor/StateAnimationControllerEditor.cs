using _School_Seducer_.Editor.Scripts.Utility;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(StateAnimationController))]
    public class StateAnimationControllerEditor : UnityEditor.Editor
    {
        [MenuItem("Window/Animation Controller Window")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow<StateAnimationControllerWindow>("Animation Controller Window");
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var animationsProp = serializedObject.FindProperty("animations");
            
            EditorGUILayout.PropertyField(animationsProp);

            for (int i = 0; i < animationsProp.arraySize; i++)
            {
                var animationProp = animationsProp.GetArrayElementAtIndex(i);
                
                var typeProp = animationProp.FindPropertyRelative("type");
                var gameObjectProp = animationProp.FindPropertyRelative("gameObject");

                EditorGUILayout.PropertyField(typeProp);
                EditorGUILayout.PropertyField(gameObjectProp);

                var animationType = (AnimationType)typeProp.enumValueIndex;

                switch (animationType)
                {
                    case AnimationType.Move:
                        var animationMoveProp = animationProp.FindPropertyRelative("animationPosition");
                        EditorGUILayout.PropertyField(animationMoveProp);
                        break;

                    case AnimationType.Rotate:
                        var animationRotateProp = animationProp.FindPropertyRelative("animationRotate");
                        EditorGUILayout.PropertyField(animationRotateProp);
                        break;
                }

                EditorGUILayout.Space(10);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.EndHorizontal();
            }
            
            if (GUILayout.Button("Open Animation Controller Window"))
            {
                StateAnimationControllerWindow.ShowWindow((StateAnimationController)target);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}