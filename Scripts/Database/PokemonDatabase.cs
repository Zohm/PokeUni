using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

public class PokemonDatabase : AssetSingleton<PokemonDatabase> //, ISerializable 
{
	[System.Serializable]
	public class PokemonItem
	{
		public int m_index = 0;
		public string m_name = "";
		public float m_size = 0f;
		public float m_weight = 0f;

		public float m_tauxCapture = 0f;
		public float m_ratioMale = 0f;

		public EPokemonType m_type1 = EPokemonType.Default;
		public EPokemonType m_type2 = EPokemonType.Default;

		public int m_pv = 0;
		public int m_atk = 0;
		public int m_def = 0;
		public int m_vitesse = 0;
		public int m_atkspe = 0;
		public int m_defspe = 0;

		public int m_pv_given = 0;
		public int m_atk_given = 0;
		public int m_def_given = 0;
		public int m_vitesse_given = 0;
		public int m_atkspe_given = 0;
		public int m_defspe_given = 0;

		public ECourbesExperience m_courbeExp;

		public int m_baseXp;
		public int m_evolution;
		public int m_lvlEvolution;

		//capacite speciale

		public List<PokemonAttack> m_attacksByLvl = new List<PokemonAttack>();

		//todo sprites (world + icone menu)
		public Sprite m_sprite_fight_face;
		public Sprite m_sprite_fight_back;

		[System.NonSerialized]
		public bool m_viewed = false;

		[System.NonSerialized]
		public bool m_catched = false;

		public PokemonItem(int index)
		{
			m_index = index;
		}
	}

	[System.Serializable]
	public class PokemonAttack
	{
		public int m_level = 0;
		public int m_attackId = 0;
	}

	public List<PokemonItem> m_pokemonItems = new List<PokemonItem> ();
	//public Dictionary<int, PokemonItem> m_pokemonItems = new Dictionary<int, PokemonItem>();	// Todo : For perf at runtime

	//----------------------------------------------------------------------------
	public static List<PokemonItem> GetPokemonList()
	{
		return Instance.m_pokemonItems;
	}

	//----------------------------------------------------------------------------
	public static void CreateNewPokemon()
	{
		int size = Instance.m_pokemonItems.Count;
		Instance.m_pokemonItems.Add (new PokemonItem(size));
	}

	//----------------------------------------------------------------------------
	public static PokemonItem GetPokemonByIndex(int index)
	{
		if( index >= 0 && index < Instance.m_pokemonItems.Count )
		{
			return Instance.m_pokemonItems [index];
		}
		else
		{
			return null;
		}
	}

	//----------------------------------------------------------------------------
	public static PokemonItem GetPokemonByName(string name)
	{
		if( !string.IsNullOrEmpty (name) )
		{
			foreach( var item in Instance.m_pokemonItems )
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
	public static List<PokemonAttack> GetListOfAttacksOfPokemonById(int pokemonId)
	{
		return pokemonId >= 0 && pokemonId < Instance.m_pokemonItems.Count ? Instance.m_pokemonItems [pokemonId].m_attacksByLvl : null;
	}

	//----------------------------------------------------------------------------
	public static List<int> GetListOfAttacksOfPokemonByIdAtLevel(int pokemonId, int level)
	{
		List<int> attacksIdAtLvl = new List<int> ();
		if( pokemonId >= 0 && pokemonId < Instance.m_pokemonItems.Count )
		{
			List<PokemonAttack> attacks = Instance.m_pokemonItems [pokemonId].m_attacksByLvl;
			foreach(PokemonAttack attack in attacks)
			{
				if( attack.m_level == level )
				{
					attacksIdAtLvl.Add (attack.m_attackId);
				}
				else if (attack.m_level > level)
				{
					return attacksIdAtLvl;
				}
			}
		}
		return attacksIdAtLvl;
	}

	//----------------------------------------------------------------------------
	public static void CreateNewLevelAttack(int index)
	{
		Instance.m_pokemonItems [index].m_attacksByLvl.Add (new PokemonAttack());
	}

	//----------------------------------------------------------------------------
	public static void RemoveLevelAttack (int index, PokemonAttack element)
	{
		Instance.m_pokemonItems [index].m_attacksByLvl.Remove (element);
	}
	
	/*
	//----------------------------------------------------------------------------
	public void OnSerialize(Serializer serializer)
	{
		if (serializer.IsWriting) {
			// If we are writing out to file, just write out the UIDs along with if they are unlocked
			int count = m_pokemonItems.Count;
			serializer.VarAfterVersion<int> (ref count, DataVersion.SavedPokemons, 0);

			foreach (PokemonItem item in m_pokemonItems) {
				serializer.VarAfterVersion<int> (ref item.m_index, DataVersion.SavedPokemons, 0);
				serializer.VarAfterVersion<bool>	(ref item.m_viewed, DataVersion.SavedPokemons, false);
				serializer.VarAfterVersion<bool>	(ref item.m_catched,	DataVersion.SavedPokemons, false);
			}
		} else if (serializer.IsReading) {
			// If we are reading from file, read in the number of items saved last time
			int count = 0;
			serializer.VarAfterVersion<int> (ref count, DataVersion.SavedPokemons, 0);

			// Then read each one in and find it in the current database and set if it has been unlocked or not
			for (int i = 0; i < count; i++) {
				int itemIndex = 0;
				bool	itemCatched = false;
				bool	itemViewed = false;

				serializer.VarAfterVersion<int> (ref itemIndex, DataVersion.SavedPokemons, 0);
				serializer.VarAfterVersion<bool>	(ref itemCatched,	DataVersion.SavedPokemons, false);
				serializer.VarAfterVersion<bool>	(ref itemViewed,	DataVersion.SavedPokemons, false);

				if (itemIndex != 0) {
					PokemonItem item = GetPokemonByIndex (itemIndex);
					if (item) {
						item.m_catched	= itemCatched;
						item.m_viewed	= itemViewed;
					}
				}
			}

			SaveSystem.OnLoadComplete;
		}
	}
	*/
}
