using UnityEngine;
using System.Collections;

public class ScreenFader : MonoBehaviour 
{
	Animator animator;
	bool isFading = false;

	// Use this for initialization
	void Start () 
	{
		animator = GetComponent<Animator> ();
	}
	
	public IEnumerator FadeToClear()
	{
		isFading = true;
		animator.SetTrigger ("FadeIn");

		while (isFading)
			yield return null;
	}

	public IEnumerator FadeToBlack()
	{
		isFading = true;
		animator.SetTrigger ("FadeOut");

		while (isFading)
			yield return null;
	}

	void AnimationComplete()
	{
		isFading = false;
	}
}
