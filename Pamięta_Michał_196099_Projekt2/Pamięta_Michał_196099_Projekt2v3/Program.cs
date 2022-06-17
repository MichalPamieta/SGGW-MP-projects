using System;
using System.Linq;
using System.Threading;

namespace Pamięta_Michał_196099_Projekt2v3
{
	//wersja z monitorami
	public class Filozof
	{
		int num;
		bool[] ksiazki_czytane;
		Object[] widelce;
		Object[] ksiazki;
		int lewy, prawy;
		Random rng = new Random();

		public Filozof(int num_, Object[] widelce_, Object[] ksiazki_)
		{
			num = num_;
			widelce = widelce_;
			ksiazki = ksiazki_;
			ksiazki_czytane = new bool[ksiazki.Length];
		}

		void Mysl()
		{
			Console.WriteLine($"Filozof {num} myśli...");
			//Thread.Sleep(rng.Next(500, 1000));
			Console.WriteLine($"Filozof {num} zgłodniał!");
		}

		void Jedz()
		{
			Console.WriteLine($"Filozof {num} zaczyna jeść...");
			//Thread.Sleep(rng.Next(500, 1000));
			Console.WriteLine($"Filozof {num} skończył jeść!");
		}
		void Czytaj()
		{
			Console.WriteLine($"Filozof {num} zaczyna czytać...");
			//Thread.Sleep(rng.Next(500, 1000));
			Console.WriteLine($"Filozof {num} skończył czytać!");
		}
		public void Dzialanie()
		{
			while (ksiazki_czytane.Contains(false))
			{
				Mysl();
				lewy = rng.Next(0, widelce.Length);
				prawy = (lewy + 1) % widelce.Length;
				int i = rng.Next(0, ksiazki.Length);
				while (ksiazki_czytane[i] == true)
				{
					i = rng.Next(0, ksiazki.Length);
				}
				ksiazki_czytane[i] = true;
				Monitor.Enter(ksiazki[i]);
				if (lewy == widelce.Length - 1)
				{
					Monitor.Enter(widelce[prawy]);
					Monitor.Enter(widelce[lewy]);
				}
				else
				{
					Monitor.Enter(widelce[lewy]);
					Monitor.Enter(widelce[prawy]);
				}
				Jedz();
				Monitor.Exit(widelce[prawy]);
				Monitor.Exit(widelce[lewy]);
				Czytaj();
				Monitor.Exit(ksiazki[i]);
			}
		}
	}
	class Program
    {
        static void Main(string[] args)
        {
			int licz_fil = 5;
			int licz_ksiazek = 6;
			Object[] widelce = new Object[licz_fil];
			for (int i = 0; i < licz_fil; i++)
			{
				widelce[i] = new Object();
			}
			Object[] ksiazki = new Object[licz_ksiazek];
			for (int i = 0; i < licz_ksiazek; i++)
			{
				ksiazki[i] = new Object();
			}
			Object stol = new Object();
			Filozof[] filozofowie = new Filozof[licz_fil];
			Thread[] watki = new Thread[licz_fil];
			for (int i = 0; i < licz_fil; i++)
			{
				filozofowie[i] = new Filozof(i, widelce, ksiazki);
				watki[i] = new Thread(filozofowie[i].Dzialanie);
			}
			foreach (var watek in watki) watek.Start();
			foreach (var watek in watki) watek.Join();
            Console.WriteLine("KONIEC");
			Console.ReadKey();
		}
    }
}
