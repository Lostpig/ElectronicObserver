﻿using System.Collections.Generic;
using ElectronicObserverTypes;
using ElectronicObserverTypes.AntiAir;
using ElectronicObserverTypes.Mocks;
using Xunit;

namespace ElectronicObserverCoreTests;

[Collection(DatabaseCollection.Name)]
public class AntiAirCutInTests
{
	private DatabaseFixture Db { get; }

	public AntiAirCutInTests(DatabaseFixture db)
	{
		Db = db;
	}

	[Fact(DisplayName = "Regular destroyer")]
	public void AntiAirCutInTest1()
	{
		ShipDataMock kamikaze = new(Db.MasterShips[ShipId.KamikazeKai])
		{
			SlotInstance = new List<IEquipmentData?>
			{
				new EquipmentDataMock(Db.MasterEquipment[EquipmentId.MainGunSmall_10cmTwinHighangleGun_AAFD]),
				new EquipmentDataMock(Db.MasterEquipment[EquipmentId.MainGunSmall_10cmTwinHighangleGun_AAFD]),
				new EquipmentDataMock(Db.MasterEquipment[EquipmentId.RadarSmall_Type13AirRadarKai_LateModel]),
			},
		};

		List<AntiAirCutIn> cutins = AntiAirCutIn.PossibleCutIns(kamikaze);

		Assert.Equal(3, cutins.Count);
		Assert.Equal(5, cutins[0].Id);
		Assert.Equal(8, cutins[1].Id);
		Assert.Equal(0, cutins[2].Id);
	}

	[Fact(DisplayName = "Destroyer with special cut-in")]
	public void AntiAirCutInTest2()
	{
		ShipDataMock isokaze = new(Db.MasterShips[ShipId.IsokazeBKai])
		{
			SlotInstance = new List<IEquipmentData?>
			{
				new EquipmentDataMock(Db.MasterEquipment[EquipmentId.MainGunSmall_10cmTwinHighangleGun_AAFD]),
				new EquipmentDataMock(Db.MasterEquipment[EquipmentId.MainGunSmall_10cmTwinHighangleGun_AAFD]),
				new EquipmentDataMock(Db.MasterEquipment[EquipmentId.RadarSmall_Type13AirRadarKai_LateModel]),
			},
		};

		List<AntiAirCutIn> cutins = AntiAirCutIn.PossibleCutIns(isokaze);

		// todo: this isn't confirmed yet
		Assert.Equal(4, cutins.Count);
		Assert.Equal(29, cutins[0].Id);
		Assert.Equal(5, cutins[1].Id);
		Assert.Equal(8, cutins[2].Id);
		Assert.Equal(0, cutins[3].Id);
	}

	[Fact(DisplayName = "Atlanta")]
	public void AntiAirCutInTest3()
	{
		ShipDataMock atlanta = new(Db.MasterShips[ShipId.AtlantaKai])
		{
			SlotInstance = new List<IEquipmentData?>
			{
				new EquipmentDataMock(Db.MasterEquipment[EquipmentId.MainGunMedium_GFCSMk_37_5inchTwinDualpurposeGunMount_ConcentratedDeployment]),
				new EquipmentDataMock(Db.MasterEquipment[EquipmentId.MainGunMedium_GFCSMk_37_5inchTwinDualpurposeGunMount_ConcentratedDeployment]),
				new EquipmentDataMock(Db.MasterEquipment[EquipmentId.RadarSmall_GFCSMk_37]),
			},
		};

		List<AntiAirCutIn> cutins = AntiAirCutIn.PossibleCutIns(atlanta);

		// todo: this isn't confirmed yet
		Assert.Equal(6, cutins.Count);
		Assert.Equal(38, cutins[0].Id);
		Assert.Equal(40, cutins[1].Id);
		Assert.Equal(41, cutins[2].Id);
		Assert.Equal(5, cutins[3].Id);
		Assert.Equal(8, cutins[4].Id);
		Assert.Equal(0, cutins[5].Id);
	}
}
