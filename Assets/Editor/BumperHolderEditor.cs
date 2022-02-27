using UnityEditor;
 using UnityEngine;
 
 [CustomEditor(typeof(BumperHolder)), CanEditMultipleObjects]
 public class PropertyHolderEditor : Editor
 {

     public SerializedProperty
         type_Prop,
         movementsComponent_Prop,
         backBumpSpeedMultiplicator_Prop,
         backBumpDuration_Prop,
         sideBumpTime_Prop,
         bumpDeviation_Prop;
     
     void OnEnable () {
         // Setup the SerializedProperties
         type_Prop = serializedObject.FindProperty ("type");
         movementsComponent_Prop = serializedObject.FindProperty("movementsComponent");
         backBumpSpeedMultiplicator_Prop = serializedObject.FindProperty ("backBumpSpeedMultiplicator");
         backBumpDuration_Prop = serializedObject.FindProperty ("backBumpDuration");
         sideBumpTime_Prop = serializedObject.FindProperty ("sideBumpTime");
         bumpDeviation_Prop = serializedObject.FindProperty ("bumpDeviation");
     }
     
     public override void OnInspectorGUI() {
         serializedObject.Update ();
         
         EditorGUILayout.PropertyField( type_Prop );
         
         BumperHolder.BumperType ty = (BumperHolder.BumperType)type_Prop.enumValueIndex;
         
         switch( ty ) {
         case BumperHolder.BumperType.BackBumper:
             
             EditorGUILayout.FloatField ( new GUIContent("backBumpSpeedMultiplicator"), backBumpSpeedMultiplicator_Prop.floatValue  );
             EditorGUILayout.FloatField ( new GUIContent("backBumpDuration"), backBumpDuration_Prop.floatValue  );
             
             EditorGUILayout.PropertyField ( movementsComponent_Prop, new GUIContent("movementsComponent") );
             break;
 
         case BumperHolder.BumperType.LeftBumper:
         case BumperHolder.BumperType.RightBumper:   
             EditorGUILayout.FloatField ( new GUIContent("sideBumpTime"), sideBumpTime_Prop.floatValue  );
             EditorGUILayout.FloatField ( new GUIContent("bumpDeviation"), bumpDeviation_Prop.floatValue  );
             
             EditorGUILayout.PropertyField ( movementsComponent_Prop, new GUIContent("movementsComponent") );
             break;
         }
         
         serializedObject.ApplyModifiedProperties ();
     }
 }
