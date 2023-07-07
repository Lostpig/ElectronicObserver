﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ElectronicObserverTypes.Mocks;

public class FleetDataMock : IFleetData
{
	public int FleetID { get; set; }
	public string Name { get; set; }
	public FleetType FleetType { get; set; }
	public int ExpeditionState { get; set; }
	public int ExpeditionDestination { get; set; }
	public DateTime ExpeditionTime { get; set; }
	public ReadOnlyCollection<int>? Members => MembersInstance switch
	{
		null => null,
#pragma warning disable S2365 // Properties should not make collection or array copies
		_ => new(MembersInstance.Select(s => s?.ShipID ?? -1).ToList()),
#pragma warning restore S2365 // Properties should not make collection or array copies
	};
	public ReadOnlyCollection<IShipData?>? MembersInstance { get; set; }
	public ReadOnlyCollection<IShipData?>? MembersWithoutEscaped => MembersInstance switch
	{
		{ } ships => Array.AsReadOnly(ships
			.Select((s, i) => EscapedShipList.Contains(i) switch
			{
				true => null,
				false => s,
			})
			.ToArray()),

		_ => null,
	};
	public ReadOnlyCollection<int> EscapedShipList { get; set; } = new(Array.Empty<int>());
	public bool IsInSortie { get; set; }
	public bool IsInPractice { get; set; }
	public int ID { get; set; }
	public int SupportType { get; set; }
	public bool IsFlagshipRepairShip { get; set; }
	public bool CanAnchorageRepair { get; set; }
	public DateTime? ConditionTime { get; set; }
	public Dictionary<string, string> RequestData { get; set; }
	public dynamic RawData { get; set; }
	public bool IsAvailable { get; set; }

	public int this[int i]
	{
		get => throw new NotImplementedException();
		set => throw new NotImplementedException();
	}

	public void LoadFromResponse(string apiname, dynamic data)
	{
		throw new NotImplementedException();
	}

	public void LoadFromRequest(string apiname, Dictionary<string, string> data)
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// The index in the api data is 1-based.
	/// </summary>
	/// <param name="index">1-based ship index</param>
	public void Escape(int index)
	{
		EscapedShipList = new(EscapedShipList.Append(index - 1).ToList());
	}

	public int GetAirSuperiority()
	{
		throw new NotImplementedException();
	}

	public string GetAirSuperiorityString()
	{
		throw new NotImplementedException();
	}

	public double GetSearchingAbility()
	{
		throw new NotImplementedException();
	}

	public string GetSearchingAbilityString(int branchWeight = 1)
	{
		throw new NotImplementedException();
	}

	public double GetContactProbability()
	{
		throw new NotImplementedException();
	}

	public Dictionary<int, double> GetContactSelectionProbability()
	{
		throw new NotImplementedException();
	}

	public void UpdateConditionTime()
	{
		throw new NotImplementedException();
	}
}
