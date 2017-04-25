using UnityEngine;
using System.Collections;

namespace BattleInfo
{
    public class PokemonBattleValues
    {
        public EPokemonType overrideType1 = EPokemonType.Default;

        public int criticalStage = 0;

        public int atk = 0;
        public int def = 0;
        public int atkspe = 0;
        public int defspe = 0;
        public int vitesse = 0;

        public int precision = 0;
        public int esquive = 0;

        public EPokemonMentalStatus mentalStatus = EPokemonMentalStatus.Default; //TODO: may have more than 1 at once => create a bit flag
        public int turnsSleep = 0;  //TODO: sleep needs to be conserved when switch
        public int turnsConfusion = 0;
        public int turnsJail = 0;

        public bool isStatChangesFromOppBlocked = false;
        public bool isUnderLilliput = false;

        public bool canSwitch = true;

        public STurnEffects turnEffects;

        public SLastAttackInfo m_lastAttackInfo;
        public AttackDatabase.AttackItem attackCopy;

		public int fleeAttempts = 0;

        public void Reset()
        {
            overrideType1 = EPokemonType.Default;
            criticalStage = 0;
            atk = 0;
            def = 0;
            atkspe = 0;
            defspe = 0;
            vitesse = 0;
            precision = 0;
            esquive = 0;
            mentalStatus = EPokemonMentalStatus.Default;
            turnsSleep = 0;
            turnsConfusion = 0;
            turnsJail = 0;
            isStatChangesFromOppBlocked = false;
            canSwitch = true;
			fleeAttempts = 0;
            turnEffects.Reset();
            m_lastAttackInfo.Reset();
        }
    }

    /*
    * Effect reseted at the end of the turn 
    */
    public struct STurnEffects
    {
        public bool isFlinched; //Apeuré

        public void Reset()
        {
            isFlinched = false;
        }
    }

    public struct SGeneralBattleValues
    {
        public void Reset()
        {
        }
    }

    public struct SLastAttackInfo
    {
        public AttackDatabase.AttackItem attack;
        public int damageDealt;

        public void Reset()
        {
            attack = new AttackDatabase.AttackItem(-1);
            damageDealt = 0;
        }
    }	
}
