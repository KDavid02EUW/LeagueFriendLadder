using Microsoft.AspNetCore.Mvc;
using Dapper;
using LeagueFriendLadder.Api.Services;
using System.Data;
using LeagueFriendLadder.Models;
using LeagueFriendLadder.Services;

namespace LeagueFriendLadder.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SummonerController : ControllerBase
    {
        private readonly DatabaseService _db;
        private readonly RiotService _riotService;

        public SummonerController(DatabaseService db, RiotService riotService)
        {
            _db = db;
            _riotService = riotService;
        }

        [HttpPost("save")]
        public async Task<IActionResult> SavePlayer([FromBody] LeagueEntryDTO player, [FromQuery] string tag, [FromQuery] string region)
        {
            if (player == null || string.IsNullOrEmpty(player.Puuid))
            {
                return BadRequest("Invalid summoner data.");
            }

            using var connection = _db.CreateConnection();

            double winrate = 0;
            if ((player.Wins + player.Losses) > 0)
            {
                winrate = _riotService.getWinrate(player);
            }

            string sql = @"
                INSERT INTO ""Summoner"" (
                    ""Puuid"", ""Name"", ""Tag"", ""Tier"", ""Rank"", 
                    ""LP"", ""Win"", ""Loss"", ""Winrate"", ""Region""
                ) 
                VALUES (
                    @Puuid, @SummonerName, @Tag, @Tier, @Rank, 
                    @LeaguePoints, @Wins, @Losses, @Winrate, @Region
                )
                ON CONFLICT (""Puuid"") DO UPDATE 
                SET ""Name"" = @SummonerName, 
                    ""Tag"" = @Tag,
                    ""Tier"" = @Tier, 
                    ""Rank"" = @Rank, 
                    ""LP"" = @LeaguePoints,
                    ""Win"" = @Wins,
                    ""Loss"" = @Losses,
                    ""Winrate"" = @Winrate,
                    ""Region"" = @Region;";

            await connection.ExecuteAsync(sql, new
            {
                player.Puuid,
                SummonerName = player.SummonerName,
                Tag = tag,
                player.Tier,
                player.Rank,
                LeaguePoints = player.LeaguePoints,
                Wins = player.Wins,
                Losses = player.Losses,
                Winrate = winrate,
                Region = region
            });

            return Ok(new { message = $"Summoner ({player.SummonerName}) successfully saved!" });
        }

        [HttpGet("{puuid}")]
        public async Task<IActionResult> GetSummoner(string puuid)
        {
            using var connection = _db.CreateConnection();
            string sql = @"SELECT * FROM ""Summoner"" WHERE ""Puuid"" = @puuid";
            var summoner = await connection.QueryFirstOrDefaultAsync(sql, new { puuid });
            if (summoner == null) return NotFound("User not found in the database");
            return Ok(summoner);
        }

        [HttpGet("ladder")]
        public async Task<IActionResult> GetLadder()
        {
            using var connection = _db.CreateConnection();
            string sql = @"
                SELECT * FROM ""Summoner"" 
                ORDER BY 
                    CASE ""Tier""
                        WHEN 'CHALLENGER' THEN 1
                        WHEN 'GRANDMASTER' THEN 2
                        WHEN 'MASTER' THEN 3
                        WHEN 'DIAMOND' THEN 4
                        WHEN 'PLATINUM' THEN 5
                        WHEN 'EMERALD' THEN 6
                        WHEN 'GOLD' THEN 7
                        WHEN 'SILVER' THEN 8
                        WHEN 'BRONZE' THEN 9
                        WHEN 'IRON' THEN 10
                        ELSE 11
                    END, 
                    ""LP"" DESC 
                LIMIT 100";

            var ladder = await connection.QueryAsync(sql);
            return Ok(ladder);
        }
    }
}