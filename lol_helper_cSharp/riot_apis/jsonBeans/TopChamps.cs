﻿// <auto-generated />
//
// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using QuickType;
//
//    var topChamps = TopChamps.FromJson(jsonString);

namespace QuickType
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class TopChamps
    {
        [JsonProperty("masteries")]
        public Mastery[] Masteries { get; set; }

        [JsonProperty("score")]
        public long Score { get; set; }

        [JsonProperty("summonerId")]
        public long SummonerId { get; set; }
    }

    public partial class Mastery
    {
        [JsonProperty("championId")]
        public long ChampionId { get; set; }

        [JsonProperty("championLevel")]
        public long ChampionLevel { get; set; }

        [JsonProperty("championPoints")]
        public long ChampionPoints { get; set; }

        [JsonProperty("championPointsSinceLastLevel")]
        public long ChampionPointsSinceLastLevel { get; set; }

        [JsonProperty("championPointsUntilNextLevel")]
        public long ChampionPointsUntilNextLevel { get; set; }

        [JsonProperty("chestGranted")]
        public bool ChestGranted { get; set; }

        [JsonProperty("formattedChampionPoints")]
        public string FormattedChampionPoints { get; set; }

        [JsonProperty("formattedMasteryGoal")]
        public string FormattedMasteryGoal { get; set; }

        [JsonProperty("highestGrade")]
        public string HighestGrade { get; set; }

        [JsonProperty("lastPlayTime")]
        public long LastPlayTime { get; set; }

        [JsonProperty("playerId")]
        public long PlayerId { get; set; }

        [JsonProperty("tokensEarned")]
        public long TokensEarned { get; set; }
    }

    public partial class TopChamps
    {
        public static TopChamps FromJson(string json) => JsonConvert.DeserializeObject<TopChamps>(json, QuickType.Converter.Settings);
    }
}
