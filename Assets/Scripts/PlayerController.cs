using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Vector2Int startPosition = new Vector2Int( 1, 1 );
    private Vector2Int currentPosition = Vector2Int.zero;
    
    private Vector2 moveInput;
    
    private InputAction moveAction;
    private InputAction playAction;
    private InputAction stockAction;
    private InputAction comboAction;

    private float lastMoveTime = 0f;
    private float moveCooldown = 0.2f;

    void Awake()
    {
        moveAction = InputSystem.actions.FindAction( "Move" );
        playAction = InputSystem.actions.FindAction( "Play" );
        // stockAction = InputSystem.actions.FindAction( "Stock" );
        // comboAction = InputSystem.actions.FindAction( "Combo" );
        
        Move( startPosition );
    }

    void OnEnable()
    {
        if( moveAction != null )
        {
            moveAction.Enable();
        }
        
        if( playAction != null )
        {
            playAction.Enable();
        }
    }
    
    void OnDisable()
    {
        if( moveAction != null )
        {
            moveAction.Disable();
        }
        
        if( playAction != null )
        {
            playAction.Disable();
        }
    }

    void Update()
    {
        if( moveAction != null )
        {
            moveInput = moveAction.ReadValue<Vector2>();
            
            if( moveInput.sqrMagnitude > 0f && Time.realtimeSinceStartup - lastMoveTime > moveCooldown )
            {
                if( moveInput.sqrMagnitude > 0f )
                {
                    Move( new Vector2Int( Mathf.RoundToInt( moveInput.x ), Mathf.RoundToInt( moveInput.y ) ) );
                }
                
                lastMoveTime = Time.realtimeSinceStartup;
                moveInput = Vector2.zero;
            }
        }

        if( playAction != null && playAction.WasPerformedThisFrame() )
        {
            List<Vector2Int> tileIndicies = new List<Vector2Int>();

            for( int i = currentPosition.x + 1; i < GridManager.width; i++ )
            {
                tileIndicies.Add( new Vector2Int( i, currentPosition.y ) );
            }
            
            GridManager.HighlightTiles( tileIndicies.ToArray() );
        }
    }

    public void Move( Vector2Int direction )
    {
        Vector3? position = GridManager.GetPositionForTile( currentPosition + direction, TileInfo.Type.Player );

        if( position != null )
        {
            currentPosition += direction;
            transform.position = (Vector3)position;
        }
    }
}
