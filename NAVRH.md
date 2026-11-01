
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



