using System;
using System.Collections.Generic;
using UnityEngine;

public class Utilities
{
    public static Color whiteClear = new Color( 1f, 1f, 1f, 0f );
    
    public static float GetParticleSystemMaxTime( ParticleSystem.MainModule module )
    {
        return module.duration + module.startLifetime.constantMax;
    }
    
    public static float BezierEaseInEaseOut( float value )
    {
        return value * value * 3f - value * value * value * 2f;
    }
    
    public static float EaseInCircular( float value )
    {
        return 1f - Mathf.Sqrt( 1f - ( value * value ) );
    }

    public static List<T> ShuffleList<T>( List<T> inputList )
    {
        if( inputList.Count == 0 )
            return inputList;

        List<T> list = new List<T>();
        list.AddRange( inputList );

        T temp;
        int randomIndex;

        // Fisher-Yates shuffle
        for( int i = 0; i < list.Count - 1; i++ )
        {
            randomIndex = UnityEngine.Random.Range( i, list.Count );
            temp = list[ i ];
            list[ i ] = list[ randomIndex ];
            list[ randomIndex ] = temp;
        }

        return list;
    }
}

public class EnumNamedArrayAttribute : PropertyAttribute
{
    public string[] names;

    public EnumNamedArrayAttribute( Type enumType )
    {
        names = Enum.GetNames( enumType );
    }
}
