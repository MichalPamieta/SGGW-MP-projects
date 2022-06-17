using System;
using System.Linq;
using System.Threading;

namespace Pamięta_Michał_196099_Projekt2_v1
{
	//wersja z semaforami w funkcji Czytaj(); jest stół / kelner;
	//filozof myśli, siada do stołu, bierze widelce, zaczyna jeść, w trakcie jedzenia bierze książkę i czyta, kończy czytać, kończy jeść i wstaje od stołu
	public class Filozof
	{
		int ktory, lewy, prawy;
		bool[] przeczytane;
		SemaphoreSlim[] widelce, ksiazki;
		SemaphoreSlim stol;
		Random rng = new Random();

		public Filozof(int ktory, SemaphoreSlim[] widelce, SemaphoreSlim[] ksiazki, SemaphoreSlim stol)
		{
			this.ktory = ktory;
			this.widelce = widelce;
			this.ksiazki = ksiazki;
			this.stol = stol;
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
			Czytaj();
			Console.WriteLine("Filozof " + ktory + " skończył jeść!");
		}
		void Czytaj()
		{
			int k = rng.Next(0, ksiazki.Length);
			while (przeczytane[k] == true) k = rng.Next(0, ksiazki.Length);
			przeczytane[k] = true;
			ksiazki[k].Wait();
			Console.WriteLine("Filozof " + ktory + " zaczyna czytać książkę " + k);
			//Thread.Sleep(rng.Next(500, 1000));
			Console.WriteLine("Filozof " + ktory + " skończył czytać książkę " + k);
			ksiazki[k].Release();
		}
		public void Ucztuj()
		{
			while (przeczytane.Contains(false))
			{
				Mysl();
				stol.Wait();
				lewy = rng.Next(0, widelce.Length);
				prawy = (lewy + 1) % widelce.Length;
				widelce[lewy].Wait();
				widelce[prawy].Wait();
				Jedz();
				widelce[prawy].Release();
				widelce[lewy].Release();
				stol.Release();
			}
		}
	}
	class Program
	{
		static void Main(string[] args)
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
			SemaphoreSlim stol = new SemaphoreSlim(ileFilozofow - 1, ileFilozofow - 1);
			Filozof[] filozofowie = new Filozof[ileFilozofow];
			Thread[] watki = new Thread[ileFilozofow];
			for (int i = 0; i < ileFilozofow; ++i)
			{
				filozofowie[i] = new Filozof(i, widelce, ksiazki, stol);
				watki[i] = new Thread(filozofowie[i].Ucztuj);
			}
			foreach (var watek in watki) watek.Start();
			foreach (var watek in watki) watek.Join();
			Console.WriteLine("KONIEC uczty");
			Console.ReadKey();
		}
	}
}
