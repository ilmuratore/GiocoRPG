// ═══════════════════════════════════════════════════════════════════
// SISTEMA SALVATAGGIO — Salvataggio/SistemaSalvataggio.cs
// ═══════════════════════════════════════════════════════════════════
// Gestisce il salvataggio su disco degli oggetti ISalvabile.
//
// DISACCOPPIAMENTO — il principio Open/Closed in azione:
//   Questa classe non conosce Guerriero, Mago o Inventario.
//   Conosce solo ISalvabile. Se aggiungiamo Chierico o ConfigPartita,
//   non modifichiamo SistemaSalvataggio — basta che implementino ISalvabile.
//
// CONFRONTO con l'approccio accoppiato (da NON fare):
//   public void Salva(Guerriero g) { ... }   // serve un metodo per tipo
//   public void Salva(Mago m)      { ... }   // se aggiungiamo Ladro → altro metodo
//
// CON interfaccia (approccio corretto):
//   public void Salva(ISalvabile o) { ... }  // funziona con qualsiasi ISalvabile
// ═══════════════════════════════════════════════════════════════════

namespace GiocoRPG.Salvataggio
{
    public class SistemaSalvataggio
    {
        // ─────────────────────────────────────────────────────────────
        // SALVA SINGOLO — scrive un oggetto su file
        // File.WriteAllText crea il file se non esiste, lo sovrascrive
        // se esiste già. Per salvataggi incrementali si userebbe
        // File.AppendAllText o uno StreamWriter.
        // ─────────────────────────────────────────────────────────────
        public static void Salva(ISalvabile oggetto, string percorso = "salvataggio.json")
        {
            string json = oggetto.Serializza();
            Console.WriteLine($" Salvataggio: {json}");
            File.WriteAllText(percorso, json);
        }

        // ─────────────────────────────────────────────────────────────
        // SALVA TUTTI — serializza una lista di oggetti come array JSON
        // Riceve IEnumerable<ISalvabile>: funziona con List<ISalvabile>,
        // array, o qualsiasi collezione che implementa IEnumerable.
        //
        // Select proietta ogni ISalvabile nella sua stringa serializzata.
        // string.Join unisce le stringhe con "," come separatore.
        // Il risultato è un array JSON: [{"tipo":"Guerriero",...}, {...}]
        // ─────────────────────────────────────────────────────────────
        public static void SalvaTutti(IEnumerable<ISalvabile> oggetti, string percorso = "salvataggio.json")
        {
            var serializzati = oggetti.Select(obj => obj.Serializza()).ToList();
            string json = "[" + string.Join(",", serializzati) + "]";
            Console.WriteLine($" Salvataggio multiplo: {json}");
            File.WriteAllText(percorso, json);
        }
    }
}
