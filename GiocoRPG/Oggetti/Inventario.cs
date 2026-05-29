// ═══════════════════════════════════════════════════════════════════
// INVENTARIO — Oggetti/Inventario.cs
// ═══════════════════════════════════════════════════════════════════
// GIORNO 5: usava List<IOggetto>.
// GIORNO 6: refactoring a Dictionary<string, (IOggetto, int)>.
//
// Vantaggi del Dictionary rispetto alla List:
//   - Ricerca per nome O(1) invece di O(n)
//   - Quantità integrata nel valore — niente campi paralleli
//   - TryGetValue sicuro: nessuna eccezione per chiavi mancanti
//   - Aggiungere lo stesso oggetto due volte somma la quantità
//     invece di creare un duplicato
//
// La chiave è il Nome dell'oggetto (string).
// Il valore è una tupla nominata: l'oggetto + la sua quantità.
//
// TUPLA NOMINATA (IOggetto Oggetto, int Quantita):
//   Raggruppa due valori senza creare una classe separata.
//   I nomi (Oggetto, Quantita) rendono il codice leggibile:
//   entry.Oggetto e entry.Quantita invece di entry.Item1 e entry.Item2.
// ═══════════════════════════════════════════════════════════════════

namespace GiocoRPG.Oggetti
{
    public class Inventario : IEnumerable<IOggetto>, ISalvabile
    {
        // Dictionary: chiave = Nome oggetto, valore = (oggetto, quantità).
        // readonly: il Dictionary stesso non può essere sostituito dopo la creazione,
        // ma il suo contenuto può essere modificato (Add, Remove...).
        private readonly Dictionary<string, (IOggetto Oggetto, int Quantita)> _oggetti = new();

        private const int MAX_SLOT = 20 ;


        // Proprietà calcolata: legge la dimensione del Dictionary.
        public int Count => _oggetti.Count;


        // ─────────────────────────────────────────────────────────────
        // AGGIUNGI
        // TryGetValue cerca la chiave in O(1).
        //   Se trovata (out var esistente): aggiorna solo la quantità.
        //   Se non trovata: crea una nuova entry.
        //
        // _oggetti[obj.Nome] = ... sovrascrive il valore esistente —
        // Dictionary non permette chiavi duplicate: assegnare sulla stessa
        // chiave aggiorna il valore senza errori.
        // ─────────────────────────────────────────────────────────────
        public void Aggiungi(IOggetto? obj, int quantita = 1) // .? = accedere al membro solo se l'oggetto non é null
        {
            ArgumentNullException.ThrowIfNull(obj);

            if (_oggetti.Count >= MAX_SLOT) throw new InventarioPienoException(MAX_SLOT);
            if (_oggetti.TryGetValue(obj.Nome, out var esistente))
            {
                // Chiave trovata: aggiorna la quantità, mantieni lo stesso oggetto
                _oggetti[obj.Nome] = (esistente.Oggetto, esistente.Quantita + quantita);
                Console.WriteLine($"Oggetto aggiunto quantità: {quantita}");
            }
            else
            {
                // Chiave assente: nuova entry con l'oggetto e la quantità iniziale
                _oggetti[obj.Nome] = (obj, quantita);
                Console.WriteLine($"Oggetto aggiunto quantità: {quantita}");
            }
        }


        // ─────────────────────────────────────────────────────────────
        // RIMUOVI
        // Scala la quantità o rimuove l'entry completamente.
        // Restituisce false se l'oggetto non esiste — nessuna eccezione.
        //
        // Se quantita richiesta >= quantita disponibile:
        //   Remove(nome) elimina l'intera entry dal Dictionary.
        // Altrimenti:
        //   Aggiorna la tupla con la quantità ridotta.
        // ─────────────────────────────────────────────────────────────
        public bool Rimuovi(string nome, int quantita = 1)
        {
            if (!_oggetti.TryGetValue(nome, out var esistente))
                return false;  // chiave non trovata: nessuna eccezione

            if (esistente.Quantita <= quantita)
                _oggetti.Remove(nome);          // rimuove l'entry completamente
            else
                _oggetti[nome] = (esistente.Oggetto, esistente.Quantita - quantita);

            return true;
        }


        // ─────────────────────────────────────────────────────────────
        // CERCA
        // TryGetValue O(1): nessun ciclo, nessun confronto uno per uno.
        // Trova() restituisce null se la chiave non esiste.
        // GetQuantita() restituisce 0 se la chiave non esiste.
        // ─────────────────────────────────────────────────────────────
        public IOggetto Trova(string nome)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(nome);
            if (!_oggetti.TryGetValue(nome, out var entry)) throw new OggettoNonTrovatoException(nome);
            return entry.Oggetto;
        }
        

        public int GetQuantita(string nome)
            => _oggetti.TryGetValue(nome, out var entry) ? entry.Quantita : 0;

        public bool Contiene(string nome) => _oggetti.ContainsKey(nome);


        // ─────────────────────────────────────────────────────────────
        // STATISTICHE — calcolate con foreach su _oggetti.Values
        // .Values restituisce la collezione di tutte le tuple del Dictionary.
        // Non produce una nuova lista: è una vista diretta sul contenuto.
        // ─────────────────────────────────────────────────────────────
        public int ValoreTotale()
        {
            int totale = 0;
            foreach (var entry in _oggetti.Values)
                totale += entry.Oggetto.Valore * entry.Quantita;
            return totale;
        }

        public List<IOggetto> FiltraTipo(string tipo)
        {
            var risultato = new List<IOggetto>();
            foreach (var entry in _oggetti.Values)
                if (entry.Oggetto.Tipo == tipo) risultato.Add(entry.Oggetto);
            return risultato;
        }


        // ─────────────────────────────────────────────────────────────
        // MOSTRA
        // ValoreTotale() viene chiamata UNA SOLA VOLTA, fuori dal foreach.
        // ─────────────────────────────────────────────────────────────
        public void Mostra()
        {
            if (_oggetti.Count == 0) { Console.WriteLine("Inventario vuoto."); return; }

            Console.WriteLine(" === INVENTARIO ===");
            foreach (var obj in _oggetti.Values)
                Console.WriteLine($" [{obj.Oggetto.Tipo}] {obj.Oggetto.Nome} | {obj.Oggetto.Valore} | {obj.Quantita} $");

            Console.WriteLine($" Totale: {ValoreTotale()} $");  // una sola volta
        }


        // USA OGGETTO (SINGOLO)
        public void UsaOggetto(string nome, Personaggio target)
        {
            ArgumentNullException.ThrowIfNull(target);
            IOggetto oggetto = Trova(nome);
            oggetto.Usa(target);
            Rimuovi(nome, 1);
        }





        // ─────────────────────────────────────────────────────────────
        // USA TUTTO
        // PROBLEMA: non si può rimuovere da un Dictionary mentre lo si
        // itera con foreach — lancerebbe InvalidOperationException.
        //
        // SOLUZIONE in due passaggi:
        //   1. Primo foreach: usa gli oggetti, raccoglie i nomi da rimuovere
        //   2. Secondo foreach: rimuove le entry dopo aver finito di iterare
        // ─────────────────────────────────────────────────────────────
        public void UsaTutto(string tipo, Personaggio bersaglio)
        {
            var daRimuovere = new List<string>();  // raccoglie i nomi, non gli oggetti

            foreach (var entry in _oggetti.Values)
            {
                if (entry.Oggetto.Tipo != tipo) continue;
                for (int i = 0; i < entry.Quantita; i++)
                    entry.Oggetto.Usa(bersaglio);
                daRimuovere.Add(entry.Oggetto.Nome);
            }

            foreach (var nome in daRimuovere)
                _oggetti.Remove(nome);  // sicuro: il primo foreach è finito
        }


        // ─────────────────────────────────────────────────────────────
        // ISALVABILE — serializzazione JSON-like
        // Costruisce manualmente la stringa con un foreach.
        // string.Join aggiunge "," tra ogni elemento senza virgola finale.
        // ─────────────────────────────────────────────────────────────
        public string Serializza()
        {
            if (_oggetti.Count == 0) return "[]";

            var parti = new List<string>();
            foreach (var entry in _oggetti.Values)
                parti.Add($"{{\"nome\":\"{entry.Oggetto.Nome}\"," +
                          $"\"tipo\":\"{entry.Oggetto.Tipo}\"," +
                          $"\"qty\":{entry.Quantita}}}");

            return "[" + string.Join(",", parti) + "]";
        }


        // ─────────────────────────────────────────────────────────────
        // IENUMERABLE — necessario per foreach e LINQ sull'inventario
        // "yield return" produce un elemento alla volta senza creare
        // una lista intermedia — più efficiente di ToList().GetEnumerator().
        //
        // Due metodi obbligatori:
        //   1. IEnumerator<IOggetto>              → versione generica (C# 2+)
        //   2. System.Collections.IEnumerator     → versione legacy (pre-generics)
        // ─────────────────────────────────────────────────────────────
        public IEnumerator<IOggetto> GetEnumerator()
        {
            foreach (var obj in _oggetti.Values)
                yield return obj.Oggetto;
        }

        System.Collections.IEnumerator
        System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
