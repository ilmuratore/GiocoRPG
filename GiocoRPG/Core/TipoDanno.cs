// ═══════════════════════════════════════════════════════════════════
// TIPO DANNO — Core/TipoDanno.cs
// ═══════════════════════════════════════════════════════════════════
// Un enum è un tipo che può assumere solo un insieme fisso di valori
// con nome. Usarlo invece di stringhe o interi garantisce che il
// compilatore rilevi errori di battitura e valori non validi.
//
// Esempio: SubisciDanno(20, TipoDanno.Veleno)
//   è più sicuro e leggibile di SubisciDanno(20, "veleno")
//   o SubisciDanno(20, 3).
//
// Questo enum viene usato come parametro opzionale in SubisciDanno()
// e come discriminante nelle override delle classi Nemico.
// ═══════════════════════════════════════════════════════════════════

namespace GiocoRPG.Core
{
    public enum TipoDanno
    {
        Fisico,   // attacco standard, nessuna resistenza speciale
        Magico,   // incantesimi generici — Scheletro è immune
        Veleno,   // danno nel tempo (non ancora implementato)
        Fuoco,    // elemento fuoco — Goblin è immune
        Ghiaccio, // elemento ghiaccio — Orco è immune
        Puro      // danno che ignora le difese elementali — Drago è immune
    }
}
