using UnityEngine;
using System.Collections;

public class Warper : MonoBehaviour 
{
	public Transform arrival;
	public GameObject arrivalMap;

	public IEnumerator TeleportToNewPos(System.Action<Vector3> pos)
	{
		ScreenFader sf = GameObject.FindGameObjectWithTag ("Fader").GetComponent<ScreenFader> ();
		yield return StartCoroutine (sf.FadeToBlack ());

		// Change Player's position
		GameObject player = GameObject.FindGameObjectWithTag ("Player");
		player.transform.position = arrival.position;
		pos(arrival.position);

		//Update current map tag
		GameObject currentMap = GameObject.FindGameObjectWithTag ("CurrentMap");
		currentMap.tag = "Untagged";
		arrivalMap.tag = "CurrentMap";

		yield return StartCoroutine (sf.FadeToClear ());
		player.GetComponent<PlayerActions> ().m_playerBlocked = false ;
	}
}
