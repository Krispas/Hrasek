public class PlayerExeptions(string mesage) : Exception(mesage);
public class PlayerNotFoundExeption(string nickname) : PlayerExeptions($"Hráč s přezdívkou {nickname} nebyl nalezen");
public class PlayerAlreadyInTournament(string nickname) : PlayerExeptions($"Hráč {nickname} je již v tomoto turnaji")
public class TournamentExeptions(string mesage) : Exception(mesage);
