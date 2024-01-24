using _School_Seducer_.Editor.Scripts.Utility;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class StateAnimationControllerWindow : EditorWindow
    {
        private SerializedObject _serializedObject;
        private SerializedProperty _animationsProperty;
        private StateAnimationController _data;
        
        [MenuItem("Services/Animation Controller Window")]
        public static void ShowWindow(StateAnimationController data)
        {
            StateAnimationControllerWindow window = CreateInstance<StateAnimationControllerWindow>();
            window._data = data;
            window.ShowModalUtility();
        }

        private void OnGUI()
        {
            if (_data != null)
            {
                EditorGUILayout.LabelField("Animations Editor");

                _serializedObject = new SerializedObject(_data);

                _serializedObject.Update();

                var animationsProp = _serializedObject.FindProperty("animations");
            
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

                    EditorGUILayout.Space();
                }

                _serializedObject.ApplyModifiedProperties();
                
                if (GUI.changed)
                {
                    EditorUtility.SetDirty(_data);
                }
            }
        }
    }
}