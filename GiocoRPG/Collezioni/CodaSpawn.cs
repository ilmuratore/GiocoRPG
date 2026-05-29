// ═══════════════════════════════════════════════════════════════════
// CODA SPAWN — Collezioni/CodaSpawn.cs
// ═══════════════════════════════════════════════════════════════════
// Gestisce l'ordine in cui i nemici entrano in scena usando una Queue.
//
// QUEUE<T> — First In First Out (FIFO):
//   Il primo nemico accodato è il primo a essere spawnato.
//   Enqueue() aggiunge in fondo — O(1).
//   Dequeue() rimuove e restituisce il primo — O(1).
//   Non si sposta nessun elemento: si spostano solo due puntatori
//   interni (testa e coda). È più efficiente di List.RemoveAt(0),
//   che è O(n) perché deve spostare tutti gli altri elementi.
//
// LAZY CREATION — Func<Nemico> invece di Nemico:
//   Salviamo la "ricetta" per creare il nemico, non l'istanza.
//   Il nemico viene creato solo quando viene effettivamente spawnato.
//   Vantaggio: nessuna memoria sprecata per nemici che non arriveranno mai
//   (es. il giocatore vince prima di raggiungere un'ondata).
// ═══════════════════════════════════════════════════════════════════

namespace GiocoRPG.Collezioni
{
    public class CodaSpawn
    {
        // Queue<Func<Nemico>>: una coda di "ricette" per creare nemici.
        // Func<Nemico> è un delegato che non prende parametri e restituisce un Nemico.
        // Esempio di cosa viene accodato: Nemico.CreaGoblin (senza parentesi = metodo, non chiamata).
        private readonly Queue<Func<Nemico>> _fabbriche = new();

        // Proprietà calcolate: leggono direttamente la dimensione della Queue.
        // Non servono campi separati — la Queue tiene già il conteggio.
        public int  Count    => _fabbriche.Count;
        public bool HaNemici => _fabbriche.Count > 0;


        // ─────────────────────────────────────────────────────────────
        // ACCODA — aggiunge una o più "ricette" in fondo alla coda
        // "params" permette di passare più argomenti separati da virgola
        // senza dover creare un array esplicitamente:
        //   coda.Accoda(Nemico.CreaGoblin, Nemico.CreaOrco);
        // Il foreach chiama Enqueue per ognuna — O(1) per elemento.
        // ─────────────────────────────────────────────────────────────
        public void Accoda(params Func<Nemico>[] fabbriche)
        {
            foreach (var f in fabbriche)
            {
                _fabbriche.Enqueue(f);               // aggiunge in fondo alla coda
                Console.WriteLine("Nemico aggiunto alla coda");
            }
        }


        // ─────────────────────────────────────────────────────────────
        // PROSSIMO NEMICO — preleva e crea il prossimo della coda
        // Dequeue() rimuove e restituisce il primo elemento (FIFO).
        // Se la coda è vuota, restituisce null invece di lanciare eccezione —
        // il chiamante può controllare con HaNemici prima di chiamare.
        //
        // "fabbrica()" — le parentesi DOPO la variabile invocano il delegato:
        //   Func<Nemico> fabbrica = Nemico.CreaGoblin;
        //   Nemico nemico = fabbrica();   // equivale a Nemico.CreaGoblin()
        // ─────────────────────────────────────────────────────────────
        public Nemico? ProssimoNemico()
        {
            if (!HaNemici) return null;

            Func<Nemico> fabbrica = _fabbriche.Dequeue();  // rimuove il primo dalla coda
            Nemico nemico = fabbrica();                     // crea il nemico solo adesso
            Console.WriteLine("Prossimo nemico in arrivo");
            return nemico;
        }


        // ─────────────────────────────────────────────────────────────
        // STAMPA CODA — mostra quanti nemici sono ancora in attesa
        // Non modifica la coda — solo lettura di Count.
        // ─────────────────────────────────────────────────────────────
        public void StampaCoda()
        {
            Console.WriteLine($"La coda: {_fabbriche.Count} Nemici.");
        }
    }
}
