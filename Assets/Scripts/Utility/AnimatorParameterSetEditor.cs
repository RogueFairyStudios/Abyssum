# if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace DEEP.Utility
{

    // Custom editor for AnimatorParameterSet, used to only show the relevant 
    // variables for the selected parameter type.
    [CustomEditor(typeof(AnimatorParameterSet))]
    [CanEditMultipleObjects]
    public class AnimatorParameterSetEditor : Editor 
    {

        SerializedProperty parameterName;
        SerializedProperty parameterType;
        SerializedProperty parameterValue;
        
        void OnEnable()
        {

            // Gets parameter name and type.
            parameterName = serializedObject.FindProperty("parameterName");
            parameterType = serializedObject.FindProperty("parameterType");

        }

        public override void OnInspectorGUI()
        {
            // Checks the parameter type and only gets the relevant value.
            if(parameterType.enumValueIndex == 0)
                parameterValue = serializedObject.FindProperty("parameterIntValue");
            else if(parameterType.enumValueIndex == 1)
                parameterValue = serializedObject.FindProperty("parameterFloatValue");
            else
                parameterValue = serializedObject.FindProperty("parameterBooleanValue");

            serializedObject.Update();
            EditorGUILayout.PropertyField(parameterName);
            EditorGUILayout.PropertyField(parameterType);
            EditorGUILayout.PropertyField(parameterValue);
            serializedObject.ApplyModifiedProperties();
        }
    }

}

# endif