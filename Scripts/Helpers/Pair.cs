using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pair<T1, T2>
{
	public T1 first;
	public T2 second;

	public Pair(T1 _first, T2 _second)
	{
		first = _first;
		second = _second;
	}

	public override string ToString ()
	{
		return string.Format ("[Pair]<{0}, {1}>", first, second);
	}

	public static bool operator == (Pair<T1, T2> p1, Pair<T1, T2> p2)
	{
		if (Pair<T1, T2>.IsNull (p1) && !Pair<T1, T2>.IsNull (p2))
			return false;

		if (!Pair<T1, T2>.IsNull (p1) && Pair<T1, T2>.IsNull (p2))
			return false;

		if (Pair<T1, T2>.IsNull (p1) && Pair<T1, T2>.IsNull (p2))
			return true;

		return p1.first.Equals(p2.first) && p1.second.Equals(p2.second);
	}

	public static bool operator != (Pair<T1, T2> p1, Pair<T1, T2> p2)
	{
		return !(p1 == p2);
	}

	public override int GetHashCode()
	{
		int hash = 17;
		hash = hash * 23 + first.GetHashCode();
		hash = hash * 23 + second.GetHashCode();
		return hash;
	}

	public override bool Equals(object obj)
	{
		var other = obj as Pair<T1, T2>;
		if (object.ReferenceEquals (other, null))
			return false;
		else
			return EqualityComparer<T1>.Default.Equals (first, other.first) && EqualityComparer<T2>.Default.Equals (second, other.second);
	}

	private static bool IsNull(object obj)
	{
		return object.ReferenceEquals(obj, null);
	}
}
