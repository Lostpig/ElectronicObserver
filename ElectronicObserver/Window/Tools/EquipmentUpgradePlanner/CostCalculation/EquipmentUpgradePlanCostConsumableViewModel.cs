﻿using ElectronicObserver.Data;
using ElectronicObserver.Observer;

namespace ElectronicObserver.Window.Tools.EquipmentUpgradePlanner.CostCalculation;
public class EquipmentUpgradePlanCostConsumableViewModel : EquipmentUpgradePlanCostItemViewModel
{
	public UseItemMaster Consumable { get; set; }

	public EquipmentUpgradePlanCostConsumableViewModel(EquipmentUpgradePlanCostItemModel model) : base(model)
	{
		Consumable = KCDatabase.Instance.MasterUseItems[model.Id];

		SubscribeToApis();
		Update();
	}

	public void Update()
	{
		KCDatabase db = KCDatabase.Instance;

		UseItem? item = db.UseItems[Consumable.ID];

		Owned = item?.Count ?? 0;
	}

	private void UpdateOnResponseReceived(string apiname, dynamic data)
	{
		Update();
	}

	public void SubscribeToApis()
	{
		APIObserver.Instance.ApiPort_Port.ResponseReceived += UpdateOnResponseReceived;

		APIObserver.Instance.ApiReqQuest_ClearItemGet.ResponseReceived += UpdateOnResponseReceived;

		APIObserver.Instance.ApiReqMember_ItemUse.ResponseReceived += UpdateOnResponseReceived;

		APIObserver.Instance.ApiReqKousyou_RemodelSlot.ResponseReceived += UpdateOnResponseReceived;
	}

	public void UnsubscribeFromApis()
	{
		APIObserver.Instance.ApiPort_Port.ResponseReceived -= UpdateOnResponseReceived;

		APIObserver.Instance.ApiReqQuest_ClearItemGet.ResponseReceived -= UpdateOnResponseReceived;

		APIObserver.Instance.ApiReqMember_ItemUse.ResponseReceived -= UpdateOnResponseReceived;

		APIObserver.Instance.ApiReqKousyou_RemodelSlot.ResponseReceived -= UpdateOnResponseReceived;
	}
}
