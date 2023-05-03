﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Shockah.SeasonAffixes
{
	internal interface IAffixSetWeightProvider
	{
		double GetWeight(IReadOnlySet<ISeasonAffix> combination, OrdinalSeason season);
	}

	internal static class IAffixSetWeightProviderExt
	{
		public static IAffixSetWeightProvider MultiplyingBy(this IAffixSetWeightProvider provider, IAffixSetWeightProvider anotherProvider)
			=> new MultiplyingAffixSetWeightProvider(provider, anotherProvider);
	}

	internal sealed class DefaultProbabilityAffixSetWeightProvider : IAffixSetWeightProvider
	{
		public double GetWeight(IReadOnlySet<ISeasonAffix> combination, OrdinalSeason season)
			=> combination.Average(a => a.GetProbabilityWeight(season));
	}

	internal sealed class MultiplyingAffixSetWeightProvider : IAffixSetWeightProvider
	{
		private IAffixSetWeightProvider Base { get; init; }
		private IAffixSetWeightProvider Multiplier { get; init; }

		public MultiplyingAffixSetWeightProvider(IAffixSetWeightProvider @base, IAffixSetWeightProvider multiplier)
		{
			this.Base = @base;
			this.Multiplier = multiplier;
		}

		public double GetWeight(IReadOnlySet<ISeasonAffix> combination, OrdinalSeason season)
		{
			double weight = Base.GetWeight(combination, season);
			if (weight > 0)
				weight *= Multiplier.GetWeight(combination, season);
			return weight;
		}
	}

	internal sealed class DelegateAffixSetWeightProvider : IAffixSetWeightProvider
	{
		private Func<IReadOnlySet<ISeasonAffix>, OrdinalSeason, double> Delegate { get; init; }

		public DelegateAffixSetWeightProvider(Func<IReadOnlySet<ISeasonAffix>, OrdinalSeason, double> @delegate)
		{
			this.Delegate = @delegate;
		}

		public double GetWeight(IReadOnlySet<ISeasonAffix> combination, OrdinalSeason season)
			=> Delegate(combination, season);
	}

	internal sealed class ConfigAffixSetWeightProvider : IAffixSetWeightProvider
	{
		private IReadOnlyDictionary<string, double> AffixWeights { get; init; }

		public ConfigAffixSetWeightProvider(IReadOnlyDictionary<string, double> affixWeights)
		{
			this.AffixWeights = affixWeights;
		}

		public double GetWeight(IReadOnlySet<ISeasonAffix> combination, OrdinalSeason season)
			=> combination.Average(a => AffixWeights.TryGetValue(a.UniqueID, out var weight) ? weight : 1.0);
	}

	internal sealed class CustomAffixSetWeightProvider : IAffixSetWeightProvider
	{
		private IReadOnlyList<Func<IReadOnlySet<ISeasonAffix>, OrdinalSeason, double?>> CustomProviders { get; init; }

		public CustomAffixSetWeightProvider(IReadOnlyList<Func<IReadOnlySet<ISeasonAffix>, OrdinalSeason, double?>> customProviders)
		{
			this.CustomProviders = customProviders;
		}

		public double GetWeight(IReadOnlySet<ISeasonAffix> combination, OrdinalSeason season)
		{
			var totalWeight = 1.0;
			foreach (var provider in CustomProviders)
			{
				var weight = provider(combination, season);
				if (weight is not null)
				{
					totalWeight *= weight.Value;
					if (totalWeight <= 0)
						break;
				}
			}
			return totalWeight;
		}
	}

	internal sealed class PairingUpTagsAffixSetWeightProvider : IAffixSetWeightProvider
	{
		private IReadOnlySet<ISeasonAffix> AllAffixes { get; init; }
		private Func<int, double> UnpairedAffixMultiplier { get; init; }
		private double OneSidedPairedAffixesMultiplier { get; init; }
		private int PairedAffixLimit { get; init; }
		private double TooManyPairedAffixesMultiplier { get; init; }

		public PairingUpTagsAffixSetWeightProvider(
			IReadOnlySet<ISeasonAffix> allAffixes,
			Func<int, double> unpairedAffixMultiplier,
			double oneSidedPairedAffixesMultiplier,
			int pairedAffixLimit,
			double tooManyPairedAffixesMultiplier
		)
		{
			this.AllAffixes = allAffixes;
			this.UnpairedAffixMultiplier = unpairedAffixMultiplier;
			this.OneSidedPairedAffixesMultiplier = oneSidedPairedAffixesMultiplier;
			this.PairedAffixLimit = pairedAffixLimit;
			this.TooManyPairedAffixesMultiplier = tooManyPairedAffixesMultiplier;
		}

		public double GetWeight(IReadOnlySet<ISeasonAffix> combination, OrdinalSeason season)
		{
			var weight = 1.0;
			var relatedAffixDictionary = combination.ToDictionary(a => a, a => combination.Where(a2 => a2.Tags.Any(t => a.Tags.Contains(t))).ToHashSet());
			foreach (var (affix, relatedAffixes) in relatedAffixDictionary)
			{
				if (relatedAffixes.Count == 1)
				{
					if (affix.Tags.Count > 0)
					{
						int possibleTagAffixes = AllAffixes.Where(a => a.Tags.Any(t => affix.Tags.Contains(t))).Count();
						weight *= UnpairedAffixMultiplier(possibleTagAffixes);
					}
				}
				else
				{
					if (relatedAffixes.Sum(a => a.GetPositivity(season)) == 0 || relatedAffixes.Sum(a => a.GetNegativity(season)) == 0)
						weight *= OneSidedPairedAffixesMultiplier;
					if (relatedAffixes.Count >= PairedAffixLimit)
						weight *= TooManyPairedAffixesMultiplier;
				}
			}
			return weight;
		}
	}

	internal sealed class AvoidingChoiceHistoryDuplicatesAffixSetWeightProvider : IAffixSetWeightProvider
	{
		private double RepeatedWeight { get; init; }

		public AvoidingChoiceHistoryDuplicatesAffixSetWeightProvider(double repeatedWeight)
		{
			this.RepeatedWeight = repeatedWeight;
		}

		public double GetWeight(IReadOnlySet<ISeasonAffix> combination, OrdinalSeason season)
		{
			foreach (var affix in combination)
				foreach (var step in SeasonAffixes.Instance.SaveData.AffixChoiceHistory)
					if (step.Contains(affix))
						return RepeatedWeight;
			return 1;
		}
	}

	internal sealed class AvoidingSetChoiceHistoryDuplicatesAffixSetWeightProvider : IAffixSetWeightProvider
	{
		private double RepeatedWeight { get; init; }

		public AvoidingSetChoiceHistoryDuplicatesAffixSetWeightProvider(double repeatedWeight)
		{
			this.RepeatedWeight = repeatedWeight;
		}

		public double GetWeight(IReadOnlySet<ISeasonAffix> combination, OrdinalSeason season)
		{
			foreach (var step in SeasonAffixes.Instance.SaveData.AffixSetChoiceHistory)
				foreach (var stepCombination in step)
					if (stepCombination.SetEquals(combination))
						return RepeatedWeight;
			return 1;
		}
	}
}