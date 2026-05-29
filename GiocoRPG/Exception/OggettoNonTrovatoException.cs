// ═══════════════════════════════════════════════════════════════════
// OGGETTO NON TROVATO EXCEPTION — Exception/OggettoNonTrovatoException.cs
// ═══════════════════════════════════════════════════════════════════
// Eccezione personalizzata per ricerca di oggetto assente nell'inventario.
//
// PERCHÉ KeyNotFoundException?
//   Cercare un oggetto per nome nell'inventario è semanticamente
//   identico a cercare una chiave in un Dictionary.
//   KeyNotFoundException è già usata dal framework per questo caso —
//   ereditare da essa comunica immediatamente l'intenzione.
//
// PROPRIETÀ NomeOggetto:
//   Il catch può usare il nome per un messaggio preciso:
//
//   catch (OggettoNonTrovatoException ex)
//       Console.WriteLine($"Non hai '{ex.NomeOggetto}' nello zaino.");
//
// NOTA: Trova() in Inventario usa questa eccezione e NON restituisce null.
//   La scelta di lanciare invece di restituire null è intenzionale:
//   se il chiamante chiede un oggetto specifico e non c'è, è un errore.
//   Restituire null sposterebbe la responsabilità del controllo al chiamante.
// ═══════════════════════════════════════════════════════════════════

namespace GiocoRPG.Exception
{
    public class OggettoNonTrovatoException : KeyNotFoundException
    {
        // Il nome dell'oggetto cercato — utile per il messaggio all'utente.
        public string NomeOggetto { get; }

        public OggettoNonTrovatoException(string nome)
            : base($"Oggetto '{nome}' non trovato nell'inventario")
        {
            NomeOggetto = nome;
        }
    }
}
