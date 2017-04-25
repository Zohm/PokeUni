using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Should be divided in BattleManager & attack 
 */

public class Attack : MonoBehaviour
{
	List<AttackDatabase.AttackItem> m_attackList = null; 

    BattleManager m_battleManager = null;

	void Start()
	{
		m_battleManager = FindObjectOfType<BattleManager> ();
		m_attackList = AttackDatabase.GetAttackList ();
	}

    BattleInfo.PokemonBattleValues GetSelfPokemonBattleValues()
	{
        return IsPlayerAttacking() ? m_battleManager.m_playerPokeValues : m_battleManager.m_oppPokeValues;
	}

    BattleInfo.PokemonBattleValues GetOppPokemonBattleValues()
	{
        return IsPlayerAttacking() ? m_battleManager.m_oppPokeValues : m_battleManager.m_playerPokeValues;
	}

    bool IsPlayerAttacking()
    {
        return m_battleManager.m_playerAttacking;
    }

	public void StartAttack(int atkId, Pokemon atackingPokemon, Pokemon defendingPokemon)
	{
		if(!IsBLockedByPhysicalStatus(atackingPokemon))
		{
			if(!IsBLockedByMentalStatus(atackingPokemon))
			{
				if (atkId < m_attackList.Count) 
				{
					AttackDatabase.AttackItem executedAtk = m_attackList [atkId];

                    //If the attack launched is Copie, and a copy has already be done, use the copied attack instead
                    if( executedAtk.m_name == "Copie" && GetSelfPokemonBattleValues().attackCopy.m_name != "" )
                    {
                        if( GetSelfPokemonBattleValues().attackCopy.m_index < m_attackList.Count )
                        {
                            executedAtk = GetSelfPokemonBattleValues().attackCopy;
                        }
                    }

                    if( executedAtk.m_name == "Mimique" )
                    {
                        //TODO: add attacks impossible to copy from generation > 1
                        //TODO: Create a function "CanLaunchMimique()"
                        if( GetOppPokemonBattleValues().m_lastAttackInfo.attack.m_name != "Mimique" ) 
                        {
                            executedAtk = GetOppPokemonBattleValues().m_lastAttackInfo.attack;
                        }
                    }

                    //Does attack launch 
					if (IsAttackLanding (executedAtk, atackingPokemon, defendingPokemon)) 
					{
						DoAttack (executedAtk, atackingPokemon, defendingPokemon);
					}
                    else //else if it failed to touch; some special case must be handled
					{
						switch(executedAtk.m_name)
						{
						case "Pied Sauté":
						case "Pied Voltige":
							{
								//dmgSelf = atackingPokemon.maxLife / 2;
							}
							break;
						default:
							break;
						}
					}
				}
			}
		}
	}

	bool IsBLockedByPhysicalStatus(Pokemon atackingPokemon)
	{
		switch(atackingPokemon.m_physicalStatus)
		{
		case EPokemonPhysicalStatus.Gele:
			{
				//20% chance by turn to get out of freeze
				if( Random.Range (0f, 1f) >= 0.8f )
				{
					atackingPokemon.m_physicalStatus = EPokemonPhysicalStatus.Default;
					return false;
				}
				else
				{
					return true;
				}
			}
		case EPokemonPhysicalStatus.Para:
			{
				return Random.Range (0f, 1f) <= 0.25f;
			}
		case EPokemonPhysicalStatus.Dodo:
			// TODO
			return true;
		default:
			return false;
		}
	}

	bool IsBLockedByMentalStatus(Pokemon atackingPokemon)
	{
		//TODO
		// isFlinched

        switch( GetSelfPokemonBattleValues().mentalStatus )
		{
		case EPokemonMentalStatus.Confus:
			{
				// TODO: dmgs
				// TODO: reduce duration 1 turn. If turn == 0, no more confused
				return true;
			}
		case EPokemonMentalStatus.Love:
			return Random.Range (0f, 1f) > 0.5f;
		default:
			return false;
		}
	}

	bool IsAttackLanding (AttackDatabase.AttackItem executedAtk, Pokemon atackingPokemon, Pokemon defendingPokemon)
	{
		//TODO
		//case "Fatal-Foudre":
        //case Blizzard

        int precisionMod = GetSelfPokemonBattleValues().precision;
        int esquiveMod = GetOppPokemonBattleValues().esquive;

        if( executedAtk.m_precision == -1 )	//Attacks with no precision always land
		{
			return true;
		}
        else if( executedAtk.m_precision == 0 ) //Special cases. Exemple: Abime
		{
            if( executedAtk.m_name == "E-Coque" || executedAtk.m_name == "Soin" || executedAtk.m_name == "Repos" )
            {
                //Already full life
                if( atackingPokemon.m_currentPV == atackingPokemon.m_pv[1] )
                {
                    return false;
                }
            }
                
            // OHKO attacks
            if( executedAtk.m_name == "Empal'Korne" || executedAtk.m_name == "Guillotine" || executedAtk.m_name == "Abîme" )
			{
				int levelDifference = atackingPokemon.m_level - defendingPokemon.m_level;
				if( levelDifference < 0 ) //OHKO attack automaticaly fails if the lvl of the attacker is less than the defender's
				{
					return false;
				}
				else //else 30% + 1% by level above defender
				{
					return Random.Range (0f, 100f) <= (float)(30 + levelDifference);
				}
			}

            if( executedAtk.m_name == "Dévorêve" )
            {
                if( defendingPokemon.m_physicalStatus == EPokemonPhysicalStatus.Dodo )
                {
                    float atkPrecisionValue = executedAtk.m_precision * GetPrecisionModificator (true, precisionMod) / GetPrecisionModificator (false, esquiveMod);
                    return Random.Range (0f, 100f) <=  atkPrecisionValue;
                }
                else
                {
                    return false;
                }
            }

			// TODO
			//...

			return true;
		}
		else   //Normal case. Landing depends on attacking pokemon's and attack's precision and defending pokemon's esquive
		{
            float atkPrecisionValue = (int) ( executedAtk.m_precision * GetPrecisionModificator (true, precisionMod) / GetPrecisionModificator (false, esquiveMod) );
			return Random.Range (0, 100) < atkPrecisionValue;
		}
	}

	float GetPrecisionModificator(bool isPrecision, int cran)
	{
		if( !isPrecision )
		{
			cran = -cran;
		}

		switch(cran)
		{
		case -6:
			return 0.33f;
		case -5:
			return 0.375f;
		case -4:
			return 0.43f;
		case -3:
			return 0.5f;
		case -2:
			return 0.6f;
		case -1:
			return 0.75f;
		case 0:
			return 1f;
		case 1:
			return 1.33f;
		case 2:
			return 1.67f;
		case 3:
			return 2f;
		case 4:
			return 2.33f;
		case 5:
			return 2.67f;
		case 6:
			return 3f;
		default:
			return 1f;
		}
	}

	void DoAttack(AttackDatabase.AttackItem executedAtk, Pokemon atackingPokemon, Pokemon defendingPokemon)
	{
		int level = (int) Mathf.Floor((atackingPokemon.m_level * 0.4f) + 2);

		// Puissance = Helping hand * Base Power * item * chargeur * mud * water * atker ability * defender ability
		// TODO: For now => Puissance = Base Power
		int puissance = GetPuissanceValue(executedAtk, defendingPokemon);

		// Is it a critical atack, and if so, does the pokemon has the sniper capacity
		int cc = GetCriticalValue(executedAtk);

		// atkValue = pokemon stat * stat modifier * ability modifier * item modifier
		// TODO: For now => atkValue = pokemon stat * stat modifier
        int atkValue = GetAtkValue(executedAtk, atackingPokemon, cc != 1);

		// defValue = pokemon stat * stat modifier * destruction or explosion mod * mod
		// TODO: For now => defValue = pokemon stat
		int defValue = GetDefValue(executedAtk, defendingPokemon, cc != 1);

		// mod1 = Burn * reflect * TVT ( combat double ) * meteo * torche
		// TODO: For now => mod1 = 
		int mod1 = 1;

		// mod2 = orbe de vie  / metronome / moi d'abord
		// TODO
		int mod2 = 1;

		// mod3 = solid rock * expert belt * tinted lens * berry
		// TODO
		int mod3 = 1;

		// random part of the formula
		int R = GetRandValueDamageFormula();

		// atk of same type as pokemon increase in force.
		float stab = GetStabValue(executedAtk, atackingPokemon);

		//type1 can be override by adaptability
        EPokemonType overrideType1 = GetOppPokemonBattleValues().overrideType1;
        EPokemonType defType1 = overrideType1 == EPokemonType.Default ? defendingPokemon.m_type1 : overrideType1;

		float type1 = GetAttackEffectOnType(executedAtk.m_type, defType1);
		float type2 = GetAttackEffectOnType(executedAtk.m_type, defendingPokemon.m_type2);

		int dmg = 0;
		int BASE = (((((level * puissance * atkValue / 50) / defValue) * mod1) + 2) * cc * mod2 * R / 100);
		dmg = (int) Mathf.Floor((int) Mathf.Floor((int) Mathf.Floor(BASE * stab) * type1) * type2) * mod3;

        int selfDamage = 0; //TODO: ajouter type of self damages (eg: Dégats de recul).
        ApplySecondaryEffect (executedAtk, atackingPokemon, defendingPokemon, ref dmg, ref puissance, ref selfDamage);

        m_battleManager.OnAttackResult(dmg, selfDamage);
	}

	// Puissance = Helping hand * Base Power * item * chargeur * mud * water * atker ability * defender ability
	// TODO: For now => Puissance = Base Power
	int GetPuissanceValue(AttackDatabase.AttackItem executedAtk, Pokemon defendingPokemon)
	{
		switch(executedAtk.m_name)
		{
		case "Balayage":
			{
				return GetPuissanceBasedOnOppWeight (defendingPokemon.m_weight);
			}
		default:
			return (int) Mathf.Floor(executedAtk.m_puissance);
		}
	}

	// atkValue = pokemon stat * stat modifier * ability modifier * item modifier
	// TODO: For now => atkValue = pokemon stat * stat modifier
    int GetAtkValue(AttackDatabase.AttackItem executedAtk, Pokemon atackingPokemon, bool isCritical)
	{
		int stat = 0;
		if( executedAtk.m_attackType == EAttackType.Physical )
		{
            stat = (int) Mathf.Floor(atackingPokemon.m_atk[1] * GetAtkStatModificator(isCritical));
		}
		else if( executedAtk.m_attackType == EAttackType.Special )
		{
            stat = (int) Mathf.Floor(atackingPokemon.m_atkspe[1] * GetAtkSpeStatModificator(isCritical));
		}
		return stat;
	}

    int GetDefValue(AttackDatabase.AttackItem executedAtk, Pokemon defendingPokemon, bool isCritical)
    {
        int stat = 0;
        if( executedAtk.m_attackType == EAttackType.Physical )
        {
            stat = (int) Mathf.Floor(defendingPokemon.m_def[1] * GetDefStatModificator(isCritical));
        }
        else if( executedAtk.m_attackType == EAttackType.Special )
        {
            stat = (int) Mathf.Floor(defendingPokemon.m_defspe[1] * GetDefSpeStatModificator(isCritical));
        }

        if(stat == 0)
        {
            stat = 1;
        }
        return stat;
    }

    /*
     * When a critical hit happens, if atk notch is less than 0, it is set back to 0. Positive notches are kept 
     */
    float GetAtkStatModificator(bool isCritical)
	{
		int cran = GetSelfPokemonBattleValues().atk;
        if( isCritical )
        {
            cran = cran < 0 ? 0 : cran;
        }
		return GetStatModificator (cran);
	}

    float GetAtkSpeStatModificator(bool isCritical)
	{
		int cran = GetSelfPokemonBattleValues().atkspe;
        if( isCritical )
        {
            cran = cran < 0 ? 0 : cran;
        }
		return GetStatModificator (cran);
	}

    /*
     * When a critical hit happens, if def notch is more than 0, it is set back to 0. Negative notches are kept 
     */
	float GetDefStatModificator(bool isCritical)
	{		
        int cran = GetSelfPokemonBattleValues().def;
        if( isCritical )
        {
            cran = cran > 0 ? 0 : cran;
        }		
		return GetStatModificator (cran);
	}

    float GetDefSpeStatModificator(bool isCritical)
	{
		int cran = GetSelfPokemonBattleValues().defspe;
        if( isCritical )
        {
            cran = cran > 0 ? 0 : cran;
        }       
		return GetStatModificator (cran);
	}

	public static float GetStatModificator(int cran)
	{
		switch(cran)
		{
		case -6:
			return 0.25f;
		case -5:
			return 0.286f;
		case -4:
			return 0.3f;
		case -3:
			return 0.4f;
		case -2:
			return 0.5f;
		case -1:
			return 0.666f;
		case 0:
			return 1f;
		case 1:
			return 1.5f;
		case 2:
			return 2f;
		case 3:
			return 2.5f;
		case 4:
			return 3f;
		case 5:
			return 3.5f;
		case 6:
			return 4f;
		default:
			return 1f;
		}
	}

	bool IsChangeStatPossible(int proba)
	{
		bool statChangeBlocked = GetOppPokemonBattleValues().isStatChangesFromOppBlocked;
		return !statChangeBlocked && Random.Range (0, 100) < proba;
	}

    /*
     * @Return the real number of notch of the modification 
     * (eg: max notch being 6, if the modification is of +2 and the current level is 5, the modification will only be 1)
     */
	void ApplyStatModificator(ref int stat, int modificator)
	{
		stat += modificator;
		if( stat > 6 )
		{
			stat = 6;
		}
		else if( stat < -6 )
		{
			stat = -6;
		}
	}

	int GetRandValueDamageFormula()
	{
		float rand = Random.Range (0f, 100f);
		if (rand <= 7.69f)
			return 85;
		else if (rand <= 15.38f)
			return 87;
		else if (rand <= 23.07f)
			return 89;
		else if (rand <= 30.76f)
			return 90;
		else if (rand <= 38.45f)
			return 92;
		else if (rand <= 46.14f)
			return 94;
		else if (rand <= 53.83f)
			return 96;
		else if (rand <= 61.52f)
			return 98;
		else if (rand <= 66.65f)
			return 86;
		else if (rand <= 71.78f)
			return 88;
		else if (rand <= 76.91f)
			return 91;
		else if (rand <= 82.04f)
			return 93;
		else if (rand <= 87.17f)
			return 95;
		else if (rand <= 92.30f)
			return 97;
		else if (rand <= 97.43f)
			return 99;
		else
			return 100;
	}

	int GetCriticalValue(AttackDatabase.AttackItem executedAtk)
	{
		float rand = Random.Range (0f, 100f);
        int criticalStage = GetSelfPokemonBattleValues().criticalStage ;

        if( executedAtk.m_name == "Pince-Masse" || executedAtk.m_name == "Poing Karaté" || executedAtk.m_name == "Tranche" || executedAtk.m_name == "Tranch'Herbe" )
		{
			criticalStage++;
		}

		bool isCrit = false;
		if (criticalStage == 0)
			isCrit = rand <= 6.25f;
		else if (criticalStage == 1)
			isCrit = rand <= 12.5f;
		else if (criticalStage == 2)
			isCrit = rand <= 50f;
		else if (criticalStage >= 3)
			isCrit = true;

		if(isCrit)
		{
			//if( sniper )
			//return 3;
			//else
			return 2;
		}
		else
		{
			return 1;
		}
	}

	float GetStabValue(AttackDatabase.AttackItem executedAtk, Pokemon atackingPokemon)
	{
		if( executedAtk.m_type == atackingPokemon.m_type1 || executedAtk.m_type == atackingPokemon.m_type2 )
		{
			/*if( capacity adaptabilite )
			{
				return 2f;
			}
			else
			{*/
				return 1.5f;
			//}
		}
		else
		{
			return 1f;
		}
	}

	float GetAttackEffectOnType(EPokemonType atkType, EPokemonType pokeType)
	{
		float returnValue = 1f;
		switch(atkType)
		{
		case EPokemonType.Acier:
			{
				if (pokeType == EPokemonType.Acier || pokeType == EPokemonType.Eau || pokeType == EPokemonType.Electrik || pokeType == EPokemonType.Feu) {
					returnValue = 0.5f;
				} else if (pokeType == EPokemonType.Fee || pokeType == EPokemonType.Glace || pokeType == EPokemonType.Roche) {
					returnValue = 2f;
				}
			}
			break;
		case EPokemonType.Combat:
			{
				if(pokeType == EPokemonType.Spectre) {
					returnValue = 0f;
				} else if (pokeType == EPokemonType.Fee || pokeType == EPokemonType.Insecte || pokeType == EPokemonType.Poison || pokeType == EPokemonType.Psy || pokeType == EPokemonType.Vol) {
					returnValue = 0.5f;
				} else if (pokeType == EPokemonType.Combat || pokeType == EPokemonType.Glace || pokeType == EPokemonType.Normal || pokeType == EPokemonType.Roche || pokeType == EPokemonType.Tenebres) {
					returnValue = 2f;
				}
			}
			break;
		case EPokemonType.Dragon:
			{
				if(pokeType == EPokemonType.Fee) {
					returnValue = 0f;
				} else if (pokeType == EPokemonType.Acier) {
					returnValue = 0.5f;
				} else if (pokeType == EPokemonType.Dragon) {
					returnValue = 2f;
				}
			}
			break;
		case EPokemonType.Eau:
			{
				if (pokeType == EPokemonType.Dragon || pokeType == EPokemonType.Eau || pokeType == EPokemonType.Plante) {
					returnValue = 0.5f;
				} else if (pokeType == EPokemonType.Feu || pokeType == EPokemonType.Roche || pokeType == EPokemonType.Sol) {
					returnValue = 2f;
				}
			}
			break;
		case EPokemonType.Electrik:
			{
				if(pokeType == EPokemonType.Sol) {
					returnValue = 0f;
				} else if (pokeType == EPokemonType.Dragon || pokeType == EPokemonType.Electrik || pokeType == EPokemonType.Plante) {
					returnValue = 0.5f;
				} else if (pokeType == EPokemonType.Eau || pokeType == EPokemonType.Vol) {
					returnValue = 2f;
				}
			}
			break;
		case EPokemonType.Fee:
			{
				if (pokeType == EPokemonType.Acier || pokeType == EPokemonType.Feu || pokeType == EPokemonType.Poison) {
					returnValue = 0.5f;
				} else if (pokeType == EPokemonType.Combat || pokeType == EPokemonType.Dragon || pokeType == EPokemonType.Tenebres) {
					returnValue = 2f;
				}
			}
			break;
		case EPokemonType.Feu:
			{
				if (pokeType == EPokemonType.Dragon || pokeType == EPokemonType.Eau || pokeType == EPokemonType.Feu || pokeType == EPokemonType.Roche) {
					returnValue = 0.5f;
				} else if (pokeType == EPokemonType.Acier || pokeType == EPokemonType.Glace || pokeType == EPokemonType.Insecte || pokeType == EPokemonType.Plante) {
					returnValue = 2f;
				}
			}
			break;
		case EPokemonType.Glace:
			{
				if (pokeType == EPokemonType.Acier || pokeType == EPokemonType.Eau || pokeType == EPokemonType.Feu || pokeType == EPokemonType.Glace) {
					returnValue = 0.5f;
				} else if (pokeType == EPokemonType.Dragon || pokeType == EPokemonType.Plante || pokeType == EPokemonType.Sol || pokeType == EPokemonType.Vol) {
					returnValue = 2f;
				}
			}
			break;
		case EPokemonType.Insecte:
			{
				if (pokeType == EPokemonType.Acier || pokeType == EPokemonType.Combat || pokeType == EPokemonType.Fee || pokeType == EPokemonType.Feu || pokeType == EPokemonType.Poison || pokeType == EPokemonType.Spectre || pokeType == EPokemonType.Vol) {
					returnValue = 0.5f;
				} else if (pokeType == EPokemonType.Plante || pokeType == EPokemonType.Psy || pokeType == EPokemonType.Tenebres) {
					returnValue = 2f;
				}
			}
			break;
		case EPokemonType.Normal:
			{
				if (pokeType == EPokemonType.Spectre) {
					returnValue = 0f;
				} else if (pokeType == EPokemonType.Acier || pokeType == EPokemonType.Roche) {
					returnValue = 0.5f;
				}
			}
			break;
		case EPokemonType.Plante:
			{
				if (pokeType == EPokemonType.Acier || pokeType == EPokemonType.Dragon || pokeType == EPokemonType.Feu || pokeType == EPokemonType.Insecte || pokeType == EPokemonType.Plante || pokeType == EPokemonType.Poison || pokeType == EPokemonType.Vol) {
					returnValue = 0.5f;
				} else if (pokeType == EPokemonType.Eau || pokeType == EPokemonType.Roche || pokeType == EPokemonType.Sol) {
					returnValue = 2f;
				}
			}
			break;
		case EPokemonType.Poison:
			{
				if(pokeType == EPokemonType.Acier) {
					returnValue = 0f;
				} else if (pokeType == EPokemonType.Poison || pokeType == EPokemonType.Roche || pokeType == EPokemonType.Sol || pokeType == EPokemonType.Spectre) {
					returnValue = 0.5f;
				} else if (pokeType == EPokemonType.Fee || pokeType == EPokemonType.Plante) {
					returnValue = 2f;
				}
			}
			break;
		case EPokemonType.Psy:
			{
				if(pokeType == EPokemonType.Tenebres) {
					returnValue = 0f;
				} else if (pokeType == EPokemonType.Acier || pokeType == EPokemonType.Psy) {
					returnValue = 0.5f;
				} else if (pokeType == EPokemonType.Combat || pokeType == EPokemonType.Poison) {
					returnValue = 2f;
				}
			}
			break;
		case EPokemonType.Roche:
			{
				if (pokeType == EPokemonType.Acier || pokeType == EPokemonType.Combat || pokeType == EPokemonType.Sol) {
					returnValue = 0.5f;
				} else if (pokeType == EPokemonType.Feu || pokeType == EPokemonType.Glace || pokeType == EPokemonType.Insecte || pokeType == EPokemonType.Vol) {
					returnValue = 2f;
				}
			}
			break;
		case EPokemonType.Sol:
			{
				if(pokeType == EPokemonType.Vol) {
					returnValue = 0f;
				} else if (pokeType == EPokemonType.Insecte || pokeType == EPokemonType.Plante) {
					returnValue = 0.5f;
				} else if (pokeType == EPokemonType.Acier || pokeType == EPokemonType.Electrik || pokeType == EPokemonType.Feu || pokeType == EPokemonType.Poison || pokeType == EPokemonType.Roche) {
					returnValue = 2f;
				}
			}
			break;
		case EPokemonType.Spectre:
			{
				if(pokeType == EPokemonType.Normal)	{
					returnValue = 0f;
				} else if (pokeType == EPokemonType.Tenebres) {
					returnValue = 0.5f;
				} else if (pokeType == EPokemonType.Psy || pokeType == EPokemonType.Spectre) {
					returnValue = 2f;
				}
			}
			break;
		case EPokemonType.Tenebres:
			{
				if (pokeType == EPokemonType.Combat || pokeType == EPokemonType.Fee || pokeType == EPokemonType.Tenebres) {
					returnValue = 0.5f;
				} else if (pokeType == EPokemonType.Psy || pokeType == EPokemonType.Spectre) {
					returnValue = 2f;
				}
			}
			break;
		case EPokemonType.Vol:
			{
				if (pokeType == EPokemonType.Acier || pokeType == EPokemonType.Electrik || pokeType == EPokemonType.Roche) {
					returnValue = 0.5f;
				} else if (pokeType == EPokemonType.Combat || pokeType == EPokemonType.Insecte || pokeType == EPokemonType.Plante) {
					returnValue = 2f;
				}
			}
			break;
		default:
			break;
		}
		return returnValue;
	}

	void ApplySecondaryEffect(AttackDatabase.AttackItem executedAtk, Pokemon atackingPokemon, Pokemon defendingPokemon, ref int dmg, ref int puissance, ref int selfDamage)
	{
		switch( executedAtk.m_type )
		{
    		case EPokemonType.Combat:
    			{
    				ApplySecondaryEffectCombatAttacks(executedAtk, atackingPokemon, defendingPokemon, ref dmg, ref puissance, ref selfDamage);
    			}
    			break;
    		case EPokemonType.Dragon:
    			{
    				ApplySecondaryEffectDrakeAttacks(executedAtk, atackingPokemon, defendingPokemon, ref dmg, ref puissance, ref selfDamage);
    			}
    			break;
    		case EPokemonType.Eau:
    			{
    				ApplySecondaryEffectWaterAttacks(executedAtk, atackingPokemon, defendingPokemon, ref dmg, ref puissance, ref selfDamage);
    			}
    			break;
    		case EPokemonType.Electrik:
    			{
    				ApplySecondaryEffectElectrikAttacks(executedAtk, atackingPokemon, defendingPokemon, ref dmg, ref puissance, ref selfDamage);
    			}
    			break;
    		case EPokemonType.Feu:
    			{
    				ApplySecondaryEffectFireAttacks(executedAtk, atackingPokemon, defendingPokemon, ref dmg, ref puissance, ref selfDamage);
    			}
    			break;
    		case EPokemonType.Glace:
    			{
    				ApplySecondaryEffectIceAttacks(executedAtk, atackingPokemon, defendingPokemon, ref dmg, ref puissance, ref selfDamage);
    			}
    			break;
            case EPokemonType.Insecte:
                {
                    ApplySecondaryEffectInsectAttacks(executedAtk, atackingPokemon, defendingPokemon, ref dmg, ref puissance, ref selfDamage);
                }
                break;
            case EPokemonType.Normal:
                {
                    ApplySecondaryEffectNormalAttacks(executedAtk, atackingPokemon, defendingPokemon, ref dmg, ref puissance, ref selfDamage);
                }
                break;
            case EPokemonType.Plante:
                {
                    ApplySecondaryEffectGrassAttacks(executedAtk, atackingPokemon, defendingPokemon, ref dmg, ref puissance, ref selfDamage);
                }
                break;
            case EPokemonType.Poison:
                {
                    ApplySecondaryEffectPoisonAttacks(executedAtk, atackingPokemon, defendingPokemon, ref dmg, ref puissance, ref selfDamage);
                }
                break;
            case EPokemonType.Psy:
                {
                    ApplySecondaryEffectPsyAttacks(executedAtk, atackingPokemon, defendingPokemon, ref dmg, ref puissance, ref selfDamage);
                }
                break;
            case EPokemonType.Roche:
                {
                    ApplySecondaryEffectRockAttacks(executedAtk, atackingPokemon, defendingPokemon, ref dmg, ref puissance, ref selfDamage);
                }
                break;
            case EPokemonType.Sol:
                {
                    ApplySecondaryEffectGroundAttacks(executedAtk, atackingPokemon, defendingPokemon, ref dmg, ref puissance, ref selfDamage);
                }
                break;
            case EPokemonType.Spectre:
                {
                    ApplySecondaryEffectGhostAttacks(executedAtk, atackingPokemon, defendingPokemon, ref dmg, ref puissance, ref selfDamage);
                }
                break;
            case EPokemonType.Vol:
                {
                    ApplySecondaryEffectFlyAttacks(executedAtk, atackingPokemon, defendingPokemon, ref dmg, ref puissance, ref selfDamage);
                }
                break;
		    default:
			    break;
		}
	}

	void ApplySecondaryEffectCombatAttacks(AttackDatabase.AttackItem executedAtk, Pokemon atackingPokemon, Pokemon defendingPokemon, ref int dmg, ref int puissance, ref int selfDamage)
	{
		switch (executedAtk.m_name) 
		{
		case "Frappe Atlas":
			{
				dmg = atackingPokemon.m_level;
			}
			break;
		case "Mawashi Geri":
			{
				OnFlinchProabability(30);
			}
			break;
		case "Riposte":
			{
                    if(m_battleManager.m_lastAttackInfo.attack.m_attackType == EAttackType.Physical)
				{
                        dmg = 2 * m_battleManager.m_lastAttackInfo.damageDealt;
				}
			}
			break;
		case "Sacrifice":
			{
				selfDamage = (int)(0.25f * dmg);
			}
			break;
		default:
			break;
		}
	}

	void ApplySecondaryEffectDrakeAttacks(AttackDatabase.AttackItem executedAtk, Pokemon atackingPokemon, Pokemon defendingPokemon, ref int dmg, ref int puissance, ref int selfDamage)
	{
		switch (executedAtk.m_name) 
		{
		case "Draco-Rage":
			{
				dmg = 40;
			}
			break;
		default:
			break;
		}
	}

	void ApplySecondaryEffectWaterAttacks(AttackDatabase.AttackItem executedAtk, Pokemon atackingPokemon, Pokemon defendingPokemon, ref int dmg, ref int puissance, ref int selfDamage)
	{
		switch (executedAtk.m_name) 
		{
    		case "Bulles d'O":
    		case "Ecume":
    			{
    				if( Random.Range (0, 100) < 10 )
    				{
                        ApplyStatModificator (ref GetOppPokemonBattleValues().vitesse, -1);
    				}
    			}
    			break;
    		case "Repli":
    			{
                    ApplyStatModificator(ref GetSelfPokemonBattleValues().def, 1);
    			}
    			break;
    		case "":
    			{

    			}
    			break;
    		default:
    			break;
		}
	}

	void ApplySecondaryEffectElectrikAttacks(AttackDatabase.AttackItem executedAtk, Pokemon atackingPokemon, Pokemon defendingPokemon, ref int dmg, ref int puissance, ref int selfDamage)
	{
		switch (executedAtk.m_name) 
		{
		case "Cage-Eclair":
			{
				TryApplyPhysicalStatus (defendingPokemon, EPokemonPhysicalStatus.Para, 100);
			}
			break;
		case "Eclair":
		case "Poing-Eclair":
		case "Tonnerre":
			{
				TryApplyPhysicalStatus (defendingPokemon, EPokemonPhysicalStatus.Para, 10);
			}
			break;
		case "Fatal-Foudre":
			{
				// if last defending pokemon's attack is Rebond, the damage are doubled
                AttackDatabase.AttackItem lastDefendingPokemonAttack = GetOppPokemonBattleValues ().m_lastAttackInfo.attack; 
				if( lastDefendingPokemonAttack.m_name == "Rebond" )
				{
					dmg *= 2;
				}

				TryApplyPhysicalStatus (defendingPokemon, EPokemonPhysicalStatus.Para, 30);
			}
			break;
		default:
			break;
		}
	}

	void ApplySecondaryEffectFireAttacks(AttackDatabase.AttackItem executedAtk, Pokemon atackingPokemon, Pokemon defendingPokemon, ref int dmg, ref int puissance, ref int selfDamage)
	{
		switch (executedAtk.m_name) 
		{
		case "Déflagration":
		case "Flammèche":
		case "Lance-Flamme":
		case "Poing de Feu":
			{
				TryApplyPhysicalStatus (defendingPokemon, EPokemonPhysicalStatus.Brul, 10);
			}
			break;
		default:
			break;
		}
	}

	void ApplySecondaryEffectIceAttacks(AttackDatabase.AttackItem executedAtk, Pokemon atackingPokemon, Pokemon defendingPokemon, ref int dmg, ref int puissance, ref int selfDamage)
	{
		switch (executedAtk.m_name) 
		{
    		case "Brume":
    			{
                    GetSelfPokemonBattleValues().isStatChangesFromOppBlocked = true;
			    }
			    break;
            case "Buée Noire":
                {
                    ResetStats();
                }
                break;
            case "Onde Boréale":
                {
                    ApplyStatModificator(ref GetOppPokemonBattleValues().atk, -1);
                }
                break;
            case "Blizzard":
            case "Laser Glace":
            case "Poing-Glace":
                {
                    TryApplyPhysicalStatus (defendingPokemon, EPokemonPhysicalStatus.Gele, 10);
                }
                break;
		    default:
			    break;
		}
	}

    void ApplySecondaryEffectInsectAttacks(AttackDatabase.AttackItem executedAtk, Pokemon atackingPokemon, Pokemon defendingPokemon, ref int dmg, ref int puissance, ref int selfDamage)
    {
        switch (executedAtk.m_name) 
        {
            case "Sécrétion":
                {
                    ApplyStatModificator(ref GetOppPokemonBattleValues().vitesse, -1);
                }
                break;
            case "Vampirisme":
                {
                    selfDamage = (int)(-dmg * 0.5f); // - to give life
                }
                break;
            default:
                break;
        }
    }

    void ApplySecondaryEffectNormalAttacks(AttackDatabase.AttackItem executedAtk, Pokemon atackingPokemon, Pokemon defendingPokemon, ref int dmg, ref int puissance, ref int selfDamage)
    {
        switch (executedAtk.m_name) 
        {
            case "Adaptation":
                {
                    if(atackingPokemon.m_moveSet[0] < m_attackList.Count) 
                    {
                        AttackDatabase.AttackItem atk = m_attackList[ atackingPokemon.m_moveSet[0] ];
                        GetSelfPokemonBattleValues().overrideType1 = atk.m_type;
                    }
                }
                break;
            case "Affûtage":
                {
                    ApplyStatModificator(ref GetSelfPokemonBattleValues().atk, 1);
                }
                break;
            case "Armure":
                {
                    ApplyStatModificator(ref GetSelfPokemonBattleValues().def, 1);
                }
                break;
            case "Berceuse":
            case "Grobisou":
                {
                    TryApplyPhysicalStatus (defendingPokemon, EPokemonPhysicalStatus.Dodo, 100);

                    if( defendingPokemon.m_physicalStatus == EPokemonPhysicalStatus.Dodo )
                    {
                        GetOppPokemonBattleValues().turnsSleep = Random.Range(1, 4); //[1, 4[
                    }
                }
                break;
            case "Boul'Armure":
                {
                    ApplyStatModificator(ref GetSelfPokemonBattleValues().def, 1);
                    //TODO: up roulade
                }
                break;
            case "Brouillard":
            case "Flash":
            case "Jet de Sable":
                {
                    ApplyStatModificator(ref GetOppPokemonBattleValues().precision, -1);
                }
                break;
            case "Constriction":
                {
                    if( Random.Range(0, 100) < 10 )
                    {
                        ApplyStatModificator(ref GetOppPokemonBattleValues().vitesse, -1);
                    }
                }
                break;
            case "Copie":
                {
                    AttackDatabase.AttackItem lastOppAttack = GetOppPokemonBattleValues().m_lastAttackInfo.attack;
                    if( lastOppAttack.m_name != "Babil" && lastOppAttack.m_name != "Gribouille" 
                        && lastOppAttack.m_name != "Lutte" && lastOppAttack.m_name != "Métronome" )
                    {
                        GetSelfPokemonBattleValues().attackCopy = lastOppAttack;
                        GetSelfPokemonBattleValues().attackCopy.m_pp = 10;
                    }
                }
                break;
            case "Coup d'Boule":
                {
                    if( Random.Range(0, 100) < 30 )
                    {
                        GetOppPokemonBattleValues().turnEffects.isFlinched = true;
                    }
                }
                break;
            case "Croc de Mort":
                {
                    if( Random.Range(0, 100) < 10 )
                    {
                        GetOppPokemonBattleValues().turnEffects.isFlinched = true;
                    }
                }
                break;
            case "Croc Fatal":
                {
                    dmg = (int)(defendingPokemon.m_currentPV * 0.5f);
                    if( dmg == 0 )
                    {
                        dmg = 1;
                    }
                }
                break;
            case "Croissance":
                {
                    ApplyStatModificator(ref GetSelfPokemonBattleValues().atkspe, 1);
                    ApplyStatModificator(ref GetSelfPokemonBattleValues().atk, 1);
                    // TODO: if soleil stats are up 2 instead of 1
                }
                break;
            case "Damoclès":
                {
                    selfDamage = (int)(dmg/3);
                }
                break;
            case "Danse-Lames":
                {
                    ApplyStatModificator(ref GetSelfPokemonBattleValues().atk, 2);
                }
                break;
            case "Destruction":
            case "Explosion":
                {
                    selfDamage = atackingPokemon.m_currentPV;
                }
                break;
            case "E-Coque":
            case "Soin":
                {
                    int maxLife = atackingPokemon.m_pv[1];
                    int heal = maxLife / 2;
                    selfDamage = atackingPokemon.m_currentPV + heal > maxLife ? maxLife : heal;
                }
                break;
            case "Ecrasement":
                {
                    if( Random.Range(0, 100) < 30 )
                    {
                        GetOppPokemonBattleValues().turnEffects.isFlinched = true;
                    }

                    if( GetOppPokemonBattleValues().isUnderLilliput )
                    {
                        dmg *= 2;
                    }
                }
                break;
            case "Empal'Korne":
            case "Guillotine":
                {
                    dmg = defendingPokemon.m_currentPV;
                }
                break;
            case "Entrave":
                {
                    //TODO: block attack selection in battleInterfaceManager
                    //TODO: fail if first to attack / the opp did not attack / failed to do so
                    //TODO: Can only block 1 attack at a time. Block the attack the opp just launched for 2 to 8 turns
                }
                break;
            case "Grincement":
                {
                    ApplyStatModificator(ref GetOppPokemonBattleValues().def, -2);
                }
                break;
            case "Groz'Yeux":
            case "Mimi-Queue":
                {
                    ApplyStatModificator(ref GetOppPokemonBattleValues().def, -1);
                }
                break;
            case "Intimidation":
                {
                    TryApplyPhysicalStatus (defendingPokemon, EPokemonPhysicalStatus.Para, 100);
                }
                break;
            case "Lilliput":
                {
                    ApplyStatModificator(ref GetSelfPokemonBattleValues().esquive, 2);
                    GetSelfPokemonBattleValues().isUnderLilliput = true;
                    //TODO: if under lilliput, attacks [Bulldoboule, Flying Press, Dracocharge, Hantise] have doubled damages
                }
                break;
            case "Lutte":
                {
                    selfDamage = Mathf.CeilToInt( 0.25f * atackingPokemon.m_pv[1] );
                }
                break;
            case "Plaquage":
                {
                    TryApplyPhysicalStatus(defendingPokemon, EPokemonPhysicalStatus.Para, 30);

                    if( GetOppPokemonBattleValues().isUnderLilliput )
                    {
                        dmg *= 2;
                    }
                }
                break;
            case "Puissance":
                {
                    ApplyStatModificator(ref GetSelfPokemonBattleValues().criticalStage, 2);
                }
                break;
            case "Reflet":
                {
                    ApplyStatModificator(ref GetSelfPokemonBattleValues().esquive, 1);
                }
                break;
            case "Rugissement":
                {
                    ApplyStatModificator(ref GetOppPokemonBattleValues().atk, -1);
                }
                break;
            case "Sonicboom":
                {
                    dmg = 20;
                }
                break;
            case "Ultrason":
                {
                    if( GetOppPokemonBattleValues().turnsConfusion == 0 )
                    {
                        GetOppPokemonBattleValues().turnsConfusion = Random.Range(1, 5); // [1, 5[
                        GetOppPokemonBattleValues().mentalStatus = EPokemonMentalStatus.Confus; //useless? 
                    }
                }
                break;
            default:
                break;
        }
    }

    void ApplySecondaryEffectGrassAttacks(AttackDatabase.AttackItem executedAtk, Pokemon atackingPokemon, Pokemon defendingPokemon, ref int dmg, ref int puissance, ref int selfDamage)
    {
        switch (executedAtk.m_name) 
        {
            case "Para-Spore":
                {
                    TryApplyPhysicalStatus(defendingPokemon, EPokemonPhysicalStatus.Para, 100);
                }
                break;
            case "Poudre Dodo":
            case "Spore":
                {
                    TryApplyPhysicalStatus(defendingPokemon, EPokemonPhysicalStatus.Dodo, 100);

                    if( defendingPokemon.m_physicalStatus == EPokemonPhysicalStatus.Dodo )
                    {
                        GetOppPokemonBattleValues().turnsSleep = Random.Range(1, 4); //[1, 4[
                    }
                }
                break;
            case "Vol-Vie":
            case "Méga-Sangsue":
                {
                    if( dmg == 1 )
                    {
                        selfDamage = 1;
                    }
                    else
                    {
                        selfDamage = (int)(-dmg * 0.5f);
                    }
                }
                break;
            default:
                break;
        }
    }

    void ApplySecondaryEffectPoisonAttacks(AttackDatabase.AttackItem executedAtk, Pokemon atackingPokemon, Pokemon defendingPokemon, ref int dmg, ref int puissance, ref int selfDamage)
    {
        switch (executedAtk.m_name) 
        {
            case "Acidarmure":
                {
                    ApplyStatModificator(ref GetSelfPokemonBattleValues().def, 2);
                }
                break;
            case "Acide":
                {
                    ApplyStatModificator(ref GetOppPokemonBattleValues().defspe, -1);
                }
                break;
            case "Dard-Venin":
            case "Détritus":
                {
                    TryApplyPhysicalStatus(defendingPokemon, EPokemonPhysicalStatus.Pois, 30);
                }
                break;
            case "Gaz Toxik":
            case "Poudre Toxik":
                {
                    TryApplyPhysicalStatus(defendingPokemon, EPokemonPhysicalStatus.Pois, 100);
                }
                break;
            case "Purédpois":
                {
                    TryApplyPhysicalStatus(defendingPokemon, EPokemonPhysicalStatus.Pois, 40);
                }
                break;
            default:
                break;
        }
    }

    void ApplySecondaryEffectPsyAttacks(AttackDatabase.AttackItem executedAtk, Pokemon atackingPokemon, Pokemon defendingPokemon, ref int dmg, ref int puissance, ref int selfDamage)
    {
        switch (executedAtk.m_name) 
        {
            case "Amnésie":
                {
                    ApplyStatModificator(ref GetSelfPokemonBattleValues().defspe, 2);
                }
                break;
            case "Bouclier":
                {
                    ApplyStatModificator(ref GetSelfPokemonBattleValues().def, 2);
                }
                break;
            case "Choc Mental":
            case "Rafale Psy":
                {
                    if( GetOppPokemonBattleValues().turnsConfusion == 0 )
                    {                        
                        if( Random.Range(0, 100) < 10 )
                        {
                            GetOppPokemonBattleValues().mentalStatus = EPokemonMentalStatus.Confus;
                            GetOppPokemonBattleValues().turnsConfusion = Random.Range(1, 5); // [1, 5[
                        }
                    }
                }
                break;
            case "Dévorêve":
                {
                    selfDamage = -dmg * 2;
                }
                break;
            case "Hâte":
                {
                    ApplyStatModificator(ref GetSelfPokemonBattleValues().vitesse, 2);
                }
                break;
            case "Hypnose":
                {
                    TryApplyPhysicalStatus(defendingPokemon, EPokemonPhysicalStatus.Dodo, 100);

                    if( defendingPokemon.m_physicalStatus == EPokemonPhysicalStatus.Dodo )
                    {
                        GetOppPokemonBattleValues().turnsSleep = Random.Range(1, 4); //[1, 4[
                    }
                }
                break;
            case "Psyko":
                {
                    if( Random.Range(0, 100) < 10 )
                    {
                        ApplyStatModificator(ref GetOppPokemonBattleValues().defspe, -1);
                    }
                }
                break;
            case "Repos":
                {
                    selfDamage = -atackingPokemon.m_pv[1];
                    atackingPokemon.m_physicalStatus = EPokemonPhysicalStatus.Default;

                    TryApplyPhysicalStatus(atackingPokemon, EPokemonPhysicalStatus.Dodo, 100);
                    GetSelfPokemonBattleValues().turnsSleep = 3;
                }
                break;
            case "Télékinésie":
                {
                    ApplyStatModificator(ref GetOppPokemonBattleValues().precision, -1);
                }
                break;
            case "Vague Psy":
                {
                    int x = Random.Range(0, 11); // [0, 11[
                    dmg = (int) (atackingPokemon.m_level * (x + 5) * 0.1f);
                }
                break;
            case "Yoga":
                {
                    ApplyStatModificator(ref GetSelfPokemonBattleValues().atk, 1);
                }
                break;
            default:
                break;
        }
    }

    void ApplySecondaryEffectRockAttacks(AttackDatabase.AttackItem executedAtk, Pokemon atackingPokemon, Pokemon defendingPokemon, ref int dmg, ref int puissance, ref int selfDamage)
    {
        switch (executedAtk.m_name) 
        {
            case "Eboulement":
                {
                    if( Random.Range(0, 100) < 30 )
                    {
                        GetOppPokemonBattleValues().turnEffects.isFlinched = true;
                    }
                }
                break;
            default:
                break;
        }
    }

    void ApplySecondaryEffectGroundAttacks(AttackDatabase.AttackItem executedAtk, Pokemon atackingPokemon, Pokemon defendingPokemon, ref int dmg, ref int puissance, ref int selfDamage)
    {
        switch (executedAtk.m_name) 
        {
            case "Abîme":
                {
                    dmg = defendingPokemon.m_pv[1];
                }
                break;
            case "Massd'Os":
                {
                    if( Random.Range(0, 100) < 10 )
                    {
                        GetOppPokemonBattleValues().turnEffects.isFlinched = true;
                    }
                }
                break;
            default:
                break;
        }
    }

    void ApplySecondaryEffectGhostAttacks(AttackDatabase.AttackItem executedAtk, Pokemon atackingPokemon, Pokemon defendingPokemon, ref int dmg, ref int puissance, ref int selfDamage)
    {
        switch (executedAtk.m_name) 
        {
            case "Onde Folie":
                {
                    if( GetOppPokemonBattleValues().turnsConfusion == 0 )
                    {                        
                        GetOppPokemonBattleValues().mentalStatus = EPokemonMentalStatus.Confus;
                        GetOppPokemonBattleValues().turnsConfusion = Random.Range(1, 5); // [1, 5[
                    }
                }
                break;
            case "Ténèbres":
                {
                    dmg = atackingPokemon.m_level;
                }
                break;
            case "Léchouille":
                {
                    TryApplyPhysicalStatus(atackingPokemon, EPokemonPhysicalStatus.Para, 30);
                }
                break;
            default:
                break;
        }
    }

    void ApplySecondaryEffectFlyAttacks(AttackDatabase.AttackItem executedAtk, Pokemon atackingPokemon, Pokemon defendingPokemon, ref int dmg, ref int puissance, ref int selfDamage)
    {
        switch (executedAtk.m_name) 
        {
            case "":
                {

                }
                break;
            default:
                break;
        }
    }

/*	void ApplySecondaryEffectCombatAttacks(AttackDatabase.AttackItem executedAtk, Pokemon atackingPokemon, Pokemon defendingPokemon, ref int dmg, ref int puissance, ref int selfDamage)
	{
		switch (executedAtk.m_name) 
		{
    		case "":
    			{

    			}
    			break;
		default:
			break;
		}
	}*/

	void TryApplyPhysicalStatus(Pokemon pokemon, EPokemonPhysicalStatus newStatus, int proba)
	{
		//&& if no clone
		if(pokemon.m_physicalStatus == EPokemonPhysicalStatus.Default)
		{
			if ( Random.Range (0, 100) < proba ) 
			{
				pokemon.m_physicalStatus = newStatus;
			}
		}
	}

	int GetPuissanceBasedOnOppWeight(float weight)
	{
		if(weight < 10f)
		{
			return 20;
		}
		else if(weight < 25f)
		{
			return 40;
		}
		else if(weight < 50f)
		{
			return 60;
		}
		else if(weight < 100f)
		{
			return 80;
		}
		else if(weight < 200f)
		{
			return 100;
		}
		else
		{
			return 120;
		}
	}

	void OnFlinchProabability(int effectProbability)
	{
		if (Random.Range(0, 100) < effectProbability)
		{
            GetOppPokemonBattleValues().turnEffects.isFlinched = true;
		}
	}

	void ResetStats()
	{
		//player
        m_battleManager.m_playerPokeValues.criticalStage = 0;
        m_battleManager.m_playerPokeValues.atk = 0;
        m_battleManager.m_playerPokeValues.def = 0;
        m_battleManager.m_playerPokeValues.atkspe = 0;
        m_battleManager.m_playerPokeValues.defspe = 0;
        m_battleManager.m_playerPokeValues.vitesse = 0;
        m_battleManager.m_playerPokeValues.precision = 0;
        m_battleManager.m_playerPokeValues.esquive = 0;

		//opp
        m_battleManager.m_oppPokeValues.criticalStage = 0;
        m_battleManager.m_oppPokeValues.atk = 0;
        m_battleManager.m_oppPokeValues.def = 0;
        m_battleManager.m_oppPokeValues.atkspe = 0;
        m_battleManager.m_oppPokeValues.defspe = 0;
        m_battleManager.m_oppPokeValues.vitesse = 0;
        m_battleManager.m_oppPokeValues.precision = 0;
        m_battleManager.m_oppPokeValues.esquive = 0;
	}
}



//case "Double Pied":		//combat
//case "Claquoir":			//eau
//case "DanseFlamme":       //feu
//case "Dard Nuée":         //Insect
//case "Double Dard":		//Insect
//case "Clone":             //Normal
//case "Cyclone":           //normal
//case "Entrave":           //normal
//case "Hurlement":         //normal
//case "Morphing":          //normal
//case "Patience":          //normal
//case "Jackpot":           //normal
//case "Morsure":           //Tenebre (normal in 1st G)
//case "Coupe-Vent":        //normal
//case "Mania":             //normal
//case "Coud'Krâne":        //normal
//case "Ultralaser":        //normal
//case "Vampigraine":       //plante
//case "Danse-Fleur":       //plante
//case "Lance-Soleil":      //plante
//case "Toxik":             //poison
//case "Mur Lumière":       //psy
//case "Protection":        //psy
//case "Téléport":          //psy
//case "Osmerang":          //psy
//case "Tunnel":            //psy
//case "Vol":               //Vol
//case "Piqué":             //Vol

/* //normal
Étreinte        
Furie       
Ligotage        
Pilonnage      
Torgnoles       
Combo-Griffe    
Poing Comète        
Frénésie        
Picanon     
*/

/*
 * When setting selected attack
case "Métronome":
{
    int size = m_attackList.Count;
    int rand = 0;
    bool stop = false;

    do {
        rand = Random(0, size);
        if( m_attackList[rand].m_name != "Lutte" && m_attackList[rand].m_name != "Métronome" )
        {
            executedAtk = m_attackList[rand];
            stop = true;
        }
    } while( !stop )
        //TODO: Add attacks impossible to launch for generation > 1
    }
break;
*/
