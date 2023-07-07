﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using ElectronicObserverTypes;

namespace ElectronicObserver.Window.Tools.EquipmentUpgradePlanner.CostCalculation;

public class EquipmentUpgradePlanCostViewModel : ObservableObject
{
	public EquipmentUpgradePlanCostModel Model { get; set; }

	public EquipmentUpgradePlanCostMaterialViewModel Fuel { get; set; }

	public EquipmentUpgradePlanCostMaterialViewModel Ammo { get; set; }

	public EquipmentUpgradePlanCostMaterialViewModel Steel { get; set; }

	public EquipmentUpgradePlanCostMaterialViewModel Bauxite { get; set; }

	/// <summary>
	/// "screws"
	/// </summary>
	public EquipmentUpgradePlanCostMaterialViewModel ImprovementMaterial { get; set; }

	/// <summary>
	/// "devmats"
	/// </summary>
	public EquipmentUpgradePlanCostMaterialViewModel DevelopmentMaterial { get; set; }

	public List<EquipmentUpgradePlanCostEquipmentViewModel> RequiredEquipments { get; set; } = new();

	public List<EquipmentUpgradePlanCostConsumableViewModel> RequiredConsumables { get; set; } = new();

	public bool HasEnoughResources => GetHasEnoughResourceValue();

	public EquipmentUpgradePlannerTranslationViewModel EquipmentUpgradePlanner { get; }

	public EquipmentUpgradePlanCostViewModel(EquipmentUpgradePlanCostModel model)
	{
		EquipmentUpgradePlanner = Ioc.Default.GetRequiredService<EquipmentUpgradePlannerTranslationViewModel>();

		Model = model;

		Fuel = new(model.Fuel, UseItemId.Fuel);
		Ammo = new(model.Ammo, UseItemId.Ammo);
		Steel = new(model.Steel, UseItemId.Steel);
		Bauxite = new(model.Bauxite, UseItemId.Bauxite);

		DevelopmentMaterial = new(model.DevelopmentMaterial, UseItemId.DevelopmentMaterial);
		ImprovementMaterial = new(model.ImprovementMaterial, UseItemId.ImproveMaterial);

		RequiredEquipments = model.RequiredEquipments.Select(item => new EquipmentUpgradePlanCostEquipmentViewModel(item)).ToList();
		RequiredConsumables = model.RequiredConsumables.Select(item => new EquipmentUpgradePlanCostConsumableViewModel(item)).ToList();

		Fuel.PropertyChanged += ResourcePropertyUpdated;
		Ammo.PropertyChanged += ResourcePropertyUpdated;
		Steel.PropertyChanged += ResourcePropertyUpdated;
		Bauxite.PropertyChanged += ResourcePropertyUpdated;

		DevelopmentMaterial.PropertyChanged += ResourcePropertyUpdated;
		ImprovementMaterial.PropertyChanged += ResourcePropertyUpdated;

		foreach (EquipmentUpgradePlanCostEquipmentViewModel equip in RequiredEquipments)
		{
			equip.PropertyChanged += ResourcePropertyUpdated;
		}

		foreach (EquipmentUpgradePlanCostConsumableViewModel consumable in RequiredConsumables)
		{
			consumable.PropertyChanged += ResourcePropertyUpdated;
		}

		Update();
	}

	private void ResourcePropertyUpdated(object? sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName is not nameof(EquipmentUpgradePlanResourceDisplayViewModel.Owned) and not nameof(EquipmentUpgradePlanResourceDisplayViewModel.Required)) return;

		Update();
	}

	private void Update()
	{
		OnPropertyChanged(nameof(HasEnoughResources));
	}

	private bool GetHasEnoughResourceValue()
	{
		if (!Fuel.EnoughOwned) return false;
		if (!Ammo.EnoughOwned) return false;
		if (!Steel.EnoughOwned) return false;
		if (!Bauxite.EnoughOwned) return false;

		if (!DevelopmentMaterial.EnoughOwned) return false;
		if (!ImprovementMaterial.EnoughOwned) return false;

		if (RequiredEquipments.Any(equip => !equip.EnoughOwned)) return false;
		if (RequiredConsumables.Any(consumable => !consumable.EnoughOwned)) return false;

		return true;
	}
}
