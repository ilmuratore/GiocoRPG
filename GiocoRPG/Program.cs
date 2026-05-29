// ═══════════════════════════════════════════════════════════════════
// PROGRAM — Program.cs
// ═══════════════════════════════════════════════════════════════════
// Punto di ingresso del programma.
//
// Il Main del Giorno 5 è conservato commentato con /* */ —
// mantiene la storia del progetto leggibile senza occupare spazio
// nell'esecuzione.
//
// Il Main attuale (Giorno 6) dimostra le quattro collezioni:
//   1. List<T>         — già nota, usata consapevolmente
//   2. Dictionary<K,V> — Inventario con quantità
//   3. HashSet<T>      — effetti attivi sul personaggio
//   4. Queue<T>        — coda di spawn nemici
// ═══════════════════════════════════════════════════════════════════

using System.Globalization;
using System.Linq.Expressions;
using System.Runtime.InteropServices.Marshalling;
using System.Security.Cryptography.X509Certificates;

namespace GiocoRPG
{
    class Program
    {
        // ─────────────────────────────────────────────────────────────
        // MAIN PRECEDENTE (Giorno 5) — conservato come riferimento
        // ─────────────────────────────────────────────────────────────
        /*
        static void Main_Giorno5(string[] args)
        {
            Console.WriteLine(" === GIOCO RPG === ");

            // DEMO 1 — Lista eterogenea e CalcolaDanno() polimorfico
            List<Personaggio> party = new()
            {
                PersonaggioFactory.CreaGuerriero("Terenzio"),
                PersonaggioFactory.CreaMago("Mago Silenzio"),
                PersonaggioFactory.CreaLadro("Lupin")
            };

            Console.WriteLine("=== CalcolaDanno() ===");
            Console.ReadLine();
            foreach (Personaggio p in party)
            {
                int danno = p.CalcolaDanno();
                Console.WriteLine($" {p.Nome} infligge {danno} danni ({p.GetType().Name})");
            }

            // DEMO 2 — TipoDanno e immunità Scheletro
            Console.WriteLine("\n === Scheletro Test Immunità danni === \n");
            Console.ReadLine();
            Nemico scheletro = Nemico.CreaScheletro();
            scheletro.SubisciDanno(20, TipoDanno.Fisico);
            scheletro.SubisciDanno(20, TipoDanno.Puro);
            scheletro.SubisciDanno(20, TipoDanno.Magico);   // ← immune!

            // DEMO 3 — SimulaScontro
            Console.WriteLine("\n SCONTRO 1 VS 1 \n");
            Console.ReadLine();
            SistemaTurni.SimulaScontro(
                PersonaggioFactory.CreaGuerriero("Arthur"),
                Nemico.CreaGoblin()
            );

            // DEMO 4 — EseguiRound: party vs boss Drago
            Console.Clear();
            Console.WriteLine("\n SCONTRO PARTY VS BOSS \n");
            Console.ReadLine();
            var sistema = new SistemaTurni();
            sistema.Aggiungi(PersonaggioFactory.CreaGuerriero("Eroe 1"));
            sistema.Aggiungi(PersonaggioFactory.CreaMago("Eroe 2"));
            sistema.Aggiungi(PersonaggioFactory.CreaLadro("Eroe 3"));
            sistema.StampaParty();
            Nemico boss = Nemico.CreaDrago();
            Console.WriteLine($"\n BOSS: {boss.Nome}\n");
            int round = 1;
            while (boss.IsVivo() && round <= 20)
            {
                Console.WriteLine($" === ROUND {round} ===");
                sistema.EseguiRound(boss);
                round++;
            }
            Console.WriteLine(boss.IsVivo()
                ? $"\n BOSS ancora vivo ({boss.Vita})"
                : $" {boss.Nome} è stato sconfitto");

            // DEMO 5 — IAttacabile come tipo
            IAttacabile combattente = new Guerriero("Simone");
            combattente.CalcolaDanno();
            List<IAttacabile> tutti = new() { new Guerriero("A"), new Mago("B"), Nemico.CreaGoblin() };
            foreach (IAttacabile personaggio in tutti)
                Console.WriteLine(personaggio.CalcolaDanno());

            // DEMO 6 — ISalvabile con is
            Personaggio p1 = new Guerriero("Test");
            if (p1 is ISalvabile salvabile)
                Console.WriteLine(salvabile.Serializza());

            // DEMO 7 — IOggetto polimorfico
            Personaggio eroe = new Guerriero("Artur");
            IOggetto[] zaino = { new Pozione("Grande", 60), new Arma("Spada", 20) };
            foreach (IOggetto strumento in zaino)
                strumento.Usa(eroe);

            // DEMO 8 — Inventario con IEnumerable
            var zaino2 = new Inventario();
            zaino2.Aggiungi(new Pozione("Pozione Grande", 60));
            zaino2.Aggiungi(new Arma("Spada", 20));
            foreach (IOggetto obj in zaino2)
                Console.WriteLine(obj.Nome);
            var armi = zaino2.Where(obj => obj.Tipo == "Arma").ToList();
        }
        */

        /*
        static void Main(string[] args)
        {
            // Il vecchio Main del Giorno 5 (incapsulamento, IOggetto, ISalvabile)
            // è commentato sopra con /*  — il Giorno 6 dimostra le collezioni. 
            /*
            Console.WriteLine("=== GIOCO RPG ===");
            // ... demo Giorno 5 ... (commentata per dare spazio alla demo Giorno 6)
            */


        // ─────────────────────────────────────────────────────────
        // 1. LIST<T> — già conosciuta, usiamola consapevolmente
        //
        // Accesso per indice party[0] → O(1): calcolo diretto dell'indirizzo.
        // party.Remove(frodo)         → O(n): deve trovarlo prima di rimuoverlo.
        //
        // List è la scelta giusta qui perché l'ordine del party conta
        // e accediamo spesso per posizione.
        // ─────────────────────────────────────────────────────────
        /*Console.WriteLine("TEST COLLECTION");

        List<Personaggio> party = new()
        {
            PersonaggioFactory.CreaGuerriero("Aragorn"),
            PersonaggioFactory.CreaMago("Gandalf"),
            PersonaggioFactory.CreaLadro("Frodo")
        };

        Console.WriteLine($"{party[0].Nome}");   // accesso O(1) per indice
        Console.WriteLine($"{party.Count}");

        Personaggio frodo = party[2];
        party.Remove(frodo);                     // O(n) — deve trovarlo prima
        Console.WriteLine($"{party.Count}");

        Console.WriteLine("Premi invio");
        Console.ReadLine();
        Console.Clear();


        // ─────────────────────────────────────────────────────────
        // 2. DICTIONARY — inventario con quantità
        //
        // Aggiungi la stessa pozione due volte → quantità sommata,
        // non due entry separate. Questo è il vantaggio del Dictionary:
        // la chiave (Nome) è unica, il valore (quantità) si aggiorna.
        //
        // Rimuovi con quantità parziale → entry non eliminata.
        // GetQuantita → O(1), nessun ciclo.
        // ─────────────────────────────────────────────────────────
        Console.WriteLine("TEST DIZIONARIO");

        var zaino = new Inventario();
        zaino.Aggiungi(new Pozione("Pozione Grande", 60), 4);  // 4 pozioni
        zaino.Aggiungi(new Arma("Spada", 100), 1);
        zaino.Mostra();

        zaino.Rimuovi("Pozione Grande", 2);  // consuma 2 delle 4: ne restano 2
        Console.WriteLine($"pozioni rimaste nello zaino: {zaino.GetQuantita("Pozione Grande")}");

        Console.WriteLine("Premi invio");
        Console.ReadLine();
        Console.Clear();


        // ─────────────────────────────────────────────────────────
        // 3. HASHSET — effetti attivi sul personaggio
        //
        // AggiungiEffetto("Veleno") due volte → un solo effetto attivo.
        // HashSet.Add restituisce false se l'elemento era già presente.
        // EffettiAttivi.Count → 3, non 4, anche con Veleno aggiunto due volte.
        // HaEffetto → O(1): usa la hash table interna, nessun ciclo.
        // ─────────────────────────────────────────────────────────
        var eroe = PersonaggioFactory.CreaGuerriero("Simone");
        eroe.AggiungiEffetto("Veleno");
        eroe.AggiungiEffetto("Veleno");    // ignorato: già presente → false
        eroe.AggiungiEffetto("Fuoco");
        eroe.AggiungiEffetto("Cura");

        Console.WriteLine($"Effetti attivi {eroe.EffettiAttivi.Count}");    // 3, non 4
        Console.WriteLine($"Ha veleno ? : {eroe.HaEffetto("Veleno")}");    // True
        eroe.ApplicaEffetti();
        eroe.RimuoviEffetto("Fuoco");
        Console.WriteLine($"Effetti attivi dopo rimozione : {eroe.EffettiAttivi.Count}");  // 2

        Console.WriteLine("Premi invio");
        Console.ReadLine();
        Console.Clear();


        // ─────────────────────────────────────────────────────────
        // 4. QUEUE — coda di spawn nemici (FIFO)
        //
        // I nemici escono nell'ordine esatto in cui sono stati accodati:
        // Goblin → Scheletro → Orco → Drago.
        // Dequeue() è O(1): sposta un puntatore, non sposta elementi.
        //
        // Func<Nemico> (senza parentesi) = passare il metodo come ricetta,
        // non chiamarlo subito. Il nemico viene creato solo al momento
        // della chiamata a ProssimoNemico() → lazy creation.
        // ─────────────────────────────────────────────────────────
        var coda = new CodaSpawn();
        coda.Accoda(
            Nemico.CreaGoblin,      // metodo passato come Func<Nemico>
            Nemico.CreaScheletro,
            Nemico.CreaOrco,
            Nemico.CreaDrago
        );
        coda.StampaCoda();

        Console.WriteLine();
        while (coda.HaNemici)
        {
            Nemico? nemico = coda.ProssimoNemico();  // Dequeue O(1) + crea il nemico
            if (nemico == null) break;

            // Ogni scontro crea un eroe fresco: il nemico non è mai lo stesso
            Personaggio protagonista = PersonaggioFactory.CreaGuerriero("Eroe");
            SistemaTurni.SimulaScontro(protagonista, nemico);
        }



        //C# introduzione ad errori non gestiti.
        int.Parse("abc"); // crash automatico. Che errore restituisce ? 0, 1, -1
        int[] arr = { 1, 2, 3 };
        Console.WriteLine(arr[5]); //crash automatico 



        Exception; //la classe padre di tutte le classi delle eccezioni
            NullReferenceException; // usato quando qualcosa é null
            IndexOutOfRangeException; // usato quando indichiamo un elemento fuori dagli indici di un array
            InvalidOperationException; // usato quando eseguiamo un operazione non valida in quel momento 
            ArgumentException; // usato quando passiamo argomenti non validi
                ArgumentNullException; //usato quando passiamo argomenti null
                ArgumentOutOfRangeException; // usato quando passiamo valodi fuori dall'intervallo valido
            FormatException; // usato quando passiamo un valore non valido (int.Parse("abc");)
            DivideByZeroException; // usato quando effettiamo una divisione per zero
            FileNotFoundException; // usato quando non trova il riferimento al file giusto

        //Creare una classe di eccezzione personalizzata.



        //Struttura Try/Catch/Finally
        try
        {
            Console.WriteLine("Inserisci la vita (1 - 200): ");
            string input = Console.ReadLine()!; // input non é un numero 
            if (!int.TryParse(input, out int vita)) Console.WriteLine("Inserisci un numero: ");
            int danno = 100 / vita; // errore se vita é pari a 0
            Console.WriteLine($"danno ridotto:  {danno}");
        }
        catch (FormatException)
        {
            Console.WriteLine("Errore: inserisci un numero intero");
        }
        catch (DivideByZeroException)
        {
            Console.WriteLine("Errore: la vita non puó essere zero");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Errore imprevisto: {ex.Message}");
        }
        finally
        {
            Console.WriteLine("Elaborazione completata.");
        };




        // .? = accede al membro solo se l'oggetto non é null 
        Personaggio? p = null;

        string nome = p.Nome;

        string inputUtente = Console.ReadLine(); 
        // ?? = permette di avere un valore di riserva se é null
        string nomeGiocatore = inputUtente ?? "Ospite";

        // ??= = permette di assegnare solo se quella é null
        nome ??= "Ospite";



        using (var log = new FileSalvataggio("partita.log"))
        {
            log.Scrivi("Partita Iniziata");
            log.Scrivi("Boss Sconfitto");
        }


    }     // fine Main
        */


        static void Main(string[] args)
        {
            Console.WriteLine("=== GIOCO RPG v2 ===");


            Console.WriteLine("=== Try Catch esempio ===");
            try
            {
                Console.WriteLine("Inserisci un numero: ");
                int valore = int.Parse(Console.ReadLine()!);
                Console.WriteLine($"Valore: {valore}");


            } catch (FormatException)
            {
                Console.WriteLine("Input non valido. Non era un numero.");
            }

            Console.WriteLine("Premi invio");
            Console.ReadLine();
            Console.Clear();

            Console.WriteLine("== Eccezioni Personalizzate ==");
            var zaino = new Inventario();
            zaino.Aggiungi(new Pozione("Pozione Grande", 50), 2);
            zaino.Aggiungi(new Arma("Spada", 100), 1);

            //Oggetto non trovato
            try
            {
                zaino.UsaOggetto("Arco", PersonaggioFactory.CreaGuerriero("Test"));
            } catch (OggettoNonTrovatoException ex)
            {
                Console.WriteLine($"Non esiste nell'inventario l'oggetto: {ex.NomeOggetto}");
            }

            try
            {
                zaino.Aggiungi(null!);
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine($"parametro nullo: {ex.ParamName}");
            }

            Console.WriteLine("Premi invio");
            Console.ReadLine();
            Console.Clear();

            Console.WriteLine("== Guadagna Esperienza ==");
            var eroe = PersonaggioFactory.CreaGuerriero("Aragorn");
            Console.WriteLine(eroe);
            eroe.GuadagnaEsperienza(80);
            eroe.GuadagnaEsperienza(50);

            Console.WriteLine(eroe); 
            
            try
            {
                eroe.GuadagnaEsperienza(-10);
            } catch(ArgumentOutOfRangeException ex)
            {
                Console.WriteLine($"Errore: {ex.Message}");
            }


            Console.WriteLine("Premi invio");
            Console.ReadLine();
            Console.Clear();

            Console.WriteLine("== Gestore Partita  ==");

            var partita = new GestionePartita(); 
            partita.StampaStato();
            partita.SelezionaClasse("m");
            partita.NuovoNemico(3);
            partita.AggiungiAlleato(PersonaggioFactory.CreaLadro("Frodo"));
            partita.StampaStato();

            while(partita.PartitaInCorso)
            {
                try { partita.EseguiTurnoGiocatore(); } catch(InvalidOperationException ex) { Console.WriteLine(ex.Message); break; };
            }


            Console.WriteLine("Premi invio");
            Console.ReadLine();
            Console.Clear();

            Console.WriteLine("== File Salvataggio  ==");

            int vita = eroe.Vita;
            string cl = eroe.Nome;

            using (var log = new FileSalvataggio("partita.log"))
            {
                log.Scrivi("Partita Iniziata");
                log.Scrivi($"Giocatore: {cl} con {vita} HP");
            }

            using var log2 = new FileSalvataggio("sessione.log");
            log2.Scrivi("Fine sessione");

        }
     
    }         // fine class Program
}             // fine namespace GiocoRPG
