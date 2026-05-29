// ═══════════════════════════════════════════════════════════════════
// PROGRAM — Program.cs
// ═══════════════════════════════════════════════════════════════════
// Punto di ingresso del programma — Giorno 7.
//
// La demo mostra in ordine i concetti del Giorno 7:
//   1. try/catch base
//   2. Eccezioni personalizzate (OggettoNonTrovatoException, ArgumentNullException)
//   3. Guard clause con GuadagnaEsperienza()
//   4. Nullable e operatori ?., ??, ??= tramite GestionePartita
//   5. IDisposable e using tramite FileSalvataggio
// ═══════════════════════════════════════════════════════════════════

namespace GiocoRPG
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== GIOCO RPG — Giorno 7 ===");


            // ─────────────────────────────────────────────────────────
            // 1. TRY/CATCH BASE
            // int.Parse lancia FormatException se l'input non è un numero.
            // Il catch specifico gestisce solo quel caso.
            // Il programma NON crasha: continua dopo il blocco try/catch.
            // ─────────────────────────────────────────────────────────
            Console.WriteLine("\n=== Try Catch esempio ===");
            try
            {
                Console.WriteLine("Inserisci un numero: ");
                int valore = int.Parse(Console.ReadLine()!);
                Console.WriteLine($"Valore: {valore}");
            }
            catch (FormatException)
            {
                Console.WriteLine("Input non valido. Non era un numero.");
            }

            Console.WriteLine("Premi invio");
            Console.ReadLine();
            Console.Clear();


            // ─────────────────────────────────────────────────────────
            // 2. ECCEZIONI PERSONALIZZATE
            // OggettoNonTrovatoException porta ex.NomeOggetto:
            //   il catch può mostrare il nome esatto senza leggere il messaggio.
            // ArgumentNullException porta ex.ParamName:
            //   il catch sa quale parametro era null.
            // ─────────────────────────────────────────────────────────
            Console.WriteLine("== Eccezioni Personalizzate ==");

            var zaino = new Inventario();
            zaino.Aggiungi(new Pozione("Pozione Grande", 50), 2);
            zaino.Aggiungi(new Arma("Spada", 100), 1);

            // Oggetto non trovato: Trova() lancia OggettoNonTrovatoException
            try
            {
                zaino.UsaOggetto("Arco", PersonaggioFactory.CreaGuerriero("Test"));
            }
            catch (OggettoNonTrovatoException ex)
            {
                Console.WriteLine($"Non esiste nell'inventario l'oggetto: {ex.NomeOggetto}");
            }

            // Argomento null: ArgumentNullException.ThrowIfNull in Aggiungi()
            try
            {
                zaino.Aggiungi(null!);   // null! = forziamo il null per testare
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine($"Parametro nullo: {ex.ParamName}");
            }

            Console.WriteLine("Premi invio");
            Console.ReadLine();
            Console.Clear();


            // ─────────────────────────────────────────────────────────
            // 3. GUARD CLAUSE — GuadagnaEsperienza
            // I primi due XP sono validi e causano un level up.
            // Il terzo è negativo: la guard clause lancia subito
            // ArgumentOutOfRangeException prima di toccare l'Esperienza.
            // ─────────────────────────────────────────────────────────
            Console.WriteLine("== Guadagna Esperienza ==");

            var eroe = PersonaggioFactory.CreaGuerriero("Aragorn");
            Console.WriteLine(eroe);

            eroe.GuadagnaEsperienza(80);   // valido
            eroe.GuadagnaEsperienza(50);   // supera 100 → level up

            Console.WriteLine(eroe);

            try
            {
                eroe.GuadagnaEsperienza(-10);   // guard clause: XP negativo
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine($"Errore: {ex.Message}");
            }

            Console.WriteLine("Premi invio");
            Console.ReadLine();
            Console.Clear();


            // ─────────────────────────────────────────────────────────
            // 4. GESTORE PARTITA — nullable, ?., ??, ??=
            // StampaStato() con campi null mostra i fallback con ??.
            // Dopo SelezionaClasse e NuovoNemico, i campi hanno valore.
            // EseguiTurnoGiocatore lancia InvalidOperationException
            // se non c'è più un nemico in scena (after kill).
            // ─────────────────────────────────────────────────────────
            Console.WriteLine("== Gestore Partita ==");

            var partita = new GestionePartita();
            partita.StampaStato();                                      // tutto null → fallback ??

            partita.SelezionaClasse("m");
            partita.NuovoNemico(3);
            partita.AggiungiAlleato(PersonaggioFactory.CreaLadro("Frodo"));
            partita.StampaStato();

            while (partita.PartitaInCorso)
            {
                try { partita.EseguiTurnoGiocatore(); }
                catch (InvalidOperationException ex) { Console.WriteLine(ex.Message); break; }
            }

            Console.WriteLine("Premi invio");
            Console.ReadLine();
            Console.Clear();


            // ─────────────────────────────────────────────────────────
            // 5. IDISPOSABLE — using e chiusura automatica del file
            //
            // Sintassi blocco: Dispose() alla chiusura della graffa }
            //   Il file viene chiuso anche se accade un'eccezione nel blocco.
            //
            // Sintassi dichiarazione (C# 8+): Dispose() alla fine del metodo.
            //   Più conciso, ma il file rimane aperto fino alla fine di Main.
            // ─────────────────────────────────────────────────────────
            Console.WriteLine("== File Salvataggio ==");

            // Sintassi blocco
            using (var log = new FileSalvataggio("partita.log"))
            {
                log.Scrivi("Partita Iniziata");
                log.Scrivi($"Giocatore: {eroe.Nome} con {eroe.Vita} HP");
            }   // ← Dispose() qui, file chiuso

            // Sintassi dichiarazione
            using var log2 = new FileSalvataggio("sessione.log");
            log2.Scrivi("Fine sessione");
            // Dispose() alla fine di Main

        }     // fine Main
    }         // fine class Program
}             // fine namespace GiocoRPG
