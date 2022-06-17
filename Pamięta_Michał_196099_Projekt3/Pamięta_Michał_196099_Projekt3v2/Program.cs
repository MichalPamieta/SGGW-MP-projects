using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Pamięta_Michał_196099_Projekt3v2
{
    class BigReaderLock
    {
        Random rnd = new Random();
        int[] stan;
        Object[] zamki;
        Queue<int> usedCores;
        public BigReaderLock(int liczbazamkow)
        {
            stan = new int[liczbazamkow];
            zamki = new Object[liczbazamkow];
            for (int i = 0; i < zamki.Length; i++)
            {
                zamki[i] = new Object();
            }
            usedCores = new Queue<int>(liczbazamkow);
        }
        private int czyDostepneRdzenieILosowoWybrany()
        {
            if (Array.Exists(stan, item => item.Equals(-1))) return -1; //zajęte przez pisarza
            if (!Array.Exists(stan, item => item.Equals(0))) return -2; //wszystkie zajęte przez czytelników - brak wolnych rdzeni
            int tmp;
            do
            {
                tmp = rnd.Next(0, stan.Length);
            } while (stan[tmp] != 0);
            return tmp;
        }
        private int czyZajeteRdzenie()
        {
            return Array.Exists(stan, item => item.Equals(1)) ? 1 : 0;
        }
        public void rd_lock()
        {
            int tmp = czyDostepneRdzenieILosowoWybrany();
            if (tmp < 0) throw new InvalidLockReaderBRL();
            lock (zamki[tmp])
            {
                while (stan[tmp] == -1) Monitor.Wait(zamki[tmp]);
                ++stan[tmp];
                usedCores.Enqueue(tmp);
            }
            Console.WriteLine("Czytacz blokuje zamek");
        }
        public void rd_unlock()
        {
            if (czyZajeteRdzenie() == 0) throw new InvalidUnlockReaderBRL();
            int tmp = usedCores.Dequeue();
            lock (zamki[tmp])
            {
                if (stan[tmp] <= 0) throw new InvalidUnlockReaderBRL();
                if (--stan[tmp] == 0) Monitor.Pulse(zamki[tmp]);
            }
            Console.WriteLine("Czytacz zwalnia zamek");
        }
        public void wr_lock()
        {
            if (czyZajeteRdzenie() == 1) throw new InvalidLockWriterBRL();
            for (int i = 0; i < zamki.Length; i++)
            {
                lock (zamki[i])
                {
                    while (stan[i] != 0) Monitor.Wait(zamki[i]);
                    stan[i] = -1;
                    usedCores.Enqueue(i);
                }
            }
            Console.WriteLine("Pisarz blokuje zamki");
        }
        public void wr_unlock()
        {
            if (!(czyDostepneRdzenieILosowoWybrany() == -1)) throw new InvalidUnlockWriterBRL();
            for (int i = 0; i < zamki.Length; i++)
            {
                lock(zamki[i])
                {
                    if (stan[i] != -1) throw new InvalidUnlockWriterBRL();
                    stan[i] = 0;
                    usedCores.Dequeue();
                    Monitor.Pulse(zamki[i]);
                }
            }
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
