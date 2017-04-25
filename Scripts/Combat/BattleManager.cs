using UnityEngine;
using System.Collections;

public class BattleManager : MonoBehaviour 
{
	public enum EBattleStates
	{
		eNone,
		eSelection,
		eOrderOfActionSelection,
		eAtkPhase1,
		eAtkPhase2,
		eEndTurn
	}

    BattleInterfaceManager m_battleInterfaceManager = null;

    public BattleInfo.PokemonBattleValues m_playerPokeValues;
    public BattleInfo.PokemonBattleValues m_oppPokeValues;

    public BattleInfo.SGeneralBattleValues m_battleValues;

    public BattleInfo.SLastAttackInfo m_lastAttackInfo;

    public bool m_playerAttacking = false;

    //Pokemon in battle
    private Pokemon m_opponentPokemon = null;
    private Pokemon m_playerPokemon = null;

	public bool m_canDoAction = false;
	public EBattleStates m_battleState = EBattleStates.eNone;
	public bool m_blockedByAnimation = false; //usefull?

	// Over turn values
	private bool m_isPlayerInPhase1 = false;
	private CCombatAction m_playerAction = null;
	private CCombatAction m_AiAction = null;

	// -----------------------------------------------------------------------------
	// Use this for initialization
	void Start () 
    {
        m_playerPokeValues = new BattleInfo.PokemonBattleValues();
        m_oppPokeValues = new BattleInfo.PokemonBattleValues();
        m_battleInterfaceManager = FindObjectOfType<BattleInterfaceManager>();
	}

	// -----------------------------------------------------------------------------
	// Update is called once per frame
	void Update () 
    {
	}

	// -----------------------------------------------------------------------------
	void GoToState( EBattleStates state )
	{
		if( m_battleState != state )
		{
			m_battleState = state;
			switch( state )
			{
			case EBattleStates.eSelection:
				{
					// Run AI during player selection
					SelectNextAIAction ();
				}
				break;
			case EBattleStates.eOrderOfActionSelection:
				{
					SelectOrderOfActions();
					GoToState (EBattleStates.eAtkPhase1);
				}
				break;
			case EBattleStates.eAtkPhase1:
				{
					ResolveAttackPhase ( EBattleStates.eAtkPhase1 );
				}
				break;
			case EBattleStates.eAtkPhase2:
				{
					ResolveAttackPhase ( EBattleStates.eAtkPhase2 );
				}
				break;
			case EBattleStates.eEndTurn:
				{
					m_playerAction = null;
					m_AiAction = null;
					m_isPlayerInPhase1 = false;

					GoToState (EBattleStates.eSelection);
				}
				break;
			default:
				break;
			}
		}
	}

    //----------------------------------------------------------------------------
    public void StartFight(Pokemon opponent)
    {
		StartCoroutine( StartBattleTransition(opponent) );  
    }
		
	//----------------------------------------------------------------------------
	IEnumerator StartBattleTransition(Pokemon opponent)
	{
		GameObject camera = GameObject.FindGameObjectWithTag ("MainCamera");
		if( camera )
		{
			SimpleBlit simpleBlitComp = camera.GetComponent<SimpleBlit> ();
			if( simpleBlitComp )
			{
				yield return StartCoroutine( simpleBlitComp.OnBeginBattleTransition() );

				InitBattle (opponent);

				yield return StartCoroutine( simpleBlitComp.OnEndBattleTransition() );

				m_canDoAction = true;
				GoToState (EBattleStates.eSelection);
			}
		}
	}

	//----------------------------------------------------------------------------
	void InitBattle(Pokemon opponent)
	{
		GameObject player = GameObject.FindGameObjectWithTag ("Player");
		if( player )
		{
			Trainer trainerComponent = player.GetComponent<Trainer>();
			if( trainerComponent )
			{
				m_playerPokemon = trainerComponent.GetPokemonFromTeamAtIndex(0);
				m_opponentPokemon = opponent;

				//Start Interface
				m_battleInterfaceManager.StartFight(m_playerPokemon, m_opponentPokemon);
			}
		}
	}

	// -----------------------------------------------------------------------------
	public void RunFromFight()
	{
		m_playerAction = new CRunAction ();
		GoToState (EBattleStates.eOrderOfActionSelection);
	}

	//----------------------------------------------------------------------------
    public void StopFight(bool playerLost = false, bool isRunningFromFight = false)
    {
        if( playerLost )
        {
            // TODO
        }
        else if( isRunningFromFight )
        {
            // TODO
        }
        else
        {
            // TODO
        }

        Reset();

        PlayerActions playerActionComponent = FindObjectOfType<PlayerActions> ();
        if( playerActionComponent )
        {
            playerActionComponent.m_playerBlocked = false;
        }
            
        m_battleInterfaceManager.StopFight();
		m_canDoAction = false;
    }

	// -----------------------------------------------------------------------------
    void Reset()
    {
        m_playerAttacking = false;
        m_playerPokeValues.Reset();
        m_oppPokeValues.Reset();
        m_lastAttackInfo.Reset();
        m_battleValues.Reset();

        m_playerPokemon = null;
		m_opponentPokemon = null;
		m_battleState = EBattleStates.eNone;
		m_blockedByAnimation = false;

		m_isPlayerInPhase1 = false;
		m_playerAction = null;
		m_AiAction = null;
    }

	// -----------------------------------------------------------------------------
	void ResolveAttackPhase ( EBattleStates state )
	{
		bool isPlayer = state == EBattleStates.eAtkPhase1 && m_isPlayerInPhase1 || state == EBattleStates.eAtkPhase2 && !m_isPlayerInPhase1;
		CCombatAction action;
		Pokemon attackingPokemon;
		Pokemon defendingPokemon;

		if( isPlayer )
		{
			m_playerAttacking = true;
			action = m_playerAction;
			attackingPokemon = m_playerPokemon;
			defendingPokemon = m_opponentPokemon;
		}
		else
		{
			m_playerAttacking = false;
			action = m_AiAction;
			attackingPokemon = m_opponentPokemon;
			defendingPokemon = m_playerPokemon;
		}

		switch( action.GetActionType () )
		{
		case ECombatActionType.Run:
			{
				// Only player can run away for now
				m_playerPokeValues.fleeAttempts++;
				if( CanRunAway() )
				{
					StopFight(false, true);
				}
				else // if the run attempt fails, the turn is lost
				{
					GoToState (m_battleState+1);
				}
			}
			break;
		case ECombatActionType.Switch:
			{
				OnSwitch (isPlayer, (CSwitchAction) action); 
			}
			break;
		case ECombatActionType.Object:
			{
				// TODO
			}
			break;
		case ECombatActionType.Attack:
			{
				CAttackAction attackAction = (CAttackAction)action;
				Attack attack = FindObjectOfType<Attack> ();
				if( attack )
				{
					attack.StartAttack(attackAction.attackId, attackingPokemon, defendingPokemon); 
				}
			}
			break;
		default:
			break;
		}
	}

	// -----------------------------------------------------------------------------
	bool CanRunAway()
	{
		/*
		 * F = (( (A * 128) / B) + (30 * C)) mod256 
		 * where:
		 * A : unmodified speed of player's active Pokémon
		 * B : unmodified speed of wild Pokémon
		 * C : number of flee attempts (counting current one)
		 */

		int A = m_playerPokemon.m_vitesse [1];
		int B = m_opponentPokemon.m_vitesse [1];
		int C = m_playerPokeValues.fleeAttempts;

		if (B == 0)
			B = 1;

		int F = (A * 128 / B + 30 * C) % 256;
		int rand = Random.Range (0, 256);

		return rand < F;
	}

	// -----------------------------------------------------------------------------
    public bool OnPlayerAttackSelected(int moveId)
    {
        if( m_playerPokemon.m_ppCurrent[moveId] == 0 )
        {
            return false;
        }

		m_playerAction = new CAttackAction (moveId);
		GoToState (EBattleStates.eOrderOfActionSelection);

        return true;
    }

	// -----------------------------------------------------------------------------
    public void OnAttackResult(int dmg, int selfDamage)
    {
		// TODO : modify this function when integrating animation

        bool playerPokeKo = false;
        bool oppPokeKo = false;

		if( dmg > 0 )
		{
			m_blockedByAnimation = true;
		}

        if( m_playerAttacking )
        {
            oppPokeKo = ApplyDamage(m_opponentPokemon, dmg);
			m_battleInterfaceManager.m_healthBarOpponent.GetComponent<FilledBar>().SetFillAmount(m_opponentPokemon.m_currentPV, EFillSpeed.Fast);

			/* //TODO - Delay self damage animation (chained after the opponent bar anim)
            playerPokeKo = ApplyDamage(m_playerPokemon, selfDamage);
            m_battleInterfaceManager.m_healthBarPlayer.GetComponent<FilledBar>().SetFillAmount(m_playerPokemon.m_currentPV, EFillSpeed.Normal);
            */
        }
        else
        {
            playerPokeKo = ApplyDamage(m_playerPokemon, dmg);
            m_battleInterfaceManager.m_healthBarPlayer.GetComponent<FilledBar>().SetFillAmount(m_playerPokemon.m_currentPV, EFillSpeed.Normal);

			/*
            oppPokeKo = ApplyDamage(m_opponentPokemon, selfDamage);
            m_battleInterfaceManager.m_healthBarOpponent.GetComponent<FilledBar>().SetFillAmount(m_opponentPokemon.m_currentPV, EFillSpeed.Normal);
            */
        }

        if( playerPokeKo )
        {
            
        }

        if( oppPokeKo )
        {
            StopFight();
        }
    }

	// -----------------------------------------------------------------------------
    /*
     * Return true if the pokemon is KO
     */
    bool ApplyDamage( Pokemon pokemon, int dmg )
    {
        pokemon.m_currentPV -= dmg;
		if (pokemon.m_currentPV < 0)
			pokemon.m_currentPV = 0;
        Mathf.Clamp( pokemon.m_currentPV, 0, pokemon.m_pv[1] );
        return pokemon.m_currentPV == 0;
    }

	// -----------------------------------------------------------------------------
	public void OnLifeBarUpdateFinished()
	{
		m_blockedByAnimation = false;
		if( m_battleState == EBattleStates.eAtkPhase1 )
		{
			GoToState ( EBattleStates.eAtkPhase2 );
		}
		else if( m_battleState == EBattleStates.eAtkPhase2 )
		{
			GoToState ( EBattleStates.eEndTurn );
		}
	}

	// -----------------------------------------------------------------------------
	void OnSwitch (bool isPlayer, CSwitchAction action)
	{
		if( isPlayer )
		{
			m_playerPokeValues.Reset ();

			// TODO: switch
			//m_playerPokemon = action.replacingPokemonUniqueId
		}
		else
		{
			m_oppPokeValues.Reset ();

			// TODO: switch
		}
	}

	// -----------------------------------------------------------------------------
	void SelectOrderOfActions()
	{
		m_isPlayerInPhase1 = false;

		// TODO - if a wild pokemon can flee the fight without attacking (Safari Park for example), add the condition here;

		// Running from a fight is resolve before any other action in the turn.
		// If a player plays an object, it will be done before any action from the AI
		if( m_playerAction.GetActionType () == ECombatActionType.Run || m_playerAction.GetActionType () == ECombatActionType.Object )
		{
			m_isPlayerInPhase1 = true;
		}
		// A switch from the player will also be done before any action from the AI, but can be done after some special attacks
		else if( m_AiAction.GetActionType () == ECombatActionType.Switch )
		{
			// For now true. Cannot always be true with some attack like "Pursuit"
			m_isPlayerInPhase1 = true;
		}
		else if( m_AiAction.GetActionType () == ECombatActionType.Attack ) // if both pokemon attacks, select the fastest attack
		{
			CAttackAction playerAttackAction = (CAttackAction)m_playerAction;
			CAttackAction aiAttackAction = (CAttackAction)m_AiAction;
			int playerPriority = AttackDatabase.GetAttackById (playerAttackAction.attackId).m_priorite;
			int aiPriority = AttackDatabase.GetAttackById (aiAttackAction.attackId).m_priorite;

			// if higher attack priority for the player's pokemon
			if( playerPriority > aiPriority )
			{
				m_isPlayerInPhase1 = true;
			}
			else if( playerPriority == aiPriority )
			{
				int playerPokemonSpeed = GetPokemonSpeed (m_playerPokemon, true);
				int aiPokemonSpeed = GetPokemonSpeed (m_opponentPokemon, false);

				// If player's pokemon is faster
				if( playerPokemonSpeed > aiPokemonSpeed )
				{
					m_isPlayerInPhase1 = true;
				}
				else if( playerPokemonSpeed == aiPokemonSpeed ) // if both pokemon goes to the same speed, then select one at random to attack first
				{
					if( Random.Range (0, 2) == 0 )
					{
						m_isPlayerInPhase1 = true;
					}
				}
			}
		}
	}

	// -----------------------------------------------------------------------------
	int GetPokemonSpeed(Pokemon pokemon, bool isPlayerPokemon)
	{
		float statModificator = Attack.GetStatModificator (isPlayerPokemon ? m_playerPokeValues.vitesse : m_oppPokeValues.vitesse);
		int baseSpeed = pokemon.m_vitesse [1];
		bool isParalized = pokemon.m_physicalStatus == EPokemonPhysicalStatus.Para;

		int speed = (int)(statModificator * baseSpeed);

		if( isParalized ) {
			speed = (int)(speed - speed * 0.25f);
		}

		return speed;
	}

	// -----------------------------------------------------------------------------
	// --------------------------      AI      -------------------------------------
	// -----------------------------------------------------------------------------
	void SelectNextAIAction()
	{
		//TODO - for now, select attack at random

		// 1/ count number of attack available
		int numberAttack = 0;
		for( int i = 0; i < 4; i++ )
		{
			if( m_opponentPokemon.m_moveSet[i] != -1 )
			{
				numberAttack++;
			}
		}

		// 2/ select one at random
		int randAttackIndex = Random.Range (0, numberAttack);

		// 3/ push action
		m_AiAction = new CAttackAction(m_opponentPokemon.m_moveSet[randAttackIndex]) ;
	}
}
