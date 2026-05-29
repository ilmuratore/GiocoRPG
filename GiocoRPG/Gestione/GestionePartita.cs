using System;
using System.Collections.Generic;
using System.Text;

namespace GiocoRPG.Gestione
{
    public class GestionePartita
    {

        private Personaggio? _giocatore;
        private Nemico? _nemicoCorrente;
        private List<Personaggio>? _alleati;

        public bool PartitaInCorso => _giocatore?.IsVivo() ?? false;
        public string NomeBoss => _nemicoCorrente?.Nome ?? "Nessun Boss";

        public void SelezionaClasse(string classe)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(classe);
            _giocatore = classe.ToLower() switch
            {
                "g" => PersonaggioFactory.CreaGuerriero("Eroe"),
                "m" => PersonaggioFactory.CreaMago("Eroe"),
                "l" => PersonaggioFactory.CreaLadro("Eroe"),
                _ => throw new ArgumentException($"Classe '{classe}' non valida. Usa: g,m,l.", nameof(classe))
            };
            Console.WriteLine($"Classe giocatore: {_giocatore.GetType().Name}");
        }


        public void NuovoNemico(int livello)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(livello);
            _nemicoCorrente = Nemico.CreaPerLivello(livello);
            Console.WriteLine($"Nemico: {_nemicoCorrente.Nome}");
        }

        public void AggiungiAlleato(Personaggio alleato)
        {
            ArgumentNullException.ThrowIfNull(alleato);
            _alleati ??= new List<Personaggio>();
            _alleati.Add(alleato);
            Console.WriteLine($"Nuovo alleato: {alleato.Nome}");
        }

        public void EseguiTurnoGiocatore()
        {
            if (_giocatore is null) throw new InvalidOperationException("Seleziona prima una classe.");
            if (_nemicoCorrente is null) throw new InvalidOperationException("Nessun nemico presente");

            int danno = _giocatore.CalcolaDanno();
            _nemicoCorrente.SubisciDanno(danno);

            if (!_nemicoCorrente.IsVivo())
            {
                _giocatore.GuadagnaEsperienza(_nemicoCorrente.XpReward);
                Console.WriteLine(_nemicoCorrente.DropItem != null ? $"Drop: {_nemicoCorrente.DropItem}" : "Nessun Item da droppare");
                _nemicoCorrente = null;
            }
        }

        public void StampaStato()
        {
            Console.WriteLine(" == Stato ==");
            Console.WriteLine($" Giocatore: {_giocatore?.Nome ?? "Non selezionato"}");
            Console.WriteLine($"Vita: {_giocatore?.Vita.ToString() ?? "-"}");
            Console.WriteLine($"Livello: {_giocatore?.Livello.ToString() ?? "-"}");
            Console.WriteLine($"Alleati: {_alleati?.Count ?? 0}");
        }
    }
}
