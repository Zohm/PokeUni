using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

public class SignDatabase : AssetSingleton<SignDatabase>
{
	[System.Serializable]
	public class SignItem
	{
		public int m_uniqueId;

		public int m_mapUniqueId;
		public int m_indexSignOnMap;
		public Vector2 m_position;

		public string m_signText;

		public SignItem(int id)
		{
			m_uniqueId = id;
		}
	}

	public List<SignItem> m_signItems = new List<SignItem> ();

	//----------------------------------------------------------------------------
	public static List<SignItem> GetSignList()
	{
		return Instance.m_signItems;
	}

	//----------------------------------------------------------------------------
	public static void CreateNewSign()
	{
		int size = Instance.m_signItems.Count;
		Instance.m_signItems.Add (new SignItem(size));
	}

	//----------------------------------------------------------------------------
	public static SignItem GetSignById(int id)
	{
		if( id >= 0 && id < Instance.m_signItems.Count )
		{
			return Instance.m_signItems [id];
		}
		else
		{
			return null;
		}
	}

	//----------------------------------------------------------------------------
	public static List<SignItem> GetSignsOfMap(int mapId)
	{
		List<SignItem> mapSigns = new List<SignItem>();
		foreach( SignItem sign in Instance.m_signItems )
		{
			if( sign.m_mapUniqueId == mapId )
			{
				mapSigns.Add (sign);
			}
		}
		return mapSigns;
	}
}
