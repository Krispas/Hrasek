public class PlayerExeptions(string mesage) : Exception(mesage);
public class PlayerNotFoundExeption(string nickname) : PlayerExeptions($"Hráč s přezdívkou {nickname} nebyl nalezen");
public class PlayerAlreadyInTournamentExeption(string nickname) : PlayerExeptions($"Hráč {nickname} je již v tomto turnaji");
public class PlayerNickanemeAreadyTakenExeption(string nickname) : PlayerExeptions($"{nickname} je již v použita");
public class PlayerPlayerTournamentExeption(string nickname) : PlayerExeptions($"Hráč nelze smazat, již odehraj první turnaj");
public class PlayersNotFoundExeption() : PlayerExeptions($"Žádní hráči nenalezeni");
public class TournamentExeptions(string mesage) : Exception(mesage);

public class TournamentAlreadyExistsExeption() : TournamentExeptions("Pouze 1 turnaj múže být aktivní");
public class TournamentNotExistingExeption() : TournamentExeptions("Musíte první vytvořit turnaj");
