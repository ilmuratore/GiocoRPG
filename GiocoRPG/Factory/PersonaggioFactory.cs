// ═══════════════════════════════════════════════════════════════════
// PERSONAGGIO FACTORY — Factory/PersonaggioFactory.cs
// ═══════════════════════════════════════════════════════════════════
// Classe statica che centralizza la creazione dei personaggi giocabili.
//
// "static class" = non può essere istanziata, contiene solo membri statici.
// È l'equivalente di una raccolta di funzioni di utilità, organizzata
// in una classe per chiarezza di namespace.
//
// ═══════════════════════════════════════════════════════════════════

namespace GiocoRPG.Factory
{
    public static class PersonaggioFactory
    {
        // Metodi di creazione esplicita: il chiamante sa quale tipo riceve.
        // Restituiscono il tipo concreto (Guerriero, Mago, Ladro) —
        // utile quando si vuole usare metodi specifici della classe
        // (es. g.AttaccoFisico(), m.LanciaIncantesimo()).
        public static Guerriero CreaGuerriero(string nome) => new(nome);
        public static Mago      CreaMago(string nome)      => new(nome);
        public static Ladro     CreaLadro(string nome)     => new(nome);

        // Restituisce il tipo base Personaggio: il chiamante non sa quale
        // sottoclasse è stata creata. Utile quando si popola una lista
        // eterogenea o si vuole varietà casuale senza conoscere il tipo.
        //
        // Esempio: List<Personaggio> party = new() { CreaRandom("Eroe1"), ... }
        // Il polimorfismo si occupa del resto: CalcolaDanno() chiamerà
        // la versione giusta a runtime.
        public static Personaggio CreaRandom(string nome)
        {
            return Random.Shared.Next(0, 3) switch
            {
                0 => CreaGuerriero(nome),
                1 => CreaMago(nome),
                _ => CreaLadro(nome)   // caso default: Ladro
            };
        }
    }
}
