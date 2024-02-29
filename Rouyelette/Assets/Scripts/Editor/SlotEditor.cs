using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Graphs;

#if UNITY_EDITOR

//[CustomEditor(typeof(Slot))]
//public class SlotEditor : Editor
//{
//    SerializedProperty method;
//    SerializedProperty splitMethod;

//    public override void OnInspectorGUI()
//    {
//        DrawDefaultInspector();

//        Slot slot = (Slot)target;

//        method = serializedObject.FindProperty("SlotMethod");
//        splitMethod = serializedObject.FindProperty("_split");

//        Slot.BoardSlotMethod slotMethod = (Slot.BoardSlotMethod)method.enumValueIndex;

//        if(slotMethod == Slot.BoardSlotMethod.split)
//        EditorGUILayout.PropertyField(splitMethod);
        
//    }
//}
#endif
