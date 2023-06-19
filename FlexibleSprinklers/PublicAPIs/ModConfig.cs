﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shockah.Kokoro;
using StardewModdingAPI;
using System.Collections.Generic;
using System.Linq;

namespace Shockah.FlexibleSprinklers
{
	public class ModConfig : IVersioned.Modifiable
	{
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)] public ISemanticVersion? Version { get; set; }
		[JsonProperty] public SprinklerBehaviorEnum SprinklerBehavior { get; internal set; } = SprinklerBehaviorEnum.ClusterWithoutVanilla;
		[JsonProperty] public bool IgnoreRange { get; internal set; } = false;
		[JsonProperty] public bool SplitDisconnectedClusters { get; internal set; } = true;
		[JsonProperty] public ClusterSprinklerBehaviorClusterOrdering ClusterBehaviorClusterOrdering { get; internal set; } = ClusterSprinklerBehaviorClusterOrdering.BiggerFirst;
		[JsonProperty] public ClusterSprinklerBehaviorBetweenClusterBalanceMode ClusterBehaviorBetweenClusterBalanceMode { get; internal set; } = ClusterSprinklerBehaviorBetweenClusterBalanceMode.Relaxed;
		[JsonProperty] public ClusterSprinklerBehaviorInClusterBalanceMode ClusterBehaviorInClusterBalanceMode { get; internal set; } = ClusterSprinklerBehaviorInClusterBalanceMode.Relaxed;
		[JsonProperty] public bool ActivateOnPlacement { get; internal set; } = false;
		[JsonProperty] public bool ActivateOnAction { get; internal set; } = false;
		[JsonProperty] public bool ActivateBeforeSleep { get; internal set; } = true;
		[JsonProperty] public float CoverageAlpha { get; internal set; } = 1;
		[JsonProperty] public float CoverageTimeInSeconds { get; internal set; } = 5;
		[JsonProperty] public float CoverageAnimationInSeconds { get; internal set; } = 1;
		[JsonProperty] public bool CoverageOverlayDuplicates { get; internal set; } = true;
		[JsonProperty] public bool ShowCoverageOnPlacement { get; internal set; } = true;
		[JsonProperty] public bool ShowCoverageOnAction { get; internal set; } = true;
		[JsonProperty] public ISet<IntPoint> Tier1Coverage { get; internal set; } = IntPoint.Zero.GetSpiralingTiles().Distinct().Take(4).ToHashSet();
		[JsonProperty] public ISet<IntPoint> Tier2Coverage { get; internal set; } = IntPoint.Zero.GetSpiralingTiles().Distinct().Take(3 * 3 - 1).ToHashSet();
		[JsonProperty] public ISet<IntPoint> Tier3Coverage { get; internal set; } = IntPoint.Zero.GetSpiralingTiles().Distinct().Take(5 * 5 - 1).ToHashSet();
		[JsonProperty] public ISet<IntPoint> Tier4Coverage { get; internal set; } = IntPoint.Zero.GetSpiralingTiles().Distinct().Take(7 * 7 - 1).ToHashSet();
		[JsonProperty] public ISet<IntPoint> Tier5Coverage { get; internal set; } = IntPoint.Zero.GetSpiralingTiles().Distinct().Take(9 * 9 - 1).ToHashSet();
		[JsonProperty] public ISet<IntPoint> Tier6Coverage { get; internal set; } = IntPoint.Zero.GetSpiralingTiles().Distinct().Take(11 * 11 - 1).ToHashSet();
		[JsonProperty] public ISet<IntPoint> Tier7Coverage { get; internal set; } = IntPoint.Zero.GetSpiralingTiles().Distinct().Take(13 * 13 - 1).ToHashSet();
		[JsonProperty] public ISet<IntPoint> Tier8Coverage { get; internal set; } = IntPoint.Zero.GetSpiralingTiles().Distinct().Take(15 * 15 - 1).ToHashSet();
		[JsonProperty] public bool CompatibilityMode { get; internal set; } = true;
		[JsonProperty] public bool WaterGardenPots { get; internal set; } = false;
		[JsonProperty] public bool WaterPetBowl { get; internal set; } = false;
		[JsonProperty] public bool WaterAtSprinkler { get; internal set; } = false;
		[JsonExtensionData] internal IDictionary<string, JToken> ExtensionData { get; set; } = new Dictionary<string, JToken>();
	}
}