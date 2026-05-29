using System;
using System.Collections.Generic;
using System.Text;

namespace GiocoRPG.Exception
{
    public class InventarioPienoException : InvalidOperationException
    {
        public int CapacitaMassima { get; }

        public InventarioPienoException(int capacita) : base($"Inventario pieno: massima {capacita} oggetto.")
        {
            CapacitaMassima = capacita;
        }
    }
}
