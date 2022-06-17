using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Pamięta_Michał_196099_Projekt3v1
{
    class BigReaderLock
    {
        int stan = 0;
        Object[] zamki;
        public BigReaderLock(int liczbazamkow)
        {
            zamki = new Object[liczbazamkow];
            for (int i = 0; i < zamki.Length; i++)
            {
                zamki[i] = new Object();
            }
        }
        public void rd_lock()
        {
            if (stan == zamki.Length) throw new InvalidStateBigReaderLock();
            lock (zamki[stan])
            {
                while (stan == -1) Monitor.Wait(zamki[stan]);
                ++stan;
            }
            Console.WriteLine("Czytacz blokuje zamek");
        }
        public void rd_unlock()
        {
            if (stan < 1) throw new InvalidStateBigReaderLock();
            int tmp = stan - 1;
            lock (zamki[tmp])
            {
                if (stan <= 0) throw new InvalidStateBigReaderLock();
                if (--stan == 0) Monitor.Pulse(zamki[stan]);
            }
            Console.WriteLine("Czytacz zwalnia zamek");
        }
        public void wr_lock()
        {
            if (stan > 0) throw new InvalidStateBigReaderLock();
            lock (zamki)
            {
                while (stan != 0) Monitor.Wait(zamki);
                stan = -1;
            }
            Console.WriteLine("Pisarz blokuje zamki");
        }
        public void wr_unlock()
        {
            lock (zamki)
            {
                if (stan != -1) throw new InvalidStateBigReaderLock();
                stan = 0;
                Monitor.PulseAll(zamki);
            }
            Console.WriteLine("Pisarz zwalnia zamki");
        }
    }
    class InvalidStateBigReaderLock : Exception
    {
        public InvalidStateBigReaderLock() { }
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
