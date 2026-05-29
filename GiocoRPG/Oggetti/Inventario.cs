// ═══════════════════════════════════════════════════════════════════
// INVENTARIO — Oggetti/Inventario.cs
// ═══════════════════════════════════════════════════════════════════
// GIORNO 5: usava List<IOggetto>.
// GIORNO 6: refactoring a Dictionary<string, (IOggetto, int)>.
// GIORNO 7: aggiunta validazione con eccezioni personalizzate.
//
// Vantaggi del Dictionary rispetto alla List:
//   - Ricerca per nome O(1) invece di O(n)
//   - Quantità integrata nel valore — niente campi paralleli
//   - TryGetValue sicuro: nessuna eccezione per chiavi mancanti
//   - Aggiungere lo stesso oggetto due volte somma la quantità
//
// La chiave è il Nome dell'oggetto (string).
// Il valore è una tupla nominata: l'oggetto + la sua quantità.
// ═══════════════════════════════════════════════════════════════════

namespace GiocoRPG.Oggetti
{
    public class Inventario : IEnumerable<IOggetto>, ISalvabile
    {
        // Dictionary: chiave = Nome oggetto, valore = (oggetto, quantità).
        private readonly Dictionary<string, (IOggetto Oggetto, int Quantita)> _oggetti = new();

        // Limite massimo di slot. Aggiungi() lancia InventarioPienoException
        // se viene superato.
        private const int MAX_SLOT = 20;

        public int Count => _oggetti.Count;


        // ─────────────────────────────────────────────────────────────
        // AGGIUNGI
        // Guard clause 1: ArgumentNullException.ThrowIfNull — C# 10+.
        //   Equivale a: if (obj == null) throw new ArgumentNullException(nameof(obj));
        //
        // Guard clause 2: InventarioPienoException se si supera MAX_SLOT.
        //
        // TryGetValue O(1): cerca la chiave senza scorrere tutto il Dictionary.
        //   Se trovata: aggiorna la quantità mantenendo lo stesso oggetto.
        //   Se non trovata: crea una nuova entry.
        // ─────────────────────────────────────────────────────────────
        public void Aggiungi(IOggetto? obj, int quantita = 1)
        {
            ArgumentNullException.ThrowIfNull(obj);

            if (_oggetti.Count >= MAX_SLOT)
                throw new InventarioPienoException(MAX_SLOT);

            if (_oggetti.TryGetValue(obj.Nome, out var esistente))
            {
                _oggetti[obj.Nome] = (esistente.Oggetto, esistente.Quantita + quantita);
                Console.WriteLine($"Oggetto aggiunto quantità: {quantita}");
            }
            else
            {
                _oggetti[obj.Nome] = (obj, quantita);
                Console.WriteLine($"Oggetto aggiunto quantità: {quantita}");
            }
        }


        // ─────────────────────────────────────────────────────────────
        // RIMUOVI
        // Scala la quantità o rimuove l'entry completamente.
        // Restituisce false se l'oggetto non esiste — nessuna eccezione.
        // ─────────────────────────────────────────────────────────────
        public bool Rimuovi(string nome, int quantita = 1)
        {
            if (!_oggetti.TryGetValue(nome, out var esistente))
                return false;

            if (esistente.Quantita <= quantita)
                _oggetti.Remove(nome);
            else
                _oggetti[nome] = (esistente.Oggetto, esistente.Quantita - quantita);

            return true;
        }


        // ─────────────────────────────────────────────────────────────
        // TROVA — cerca per nome, lancia se non trovato
        // A differenza di TryGetValue (che restituisce false se assente),
        // questo metodo lancia OggettoNonTrovatoException.
        // Scelta intenzionale: chi chiede un oggetto specifico
        // si aspetta che ci sia — la sua assenza è un errore.
        //
        // ArgumentException.ThrowIfNullOrWhiteSpace — C# 10+.
        //   Equivale a: if (string.IsNullOrWhiteSpace(nome)) throw ...
        // ─────────────────────────────────────────────────────────────
        public IOggetto Trova(string nome)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(nome);
            if (!_oggetti.TryGetValue(nome, out var entry))
                throw new OggettoNonTrovatoException(nome);
            return entry.Oggetto;
        }

        public int GetQuantita(string nome)
            => _oggetti.TryGetValue(nome, out var entry) ? entry.Quantita : 0;

        public bool Contiene(string nome) => _oggetti.ContainsKey(nome);


        // ─────────────────────────────────────────────────────────────
        // STATISTICHE — calcolate con foreach su _oggetti.Values
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
        // ─────────────────────────────────────────────────────────────
        public void Mostra()
        {
            if (_oggetti.Count == 0) { Console.WriteLine("Inventario vuoto."); return; }

            Console.WriteLine(" === INVENTARIO ===");
            foreach (var obj in _oggetti.Values)
                Console.WriteLine($" [{obj.Oggetto.Tipo}] {obj.Oggetto.Nome} | {obj.Oggetto.Valore} | x{obj.Quantita}");

            Console.WriteLine($" Totale: {ValoreTotale()} $");
        }


        // ─────────────────────────────────────────────────────────────
        // USA OGGETTO — usa un singolo oggetto e lo rimuove
        // Guard clause: target non può essere null.
        // Trova() lancia OggettoNonTrovatoException se l'oggetto non c'è —
        // l'eccezione si propaga al chiamante senza catch qui.
        // ─────────────────────────────────────────────────────────────
        public void UsaOggetto(string nome, Personaggio target)
        {
            ArgumentNullException.ThrowIfNull(target);
            IOggetto oggetto = Trova(nome);   // lancia OggettoNonTrovatoException se assente
            oggetto.Usa(target);
            Rimuovi(nome, 1);
        }


        // ─────────────────────────────────────────────────────────────
        // USA TUTTO
        // Non si può rimuovere da un Dictionary mentre lo si itera.
        // Soluzione: raccogliamo prima i nomi da rimuovere, poi rimuoviamo.
        // ─────────────────────────────────────────────────────────────
        public void UsaTutto(string tipo, Personaggio bersaglio)
        {
            var daRimuovere = new List<string>();

            foreach (var entry in _oggetti.Values)
            {
                if (entry.Oggetto.Tipo != tipo) continue;
                for (int i = 0; i < entry.Quantita; i++)
                    entry.Oggetto.Usa(bersaglio);
                daRimuovere.Add(entry.Oggetto.Nome);
            }

            foreach (var nome in daRimuovere)
                _oggetti.Remove(nome);
        }


        // ─────────────────────────────────────────────────────────────
        // ISALVABILE
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
        // una lista intermedia.
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
