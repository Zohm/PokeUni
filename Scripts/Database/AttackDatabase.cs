using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

public class AttackDatabase : AssetSingleton<AttackDatabase> 
{
	[System.Serializable]
	public class AttackItem
	{
		public int m_index = 0;
		public string m_name = "";

		public string m_description = "";

		public EAttackType m_attackType = EAttackType.Default;
		public EPokemonType m_type = EPokemonType.Default;

		public int m_puissance = 0;
		public int m_precision = 100;
		public int m_priorite = 0;
		public int m_pp = 0;

		public AttackItem(int id)
		{
			m_index = id;
		}
	}

	public List<AttackItem> m_attackItems = new List<AttackItem> ();

	//----------------------------------------------------------------------------
	public static List<AttackItem> GetAttackList()
	{
		return Instance.m_attackItems;
	}

	//----------------------------------------------------------------------------
	public static void CreateNewAttack()
	{
		int size = Instance.m_attackItems.Count;
		Instance.m_attackItems.Add (new AttackItem(size));
	}

	//----------------------------------------------------------------------------
	public static AttackItem GetAttackById(int id)
	{
		return id >= 0 && id < Instance.m_attackItems.Count ? Instance.m_attackItems [id] : null;
	}
}
