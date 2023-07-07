﻿using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using ElectronicObserver.Data;
using ElectronicObserver.Resource;
using ElectronicObserver.Utility.Data;
using ElectronicObserver.Utility.Mathematics;
using ElectronicObserver.ViewModels.Translations;
using ElectronicObserverTypes;

namespace ElectronicObserver.Window.Wpf.BaseAirCorps;

public class BaseAirCorpsItemViewModel : ObservableObject
{
	public FormBaseAirCorpsTranslationViewModel FormBaseAirCorps { get; }

	public BaseAirCorpsItemControlViewModel Name { get; }
	public BaseAirCorpsItemControlViewModel ActionKind { get; }
	public BaseAirCorpsItemControlViewModel AirSuperiority { get; }
	public BaseAirCorpsItemControlViewModel Distance { get; }
	public BaseAirCorpsSquadronViewModel Squadrons { get; }

	public int MapAreaId { get; set; }
	public Visibility Visibility { get; set; } = Visibility.Collapsed;
	public ICommand CopyOrganizationCommand { get; }
	public ICommand DisplayRelocatedEquipmentsCommand { get; }

	public BaseAirCorpsItemViewModel(ICommand copyOrganizationCommand, ICommand displayRelocatedEquipmentsCommand)
	{
		FormBaseAirCorps = Ioc.Default.GetService<FormBaseAirCorpsTranslationViewModel>()!;

		CopyOrganizationCommand = copyOrganizationCommand;
		DisplayRelocatedEquipmentsCommand = displayRelocatedEquipmentsCommand;

		Name = new()
		{
			Text = "*",
			Visible = false,
		};

		ActionKind = new()
		{
			Text = "*",
			Visible = false,
		};

		AirSuperiority = new()
		{
			Text = "*",
			ImageIndex = ResourceManager.EquipmentContent.CarrierBasedFighter,
			Visible = false,
		};

		Distance = new()
		{
			Text = "*",
			ImageIndex = IconContent.ParameterAircraftDistance,
			Visible = false,
		};

		Squadrons = new()
		{
			Visible = false,
		};
	}

	public void Update(int baseAirCorpsID)
	{
		KCDatabase db = KCDatabase.Instance;
		var corps = db.BaseAirCorps[baseAirCorpsID];

		if (corps == null)
		{
			baseAirCorpsID = -1;
			MapAreaId = -1;
		}
		else
		{
			MapAreaId = corps.MapAreaID;
			Name.Text = string.Format("#{0} - {1}", corps.MapAreaID, corps.Name);
			Name.Tag = corps.MapAreaID;
			var sb = new StringBuilder();


			string areaName = KCDatabase.Instance.MapArea.ContainsKey(corps.MapAreaID) ? KCDatabase.Instance.MapArea[corps.MapAreaID].NameEN : FormBaseAirCorps.UnknownArea;

			sb.AppendLine(FormBaseAirCorps.Area + areaName);

			// state

			if (corps.Squadrons.Values.Any(sq => sq != null && sq.Condition > 1))
			{
				// 疲労
				int tired = corps.Squadrons.Values.Max(sq => sq?.Condition ?? 0);

				if (tired == 2)
				{
					Name.ImageIndex = IconContent.ConditionTired;
					sb.AppendLine(GeneralRes.Tired);
				}
				else
				{
					Name.ImageIndex = IconContent.ConditionVeryTired;
					sb.AppendLine(GeneralRes.VeryTired);
				}

			}
			else if (corps.Squadrons.Values.Any(sq => sq != null && sq.AircraftCurrent < sq.AircraftMax))
			{
				// 未補給
				Name.ImageIndex = IconContent.FleetNotReplenished;
				sb.AppendLine(FormBaseAirCorps.Unsupplied);
			}
			else
			{
				Name.ImageIndex = null;
			}

			sb.AppendLine(string.Format(FormBaseAirCorps.AirControlSummary,
				db.BaseAirCorps.Values.Where(c => c.MapAreaID == corps.MapAreaID && c.ActionKind == AirBaseActionKind.AirDefense).Select(c => Calculator.GetAirSuperiority(c)).DefaultIfEmpty(0).Sum(),
				db.BaseAirCorps.Values.Where(c => c.MapAreaID == corps.MapAreaID && c.ActionKind == AirBaseActionKind.AirDefense).Select(c => Calculator.GetAirSuperiority(c, isHighAltitude: true)).DefaultIfEmpty(0).Sum()
			));

			Name.ToolTip = sb.ToString();


			ActionKind.Text = "[" + Constants.GetBaseAirCorpsActionKind(corps.ActionKind) + "]";

			{
				int airSuperiority = Calculator.GetAirSuperiority(corps);
				if (Utility.Configuration.Config.FormFleet.ShowAirSuperiorityRange)
				{
					int airSuperiority_max = Calculator.GetAirSuperiority(corps, true);
					if (airSuperiority < airSuperiority_max)
						AirSuperiority.Text = string.Format("{0} ～ {1}", airSuperiority, airSuperiority_max);
					else
						AirSuperiority.Text = airSuperiority.ToString();
				}
				else
				{
					AirSuperiority.Text = airSuperiority.ToString();
				}

				var tip = new StringBuilder();
				tip.AppendFormat(GeneralRes.BaseTooltip,
					(int)(airSuperiority / 3.0),
					(int)(airSuperiority / 1.5),
					Math.Max((int)(airSuperiority * 1.5 - 1), 0),
					Math.Max((int)(airSuperiority * 3.0 - 1), 0));

				if (corps.ActionKind == AirBaseActionKind.AirDefense)
				{
					int airSuperiorityHighAltitude = Calculator.GetAirSuperiority(corps, isHighAltitude: true);
					int airSuperiorityHighAltitudeMax = Calculator.GetAirSuperiority(corps, isAircraftLevelMaximum: true, isHighAltitude: true);

					tip.AppendFormat(GeneralRes.HighAltitudeAirState,
						Utility.Configuration.Config.FormFleet.ShowAirSuperiorityRange && airSuperiorityHighAltitude != airSuperiorityHighAltitudeMax ?
							$"{airSuperiorityHighAltitude} ～ {airSuperiorityHighAltitudeMax}" :
							airSuperiorityHighAltitude.ToString(),
						(int)(airSuperiorityHighAltitude / 3.0),
						(int)(airSuperiorityHighAltitude / 1.5),
						Math.Max((int)(airSuperiorityHighAltitude * 1.5 - 1), 0),
						Math.Max((int)(airSuperiorityHighAltitude * 3.0 - 1), 0));
				}

				AirSuperiority.ToolTip = tip.ToString();
			}
			int dist_text = corps.Distance;

			Distance.Text = dist_text.ToString();

			Squadrons.SetSlotList(corps);
			Squadrons.ToolTip = GetEquipmentString(corps);
			Distance.ToolTip = string.Format(FormBaseAirCorps.TotalDistance, corps.Distance);

		}


		Name.Visible =
			ActionKind.Visible =
				AirSuperiority.Visible =
					Distance.Visible =
						Squadrons.Visible =
							baseAirCorpsID != -1;

		Visibility = (baseAirCorpsID != -1).ToVisibility();
	}

	public void ConfigurationChanged()
	{

		var config = Utility.Configuration.Config;

		var mainfont = config.UI.MainFont;
		var subfont = config.UI.SubFont;

		// Name.Font = mainfont;
		// ActionKind.Font = mainfont;
		// AirSuperiority.Font = mainfont;
		// Distance.Font = mainfont;
		Squadrons.Font = subfont;

		Squadrons.ShowAircraft = config.FormFleet.ShowAircraft;
		Squadrons.ShowAircraftLevelByNumber = config.FormFleet.ShowAircraftLevelByNumber;
		Squadrons.LevelVisibility = config.FormFleet.EquipmentLevelVisibility;

	}


	private string GetEquipmentString(BaseAirCorpsData? corps)
	{
		StringBuilder sb = new();

		if (corps == null) return GeneralRes.BaseNotOpen;

		foreach (var squadron in corps.Squadrons.Values)
		{
			if (squadron == null) continue;

			IEquipmentData? eq = squadron.EquipmentInstance;

			switch (squadron.State)
			{
				case 0:     // 未配属
				default:
					sb.AppendLine(FormBaseAirCorps.Empty);
					break;

				case 1:     // 配属済み
					if (eq == null) goto case 0;

					sb.AppendFormat("[{0}/{1}] ",
						squadron.AircraftCurrent,
						squadron.AircraftMax);

					switch (squadron.Condition)
					{
						case 1:
						default:
							break;
						case 2:
							sb.Append("[" + GeneralRes.Tired + "] ");
							break;
						case 3:
							sb.Append("[" + GeneralRes.VeryTired + "] ");
							break;
					}

					sb.AppendFormat($"{FormBaseAirCorps.Range}\n", eq.NameWithLevel, eq.MasterEquipment.AircraftDistance);
					break;

				case 2:     // 配置転換中
					sb.AppendFormat($"{GeneralRes.BaseRelocate}\n",
						DateTimeHelper.TimeToCSVString(squadron.RelocatedTime));
					break;
			}
		}

		return sb.ToString();
	}
}
