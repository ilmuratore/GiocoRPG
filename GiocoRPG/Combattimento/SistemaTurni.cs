// ═══════════════════════════════════════════════════════════════════
// SISTEMA TURNI — Combattimento/SistemaTurni.cs
// ═══════════════════════════════════════════════════════════════════
// Questa classe gestisce gli scontri tra Personaggi.
//
// È il punto dove il polimorfismo brilla di più:
// SistemaTurni non sa nulla di Guerriero, Mago, Ladro o Drago.
// Lavora esclusivamente con il tipo base Personaggio e chiama
// CalcolaDanno() e SubisciDanno() — il runtime sceglie la versione
// corretta per ciascun oggetto (late binding / vtable).
//
// REFACTOR rispetto al Giorno 3:
//   RIMOSSO — GeneraNemici() con array paralleli (approccio procedurale)
//   RIMOSSO — CombattiControNemico() con parametri ref
//   RIMOSSO — CalcolaDanno(int attacco, int difesa) come funzione statica
//   AGGIUNTO — EseguiRound(): party vs bersaglio con polimorfismo
//   AGGIUNTO — AssegnaRicompensa(): usa IPremiabile, non Nemico
//   AGGIORNATO — SimulaScontro() usa CalcolaDanno() sull'oggetto
// ═══════════════════════════════════════════════════════════════════

namespace GiocoRPG.Combattimento
{
    public class SistemaTurni
    {
        // Lista privata: chi usa SistemaTurni non può accedere direttamente
        // ai combattenti. Deve usare Aggiungi() — incapsulamento.
        // "readonly" = la lista stessa non può essere sostituita,
        // ma il suo contenuto può essere modificato (Add, Remove...).
        private readonly List<Personaggio> _combattenti = new();

        // Aggiunge un combattente al party.
        // Accetta qualsiasi Personaggio: Guerriero, Mago, Ladro — upcasting implicito.
        public void Aggiungi(Personaggio p) => _combattenti.Add(p);

        // Stampa tutti i combattenti chiamando il loro ToString() polimorfico.
        // Ogni tipo mostra le proprie statistiche nel formato che ha definito.
        public void StampaParty()
        {
            Console.WriteLine("=== PARTY ===");
            foreach (var personaggio in _combattenti)
            {
                Console.WriteLine($" {personaggio}");  // chiama ToString() polimorfico
            }
        }


        // ─────────────────────────────────────────────────────────────
        // STAMPA STATO — metodo statico di utilità visiva
        // "static" = non serve un'istanza di SistemaTurni per chiamarlo.
        // Visualizza una barra vita testuale: █ per vita rimanente, ░ per persa.
        //
        // Esempio output: " Drago Antico [██░░░░░░░░] 100/500"
        // ─────────────────────────────────────────────────────────────
        public static void StampaStato(Personaggio p)
        {
            // PercentualeVita() restituisce 0.0-1.0, moltiplicato per 10
            // otteniamo quanti blocchi █ disegnare (0-10).
            int    pieno = Math.Clamp((int)(p.PercentualeVita() * 10), 0, 10);
            string barra = "[" + new string('█', pieno) + new string('░', 10 - pieno) + "]";
            Console.WriteLine($" {p.Nome} {barra} {p.Vita}/{p.VitaMax}");
        }


        // ─────────────────────────────────────────────────────────────
        // SIMULA SCONTRO 1 VS 1 — statico, non serve un'istanza
        // Riceve due Personaggio: può essere Guerriero vs Goblin,
        // Mago vs Drago, Ladro vs Orco — qualsiasi combinazione.
        //
        // Chi attacca per primo? Chi ha l'Attacco più alto.
        // I turni si alternano finché uno dei due muore.
        //
        // POLIMORFISMO IN AZIONE:
        //   int danno = primo.CalcolaDanno()
        //   → se primo è Guerriero: Attacco + Forza + dado10
        //   → se primo è Mago: Magia + dado (scala mana)
        //   → se primo è Drago: Attacco + dado
        //   Il compilatore non lo sa — lo decide il runtime.
        // ─────────────────────────────────────────────────────────────
        public static Personaggio SimulaScontro(Personaggio a, Personaggio b)
        {
            Personaggio primo   = a.Attacco >= b.Attacco ? a : b;
            Personaggio secondo = primo == a ? b : a;

            Console.WriteLine($"\n {primo.Nome} (ATK: {primo.Attacco}) attacca per primo. \n");

            int turno = 1;
            while (primo.IsVivo() && secondo.IsVivo())
            {
                Console.WriteLine($" == turno {turno} == ");

                // ← polimorfismo: versione corretta scelta a runtime
                int danno = primo.CalcolaDanno();
                secondo.SubisciDanno(danno);
                if (!secondo.IsVivo()) break;

                // ← polimorfismo
                danno = secondo.CalcolaDanno();
                primo.SubisciDanno(danno);
                turno++;
            }

            Personaggio vincitore = primo.IsVivo() ? primo : secondo;
            Console.WriteLine($"\n {vincitore.Nome} vince lo scontro!");
            return vincitore;
        }


        // ─────────────────────────────────────────────────────────────
        // ESEGUI ROUND — tutto il party attacca un singolo bersaglio
        // Metodo di istanza: usa la lista _combattenti dell'istanza.
        //
        // Differenza rispetto a SimulaScontro:
        //   SimulaScontro → 1 vs 1, statico, autonomo
        //   EseguiRound   → N vs 1, instance, usa il party interno
        //
        // "continue" salta i combattenti già morti senza interrompere il ciclo.
        // "break" interrompe il round non appena il bersaglio muore.
        // ─────────────────────────────────────────────────────────────
        public void EseguiRound(Personaggio bersaglio)
        {
            foreach (var c in _combattenti)
            {
                if (!c.IsVivo()) continue;  // salta se già morto

                // ← polimorfismo: Guerriero? Mago? Ladro? Il runtime decide.
                int danno = c.CalcolaDanno();
                Console.WriteLine($" {c.Nome} (Lv:{c.Livello}) attacca per {danno} danni.");
                bersaglio.SubisciDanno(danno);

                if (!bersaglio.IsVivo()) break;  // round finito: bersaglio sconfitto
            }
        }


        // ─────────────────────────────────────────────────────────────
        // ASSEGNA RICOMPENSA — usa IPremiabile, non Nemico
        // Il parametro è IPremiabile, non Nemico: questo metodo
        // non sa (e non deve sapere) il tipo concreto del nemico sconfitto.
        // Funziona con qualsiasi oggetto che implementa IPremiabile.
        // ─────────────────────────────────────────────────────────────
        public static void AssegnaRicompensa(Personaggio giocatore, IPremiabile nemico)
        {
            Console.WriteLine($" +{nemico.XpReward} XP per {giocatore.Nome}!");
            if (nemico.DropItem != null)
            {
                Console.WriteLine($" Trovato: {nemico.DropItem}");
            }
        }
    }
}
