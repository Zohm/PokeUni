using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections;

public class DatabaseWindow : EditorWindow
{
	private enum ECategories : int
	{
		eCategory_Pokemon = 0,
		eCategory_Attack,
		eCategory_Map,
		eCategory_Sign,
	}


	private int m_selectedCategory = 0;
	private string[] m_databaseCategories = new string[] { "Pokemon", "Attack", "Map", "Sign" };

	PokemonDatabaseEditor pokemonEditor = null;
	AttackDatabaseEditor attackEditor = null;
	MapDatabaseEditor mapEditor = null;
	SignDatabaseEditor signEditor = null;

	public void OnEnable()
	{
		pokemonEditor = (PokemonDatabaseEditor)Editor.CreateEditor (PokemonDatabase.Instance);
		pokemonEditor.OnEnable ();

		attackEditor = (AttackDatabaseEditor)Editor.CreateEditor (AttackDatabase.Instance);
		attackEditor.OnEnable ();

		mapEditor = (MapDatabaseEditor)Editor.CreateEditor (MapDatabase.Instance);
		mapEditor.OnEnable ();

		signEditor = (SignDatabaseEditor)Editor.CreateEditor (SignDatabase.Instance);
		signEditor.OnEnable ();
	}

	[MenuItem ("Database/Open")]
	public static void ShowWindow()
	{
		EditorWindow.GetWindow (typeof(DatabaseWindow), false, "Database");
	}

	public void OnGUI()
	{
		/*GUI.skin.button.margin = new RectOffset(0, 0, 0, 0);
		m_selectedCategory = GUI.SelectionGrid (new Rect(0, 0, 240, 40 ), m_selectedCategory, m_databaseCategories,3);*/
		
		m_selectedCategory = GUILayout.Toolbar (m_selectedCategory, m_databaseCategories, GUILayout.Width (80*m_databaseCategories.GetLength(0)), GUILayout.Height (20));
		switch( (ECategories)m_selectedCategory )
		{
		case ECategories.eCategory_Pokemon:
			pokemonEditor.OnInspectorGUI ();
			break;
		case ECategories.eCategory_Attack:
			attackEditor.OnInspectorGUI ();
			break;
		case ECategories.eCategory_Map:
			mapEditor.OnInspectorGUI ();
			break;
		case ECategories.eCategory_Sign:
			signEditor.OnInspectorGUI ();
			break;
		default:
			break;
		}
	}

	void DrawSeparation(int finalHeight)
	{
		Handles.BeginGUI ();
		//Handles.color = Color.black;
		Handles.DrawLine ( new Vector3(0, finalHeight), new Vector3(this.position.width, finalHeight) );
		Handles.DrawLine ( new Vector3(0, finalHeight++), new Vector3(this.position.width, finalHeight) );
		Handles.DrawLine ( new Vector3(0, finalHeight++), new Vector3(this.position.width, finalHeight) );
		Handles.EndGUI ();
	}
}
