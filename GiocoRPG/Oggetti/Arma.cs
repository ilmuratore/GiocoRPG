// ═══════════════════════════════════════════════════════════════════
// ARMA — Oggetti/Arma.cs
// ═══════════════════════════════════════════════════════════════════
// Implementa IOggetto: l'Arma è un oggetto equipaggiabile che
// aumenta l'attacco del personaggio quando viene usata.
//
// Come Pozione, non estende nessuna classe — implementa solo IOggetto.
// Lo stesso metodo Usa(Personaggio) del ciclo in Program.cs
// che chiama Pozione.Usa() può chiamare Arma.Usa() sullo stesso array:
//   foreach (IOggetto o in zaino) o.Usa(eroe);
// Il runtime sceglie la versione corretta — polimorfismo via interfaccia.
// ═══════════════════════════════════════════════════════════════════

namespace GiocoRPG.Oggetti
{
    public class Arma : IOggetto
    {
        public string Nome     { get; }
        public string Tipo     { get; } = "Arma";  // valore fisso per tutte le armi
        public int    Valore   { get; }
        public int    BonusAtk { get; }  // bonus ATK fornito quando equipaggiata

        public Arma(string nome, int bonusAtk, int valore = 100)
        {
            Nome     = nome;
            BonusAtk = bonusAtk;
            Valore   = valore;
        }

        // ─────────────────────────────────────────────────────────────
        // USA — implementazione di IOggetto.Usa()
        // In questa versione "usare" un'arma significa equipaggiarla:
        // si stampa il messaggio con il bonus ATK.
        //
        // Nota: questo non modifica effettivamente l'Attacco del personaggio
        // (Attacco ha private set in Personaggio). Un'implementazione
        // completa richiederebbe un metodo Equipaggia() sul personaggio.
        // ─────────────────────────────────────────────────────────────
        public void Usa(Personaggio bersaglio)
        {
            Console.WriteLine($" {bersaglio.Nome} equipaggia {Nome} (+{BonusAtk} ATK).");
        }
    }
}
