using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

public class MapDatabase : AssetSingleton<MapDatabase>
{
	[System.Serializable]
	public class MapItem
	{
		public readonly int m_uniqueId = 0;
		public int m_levelIndex = 0;
		public string m_name = "";

		public GameObject m_mapPrefab = null;

		public List<MapWildPokemon> m_wildPokemonList = new List<MapWildPokemon> ();

		public MapItem(int id)
		{
			m_uniqueId = id;
		}
	}

	[System.Serializable]
	public class MapWildPokemon
	{
		public int m_pokemonId = 0;
		public int m_minLvl = 0;
		public int m_maxLvl = 0;
		public int m_proba = 0;
	}

	public List<MapItem> m_mapItems = new List<MapItem> ();

	//----------------------------------------------------------------------------
	public static List<MapItem> GetMapList()
	{
		return Instance.m_mapItems;
	}

	//----------------------------------------------------------------------------
	public static void CreateNewMap()
	{
		int size = Instance.m_mapItems.Count;
		Instance.m_mapItems.Add (new MapItem(size));
	}

	//----------------------------------------------------------------------------
	public static MapItem GetMaByUniqueId(int id)
	{
		if( id >= 0 && id < Instance.m_mapItems.Count )
		{
			return Instance.m_mapItems [id];
		}
		else
		{
			return null;
		}
	}

	//----------------------------------------------------------------------------
	public static MapItem GetMapByName(string name)
	{
		if( !string.IsNullOrEmpty (name) )
		{
			foreach( var item in Instance.m_mapItems )
			{
				if( item.m_name.Equals (name) )
				{
					return item;
				}
			}
		}
		return null;
	}

	//----------------------------------------------------------------------------
	public static void CreateNewWildPokemon(int index)
	{
		Instance.m_mapItems [index].m_wildPokemonList.Add (new MapWildPokemon());
	}

	//----------------------------------------------------------------------------
	public static void RemoveWildPokemon (int index, MapWildPokemon element)
	{
		Instance.m_mapItems [index].m_wildPokemonList.Remove (element);
	}

	public static List<MapWildPokemon> GetListOfWildPokemons(string name)
	{
		MapItem map = GetMapByName (name);
		if( map != null )
		{
			return map.m_wildPokemonList;
		}
		return null;
	}
}