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
public class Edge
{
	public City start;
	public City end;
	public int cost;
	public bool stopSearch;

	public Edge (City c1, City c2, int cost)
	{
		start = c1;
		end = c2;
		this.cost = cost;
	}
}

