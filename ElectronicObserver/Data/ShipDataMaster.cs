﻿using ElectronicObserver.Resource.Record;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElectronicObserverTypes;

namespace ElectronicObserver.Data
{
	public interface IShipDataMaster
	{
		/// <summary>
		/// 艦船ID
		/// </summary>
		int ShipID { get; }

		/// <summary>
		/// 艦船ID
		/// </summary>
		public ShipId ShipId { get; }

		/// <summary>
		/// 図鑑番号
		/// </summary>
		int AlbumNo { get; }

		/// <summary>
		/// 母港ソート順
		/// </summary>
		int SortID { get; }

		/// <summary>
		/// 名前
		/// </summary>
		string Name { get; }
		
		/// <summary>
		/// Name in japanese
		/// </summary>
		public string NameJP { get; }

		/// <summary>
		/// 読み
		/// </summary>
		string NameReading { get; }

		/// <summary>
		/// 艦種
		/// </summary>
		ShipTypes ShipType { get; }

		/// <summary>
		/// 艦型
		/// </summary>
		int ShipClass { get; }

		/// <summary>
		/// 改装Lv.
		/// </summary>
		int RemodelAfterLevel { get; }

		/// <summary>
		/// 改装後の艦船ID
		/// 0=なし
		/// </summary>
		int RemodelAfterShipID { get; }

		/// <summary>
		/// 改装後の艦船
		/// </summary>
		ShipDataMaster RemodelAfterShip { get; }

		/// <summary>
		/// 改装前の艦船ID
		/// 0=なし
		/// </summary>
		int RemodelBeforeShipID { get; }

		/// <summary>
		/// 改装前の艦船
		/// </summary>
		ShipDataMaster RemodelBeforeShip { get; }

		/// <summary>
		/// 改装に必要な弾薬
		/// </summary>
		int RemodelAmmo { get; }

		/// <summary>
		/// 改装に必要な鋼材
		/// </summary>
		int RemodelSteel { get; }

		/// <summary>
		/// 改装に改装設計図が必要かどうか
		/// </summary>
		int NeedBlueprint { get; }

		/// <summary>
		/// 改装に試製甲板カタパルトが必要かどうか
		/// </summary>
		int NeedCatapult { get; }

		/// <summary>
		/// 改装に戦闘詳報が必要かどうか
		/// </summary>
		int NeedActionReport { get; }

		/// <summary>
		/// 耐久初期値
		/// </summary>
		int HPMin { get; }

		/// <summary>
		/// 耐久最大値
		/// </summary>
		int HPMax { get; }

		/// <summary>
		/// 装甲初期値
		/// </summary>
		int ArmorMin { get; }

		/// <summary>
		/// 装甲最大値
		/// </summary>
		int ArmorMax { get; }

		/// <summary>
		/// 火力初期値
		/// </summary>
		int FirepowerMin { get; }

		/// <summary>
		/// 火力最大値
		/// </summary>
		int FirepowerMax { get; }

		/// <summary>
		/// 雷装初期値
		/// </summary>
		int TorpedoMin { get; }

		/// <summary>
		/// 雷装最大値
		/// </summary>
		int TorpedoMax { get; }

		/// <summary>
		/// 対空初期値
		/// </summary>
		int AAMin { get; }

		/// <summary>
		/// 対空最大値
		/// </summary>
		int AAMax { get; }

		/// <summary>
		/// 対潜
		/// </summary>
		ShipParameterRecord.Parameter ASW { get; }

		/// <summary>
		/// 回避
		/// </summary>
		ShipParameterRecord.Parameter Evasion { get; }

		/// <summary>
		/// 索敵
		/// </summary>
		ShipParameterRecord.Parameter LOS { get; }

		/// <summary>
		/// 運初期値
		/// </summary>
		int LuckMin { get; }

		/// <summary>
		/// 運最大値
		/// </summary>
		int LuckMax { get; }

		/// <summary>
		/// 速力
		/// 0=陸上基地, 5=低速, 10=高速
		/// </summary>
		int Speed { get; }

		/// <summary>
		/// 射程
		/// </summary>
		int Range { get; }

		/// <summary>
		/// 装備スロットの数
		/// </summary>
		int SlotSize { get; }

		/// <summary>
		/// 各スロットの航空機搭載数
		/// </summary>
		ReadOnlyCollection<int> Aircraft { get; }

		/// <summary>
		/// 搭載
		/// </summary>
		int AircraftTotal { get; }

		/// <summary>
		/// 初期装備のID
		/// </summary>
		ReadOnlyCollection<int> DefaultSlot { get; }

		/// <summary>
		/// 特殊装備カテゴリ　指定がない場合は null
		/// </summary>
		IEnumerable<int> SpecialEquippableCategories { get; }

		/// <summary>
		/// 装備可能なカテゴリ
		/// </summary>
		IEnumerable<int> EquippableCategories { get; }

		/// <summary>
		/// 建造時間(分)
		/// </summary>
		int BuildingTime { get; }

		/// <summary>
		/// 解体資材
		/// </summary>
		ReadOnlyCollection<int> Material { get; }

		/// <summary>
		/// 近代化改修の素材にしたとき上昇するパラメータの量
		/// </summary>
		ReadOnlyCollection<int> PowerUp { get; }

		/// <summary>
		/// レアリティ
		/// </summary>
		int Rarity { get; }

		/// <summary>
		/// ドロップ/ログイン時のメッセージ
		/// </summary>
		string MessageGet { get; }

		/// <summary>
		/// 艦船名鑑でのメッセージ
		/// </summary>
		string MessageAlbum { get; }

		/// <summary>
		/// 搭載燃料
		/// </summary>
		int Fuel { get; }

		/// <summary>
		/// 搭載弾薬
		/// </summary>
		int Ammo { get; }

		/// <summary>
		/// ボイス再生フラグ
		/// </summary>
		int VoiceFlag { get; }

		/// <summary>
		/// グラフィック設定データへの参照
		/// </summary>
		ShipGraphicData GraphicData { get; }

		/// <summary>
		/// リソースのファイル/フォルダ名
		/// </summary>
		string ResourceName { get; }

		/// <summary>
		/// 画像リソースのバージョン
		/// </summary>
		string ResourceGraphicVersion { get; }

		/// <summary>
		/// ボイスリソースのバージョン
		/// </summary>
		string ResourceVoiceVersion { get; }

		/// <summary>
		/// 母港ボイスリソースのバージョン
		/// </summary>
		string ResourcePortVoiceVersion { get; }

		/// <summary>
		/// 衣替え艦：ベースとなる艦船ID
		/// </summary>
		int OriginalCostumeShipID { get; }

		/// <summary>
		/// ケッコンカッコカリ後のHP
		/// </summary>
		int HPMaxMarried { get; }

		/// <summary>
		/// HP改修可能値(未婚時)
		/// </summary>
		int HPMaxModernizable { get; }

		/// <summary>
		/// HP改修可能値(既婚時)
		/// </summary>
		int HPMaxMarriedModernizable { get; }

		/// <summary>
		/// 近代化改修後のHP(未婚時)
		/// </summary>
		int HPMaxModernized { get; }

		/// <summary>
		/// 近代化改修後のHP(既婚時)
		/// </summary>
		int HPMaxMarriedModernized { get; }

		/// <summary>
		/// 対潜改修可能値
		/// </summary>
		int ASWModernizable { get; }

		/// <summary>
		/// 深海棲艦かどうか
		/// </summary>
		bool IsAbyssalShip { get; }

		/// <summary>
		/// クラスも含めた艦名
		/// </summary>
		string NameWithClass { get; }

		/// <summary>
		/// 艦種インスタンス
		/// </summary>
		ShipType ShipTypeInstance { get; }

		/// <summary>
		/// 陸上基地かどうか
		/// </summary>
		bool IsLandBase { get; }

		/// <summary>
		/// 図鑑に載っているか
		/// </summary>
		bool IsListedInAlbum { get; }

		/// <summary>
		/// 改装段階
		/// 初期 = 0, 改 = 1, 改二 = 2, ...
		/// </summary>
		int RemodelTier { get; }

		/// <summary>
		/// 改装段階
		/// 初期 = 0, 改 = 1, 改二 = 2, ...
		/// </summary>
		RemodelTier RemodelTierTyped { get; }

		/// <summary>
		/// 艦種名
		/// </summary>
		string ShipTypeName { get; }

		/// <summary>
		/// 潜水艦系か (潜水艦/潜水空母)
		/// </summary>
		bool IsSubmarine { get; }

		/// <summary>
		/// 空母系か (軽空母/正規空母/装甲空母)
		/// </summary>
		bool IsAircraftCarrier { get; }

		/// <summary>
		/// Regular Carrier (CV/CVB)
		/// </summary>
		bool IsRegularCarrier { get; }

		/// <summary>
		/// 護衛空母か
		/// </summary>
		bool IsEscortAircraftCarrier { get; }

		int ID { get; }

		/// <summary>
		/// 生の受信データ(api_data)
		/// </summary>
		dynamic RawData { get; }

		/// <summary>
		/// 現在のデータが有効かを取得します。
		/// </summary>
		bool IsAvailable { get; }

		Color GetShipNameColor();
		ShipDataMaster BaseShip();
		string ToString();

		/// <summary>
		/// Responseを読み込みます。
		/// </summary>
		/// <param name="apiname">読み込むAPIの名前。</param>
		/// <param name="data">受信したデータ。</param>
		void LoadFromResponse(string apiname, dynamic data);
	}

	/// <summary>
	/// 艦船のマスターデータを保持します。
	/// </summary>
	public class ShipDataMaster : ResponseWrapper, IIdentifiable, IShipDataMaster
	{

		/// <summary>
		/// 艦船ID
		/// </summary>
		public int ShipID => (int)RawData.api_id;

		/// <summary>
		/// 艦船ID
		/// </summary>
		public ShipId ShipId => (ShipId)RawData.api_id;

		/// <summary>
		/// 図鑑番号
		/// </summary>
		public int AlbumNo => !RawData.api_sortno() ? 0 : (int)RawData.api_sortno;

		/// <summary>
		/// 母港ソート順
		/// </summary>
		public int SortID => !RawData.api_sort_id() ? 0 : (int)RawData.api_sort_id;

		/// <summary>
		/// 名前
		/// </summary>
		public string Name => Window.FormMain.Instance.Translator.GetTranslation(RawData.api_name, Utility.TranslationType.Ships);

		/// <summary>
		/// Name in japanese
		/// </summary>
		public string NameJP => RawData.api_name;

		/// <summary>
		/// 読み
		/// </summary>
		public string NameReading => RawData.api_yomi;

		/// <summary>
		/// 艦種
		/// </summary>
		public ShipTypes ShipType => (ShipTypes)(int)RawData.api_stype;

		/// <summary>
		/// 艦型
		/// </summary>
		public int ShipClass => (int)RawData.api_ctype;


		/// <summary>
		/// 改装Lv.
		/// </summary>
		public int RemodelAfterLevel => !RawData.api_afterlv() ? 0 : (int)RawData.api_afterlv;

		/// <summary>
		/// 改装後の艦船ID
		/// 0=なし
		/// </summary>
		public int RemodelAfterShipID => !RawData.api_aftershipid() ? 0 : int.Parse((string)RawData.api_aftershipid);

		/// <summary>
		/// 改装後の艦船
		/// </summary>
		public ShipDataMaster RemodelAfterShip => RemodelAfterShipID > 0 ? KCDatabase.Instance.MasterShips[RemodelAfterShipID] : null;


		/// <summary>
		/// 改装前の艦船ID
		/// 0=なし
		/// </summary>
		public int RemodelBeforeShipID { get; internal set; }

		/// <summary>
		/// 改装前の艦船
		/// </summary>
		public ShipDataMaster RemodelBeforeShip => RemodelBeforeShipID > 0 ? KCDatabase.Instance.MasterShips[RemodelBeforeShipID] : null;


		/// <summary>
		/// 改装に必要な弾薬
		/// </summary>
		public int RemodelAmmo => !RawData.api_afterbull() ? 0 : (int)RawData.api_afterbull;

		/// <summary>
		/// 改装に必要な鋼材
		/// </summary>
		public int RemodelSteel => !RawData.api_afterfuel() ? 0 : (int)RawData.api_afterfuel;

		/// <summary>
		/// 改装に改装設計図が必要かどうか
		/// </summary>
		public int NeedBlueprint { get; internal set; }

		/// <summary>
		/// 改装に試製甲板カタパルトが必要かどうか
		/// </summary>
		public int NeedCatapult { get; internal set; }

		/// <summary>
		/// 改装に戦闘詳報が必要かどうか
		/// </summary>
		public int NeedActionReport { get; internal set; }


		#region Parameters

		/// <summary>
		/// 耐久初期値
		/// </summary>
		public int HPMin
		{
			get
			{
				if (RawData.api_taik())
				{
					return (int)RawData.api_taik[0];
				}
				else
				{
					return GetParameterElement()?.HPMin ?? 0;
				}
			}
		}

		/// <summary>
		/// 耐久最大値
		/// </summary>
		public int HPMax
		{
			get
			{
				if (RawData.api_taik())
				{
					return (int)RawData.api_taik[1];
				}
				else
				{
					return GetParameterElement()?.HPMax ?? 0;
				}
			}
		}

		/// <summary>
		/// 装甲初期値
		/// </summary>
		public int ArmorMin
		{
			get
			{
				if (RawData.api_souk())
				{
					return (int)RawData.api_souk[0];
				}
				else
				{
					return GetParameterElement()?.ArmorMin ?? 0;
				}
			}
		}

		/// <summary>
		/// 装甲最大値
		/// </summary>
		public int ArmorMax
		{
			get
			{
				if (RawData.api_souk())
				{
					return (int)RawData.api_souk[1];
				}
				else
				{
					return GetParameterElement()?.ArmorMax ?? 0;
				}
			}
		}

		/// <summary>
		/// 火力初期値
		/// </summary>
		public int FirepowerMin
		{
			get
			{
				if (RawData.api_houg())
				{
					return (int)RawData.api_houg[0];
				}
				else
				{
					return GetParameterElement()?.FirepowerMin ?? 0;
				}
			}
		}

		/// <summary>
		/// 火力最大値
		/// </summary>
		public int FirepowerMax
		{
			get
			{
				if (RawData.api_houg())
				{
					return (int)RawData.api_houg[1];
				}
				else
				{
					return GetParameterElement()?.FirepowerMax ?? 0;
				}
			}
		}

		/// <summary>
		/// 雷装初期値
		/// </summary>
		public int TorpedoMin
		{
			get
			{
				if (RawData.api_raig())
				{
					return (int)RawData.api_raig[0];
				}
				else
				{
					return GetParameterElement()?.TorpedoMin ?? 0;
				}
			}
		}

		/// <summary>
		/// 雷装最大値
		/// </summary>
		public int TorpedoMax
		{
			get
			{
				if (RawData.api_raig())
				{
					return (int)RawData.api_raig[1];
				}
				else
				{
					return GetParameterElement()?.TorpedoMax ?? 0;
				}
			}
		}

		/// <summary>
		/// 対空初期値
		/// </summary>
		public int AAMin
		{
			get
			{
				if (RawData.api_tyku())
				{
					return (int)RawData.api_tyku[0];
				}
				else
				{
					return GetParameterElement()?.AAMin ?? 0;
				}
			}
		}

		/// <summary>
		/// 対空最大値
		/// </summary>
		public int AAMax
		{
			get
			{
				if (RawData.api_tyku())
				{
					return (int)RawData.api_tyku[1];
				}
				else
				{
					return GetParameterElement()?.AAMax ?? 0;
				}
			}
		}


		/// <summary>
		/// 対潜
		/// </summary>
		public ShipParameterRecord.Parameter ASW => GetParameterElement()?.ASW;

		/// <summary>
		/// 回避
		/// </summary>
		public ShipParameterRecord.Parameter Evasion => GetParameterElement()?.Evasion;

		/// <summary>
		/// 索敵
		/// </summary>
		public ShipParameterRecord.Parameter LOS => GetParameterElement()?.LOS;


		/// <summary>
		/// 運初期値
		/// </summary>
		public int LuckMin
		{
			get
			{
				if (RawData.api_luck())
				{
					return (int)RawData.api_luck[0];
				}
				else
				{
					return GetParameterElement()?.LuckMin ?? 0;
				}
			}
		}

		/// <summary>
		/// 運最大値
		/// </summary>
		public int LuckMax
		{
			get
			{
				if (RawData.api_luck())
				{
					return (int)RawData.api_luck[1];
				}
				else
				{
					return GetParameterElement()?.LuckMax ?? 0;
				}
			}
		}

		/// <summary>
		/// 速力
		/// 0=陸上基地, 5=低速, 10=高速
		/// </summary>
		public int Speed => (int)RawData.api_soku;

		/// <summary>
		/// 射程
		/// </summary>
		public int Range
		{
			get
			{
				if (RawData.api_leng())
				{
					return (int)RawData.api_leng;
				}
				else
				{
					return GetParameterElement()?.Range ?? 0;
				}
			}
		}
		#endregion


		/// <summary>
		/// 装備スロットの数
		/// </summary>
		public int SlotSize => (int)RawData.api_slot_num;

		/// <summary>
		/// 各スロットの航空機搭載数
		/// </summary>
		public ReadOnlyCollection<int> Aircraft
		{
			get
			{
				if (RawData.api_maxeq())
				{
					return Array.AsReadOnly((int[])RawData.api_maxeq);
				}
				else
				{
					var p = GetParameterElement();
					if (p != null && p.Aircraft != null)
						return Array.AsReadOnly(p.Aircraft);
					else
						return Array.AsReadOnly(new[] { 0, 0, 0, 0, 0 });
				}
			}
		}

		/// <summary>
		/// 搭載
		/// </summary>
		public int AircraftTotal => Aircraft.Sum(a => Math.Max(a, 0));


		/// <summary>
		/// 初期装備のID
		/// </summary>
		public ReadOnlyCollection<int> DefaultSlot
		{
			get
			{
				var p = GetParameterElement();
				if (p != null && p.DefaultSlot != null)
					return Array.AsReadOnly(p.DefaultSlot);
				else
					return null;
			}
		}


		internal int[] specialEquippableCategory = null;
		/// <summary>
		/// 特殊装備カテゴリ　指定がない場合は null
		/// </summary>
		public IEnumerable<int> SpecialEquippableCategories => specialEquippableCategory;

		/// <summary>
		/// 装備可能なカテゴリ
		/// </summary>
		public IEnumerable<int> EquippableCategories
		{
			get
			{
				if (specialEquippableCategory != null)
					return SpecialEquippableCategories;
				else
					return KCDatabase.Instance.ShipTypes[(int)ShipType].EquippableCategories;
			}
		}


		/// <summary>
		/// 建造時間(分)
		/// </summary>
		public int BuildingTime => !RawData.api_buildtime() ? 0 : (int)RawData.api_buildtime;


		/// <summary>
		/// 解体資材
		/// </summary>
		public ReadOnlyCollection<int> Material => Array.AsReadOnly(!RawData.api_broken() ? new[] { 0, 0, 0, 0 } : (int[])RawData.api_broken);

		/// <summary>
		/// 近代化改修の素材にしたとき上昇するパラメータの量
		/// </summary>
		public ReadOnlyCollection<int> PowerUp => Array.AsReadOnly(!RawData.api_powup() ? new[] { 0, 0, 0, 0 } : (int[])RawData.api_powup);

		/// <summary>
		/// レアリティ
		/// </summary>
		public int Rarity => !RawData.api_backs() ? 0 : (int)RawData.api_backs;

		/// <summary>
		/// ドロップ/ログイン時のメッセージ
		/// </summary>
		public string MessageGet => GetParameterElement()?.MessageGet?.Replace("<br>", "\r\n") ?? "";

		/// <summary>
		/// 艦船名鑑でのメッセージ
		/// </summary>
		public string MessageAlbum => GetParameterElement()?.MessageAlbum?.Replace("<br>", "\r\n") ?? "";


		/// <summary>
		/// 搭載燃料
		/// </summary>
		public int Fuel => !RawData.api_fuel_max() ? 0 : (int)RawData.api_fuel_max;

		/// <summary>
		/// 搭載弾薬
		/// </summary>
		public int Ammo => !RawData.api_bull_max() ? 0 : (int)RawData.api_bull_max;


		/// <summary>
		/// ボイス再生フラグ
		/// </summary>
		public int VoiceFlag => !RawData.api_voicef() ? 0 : (int)RawData.api_voicef;


		/// <summary>
		/// グラフィック設定データへの参照
		/// </summary>
		public ShipGraphicData GraphicData => KCDatabase.Instance.ShipGraphics[ShipID];

		/// <summary>
		/// リソースのファイル/フォルダ名
		/// </summary>
		public string ResourceName => GraphicData?.ResourceName ?? "";

		/// <summary>
		/// 画像リソースのバージョン
		/// </summary>
		public string ResourceGraphicVersion => GraphicData?.GraphicVersion ?? "";

		/// <summary>
		/// ボイスリソースのバージョン
		/// </summary>
		public string ResourceVoiceVersion => GraphicData?.VoiceVersion ?? "";

		/// <summary>
		/// 母港ボイスリソースのバージョン
		/// </summary>
		public string ResourcePortVoiceVersion => GraphicData?.PortVoiceVersion ?? "";



		/// <summary>
		/// 衣替え艦：ベースとなる艦船ID
		/// </summary>
		public int OriginalCostumeShipID => GetParameterElement()?.OriginalCostumeShipID ?? -1;



		//以下、自作計算プロパティ群

		public static readonly int HPModernizableLimit = 2;
		public static readonly int ASWModernizableLimit = 9;


		/// <summary>
		/// ケッコンカッコカリ後のHP
		/// </summary>
		public int HPMaxMarried
		{
			get
			{
				int incr;
				if (HPMin < 30) incr = 4;
				else if (HPMin < 40) incr = 5;
				else if (HPMin < 50) incr = 6;
				else if (HPMin < 70) incr = 7;
				else if (HPMin < 90) incr = 8;
				else incr = 9;

				return Math.Min(HPMin + incr, HPMax);
			}
		}

		/// <summary>
		/// HP改修可能値(未婚時)
		/// </summary>
		public int HPMaxModernizable => Math.Min(HPMax - HPMin, HPModernizableLimit);

		/// <summary>
		/// HP改修可能値(既婚時)
		/// </summary>
		public int HPMaxMarriedModernizable => Math.Min(HPMax - HPMaxMarried, HPModernizableLimit);

		/// <summary>
		/// 近代化改修後のHP(未婚時)
		/// </summary>
		public int HPMaxModernized => Math.Min(HPMin + HPMaxModernizable, HPMax);


		/// <summary>
		/// 近代化改修後のHP(既婚時)
		/// </summary>
		public int HPMaxMarriedModernized => Math.Min(HPMaxMarried + HPMaxModernizable, HPMax);



		/// <summary>
		/// 対潜改修可能値
		/// </summary>
		public int ASWModernizable => ASW == null || ASW.Maximum == 0 ? 0 : ASWModernizableLimit;


		/// <summary>
		/// 深海棲艦かどうか
		/// </summary>
		public bool IsAbyssalShip => ShipID > 1500;


		/// <summary>
		/// クラスも含めた艦名
		/// </summary>
		public string NameWithClass
		{
			get
			{
				if (!IsAbyssalShip || NameReading == "" || NameReading == "-")
					return Name;
				else
					return $"{Name} {NameReading}";
			}
		}

		/// <summary>
		/// 艦種インスタンス
		/// </summary>
		public ShipType ShipTypeInstance => KCDatabase.Instance.ShipTypes[(int)ShipType];

        /// <summary>
        /// 陸上基地かどうか
        /// </summary>
        public bool IsLandBase => Speed == 0;


		/// <summary>
		/// 図鑑に載っているか
		/// </summary>
		public bool IsListedInAlbum => 0 < AlbumNo && AlbumNo <= 420;


		/// <summary>
		/// 改装段階
		/// 初期 = 0, 改 = 1, 改二 = 2, ...
		/// </summary>
		public int RemodelTier
		{
			get
			{
				int tier = 0;
				var ship = this;
				while (ship.RemodelBeforeShip != null)
				{
					tier++;
					ship = ship.RemodelBeforeShip;
				}

				return tier;
			}
		}

		/// <summary>
		/// 改装段階
		/// 初期 = 0, 改 = 1, 改二 = 2, ...
		/// </summary>
		public RemodelTier RemodelTierTyped => (RemodelTier) RemodelTier;

		/// <summary>
		/// 艦種名
		/// </summary>
		public string ShipTypeName => Window.FormMain.Instance.Translator.GetTranslation(KCDatabase.Instance.ShipTypes[(int)ShipType].Name, Utility.TranslationType.ShipTypes);


		/// <summary>
		/// 潜水艦系か (潜水艦/潜水空母)
		/// </summary>
		public bool IsSubmarine => ShipType == ShipTypes.Submarine || ShipType == ShipTypes.SubmarineAircraftCarrier;

		/// <summary>
		/// 空母系か (軽空母/正規空母/装甲空母)
		/// </summary>
		public bool IsAircraftCarrier => ShipType == ShipTypes.LightAircraftCarrier || ShipType == ShipTypes.AircraftCarrier || ShipType == ShipTypes.ArmoredAircraftCarrier;

		/// <summary>
		/// Regular Carrier (CV/CVB)
		/// </summary>
		public bool IsRegularCarrier => ShipType == ShipTypes.AircraftCarrier || ShipType == ShipTypes.ArmoredAircraftCarrier;

		/// <summary>
		/// 護衛空母か
		/// </summary>
		public bool IsEscortAircraftCarrier => ShipType == ShipTypes.LightAircraftCarrier && ASW.Minimum > 0;


		/// <summary>
		/// 自身のパラメータレコードを取得します。
		/// </summary>
		/// <returns></returns>
		private ShipParameterRecord.ShipParameterElement GetParameterElement()
		{
			return RecordManager.Instance.ShipParameter[ShipID];
		}



		private static readonly Color[] ShipNameColors = new Color[] {
			Utility.Configuration.Config.UI.ForeColor,
			Utility.Configuration.Config.UI.Compass_ShipNameColor2,
			Utility.Configuration.Config.UI.Compass_ShipNameColor3,
			Utility.Configuration.Config.UI.Compass_ShipNameColor4,
			Utility.Configuration.Config.UI.Compass_ShipNameColor5,
			Utility.Configuration.Config.UI.Compass_ShipNameColor6,
			Utility.Configuration.Config.UI.Compass_ShipNameColor7
		};

		public Color GetShipNameColor()
		{

			if (!IsAbyssalShip)
			{
				return SystemColors.ControlText;
			}

			bool isLateModel = Name.Contains("後期型") || Name.Contains("Late Type");
			bool isRemodeled = Name.Contains("改") || Name.Contains("Kai");
			bool isDestroyed = Name.Contains("-壊") || Name.Contains("Damaged");
			bool isDemon = Name.Contains("鬼") || Name.Contains("Demon");
			bool isPrincess = Name.Contains("姫") || Name.Contains("Princess");
			bool isWaterDemon = Name.Contains("水鬼") || Name.Contains("Water Demon");
			bool isWaterPrincess = Name.Contains("水姫") || Name.Contains("Water Princess");
			bool isElite = NameReading == "elite";
			bool isFlagship = NameReading == "flagship";


			if (isDestroyed)
				return Utility.Configuration.Config.UI.Compass_ShipNameColorDestroyed;

			else if (isWaterPrincess)
				return ShipNameColors[6];
			else if (isWaterDemon)
				return ShipNameColors[5];
			else if (isPrincess)
				return ShipNameColors[4];
			else if (isDemon)
				return ShipNameColors[3];
			else
			{

				int tier;

				if (isFlagship)
					tier = 2;
				else if (isElite)
					tier = 1;
				else
					tier = 0;

				if (isLateModel || isRemodeled)
					tier += 3;

				return ShipNameColors[tier];
			}
		}


		public ShipDataMaster()
		{
			RemodelBeforeShipID = 0;
		}

		public ShipDataMaster BaseShip()
		{
			ShipDataMaster ship = this;

			while (ship.RemodelBeforeShip != null)
			{
				ship = ship.RemodelBeforeShip;
			}

			return ship;
		}

		public int ID => ShipID;


		public override string ToString() => $"[{ShipID}] {NameWithClass}";


	}

}
