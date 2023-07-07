﻿using ElectronicObserver.Data;
using ElectronicObserver.Properties.Window;
using ElectronicObserver.ViewModels.Translations;

namespace ElectronicObserver.Window.Tools.SortieRecordViewer.SortieDetail;

public class SortieDetailTranslationViewModel : TranslationBaseViewModel
{
	public string Title => SortieRecordViewer.SortieDetail;

	public string File => FormMain.File;
	public string CopySortieData => SortieRecordViewer.CopySortieData;
	public string LoadSortieData => SortieRecordViewer.LoadSortieData;
	public string AirControlSimulator => Tools.AirControlSimulator.AirControlSimulator.Title;
	public string CopyLink => SortieRecordViewer.CopyLink;
	public string Open => SortieRecordViewer.Open;

	public string BattleDetail_FriendFleet => ConstantsRes.BattleDetail_FriendFleet;
	public string BattleDetail_FriendMainFleet => ConstantsRes.BattleDetail_FriendMainFleet;
	public string BattleDetail_FriendEscortFleet => ConstantsRes.BattleDetail_FriendEscortFleet;

	public string BattleDetail_EnemyFleet => ConstantsRes.BattleDetail_EnemyFleet;
	public string BattleDetail_EnemyMainFleet => ConstantsRes.BattleDetail_EnemyMainFleet;
	public string BattleDetail_EnemyEscortFleet => ConstantsRes.BattleDetail_EnemyEscortFleet;

	public string BattleDetail_BattleEnd => ConstantsRes.BattleDetail_BattleEnd;
}
