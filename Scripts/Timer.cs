using UnityEngine;
using System.Collections;

public class Timer 
{
	const float UNLIMITED_LIFETIME = -1f;

	float m_lifeTime;
	float m_startTime;

	public Timer(float lifeTime = UNLIMITED_LIFETIME)
	{
		SetLifeTime (lifeTime);
	}

	public void SetInfinite()
	{
		SetLifeTime (UNLIMITED_LIFETIME);
	}

	public void SetExpired()
	{
		SetLifeTime (0f);
	}

	public void SetLifeTime(float lifeTime)
	{
		Reset ();
		m_lifeTime = lifeTime;
	}

	public float GetLifeTime()
	{
		return m_lifeTime;
	}

	/*
	 * Time since the start of the timer 
	 */
	float GetCurrentTime()
	{
		return GetTime() - m_startTime;
	}

	/*
	 * Time remaining before the timer is considered expired (time to reach set life time)
	 */
	float GetRemaining()
	{
		return Mathf.Max (0f, m_lifeTime - GetCurrentTime ());
	}

	void Reset()
	{
		m_lifeTime = UNLIMITED_LIFETIME;
		m_startTime = GetTime();
	}

	/*
	 * Has the time reach its life time, or is the lifetime set to 0
	 */
	public bool HasExpired()
	{
		if( IsInfinite() )
		{
			return false;
		}
		return m_lifeTime == 0f || GetCurrentTime () >= m_lifeTime;
	}

	public bool IsInfinite()
	{
		return m_lifeTime == UNLIMITED_LIFETIME;
	}

	/*
	 * Get time since the game started
	 */
	float GetTime()
	{
		return Time.time;
	}
}
