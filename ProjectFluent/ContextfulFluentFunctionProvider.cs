﻿using StardewModdingAPI;
using System.Collections.Generic;
using System.Linq;

namespace Shockah.ProjectFluent
{
	internal interface IContextfulFluentFunctionProvider
	{
		IEnumerable<(string name, ContextfulFluentFunction function)> GetFluentFunctionsForMod(IManifest mod);
	}

	internal delegate IFluentApi.IFluentFunctionValue ContextfulFluentFunction(IGameLocale locale, IReadOnlyList<IFluentApi.IFluentFunctionValue> positionalArguments, IReadOnlyDictionary<string, IFluentApi.IFluentFunctionValue> namedArguments);

	internal class ContextfulFluentFunctionProvider: IContextfulFluentFunctionProvider
	{
		private IManifest ProjectFluentMod { get; set; }
		private IFluentFunctionProvider FluentFunctionProvider { get; set; }

		public ContextfulFluentFunctionProvider(IManifest projectFluentMod, IFluentFunctionProvider fluentFunctionProvider)
		{
			this.ProjectFluentMod = projectFluentMod;
			this.FluentFunctionProvider = fluentFunctionProvider;
		}

		public IEnumerable<(string name, ContextfulFluentFunction function)> GetFluentFunctionsForMod(IManifest mod)
		{
			var remainingFunctions = FluentFunctionProvider.GetFluentFunctions().ToList();

			var projectFluentFunctions = remainingFunctions.Where(f => f.mod.UniqueID == ProjectFluentMod.UniqueID).ToList();
			foreach (var function in projectFluentFunctions)
				remainingFunctions.Remove(function);

			var modFunctions = remainingFunctions.Where(f => f.mod.UniqueID == mod.UniqueID).ToList();
			foreach (var function in modFunctions)
				remainingFunctions.Remove(function);

			IEnumerable<(string name, ContextfulFluentFunction function)> EnumerableFunctions(IEnumerable<(IManifest mod, string name, IFluentApi.FluentFunction function)> input)
			{
				foreach (var function in input)
				{
					IFluentApi.IFluentFunctionValue ContextfulFunction(IGameLocale locale, IReadOnlyList<IFluentApi.IFluentFunctionValue> positionalArguments, IReadOnlyDictionary<string, IFluentApi.IFluentFunctionValue> namedArguments)
						=> function.function(locale, mod, positionalArguments, namedArguments);

					yield return (function.name, ContextfulFunction);
					yield return ($"{ProjectFluentMod.UniqueID}/{function.name}", ContextfulFunction);
				}
			}

			foreach (var function in EnumerableFunctions(projectFluentFunctions))
				yield return function;
			foreach (var function in EnumerableFunctions(modFunctions))
				yield return function;
			foreach (var function in EnumerableFunctions(remainingFunctions))
				yield return function;
		}
	}
}