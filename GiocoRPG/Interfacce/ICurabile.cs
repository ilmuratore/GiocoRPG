// ═══════════════════════════════════════════════════════════════════
// ICURABILE — Interfacce/ICurabile.cs
// ═══════════════════════════════════════════════════════════════════
// Contratto per qualsiasi entità che può essere curata e che ha vita.
//
// Le interfacce possono dichiarare proprietà (senza backing field):
// chi implementa l'interfaccia fornirà il campo interno e la logica.
//
// Uso tipico:
//   void UsaPozione(ICurabile bersaglio) — accetta qualsiasi entità curabile,
//   non solo Guerriero o Mago. Se domani aggiungiamo un Mercante,
//   basta che implementi ICurabile per ricevere cure.
// ═══════════════════════════════════════════════════════════════════

namespace GiocoRPG.Interfacce
{
    public interface ICurabile
    {
        // Proprietà dichiarate nell'interfaccia: solo getter.
        // La classe che implementa l'interfaccia decide come calcolarle.
        int Vita    { get; }
        int VitaMax { get; }

        // Aggiunge vita al personaggio.
        // L'implementazione in Personaggio clampa automaticamente a VitaMax.
        void Cura(int quantita);

        // Controlla se il personaggio è ancora in vita.
        // Usato da SistemaTurni per saltare combattenti già sconfitti.
        bool IsVivo();
    }
}
