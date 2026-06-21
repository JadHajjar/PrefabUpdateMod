using Colossal;
using Colossal.IO.AssetDatabase;
using Colossal.Json;

using Game.Modding;
using Game.Settings;
using Game.UI.Widgets;

using System.Collections.Generic;

using Unity.Entities;

namespace PrefabUpdateMod
{
	[FileLocation(nameof(PrefabUpdateMod))]
	[SettingsUIGroupOrder(kDropdownGroup)]
	[SettingsUIShowGroupName()]
	public class Setting : ModSetting
	{
		public const string kSection = "Main";
		public const string kDropdownGroup = "Main";

		public Setting(IMod mod) : base(mod) { }

		[Exclude]
		[SettingsUIHidden]
		public int ListVersion { get; set; }

		[Exclude]
		[SettingsUIValueVersion(typeof(Setting), nameof(ListVersion))]
		[SettingsUIDropdown(typeof(Setting), nameof(GetIntDropdownItems))]
		[SettingsUISection(kSection, kDropdownGroup)]
		public int SelectedPack { get; set; }

		[Exclude]
		[SettingsUISection(kSection, kDropdownGroup)]
		public bool Button { set => World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<PrefabUpdateModSystem>()?.UpdateAssets(SelectedPack); }

		public DropdownItem<int>[] GetIntDropdownItems() => World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<PrefabUpdateModSystem>()?.GetAssetPacks() ?? new DropdownItem<int>[0];

		public override void SetDefaults()
		{
		}
	}

	public class LocaleEN : IDictionarySource
	{
		private readonly Setting m_Setting;
		public LocaleEN(Setting setting)
		{
			m_Setting = setting;
		}

		public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts)
		{
			return new Dictionary<string, string>
			{
				{ m_Setting.GetSettingsLocaleID(), "Asset Update Mod" },
				{ m_Setting.GetOptionTabLocaleID(Setting.kSection), "Main" },
				{ m_Setting.GetOptionGroupLocaleID(Setting.kDropdownGroup), "Main" },

				{ m_Setting.GetOptionLabelLocaleID(nameof(Setting.Button)), "UPDATE ASSETS" },
				{ m_Setting.GetOptionDescLocaleID(nameof(Setting.Button)), $"Updates all placed assets from the selected pack." },

				{ m_Setting.GetOptionLabelLocaleID(nameof(Setting.SelectedPack)), "Asset Pack" },
				{ m_Setting.GetOptionDescLocaleID(nameof(Setting.SelectedPack)), $"Select a pack to be updated." },
			};
		}

		public void Unload()
		{

		}
	}
}
