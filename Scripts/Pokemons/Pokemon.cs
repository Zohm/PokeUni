using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pokemon : ScriptableObject 
{
	uint m_uniqueRandId;
	int m_index = 0;
	public string m_name = "";
	public string m_nickname = "";
	public float m_size = 0f;
	public float m_weight = 0f;

	public float m_tauxCapture = 0f;
	public float m_ratioMale = 0f;

	public EPokemonType m_type1 = EPokemonType.Default;
	public EPokemonType m_type2 = EPokemonType.Default;

	// TODO indexes = 0: base stat; 1: current stat; 2: IV; 3: EV; 4: Given EV when defeated
	public int[] m_pv 		= new int[5];
	public int[] m_atk 		= new int[5];
	public int[] m_def 		= new int[5];
	public int[] m_vitesse 	= new int[5];
	public int[] m_atkspe 	= new int[5];
	public int[] m_defspe 	= new int[5];

	ECourbesExperience m_courbeEvolution;
	int m_baseXp;
	int m_evolution;
	int m_lvlEvolution;

	// TODO sprites (world + combat + icone menu)
	public Sprite m_sprite_fight_face;
	public Sprite m_sprite_fight_back;

	EPokemonNatures m_nature = EPokemonNatures.Default;
	// TODO sexe

	public int m_level 		= 1;
	public int m_currentXp 	= 0;
	public int m_nextLvlXp 	= 0;

	// TODO ? Keep attacks in local memory
	public int[] m_moveSet 		= new int[4];
	public int[] m_ppMax 		= new int[4];
	public int[] m_ppCurrent 	= new int[4];

	// TODO Objet tenu
	// TODO bonheur
	// TODO shiney

	public int m_currentPV = 0;
	public EPokemonPhysicalStatus m_physicalStatus = EPokemonPhysicalStatus.Default;
	// TODO taux critique
	// TODO ...

    public void GeneratePokemon(int indexPokemon, int lvl)
	{
        GeneratePokemon (indexPokemon, lvl, lvl);
	}

	//-----------------------------------------------------------------------------------------------------------
    public void GeneratePokemon(int indexPokemon, int lvlMin, int lvlMax)
	{
		PokemonDatabase.PokemonItem pokemonDb = PokemonDatabase.GetPokemonByIndex (indexPokemon);
		if( pokemonDb != null )
		{
			// TODO Generate m_uniqueRandId
			m_index = pokemonDb.m_index;
			m_name = pokemonDb.m_name;
			m_type1 = pokemonDb.m_type1;
			m_type2 = pokemonDb.m_type2;
			m_size = pokemonDb.m_size;
			m_weight = pokemonDb.m_weight;

			m_tauxCapture = pokemonDb.m_tauxCapture;
			m_ratioMale = pokemonDb.m_ratioMale;

			// Sprites
			m_sprite_fight_face = pokemonDb.m_sprite_fight_face;
			m_sprite_fight_back = pokemonDb.m_sprite_fight_back;

			// Experience
			m_courbeEvolution = pokemonDb.m_courbeExp;
			m_baseXp = pokemonDb.m_baseXp;
			m_evolution = pokemonDb.m_evolution;
			m_lvlEvolution = pokemonDb.m_lvlEvolution;

			m_level = Random.Range (lvlMin, lvlMax + 1);	//min inclusive, max exclusive. Hence the max+1
			m_nature = (EPokemonNatures)Random.Range (1, (int)EPokemonNatures.MAX);

			//Init base stats
			m_pv 		[0] = pokemonDb.m_pv;
			m_atk 		[0] = pokemonDb.m_atk;
			m_def 		[0] = pokemonDb.m_def;
			m_vitesse 	[0] = pokemonDb.m_vitesse;
			m_atkspe 	[0] = pokemonDb.m_atkspe;
			m_defspe 	[0] = pokemonDb.m_defspe;

			//Init EV given stats
			m_pv 		[4] = pokemonDb.m_pv_given;
			m_atk 		[4] = pokemonDb.m_atk_given;
			m_def 		[4] = pokemonDb.m_def_given;
			m_vitesse 	[4] = pokemonDb.m_vitesse_given;
			m_atkspe 	[4] = pokemonDb.m_atkspe_given;
			m_defspe 	[4] = pokemonDb.m_defspe_given;

			InitExperience ();
			GenerateStats ();

			// full life on generation
			m_currentPV = m_pv [1];

			InitAttacks (pokemonDb);
		}
	}

	//-----------------------------------------------------------------------------------------------------------
	//-------------------------------------          EXPERIENCE 				 --------------------------------
	//-----------------------------------------------------------------------------------------------------------
	void InitExperience()
	{
		switch( m_courbeEvolution )
		{
		case ECourbesExperience.Rapide:
			{
				m_currentXp = (int) (0.8f * Mathf.Pow ((float)m_level, 3f)); 
				m_nextLvlXp = (int) (0.8f * Mathf.Pow ((float)m_level+1, 3f)); 
				break;
			}
		case ECourbesExperience.Moyenne:
			{
				m_currentXp = (int) Mathf.Pow ((float)m_level, 3f); 
				m_nextLvlXp = (int) Mathf.Pow ((float)m_level+1, 3f); 
				break;
			}
		case ECourbesExperience.Parabolique:
			{
				m_currentXp = (int)(1.2f * Mathf.Pow ((float)m_level, 3f) - 15f * Mathf.Pow ((float)m_level, 2f) + 100f * m_level - 140f);
				m_nextLvlXp = (int)(1.2f * Mathf.Pow ((float)m_level+1, 3f) - 15f * Mathf.Pow ((float)m_level+1, 2f) + 100f * (m_level+1) - 140f);
				break;
			}
		case ECourbesExperience.Lente:
			{
				m_currentXp = (int) (1.25f * Mathf.Pow ((float)m_level, 3f)); 
				m_nextLvlXp = (int) (1.25f * Mathf.Pow ((float)m_level+1, 3f)); 
				break;
			}
		case ECourbesExperience.Erratique:
			{
				m_currentXp = (int) GetExpAtLvlCourbeErratique (m_level);
				m_nextLvlXp = (int) GetExpAtLvlCourbeErratique (m_level+1);
				break;
			}
		case ECourbesExperience.Fluctuante:
			{
				m_currentXp = (int) GetExpAtLvlCourbeFluctuante (m_level);
				m_nextLvlXp = (int) GetExpAtLvlCourbeFluctuante (m_level+1);
				break;
			}
		default:
			break;
		}
	}

	//-----------------------------------------------------------------------------------------------------------
	float GetExpAtLvlCourbeErratique(int N)
	{
		if( N <= 50 )
		{
			return Mathf.Pow ((float)N, 3f) * (100f-N) * 0.02f;
		}
		else if( N <= 68 )
		{
			return Mathf.Pow ((float)N, 3f) * (150f-N) * 0.01f;
		}
		else if( N <= 98 )
		{
			int mod = N % 3;
			float p = 0f;
			if (mod == 1)
				p = 0.008f;
			else if (mod == 2)
				p = 0.014f;
			
			return Mathf.Pow ((float)N, 3f) * (1.274f - (0.02f*N/3) - p);
		}
		else if( N < 100 )
		{
			return Mathf.Pow ((float)N, 3f) * (160f-N) * 0.01f;
		}

		return 0;
	}

	//-----------------------------------------------------------------------------------------------------------
	float GetExpAtLvlCourbeFluctuante(int N)
	{
		if( N <= 15 )
		{
			return Mathf.Pow ((float)N, 3f) * (24f + ((N+1)/3f)) * 0.02f;
		}
		else if( N <= 35 )
		{
			return Mathf.Pow ((float)N, 3f) * (14f + N) * 0.02f;
		}
		else if( N < 100 )
		{
			return Mathf.Pow ((float)N, 3f) * (32f + (N/2)) * 0.02f;
		}

		return 0;
	}

	//-----------------------------------------------------------------------------------------------------------
	void AddExperienceGainedFromKo(Pokemon defeatedPokemon)
	{
		m_currentXp += /* a * (= bonus multiplicateur) */ defeatedPokemon.m_baseXp * defeatedPokemon.m_level / 7;
	}

	//-----------------------------------------------------------------------------------------------------------
	//-------------------------------------          STATS 				 ----------------------------------------
	//-----------------------------------------------------------------------------------------------------------
	void GenerateStats()
	{
		// IV => valeur aléatoire entre [0,31]
		m_pv 		[2] = Random.Range (0, 32);
		m_atk 		[2] = Random.Range (0, 32);
		m_def 		[2] = Random.Range (0, 32);
		m_vitesse 	[2] = Random.Range (0, 32);
		m_atkspe 	[2] = Random.Range (0, 32);
		m_defspe 	[2] = Random.Range (0, 32);

		// EV => On generation, all value are set to 0
		m_pv 		[3] = 0;
		m_atk 		[3] = 0;
		m_def 		[3] = 0;
		m_vitesse 	[3] = 0;
		m_atkspe 	[3] = 0;
		m_defspe 	[3] = 0;

		CalculatePV ();
		CalculateStats ();
		ApplyNatureEffect ();
	}

	//-----------------------------------------------------------------------------------------------------------
	void CalculatePV ()
	{
		// (((2 * BASE + IV + EV/4) * LEVEL) / 100) + LEVEL + 10
		m_pv [1] = (int) (Mathf.Floor (((m_pv [2] + 2f * m_pv [0] + m_pv [3] * 0.25f) * m_level * 0.01f) + m_level + 10f));
	}

	//-----------------------------------------------------------------------------------------------------------
	void CalculateStats ()
	{
		// [(((2 * BASE + IV + EV/4) * LEVEL) /100)) + 5] * NATURE
		m_atk 		[1] = (int) (Mathf.Floor (((m_atk [2] + 2f * m_atk [0] + m_atk [3] * 0.25f) 			 * m_level * 0.01f) + 5f));
		m_def 		[1] = (int) (Mathf.Floor (((m_def [2] + 2f * m_def [0] + m_def [3] * 0.25f) 			 * m_level * 0.01f) + 5f));
		m_vitesse 	[1] = (int) (Mathf.Floor (((m_vitesse [2] + 2f * m_vitesse [0] + m_vitesse [3] * 0.25f)  * m_level * 0.01f) + 5f));
		m_atkspe 	[1] = (int) (Mathf.Floor (((m_atkspe [2] + 2f * m_atkspe [0] + m_atkspe [3] * 0.25f) 	 * m_level * 0.01f) + 5f));
		m_defspe 	[1] = (int) (Mathf.Floor (((m_defspe [2] + 2f * m_defspe [0] + m_defspe [3] * 0.25f) 	 * m_level * 0.01f) + 5f));
	}

	//-----------------------------------------------------------------------------------------------------------
	void ApplyNatureEffect ()
	{
		// ATK
		if ( m_nature == EPokemonNatures.Brave || m_nature == EPokemonNatures.Mauvais || m_nature == EPokemonNatures.Rigide || m_nature == EPokemonNatures.Solo )
			m_atk [1] = (int) (m_atk [1] * 1.1f);
		else if ( m_nature == EPokemonNatures.Assure || m_nature == EPokemonNatures.Calme || m_nature == EPokemonNatures.Modeste || m_nature == EPokemonNatures.Timide )
			m_atk [1] = (int) (m_atk [1] * 0.9f);

		// DEF
		if ( m_nature == EPokemonNatures.Lache || m_nature == EPokemonNatures.Malin || m_nature == EPokemonNatures.Relax || m_nature == EPokemonNatures.Assure )
			m_def [1] = (int) (m_def [1] * 1.1f);
		else if ( m_nature == EPokemonNatures.Solo || m_nature == EPokemonNatures.Doux || m_nature == EPokemonNatures.Gentil || m_nature == EPokemonNatures.Presse )
			m_def [1] = (int) (m_def [1] * 0.9f);

		// ATKSPE
		if ( m_nature == EPokemonNatures.Discret || m_nature == EPokemonNatures.Foufou || m_nature == EPokemonNatures.Modeste || m_nature == EPokemonNatures.Doux )
			m_atkspe [1] = (int) (m_atkspe [1] * 1.1f);
		else if ( m_nature == EPokemonNatures.Malin || m_nature == EPokemonNatures.Rigide || m_nature == EPokemonNatures.Jovial || m_nature == EPokemonNatures.Prudent )
			m_atkspe [1] = (int) (m_atkspe [1] * 0.9f);

		// DEFSPE
		if ( m_nature == EPokemonNatures.Malpoli || m_nature == EPokemonNatures.Calme || m_nature == EPokemonNatures.Gentil || m_nature == EPokemonNatures.Prudent )
			m_defspe [1] = (int) (m_defspe [1] * 1.1f);
		else if ( m_nature == EPokemonNatures.Foufou || m_nature == EPokemonNatures.Lache || m_nature == EPokemonNatures.Mauvais || m_nature == EPokemonNatures.Naif )
			m_defspe [1] = (int) (m_defspe [1] * 0.9f);

		// VITESS
		if ( m_nature == EPokemonNatures.Timide || m_nature == EPokemonNatures.Presse || m_nature == EPokemonNatures.Jovial || m_nature == EPokemonNatures.Naif )
			m_vitesse [1] = (int) (m_vitesse [1] * 1.1f);
		else if ( m_nature == EPokemonNatures.Malpoli || m_nature == EPokemonNatures.Discret || m_nature == EPokemonNatures.Relax || m_nature == EPokemonNatures.Brave )
			m_vitesse [1] = (int) (m_vitesse [1] * 0.9f);
	}

	//-----------------------------------------------------------------------------------------------------------
	//-------------------------------------          ATTACKS 			 ----------------------------------------
	//-----------------------------------------------------------------------------------------------------------
	void InitAttacks(PokemonDatabase.PokemonItem pokemonDb)
	{
		List<PokemonDatabase.PokemonAttack> pokeAttacks = pokemonDb.m_attacksByLvl;

		// Get Moveset && attacks info
		int i = 0;
		while( i < pokeAttacks.Count && pokeAttacks[i].m_level <= m_level )
		{
			i++;
		}

		for(int j = 0; j < 4; j++)
		{
			if( i > 0 )
			{
				// Fill attack id
				m_moveSet[j] = pokeAttacks[j].m_attackId;

				// Get attack's info
				AttackDatabase.AttackItem attack = AttackDatabase.GetAttackById (pokeAttacks[j].m_attackId);
				m_ppMax[j] = attack.m_pp;
				m_ppCurrent[j] = attack.m_pp;

				i--;
			}
			else
			{
				m_moveSet [j] = -1;
			}
		}
	}
}
