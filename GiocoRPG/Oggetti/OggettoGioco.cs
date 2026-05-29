// ═══════════════════════════════════════════════════════════════════
// OGGETTO GIOCO — Oggetti/OggettoGioco.cs
// ═══════════════════════════════════════════════════════════════════
// Rappresenta un oggetto raccoglibile nell'inventario.
//
// A differenza di Personaggio, OggettoGioco è una classe concreta
// normale (non abstract, non fa parte di una gerarchia di ereditarietà).
// Ogni proprietà ha "public set" perché gli oggetti sono dati mutabili
// che possono cambiare stato (es. un oggetto usato, venduto, migliorato).
//
// Mostra un uso diverso del costruttore rispetto a Personaggio:
//   - Costruttore completo (5 parametri)
//   - Costruttore semplificato (2 parametri) via "this(...)"
//   - Costruttore vuoto per l'object initializer
// ═══════════════════════════════════════════════════════════════════

namespace GiocoRPG.Oggetti
{
    public class OggettoGioco
    {
        // "public set" necessario per usare l'object initializer { Nome = ..., Valore = ... }.
        // Con init al posto di set rendo piu robusto l'istanza di un Oggetto rendendo modificabile gli attributi solo
        // con il costruttore e quindi al suo mount di istanza, con required inoltre introduco anche il concetto 
        // di parametro obbligatorio richiesto altrimenti l'object initializer restituisce errore
        public required string Nome { get; init; }
        public int Valore { get; init; } = 0;
        public float  Peso    { get; set; }
        public bool   IsRaro  { get; set; }
        public required string Tipo    { get; init; }

        // ─────────────────────────────────────────────────────────────
        // COSTRUTTORI CON CONCATENAZIONE
        // ─────────────────────────────────────────────────────────────

        // Costruttore completo: riceve tutti e cinque i parametri.
        public OggettoGioco(string nome, int valore, float peso, bool isRaro, string tipo)
        {
            Nome    = nome;
            Valore  = valore;
            Peso    = peso;
            IsRaro  = isRaro;
            Tipo    = tipo;
        }

        // Costruttore semplificato: solo nome e valore.
        // ": this(...)" delega al costruttore completo — stesso principio
        // di base() verso il padre, ma tra costruttori della STESSA classe.
        // Evita di duplicare la logica di assegnazione.
        public OggettoGioco(string nome, int valore)
            : this(nome, valore, 1.0f, false, "Generico") { }

        // Costruttore senza parametri: necessario per l'object initializer.
        // Usato in GeneraOggetti() con la sintassi:
        //   new OggettoGioco { Nome = "Spada", Valore = 100, ... }
        public OggettoGioco() { }


        // ─────────────────────────────────────────────────────────────
        // GENERAOGGETTI — factory statico
        // Crea un array di oggetti casuali. Le proprietà sono array
        // readonly privati: i valori possibili sono fissi a compile-time.
        //
        // Confronto con il vecchio approccio procedurale (Giorno 1):
        //   PRIMA: string[] nomi; int[] valori; float[] pesi; — array paralleli
        //   DOPO:  OggettoGioco[] — tutti i dati di un oggetto sono nello
        //          stesso posto. Nessun rischio di disallineamento degli indici.
        // ─────────────────────────────────────────────────────────────
        private static readonly string[] NomiPossibili =
            { "Spada", "Scudo", "Pozione", "Amuleto", "Arco", "Stivali", "Elmo", "Anello" };

        private static readonly string[] TipiPossibili =
            { "Arma", "Armatura", "Consumabile", "Accessorio" };

        public static OggettoGioco[] GeneraOggetti(int quantita)
        {
            OggettoGioco[] oggetti = new OggettoGioco[quantita];

            for (int i = 0; i < quantita; i++)
            {
                // Object initializer: crea l'oggetto con il costruttore vuoto
                // e assegna le proprietà in un blocco { }.
                // Leggibile e comodo quando le proprietà sono molte.
                oggetti[i] = new OggettoGioco
                {
                    Nome   = NomiPossibili[Random.Shared.Next(NomiPossibili.Length)],
                    Valore = Random.Shared.Next(10, 501),
                    Peso   = Random.Shared.Next(1, 51) / 10f,
                    IsRaro = Random.Shared.Next(1, 11) == 1,  // 10% di probabilità
                    Tipo   = TipiPossibili[Random.Shared.Next(TipiPossibili.Length)]
                };
            }

            return oggetti;
        }

        public override string ToString()
        {
            string tag = IsRaro ? " [RARO]" : "";
            return $"{tag} {Nome} | Tipo: {Tipo} | Valore: {Valore} | Peso: {Peso:F1}kg";
        }
    }
}
