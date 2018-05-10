using System;
using GolfApp.Algorithm.Impl;
using GolfApp.Input;
using GolfApp.Output;

namespace GolfApp
{
    public static class Program
    {   
        public const double FloatEps = 10e-6;
        
        static void Main(string[] args)
        {
            var balancedHitFinder = new BalancedHitFinderImpl();
            var planarMatchingFinder = new PlanarMatchingFinderImpl(balancedHitFinder);

            var taskParser = new TextFileTaskParser(args[0]);
            var matchingPrinter = new ConsoleMatchingPrinter();

            var task = taskParser.Parse();
            var matching = planarMatchingFinder.FindPlanarMatching(task.Balls, task.Holes);

            matchingPrinter.Print(matching);
        }
    }
}