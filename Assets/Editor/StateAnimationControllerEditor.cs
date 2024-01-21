using _School_Seducer_.Editor.Scripts.Utility;
using UnityEditor;

namespace Editor
{
    [CustomEditor(typeof(StateAnimationController))]
    public class StateAnimationControllerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var animationsProp = serializedObject.FindProperty("animations");
            var startAnimateTypeProp = serializedObject.FindProperty("startAnimateType");

            EditorGUILayout.PropertyField(startAnimateTypeProp);

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
                        var animationMoveProp = animationProp.FindPropertyRelative("animationMove");
                        EditorGUILayout.PropertyField(animationMoveProp);
                        break;

                    case AnimationType.Rotate:
                        var animationRotateProp = animationProp.FindPropertyRelative("animationRotate");
                        EditorGUILayout.PropertyField(animationRotateProp);
                        break;
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}