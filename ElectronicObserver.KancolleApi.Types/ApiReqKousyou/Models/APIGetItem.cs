﻿namespace ElectronicObserver.KancolleApi.Types.ApiReqKousyou.Models;

public class ApiGetItem
{
	[JsonPropertyName("api_id")]
	public int ApiId { get; set; }

	[JsonPropertyName("api_slotitem_id")]
	public int ApiSlotitemId { get; set; }
}
