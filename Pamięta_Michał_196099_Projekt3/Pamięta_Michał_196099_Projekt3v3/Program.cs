using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Pamięta_Michał_196099_Projekt3v3
{
    class BigReaderLock
    {
        Random rnd = new Random();
        Object[] zamki;
        Queue<int> usedCores;
        public BigReaderLock(int liczbazamkow)
        {
            zamki = new Object[liczbazamkow];
            for (int i = 0; i < zamki.Length; i++)
            {
                zamki[i] = new Object();
            }
            usedCores = new Queue<int>(liczbazamkow);
        }
        private int czyDostepneRdzenieILosowoWybrany()
        {
            if (usedCores.Contains(-1)) return -1; //zajęte przez pisarza
            if (usedCores.Count == zamki.Length) return -2; //wszystkie zajęte przez czytelników - brak wolnych rdzeni
            int tmp;
            do
            {
                tmp = rnd.Next(0, zamki.Length);
            } while (usedCores.Contains(tmp));
            return tmp;
        }
        private bool czyZajeteRdzenie()
        {
            return usedCores.Contains(-1) ? true : false;
        }
        public void rd_lock()
        {
            int tmp = czyDostepneRdzenieILosowoWybrany();
            if (tmp < 0) throw new InvalidLockReaderBRL();
            lock (zamki[tmp])
            {
                while (usedCores.Contains(-1)) Monitor.Wait(zamki[tmp]);
                usedCores.Enqueue(tmp);
            }
            Console.WriteLine("Czytacz blokuje zamek");
        }
        public void rd_unlock()
        {
            if (czyZajeteRdzenie()) throw new InvalidUnlockReaderBRL();
            int tmp = usedCores.Dequeue();
            lock (zamki[tmp])
            {
                Monitor.Pulse(zamki[tmp]);
            }
            Console.WriteLine("Czytacz zwalnia zamek");
        }
        public void wr_lock()
        {
            if (czyZajeteRdzenie()) throw new InvalidLockWriterBRL();
            usedCores.Enqueue(-1);
            for (int i = 0; i < zamki.Length; i++)
            {
                lock (zamki[i])
                {
                    while (!usedCores.Contains(-1)) Monitor.Wait(zamki[i]);
                    usedCores.Enqueue(i);
                }
            }
            usedCores.Enqueue(-1);
            Console.WriteLine("Pisarz blokuje zamki");
        }
        public void wr_unlock()
        {
            if (!(czyDostepneRdzenieILosowoWybrany() == -1)) throw new InvalidUnlockWriterBRL();
            usedCores.Dequeue();
            for (int i = 0; i < zamki.Length; i++)
            {
                lock (zamki[i])
                {
                    if (!usedCores.Contains(-1)) throw new InvalidUnlockWriterBRL();
                    usedCores.Dequeue();
                    Monitor.Pulse(zamki[i]);
                }
            }
            usedCores.Dequeue();
            Console.WriteLine("Pisarz zwalnia zamki");
        }
    }
    class InvalidLockReaderBRL : Exception
    {
        public InvalidLockReaderBRL() { }
    }
    class InvalidUnlockReaderBRL : Exception
    {
        public InvalidUnlockReaderBRL() { }
    }
    class InvalidUnlockWriterBRL : Exception
    {
        public InvalidUnlockWriterBRL() { }
    }
    class InvalidLockWriterBRL : Exception
    {
        public InvalidLockWriterBRL() { }
    }
    class Program
    {
        static void Main(string[] args)
        {
            int liczbazamkow = 4;
            BigReaderLock brl = new BigReaderLock(liczbazamkow);

            brl.wr_lock();
            brl.wr_unlock();

            brl.rd_lock();
            brl.rd_unlock();
            brl.rd_lock();
            brl.rd_unlock();
            brl.rd_lock();
            brl.rd_unlock();

            brl.wr_lock();
            brl.wr_unlock();

            brl.rd_lock();
            brl.rd_unlock();

            brl.wr_lock();
            brl.wr_unlock();

            for (int i = 0; i < liczbazamkow; i++)
                brl.rd_lock();
            for (int i = 0; i < liczbazamkow; i++)
                brl.rd_unlock();

            brl.wr_lock();
            brl.wr_unlock();

            Console.WriteLine("\nKoniec");
            Console.ReadKey();
        }
    }
}
