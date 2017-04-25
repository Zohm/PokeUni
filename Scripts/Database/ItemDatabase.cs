using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

public class ItemDatabase : AssetSingleton<ItemDatabase> 
{
	[System.Serializable]
	public class ItemItem
	{
		public int m_index = 0;
		public string m_name = "";

		public string m_description = "";

		public EItemCategory m_category = EItemCategory.EMiscellaneous;

		public ItemItem(int id)
		{
			m_index = id;
		}
	}

	public List<ItemItem> m_ItemItems = new List<ItemItem> ();

	//----------------------------------------------------------------------------
	public static ItemItem GetItemById(int id)
	{
		return id >= 0 && id < Instance.m_ItemItems.Count ? Instance.m_ItemItems [id] : null;
	}
}
