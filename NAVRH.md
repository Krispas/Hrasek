
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
