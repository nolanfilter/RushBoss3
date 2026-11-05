using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyController : MonoBehaviour
{
    public GameObject hitFlashObject;
    
    private Vector2Int startPosition = new Vector2Int( 4, 1 );
    private Vector2Int currentPosition = Vector2Int.zero;

    // TODO make behavior graph
    private float randomCooldown;
    private float lastMoveTime;
    
    private IEnumerator hitFlashCoroutine;
    
    void Awake()
    {
        Move( startPosition );

        randomCooldown = Random.Range( 0.25f, 1f );
    }

    void OnEnable()
    {
        Reset();
    }

    void Update()
    {
        if( Time.realtimeSinceStartup - lastMoveTime > randomCooldown )
        {
            Vector2 randomDirection = Random.onUnitCircle;
            
            Move( new Vector2Int( Mathf.RoundToInt( randomDirection.x ), Mathf.RoundToInt( randomDirection.y ) ) );
            
            randomCooldown = Random.Range( 0.25f, 1f );
            lastMoveTime = Time.realtimeSinceStartup;
        }
    }
    
    private void Reset()
    {
        if( hitFlashObject )
        {
            hitFlashObject.SetActive( false );
        }
    }

    private void Move( Vector2Int direction )
    {
        Vector3? position = GridManager.GetPositionForTile( currentPosition + direction, TileInfo.Type.Enemy );

        if( position != null )
        {
            currentPosition += direction;
            transform.position = (Vector3)position;
        }
    }

    public void Hurt( float damage )
    {
        if( hitFlashCoroutine != null )
        {
            StopCoroutine( hitFlashCoroutine );
            hitFlashCoroutine = null;
        }

        hitFlashCoroutine = DoHitFlash();
        StartCoroutine( hitFlashCoroutine );
    }
    
    private IEnumerator DoHitFlash()
    {
        // length of player controller muzzle flash
        yield return new WaitForSeconds( 0.1f );
        
        if( hitFlashObject )
        {
            hitFlashObject.transform.localPosition = Random.insideUnitCircle + Vector2.up * 1.5f;
            hitFlashObject.SetActive( true );
        }

        yield return new WaitForSeconds( 0.1f );
        
        if( hitFlashObject )
        {
            hitFlashObject.SetActive( false );
        }

        hitFlashCoroutine = null;
    }
}
