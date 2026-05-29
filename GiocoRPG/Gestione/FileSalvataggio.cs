// ═══════════════════════════════════════════════════════════════════
// FILE SALVATAGGIO — Gestione/FileSalvataggio.cs
// ═══════════════════════════════════════════════════════════════════
// Classe che gestisce la scrittura su file di log/salvataggio.
//
// PERCHÉ IDisposable?
//   Il file sul disco è una risorsa del sistema operativo — non è
//   memoria gestita dal Garbage Collector. Se non lo chiudiamo
//   esplicitamente, rimane aperto e altri processi non possono
//   accedervi. I dati nel buffer potrebbero non essere scritti su disco.
//
//   Implementando IDisposable, possiamo usare il pattern "using"
//   che garantisce la chiusura automatica al termine del blocco,
//   anche in caso di eccezione.
//
// PATTERN using:
//   using (var log = new FileSalvataggio("file.log"))
//   {
//       log.Scrivi("messaggio");
//   }   // ← Dispose() chiamato automaticamente qui
//
//   Oppure con la sintassi C# 8+:
//   using var log = new FileSalvataggio("file.log");
//   log.Scrivi("messaggio");
//   // Dispose() alla fine del metodo che contiene questa riga
// ═══════════════════════════════════════════════════════════════════

namespace GiocoRPG.Gestione
{
    public class FileSalvataggio : IDisposable
    {
        // StreamWriter: gestisce la scrittura su file di testo.
        // "?" perché potrebbe essere null se il costruttore fallisce
        // (es. permessi negati sul percorso).
        private StreamWriter? _writer;

        // Flag per evitare di chiamare Dispose() due volte.
        // La seconda chiamata non deve fare nulla — non deve lanciare.
        private bool _disposed = false;

        // Apre il file in modalità "append: true": non sovrascrive il contenuto
        // esistente, aggiunge le nuove righe in fondo.
        public FileSalvataggio(string percorso)
        {
            _writer = new StreamWriter(percorso, append: true);
            Console.WriteLine($" [LOG] File aperto: {percorso}");
        }

        // ─────────────────────────────────────────────────────────────
        // SCRIVI — aggiunge una riga con timestamp al file
        // ObjectDisposedException.ThrowIf: lancia se Dispose() è già stato
        // chiamato. Impedisce di scrivere su un file già chiuso.
        //
        // _writer! — il "!" è il null-forgiving operator: diciamo al
        // compilatore "so che _writer non è null qui" perché controlliamo
        // _disposed prima. Se _disposed è false, _writer non è null.
        // ─────────────────────────────────────────────────────────────
        public void Scrivi(string linea)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            _writer!.WriteLine($"[{DateTime.Now:HH:mm:ss}] {linea}");
        }

        // ─────────────────────────────────────────────────────────────
        // DISPOSE — chiude il file e libera le risorse
        // Chiamato automaticamente da "using" o manualmente.
        //
        // _writer?.Flush() — il "?" è null-conditional: Flush solo se
        //   _writer non è null (potrebbe esserlo se il costruttore ha fallito).
        //   Flush() svuota il buffer in memoria e scrive tutto sul disco.
        //   Senza Flush(), le ultime righe scritte potrebbero andare perse.
        //
        // _writer?.Dispose() — chiude il file e rilascia l'handle di sistema.
        // ─────────────────────────────────────────────────────────────
        public void Dispose()
        {
            if (!_disposed)
            {
                _writer?.Flush();
                _writer?.Dispose();
                _disposed = true;
                Console.WriteLine("[LOG] File chiuso");
            }
        }
    }
}
