using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace GiocoRPG.Exception
{
    public class VitaInvalidaException : ArgumentOutOfRangeException
    {

        public int ValoreRicevuto { get; }
        public int VitaMax { get; }

        public VitaInvalidaException(int valore, int vitaMax)
            : base(paramName: "vita", actualValue: valore, message: $"La vita deve essere tra 0 e {vitaMax}. Valore ricevuto: {valore}")
        {
            ValoreRicevuto = valore;
            VitaMax = vitaMax;
        }
        
    }
}
