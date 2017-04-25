using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour 
{
	public GameObject m_dialogueBox;
	public Text m_text;
	public GameObject m_arrow;
	public float m_textAnimationSpeed = 0.02f;

	private bool m_isBoxActive = false;
	private bool m_isArrowActive = false;

	private string m_textToDisplay = "";
	private int m_indexLetterText = 0;

	//----------------------------------------------------------------------------
	// Use this for initialization
	void Start () 
	{
		m_isBoxActive = false;
		m_dialogueBox.SetActive (false);

		m_isArrowActive = false;
		m_arrow.SetActive (false);
	}

	//----------------------------------------------------------------------------
	// Update is called once per frame
	void Update () 
	{
		if( m_isBoxActive )
		{
			if( Input.GetKeyUp (KeyCode.A) )
			{
				if( m_isArrowActive )
				{
					m_isArrowActive = false;
					m_arrow.SetActive (false);
					StartCoroutine (AnimateText(m_textToDisplay));
				}
			}

			if( Input.GetKeyUp (KeyCode.B) )
			{
				if( !m_isArrowActive )
				{
					m_isBoxActive = false;
					m_dialogueBox.SetActive (false);

					PlayerActions playerActionComp = FindObjectOfType<PlayerActions> ();
					if( playerActionComp )
					{
						playerActionComp.m_playerBlocked = false;
					}
				}
			}
		}
	}

	//----------------------------------------------------------------------------
	public void ReadSign(string text)
	{
		//Open dialogue box
		m_isBoxActive = true;
		m_dialogueBox.SetActive (true);

		DisplayText(text);
	}

	//----------------------------------------------------------------------------
	public void DisplayText(string text)
	{
		m_textToDisplay = text;
		m_indexLetterText = 0;

		StartCoroutine (AnimateText(text));
	}

	//----------------------------------------------------------------------------
	IEnumerator AnimateText(string completeText)
	{
		m_text.text = "";
		int countLettersCurrenLine = 0;
		int countLines = 0;

		while( m_indexLetterText < completeText.Length && !m_isArrowActive )
		{
			if( completeText [m_indexLetterText] == ' ' )
			{
				if( CheckIfShouldGoToLine (m_indexLetterText, completeText, countLettersCurrenLine) )
				{
					if( countLines == 0 )
					{
						m_text.text += "\n";
						m_indexLetterText++;
						countLettersCurrenLine = 0;
						countLines++;
					}
					else
					{
						m_isArrowActive = true;
						m_arrow.SetActive (true);
					}
				}
			}

			m_text.text += completeText [m_indexLetterText++];
			countLettersCurrenLine++;

			yield return new WaitForSeconds (0.05f);
		}
	}

	//----------------------------------------------------------------------------
	bool CheckIfShouldGoToLine(int index, string completeText, int countLettersCurrenLine)
	{
		int count = 1;
		while( index + count < completeText.Length && completeText[index + count] != ' ' )
		{
			count++;
		}

		return (countLettersCurrenLine + count) * 8 > m_text.rectTransform.rect.width;
	}
}
