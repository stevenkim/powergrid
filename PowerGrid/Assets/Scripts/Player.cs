// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 4.0.30319.1
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
public class Player  : MonoBehaviour, IComparable
{
	public int cash;
	public ArrayList powerPlants = new ArrayList();
	public ArrayList cities = new ArrayList();
	
	public Player ()
	{
		Reset();
	}

	public int GetOrderScore() {
		int maxPowerPlantValue = 0;
		foreach (PowerPlant pp in powerPlants) {
			if(maxPowerPlantValue > pp.baseCost)
				maxPowerPlantValue = pp.baseCost;
		}

		return 1000000 * cities.Count + 1000*maxPowerPlantValue + cash;
	}

	public int CompareTo( object p) {
		return GetOrderScore().CompareTo (((Player)p).GetOrderScore());
	}

	public int GetCityCount() {
		return cities.Count;
	}

	public void Reset() {
		powerPlants.Clear ();
		cash = 50;
		cities.Clear ();
	}

	public override string ToString() {
		string info = name + " cash:" + cash + " [";

		foreach (PowerPlant pp in powerPlants)
			info += pp.ToString ();
		info += "] " + cities.Count;
		return info;
	}
}

