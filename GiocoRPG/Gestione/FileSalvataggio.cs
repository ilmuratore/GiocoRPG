using System;
using System.Collections.Generic;
using System.Text;

namespace GiocoRPG.Gestione
{
    public class FileSalvataggio : IDisposable
    {
        private StreamWriter? _writer;
        private bool _disposed = false;

        public FileSalvataggio(string percorso)
        {
            _writer = new StreamWriter(percorso, append: true);
            Console.WriteLine($" [LOG] File aperto: {percorso}");
        }

        public void Scrivi(string linea)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            _writer!.WriteLine($"[{DateTime.Now:HH:mm:ss}] {linea}");
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _writer?.Flush();
                _writer?.Dispose();
                _disposed = true;
                Console.WriteLine("[LOG] File chiuso");
            }
        }


    }
}
