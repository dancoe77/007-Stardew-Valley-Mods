﻿using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData;
using System;

namespace Shockah.ProjectFluent
{
	public class FluentApi: IFluentApi
	{
		private IFluentProvider FluentProvider { get; set; }
		private IFluentFunctionManager FluentFunctionManager { get; set; }
		private IFluentValueFactory FluentValueFactory { get; set; }

		internal FluentApi(IFluentProvider fluentProvider, IFluentFunctionManager fluentFunctionManager, IFluentValueFactory fluentValueFactory)
		{
			this.FluentProvider = fluentProvider;
			this.FluentFunctionManager = fluentFunctionManager;
			this.FluentValueFactory = fluentValueFactory;
		}

		public IGameLocale CurrentLocale =>
			ProjectFluent.Instance.CurrentLocale;

		public IGameLocale GetBuiltInLocale(LocalizedContentManager.LanguageCode languageCode)
			=> new IGameLocale.BuiltIn(languageCode);

		public IGameLocale GetModLocale(ModLanguage language)
			=> new IGameLocale.Mod(language);

		public IFluent<string> GetLocalizations(IGameLocale locale, IManifest mod, string? name = null)
			=> FluentProvider.GetFluent(locale, mod, name);

		public IFluent<string> GetLocalizationsForCurrentLocale(IManifest mod, string? name = null)
			=> new CurrentLocaleFluent(mod, name);

		public IEnumFluent<EnumType> GetEnumFluent<EnumType>(IFluent<string> baseFluent, string keyPrefix) where EnumType : Enum
			=> new EnumFluent<EnumType>(baseFluent, keyPrefix);

		public IFluent<T> GetMappingFluent<T>(IFluent<string> baseFluent, Func<T, string> mapper)
			=> new MappingFluent<T>(baseFluent, mapper);

		public void RegisterFunction(IManifest mod, string name, IFluentApi.FluentFunction function)
			=> FluentFunctionManager.RegisterFunction(mod, name, function);

		public void UnregisterFunction(IManifest mod, string name)
			=> FluentFunctionManager.UnregisterFunction(mod, name);

		public IFluentApi.IFluentFunctionValue CreateStringValue(string value)
			=> FluentValueFactory.CreateStringValue(value);

		public IFluentApi.IFluentFunctionValue CreateIntValue(int value)
			=> FluentValueFactory.CreateIntValue(value);

		public IFluentApi.IFluentFunctionValue CreateLongValue(long value)
			=> FluentValueFactory.CreateLongValue(value);

		public IFluentApi.IFluentFunctionValue CreateFloatValue(float value)
			=> FluentValueFactory.CreateFloatValue(value);

		public IFluentApi.IFluentFunctionValue CreateDoubleValue(double value)
			=> FluentValueFactory.CreateDoubleValue(value);
	}
}