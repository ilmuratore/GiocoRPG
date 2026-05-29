// ═══════════════════════════════════════════════════════════════════
// VITA INVALIDA EXCEPTION — Exception/VitaInvalidaException.cs
// ═══════════════════════════════════════════════════════════════════
// Eccezione personalizzata per valori di vita non validi.
//
// PERCHÉ PERSONALIZZATA?
//   ArgumentOutOfRangeException esiste già, ma non porta informazioni
//   sul dominio. VitaInvalidaException porta ValoreRicevuto e VitaMax:
//   chi cattura questa eccezione sa esattamente cosa è andato storto
//   senza leggere il testo del messaggio.
//
// GERARCHIA:
//   VitaInvalidaException → ArgumentOutOfRangeException → ArgumentException → Exception
//   Un catch (ArgumentException) cattura anche questa — IS-A funziona
//   per le eccezioni esattamente come per le classi normali.
//
// CONVENZIONE:
//   Tutti i nomi di eccezioni terminano con "Exception".
//   Si eredita dal tipo più pertinente, non sempre da Exception direttamente.
// ═══════════════════════════════════════════════════════════════════

namespace GiocoRPG.Exception
{
    public class VitaInvalidaException : ArgumentOutOfRangeException
    {
        // Proprietà custom: il catch può usarle senza fare parsing del messaggio.
        public int ValoreRicevuto { get; }
        public int VitaMax        { get; }

        // base() richiama il costruttore di ArgumentOutOfRangeException
        // con tre argomenti: nome parametro, valore ricevuto, messaggio leggibile.
        public VitaInvalidaException(int valore, int vitaMax)
            : base(
                paramName:   "vita",
                actualValue: valore,
                message:     $"La vita deve essere tra 0 e {vitaMax}. Valore ricevuto: {valore}")
        {
            ValoreRicevuto = valore;
            VitaMax        = vitaMax;
        }
    }
}
