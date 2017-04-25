using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections;

public class DatabaseEditor : Editor 
{
	protected SerializedProperty m_itemListProperty = null;
	protected ReorderableList m_itemList = null;

	protected int m_currentlySelected = 0;


	//----------------------------------------------------------------------------
	public virtual void OnEnable()
	{
		if (this.m_itemList == null)
		{
			this.m_itemList = new ReorderableList(this.serializedObject, this.m_itemListProperty, true, false, true, true);
			this.m_itemList.onAddCallback = this.AddToList;
			this.m_itemList.onRemoveCallback = this.RemoveFromList;
			this.m_itemList.drawElementCallback = this.DrawElement;
			this.m_itemList.elementHeight = EditorGUIUtility.singleLineHeight + 2f;
			this.m_itemList.headerHeight = 3f;
		}
	}

	//----------------------------------------------------------------------------
	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();

		OnInternalInspectorGUI ();

		this.m_itemList.DoLayoutList();
		this.serializedObject.ApplyModifiedProperties();
	}

	//----------------------------------------------------------------------------
	protected virtual void OnInternalInspectorGUI() {}

	//----------------------------------------------------------------------------
	protected virtual void AddToList(ReorderableList list)
	{
		this.serializedObject.Update();
		list.index = list.serializedProperty.arraySize - 1;
		this.serializedObject.ApplyModifiedProperties();
	}

	//----------------------------------------------------------------------------
	protected virtual void RemoveFromList(ReorderableList list)
	{
		ReorderableList.defaultBehaviours.DoRemoveButton(list);
		this.m_itemList.DoLayoutList();
		this.serializedObject.ApplyModifiedProperties();
		this.serializedObject.Update();
	}

	//----------------------------------------------------------------------------
	protected virtual void DrawElement(Rect rect, int index, bool selected, bool focused) {}
}
