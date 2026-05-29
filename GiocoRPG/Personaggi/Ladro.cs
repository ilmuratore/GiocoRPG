// ═══════════════════════════════════════════════════════════════════
// LADRO — Personaggi/Ladro.cs
// ═══════════════════════════════════════════════════════════════════
// Ladro è una classe concreta che estende Personaggio.
//
// Profilo: vita media, attacco medio, alta Agilita.
//          La sua specialità è il colpo critico: 40% di probabilità
//          di infliggere il triplo del danno base.
//
// A differenza del Guerriero, il Ladro non accumula stat in danno —
// fa affidamento sulla fortuna. Alta varianza = alto rischio/rendimento.
// ═══════════════════════════════════════════════════════════════════

namespace GiocoRPG.Personaggi
{
    public class Ladro : Personaggio
    {
        public Ladro(string nome)
        : base(
            nome,
            vitaMax:     100,
            attacco:      18,
            difesa:       10,
            forza:        12,
            agilita:      40,   // 40% probabilità critico
            intelligenza: 14,
            mana:         30,
            manaMax:      30,
            Magia:         8)
        {
        }


        // ─────────────────────────────────────────────────────────────
        // CALCOLADANNO — override con critico probabilistico
        // Random.Shared.Next(1, 101) genera un numero tra 1 e 100.
        // Se il numero è <= Agilita (40), scatta il critico.
        //
        // Con Agilita = 40, il Ladro ha:
        //   - 40% probabilità di fare 18 * 3 = 54 danni
        //   - 60% probabilità di fare 18 danni
        // Danno medio atteso: 0.4 * 54 + 0.6 * 18 = 21.6 + 10.8 = 32.4
        // ─────────────────────────────────────────────────────────────
        public override int CalcolaDanno()
        {
            bool critico = Random.Shared.Next(1, 101) <= Agilita;
            if (critico)
            {
                Console.WriteLine($" {Nome} colpisce nell'ombra — CRITICO!");
            }
            else
            {
                Console.WriteLine($" {Nome} colpisce furtivamente.");
            }
            return critico ? Attacco * 3 : Attacco;
        }

        // Metodo specifico del Ladro: attacco furtivo che chiama CalcolaDanno().
        // Qui non specifichiamo il tipo di danno → Fisico per default.
        public void AttaccaFurtivo(Personaggio bersaglio)
        {
            int danno = CalcolaDanno();
            bersaglio.SubisciDanno(danno);
        }
    }
}
