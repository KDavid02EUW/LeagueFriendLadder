# LeagueFriendLadder

A League of Legends statisztikakövető és baráti ranglista webalkalmazás, amely egy egyetemi szakdolgozat keretein belül készült. A projekt célja a különböző régiókban játszó, de azonos baráti körhöz tartozó játékosok teljesítményének központosított aggregálása és összehasonlítása.

## Főbb Funkciók
* **Felhasználókezelés:** Biztonságos, hashelt jelszavakkal történő regisztráció és hitelesítés.
* **Riot Games API Integráció:** Játékosok adatainak lekérése Riot ID (GameName + TagLine) és PUUID alapján.
* **Dinamikus Ranglista (Ladder):** Több összekapcsolt Riot-fiók statisztikáinak (Tier, Rank, LP, Winrate) aggregált megjelenítése.
* **Részletes Meccselőzmények:** Az utolsó 20 mérkőzés adatainak és KDA statisztikáinak listázása egy interaktív, lenyitható panelen.
* **Adminisztrációs Panel:** Felhasználók és a hozzájuk csatolt Summoner fiókok jogosultság alapú kezelése.

## Alkalmazott Technológiák
* **Frontend:** Blazor WebAssembly (.NET 8)
* **Backend:** ASP.NET Core Web API
* **Adatbázis:** PostgreSQL (Neon.tech felhőalapú szolgáltatás)
* **ORM:** Entity Framework Core
* **Külső API:** Riot Games API (Account-V1, Summoner-V4, League-V4, Match-V5)

## Telepítés és Futtatás (Lokális környezet)

### Előfeltételek
* [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
* PostgreSQL adatbázis hozzáférés (lokális vagy felhő)
* Érvényes Riot Games Developer API kulcs

### Lépések
1. **Adattár klónozása:**
   ```bash
   git clone [https://github.com/FELHASZNALONEVED/LeagueFriendLadder.git](https://github.com/FELHASZNALONEVED/LeagueFriendLadder.git)
    ```
2. **Konfiguráció**
1. Navigálj a Backend API projektbe, és hozd létre vagy módosítsd az appsettings.json fájlt a saját hitelesítő adataiddal. 
   Fontos: A valós API kulcsodat soha ne commitold a verziókezelőbe!
   ```json
     {
        "ConnectionStrings": {
        "PostgreSql": "Host=localhost;Database=LeagueLadder;Username=postgres;Password=titkos"
        },
        "RiotApi": {
        "ApiKey": "YOUR_RIOT_API_KEY"
        }
    }
     ```
3. **Adatbázis migrációk futtatása**
    ```bash
    dotnet ef database update
    ```


4. **Alkalmazás indítása**
Nyisd meg a LeagueFriendLadder.sln fájlt Visual Studio-ban és indítsd el a projektet a zöld "Play" gombbal, vagy használd a parancssort a projekt gyökerében:
```bash
    dotnet run
```