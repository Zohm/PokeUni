using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(SignDatabase))]
public class SignDatabaseEditor : DatabaseEditor 
{
	private string[] m_mapNames;

	//----------------------------------------------------------------------------
	public override void OnEnable()
	{
		m_itemListProperty = this.serializedObject.FindProperty("m_signItems");

		base.OnEnable ();

		CreateMapNameList ();
	}

	//----------------------------------------------------------------------------
	protected override void OnInternalInspectorGUI()
	{
		SignDatabase.SignItem item = SignDatabase.GetSignById(m_currentlySelected);

		if (item != null)
		{
			GUILayout.BeginHorizontal();
			{
				GUILayout.BeginVertical();
				{
					EditorGUILayout.LabelField ("Sign Index", item.m_uniqueId.ToString ());

					item.m_mapUniqueId = EditorGUILayout.Popup ("Map", item.m_mapUniqueId, m_mapNames);
					item.m_indexSignOnMap = EditorGUILayout.IntField ("Index sign on map", item.m_indexSignOnMap);

					//Automaticaly filled
					EditorGUILayout.LabelField ("Sign Position", item.m_position.ToString ("0.00"));

					item.m_signText = EditorGUILayout.TextField ("Sign's text", item.m_signText);	//TextArea?
				}
				GUILayout.EndVertical();
			}
			GUILayout.EndHorizontal();
		}

		if (GUI.changed)
		{
			UpdateSignObjectAndPosition (item);

			EditorUtility.SetDirty(target);
		}
	}

	//----------------------------------------------------------------------------
	private void CreateMapNameList()
	{
		List<MapDatabase.MapItem> mapList = MapDatabase.GetMapList ();
		m_mapNames = new string[mapList.Count];

		foreach(MapDatabase.MapItem map in mapList )
		{
			m_mapNames [map.m_uniqueId] = map.m_name;
		}
	}

	//----------------------------------------------------------------------------
	private void UpdateSignObjectAndPosition ( SignDatabase.SignItem item )
	{
		MapDatabase.MapItem map = MapDatabase.GetMaByUniqueId (item.m_mapUniqueId);
		if( map != null )
		{
			foreach( Transform obj in map.m_mapPrefab.transform )
			{
				GameObject sign = obj.gameObject;
				if( sign.name.Equals ("Sign") )
				{
					foreach( Transform insideObj in sign.transform )
					{
						GameObject signCollision = insideObj.gameObject;
						if (signCollision.name.Equals ("Collision")) 
						{
							PolygonCollider2D collider = signCollision.GetComponent<PolygonCollider2D> ();
							if( collider && item.m_indexSignOnMap < collider.pathCount )
							{
								item.m_position = collider.GetPath (item.m_indexSignOnMap)[1];
								return;
							}
						}
					}
				}
			}
		}
	}

	//----------------------------------------------------------------------------
	protected override void AddToList(ReorderableList list)
	{
		SignDatabase.CreateNewSign();
		base.AddToList (list);
	}

	//----------------------------------------------------------------------------
	protected override void DrawElement(Rect rect, int index, bool selected, bool focused)
	{
		SignDatabase.SignItem item = SignDatabase.GetSignById(index);

		if (item != null)
		{
			if (!string.IsNullOrEmpty(item.m_signText))
			{
				GUI.Label(rect, m_mapNames[item.m_mapUniqueId] + "_Sign_" + item.m_indexSignOnMap.ToString ());
			}
			else
			{
				GUI.Label(rect, "NewSign!");
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
