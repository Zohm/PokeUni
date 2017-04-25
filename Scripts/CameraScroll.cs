using UnityEngine;
using System.Collections;

public class CameraScroll : MonoBehaviour {

	public GameObject m_player;
	private Transform m_playerTransform;
	private GameObject m_currentMap;

	// Use this for initialization
	void Start () 
	{
		if( m_player )
		{
			m_playerTransform = m_player.transform;
			//Since we only update the camera position when the player is moving, we need to update it once at start to center it on the player
			UpdateScroll ();
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		UpdateScroll();
	}

	void UpdateScroll ()
	{
		if (m_playerTransform) 
		{
			m_currentMap = GameObject.FindWithTag ("CurrentMap");
			if( m_currentMap != null )
			{
				Vector3 mapPos = m_currentMap.transform.position;
				Tiled2Unity.TiledMap mapInfos = m_currentMap.GetComponent<Tiled2Unity.TiledMap> ();

				float cameraHalfVerticalSize = Camera.main.orthographicSize;
				float cameraHalfHorizontalSize = cameraHalfVerticalSize * Screen.width / Screen.height;

				float min_x = 0.0f;
				float max_x = 0.0f;
				// If the map width is less than the camera horizontal size, clip it on the middle of the middle of the map so it does not move on the X axis
				if( cameraHalfHorizontalSize * 2f >= mapInfos.MapWidthInPixels * 0.01f )
				{
					min_x = max_x = mapPos.x + mapInfos.MapWidthInPixels * 0.01f * 0.5f;
				}
				else //else the camera is centered on the player when far from an edge, and clamp to the edge when next to it
				{
					min_x = mapPos.x + cameraHalfHorizontalSize;
					max_x = mapPos.x + mapInfos.MapWidthInPixels * 0.01f - cameraHalfHorizontalSize;
				}

				float min_y = 0.0f;
				float max_y = 0.0f; 
				if( cameraHalfVerticalSize * 2f >= mapInfos.MapHeightInPixels * 0.01f )
				{
					min_y = max_y = mapPos.y - mapInfos.MapHeightInPixels * 0.01f * 0.5f;
				}
				else
				{
					min_y = cameraHalfVerticalSize + mapPos.y - mapInfos.MapHeightInPixels * 0.01f;
					max_y = mapPos.y - cameraHalfVerticalSize;
				}

				transform.position = new Vector3 (
					Mathf.Clamp (m_playerTransform.position.x, min_x, max_x), 
					Mathf.Clamp (m_playerTransform.position.y, min_y, max_y), 
					transform.position.z);
			}
		}
	}
}
