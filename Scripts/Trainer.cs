using UnityEngine;
using System.Collections;

public class Trainer : MonoBehaviour 
{
	//Start - Move to base class 
	///string name = "";
	// End - Move

	ArrayList pokemonTeam = new ArrayList();

	// TEMP - Until player gets pokemon by story script
	private void Start()
	{
		Pokemon pokemon = Pokemon.CreateInstance<Pokemon> ();
		pokemon.GeneratePokemon (6, 20);
		pokemonTeam.Add (pokemon);
	}

	void AddPokemonToTeam(Pokemon pokemon)
	{
		//Maximum 6 pokemon in the team
		if( pokemonTeam.Count < 6 )
		{
			pokemonTeam.Add (pokemon);
		}
	}

	void RemovePokemonFromTeam(int index)
	{
		//Minimum 1 pokemon in the team
		if( pokemonTeam.Count > 1 )
		{
			pokemonTeam.RemoveAt (index);
		}
	}

	public Pokemon GetPokemonFromTeamAtIndex(int index)
	{
		if( index < pokemonTeam.Count )
		{
			return (Pokemon)pokemonTeam [index];
		}
		return null;
	}
}
