using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(AttackDatabase))]
public class AttackDatabaseEditor : DatabaseEditor 
{
	//----------------------------------------------------------------------------
	public override void OnEnable()
	{
		m_itemListProperty = this.serializedObject.FindProperty("m_attackItems");

		base.OnEnable ();
	}

	//----------------------------------------------------------------------------
	protected override void OnInternalInspectorGUI()
	{
		AttackDatabase.AttackItem item = AttackDatabase.GetAttackById(m_currentlySelected);

		if (item != null)
		{
			GUILayout.BeginHorizontal();
			{
				GUILayout.BeginVertical();
				{
					EditorGUILayout.LabelField ("Attack Index", item.m_index.ToString ());
					item.m_name = EditorGUILayout.TextField("Attack Name", item.m_name);

					item.m_description = EditorGUILayout.TextField("Attack description", item.m_description);

					GUILayout.BeginHorizontal ();
					{
						item.m_attackType = (EAttackType)EditorGUILayout.EnumPopup ("Nature of attack", item.m_attackType);
						item.m_type = (EPokemonType)EditorGUILayout.EnumPopup ("Type of attack", item.m_type);
					}
					GUILayout.EndHorizontal();

					item.m_puissance = EditorGUILayout.IntField ("Puissance", item.m_puissance);
					item.m_precision = EditorGUILayout.IntField ("Precision", item.m_precision);
					item.m_priorite = EditorGUILayout.IntField ("Priorite", item.m_priorite);
					item.m_pp = EditorGUILayout.IntField ("PP", item.m_pp);
				}
				GUILayout.EndVertical();
			}
			GUILayout.EndHorizontal();
		}

		if (GUI.changed)
		{
			EditorUtility.SetDirty(target);
		}
	}

	//----------------------------------------------------------------------------
	protected override void AddToList(ReorderableList list)
	{
		AttackDatabase.CreateNewAttack();
		base.AddToList (list);
	}

	//----------------------------------------------------------------------------
	protected override void DrawElement(Rect rect, int index, bool selected, bool focused)
	{
		AttackDatabase.AttackItem item = AttackDatabase.GetAttackById(index);

		if (item != null)
		{
			if (!string.IsNullOrEmpty(item.m_name))
			{
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
					m_currentlySelected = index;	
					this.Repaint();
				}
			}
		}
	}
}
