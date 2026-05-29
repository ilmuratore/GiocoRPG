// ═══════════════════════════════════════════════════════════════════
// IPREMIABILE — Interfacce/IPremiabile.cs
// ═══════════════════════════════════════════════════════════════════
// Contratto per entità che danno ricompense quando sconfitte.
// Attualmente usata da Nemico, ma potrebbe essere usata anche da
// forzieri, trappole, puzzle — qualsiasi cosa dia XP e drop.
//
// Il metodo AssegnaRicompense() in SistemaTurni riceve IPremiabile,
// non Nemico: non sa (e non deve sapere) il tipo concreto.
// Se domani aggiungessimo "BossEvento", non modifichiamo SistemaTurni —
// basta che BossEvento implementi IPremiabile.
// ═══════════════════════════════════════════════════════════════════

namespace GiocoRPG.Interfacce
{
    public interface IPremiabile
    {
        int     XpReward { get; }   // punti esperienza assegnati alla sconfitta
        string? DropItem { get; }   // oggetto droppato — null se nessun drop
    }
}
