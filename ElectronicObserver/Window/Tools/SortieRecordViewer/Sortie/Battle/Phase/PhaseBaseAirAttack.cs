﻿using System.Collections.Generic;
using System.Linq;
using ElectronicObserver.KancolleApi.Types.Models;
using ElectronicObserver.Properties.Data;

namespace ElectronicObserver.Window.Tools.SortieRecordViewer.Sortie.Battle.Phase;

public class PhaseBaseAirAttack : PhaseBase
{
	public override string Title => BattleRes.BattlePhaseLandBasedAir;

	public List<PhaseBaseAirAttackUnit> Units { get; } = new();

	public PhaseBaseAirAttack(List<ApiAirBaseAttack> apiAirBaseAttack)
	{
		foreach (PhaseBaseAirAttackUnit attackUnit in apiAirBaseAttack.Select((ab, i) => new PhaseBaseAirAttackUnit(ab, i)))
		{
			Units.Add(attackUnit);
		}
	}

	public override BattleFleets EmulateBattle(BattleFleets battleFleets)
	{
		FleetsBeforePhase = battleFleets.Clone();

		foreach (PhaseBaseAirAttackUnit attackUnit in Units)
		{
			battleFleets = attackUnit.EmulateBattle(battleFleets);
		}

		FleetsAfterPhase = battleFleets.Clone();

		return battleFleets;
	}
}
