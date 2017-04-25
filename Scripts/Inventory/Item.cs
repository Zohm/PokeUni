using UnityEngine;
using System.Collections;

	
public enum EItemCategory : uint 
{
	EMiscellaneous = 0,
	ECombat,
	ECapacity,
	EBerry,
	EUnique
}

public class Item : MonoBehaviour 
{
	int m_id;
	EItemCategory m_category = EItemCategory.EMiscellaneous;
	bool m_isRegistered = false;

	public Item( int id )
	{
		m_id = id;

		// Get other info from db
	}

	/*
	 * Use, Give and Throw should be inside Inventory class
	 * */
	public void Use() {
		
	}

	public void Give() {
		
	}

	public void Throw() {
		
	}

	public bool CanBeUsed() {
		
		return m_category == EItemCategory.ECombat || m_category == EItemCategory.EBerry;
	}

	public bool CanBeGiven() {

		return m_category == EItemCategory.EMiscellaneous || m_category == EItemCategory.EBerry;
	}

	public bool CanBeThrown() {

		return m_category == EItemCategory.EMiscellaneous || m_category == EItemCategory.ECombat || m_category == EItemCategory.EBerry;
	}

	/*
	 * Might be useless if the specific object are directly mapped to keyboard key
	 * eg: "v" would start the bike
	 * 
	bool CanBeRegistered() {

		ItemDatabase.ItemItem itemData = ItemDatabase.GetItemById (m_id);
		return m_category == EItemCategory.EUnique && !m_isRegistered && itemData.m_canBeRegistered;
	}

	bool CanBeUnregistered() {

		return m_isRegistered;
	}
	*/
}
