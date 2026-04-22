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
            try
            {
                using var connection = _db.CreateConnection();

                string sql = @"
                INSERT INTO summoners (puuid, name, tag, tier, rank, lp, win, loss, winrate, region, userid) 
                VALUES (@Puuid, @SummonerName, @Tag, @Tier, @Rank, @LeaguePoints, @Wins, @Losses, @Winrate, @Region, @UserId)
                ON CONFLICT (puuid) DO UPDATE 
                SET name = @SummonerName, tag = @Tag, tier = @Tier, rank = @Rank, 
                    lp = @LeaguePoints, win = @Wins, loss = @Losses, winrate = @Winrate, region = @Region;";

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
                    Winrate = player.Winrate,
                    Region = region,
                    UserId = 1
                });

                return Ok(new { message = "Success!" });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Database Error: " + ex.Message);

                return StatusCode(500, "Error in the API: " + ex.Message);
            }
        }

        [HttpGet("{puuid}")]
        public async Task<IActionResult> GetSummoner(string puuid)
        {
            using var connection = _db.CreateConnection();
            string sql = @"SELECT * FROM summoners WHERE puuid = @puuid";
            var summoner = await connection.QueryFirstOrDefaultAsync(sql, new { puuid });
            if (summoner == null) return NotFound("User not found in the database");
            return Ok(summoner);
        }

        [HttpGet("ladder")]
        public async Task<IActionResult> GetLadder()
        {
            using var connection = _db.CreateConnection();
            string sql = @"
                SELECT * FROM summoners 
                ORDER BY 
                    CASE tier
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
                    lp DESC 
                LIMIT 100";

            var ladder = await connection.QueryAsync(sql);
            return Ok(ladder);
        }
    }
}