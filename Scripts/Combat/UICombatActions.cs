using UnityEngine;
using System.Collections;

public enum UICombatActionType
{
	AttackAnimation = 0,
	HealthChange,
	PhysicalStatusChange,
	MentalStatusChange
}

//----------------------------------------------------------------------------
public abstract class CUICombatAction	//UI actions for visual understanding of the combat
{
	int turn; //At which turn should the action be played

	//TODO :
	// bool blockNewActionPush; // for multipleTurnAttackAction. Or if queue as an action at turn start, just block any push for this turn 
	// Pokemon doing the anim

	protected CUICombatAction( int _turn ) { turn = _turn; }

	//public abstract void DoAction(); // Text display followed by anim for example
	protected virtual void DisplayText() {} // force override? CHealthChangeAction might not need text display

	public abstract UICombatActionType GetUiActionType();
}

//----------------------------------------------------------------------------
class CUIAttackAnimationAction : CUICombatAction
{
	bool success = true; //Does the attack hits
	// TODO: attackId to get anim

	CUIAttackAnimationAction( int _turn, bool _success = true ) : base(_turn) { success = _success; }

	public override UICombatActionType GetUiActionType() { return UICombatActionType.AttackAnimation; }
}

//----------------------------------------------------------------------------
class CUIHealthChangeAction : CUICombatAction
{
	int healthChangeValue;

	CUIHealthChangeAction( int _turn, int value = 0 ) : base(_turn) { healthChangeValue = value; }

	public override UICombatActionType GetUiActionType() { return UICombatActionType.HealthChange; }
}

//----------------------------------------------------------------------------
class CUIPhysicalStatusChangeAction : CUICombatAction
{
	EPokemonPhysicalStatus newPhysicalStatus;

	CUIPhysicalStatusChangeAction( int _turn, EPokemonPhysicalStatus status = EPokemonPhysicalStatus.Default ) : base(_turn) { newPhysicalStatus = status; }

	public override UICombatActionType GetUiActionType() { return UICombatActionType.PhysicalStatusChange; }
}

//----------------------------------------------------------------------------
class CUIMentalStatusChangeAction : CUICombatAction
{
	EPokemonMentalStatus newMentalStatus;

	CUIMentalStatusChangeAction( int _turn, EPokemonMentalStatus status = EPokemonMentalStatus.Default ) : base(_turn) { newMentalStatus = status; }

	public override UICombatActionType GetUiActionType() { return UICombatActionType.MentalStatusChange; }
}

// multipleTurnAttackAction => special attacks that takes place over multiple turns (exemple: patience)
