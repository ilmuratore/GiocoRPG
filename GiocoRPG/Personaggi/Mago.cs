// ═══════════════════════════════════════════════════════════════════
// MAGO — Personaggi/Mago.cs
// ═══════════════════════════════════════════════════════════════════
// Mago è una classe concreta che estende Personaggio.
//
// Profilo: vita bassa, alta Magia, pool di Mana ampio.
//          Danno = Magia + dado (se ha mana) oppure Attacco (fallback).
//
// Specialità: tre incantesimi con TipoDanno diversi.
//   - LanciaIncantesimo         → TipoDanno.Magico   (Scheletro immune)
//   - LanciaIncantesimoFuoco    → TipoDanno.Fuoco    (Goblin immune)
//   - LanciaIncantesimoGhiaccio → TipoDanno.Ghiaccio (Orco immune)
// Ognuno sfrutta il sistema di immunità dei nemici.
//
// GIORNO 5 — aggiunge ISalvabile:
//   Serializza() produce una stringa JSON-like con lo stato corrente.
// ═══════════════════════════════════════════════════════════════════

namespace GiocoRPG.Personaggi
{
    public class Mago : Personaggio, ISalvabile
    {
        public Mago(string nome)
        : base(
            nome,
            vitaMax:      80,
            attacco:      12,
            difesa:        6,
            forza:         6,
            agilita:      10,
            intelligenza: 25,
            mana:        100,   // pool di mana pieno all'inizio
            manaMax:     100,
            Magia:        20)
        {
        }


        // ─────────────────────────────────────────────────────────────
        // CALCOLADANNO — override con side effect sul mana
        // Ogni chiamata a CalcolaDanno() consuma 20 mana se disponibile.
        // Quando il mana finisce, il Mago torna a un attacco fisico debole.
        //
        // TRADE-OFF di design:
        //   PRO  — SistemaTurni chiama p.CalcolaDanno() uniformemente
        //          su tutti i personaggi, senza sapere chi è il Mago.
        //   CONTRO — Il nome "CalcolaDanno" non suggerisce un side effect.
        //            Chiamarlo due volte di fila consuma mana due volte.
        // ─────────────────────────────────────────────────────────────
        public override int CalcolaDanno()
        {
            const int COSTO_MANA = 20;
            if (Mana < COSTO_MANA)
            {
                Console.WriteLine($"{Nome} è a corto di mana! Usa attacco fisico.");
                return Attacco;  // fallback: attacco fisico debole
            }
            Mana -= COSTO_MANA;  // Mana è "protected set" in Personaggio
            return Magia + Random.Shared.Next(1, 10);
        }


        // ─────────────────────────────────────────────────────────────
        // INCANTESIMI — metodi specifici del Mago
        // Ogni incantesimo chiama CalcolaDanno() per il danno base
        // e poi SubisciDanno() con il TipoDanno appropriato.
        // Il tipo di danno determina quali nemici ne sono immuni.
        // ─────────────────────────────────────────────────────────────

        // Danno magico generico — Scheletro è immune
        public void LanciaIncantesimo(Personaggio bersaglio)
        {
            int dannoMagico = CalcolaDanno();
            Console.WriteLine($" {Nome} lancia un incantesimo ({dannoMagico} danni | Mana: {Mana}/{ManaMax})");
            bersaglio.SubisciDanno(dannoMagico, TipoDanno.Magico);
        }

        // Danno fuoco — Goblin è immune
        public void LanciaIncantesimoFuoco(Personaggio bersaglio)
        {
            int dannoMagico = CalcolaDanno();
            Console.WriteLine($" {Nome} lancia palla di fuoco ({dannoMagico} danni | Mana: {Mana}/{ManaMax})");
            bersaglio.SubisciDanno(dannoMagico, TipoDanno.Fuoco);
        }

        // Danno ghiaccio — Orco è immune
        public void LanciaIncantesimoGhiaccio(Personaggio bersaglio)
        {
            int dannoMagico = CalcolaDanno();
            Console.WriteLine($" {Nome} lancia polvere di diamanti ({dannoMagico} danni | Mana: {Mana}/{ManaMax})");
            bersaglio.SubisciDanno(dannoMagico, TipoDanno.Ghiaccio);
        }


        // ─────────────────────────────────────────────────────────────
        // ISALVABILE — implementazione del contratto
        // Include mana e manaMax oltre alle stat base: sono specifici
        // del Mago e necessari per ripristinare lo stato correttamente.
        // Nota: "string" (alias C#) invece di "String" (tipo .NET) —
        // in C# si preferisce sempre l'alias minuscolo per i tipi primitivi.
        // ─────────────────────────────────────────────────────────────
        public string Serializza() =>
            $"{{\"tipo\":\"Mago\",\"nome\":\"{Nome}\"," +
            $"\"vita\":{Vita},\"vitaMax\":{VitaMax}," +
            $"\"livello\":{Livello},\"mana\":{Mana},\"manaMax\":{ManaMax}}}";
    }
}
