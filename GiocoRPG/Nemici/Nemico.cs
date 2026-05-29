// ═══════════════════════════════════════════════════════════════════
// NEMICO — Nemici/Nemico.cs
// ═══════════════════════════════════════════════════════════════════
// Nemico è la classe base di tutti i nemici del gioco.
// Estende Personaggio: un Nemico IS-A Personaggio.
//
// PRIMA del Giorno 4, Nemico era una classe separata con le proprie
// proprietà Nome, Vita, Attacco, Difesa — duplicate rispetto a
// Personaggio. Non poteva essere passato a SimulaScontro(Personaggio a, b).
//
// DOPO il refactor: Nemico : Personaggio → entra nella gerarchia.
// SistemaTurni.SimulaScontro(guerriero, goblin) funziona direttamente.
//
// GIORNO 5 — aggiunta interfaccia IPremiabile:
//   XpReward e DropItem espongono la ricompensa al momento della sconfitta.
//   AssegnaRicompense() in SistemaTurni riceve IPremiabile — non sa
//   (e non deve sapere) che si tratta di un Nemico specifico.
//
// PATTERN FACTORY METHOD:
// Le classi concrete (NemicoBase, Goblin, Orco, Scheletro, Drago)
// sono "internal": visibili solo all'interno di questo progetto.
// Chi crea nemici usa sempre i metodi statici pubblici:
//   Nemico.CreaGoblin(), Nemico.CreaDrago(), ecc.
// ═══════════════════════════════════════════════════════════════════

namespace GiocoRPG.Nemici
{
    public class Nemico : Personaggio, IPremiabile
    {
        // IPremiabile richiede XpReward e DropItem.
        // "protected set" = le sottoclassi possono modificarli.
        // "string?" = può essere null se il nemico non droppa nulla.
        public int     XpReward { get; protected set; }
        public string? DropItem { get; protected set; }


        // Costruttore protected: non istanziabile direttamente dall'esterno.
        // Le sottoclassi lo chiamano via base(...).
        // "drop = null" è un parametro opzionale: i nemici senza drop
        // non devono passare il parametro esplicitamente.
        protected Nemico(string nome, int vitaMax, int attacco, int difesa,
                         int xp, string? drop = null)
        : base(
            nome, vitaMax, attacco, difesa,
            forza:        10,
            agilita:      10,
            intelligenza: 10,
            mana:         20,
            manaMax:      20,
            Magia:         5)
        {
            XpReward = xp;
            DropItem = drop;
        }


        // Implementazione di default: attacco con variazione casuale ±5.
        // Le sottoclassi (Goblin, Orco, Drago...) possono fare override.
        public override int CalcolaDanno()
        {
            return Attacco + Random.Shared.Next(-5, 5);
        }

        // Aggiunge XP e drop a base.ToString() per mostrare
        // le ricompense insieme alle statistiche del nemico.
        public override string ToString()
            => base.ToString() + $" | XP: {XpReward}" +
               (DropItem != null ? $" | Drop: {DropItem}" : "");


        // ─────────────────────────────────────────────────────────────
        // FACTORY METHODS — unica porta d'accesso per creare nemici
        // Ogni metodo restituisce il tipo base "Nemico" anche se
        // internamente crea una sottoclasse. Il chiamante non sa
        // (e non deve sapere) che Goblin e Drago sono classi diverse.
        // ─────────────────────────────────────────────────────────────
        public static Nemico CreaNemicoBase() => new NemicoBase("Nemico Base", vitaMax: 20, attacco: 10, difesa: 10, xp: 20);
        public static Nemico CreaGoblin()     => new Goblin();
        public static Nemico CreaOrco()       => new Orco();
        public static Nemico CreaScheletro()  => new Scheletro();
        public static Nemico CreaDrago()      => new Drago();

        // Factory intelligente: sceglie il tipo di nemico in base al livello.
        // "<= 2" è un pattern relazionale (C# 9+).
        // "_" è il caso default — corrisponde a qualsiasi valore non catturato.
        public static Nemico CreaPerLivello(int livello) => livello switch
        {
            <= 2 => CreaGoblin(),
            <= 5 => CreaOrco(),
            <= 8 => CreaScheletro(),
            _    => CreaDrago()
        };
    }


    // ═══════════════════════════════════════════════════════════════════
    // NEMICO BASE — placeholder generico
    // internal = visibile solo in questo assembly. Non esposto all'esterno.
    // ═══════════════════════════════════════════════════════════════════
    internal class NemicoBase : Nemico
    {
        public NemicoBase(string nome, int vitaMax, int attacco, int difesa, int xp)
            : base(nome, vitaMax, attacco, difesa, xp) { }

        public override int CalcolaDanno() => Attacco + Random.Shared.Next(-5, 10);
    }


    // ═══════════════════════════════════════════════════════════════════
    // GOBLIN — immune al fuoco
    // Override di SubisciDanno() per gestire l'immunità.
    // La guard clause "if immune → return" è un pattern comune:
    // controlla la condizione speciale all'inizio ed esce subito,
    // senza annidare il comportamento normale in un else.
    // ═══════════════════════════════════════════════════════════════════
    internal class Goblin : Nemico
    {
        public bool ImmuneFuoco { get; } = true;

        public Goblin() : base("Boss Goblin", vitaMax: 100, attacco: 30, difesa: 25,
                               xp: 40, drop: "Mazza da Goblin") { }

        public override int CalcolaDanno() => Attacco + Random.Shared.Next(-5, 10);

        public override void SubisciDanno(int danno, TipoDanno tipo = TipoDanno.Fisico)
        {
            if (tipo == TipoDanno.Fuoco)
            {
                Console.WriteLine($"{Nome} è immune al fuoco!");
                return;  // guard clause: esce senza ridurre la vita
            }
            base.SubisciDanno(danno, tipo);  // comportamento normale per gli altri tipi
        }

        public override string ToString() => base.ToString() + " | [IMMUNE AL FUOCO]";
    }


    // ═══════════════════════════════════════════════════════════════════
    // ORCO — immune al ghiaccio
    // Stesso pattern del Goblin, tipo di danno diverso.
    // ═══════════════════════════════════════════════════════════════════
    internal class Orco : Nemico
    {
        public bool ImmuneGhiaccio { get; } = true;

        public Orco() : base("Boss Orco", vitaMax: 200, attacco: 20, difesa: 10,
                             xp: 60, drop: "Dente di Orco") { }

        public override int CalcolaDanno() => Attacco + Random.Shared.Next(-5, 10);

        public override void SubisciDanno(int danno, TipoDanno tipo = TipoDanno.Fisico)
        {
            if (tipo == TipoDanno.Ghiaccio)
            {
                Console.WriteLine($"{Nome} è immune al ghiaccio!");
                return;
            }
            base.SubisciDanno(danno, tipo);
        }

        public override string ToString() => base.ToString() + " | [IMMUNE AL GHIACCIO]";
    }


    // ═══════════════════════════════════════════════════════════════════
    // SCHELETRO — immune alla magia
    // Il Mago deve usare TipoDanno.Fuoco o TipoDanno.Ghiaccio
    // per danneggiarlo — LanciaIncantesimo() (Magico) non ha effetto.
    // ═══════════════════════════════════════════════════════════════════
    internal class Scheletro : Nemico
    {
        public bool ImmuneMagico { get; } = true;

        public Scheletro() : base("Boss Scheletro", vitaMax: 100, attacco: 15, difesa: 15,
                                  xp: 70, drop: "Ossa Antiche") { }

        public override int CalcolaDanno() => Attacco + Random.Shared.Next(-5, 10);

        public override void SubisciDanno(int danno, TipoDanno tipo = TipoDanno.Fisico)
        {
            if (tipo == TipoDanno.Magico)
            {
                Console.WriteLine($"{Nome} è immune alla Magia!");
                return;
            }
            base.SubisciDanno(danno, tipo);
        }

        public override string ToString() => base.ToString() + " | [IMMUNE ALLA MAGIA]";
    }


    // ═══════════════════════════════════════════════════════════════════
    // DRAGO — immune a Puro e Magico
    // Il boss finale ha doppia immunità: né gli incantesimi generici
    // né gli attacchi "puri" lo danneggiano.
    // Solo Fisico, Fuoco e Ghiaccio sono efficaci.
    // ═══════════════════════════════════════════════════════════════════
    internal class Drago : Nemico
    {
        public bool ImmunePuro { get; } = true;

        public Drago() : base("Drago Antico", vitaMax: 500, attacco: 50, difesa: 50,
                              xp: 200, drop: "Cuore di Drago") { }

        public override int CalcolaDanno() => Attacco + Random.Shared.Next(-5, 10);

        // Override con doppia condizione: il Drago respinge sia Puro che Magico.
        // L'operatore || è short-circuit: se il primo è true, il secondo non viene valutato.
        public override void SubisciDanno(int danno, TipoDanno tipo = TipoDanno.Fisico)
        {
            if (tipo == TipoDanno.Puro || tipo == TipoDanno.Magico)
            {
                Console.WriteLine($"{Nome} è immune agli attacchi puri e magici!");
                return;
            }
            base.SubisciDanno(danno, tipo);
        }

        public override string ToString() => base.ToString() + " | [IMMUNE ALLA MAGIA E ATTACCHI PURI]";
    }
}
