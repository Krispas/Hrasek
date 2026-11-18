
## Exeptions

### PlayerExeptions
- Základní třída pro výjimky spojené s hráči.
- Přijímá chybovou zprávu (`message`).

**Odvozené třídy:**
- `PlayerNotFoundExeption` – hráč s danou přezdívkou neexistuje.
- `PlayerAlreadyInTournamentExeption` – hráč je již přihlášen do turnaje.
- `PlayerNickanemeAreadyTakenExeption` – přezdívka je již obsazená.
- `PlayerPlayerTournamentExeption` – hráče nelze smazat, protože již odehrál turnaj.
- `PlayersNotFoundExeption` – nebyli nalezeni žádní hráči.

### TournamentExeptions
- Základní třída pro výjimky spojené s turnaji.
- Přijímá chybovou zprávu (`message`).

**Odvozené třídy:**
- `TournamentAlreadyExistsExeption` – již existuje aktivní turnaj.
- `TournamentNotExistingExeption` – turnaj ještě nebyl vytvořen.
- `TournamentNotStartedExeption` – turnaj ještě nebyl spuštěn.

## Matches

### Třída Match
- Uchovává informace a jednotlivých zápasech.
- Používá se při losování a ukládání výsledků.

**Uchovává:**
- `Player1` (Player) – první hráč zápasu.
- `Player2` (Player) – druhý hráč zápasu.
- `Round` (int) – číslo kola.
- `scoreP1` (int) – skóre prvního hráče.
- `scoreP2` (int) – skóre druhého hráče.
- `winner` (Player) – vítěz zápasu.

## Ostatní třídy

### Třída Player
- Uchovává základní údaje o jednom hráči.

Uchovává:
- `Name` (string) – jméno hráče
- `nickname` (string) – jedinečná přezdívka hráče
- `tournamentsPlayed` (int) – počet turnajů které hráč už odehrál

### Třída PlayerManager
- Stará se o seznam všech hráčů, jejich přidávání, mazání a hlídá aby se neopakovaly přezdívky

Uchovává:
- `players` (List<Player>) – seznam všech hráčů

### Třída Tournament
- Uchovává údaje a aktuální stav jednoho turnaje

Uchovává:
- `Name` (string) – název turnaje
- `Discipline` (string) – disciplína nebo hra ve které se soutěží
- `tournamentPlayers` (List<Player>) – seznam hráčů přihlášených doturnaje
- `tournamentStarted` (bool) – určuje jestli už turnaj začal.
- `matches` (List<Match>) – seznam všech zápasů v turnaji
- `numberOfRounds` (int?) – celkový počet kol turnaje
- `currentRound` (int) – číslo aktuálního kola

### Třída TournamentManager
- Řídí průběh turnaje – založení, přihlášení hráčů, spuštění, přechod do dalšího kola a ukončení

Uchovává:
- `activeTournament` (Tournament?) – právě spuštěný nebo rozpracovaný turnaj
- `playerManager` (PlayerManager) – odkaz na správce hráčů.

### Třída MatchManager
- Vytváří zápasy v turnaji a zapisuje do nich vítěze, remízy a body

Uchovává:
- `activeTournament` (Tournament) – odkaz na aktivní turnaj
- `playerManager` (PlayerManager) – odkaz na správce hráčů
  
### Třída Losovani
- Statická třída, která rozlosuje dvojice hráčů pro dané kolo systémem každý s každým

Uchovává:
- `VolnyLos` (string) – konstanta `"VOLNY_LOS"`, kterou dostane hráč při lichém počtu účastníků

### Třída LosovaniOvereni
- Otestování losování. Kontroluje že každý hráč hraje v kole jen jednou, dvojice se neopakují a volný los vyjde na každého stejněkrát

### Třída Ulozeni
- Stará se o uložení rozehraného turnaje a hráčů do souboru a jejich zpětné načtení

Uchovává:
- `cesta` (string) – název a cesta k souboru (`"turnaj.txt"`)

## Formát ukládání

### turnaj.txt
- Data se ukládají do textového souboru `turnaj.txt` který je rozdělený do čtyř sekcí. Každý záznam je na novém řádku a hodnoty jsou oddělené středníkem (`;`):
- PLAYERS: seznam všech hráčů ve tvaru `Name;nickname;tournamentsPlayed`
- TOURNAMENT: informace o aktivním turnaji ve tvaru `Name;Discipline;tournamentStarted;numberOfRounds;currentRound` 
- TOURNAMENT_PLAYERS: přezdívky hráčů přihlášených do turnaje
- MATCHES: jednotlivé zápasy ve tvaru `Player1;Player2;Round;scoreP1;scoreP2;winner;matchFinished` 

### Odůvodnění
- Textový soubor se dá kdykoliv otevřít v Poznámkovém bloku a hned je vidět co se do něj uložilo takže se to dobře kontroluje a testuje
- Rozdělení řádků přes `Split(';')` funguje přímo v základním C# bez stahování balíčků
- Do souboru se nemusí ukládat celé objekty hráčů pořád dokola, u turnaje a zápasů stačí uložit jejich unikátní `nickname` a při načítání se podle ní hráči zase spárují
- Při načítání program kontroluje jestli nechybí nějaká sekce nebo část řádku, takže když někdo soubor upraví nebo poškodí program nespadne a jen vypíše chybu



