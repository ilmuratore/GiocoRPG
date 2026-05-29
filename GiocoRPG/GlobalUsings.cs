// ═══════════════════════════════════════════════════════════════════
// GLOBAL USINGS
// ═══════════════════════════════════════════════════════════════════
// Invece di scrivere "using GiocoRPG.Core;" in ogni singolo file,
// dichiariamo qui tutti i namespace del progetto una volta sola.
//
// "global using" = importazione valida in tutti i file del progetto.
// È una funzionalità di C# 10+ (introdotta con .NET 6).
//
// Regola pratica:
//   - global using → namespace interni del nostro progetto
//   - using locale  → librerie esterne o namespace usati in un solo file
// ═══════════════════════════════════════════════════════════════════

global using GiocoRPG.Combattimento;  // SistemaTurni
global using GiocoRPG.Personaggi;     // Guerriero, Mago, Ladro
global using GiocoRPG.Nemici;         // Nemico e sottoclassi
global using GiocoRPG.Oggetti;        // OggettoGioco, Pozione, Arma, Inventario
global using GiocoRPG.Core;           // Personaggio, TipoDanno
global using GiocoRPG.Factory;        // PersonaggioFactory
global using GiocoRPG.Interfacce;     // IAttacabile, ICurabile, IOggetto, IPremiabile, ISalvabile
global using GiocoRPG.Salvataggio;    // SistemaSalvataggio
global using GiocoRPG.Collezioni;     // CodaSpawn
global using GiocoRPG.Exception;      // VitaInvalidaException, InventarioPienoException, OggettoNonTrovatoException
global using GiocoRPG.Gestione;       // GestionePartita, FileSalvataggio
