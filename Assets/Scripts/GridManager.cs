using System.Collections;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager instance;

    public GameObject tilePrefab;
    public Transform tileRoot;

    private Color playerColor = new Color( 0.75f, 0.75f, 0.75f, 1f );
    private Color enemyColor = new Color( 0.25f, 0.25f, 0.25f, 1f );
    
    private Color fillDefault = new Color( 1f, 1f, 1f, 1f ); 
    private Color fillHighlight = new Color( 0.75f, 0.75f, 0.75f, 1f ); 
    private Color fillLowlight = new Color( 0.25f,0.25f, 0.25f, 1f ); 
    
    private static GameObject[,] tileObjects;
    private static TileInfo[,] tiles;

    public const int width = 6;
    public const int height = 3;

    void Awake()
    {
        instance = this;
        
        foreach( Transform child in tileRoot )
        {
            Destroy( child.gameObject );
        }
        
        tileObjects = new GameObject[ width, height ];
        tiles = new TileInfo[ width, height ];

        int midWidth = width / 2;

        float startX = -6.25f;
        float startY = -1.5f;
        float intervalX = 2.5f;
        float intervalY = 1.5f;
        
        for( int i = 0; i < width; i++ )
        {
            for( int j = 0; j < height; j++ )
            {
                Vector3 position = new Vector3( startX + i * intervalX, startY + j * intervalY, 0f ) ;
                TileBridge tileBridge = null;
                
                if( tilePrefab )
                {
                    GameObject tileObject = Instantiate( tilePrefab, tileRoot );

                    tileObject.transform.localPosition = position;

                    tileBridge = tileObject.GetComponent<TileBridge>();

                    tileObjects[ i, j ] = tileObject;
                }
                
                tiles[ i, j ] = new TileInfo( i < midWidth ? TileInfo.Type.Player : TileInfo.Type.Enemy,
                    position, tileBridge );
                
                SetTileFillColor( i, j, fillDefault );
            }
        }

        UpdateTileOutline();
    }

    public static Vector3? GetPositionForTile( Vector2Int tileIndex, TileInfo.Type type )
    {
        if( tileIndex.x >= 0 && tileIndex.x < width && tileIndex.y >= 0 && tileIndex.y < height && type == tiles[ tileIndex.x, tileIndex.y ].type )
        {
            return tiles[ tileIndex.x, tileIndex.y ].position;
        }

        return null;
    }

    public static void HighlightTiles( Vector2Int[] tileIndices, float duration = 0.1f )
    {
        if( instance )
        {
            instance.internalHighlightTiles( tileIndices, duration );
        }
    }

    private void internalHighlightTiles( Vector2Int[] tileIndices, float duration )
    {
        StartCoroutine( DoHighlightTiles( tileIndices, duration ) );
    }

    private void UpdateTileOutline()
    {
        for( int i = 0; i < width; i++ )
        {
            for( int j = 0; j < height; j++ )
            {
                if( tiles[ i, j ] != null && tiles[ i, j ].tileBridge && tiles[ i, j ].tileBridge.outlineRenderer )
                {
                    switch( tiles[ i, j ].type )
                    {
                        case TileInfo.Type.Player: tiles[ i, j ].tileBridge.outlineRenderer.color = playerColor;
                            break;
                        case TileInfo.Type.Enemy: tiles[ i, j ].tileBridge.outlineRenderer.color = enemyColor;
                            break;
                    }
                }
            }
        }
    }

    private void SetTileFillColor( int x, int y, Color color )
    {
        if( x >= 0 && x < width && y >= 0 && y < height && tiles[ x, y ] != null && tiles[ x, y ].tileBridge && tiles[ x, y ].tileBridge.fillRenderer )
        {
            tiles[ x, y ].tileBridge.fillRenderer.color = color;
        }
    }

    private IEnumerator DoHighlightTiles( Vector2Int[] tileIndices, float duration )
    {
        float currentTime = -Time.deltaTime;

        for( int i = 0; i < tileIndices.Length; i++ )
        {
            SetTileFillColor( tileIndices[ i ].x, tileIndices[ i ].y, fillHighlight );
        }
        
        do
        {
            currentTime += Time.deltaTime;

            yield return null;

        } while ( currentTime < duration );
        
        for( int i = 0; i < tileIndices.Length; i++ )
        {
            SetTileFillColor( tileIndices[ i ].x, tileIndices[ i ].y, fillDefault );
        }
    }
}

public class TileInfo
{
    public enum Type
    {
        None = 0,
        Player = 1,
        Enemy = 2,
        Blocked = 3,
    }

    public Type type;
    public Vector3 position;
    public TileBridge tileBridge;

    public TileInfo( Type type, Vector3 position, TileBridge tileBridge )
    {
        this.type = type;
        this.position = position;
        this.tileBridge = tileBridge;
    }
}