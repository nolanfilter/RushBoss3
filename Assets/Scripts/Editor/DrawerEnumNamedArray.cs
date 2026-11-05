using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

// adapted from https://discussions.unity.com/t/showing-an-array-with-enum-as-keys-in-the-property-inspector/218840/3
[ CustomPropertyDrawer( typeof( EnumNamedArrayAttribute ) ) ]
public class DrawerEnumNamedArray : PropertyDrawer
{
    public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
    {
        EnumNamedArrayAttribute enumNames = attribute as EnumNamedArrayAttribute;
        
        int index = System.Convert.ToInt32( property.propertyPath.Substring( property.propertyPath.IndexOf( "[" ) ).Replace( "[", "" ).Replace( "]", "" ) );

        if( enumNames != null && index >= 0 && index < enumNames.names.Length && !string.IsNullOrEmpty( enumNames.names[ index ] ) )
        {
            label.text = enumNames.names[ index ];
        }

        EditorGUI.PropertyField( position, property, label, true );
    }
}