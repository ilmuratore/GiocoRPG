// ═══════════════════════════════════════════════════════════════════
// GESTIONE PARTITA — Gestione/GestionePartita.cs
// ═══════════════════════════════════════════════════════════════════
// Classe che orchestra una partita: giocatore, nemico corrente, alleati.
//
// CAMPI NULLABLE:
//   I tre campi privati usano "?" per dichiarare esplicitamente
//   che queste entità potrebbero non essere ancora presenti.
//   Il compilatore avvisa se si accede a questi campi senza controllo null.
//
// OPERATORI NULL:
//   ?.  → accede al membro solo se l'oggetto non è null
//   ??  → restituisce il valore di destra se il sinistro è null
//   ??= → assegna solo se la variabile è null (lazy initialization)
//
// GUARD CLAUSE:
//   Ogni metodo verifica le precondizioni all'inizio e lancia
//   immediatamente se qualcosa non va — il corpo del metodo
//   può assumere che tutto sia valido.
// ═══════════════════════════════════════════════════════════════════

namespace GiocoRPG.Gestione
{
    public class GestionePartita
    {
        // Campi nullable: possono non essere ancora impostati.
        // "?" = il compilatore avvisa se li usiamo senza controllo null.
        private Personaggio?       _giocatore;
        private Nemico?            _nemicoCorrente;
        private List<Personaggio>? _alleati;   // lazy: creata solo al primo alleato


        // ─────────────────────────────────────────────────────────────
        // PROPRIETÀ CALCOLATE CON NULL SAFETY
        //
        // PartitaInCorso: _giocatore?.IsVivo() restituisce bool? (nullable).
        //   ?? false: se _giocatore è null, la partita non è in corso.
        //
        // NomeBoss: _nemicoCorrente?.Nome restituisce string?.
        //   ?? "Nessun Boss": fallback se non c'è nessun nemico.
        // ─────────────────────────────────────────────────────────────
        public bool   PartitaInCorso => _giocatore?.IsVivo() ?? false;
        public string NomeBoss       => _nemicoCorrente?.Nome ?? "Nessun Boss";


        // ─────────────────────────────────────────────────────────────
        // SELEZIONA CLASSE
        // Guard clause: ArgumentException.ThrowIfNullOrWhiteSpace — C# 10+.
        // Switch expression: ogni caso restituisce un Personaggio diverso.
        // "_" (default): classe non riconosciuta → eccezione con messaggio chiaro.
        // nameof(classe): il nome del parametro aggiornato automaticamente.
        // ─────────────────────────────────────────────────────────────
        public void SelezionaClasse(string classe)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(classe);

            _giocatore = classe.ToLower() switch
            {
                "g" => PersonaggioFactory.CreaGuerriero("Eroe"),
                "m" => PersonaggioFactory.CreaMago("Eroe"),
                "l" => PersonaggioFactory.CreaLadro("Eroe"),
                _   => throw new ArgumentException(
                           $"Classe '{classe}' non valida. Usa: g, m, l.",
                           nameof(classe))
            };
            Console.WriteLine($"Classe giocatore: {_giocatore.GetType().Name}");
        }


        // ─────────────────────────────────────────────────────────────
        // NUOVO NEMICO
        // ArgumentOutOfRangeException.ThrowIfNegativeOrZero — C# 8+.
        // Livello 0 o negativo non ha senso → guard clause.
        // ─────────────────────────────────────────────────────────────
        public void NuovoNemico(int livello)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(livello);
            _nemicoCorrente = Nemico.CreaPerLivello(livello);
            Console.WriteLine($"Nemico: {_nemicoCorrente.Nome}");
        }


        // ─────────────────────────────────────────────────────────────
        // AGGIUNGI ALLEATO
        // ??= (null-coalescing assignment): crea la lista solo al primo
        // alleato. Equivale a: if (_alleati == null) _alleati = new();
        // Lazy initialization: non spreca memoria se non si aggiungono alleati.
        // ─────────────────────────────────────────────────────────────
        public void AggiungiAlleato(Personaggio alleato)
        {
            ArgumentNullException.ThrowIfNull(alleato);

            _alleati ??= new List<Personaggio>();   // crea la lista solo la prima volta
            _alleati.Add(alleato);
            Console.WriteLine($"Nuovo alleato: {alleato.Nome}");
        }


        // ─────────────────────────────────────────────────────────────
        // ESEGUI TURNO GIOCATORE
        // Guard clause con "is null": preferito a "== null" per i
        // nullable reference types — più chiaro sull'intenzione.
        //
        // Se il nemico muore:
        //   - GuadagnaEsperienza() usa XpReward da IPremiabile
        //   - DropItem?.  → mostra il drop solo se non è null
        //   - _nemicoCorrente = null → azzera il riferimento
        // ─────────────────────────────────────────────────────────────
        public void EseguiTurnoGiocatore()
        {
            if (_giocatore is null)
                throw new InvalidOperationException("Seleziona prima una classe.");
            if (_nemicoCorrente is null)
                throw new InvalidOperationException("Nessun nemico presente.");

            int danno = _giocatore.CalcolaDanno();
            _nemicoCorrente.SubisciDanno(danno);

            if (!_nemicoCorrente.IsVivo())
            {
                _giocatore.GuadagnaEsperienza(_nemicoCorrente.XpReward);

                // DropItem? è string? — null se il nemico non droppa niente
                Console.WriteLine(_nemicoCorrente.DropItem != null
                    ? $"Drop: {_nemicoCorrente.DropItem}"
                    : "Nessun item da droppare.");

                _nemicoCorrente = null;   // nemico sconfitto: azzera il riferimento
            }
        }


        // ─────────────────────────────────────────────────────────────
        // STAMPA STATO
        // ?. e ?? combinati su ogni campo nullable:
        //   _giocatore?.Nome    → null se _giocatore è null
        //   ?? "Non selezionato" → fallback leggibile
        //
        // _giocatore?.Vita.ToString() → necessario .ToString() perché
        //   Vita è int: int? non si concatina direttamente con ??.
        //
        // _alleati?.Count ?? 0 → 0 se la lista non è ancora stata creata.
        // ─────────────────────────────────────────────────────────────
        public void StampaStato()
        {
            Console.WriteLine(" == Stato ==");
            Console.WriteLine($" Giocatore: {_giocatore?.Nome ?? "Non selezionato"}");
            Console.WriteLine($" Vita:      {_giocatore?.Vita.ToString() ?? "-"}");
            Console.WriteLine($" Livello:   {_giocatore?.Livello.ToString() ?? "-"}");
            Console.WriteLine($" Boss:      {NomeBoss}");
            Console.WriteLine($" Alleati:   {_alleati?.Count ?? 0}");
        }
    }
}
