using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(PokemonDatabase))]
public class PokemonDatabaseEditor : DatabaseEditor 
{
	//private Dictionary<int, PokemonItem> m_itemList = null;

	private string[] m_pokemonNames;
	private string m_oldPokemonName = "";
	private string m_newPokemonName = "";

	private string[] m_attackNames;

	bool m_delete = false;
	private KeyValuePair<int, PokemonDatabase.PokemonAttack> m_elementToDelete;

	bool m_listUnfolded = false;

	//----------------------------------------------------------------------------
	public override void OnEnable()
	{
		m_itemListProperty = this.serializedObject.FindProperty("m_pokemonItems");

		base.OnEnable ();

		CreatePokemonNameList ();
		CreateAttackNamesList ();
	}

	//----------------------------------------------------------------------------
	protected override void OnInternalInspectorGUI()
	{
		PokemonDatabase.PokemonItem item = PokemonDatabase.GetPokemonByIndex(m_currentlySelected);

		if (item != null)
		{
			GUILayout.BeginHorizontal();
			{
				GUILayout.BeginVertical();
				{
					item.m_index = EditorGUILayout.IntField ("Pokemon Index", item.m_index);

					m_oldPokemonName = item.m_name;
					item.m_name = EditorGUILayout.TextField("Pokemon Name", item.m_name);
					m_newPokemonName = item.m_name;

					GUILayout.BeginHorizontal ();
					{
						item.m_size = EditorGUILayout.FloatField("Pokemon Size", item.m_size);
						item.m_weight = EditorGUILayout.FloatField("Pokemon Weight", item.m_weight);
					}
					GUILayout.EndHorizontal();

					GUILayout.BeginHorizontal ();
					{
						item.m_tauxCapture = EditorGUILayout.FloatField("Taux Capture", item.m_tauxCapture);
						item.m_ratioMale = EditorGUILayout.FloatField("Ratio Male", item.m_ratioMale);
					}
					GUILayout.EndHorizontal();

					GUILayout.BeginHorizontal ();
					{
						item.m_type1 = (EPokemonType)EditorGUILayout.EnumPopup ("Type 1", item.m_type1);
						item.m_type2 = (EPokemonType)EditorGUILayout.EnumPopup ("Type 2", item.m_type2);
					}
					GUILayout.EndHorizontal();

					GUILayout.BeginHorizontal ();
					{
						item.m_pv = EditorGUILayout.IntField ("Base pv", item.m_pv);
						item.m_pv_given = EditorGUILayout.IntField ("EV pv given", item.m_pv_given);
					}
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal ();
					{
						item.m_atk = EditorGUILayout.IntField ("Base atk", item.m_atk);
						item.m_atk_given = EditorGUILayout.IntField ("EV atk given", item.m_atk_given);
					}
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal ();
					{
						item.m_def = EditorGUILayout.IntField ("Base def", item.m_def);
						item.m_def_given = EditorGUILayout.IntField ("EV def given", item.m_def_given);
					}
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal ();
					{
						item.m_vitesse = EditorGUILayout.IntField ("Base vitesse", item.m_vitesse);
						item.m_vitesse_given = EditorGUILayout.IntField ("EV vitesse given", item.m_vitesse_given);
					}
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal ();
					{
						item.m_atkspe = EditorGUILayout.IntField ("Base atkspe", item.m_atkspe);
						item.m_atkspe_given = EditorGUILayout.IntField ("EV atkspe given", item.m_atkspe_given);
					}
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal ();
					{
						item.m_defspe = EditorGUILayout.IntField ("Base defspe", item.m_defspe);
						item.m_defspe_given = EditorGUILayout.IntField ("EV defspe given", item.m_defspe_given);
					}
					GUILayout.EndHorizontal();

					item.m_baseXp = EditorGUILayout.IntField ("Base experience", item.m_baseXp);
					item.m_courbeExp = (ECourbesExperience)EditorGUILayout.EnumPopup ("Courbe d'experience", item.m_courbeExp);

					GUILayout.BeginHorizontal ();
					{
						item.m_lvlEvolution = EditorGUILayout.IntField ("Level evolution", item.m_lvlEvolution);
						item.m_evolution = EditorGUILayout.Popup ("Evolution", item.m_evolution, m_pokemonNames);
					}
					GUILayout.EndHorizontal();


					//item.m_sprite_fight_face = EditorGUILayout.ObjectField ("Sprite combat face", item.m_sprite_fight_face, typeof(Sprite), false) as Sprite;
					//item.m_sprite_fight_back = EditorGUILayout.ObjectField ("Sprite combat back", item.m_sprite_fight_back, typeof(Sprite), false) as Sprite;

					// Attack list
					GUILayout.BeginHorizontal ();
					{
						EditorGUILayout.LabelField ("Attacks by level");
						if( GUILayout.Button (m_listUnfolded ? "Hide" : "Show") )
						{
							m_listUnfolded = !m_listUnfolded;
						}
					}
					GUILayout.EndHorizontal();

					if( m_listUnfolded )
					{
						foreach(PokemonDatabase.PokemonAttack element in item.m_attacksByLvl)
						{
							GUILayout.BeginHorizontal ();
							{
								EditorGUILayout.LabelField(" ", GUILayout.MaxWidth(40));
								element.m_level = EditorGUILayout.IntField ("Level", element.m_level);
								EditorGUILayout.LabelField(" ", GUILayout.Width(40));
								element.m_attackId = EditorGUILayout.Popup ("Attack", element.m_attackId, m_attackNames);
								EditorGUILayout.LabelField(" ", GUILayout.Width(40));
								if (GUILayout.Button ("-")) 
								{
									m_delete = true;
									m_elementToDelete = new KeyValuePair<int, PokemonDatabase.PokemonAttack> (item.m_index, element);
								}
							}
							GUILayout.EndHorizontal();
						}

						GUILayout.BeginHorizontal ();
						{
							EditorGUILayout.LabelField(" ", GUILayout.MaxWidth(40));
							if( GUILayout.Button ("+") )
							{
								AddNewLevelAttack (item.m_index);
							}
						}
						GUILayout.EndHorizontal();
					}
				}
				GUILayout.EndVertical();

				GUILayout.BeginVertical();
				{
					item.m_sprite_fight_face = EditorGUILayout.ObjectField ("Sprite combat face", item.m_sprite_fight_face, typeof(Sprite), false) as Sprite;

					/*Rect rect_fight_face = GUILayoutUtility.GetRect(75.0f, 75.0f, GUILayout.MaxWidth(75.0f));
					if (item.m_sprite_fight_face)
					{
						GUI.DrawTexture(rect_fight_face, item.m_sprite_fight_face.texture);
					}*/
				}
				GUILayout.EndVertical();

				GUILayout.BeginVertical();
				{
					item.m_sprite_fight_back = EditorGUILayout.ObjectField ("Sprite combat back", item.m_sprite_fight_back, typeof(Sprite), false) as Sprite;
					/*Rect rect_fight_back = GUILayoutUtility.GetRect(75.0f, 75.0f, GUILayout.MaxWidth(75.0f));
					if (item.m_sprite_fight_back)
					{
						GUI.DrawTexture(rect_fight_back, item.m_sprite_fight_back.texture);
					}*/
				}
				GUILayout.EndVertical();
			}
			GUILayout.EndHorizontal();
		}

		// Delay deletion so we don't do it during the for each loop
		if( m_delete )
		{
			RemoveLevelAttack (m_elementToDelete.Key, m_elementToDelete.Value);
			m_delete = false;
		}

		if (GUI.changed)
		{
			//Can't have same double type
			if(item.m_type1 == item.m_type2)
			{
				item.m_type2 = EPokemonType.Default;
			}

			UpdatePokemonNamesList ();
			EditorUtility.SetDirty(target);
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
	private void CreateAttackNamesList()
	{
		List<AttackDatabase.AttackItem> attackList = AttackDatabase.GetAttackList ();
		m_attackNames = new string[attackList.Count];

		foreach(AttackDatabase.AttackItem attack in attackList )
		{
			m_attackNames [attack.m_index] = attack.m_name;
		}
	}

	//----------------------------------------------------------------------------
	public void UpdatePokemonNamesList()
	{
		if( !m_oldPokemonName.Equals (m_newPokemonName) )
		{
			CreatePokemonNameList ();
			m_oldPokemonName = m_newPokemonName;
		}
	}

	//----------------------------------------------------------------------------
	protected override void AddToList(ReorderableList list)
	{
		PokemonDatabase.CreateNewPokemon();
		base.AddToList (list);
	}

	//----------------------------------------------------------------------------
	protected override void DrawElement(Rect rect, int index, bool selected, bool focused)
	{
		PokemonDatabase.PokemonItem item = PokemonDatabase.GetPokemonByIndex(index);

		if (item != null)
		{
			if (!string.IsNullOrEmpty(item.m_name))
			{
				if( item.m_index < 10 )
					GUI.Label(rect, "00" + item.m_index.ToString () + ": " + item.m_name);
				else if( item.m_index < 100 )
					GUI.Label(rect, "0" + item.m_index.ToString () + ": " + item.m_name);
				else
					GUI.Label(rect, item.m_index.ToString () + ": " + item.m_name);
			}
			else
			{
				GUI.Label(rect, "Un-named!");
			}

			if (selected)
			{
				if (m_currentlySelected != index)
				{
					m_listUnfolded = false;
					m_currentlySelected = index;	
					this.Repaint();
				}
			}
		}
	}

	//----------------------------------------------------------------------------
	void AddNewLevelAttack (int index)
	{
		PokemonDatabase.CreateNewLevelAttack (index);
	}

	//----------------------------------------------------------------------------
	void RemoveLevelAttack(int index, PokemonDatabase.PokemonAttack element)
	{
		PokemonDatabase.RemoveLevelAttack (index, element);
	}
}
