﻿using System;
using System.Linq;
using ElectronicObserver.Data;
using ElectronicObserver.Properties.Data;
using ElectronicObserverTypes;
using ElectronicObserverTypes.Attacks;

namespace ElectronicObserver.Window.Tools.SortieRecordViewer.Sortie.Battle.Phase;

public sealed class AirBaseRaidAttackViewModel : AttackViewModelBase
{
	private int WaveIndex { get; }
	public BattleIndex DefenderIndex { get; }
	public IBaseAirCorpsData Defender { get; }
	private double Damage { get; }
	private HitType HitType { get; }
	private AirAttack AttackType { get; }
	public string DamageDisplay { get; }

	public string AttackerName => (WaveIndex, DefenderIndex.FleetFlag) switch
	{
		(> 0, _) => string.Format(BattleRes.AirSquadronWave, WaveIndex),
		(_, FleetFlag.Player) => BattleRes.EnemyAirSquadron,
		(_, FleetFlag.Enemy) => BattleRes.FriendlyAirSquadron,
		_ => "???",
	};

	public AirBaseRaidAttackViewModel(BattleFleets fleets, int waveIndex, AirBattleAttack attack)
	{
		WaveIndex = waveIndex;
		Damage = attack.Defenders.First().Damage;
		HitType = attack.Defenders.First().CriticalFlag;

		DefenderIndex = attack.Defenders.First().Defender;
		Defender = fleets.GetAirBase(DefenderIndex)!;
		AttackType = attack.AttackType;

		DamageDisplay =
			$"[{GetAttackKind(AttackType)}] " +
			$"{AttackDisplay(attack.Defenders.First().GuardsFlagship, Damage, HitType)}";

		int hpAfterAttacks = Math.Max(0, Defender.HPCurrent - (int)Damage);

		if (Defender.HPCurrent > 0 && Defender.HPCurrent != hpAfterAttacks)
		{
			DamageDisplay += $" ({Defender.HPCurrent} → {hpAfterAttacks})";
		}
	}

	private static string GetAttackKind(AirAttack airAttack) => airAttack switch
	{
		AirAttack.Torpedo => ConstantsRes.TorpedoAttack,
		AirAttack.Bombing => ConstantsRes.BombingAttack,
		AirAttack.TorpedoBombing => ConstantsRes.TorpBombingAttack,
		_ => ConstantsRes.Unknown,
	};
}
