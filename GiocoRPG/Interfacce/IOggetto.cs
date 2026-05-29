// ═══════════════════════════════════════════════════════════════════
// IOGGETTO — Interfacce/IOggetto.cs
// ═══════════════════════════════════════════════════════════════════
// Contratto per qualsiasi oggetto dell'inventario.
// Pozione, Arma — tipi completamente diversi — condividono questo
// contratto: hanno un nome, un tipo, un valore e si possono usare.
//
// Il metodo Usa() riceve Personaggio (tipo base della gerarchia):
// funziona con qualsiasi personaggio senza downcast.
//
// POLIMORFISMO VIA INTERFACCIA:
//   IOggetto[] zaino = { new Pozione("Grande", 60), new Arma("Spada", 20) };
//   foreach (IOggetto o in zaino)
//       o.Usa(eroe);  // ← dispatch: Pozione.Usa o Arma.Usa a runtime
//
// Questo è lo stesso principio di CalcolaDanno() — stessa riga di
// codice, comportamenti diversi in base al tipo concreto dell'oggetto.
// ═══════════════════════════════════════════════════════════════════

namespace GiocoRPG.Interfacce
{
    public interface IOggetto
    {
        string Nome   { get; }  // nome dell'oggetto
        string Tipo   { get; }  // categoria: "Arma", "Consumabile", ecc.
        int    Valore { get; }  // valore in monete

        // Usa l'oggetto sul bersaglio. L'effetto dipende dal tipo concreto:
        //   Pozione → cura il bersaglio
        //   Arma    → stampa il bonus ATK equipaggiato
        void Usa(Personaggio bersaglio);
    }
}
