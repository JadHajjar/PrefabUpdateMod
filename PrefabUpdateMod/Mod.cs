using Colossal.IO.AssetDatabase;

using Game;
using Game.Modding;
using Game.SceneFlow;

namespace PrefabUpdateMod
{
	public class Mod : IMod
	{
		internal static Setting Settings;

		public void OnLoad(UpdateSystem updateSystem)
		{
			updateSystem.UpdateAt<PrefabUpdateModSystem>(SystemUpdatePhase.MainLoop);

			Settings = new Setting(this);
			Settings.RegisterInOptionsUI();
			GameManager.instance.localizationManager.AddSource("en-US", new LocaleEN(Settings));

			AssetDatabase.global.LoadSettings(nameof(PrefabUpdateMod), Settings, new Setting(this));
		}

		public void OnDispose()
		{
			Settings?.UnregisterInOptionsUI();
			Settings = null;
		}
	}
}
