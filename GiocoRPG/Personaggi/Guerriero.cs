// ═══════════════════════════════════════════════════════════════════
// GUERRIERO — Personaggi/Guerriero.cs
// ═══════════════════════════════════════════════════════════════════
// Guerriero è una classe CONCRETA che estende Personaggio.
//
// "Concreta" = può essere istanziata: new Guerriero("Arthur") funziona.
// Deve implementare CalcolaDanno() perché Personaggio lo dichiara
// abstract — il compilatore rifiuta la compilazione altrimenti.
//
// Profilo: alta vita, alto attacco fisico, buona difesa.
//          Danno = Attacco + Forza + dado a 10 facce.
//
// GIORNO 5 — aggiunge ISalvabile:
//   Serializza() produce una stringa JSON-like con lo stato corrente.
//   Il Ladro non implementa ISalvabile per scelta: se viene passato
//   a SistemaSalvataggio.SalvaTutti(), il compilatore segnala errore.
// ═══════════════════════════════════════════════════════════════════

namespace GiocoRPG.Personaggi
{
    public class Guerriero : Personaggio, ISalvabile
    {
        // Il costruttore riceve solo il nome: le statistiche sono fisse
        // per questa classe e vengono passate direttamente a base().
        //
        // ": base(...)" richiama il costruttore di Personaggio.
        // Senza questa chiamata il compilatore darebbe errore:
        // Personaggio non ha un costruttore senza parametri.
        public Guerriero(string nome)
        : base(
            nome,
            vitaMax:      150,
            attacco:       25,
            difesa:        18,
            forza:         20,
            agilita:       12,
            intelligenza:   8,
            mana:          20,
            manaMax:       20,
            Magia:          5)
        {
            // Il corpo è vuoto: tutta l'inizializzazione avviene in Personaggio.
        }


        // ─────────────────────────────────────────────────────────────
        // IMPLEMENTAZIONE DEL METODO ASTRATTO — obbligatoria
        // "override" dice al compilatore che stiamo intenzionalmente
        // ridefinendo un metodo della classe padre.
        //
        // Danno = Attacco (stat base) + Forza (stat del guerriero)
        //       + numero casuale tra 1 e 10 (variabilità)
        //
        // Il polimorfismo entra in gioco quando SistemaTurni chiama
        // p.CalcolaDanno() su un Personaggio: se p è un Guerriero,
        // viene eseguita questa versione.
        // ─────────────────────────────────────────────────────────────
        public override int CalcolaDanno()
        {
            return Attacco + Forza + Random.Shared.Next(1, 11);
        }


        // ─────────────────────────────────────────────────────────────
        // METODO SPECIFICO DEL GUERRIERO
        // Non è polimorfico: solo Guerriero ha AttaccoFisico().
        // Per chiamarlo dall'esterno serve un riferimento di tipo
        // Guerriero, non Personaggio (downcast necessario).
        //
        // SistemaTurni usa CalcolaDanno() + SubisciDanno() direttamente,
        // non questo metodo — che è utile per interazioni manuali.
        // ─────────────────────────────────────────────────────────────
        public void AttaccoFisico(Personaggio bersaglio)
        {
            int danno = CalcolaDanno();
            Console.WriteLine($" {Nome} colpisce il bersaglio e causa {danno} danni.");
            bersaglio.SubisciDanno(danno);  // TipoDanno.Fisico per default
        }


        // ─────────────────────────────────────────────────────────────
        // ISALVABILE — implementazione del contratto
        // Serializza lo stato corrente come stringa JSON-like.
        // Il Giorno 9-10 useremo System.Text.Json per farlo automaticamente.
        // ─────────────────────────────────────────────────────────────
        public string Serializza() =>
            $"{{\"tipo\":\"Guerriero\",\"nome\":\"{Nome}\"," +
            $"\"vita\":{Vita},\"vitaMax\":{VitaMax}," +
            $"\"livello\":{Livello},\"attacco\":{Attacco}}}";
    }
}
