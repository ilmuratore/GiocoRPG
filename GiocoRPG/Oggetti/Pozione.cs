// ═══════════════════════════════════════════════════════════════════
// POZIONE — Oggetti/Pozione.cs
// ═══════════════════════════════════════════════════════════════════
// Implementa IOggetto: la Pozione è un oggetto consumabile che cura
// il personaggio bersaglio quando viene usata.
//
// Non estende nessuna classe — implementa solo l'interfaccia.
// Questo dimostra che IOggetto è trasversale alla gerarchia:
// Pozione non è un Personaggio, non è un OggettoGioco,
// è semplicemente qualcosa che rispetta il contratto IOggetto.
// ═══════════════════════════════════════════════════════════════════

namespace GiocoRPG.Oggetti
{
    public class Pozione : IOggetto
    {
        public string Nome  { get; }
        public string Tipo  { get; } = "Consumabile";  // valore fisso: non cambia mai
        public int    Valore { get; }

        // "_cura" è privato e readonly: il campo non deve essere accessibile
        // dall'esterno né modificabile dopo la costruzione.
        // "readonly" è un vincolo più forte di "private set": neanche
        // la classe stessa può cambiarlo dopo il costruttore.
        private readonly int _cura;

        public Pozione(string nome, int cura, int valore = 50)
        {
            Nome   = nome;
            _cura  = cura;
            Valore = valore;
        }

        // ─────────────────────────────────────────────────────────────
        // USA — implementazione di IOggetto.Usa()
        // Riceve Personaggio (tipo base): funziona con Guerriero, Mago, Ladro.
        // Chiama Cura() — metodo garantito da ICurabile, implementato in Personaggio.
        //
        // Il recupero effettivo può essere minore di _cura se il personaggio
        // era quasi al massimo: il setter di Vita clampa automaticamente.
        // Per questo calcoliamo la differenza prima/dopo invece di stampare _cura.
        // ─────────────────────────────────────────────────────────────
        public void Usa(Personaggio bersaglio)
        {
            int prima = bersaglio.Vita;
            bersaglio.Cura(_cura);
            int recuperato = bersaglio.Vita - prima;
            Console.WriteLine($" {Nome}: {bersaglio.Nome} recupera {recuperato} Punti vita.");
        }
    }
}
