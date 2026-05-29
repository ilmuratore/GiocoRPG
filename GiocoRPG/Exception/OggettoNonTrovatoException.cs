using System;
using System.Collections.Generic;
using System.Text;

namespace GiocoRPG.Exception
{
    public class OggettoNonTrovatoException : KeyNotFoundException
    {
        public string NomeOggetto { get; }

        public OggettoNonTrovatoException(string nome) : base($"Oggetto '{nome}' non trovato nell'inventario")
        {
            NomeOggetto = nome;
        }
    }
}
