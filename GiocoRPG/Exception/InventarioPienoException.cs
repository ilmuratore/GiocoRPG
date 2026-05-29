// ═══════════════════════════════════════════════════════════════════
// INVENTARIO PIENO EXCEPTION — Exception/InventarioPienoException.cs
// ═══════════════════════════════════════════════════════════════════
// Eccezione personalizzata per inventario che ha raggiunto il limite.
//
// PERCHÉ InvalidOperationException?
//   Aggiungere un oggetto a un inventario pieno è un'operazione
//   tecnicamente valida (il metodo esiste, i parametri sono corretti),
//   ma lo stato corrente dell'inventario non lo permette.
//   InvalidOperationException descrive esattamente questo: "l'operazione
//   non è valida nello stato corrente dell'oggetto".
//
// PROPRIETÀ CapacitaMassima:
//   Chi cattura questa eccezione può mostrare all'utente il limite esatto
//   senza hard-codare il numero nel messaggio dell'interfaccia.
//
//   catch (InventarioPienoException ex)
//       Console.WriteLine($"Zaino pieno: massimo {ex.CapacitaMassima} oggetti.");
// ═══════════════════════════════════════════════════════════════════

namespace GiocoRPG.Exception
{
    public class InventarioPienoException : InvalidOperationException
    {
        // Il limite massimo al momento dell'eccezione.
        public int CapacitaMassima { get; }

        public InventarioPienoException(int capacita)
            : base($"Inventario pieno: massimo {capacita} oggetti.")
        {
            CapacitaMassima = capacita;
        }
    }
}
