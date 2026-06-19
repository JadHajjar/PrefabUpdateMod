using Colossal.Serialization.Entities;

using Game;
using Game.Common;
using Game.Prefabs;
using Game.SceneFlow;
using Game.UI.Widgets;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Unity.Collections;
using Unity.Entities;

namespace PrefabUpdateMod
{
	internal partial class PrefabUpdateModSystem : GameSystemBase
	{
		private PrefabSystem _prefabSystem;

		protected override void OnCreate()
		{
			base.OnCreate();

			_prefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();

			Enabled = false;
		}

		protected override void OnUpdate()
		{
		}

		protected override void OnGameLoadingComplete(Purpose purpose, GameMode mode)
		{
			Mod.Settings.ListVersion++;

			base.OnGameLoadingComplete(purpose, mode);
		}

		internal void UpdateAssets(int selectedPack)
		{
			if (selectedPack == 0)
			{
				return;
			}

			var pack = selectedPack == 1 ? null : _prefabSystem.GetPrefab<AssetPackPrefab>(new Entity { Index = (selectedPack - (selectedPack % 100)) / 100, Version = selectedPack % 10 });
			var em = _prefabSystem.EntityManager;
			var q = em.CreateEntityQuery(ComponentType.ReadOnly<PrefabRef>(), ComponentType.ReadOnly<Game.Objects.Object>());
			var ents = q.ToEntityArray(Allocator.Temp);
			var refs = q.ToComponentDataArray<PrefabRef>(Allocator.Temp);

			if (selectedPack == 1)
			{
				for (var i = 0; i < ents.Length; i++)
				{
					if (_prefabSystem.TryGetPrefab<PrefabBase>(refs[i], out var prefab) && prefab.isBuiltin)
					{
						em.AddComponent<Updated>(ents[i]);
					}
				}
			}
			else
			{
				for (var i = 0; i < ents.Length; i++)
				{
					if (_prefabSystem.TryGetPrefab<PrefabBase>(refs[i], out var prefab) && prefab.TryGet<AssetPackItem>(out var packs) && packs.m_Packs.Contains(pack))
					{
						em.AddComponent<Updated>(ents[i]);
					}
				}
			}
		}

		internal DropdownItem<int>[] GetAssetPacks()
		{
			var customPacks = new List<DropdownItem<int>>();
			var ents = _prefabSystem.EntityManager
				.CreateEntityQuery(ComponentType.ReadOnly<AssetPackData>())
				.ToEntityArray(Allocator.Temp);

			for (var i = 0; i < ents.Length; i++)
			{
				var assetName = _prefabSystem
					.GetPrefab<AssetPackPrefab>(ents[i])
					.name;

				customPacks.Add(new DropdownItem<int>
				{
					value = (ents[i].Index * 100) + ents[i].Version,
					displayName = GetPackName(assetName)
				});
			}

			customPacks.Sort((a, b) =>
				string.Compare(a.displayName.value, b.displayName.value, StringComparison.OrdinalIgnoreCase));

			var items = new DropdownItem<int>[customPacks.Count + 2];

			items[0] = new DropdownItem<int>
			{
				value = 0,
				displayName = "Select an asset pack"
			};

			items[1] = new DropdownItem<int>
			{
				value = 1,
				displayName = "Vanilla Assets Filter"
			};

			for (var i = 0; i < customPacks.Count; i++)
			{
				items[i + 2] = customPacks[i];
			}

			return items;
		}

		private static string GetPackName(string assetName)
		{
			var titleId = "Assets.NAME[" + assetName + "]";

			return GameManager.instance.localizationManager.activeDictionary.TryGetValue(titleId, out var name)
				? name
				: Regex.Replace(Regex.Replace(assetName,
					@"([a-z])([A-Z])", x => $"{x.Groups[1].Value} {x.Groups[2].Value}"),
					@"(\b)(?<!')([a-z])", x => $"{x.Groups[1].Value}{x.Groups[2].Value.ToUpper()}");
		}
	}
}
