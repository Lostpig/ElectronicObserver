﻿using ElectronicObserver.KancolleApi.Types.Interfaces;
using ElectronicObserver.KancolleApi.Types.Models;
using ElectronicObserverTypes;

namespace ElectronicObserver.KancolleApi.Types.ApiReqCombinedBattle.EcNightToDay;

public class ApiReqCombinedBattleEcNightToDayResponse : IDayFromNightBattleApiResponse, IEnemyCombinedFleetBattle
{
	[JsonPropertyName("api_deck_id")]
	public int ApiDeckId { get; set; }

	[JsonPropertyName("api_formation")]
	public List<int> ApiFormation { get; set; } = new();

	[JsonPropertyName("api_f_nowhps")]
	public List<int> ApiFNowhps { get; set; } = new();

	[JsonPropertyName("api_f_maxhps")]
	public List<int> ApiFMaxhps { get; set; } = new();

	[JsonPropertyName("api_fParam")]
	public List<List<int>> ApiFParam { get; set; } = new();

	[JsonPropertyName("api_ship_ke")]
	public List<int> ApiShipKe { get; set; } = new();

	[JsonPropertyName("api_ship_lv")]
	public List<int> ApiShipLv { get; set; } = new();

	[JsonPropertyName("api_ship_ke_combined")]
	public List<int> ApiShipKeCombined { get; set; } = new();

	[JsonPropertyName("api_ship_lv_combined")]
	public List<int> ApiShipLvCombined { get; set; } = new();

	[JsonPropertyName("api_e_nowhps")]
	public List<object> ApiENowhps { get; set; } = new();

	[JsonPropertyName("api_e_maxhps")]
	public List<object> ApiEMaxhps { get; set; } = new();

	[JsonPropertyName("api_e_nowhps_combined")]
	public List<int> ApiENowhpsCombined { get; set; } = new();

	[JsonPropertyName("api_e_maxhps_combined")]
	public List<int> ApiEMaxhpsCombined { get; set; } = new();

	[JsonPropertyName("api_eSlot")]
	public List<List<int>> ApiESlot { get; set; } = new();

	[JsonPropertyName("api_eSlot_combined")]
	public List<List<int>> ApiESlotCombined { get; set; } = new();

	[JsonPropertyName("api_eParam")]
	public List<List<int>> ApiEParam { get; set; } = new();

	[JsonPropertyName("api_eParam_combined")]
	public List<List<int>> ApiEParamCombined { get; set; } = new();

	[JsonPropertyName("api_touch_plane")]
	public List<object> ApiTouchPlane { get; set; } = new();

	[JsonPropertyName("api_flare_pos")]
	public List<int> ApiFlarePos { get; set; } = new();

	[JsonPropertyName("api_n_support_flag")]
	public SupportType ApiNSupportFlag { get; set; }

	[JsonPropertyName("api_n_support_info")]
	public ApiSupportInfo? ApiNSupportInfo { get; set; }

	[JsonPropertyName("api_n_hougeki1")]
	public ApiHougeki? ApiNHougeki1 { get; set; }

	[JsonPropertyName("api_n_hougeki2")]
	public ApiHougeki? ApiNHougeki2 { get; set; }

	[JsonPropertyName("api_day_flag")]
	public int ApiDayFlag { get; set; }

	[JsonPropertyName("api_search")]
	public List<DetectionType> ApiSearch { get; set; } = new();

	[JsonPropertyName("api_air_base_injection")]
	public ApiAirBaseInjection? ApiAirBaseInjection { get; set; }

	[JsonPropertyName("api_injection_kouku")]
	public ApiInjectionKouku? ApiInjectionKouku { get; set; }

	[JsonPropertyName("api_air_base_attack")]
	public List<ApiAirBaseAttack>? ApiAirBaseAttack { get; set; }

	[JsonPropertyName("api_friendly_info")]
	public ApiFriendlyInfo? ApiFriendlyInfo { get; set; }

	[JsonPropertyName("api_friendly_battle")]
	public ApiFriendlyBattle? ApiFriendlyBattle { get; set; }

	[JsonPropertyName("api_friendly_kouku")]
	public ApiKouku? ApiFriendlyKouku { get; set; }

	[JsonPropertyName("api_support_flag")]
	public SupportType ApiSupportFlag { get; set; }

	[JsonPropertyName("api_support_info")]
	public ApiSupportInfo? ApiSupportInfo { get; set; }

	[JsonPropertyName("api_stage_flag")]
	public List<int> ApiStageFlag { get; set; } = new();

	[JsonPropertyName("api_kouku")]
	public ApiKouku? ApiKouku { get; set; }

	[JsonPropertyName("api_opening_taisen_flag")]
	public int ApiOpeningTaisenFlag { get; set; }

	[JsonPropertyName("api_opening_taisen")]
	public ApiHougeki1? ApiOpeningTaisen { get; set; }

	[JsonPropertyName("api_opening_flag")]
	public int ApiOpeningFlag { get; set; }

	[JsonPropertyName("api_opening_atack")]
	public ApiRaigekiClass? ApiOpeningAtack { get; set; }

	[JsonPropertyName("api_hourai_flag")]
	public List<int> ApiHouraiFlag { get; set; } = new();

	[JsonPropertyName("api_hougeki1")]
	public ApiHougeki1? ApiHougeki1 { get; set; }

	[JsonPropertyName("api_hougeki2")]
	public ApiHougeki1? ApiHougeki2 { get; set; }

	[JsonPropertyName("api_raigeki")]
	public ApiRaigekiClass? ApiRaigeki { get; set; }

	[JsonPropertyName("api_escape_idx")]
	public List<int>? ApiEscapeIdx { get; set; }

	[JsonPropertyName("api_xal01")]
	public int? ApiXal01 { get; set; }

	[JsonPropertyName("api_smoke_type")]
	public int? ApiSmokeType { get; set; }
}
