using UnityEngine;
using System.Collections;

public class AssetSingleton<ScriptType> : ScriptableObject where ScriptType : ScriptableObject
{

	private static ScriptType m_instance = null;

    //------------------------------------------------------------------------ 
    //------------------------------------------------------------------------ 
    public static ScriptType Instance
    {
        get
        {
            if (m_instance != null)
            {
                return m_instance;
            }
            else
            {
                m_instance = (ScriptType)Resources.Load("AssetSingletons/" + typeof(ScriptType).Name, typeof(ScriptType));

                if (m_instance == null)
                {
                    Debug.Log("Unable to find AssetSingleton of type" + typeof(ScriptType).Name + " file!");
                }

                return m_instance;
            }
        }
    }
}
