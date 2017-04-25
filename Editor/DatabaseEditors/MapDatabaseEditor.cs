using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(MapDatabase))]
public class MapDatabaseEditor : DatabaseEditor 
{
	private string[] m_pokemonNames;

	bool m_delete = false;
	private KeyValuePair<int, MapDatabase.MapWildPokemon> m_elementToDelete;

	bool m_listUnfolded = false;

	//----------------------------------------------------------------------------
	public override void OnEnable()
	{
		m_itemListProperty = this.serializedObject.FindProperty("m_mapItems");
		base.OnEnable ();

		CreatePokemonNameList ();
	}
		
	//----------------------------------------------------------------------------
	protected override void OnInternalInspectorGUI()
	{
		MapDatabase.MapItem item = MapDatabase.GetMaByUniqueId(m_currentlySelected);

		if (item != null) {
			GUILayout.BeginVertical ();
			{
				EditorGUILayout.LabelField ("Map unique ID", item.m_uniqueId.ToString ());
				item.m_levelIndex = EditorGUILayout.IntField ("Map level index", item.m_levelIndex);

				item.m_name = EditorGUILayout.TextField ("Map Name", item.m_name);

				item.m_mapPrefab = EditorGUILayout.ObjectField ("Map prefab", item.m_mapPrefab, typeof(GameObject), false) as GameObject;

				// Attack list
				GUILayout.BeginHorizontal ();
				{
					EditorGUILayout.LabelField ("Encounterable wild Pokemons");
					if (GUILayout.Button (m_listUnfolded ? "Hide" : "Show")) {
						m_listUnfolded = !m_listUnfolded;
					}
				}
				GUILayout.EndHorizontal ();

				if (m_listUnfolded) 
				{
					foreach (MapDatabase.MapWildPokemon element in item.m_wildPokemonList) 
					{
						GUILayout.BeginHorizontal ();
						{
							EditorGUILayout.LabelField (" ", GUILayout.MaxWidth (40));
							element.m_pokemonId = EditorGUILayout.Popup (element.m_pokemonId, m_pokemonNames);
							EditorGUILayout.LabelField (" ", GUILayout.Width (20));
							element.m_minLvl = EditorGUILayout.IntField ("Level min", element.m_minLvl);
							EditorGUILayout.LabelField (" ", GUILayout.Width (20));
							element.m_maxLvl = EditorGUILayout.IntField ("Level max", element.m_maxLvl);
							EditorGUILayout.LabelField (" ", GUILayout.Width (20));
							element.m_proba = EditorGUILayout.IntField ("Encounter Proba", element.m_proba);
							EditorGUILayout.LabelField (" ", GUILayout.Width (20));

							if (GUILayout.Button ("-")) {
								m_delete = true;
								m_elementToDelete = new KeyValuePair<int, MapDatabase.MapWildPokemon> (item.m_uniqueId, element);
							}
						}
						GUILayout.EndHorizontal ();
					}

					GUILayout.BeginHorizontal ();
					{
						EditorGUILayout.LabelField (" ", GUILayout.MaxWidth (40));
						if (GUILayout.Button ("+")) {
							AddNewWildPokemon (item.m_uniqueId);
						}
					}
					GUILayout.EndHorizontal ();
				}
			}
			GUILayout.EndVertical ();
		}

		// Delay deletion so we don't do it during the for each loop
		if( m_delete )
		{
			RemoveWildPokemon (m_elementToDelete.Key, m_elementToDelete.Value);
			m_delete = false;
		}

		if (GUI.changed)
		{
			EditorUtility.SetDirty(target);
		}
	}

	//----------------------------------------------------------------------------
	protected override void AddToList(ReorderableList list)
	{
		MapDatabase.CreateNewMap();
		base.AddToList (list);
	}

	//----------------------------------------------------------------------------
	protected override void DrawElement(Rect rect, int id, bool selected, bool focused)
	{
		MapDatabase.MapItem item = MapDatabase.GetMaByUniqueId(id);

		if (item != null)
		{
			if (!string.IsNullOrEmpty(item.m_name))
			{
				GUI.Label(rect, item.m_name);
			}
			else
			{
				GUI.Label(rect, "Un-named!");
			}

			if (selected)
			{
				if (m_currentlySelected != id)
				{
					m_listUnfolded = false;
					m_currentlySelected = id;
					this.Repaint();
				}
			}
		}
	}

	//----------------------------------------------------------------------------
	private void CreatePokemonNameList()
	{
		List<PokemonDatabase.PokemonItem> pokemonList = PokemonDatabase.GetPokemonList ();
		m_pokemonNames = new string[pokemonList.Count];

		foreach(PokemonDatabase.PokemonItem pokemon in pokemonList )
		{
			m_pokemonNames [pokemon.m_index] = pokemon.m_name;
		}
	}

	//----------------------------------------------------------------------------
	void AddNewWildPokemon (int index)
	{
		MapDatabase.CreateNewWildPokemon (index);
	}

	//----------------------------------------------------------------------------
	void RemoveWildPokemon(int index, MapDatabase.MapWildPokemon element)
	{
		MapDatabase.RemoveWildPokemon (index, element);
	}
}
