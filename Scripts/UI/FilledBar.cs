using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum EFillSpeed
{
	Slow,
	Normal,
	Fast,
	VeryFast
}

public class FilledBar : MonoBehaviour 
{
	private Image m_bar = null;
	private float m_fillAmount = 1.0f;
	private float m_maxValue;

	private EFillSpeed m_lerpSpeed;

	//Color changing
	public bool m_isChangingColor = false;
	public float ratioColor1;
	public Color m_fullColor;
	public float ratioColor2;
	public Color m_mediumColor;
	public Color m_lowColor;

	//Text
	public bool m_hasText = false;
	public Text m_text_remaining;
	public Text m_text_max;

	//----------------------------------------------------------------------------
	// On object creation
	void Awake()
	{
		m_bar = gameObject.GetComponent<Image> ();
	}

	//----------------------------------------------------------------------------
	// On start display. Called from the manager handling the image bar
	public void InitBar(float fillAmount, float maxValue)
	{
		m_maxValue = maxValue;
		m_fillAmount = MapAmount (fillAmount, 0f, m_maxValue, 0f, 1f);
		m_bar.fillAmount = m_fillAmount;

		InitColor ();
		InitText (fillAmount);
	}

	//----------------------------------------------------------------------------
	void InitColor()
	{
		if( m_isChangingColor )
		{
			if( m_fillAmount > ratioColor1 )
			{
				m_bar.color = m_fullColor;
			}
			else if( m_fillAmount > ratioColor2 )
			{
				m_bar.color = m_mediumColor;
			}
			else
			{
				m_bar.color = m_lowColor;
			}
		}
	}

	//----------------------------------------------------------------------------
	void InitText(float fillAmount)
	{
		if( m_hasText )
		{
			m_text_max.text = m_maxValue.ToString ();
			m_text_remaining.text = fillAmount.ToString ();
		}
	}

	//----------------------------------------------------------------------------
	// Upate bar values if needed
	void Update () 
	{
		if(m_bar.fillAmount != m_fillAmount)
		{
			UpdateBarAmount ();
			UpdateBarColor ();
			UpdateText ();
		}
	}

	//----------------------------------------------------------------------------
	// To set a new value for the bar
	public void SetFillAmount(float amount, EFillSpeed speed)
	{
		m_fillAmount = MapAmount (amount, 0f, m_maxValue, 0f, 1f);
		m_lerpSpeed = speed;
	}

	//----------------------------------------------------------------------------
	// Map container range to the container one [0,1]
	float MapAmount(float value, float inMin, float inMax, float outMin, float outMax)
	{
		return (value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
	}

	//----------------------------------------------------------------------------
	// return lerp speed from enum value
	float GetSpeed()
	{
		switch(m_lerpSpeed)
		{
		case EFillSpeed.Slow:
			return 0.5f;
		case EFillSpeed.Normal:
			return 1.0f;
		case EFillSpeed.Fast:
			return 2.0f;
		case EFillSpeed.VeryFast:
			return 4.0f;
		default:
			return 1.0f;
		}
	}

	//----------------------------------------------------------------------------
	void UpdateBarAmount()
	{
		if( m_bar.fillAmount - m_fillAmount < 0.005f )
		{
			m_bar.fillAmount = m_fillAmount;
			OnBarUpdateFinished ();
		}
		else
		{
			m_bar.fillAmount = Mathf.Lerp (m_bar.fillAmount, m_fillAmount, Time.deltaTime * GetSpeed ());
		}
	}

	//----------------------------------------------------------------------------
	void OnBarUpdateFinished()
	{
		// TEMP hack until combat animation system is implemented
		BattleManager battleManager = FindObjectOfType<BattleManager> ();
		if( battleManager != null ) {
			battleManager.OnLifeBarUpdateFinished ();
		}
	}

	//----------------------------------------------------------------------------
	void UpdateBarColor()
	{
		// if color changing feature is activated, update bar color aswell
		if( m_isChangingColor )
		{
			if( m_fillAmount > ratioColor1 )
			{
				m_bar.color = m_fullColor;
			}
			else if( m_fillAmount > ratioColor2 )
			{
				m_bar.color = m_mediumColor;
			}
			else
			{
				m_bar.color = m_lowColor;
			}

			//m_bar.color = Color.Lerp (m_lowColor, m_fullColor, m_bar.fillAmount);
		}
	}

	void UpdateText()
	{
		if( m_hasText )
		{
			m_text_remaining.text = ( Mathf.Floor (MapAmount (m_bar.fillAmount, 0, 1, 0, m_maxValue)) ).ToString ();
		}
	}
}
