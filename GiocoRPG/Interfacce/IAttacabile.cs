// ═══════════════════════════════════════════════════════════════════
// IATTACABILE — Interfacce/IAttacabile.cs
// ═══════════════════════════════════════════════════════════════════
// Contratto per qualsiasi entità che sa attaccare e ricevere danni.
// Non importa se è un Guerriero, un Nemico o un oggetto trappola:
// basta implementare questi due metodi per partecipare al combattimento.
//
// CONVENZIONE: i nomi delle interfacce iniziano con la lettera I
// maiuscola — è uno standard universale nel mondo C# e .NET.
//
// NOTA: il nome corretto sarebbe "IAttaccabile" (doppia c).
// È un refuso mantenuto per coerenza con il progetto.
// ═══════════════════════════════════════════════════════════════════

namespace GiocoRPG.Interfacce
{
    public interface IAttacabile
    {
        // Ogni classe che implementa questa interfaccia DEVE fornire
        // la propria logica di calcolo del danno.
        // Grazie a questo metodo, SistemaTurni può chiamare CalcolaDanno()
        // senza sapere se l'oggetto è un Guerriero, un Mago o un Goblin.
        int CalcolaDanno();

        // Il tipo di danno ha un valore di default (Fisico): chi chiama
        // SubisciDanno(20) senza specificare il tipo usa Fisico automaticamente.
        void SubisciDanno(int danno, TipoDanno tipo = TipoDanno.Fisico);

        // ─────────────────────────────────────────────────────────────
        // METODO DI DEFAULT (C# 8+)
        // Fornisce un'implementazione di base che le classi POSSONO
        // sovrascrivere con override, ma non sono obbligate.
        // Se una classe non fa override, viene usata questa versione.
        // A differenza dei metodi abstract, non è obbligatorio implementarlo.
        // ─────────────────────────────────────────────────────────────
        void Descrivi() => Console.WriteLine("Sono un combattente.");
    }
}
