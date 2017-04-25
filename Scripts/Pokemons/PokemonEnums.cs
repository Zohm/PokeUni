using UnityEngine;
using System.Collections;

//-----------------------------------------------------------------------------------------------------------
public enum EPokemonType : int
{
	Default = -1,
	Acier = 0,
	Combat,
	Dragon,
	Eau,
	Electrik,
	Fee,
	Feu,
	Glace,
	Insecte,
	Normal,
	Plante,
	Poison,
	Psy,
	Roche,
	Sol,
	Spectre,
	Tenebres,
	Vol
}

//-----------------------------------------------------------------------------------------------------------
public enum EPokemonPhysicalStatus : int
{
	Default = 0,
	Gele,
	Para,
	Pois,
	Brul,
	Dodo,
}

//-----------------------------------------------------------------------------------------------------------
public enum EPokemonMentalStatus : int
{
	Default = 0,
	Confus,
	Love,
}

//-----------------------------------------------------------------------------------------------------------
public enum EPokemonNatures : int
{
	Default = 0,
	Assure,
	Bizarre,
	Brave,
	Calme,
	Discret,
	Docile,
	Doux,
	Foufou,
	Gentil,
	Hardi,
	Jovial,
	Lache,
	Malin,
	Malpoli,
	Mauvais,
	Modeste,
	Naif,
	Presse,
	Prudent,
	Pudique,
	Relax,
	Rigide,
	Serieux,
	Solo,
	Timide,

	MAX //Must always be last 
}

//-----------------------------------------------------------------------------------------------------------
public enum ECourbesExperience : int
{
	Rapide = 0,
	Moyenne,
	Parabolique,
	Lente,
	Erratique,
	Fluctuante
}

//-----------------------------------------------------------------------------------------------------------
public enum EStatType : int
{
	Base = 0,
	Current = 1,
	IV = 2,
	EV = 3
}
