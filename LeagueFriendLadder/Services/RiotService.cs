using LeagueFriendLadder.Models;
using System.Net.Http.Json;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;

public class RiotService
{
    private readonly HttpClient _http;
    private readonly string apiKey = "RGAPI-15de7229-e9c5-49f8-a1d0-69a63a7bb4ab";

    public RiotService(HttpClient http)
    {
        _http = http;
    }

    public async Task<RiotAccount?> GetRiotID(string riotId)
    {
        var parts = riotId.Split("#");
        if (parts.Length < 2)
        {
            throw new ArgumentException("Invalid format. Use Name#Tag.");
        }

        var summonerName = parts[0];
        var tag = parts[1];

        var url = $"https://europe.api.riotgames.com/riot/account/v1/accounts/by-riot-id/{summonerName}/{tag}?api_key={apiKey}";
        return await _http.GetFromJsonAsync<RiotAccount>(url);
    }

    public async Task<LeagueEntryDTO?> GetSummonerDetailsByPuuidAsync(string puuid, string region)
    {
        var url = $"https://{region.ToLower()}.api.riotgames.com/lol/league/v4/entries/by-puuid/{puuid}?api_key={apiKey}";
        var response = await _http.GetFromJsonAsync<List<LeagueEntryDTO>>(url);

        if (response != null)
        {
            foreach (var entry in response)
            {
                if (entry.QueueType == "RANKED_SOLO_5x5")
                {
                    entry.Winrate = getWinrate(entry);
                    return entry;
                }
            }
        }

        return null;
    }
    public double getWinrate(LeagueEntryDTO e)
    {
        double games = e.Wins + e.Losses;
        return Math.Round((e.Wins / games) * 100 ,1);
    }
    public async Task<RiotAccount?> GetRiotIDByPuuid(string puuid)
    {
        var url = $"https://europe.api.riotgames.com/riot/account/v1/accounts/by-puuid/{puuid}?api_key={apiKey}";
        return await _http.GetFromJsonAsync<RiotAccount>(url);
    }

    public async Task<List<MatchDTO>> getMatchesByPuuid(string puuid)
    {
        var urlIds = $"https://europe.api.riotgames.com/lol/match/v5/matches/by-puuid/{puuid}/ids?start=0&count=20&api_key={apiKey}";
        var ids = await _http.GetFromJsonAsync<List<string>>(urlIds);

        var resultList = new List<MatchDTO>();

        if (ids != null)
        {
            foreach (var id in ids)
            {
                var matchUrl = $"https://europe.api.riotgames.com/lol/match/v5/matches/{id}?api_key={apiKey}";

                var rawMatch = await _http.GetFromJsonAsync<RiotMatchRoot>(matchUrl);

                if (rawMatch?.info?.participants != null)
                {
                    var newMatch = new MatchDTO { MatchID = id };

                    foreach (var p in rawMatch.info.participants)
                    {
                        newMatch.Participants.Add(new ParticipantDTO
                        {
                            championName = p.championName,
                            deaths = p.deaths,
                            damageDealtToTurrets = p.damageDealtToTurrets,
                            totalDamageDealtToChampions = p.totalDamageDealtToChampions,
                            totalDamageTaken = p.totalDamageTaken,
                            damageDealtToObjectives = p.damageDealtToObjectives,
                            damageSelfMitigated = p.damageSelfMitigated,
                            controlWardsPlaced = p.controlWardsPlaced,
                            dragonKills = p.dragonKills,
                            goldEarned = p.goldEarned,
                            teamPosition = p.teamPosition,
                            item0 = p.item0,
                            item1 = p.item1,
                            item2 = p.item2,
                            item3 = p.item3,
                            item4 = p.item4,
                            item5 = p.item5,
                            item6 = p.item6,
                            kills = p.kills,
                            assists = p.assists,
                            largestCrit = p.largestCrit,
                            largestMultiKill = p.largestMultiKill,
                            summoner1ID = p.summoner1ID,
                            summoner2ID = p.summoner2ID,
                            riotIDTagLine = p.riotIDTagLine,
                            riotIDGameName = p.riotIDGameName,
                            totalHeal = p.totalHeal,
                            visionScore = p.visionScore,
                            visionWardsBought = p.visionWardsBought,
                            win = p.win,
                            champLevel = p.champLevel,
                            totalAllyJungleMinionsKilled = p.totalAllyJungleMinionsKilled,
                            totalEnemyJungleMinionsKilled = p.totalEnemyJungleMinionsKilled,
                            neutralMinionsKilled = p.neutralMinionsKilled,
                            totalMinionsKilled = p.totalMinionsKilled + p.neutralMinionsKilled,
                        });
                    }
                    resultList.Add(newMatch);
                }
            }
        }
        return resultList;
    }

}
