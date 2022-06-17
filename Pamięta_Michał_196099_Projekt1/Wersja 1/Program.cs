using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

/*
 PROJEKT 1 - OBLICZYĆ CAŁKĘ OZNACZONĄ FUNKCJI: f(x) = 3*x^3 + cos(7*x) - ln(2*x) na przedziale [1,40] metodą prostokątów
 Wersja 1 - Wszystkie wątki sumują wynik w tej samej współdzielonej zmiennej - bez synchronizacji (jest to, oczywiście, wersja niepoprawna).
 Wartość przybliżona obliczonego wyrażenia (z kalkulatora): 1919863,512825219
 */
namespace Wersja_1
{
    class Program
    {
        class Calka
        {
            public double p, k, dx;
            public Calka(double poczatek, double koniec, double dx)
            {
                p = poczatek;
                k = koniec;
                this.dx = dx;
            }
            public void Obliczanie(double x, double y)
            {
                for (double i = x; i < y; i+=dx)
                {
                    wynik += funkcja(x + i * dx);
                }
            }
        }
        static void Calkowanie(Calka c, int w)
        {
            wynik = 0;
            Thread[] t = new Thread[w];
            double tmp = (c.k - c.p) / (w * 1.0);
            for (int i = 0; i < w; i++) t[i] = new Thread(() => c.Obliczanie(tmp*i+1, tmp*(i+1)+1));
            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < w; i++) 
                t[i].Start();
            for (int i = 0; i < w; i++)
                t[i].Join();
            sw.Stop();
            Console.WriteLine("\n Wynik: " + wynik);
            Console.WriteLine("\n Czas: " + sw.Elapsed);
            Console.WriteLine("\n Czas (milisekundy): " + sw.ElapsedMilliseconds);
            Console.WriteLine("\n Tiki: " + sw.ElapsedTicks);
        }
        static double funkcja(double x)
        {
            return 3 * Math.Pow(x, 3) + Math.Cos(7 * x) - Math.Log(2 * x);
        }
        static double wynik = 0;
        static void Main(string[] args)
        {
            double poczatek = 1;
            double koniec = 40;
            double dx = 1e-5;
            int watki = 8;
            Calka calka = new Calka(poczatek, koniec, dx);
            Console.WriteLine("PRÓBA 1");
            for (int i = 1; i < 6; i++)
            {
                Console.WriteLine("TEST " + i);
                Calkowanie(calka, watki);
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
            }
            Console.WriteLine("PRÓBA 2");
            dx = 1e-7;
            calka.dx = dx;
            for (int i = 1; i < 6; i++)
            {
                Console.WriteLine("TEST " + i);
                Calkowanie(calka, watki);
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
            }
            Console.ReadKey();
        }
    }
}
