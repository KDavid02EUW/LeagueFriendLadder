using Microsoft.AspNetCore.Mvc;
using Dapper;
using LeagueFriendLadder.Api.Services;

namespace LeagueFriendLadder.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SummonerController : ControllerBase
    {
        private readonly DatabaseService _db;

        public SummonerController(DatabaseService db)
        {
            _db = db;
        }

        [HttpPost("save")]
        public async Task<IActionResult> SavePlayer([FromBody] LeagueEntryDTO player)
        {
            using var connection = _db.CreateConnection();

            // SQL lekérdezés: Ha már létezik a PUUID, frissítjük, ha nem, beszúrjuk
            string sql = @"
            INSERT INTO players (puuid, summoner_name, tier, rank, lp) 
            VALUES (@Puuid, @SummonerName, @Tier, @Rank, @LeaguePoints)
            ON CONFLICT (puuid) DO UPDATE 
            SET summoner_name = @SummonerName, lp = @LeaguePoints;";

            await connection.ExecuteAsync(sql, player);
            return Ok(new { message = "Player saved successfully!" });
        }
    }
}
