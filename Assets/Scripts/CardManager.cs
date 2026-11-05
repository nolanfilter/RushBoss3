using System;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public static CardManager instance;
    
    public GameObject cardPrefab;
    
    public enum CardType
    {
        Invalid = -1,
        Ranged = 0,
        Melee = 1,
    }
    
    [EnumNamedArray( typeof( CardType ) )]
    public Sprite[] cardSprites = new Sprite[ System.Enum.GetValues( typeof( CardType ) ).Length - 1 ];

    public Transform deckRoot;
    public Transform handRoot;
    public Transform stockRoot;
    
    private CardBridge[] cardBridges =  Array.Empty<CardBridge>();
    
    void Awake()
    {
        instance = this;

        if( deckRoot )
        {
            foreach( Transform child in deckRoot )
            {
                Destroy( child.gameObject );
            }
        }
        
        if( handRoot )
        {
            foreach( Transform child in handRoot )
            {
                Destroy( child.gameObject );
            }
        }
        
        if( stockRoot )
        {
            foreach( Transform child in stockRoot )
            {
                Destroy( child.gameObject );
            }
        }
    }

    public static void SetDeckSize( int deckSize )
    {
        if( instance )
        {
            instance.internalSetDeckSize( deckSize );
        }
    }

    private void internalSetDeckSize( int deckSize )
    {
        // Debug.Log( "setting deck size to: " + deckSize );
        
        deckSize = Mathf.Min( deckSize, 7 );
        
        if( cardBridges.Length != deckSize )
        {
            for( int i = 0; i < cardBridges.Length; i++ )
            {
                if( cardBridges[ i ] )
                {
                    Destroy( cardBridges[ i ].gameObject );
                }
            }

            cardBridges = new CardBridge[ deckSize ];

            if( cardPrefab )
            {
                for( int i = 0; i < cardBridges.Length; i++ )
                {
                    GameObject cardObject = Instantiate( cardPrefab );

                    cardBridges[ i ] = cardObject.GetComponent<CardBridge>();

                    cardObject.SetActive( false );
                }
            }
        }
    }
    
    public static void UpdateDisplay( int deckSize, CardType currentCard, List<CardType> stock )
    {
        if( instance )
        {
            instance.internalUpdateDisplay( deckSize, currentCard, stock );
        }
    }

    private void internalUpdateDisplay( int deckSize, CardType currentCard, List<CardType> stock )
    {
        // Debug.Log( "update display with deckSize: " + deckSize + ", currentCard: " + currentCard + ", stock.Count: " + stock.Count );

        for( int i = 0; i < cardBridges.Length; i++ )
        {
            if( cardBridges[ i ] )
            {
                cardBridges[ i ].gameObject.SetActive( false );
            }
        }
        
        int deckBridgeIndex = 0;
        
        if( deckRoot )
        {
            // deckSize = Mathf.Min( deckSize, 3 );
            
            for( int i = 0; i < deckSize; i++ )
            {
                CardBridge cardBridge = cardBridges[ deckBridgeIndex ];
                
                if( cardBridge )
                {
                    if( cardBridge.rectTransform )
                    {
                        cardBridge.rectTransform.SetParent( deckRoot );
                    }

                    if( cardBridge.backgroundImage )
                    {
                        cardBridge.backgroundImage.color = new Color( 0.25f, 0.25f, 0.25f, 1f );
                    }

                    if( cardBridge.iconImage )
                    {
                        cardBridge.iconImage.sprite = null;
                        cardBridge.iconImage.enabled = false;
                    }

                    cardBridge.gameObject.SetActive( true );
                }

                deckBridgeIndex++;
            }
        }

        if( handRoot && currentCard != CardType.Invalid )
        {
            CardBridge cardBridge = cardBridges[ deckBridgeIndex ];
                
            if( cardBridge )
            {
                if( cardBridge.rectTransform )
                {
                    cardBridge.rectTransform.SetParent( handRoot );
                }

                if( cardBridge.backgroundImage )
                {
                    cardBridge.backgroundImage.color = Color.white;
                }

                if( cardBridge.iconImage )
                {
                    cardBridge.iconImage.sprite = GetSpriteForCardType( currentCard );
                    cardBridge.iconImage.enabled = true;
                }

                cardBridge.gameObject.SetActive( true );
            }

            deckBridgeIndex++;
        }

        if( stockRoot )
        {
            for( int i = 0; i < stock.Count; i++ )
            {
                CardBridge cardBridge = cardBridges[ deckBridgeIndex ];
                
                if( cardBridge )
                {
                    if( cardBridge.rectTransform )
                    {
                        cardBridge.rectTransform.SetParent( stockRoot );
                        cardBridge.rectTransform.SetSiblingIndex( i );
                    }

                    if( cardBridge.backgroundImage )
                    {
                        cardBridge.backgroundImage.color = Color.white;
                    }

                    if( cardBridge.iconImage )
                    {
                        cardBridge.iconImage.sprite = GetSpriteForCardType( stock[ i ] );
                        cardBridge.iconImage.enabled = true;
                    }

                    cardBridge.gameObject.SetActive( true );
                }

                deckBridgeIndex++;
            }
        }
    }

    private Sprite GetSpriteForCardType( CardType cardType )
    {
        int index = (int)cardType;

        // Debug.Log( index );
        
        if( index >= 0 && index < cardSprites.Length )
        {
            return cardSprites[ index ];
        }

        return null;
    }
}
