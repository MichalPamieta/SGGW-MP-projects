using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

/*
 PROJEKT 1 - OBLICZYĆ CAŁKĘ OZNACZONĄ FUNKCJI: f(x) = 3*x^3 + cos(7*x) - ln(2*x) na przedziale [1,40] metodą prostokątów
 Wersja 4 - Jakiś inny wariant. Każdy wątek liczy swoją sumę lokalną i np. stosujemy operację redukcji - jeśli mamy taką operację w naszym języku.
 Co najmniej zaś należy użyć np. zadań zamiast wątków. W języku C# zaleca się użycie TPL, np. Parallel.For() lub Parallel.Invoke().
 */
namespace Wersja_4
{
    class Program
    {
        class Calka
        {
            //private static Object zamek = new Object();
            public double p, k, dx;
            public Calka(double poczatek, double koniec, double dx)
            {
                p = poczatek;
                k = koniec;
                this.dx = dx;
            }
            public void Obliczanie(double x, double y)
            {
                double tmp = 0;
                for (double i = x; i < y; i += dx)
                {
                    tmp += funkcja(i) * dx;
                }
                //lock(zamek)
                lock (this)
                {
                    wynik += tmp;
                }
            }
        }
        static void Calkowanie(Calka c)
        {
            wynik = 0;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Parallel.Invoke(
                () => c.Obliczanie(1, 5),
                () => c.Obliczanie(5, 10),
                () => c.Obliczanie(10, 15),
                () => c.Obliczanie(15, 20),
                () => c.Obliczanie(20, 25),
                () => c.Obliczanie(25, 30),
                () => c.Obliczanie(30, 35),
                () => c.Obliczanie(35, 40)
            );
            sw.Stop();
            Console.WriteLine("\n Wynik: " + wynik);
            Console.WriteLine("\n Czas: " + sw.Elapsed);
            Console.WriteLine("\n Czas (milisekundy): " + sw.ElapsedMilliseconds);
            Console.WriteLine("\n Czas (tiki): " + sw.ElapsedTicks);
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
            Calka calka = new Calka(poczatek, koniec, dx);
            Console.WriteLine("PRÓBA 1");
            for (int i = 1; i < 6; i++)
            {
                Console.WriteLine("TEST " + i);
                Calkowanie(calka);
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
                Calkowanie(calka);
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
            }
            Console.ReadKey();
        }
    }
}
