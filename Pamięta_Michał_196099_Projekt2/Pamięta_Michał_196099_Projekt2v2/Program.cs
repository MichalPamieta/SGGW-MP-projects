using System;
using System.Linq;
using System.Threading;

namespace Pamięta_Michał_196099_Projekt2v2
{
	//wersja z semaforami i całą logiką w jednej funkcji - Ucztuj(); reszta funkcji to same komunikaty; nie ma stołu / kelnera;
	//filozof myśli, bierze książkę oraz widelce, zaczyna jeść i dopiero po zjedzeniu, czyta książkę, a następnie odkłada książkę
	public class Filozof
	{
		int ktory, lewy, prawy;
		bool[] przeczytane;
		SemaphoreSlim[] widelce, ksiazki;
		Random rng = new Random();

		public Filozof(int ktory, SemaphoreSlim[] widelce, SemaphoreSlim[] ksiazki)
		{
			this.ktory = ktory;
			this.widelce = widelce;
			this.ksiazki = ksiazki;
			przeczytane = new bool[ksiazki.Length];
		}

		void Mysl()
		{
			Console.WriteLine("Filozof " + ktory + " myśli...");
			//Thread.Sleep(rng.Next(500, 1000));
			Console.WriteLine("Filozof " + ktory + " zgłodniał!");
		}

		void Jedz()
		{
			Console.WriteLine("Filozof " + ktory + " zaczyna jeść. Używa widelcy: " + lewy + " i " + prawy);
			//Thread.Sleep(rng.Next(500, 1000));
			Console.WriteLine("Filozof " + ktory + " skończył jeść!");
		}
		void Czytaj(int k)
		{
			Console.WriteLine("Filozof " + ktory + " zaczyna czytać książkę " + k);
			//Thread.Sleep(rng.Next(500, 1000));
			Console.WriteLine("Filozof " + ktory + " skończył czytać książkę " + k);
		}
		public void Ucztuj()
		{
			while (przeczytane.Contains(false))
			{
				Mysl();
				lewy = rng.Next(0, widelce.Length);
				prawy = (lewy + 1) % widelce.Length;
				int k = rng.Next(0, ksiazki.Length);
				while (przeczytane[k] == true) k = rng.Next(0, ksiazki.Length);
				przeczytane[k] = true;
				ksiazki[k].Wait();
				if (lewy == widelce.Length - 1)
                {
					widelce[prawy].Wait();
					widelce[lewy].Wait();
                }
                else
                {
					widelce[lewy].Wait();
					widelce[prawy].Wait();
				}
				Jedz();
				widelce[prawy].Release();
				widelce[lewy].Release();
				Czytaj(k);
				ksiazki[k].Release();
			}
		}
	}

	class Program
    {
		static void Main()
		{
			//liczba filozofów uczestniczących w uczcie
			int ileFilozofow = 5;
			SemaphoreSlim[] widelce = new SemaphoreSlim[ileFilozofow];
			for (int i = 0; i < ileFilozofow; ++i)
			{
				widelce[i] = new SemaphoreSlim(1, 1);
			}
			//ilość książek, które każdy ma przeczytać
			int k = 10;
			SemaphoreSlim[] ksiazki = new SemaphoreSlim[k];
			for (int i = 0; i < k; ++i)
			{
				ksiazki[i] = new SemaphoreSlim(1, 1);
			}
			Filozof[] filozofowie = new Filozof[ileFilozofow];
			Thread[] watki = new Thread[ileFilozofow];
			for (int i = 0; i < ileFilozofow; ++i)
			{
				filozofowie[i] = new Filozof(i, widelce, ksiazki);
				watki[i] = new Thread(filozofowie[i].Ucztuj);
			}
			foreach (var watek in watki) watek.Start();
			foreach (var watek in watki) watek.Join();
			Console.WriteLine("KONIEC uczty");
			Console.ReadKey();
		}
	}
}
