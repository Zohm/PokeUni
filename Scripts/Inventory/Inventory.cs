using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Inventory : MonoBehaviour 
{
	// Map< item Id, item count in inventory >
	Dictionary< int, Pair<Item, int> > m_items = new Dictionary< int, Pair<Item, int> >();

	//------------------------------------------------------------------------------
	bool CanAddItemToInventory( int itemId, int quantity = 1 )
	{
		if( m_items.ContainsKey( itemId ) )
		{
			int potentialQuantity = m_items[itemId].second + quantity;
			if( potentialQuantity > 99 )
				return false;
		}
		else 
		{
			if( quantity > 99 )
				return false;
		}
		return true;
	}

	//------------------------------------------------------------------------------
	void AddItemToInventory( int itemId, int quantity = 1 )
	{
		if( CanAddItemToInventory( itemId, quantity ) )
		{
			if( m_items.ContainsKey( itemId ) )
			{
				m_items[itemId].second += quantity;
			}
			else
			{
				m_items.Add( itemId, new Pair<Item, int>(new Item(itemId), quantity) ) ;
			}
		}
	}

	//------------------------------------------------------------------------------
	void RemoveItemFromInventory( int itemId, int quantity = 1 )
	{
		if( m_items.ContainsKey( itemId ) && m_items[itemId].first.CanBeThrown() )
		{
			m_items[itemId].second -= quantity;

			if( m_items[itemId].second <= 0 )
			{
				m_items.Remove( itemId );
			}
		}
	}

	//------------------------------------------------------------------------------
	void GiveItemToPokemon( int itemId, Pokemon pokemon )
	{
		if( m_items.ContainsKey( itemId ) && m_items[itemId].first.CanBeGiven() )
		{
			// TODO - First give object to Pokemon

			// Then remove it from the inventory
			RemoveItemFromInventory(itemId);
		}
	}

	//------------------------------------------------------------------------------
	void UseItemOnPokemon( int itemId, Pokemon pokemon )
	{
		if( m_items.ContainsKey( itemId ) && m_items[itemId].first.CanBeUsed() )
		{
			m_items[itemId].first.Use();

			// Then remove it from the inventory
			RemoveItemFromInventory(itemId);
		}
	}
}
