﻿// <auto-generated />
//
// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using QuickType;
//
//    var champInfo = ChampInfo.FromJson(jsonString);

namespace QuickType
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class ChampInfo
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("alias")]
        public string Alias { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("shortBio")]
        public string ShortBio { get; set; }

        [JsonProperty("tacticalInfo")]
        public TacticalInfo TacticalInfo { get; set; }

        [JsonProperty("playstyleInfo")]
        public PlaystyleInfo PlaystyleInfo { get; set; }

        [JsonProperty("squarePortraitPath")]
        public string SquarePortraitPath { get; set; }

        [JsonProperty("stingerSfxPath")]
        public string StingerSfxPath { get; set; }

        [JsonProperty("chooseVoPath")]
        public string ChooseVoPath { get; set; }

        [JsonProperty("banVoPath")]
        public string BanVoPath { get; set; }

        [JsonProperty("roles")]
        public string[] Roles { get; set; }

        [JsonProperty("recommendedItemDefaults")]
        public object[] RecommendedItemDefaults { get; set; }

        [JsonProperty("skins")]
        public Skin[] Skins { get; set; }

        [JsonProperty("passive")]
        public Passive Passive { get; set; }

        [JsonProperty("spells")]
        public Spell[] Spells { get; set; }
    }

    public partial class Passive
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("abilityIconPath")]
        public string AbilityIconPath { get; set; }

        [JsonProperty("abilityVideoPath")]
        public string AbilityVideoPath { get; set; }

        [JsonProperty("abilityVideoImagePath")]
        public string AbilityVideoImagePath { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }

    public partial class PlaystyleInfo
    {
        [JsonProperty("damage")]
        public long Damage { get; set; }

        [JsonProperty("durability")]
        public long Durability { get; set; }

        [JsonProperty("crowdControl")]
        public long CrowdControl { get; set; }

        [JsonProperty("mobility")]
        public long Mobility { get; set; }

        [JsonProperty("utility")]
        public long Utility { get; set; }
    }

    public partial class Skin
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("isBase")]
        public bool IsBase { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("splashPath")]
        public string SplashPath { get; set; }

        [JsonProperty("uncenteredSplashPath")]
        public string UncenteredSplashPath { get; set; }

        [JsonProperty("tilePath")]
        public string TilePath { get; set; }

        [JsonProperty("loadScreenPath")]
        public string LoadScreenPath { get; set; }

        [JsonProperty("skinType")]
        public string SkinType { get; set; }

        [JsonProperty("rarity")]
        public string Rarity { get; set; }

        [JsonProperty("isLegacy")]
        public bool IsLegacy { get; set; }

        [JsonProperty("splashVideoPath")]
        public object SplashVideoPath { get; set; }

        [JsonProperty("collectionSplashVideoPath")]
        public object CollectionSplashVideoPath { get; set; }

        [JsonProperty("featuresText")]
        public object FeaturesText { get; set; }

        [JsonProperty("chromaPath")]
        public string ChromaPath { get; set; }

        [JsonProperty("emblems")]
        public object Emblems { get; set; }

        [JsonProperty("regionRarityId")]
        public long RegionRarityId { get; set; }

        [JsonProperty("rarityGemPath")]
        public object RarityGemPath { get; set; }

        [JsonProperty("skinLines")]
        public SkinLine[] SkinLines { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("loadScreenVintagePath", NullValueHandling = NullValueHandling.Ignore)]
        public string LoadScreenVintagePath { get; set; }

        [JsonProperty("chromas", NullValueHandling = NullValueHandling.Ignore)]
        public Chroma[] Chromas { get; set; }
    }

    public partial class Chroma
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("chromaPath")]
        public string ChromaPath { get; set; }

        [JsonProperty("colors")]
        public string[] Colors { get; set; }

        [JsonProperty("descriptions")]
        public Description[] Descriptions { get; set; }

        [JsonProperty("rarities")]
        public Rarity[] Rarities { get; set; }
    }

    public partial class Description
    {
        [JsonProperty("region")]
        public string Region { get; set; }

        [JsonProperty("description")]
        public string DescriptionDescription { get; set; }
    }

    public partial class Rarity
    {
        [JsonProperty("region")]
        public string Region { get; set; }

        [JsonProperty("rarity")]
        public long RarityRarity { get; set; }
    }

    public partial class SkinLine
    {
        [JsonProperty("id")]
        public long Id { get; set; }
    }

    public partial class Spell
    {
        [JsonProperty("spellKey")]
        public string SpellKey { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("abilityIconPath")]
        public string AbilityIconPath { get; set; }

        [JsonProperty("abilityVideoPath")]
        public string AbilityVideoPath { get; set; }

        [JsonProperty("abilityVideoImagePath")]
        public string AbilityVideoImagePath { get; set; }

        [JsonProperty("cost")]
        public string Cost { get; set; }

        [JsonProperty("cooldown")]
        public string Cooldown { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("dynamicDescription")]
        public string DynamicDescription { get; set; }

        [JsonProperty("range")]
        public long[] Range { get; set; }

        [JsonProperty("costCoefficients")]
        public long[] CostCoefficients { get; set; }

        [JsonProperty("cooldownCoefficients")]
        public double[] CooldownCoefficients { get; set; }

        [JsonProperty("coefficients")]
        public Coefficients Coefficients { get; set; }

        [JsonProperty("effectAmounts")]
        public Dictionary<string, long[]> EffectAmounts { get; set; }

        [JsonProperty("ammo")]
        public Ammo Ammo { get; set; }

        [JsonProperty("maxLevel")]
        public long MaxLevel { get; set; }
    }

    public partial class Ammo
    {
        [JsonProperty("ammoRechargeTime")]
        public long[] AmmoRechargeTime { get; set; }

        [JsonProperty("maxAmmo")]
        public long[] MaxAmmo { get; set; }
    }

    public partial class Coefficients
    {
        [JsonProperty("coefficient1")]
        public long Coefficient1 { get; set; }

        [JsonProperty("coefficient2")]
        public long Coefficient2 { get; set; }
    }

    public partial class TacticalInfo
    {
        [JsonProperty("style")]
        public long Style { get; set; }

        [JsonProperty("difficulty")]
        public long Difficulty { get; set; }

        [JsonProperty("damageType")]
        public string DamageType { get; set; }
    }

    public partial class ChampInfo
    {
        public static ChampInfo FromJson(string json) => JsonConvert.DeserializeObject<ChampInfo>(json, QuickType.Converter.Settings);
    }
}