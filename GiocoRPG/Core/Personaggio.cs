// ═══════════════════════════════════════════════════════════════════
// PERSONAGGIO — Core/Personaggio.cs
// ═══════════════════════════════════════════════════════════════════
// Classe BASE e ASTRATTA della gerarchia.
//
// ASTRATTA significa:
//   1. Non può essere istanziata direttamente: new Personaggio(...) → ERRORE
//   2. Può contenere metodi astratti (senza corpo) che le sottoclassi
//      DEVONO implementare obbligatoriamente.
//
// Questa classe rappresenta il contratto comune a Guerriero, Mago,
// Ladro e Nemico. Tutto ciò che è condiviso vive qui.
//
// SCELTA DI DESIGN — tutte le stat nella classe base:
//   Forza, Agilita, Magia, Mana sono proprietà di Personaggio.
//   Il vantaggio è che il costruttore è unico e SistemaTurni non deve
//   fare downcast per leggere le statistiche.
//
// GIORNO 5 — interfacce aggiunte:
//   IComparable<Personaggio> → abilita List.Sort() e LINQ OrderBy
//   IAttacabile              → CalcolaDanno() e SubisciDanno()
//   ICurabile                → Vita, VitaMax, Cura(), IsVivo()
//
// GIORNO 6 — HashSet aggiunto:
//   _effettiAttivi → stati attivi (veleno, fuoco, cura)
//   HashSet garantisce unicità: aggiungere "Veleno" due volte conta come uno.
//   Contains è O(1): verificare un effetto ad ogni turno è istantaneo.
//
// GIORNO 7 — eccezioni e guard clause:
//   GuadagnaEsperienza() → lancia su XP negativo o personaggio morto
//   Esperienza e EsperienzaPerLivello → sistema di level up
// ═══════════════════════════════════════════════════════════════════

namespace GiocoRPG.Core
{
    public abstract class Personaggio : IComparable<Personaggio>, IAttacabile, ICurabile
    {
        // ─────────────────────────────────────────────────────────────
        // CAMPO PRIVATO PER VITA CON VALIDAZIONE
        // Non esponiamo _vita direttamente: la proprietà Vita si occupa
        // di clampare il valore tra 0 e VitaMax usando Math.Clamp.
        // Così è impossibile avere vita negativa o superiore al massimo.
        // ─────────────────────────────────────────────────────────────
        private int _vita;

        // Proprietà con "private set": leggibili da tutti,
        // modificabili solo dall'interno di questa classe.
        // Le sottoclassi NON possono cambiarle direttamente — devono
        // passare dal costruttore base().
        public string Nome        { get; private set; }
        public int    VitaMax     { get; private set; }
        public int    Attacco     { get; private set; }
        public int    Difesa      { get; private set; }
        public int    Forza       { get; private set; }
        public int    Agilita     { get; private set; }
        public int    Intelligenza { get; private set; }

        // Mana è "protected set": le sottoclassi (es. Mago) possono
        // modificarlo direttamente. Necessario perché CalcolaDanno()
        // del Mago scala il mana all'interno della stessa gerarchia.
        public int Mana    { get; protected set; }
        public int ManaMax { get; private set; }
        public int Magia   { get; private set; }

        // Vita: get pubblico, set protected con validazione automatica.
        // Math.Clamp(value, 0, VitaMax) garantisce che il valore rimanga
        // sempre nell'intervallo [0, VitaMax], senza bisogno di if.
        public int Vita
        {
            get => _vita;
            protected set => _vita = Math.Clamp(value, 0, VitaMax);
        }

        // Livello inizia a 1 ed è modificabile dalle sottoclassi.
        public int Livello { get; protected set; } = 1;

        // ─────────────────────────────────────────────────────────────
        // ESPERIENZA E LEVEL UP
        // Esperienza: punti accumulati dal personaggio.
        // EsperienzaPerLivello: soglia calcolata dinamicamente —
        //   cresce con il livello: lv1=100, lv2=200, lv3=300...
        //   Usando una proprietà calcolata (=>) invece di un campo,
        //   si aggiorna automaticamente ogni volta che Livello cambia.
        // ─────────────────────────────────────────────────────────────
        public int Esperienza          { get; private set; }
        public int EsperienzaPerLivello => 100 * Livello;

        // ─────────────────────────────────────────────────────────────
        // PROPRIETÀ STATICA — condivisa tra tutte le istanze
        // Conta quanti Personaggio (di qualsiasi tipo) sono stati creati
        // dall'avvio del programma. "static" appartiene alla classe,
        // non a una singola istanza.
        // ─────────────────────────────────────────────────────────────
        public static int TotaleCreati { get; private set; }


        // ─────────────────────────────────────────────────────────────
        // HASHSET — effetti attivi sul personaggio
        //
        // HashSet<string> garantisce che ogni effetto sia presente
        // al massimo una volta: Add("Veleno") due volte → un solo "Veleno".
        // Add restituisce true se aggiunto, false se era già presente.
        //
        // IReadOnlyCollection<string>: espone la collezione in sola lettura.
        // Chi legge EffettiAttivi non può chiamare Add() o Remove() —
        // deve passare dai metodi pubblici di questa classe.
        // ─────────────────────────────────────────────────────────────
        private readonly HashSet<string> _effettiAttivi = new();

        public IReadOnlyCollection<string> EffettiAttivi => _effettiAttivi;

        public void AggiungiEffetto(string effetto)
        {
            if (_effettiAttivi.Add(effetto))
                Console.WriteLine($"Il personaggio è afflitto da: {effetto}");
            else
                Console.WriteLine($"Il personaggio è già afflitto da: {effetto}");
        }

        public void RimuoviEffetto(string effetto)
        {
            _effettiAttivi.Remove(effetto);
            Console.WriteLine($"L'effetto di: {effetto} è terminato");
        }

        // Contains O(1): verificare un effetto ad ogni turno è istantaneo.
        public bool HaEffetto(string effetto) => _effettiAttivi.Contains(effetto);

        public void ApplicaEffetti()
        {
            if (_effettiAttivi.Count == 0) return;

            foreach (string effetto in _effettiAttivi)
            {
                switch (effetto)
                {
                    case "Veleno": SubisciDanno(5,  TipoDanno.Veleno); break;
                    case "Fuoco":  SubisciDanno(10, TipoDanno.Fuoco);  break;
                    case "Cura":   Cura(10);                            break;
                }
            }
        }


        // ─────────────────────────────────────────────────────────────
        // COSTRUTTORE PROTECTED
        // "protected" = richiamabile solo dalle classi figlie via base().
        // Non ha senso renderlo public: nessuno dovrebbe costruire un
        // Personaggio generico dall'esterno.
        // ─────────────────────────────────────────────────────────────
        protected Personaggio(
            string nome, int vitaMax, int attacco, int difesa,
            int forza, int agilita, int intelligenza,
            int mana, int manaMax, int Magia)
        {
            this.Nome         = nome;
            this.VitaMax      = vitaMax;
            this.Attacco      = attacco;
            this.Difesa       = difesa;
            this.Forza        = forza;
            this.Agilita      = agilita;
            this.Intelligenza = intelligenza;
            this.Mana         = mana;
            this.ManaMax      = manaMax;
            this.Magia        = Magia;
            this.Vita         = vitaMax;

            TotaleCreati++;
        }


        // ─────────────────────────────────────────────────────────────
        // METODO ASTRATTO — il cuore del contratto
        // Nessun corpo: è solo una firma. Ogni sottoclasse DEVE
        // fornire la propria implementazione o il compilatore rifiuta.
        // ─────────────────────────────────────────────────────────────
        public abstract int CalcolaDanno();


        // ─────────────────────────────────────────────────────────────
        // METODO VIRTUAL — comportamento di default sovrascrivibile
        // "virtual" significa che le sottoclassi POSSONO fare override.
        // Il parametro "tipo" ha un valore di default (TipoDanno.Fisico):
        // chi chiama SubisciDanno(20) ottiene TipoDanno.Fisico automaticamente.
        // ─────────────────────────────────────────────────────────────
        public virtual void SubisciDanno(int danno, TipoDanno tipo = TipoDanno.Fisico)
        {
            int effettivo = Math.Max(0, danno - Difesa);
            Vita -= effettivo;
            Console.WriteLine($" {Nome} subisce {effettivo} danni [{tipo}]. Vita: {Vita}/{VitaMax}");
            if (Vita == 0) Console.WriteLine($"{Nome} è morto.");
        }


        // ─────────────────────────────────────────────────────────────
        // CURA — richiesto da ICurabile
        // Il setter di Vita clampa automaticamente: non è possibile
        // superare VitaMax. Il recupero effettivo può essere minore
        // di quantita se il personaggio era quasi al massimo.
        // ─────────────────────────────────────────────────────────────
        public void Cura(int quantita)
        {
            int prima = Vita;
            Vita += quantita;
            Console.WriteLine($"{Nome} recupera {Vita - prima} Punti Vita. Vita Attuale: {Vita}/{VitaMax}");
        }


        // ─────────────────────────────────────────────────────────────
        // GUADAGNA ESPERIENZA — guard clause + level up
        //
        // GUARD CLAUSE 1: xp negativo è una violazione di contratto.
        //   ArgumentOutOfRangeException con nameof(xp) — se il parametro
        //   viene rinominato, nameof dà errore di compilazione e ricorda
        //   di aggiornare il messaggio.
        //
        // GUARD CLAUSE 2: un personaggio morto non guadagna esperienza.
        //   InvalidOperationException: l'operazione non è valida
        //   nello stato corrente (il personaggio è morto).
        //
        // WHILE invece di IF: si può salire più livelli con molti XP.
        //   VitaMax += 20 aumenta la vita massima ad ogni level up —
        //   il personaggio diventa più resistente salendo di livello.
        // ─────────────────────────────────────────────────────────────
        public void GuadagnaEsperienza(int xp)
        {
            if (xp < 0)
                throw new ArgumentOutOfRangeException(nameof(xp),
                    $"L'esperienza non può essere negativa. Ricevuto: {xp}");
            if (!IsVivo())
                throw new InvalidOperationException($"{Nome} è morto.");

            Esperienza += xp;
            Console.WriteLine($"{Nome} +{xp} XP ({Esperienza}/{EsperienzaPerLivello})");

            // while: si può salire più livelli in un colpo solo con molti XP
            while (Esperienza >= EsperienzaPerLivello)
            {
                Esperienza -= EsperienzaPerLivello;
                Livello++;
                VitaMax += 20;   // ogni livello aumenta la vita massima
                Vita = VitaMax;  // ripristina la vita al nuovo massimo
                Console.WriteLine($"{Nome} → Level UP {Livello}!");
            }
        }


        // ─────────────────────────────────────────────────────────────
        // METODI DI UTILITÀ
        // ─────────────────────────────────────────────────────────────

        // Restituisce true se il personaggio è ancora in vita.
        public bool IsVivo() => Vita > 0;

        // Restituisce la vita come valore tra 0.0 e 1.0.
        public float PercentualeVita() => (float)Vita / VitaMax;


        // ─────────────────────────────────────────────────────────────
        // ICOMPARABLE — ordinamento naturale
        // CompareTo: negativo = this viene prima, 0 = uguale, positivo = dopo.
        // Ordine primario: livello crescente.
        // Ordine secondario (parità): vita decrescente — chi ha più vita prima.
        // ─────────────────────────────────────────────────────────────
        public int CompareTo(Personaggio? altro)
        {
            if (altro == null) return 1;
            int cmpLivello = Livello.CompareTo(altro.Livello);
            if (cmpLivello != 0) return cmpLivello;
            return altro.Vita.CompareTo(Vita);
        }


        // ─────────────────────────────────────────────────────────────
        // OVERRIDE DI TOSTRING, EQUALS, GETHASHCODE
        // Regola: se si fa override di Equals, fare SEMPRE anche GetHashCode.
        // Dictionary e HashSet usano GetHashCode per organizzare gli elementi.
        // ─────────────────────────────────────────────────────────────
        public override string ToString()
        {
            return $"[{Nome}] " +
                   $"Vita: {Vita}/{VitaMax} ({PercentualeVita():P0}) | " +
                   $"Mana: {Mana}/{ManaMax} | " +
                   $"ATK: {Attacco} | DEF: {Difesa} | " +
                   $"FOR: {Forza} | AGI: {Agilita} | INT: {Intelligenza} | " +
                   $"MAG: {Magia} | LIV: {Livello}";
        }

        public override bool Equals(object? obj) => obj is Personaggio p && Nome == p.Nome;
        public override int GetHashCode() => HashCode.Combine(Nome);
    }
}
