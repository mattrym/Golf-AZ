using GolfApp.Algorithm;
using GolfApp.Input;

namespace GolfAppTests.CorectnessTests
{
    public class TestCaseChecker
    {
        private IPlanarMatchingFinder MatchingFinder { get; }

        public TestCaseChecker()
        {
        }

        public TestCaseChecker(IPlanarMatchingFinder matchingFinder)
        {
            MatchingFinder = matchingFinder;
        }

        public bool Check(string filePath)
        {
            var taskParser = new TextFileTaskParser(filePath);
            var task = taskParser.Parse();
            var matching = MatchingFinder.FindPlanarMatching(task.Balls, task.Holes);

            return matching.IsPlanar();
        }
    }

    
}