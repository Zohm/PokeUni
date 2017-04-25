using UnityEngine;
using System.Collections;

public enum ECombatActionType
{
	Attack = 0,
	Object,
	Switch,
	Run
}

//----------------------------------------------------------------------------
public abstract class CCombatAction
{
	public abstract ECombatActionType GetActionType();
}

//----------------------------------------------------------------------------
public class CAttackAction : CCombatAction
{
	public int attackId;

	public CAttackAction( int _attackId ) { attackId = _attackId; }

	public override ECombatActionType GetActionType() { return ECombatActionType.Attack; }
}

//----------------------------------------------------------------------------
public class CObjectAction : CCombatAction
{
	public int objectId;
	Pokemon target;

	public CObjectAction( int _objectId, Pokemon _target ) { objectId = _objectId; target = _target; }

	public override ECombatActionType GetActionType() { return ECombatActionType.Object; }
}

//----------------------------------------------------------------------------
public class CSwitchAction : CCombatAction
{
	public int replacingPokemonUniqueId;

	public CSwitchAction( int id ) { replacingPokemonUniqueId = id; }

	public override ECombatActionType GetActionType() { return ECombatActionType.Switch; }
}

//----------------------------------------------------------------------------
public class CRunAction : CCombatAction
{
	public override ECombatActionType GetActionType() { return ECombatActionType.Run; }
}
