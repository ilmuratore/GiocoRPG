// ═══════════════════════════════════════════════════════════════════
// ISALVABILE — Interfacce/ISalvabile.cs
// ═══════════════════════════════════════════════════════════════════
// Contratto per qualsiasi oggetto che può essere serializzato
// e salvato su disco. Guerriero, Mago... basta implementare
// Serializza() e SistemaSalvataggio.Salva() fa il resto.
//
// DISACCOPPIAMENTO:
//   SistemaSalvataggio.Salva(ISalvabile oggetto) non conosce Guerriero,
//   Mago o Inventario. Se aggiungiamo Chierico, non modifichiamo
//   SistemaSalvataggio — basta che Chierico implementi ISalvabile.
//
// Un'interfaccia con un solo metodo si chiama interfaccia "segregata":
// è il principio ISP (Interface Segregation Principle) — le interfacce
// devono essere piccole e focalizzate, non monolitiche.
// ═══════════════════════════════════════════════════════════════════

namespace GiocoRPG.Interfacce
{
    public interface ISalvabile
    {
        // Restituisce una rappresentazione stringa (JSON-like) dello stato
        // corrente dell'oggetto. Giorno 9-10 useremo System.Text.Json
        // per la serializzazione automatica.
        string Serializza();
    }
}
