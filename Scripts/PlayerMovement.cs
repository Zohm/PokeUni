using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine.Assertions;

public enum EInput : byte {
	eInput_None,
	eInput_Left,
	eInput_Right,
	eInput_Up,
	eInput_Down
}

public class PlayerMovement : MonoBehaviour 
{
    Vector3 pos;                                // For movement
	public float speed = Config.PLAYER_SPEED;                         // Speed of movement
    Animator m_animator;
	ArrayList previousInput;

	PlayerActions playerActionComp = null;

	bool m_movedOnGrass = false;

	public EInput m_facingDirection = EInput.eInput_Down;

	//-----------------------------------------------------------------------------------------------------------
    private void Start()
    {
        pos = transform.position;          // Take the initial position
        m_animator = GetComponent<Animator>();
		previousInput = new ArrayList (4);

		playerActionComp = GetComponent<PlayerActions> ();
    }

	private void Update()
	{
		if( Input.GetKeyUp (KeyCode.LeftArrow) )
		{
			RemoveInputFromList (EInput.eInput_Left);
		}
		if( Input.GetKeyUp (KeyCode.RightArrow) )
		{
			RemoveInputFromList (EInput.eInput_Right);
		}
		if( Input.GetKeyUp (KeyCode.UpArrow) )
		{
			RemoveInputFromList (EInput.eInput_Up);
		}
		if( Input.GetKeyUp (KeyCode.DownArrow) )
		{
			RemoveInputFromList (EInput.eInput_Down);
		}
	}

	//-----------------------------------------------------------------------------------------------------------
	private void FixedUpdate()
    {
		if( playerActionComp && playerActionComp.m_playerBlocked )
		{
			return;
		}

		bool isWalking = true;
		if( transform.position == pos )
		{
			if( m_movedOnGrass )
			{
				m_movedOnGrass = false;
				if( LookForRandomEncounter () )
				{
					return;
				}
			}

			UpdateNewInputs ();

			switch(GetLastInputFromList())
			{
			case(EInput.eInput_Left):
				MoveLeft (ref isWalking);
				break;
			case(EInput.eInput_Right):
				MoveRight (ref isWalking);
				break;
			case(EInput.eInput_Up):
				MoveUp (ref isWalking);
				break;
			case(EInput.eInput_Down):
				MoveDown (ref isWalking);
				break;
			default:
				isWalking = false;
				break;
			}
		}

		m_animator.SetBool("isWalking", isWalking ? true : false);
		transform.position = Vector3.MoveTowards(transform.position, pos, Time.deltaTime * speed);    // Move there
    }

	//-----------------------------------------------------------------------------------------------------------
	private void UpdateNewInputs()
	{
		if( Input.GetKey(KeyCode.LeftArrow) && !previousInput.Contains (EInput.eInput_Left) )
		{
			AddInputToList (EInput.eInput_Left);
		}
		else if( Input.GetKey(KeyCode.RightArrow) && !previousInput.Contains (EInput.eInput_Right) )
		{
			AddInputToList (EInput.eInput_Right);
		}
		else if( Input.GetKey(KeyCode.UpArrow) && !previousInput.Contains (EInput.eInput_Up) )
		{
			AddInputToList (EInput.eInput_Up);
		}
		else if( Input.GetKey(KeyCode.DownArrow) && !previousInput.Contains (EInput.eInput_Down) )
		{
			AddInputToList (EInput.eInput_Down);
		}
	}

	//-----------------------------------------------------------------------------------------------------------
	private void MoveLeft(ref bool isWalking)
	{
		Vector3 newPos = pos + Vector3.left * 0.32f;
		if( CanPlayerMoveToPosition (newPos) )
		{
			pos = newPos;
		}
		else
		{
			isWalking = false;
		}


		m_animator.SetFloat("input_x", -1f);
		m_animator.SetFloat("input_y", 0f);
	}

	//-----------------------------------------------------------------------------------------------------------
	private void MoveRight(ref bool isWalking)
	{
		Vector3 newPos = pos + Vector3.right * 0.32f;
		if (CanPlayerMoveToPosition (newPos)) 
		{
			pos = newPos;
		}
		else
		{
			isWalking = false;
		}

		m_animator.SetFloat ("input_x", 1f);
		m_animator.SetFloat ("input_y", 0f);
	}

	//-----------------------------------------------------------------------------------------------------------
	private void MoveUp(ref bool isWalking)
	{
		Vector3 newPos = pos + Vector3.up * 0.32f;
		if (CanPlayerMoveToPosition (newPos)) 
		{
			pos = newPos;
		}
		else
		{
			isWalking = false;
		}


		m_animator.SetFloat ("input_x", 0f);
		m_animator.SetFloat ("input_y", 1f);
	}

	//-----------------------------------------------------------------------------------------------------------
	private void MoveDown(ref bool isWalking)
	{
		Vector3 newPos = pos + Vector3.down * 0.32f;
		if (CanPlayerMoveToPosition (newPos)) 
		{
			pos = newPos;
		}
		else
		{
			isWalking = false;
		}


		m_animator.SetFloat ("input_x", 0f);
		m_animator.SetFloat ("input_y", -1f);
	}

	//-----------------------------------------------------------------------------------------------------------
	private void AddInputToList(EInput pressed)
	{
		previousInput.Add (pressed);

		//Update facing direction
		m_facingDirection = pressed;
	}

	//-----------------------------------------------------------------------------------------------------------
	private void RemoveInputFromList(EInput pressed)
	{
		previousInput.Remove (pressed);

		//Update facing direction
		if( previousInput.Count > 0 )
		{
			m_facingDirection = GetLastInputFromList ();
		}
	}

	//-----------------------------------------------------------------------------------------------------------
	public EInput GetLastInputFromList()
	{
		return previousInput.Count > 0 ? (EInput) previousInput [previousInput.Count-1] : EInput.eInput_None ;
	}

	//-----------------------------------------------------------------------------------------------------------
	bool CanPlayerMoveToPosition(Vector2 destination)
	{
		Collider2D collision = Physics2D.OverlapPoint (destination);
		if( collision )
		{
			switch(LayerMask.LayerToName(collision.gameObject.layer))
			{
			case "Wall":
				return false;
			case "Grass":
				{
					m_movedOnGrass = true;
					return true;
				}
			case "Door":
				return false;
			case "Water":
				return false;
			case "Sign":
				return false;
			case "Warper":
				{
					playerActionComp.m_playerBlocked = true;
					Warper warper = collision.gameObject.GetComponent<Warper> ();
					StartCoroutine (warper.TeleportToNewPos ((x) => pos = x));
				
					return false;
				}
			default:
				break;
			}
		}
		return true;
	}

	//-----------------------------------------------------------------------------------------------------------
	bool LookForRandomEncounter()
	{
		if( Random.Range (0, 101) < Config.ENCOUNTER_PROBA )
		{
			//get current area wild pokemons
			GameObject currentMapObject = GameObject.FindWithTag ("CurrentMap");
			if (currentMapObject) {
				List<MapDatabase.MapWildPokemon> listPoke = MapDatabase.GetListOfWildPokemons (currentMapObject.name);
				if (listPoke != null && listPoke.Count > 0) 
				{
					//random to choose which pokemon is encountered
					int randValue = Random.Range (0, 101);
					int proba = 0; //listPoke [0].m_proba;
					int indexChoice = -1;
					for(int i = 0; i < listPoke.Count && indexChoice == -1; i++)
					{
						proba += listPoke [i].m_proba;
						if( randValue < proba ) //we found the pokemon
						{
							indexChoice = i;
						}
					}

					//In case the proba in DB were not equal to 100, there may not be a pokemon found
					Assert.AreNotEqual (indexChoice, -1);
					if( indexChoice != -1 )
					{
						Pokemon wildPokemon = Pokemon.CreateInstance<Pokemon> ();
                        wildPokemon.GeneratePokemon (listPoke[indexChoice].m_pokemonId, listPoke[indexChoice].m_minLvl, listPoke[indexChoice].m_maxLvl);

                        BattleManager battleManager = FindObjectOfType<BattleManager> ();
                        if( battleManager )
                        {
                            //At least one pokemon available, block Player action before fight
                            playerActionComp.m_playerBlocked = true;
                            m_animator.SetBool("isWalking", false);

                            //TODO: Anim transition
                            //start anim
                            //while anim, generate pokemon

                            //TODO
                            //display fight UI
                            //StartEncounter ();
                            battleManager.StartFight(wildPokemon);

                            return true;
                        }
					}
				}
			}
		}
		return false;
	}


}
