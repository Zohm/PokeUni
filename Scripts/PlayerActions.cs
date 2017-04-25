using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerActions : MonoBehaviour 
{
	//List of signs in current map. Should be updated at every load of a new level
	List<SignDatabase.SignItem> signList = null;

	public bool m_playerBlocked = false;

	//-----------------------------------------------------------------------------------------------------------
	// Use this for initialization
	void Start () 
	{
		GameObject currentMapObject = GameObject.FindWithTag ("CurrentMap");
		if( currentMapObject )
		{
			MapDatabase.MapItem currentMapDb = MapDatabase.GetMapByName (currentMapObject.name);
			if( currentMapDb != null )
			{
				signList = SignDatabase.GetSignsOfMap (currentMapDb.m_uniqueId);
			}
		}
	}

	//-----------------------------------------------------------------------------------------------------------
	// Update is called once per frame
	void Update () 
	{
		if ( !m_playerBlocked && Input.GetKeyUp (KeyCode.A) ) 
		{
			CheckPossibleActions ();
		}
	}

	//-----------------------------------------------------------------------------------------------------------
	void CheckPossibleActions ()
	{
		//Get player position
		Vector2 playerPos = transform.position;

		CheckOnpositionAction (playerPos);
		CheckInFrontOfPlayerAction (playerPos);
	}

	//-----------------------------------------------------------------------------------------------------------
	void CheckOnpositionAction (Vector3 pos)
	{
		// check for invisible hidden object
	}

	//-----------------------------------------------------------------------------------------------------------
	void CheckInFrontOfPlayerAction (Vector3 pos)
	{
		Vector3 facingTilePos = new Vector3();
		GetFacingTilePosition (pos, ref facingTilePos);
		Collider2D collision = Physics2D.OverlapPoint (facingTilePos);
		if (collision) {
			switch (LayerMask.LayerToName (collision.gameObject.layer)) {
			case "Water":
				break;
			case "Sign":
				ReadSign (facingTilePos);
				break;
			default:
				break;
			}
		}
	}

	//-----------------------------------------------------------------------------------------------------------
	void GetFacingTilePosition(Vector3 pos, ref Vector3 facingTilePos)
	{
		PlayerMovement playerMovementComp = gameObject.GetComponent<PlayerMovement> ();
		if( playerMovementComp )
		{
			switch( playerMovementComp.m_facingDirection )
			{
			case(EInput.eInput_Left):
				facingTilePos = pos + Vector3.left * 0.32f;
				break;
			case(EInput.eInput_Right):
				facingTilePos = pos + Vector3.right * 0.32f;
				break;
			case(EInput.eInput_Up):
				facingTilePos = pos + Vector3.up * 0.32f;
				break;
			case(EInput.eInput_Down):
				facingTilePos = pos + Vector3.down * 0.32f;
				break;
			default:
				break;
			}
		}
	}

	//-----------------------------------------------------------------------------------------------------------
	void ReadSign(Vector2 facingTilePos)
	{
		//Player position is the center of the sprite. We want to get the left top corner position
		facingTilePos [0] -= 0.16f;
		facingTilePos [1] += 0.16f;

		foreach( SignDatabase.SignItem sign in signList )
		{
			if( sign.m_position == facingTilePos )
			{
				DialogueManager dialogManager = FindObjectOfType<DialogueManager> ();
				if( dialogManager )
				{
					m_playerBlocked = true;
					dialogManager.ReadSign (sign.m_signText);
				}
				return;
			}
		}
	}
}