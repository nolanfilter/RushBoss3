using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

[Serializable]
public class ShuffleBag {

    public int index = -1;
    public int[] indices = new int[ 0 ];
    public int[] blacklistIndices;

    private bool useOwnRandomness = false;
    
    // optional native C# Random class to not interfere with determinism
    private Random random;
    
    public int length => indices.Length;

    public int size
    {
        get
        {
            Debug.Log( "index: " + index + ", blacklistIndices: " + ArrayToString( blacklistIndices ) );
            
            int progress = index;

            if( blacklistIndices != null )
            {
                foreach( int blacklistIndex in blacklistIndices )
                {
                    if( blacklistIndex >= index )
                    {
                        progress++;
                    }
                }
            }
            
            return length - progress;
        }
    }

    public ShuffleBag( int[] indices, bool useOwnRandomness = false, int[] blacklistIndices = null )
    {
        this.indices = indices;
        
        for( int i = 0; i < indices.Length; i++ )
        {
            // TODO this is fragile
            if( indices[ i ] == 0 )
            {
                indices[ i ] = i;
            }
        }
        
        this.useOwnRandomness = useOwnRandomness;

        if( useOwnRandomness )
        {
            random = new Random();
        }

        this.blacklistIndices = blacklistIndices;
        
        Shuffle();
    }

    public ShuffleBag( int length, bool useOwnRandomness = false, int[] blacklistIndices = null ) : this( new int[ length ], useOwnRandomness, blacklistIndices ) {}

    public int GetNext( bool useBlacklist = false )
    {
        if( indices.Length == 0 || ( blacklistIndices != null && indices.Length == blacklistIndices.Length ) )
        {
            return -1;
        }

        if( index == -1 || index == indices.Length )
        {
            Shuffle();
        }

        int next = indices[ index++ ];
        
        if( useBlacklist && blacklistIndices != null )
        {
            int tries = indices.Length;
            
            while( Array.IndexOf( blacklistIndices, next ) != -1 && tries-- >= 0 )
            {
                if( index == indices.Length )
                {
                    Shuffle();
                }
            
                next = indices[ index++ ];
            }

            if( tries == -1 )
            {
                next = -1;
            }
        }

        return next;
    }

    public void Remove( int indexToRemove = -1 )
    {
        if( indexToRemove == -1 && index == 0 )
        {
            return;
        }
        
        int blacklistIndicesLength = blacklistIndices?.Length ?? 0;

        int[] newBlacklistIndices = new int[ blacklistIndicesLength + 1 ];

        for( int i = 0; i < blacklistIndicesLength; i++ )
        {
            newBlacklistIndices[ i ] = blacklistIndices[ i ];
        }

        // if no index specified, remove latest index
        if( indexToRemove == -1 )
        {
            indexToRemove = indices[ index - 1 ];
        }

        newBlacklistIndices[ blacklistIndicesLength ] = indexToRemove;

        blacklistIndices = newBlacklistIndices;
    }

    public void Shuffle()
    {
        index = 0;

        if( indices.Length == 0 )
            return;

        int temp;
        int randomIndex;

        // Fisher-Yates shuffle
        for( int i = 0; i < indices.Length - 1; i++ )
        {
            if( useOwnRandomness )
            {
                randomIndex = random.Next( indices.Length - i ) + i;
            }
            else
            {
                randomIndex = UnityEngine.Random.Range( i, indices.Length );
            }

            temp = indices[ i ];
            indices[ i ] = indices[ randomIndex ];
            indices[ randomIndex ] = temp;
        }
    }

    private string ArrayToString( int[] array )
    {
        string values = "";

        if( array != null )
        {
            for( int i = 0; i < array.Length; i++ )
            {
                values += array[ i ];

                if( i < array.Length - 1 )
                {
                    values += ", ";
                }
            }
        }

        return values;
    }
}