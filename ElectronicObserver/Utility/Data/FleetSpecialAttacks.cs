﻿using System.Collections.Generic;
using System.Linq;
using ElectronicObserverTypes;
using ElectronicObserverTypes.Attacks.Specials;

namespace ElectronicObserver.Utility.Data;

public static class FleetSpecialAttacks
{
	public static List<SpecialAttack> GetSpecialAttacks(this IFleetData fleet)
	{
		List<SpecialAttack> attacks = new List<SpecialAttack>()
		{
			new NelsonSpecialAttack(fleet),
			new NagatoSpecialAttack(fleet),
			new ColoradoSpecialAttack(fleet),
			new Yamato123SpecialAttack(fleet),
			new Yamato12SpecialAttack(fleet),
			new KongouSpecialAttack(fleet),
			new SubmarineSpecialAttack(fleet),
		};	

		return attacks
			.Where(attack => attack.CanTrigger())
			.ToList();
	}
}
