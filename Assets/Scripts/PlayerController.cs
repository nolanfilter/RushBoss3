using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public GameObject muzzleFlashObject;
    public GameObject slashObject;
    public GameObject bigMuzzleFlashObject;
    public GameObject bigSlashObject;
    public GameObject fireballObject;

    private Vector2Int startPosition = new Vector2Int( 1, 1 );
    private Vector2Int currentPosition = Vector2Int.zero;
    
    private Vector2 moveInput;
    
    private InputAction moveAction;
    private InputAction playAction;
    private InputAction stockAction;
    private InputAction comboAction;

    private float lastMoveTime = 0f;
    private float moveCooldown = 0.2f;

    private IEnumerator muzzleFlashCoroutine;
    private IEnumerator slashCoroutine;

    private CardManager.CardType[] startingDeck = new []
    {
        CardManager.CardType.Ranged,
        CardManager.CardType.Ranged,
        CardManager.CardType.Ranged,
        CardManager.CardType.Melee,
        CardManager.CardType.Melee,
    };

    // private List<int> currentDeck = new List<int>();

    // private ShuffleBag deckShuffleBag;
    private List<CardManager.CardType> deck = new List<CardManager.CardType>();
    private List<CardManager.CardType> discard = new List<CardManager.CardType>();
    private CardManager.CardType currentCard = CardManager.CardType.Invalid;
    private List<CardManager.CardType> stock = new List<CardManager.CardType>();
    
    void Awake()
    {
        moveAction = InputSystem.actions.FindAction( "Move" );
        playAction = InputSystem.actions.FindAction( "Play" );
        stockAction = InputSystem.actions.FindAction( "Stock" );
        comboAction = InputSystem.actions.FindAction( "Combo" );
        
        // deckShuffleBag = new ShuffleBag( startingDeck.Length );
        CardManager.SetDeckSize( startingDeck.Length );
        
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
        
        if( stockAction != null )
        {
            stockAction.Enable();
        }
        
        if( comboAction != null )
        {
            comboAction.Enable();
        }
        
        Reset();
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
        
        if( stockAction != null )
        {
            stockAction.Disable();
        }
        
        if( comboAction != null )
        {
            comboAction.Disable();
        }
    }

    void Update()
    {
        if( moveAction != null )
        {
            moveInput = moveAction.ReadValue<Vector2>();
            
            if( moveInput.sqrMagnitude > 0f && Time.realtimeSinceStartup - lastMoveTime > moveCooldown && muzzleFlashCoroutine == null )
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
            Play();
        }

        if( stockAction != null && stockAction.WasPerformedThisFrame() )
        {
            // Debug.Log( "stocking " + currentCard );
            
            if( currentCard != CardManager.CardType.Invalid )
            {
                stock.Add( currentCard );
                
                Draw();
            }
        }
        
        if( comboAction != null && comboAction.WasPerformedThisFrame() )
        {
            if( stock.Count > 0 )
            {
                // TODO action

                discard.AddRange( stock );
                stock.Clear();
                
                if( currentCard == CardManager.CardType.Invalid )
                {
                    StartCoroutine( DoShuffle() );
                }
                else
                {
                    CardManager.UpdateDisplay( deck.Count, currentCard, stock );
                }
            }
        }
    }

    private void Reset()
    {
        if( muzzleFlashObject )
        {
            muzzleFlashObject.SetActive( false );
        }

        if( slashObject )
        {
            slashObject.SetActive( false );
        }
        
        discard.Clear();
        deck.Clear();
        deck.AddRange( startingDeck );
        deck = Utilities.ShuffleList( deck );
        Draw();
    }

    private void Draw()
    {
        currentCard = CardManager.CardType.Invalid;

        if( deck.Count > 0 )
        {
            currentCard = deck[ 0 ];
            
            deck.RemoveAt( 0 );
        }
        
        CardManager.UpdateDisplay( deck.Count, currentCard, stock );
    }

    private void Move( Vector2Int direction )
    {
        Vector3? position = GridManager.GetPositionForTile( currentPosition + direction, TileInfo.Type.Player );

        if( position != null )
        {
            currentPosition += direction;
            transform.position = (Vector3)position;
        }
    }

    private void Play()
    {
        if( currentCard != CardManager.CardType.Invalid )
        {
            switch( currentCard )
            {
                case CardManager.CardType.Ranged:
                {
                    if( muzzleFlashCoroutine != null )
                    {
                        StopCoroutine( muzzleFlashCoroutine );
                        muzzleFlashCoroutine = null;
                    }

                    muzzleFlashCoroutine = DoMuzzleFlash();
                    StartCoroutine( muzzleFlashCoroutine );
                    
                    RaycastHit2D hit = Physics2D.Raycast( transform.position + Vector3.right, Vector2.right );

                    if( hit )
                    {
                        Debug.Log( "hit: " + hit.collider.gameObject.name );
                
                        EnemyController enemyController = hit.collider.gameObject.GetComponent<EnemyController>();

                        if( enemyController )
                        {
                            enemyController.Hurt( 1f );
                        }
                    }
                } break;

                case CardManager.CardType.Melee:
                {
                    if( slashCoroutine != null )
                    {
                        StopCoroutine( slashCoroutine );
                        slashCoroutine = null;
                    }

                    slashCoroutine = DoSlash();
                    StartCoroutine( slashCoroutine );
                    
                    RaycastHit2D hit = Physics2D.Raycast( transform.position + Vector3.right, Vector2.right, 1f );

                    if( hit )
                    {
                        Debug.Log( "hit: " + hit.collider.gameObject.name );
                
                        EnemyController enemyController = hit.collider.gameObject.GetComponent<EnemyController>();

                        if( enemyController )
                        {
                            enemyController.Hurt( 1f );
                        }
                    }
                } break;
            }
            
            discard.Add( currentCard );

            if( deck.Count > 0 )
            {
                Draw();
            }
            else
            {
                StartCoroutine( DoShuffle() );
            }
        }
    }

    private IEnumerator DoShuffle()
    {
        currentCard = CardManager.CardType.Invalid;
        
        CardManager.UpdateDisplay( 0, currentCard, stock );

        yield return new WaitForSeconds( 1f );

        deck.AddRange( discard );
        discard.Clear();
        
        deck = Utilities.ShuffleList( deck );

        Draw();
    }
    
    private IEnumerator DoMuzzleFlash()
    {
        if( muzzleFlashObject )
        {
            muzzleFlashObject.SetActive( true );
        }

        yield return new WaitForSeconds( 0.1f );
        
        if( muzzleFlashObject )
        {
            muzzleFlashObject.SetActive( false );
        }

        muzzleFlashCoroutine = null;
    }
    
    private IEnumerator DoSlash()
    {
        if( slashObject )
        {
            slashObject.SetActive( true );
        }

        yield return new WaitForSeconds( 0.1f );
        
        if( slashObject )
        {
            slashObject.SetActive( false );
        }

        muzzleFlashCoroutine = null;
    }
}
