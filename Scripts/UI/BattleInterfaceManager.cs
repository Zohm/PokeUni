using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Assertions;

public enum EBattleCategories
{
	Main,
	Fight,
	Bag,
	Pokemon
}

public enum EArrowPos
{
	TopLeft,
	TopRight,
	BotLeft,
	BotRight
}

public class BattleInterfaceManager : MonoBehaviour 
{
	const float DISPLAY_TIME = 1.5f;

    BattleManager m_battleManager = null;

	public GameObject m_battleBox;
	bool m_isInBattle = false;	//To know if should activate or not the battle interface
	EArrowPos m_arrowPos = EArrowPos.TopLeft;

//UI Elements
	// Sprites
	public Image m_spritePlayer;
	public Image m_spriteOpponent;

	// Pokemons Info
	public Text m_nameOpponent;
	public Text m_namePlayer;
	public Text m_levelOpponent;
	public Text m_levelPlayer;

	// Pokemons Life
	public GameObject m_healthBarOpponent;
	public GameObject m_healthBarPlayer;

	// Fight text
	public Text m_fightText;

	Queue m_textsToDisplay = null;
	Timer m_textTimer = null;

	// Categories' menu
	EBattleCategories m_currentMenu = EBattleCategories.Main;
	public GameObject m_categoryMenu;
	public GameObject m_categoryMenu_Arrow_Fight;
	public GameObject m_categoryMenu_Arrow_Bag;
	public GameObject m_categoryMenu_Arrow_Pokemon;
	public GameObject m_categoryMenu_Arrow_Run;

	//Fight menus
	public GameObject m_fightMenu;
		// Fight Menu Info
	public Text m_currentPP;
	public Text m_maxPP;
	public Text m_typeAtk;

		// Fight Menu moves
	public Text m_move1;
	public GameObject m_move1_arrow;
	public Text m_move2;
	public GameObject m_move2_arrow;
	public Text m_move3;
	public GameObject m_move3_arrow;
	public Text m_move4;
	public GameObject m_move4_arrow;

//Pokemon in battle
	private Pokemon m_opponentPokemon;
	private Pokemon m_playerPokemon;

//Battle Animation + texts
	public Queue m_combatActionQueue = null; // CUICombatAction & derivated

	//----------------------------------------------------------------------------
	// Use this for initialization
	void Start () 
	{
		m_textsToDisplay = new Queue();
		m_textTimer = new Timer();
		m_combatActionQueue = new Queue ();

        m_battleManager = FindObjectOfType<BattleManager> ();
        Assert.AreNotEqual (m_battleManager, null);

		Reset ();
	}

	//----------------------------------------------------------------------------
	void Init()
	{
		m_isInBattle = true;
		m_opponentPokemon = null;
		m_playerPokemon = null;

		m_battleBox.SetActive (true);
		InitMainMenu ();
	}

    void RunFromFight()
    {
        //TODO: only work if against a wild Pokémon

		m_battleManager.RunFromFight ();
    }
        
    public void StopFight()
    {
        Reset();
    }

    void Reset()
    {
        m_isInBattle = false;
        m_opponentPokemon = null;
        m_playerPokemon = null;
        m_battleBox.SetActive (false);
		m_fightText.text = "";

		m_textsToDisplay.Clear ();
		m_combatActionQueue.Clear ();
		m_textTimer.SetInfinite();
    }

	//----------------------------------------------------------------------------
	void InitMainMenu()
	{
		m_currentMenu = EBattleCategories.Main;
		m_arrowPos = EArrowPos.TopLeft;

		m_categoryMenu.SetActive (true);
		m_categoryMenu_Arrow_Fight.SetActive (true);
		m_categoryMenu_Arrow_Bag.SetActive (false);
		m_categoryMenu_Arrow_Pokemon.SetActive (false);
		m_categoryMenu_Arrow_Run.SetActive (false);

		m_fightMenu.SetActive (false);
		m_move1_arrow.SetActive (false);
		m_move2_arrow.SetActive (false);
		m_move3_arrow.SetActive (false);
		m_move4_arrow.SetActive (false);
	}

	//----------------------------------------------------------------------------
	// Update is called once per frame
	void Update () 
	{
		if( m_isInBattle && m_battleManager.m_canDoAction )
		{
			if (m_battleManager.m_battleState == BattleManager.EBattleStates.eSelection) 
			{
				//run AI

				//Update current menu display
				switch (m_currentMenu) {
				case EBattleCategories.Main:
					UpdateMainMenu ();
					break;
				case EBattleCategories.Fight:
					UpdateFightMenu ();
					break;
				case EBattleCategories.Bag:
					UpdateBagMenu ();
					break;
				case EBattleCategories.Pokemon:
					UpdatePokemonMenu ();
					break;
				default:
					break;
				}
			} 
			else if (m_battleManager.m_battleState == BattleManager.EBattleStates.eAtkPhase1) 
			{
				UpdateTextDisplay ();
			} 
			else if (m_battleManager.m_battleState == BattleManager.EBattleStates.eAtkPhase2) 
			{
				//UpdateTextDisplay ();
			} 
			else if (m_battleManager.m_battleState == BattleManager.EBattleStates.eEndTurn) 
			{
				
			}
		}
	}

	//----------------------------------------------------------------------------
	void UpdateMainMenu ()
	{
		if (Input.GetKeyUp (KeyCode.LeftArrow)) 
		{
			if( m_arrowPos == EArrowPos.TopRight )
			{
				m_arrowPos = EArrowPos.TopLeft;
				m_categoryMenu_Arrow_Bag.SetActive (false);
				m_categoryMenu_Arrow_Fight.SetActive (true);
			}
			else if( m_arrowPos == EArrowPos.BotRight )
			{
				m_arrowPos = EArrowPos.BotLeft;
				m_categoryMenu_Arrow_Run.SetActive (false);
				m_categoryMenu_Arrow_Pokemon.SetActive (true);
			}
		}
		else if(Input.GetKeyUp (KeyCode.RightArrow)) 
		{
			if( m_arrowPos == EArrowPos.TopLeft )
			{
				m_arrowPos = EArrowPos.TopRight;
				m_categoryMenu_Arrow_Fight.SetActive (false);
				m_categoryMenu_Arrow_Bag.SetActive (true);
			}
			else if( m_arrowPos == EArrowPos.BotLeft )
			{
				m_arrowPos = EArrowPos.BotRight;
				m_categoryMenu_Arrow_Pokemon.SetActive (false);
				m_categoryMenu_Arrow_Run.SetActive (true);
			}
		}
		else if(Input.GetKeyUp (KeyCode.UpArrow)) 
		{
			if( m_arrowPos == EArrowPos.BotLeft )
			{
				m_arrowPos = EArrowPos.TopLeft;
				m_categoryMenu_Arrow_Pokemon.SetActive (false);
				m_categoryMenu_Arrow_Fight.SetActive (true);
			}
			else if( m_arrowPos == EArrowPos.BotRight )
			{
				m_arrowPos = EArrowPos.TopRight;
				m_categoryMenu_Arrow_Run.SetActive (false);
				m_categoryMenu_Arrow_Bag.SetActive (true);
			}
		}
		else if(Input.GetKeyUp (KeyCode.DownArrow)) 
		{
			if( m_arrowPos == EArrowPos.TopLeft )
			{
				m_arrowPos = EArrowPos.BotLeft;
				m_categoryMenu_Arrow_Fight.SetActive (false);
				m_categoryMenu_Arrow_Pokemon.SetActive (true);
			}
			else if( m_arrowPos == EArrowPos.TopRight )
			{
				m_arrowPos = EArrowPos.BotRight;
				m_categoryMenu_Arrow_Bag.SetActive (false);
				m_categoryMenu_Arrow_Run.SetActive (true);
			}
		}
		else if(Input.GetKeyUp (KeyCode.A)) 
		{
			switch( m_arrowPos )
			{
    			case EArrowPos.TopLeft:
    				m_currentMenu = EBattleCategories.Fight;
    				InitFightMenu ();
    				break;
    			case EArrowPos.TopRight:
    				//m_currentMenu = EBattleCategories.Bag;
    				break;
    			case EArrowPos.BotLeft:
    				//m_currentMenu = EBattleCategories.Pokemon;
    				break;
                case EArrowPos.BotRight:
                    RunFromFight();
				break;
    			default:
    				break;
			}
		}
	}

	//----------------------------------------------------------------------------
	void InitFightMenu()
	{
		m_categoryMenu.SetActive (false);
		m_fightMenu.SetActive (true);
		m_move1_arrow.SetActive (true);
	}

	//----------------------------------------------------------------------------
	void UpdateFightMenu ()
	{
		if (Input.GetKeyUp (KeyCode.LeftArrow)) 
		{
            if( m_arrowPos == EArrowPos.TopRight )
			{
				m_arrowPos = EArrowPos.TopLeft;
				m_move2_arrow.SetActive (false);
				m_move1_arrow.SetActive (true);
				m_currentPP.text = m_playerPokemon.m_ppCurrent[0].ToString ();
				m_maxPP.text = m_playerPokemon.m_ppMax[0].ToString ();
				//m_typeAtk = m_playerPokemon.m_moveSet[0] != -1 ? AttackDatabase.GetAttackById (m_playerPokemon.m_moveSet[0]).m_type : "-";
			}
            else if( m_arrowPos == EArrowPos.BotRight && m_move3.text != "-" )
			{
				m_arrowPos = EArrowPos.BotLeft;
				m_move4_arrow.SetActive (false);
				m_move3_arrow.SetActive (true);
				m_currentPP.text = m_playerPokemon.m_ppCurrent[2].ToString ();
				m_maxPP.text = m_playerPokemon.m_ppMax[2].ToString ();
			}
		}
		else if(Input.GetKeyUp (KeyCode.RightArrow)) 
		{
            if( m_arrowPos == EArrowPos.TopLeft && m_move2.text != "-" )
			{
				m_arrowPos = EArrowPos.TopRight;
				m_move1_arrow.SetActive (false);
				m_move2_arrow.SetActive (true);
				m_currentPP.text = m_playerPokemon.m_ppCurrent[1].ToString ();
				m_maxPP.text = m_playerPokemon.m_ppMax[1].ToString ();
			}
            else if( m_arrowPos == EArrowPos.BotLeft && m_move4.text != "-" )
			{
				m_arrowPos = EArrowPos.BotRight;
				m_move3_arrow.SetActive (false);
				m_move4_arrow.SetActive (true);
				m_currentPP.text = m_playerPokemon.m_ppCurrent[3].ToString ();
				m_maxPP.text = m_playerPokemon.m_ppMax[3].ToString ();
			}
		}
		else if(Input.GetKeyUp (KeyCode.UpArrow)) 
		{
            if( m_arrowPos == EArrowPos.BotLeft )
			{
				m_arrowPos = EArrowPos.TopLeft;
				m_move3_arrow.SetActive (false);
				m_move1_arrow.SetActive (true);
				m_currentPP.text = m_playerPokemon.m_ppCurrent[0].ToString ();
				m_maxPP.text = m_playerPokemon.m_ppMax[0].ToString ();
			}
            else if( m_arrowPos == EArrowPos.BotRight && m_move2.text != "-" )
			{
				m_arrowPos = EArrowPos.TopRight;
				m_move4_arrow.SetActive (false);
				m_move2_arrow.SetActive (true);
				m_currentPP.text = m_playerPokemon.m_ppCurrent[1].ToString ();
				m_maxPP.text = m_playerPokemon.m_ppMax[1].ToString ();
			}
		}
		else if(Input.GetKeyUp (KeyCode.DownArrow)) 
		{
            if( m_arrowPos == EArrowPos.TopLeft && m_move3.text != "-" )
			{
				m_arrowPos = EArrowPos.BotLeft;
				m_move1_arrow.SetActive (false);
				m_move3_arrow.SetActive (true);
				m_currentPP.text = m_playerPokemon.m_ppCurrent[2].ToString ();
				m_maxPP.text = m_playerPokemon.m_ppMax[2].ToString ();
			}
            else if( m_arrowPos == EArrowPos.TopRight && m_move4.text != "-" )
			{
				m_arrowPos = EArrowPos.BotRight;
				m_move2_arrow.SetActive (false);
				m_move4_arrow.SetActive (true);
				m_currentPP.text = m_playerPokemon.m_ppCurrent[3].ToString ();
				m_maxPP.text = m_playerPokemon.m_ppMax[3].ToString ();
			}
		}
		else if(Input.GetKeyUp (KeyCode.A)) 
		{
            int moveId = 0;
			switch( m_arrowPos )
			{
                case EArrowPos.TopLeft:
                    moveId = 0;
				break;
    			case EArrowPos.TopRight:
                    moveId = 1;
    				break;
    			case EArrowPos.BotLeft:
                    moveId = 2;
    				break;
    			case EArrowPos.BotRight:
                    moveId = 3;
    				break;
                default:
    				break;
			}

            if( m_battleManager.OnPlayerAttackSelected(moveId) )
            {
                //Temp
                InitMainMenu ();
            }
		}
		else if(Input.GetKeyUp (KeyCode.B))
		{
			InitMainMenu ();
		}
	}

	//----------------------------------------------------------------------------
	void UpdateBagMenu ()
	{
		//TODO
	}

	//----------------------------------------------------------------------------
	void UpdatePokemonMenu ()
	{
		//TODO
	}

	//----------------------------------------------------------------------------
	void InitBattlePokemons(Pokemon player, Pokemon opponent)
	{
		// Sprites
		m_spritePlayer.sprite = player.m_sprite_fight_back;
		m_spriteOpponent.sprite = opponent.m_sprite_fight_face;

		// Pokemons Info
		m_nameOpponent.text = opponent.m_name;
		m_namePlayer.text = player.m_name;
		m_levelOpponent.text = opponent.m_level.ToString ();
		m_levelPlayer.text = player.m_level.ToString ();

		// Pokemons Life
		//m_remainingLife.text = player.m_currentPV.ToString ();
		//m_maxLife.text = player.m_pv[(int)EStatType.Current].ToString ();

		// Fight Menu Info
		m_currentPP.text = player.m_ppCurrent[0].ToString ();
		m_maxPP.text = player.m_ppMax[0].ToString ();
		//public Text m_typeAtk;

		// Fight Menu moves
		m_move1.text = player.m_moveSet[0] != -1 ? AttackDatabase.GetAttackById (player.m_moveSet[0]).m_name : "-";
		m_move2.text = player.m_moveSet[1] != -1 ? AttackDatabase.GetAttackById (player.m_moveSet[1]).m_name : "-";
		m_move3.text = player.m_moveSet[2] != -1 ? AttackDatabase.GetAttackById (player.m_moveSet[2]).m_name : "-";
		m_move4.text = player.m_moveSet[3] != -1 ? AttackDatabase.GetAttackById (player.m_moveSet[3]).m_name : "-";

		//m_typeAtk = player.m_moveSet[0] != -1 ? AttackDatabase.GetAttackById (player.m_moveSet[0]).m_name : "-";

		//Pokemon in battle
		m_opponentPokemon = opponent;
		m_playerPokemon = player;
	}

	//----------------------------------------------------------------------------
    public void StartFight(Pokemon player, Pokemon opponent)
	{
        Init ();
        InitBattlePokemons (player , opponent);

		FilledBar playerHealthBar = m_healthBarPlayer.GetComponent<FilledBar>();
		if( playerHealthBar )
		{
			playerHealthBar.InitBar (m_playerPokemon.m_currentPV, m_playerPokemon.m_pv[1]);
		}

		FilledBar opponentHealthBar = m_healthBarOpponent.GetComponent<FilledBar>();
		if( opponentHealthBar )
		{
			opponentHealthBar.InitBar (m_opponentPokemon.m_currentPV, m_opponentPokemon.m_pv[1]);
		}
	}

	public void AddTextToDiplay(string text)
	{
		m_textsToDisplay.Enqueue (text);
	}

	void StartNextTextDisplay()
	{
		if( m_textsToDisplay.Count > 0 )
		{
			m_fightText.text = m_textsToDisplay.Dequeue ().ToString ();
			m_textTimer.SetLifeTime (DISPLAY_TIME);
		}
		else
		{
			m_textTimer.SetInfinite ();
			//TODO: warn can pass to next phase of combat
		}
	}

	void UpdateTextDisplay()
	{
		if( m_textTimer.HasExpired () )
		{
			StartNextTextDisplay ();
		}
	}


	/* //TEMP - TEST
	FilledBar barComp = new FilledBar ();
	if( barComp = m_healthBarPlayer.GetComponent<FilledBar>() )
	{
		if( m_playerPokemon.m_currentPV >= 50 )
			m_playerPokemon.m_currentPV -= 50;
		else
			m_playerPokemon.m_currentPV = 0;
		barComp.SetFillAmount(m_playerPokemon.m_currentPV, EFillSpeed.Fast);
	}
	*/
}
