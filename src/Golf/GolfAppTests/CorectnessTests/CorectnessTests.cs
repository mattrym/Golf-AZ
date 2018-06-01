using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using FluentAssertions;
using GolfApp.Algorithm;
using GolfApp.Structures;
using Xunit;
using Xunit.Abstractions;

namespace GolfAppTests.CorectnessTests
{
    public class CorectnessTests
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly IPlanarMatchingFinder _planarMatchingFinder;

        public CorectnessTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            var balancedHitFinder = new BalancedHitFinder();
            _planarMatchingFinder = new PlanarMatchingFinder(balancedHitFinder);
        }

        [Fact]
        void PlanarMatchingFinder_CheckComplexity()
        {
            var stopwatch = new Stopwatch();
            var executions = new List<Tuple<int, long>>();

            for (int i = 1; i < 500; i++)
            {
                var task = GenerateTask(i);

                stopwatch.Reset();
                stopwatch.Start();

                var matching = _planarMatchingFinder.FindPlanarMatching(task.Balls, task.Holes);
         
                stopwatch.Stop();       
                executions.Add(new Tuple<int, long>(i, stopwatch.ElapsedTicks));
            }

            PrintChartToFile(executions);
        }

        [Fact]
        public void PlanarMatchingFinder_CheckCorectness()
        {
            for (int i = 1; i < 500; i++)
            {
                var task = GenerateTask(i);
                var matching = _planarMatchingFinder.FindPlanarMatching(task.Balls, task.Holes);

                matching.IsPlanar().Should().BeTrue();
            }
        }

        private void PrintChartToFile(List<Tuple<int, long>> executions)
        {
            var chart =
                 new System.Windows.Forms.DataVisualization.Charting.Chart {Size = new System.Drawing.Size(640, 320)};
            chart.ChartAreas.Add("ChartArea1");
            chart.Legends.Add("legend1");

            chart.Series.Add("executions");
            chart.Series["executions"].LegendText = "ExecutionTime";
            chart.Series["executions"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;

            foreach (var execution in executions)
            {
                chart.Series["executions"].Points.AddXY(execution.Item1, execution.Item2);
            }

            chart.SaveImage(@"..\..\TestFiles\ExecutionTimeChart.png", System.Drawing.Imaging.ImageFormat.Png);
        }

        private Task GenerateTask(int size)
        {
            var task = new Task();
            int radius = 100;
            Vector3 pointVector = new Vector3(radius, 0, 0); //the vector you keep rotating

            float angle = (float)Math.PI*2 / (float)(size*2+1);
            var rotationMatrix = Matrix4x4.CreateRotationZ(angle);

            var balls = new List<Ball>();
            var holes = new List<Hole>();

            var sequence = new int[size*2];
            var rand = new Random();

            for (int i = 0; i < size; i++)
            {
                var idx = 0;
                do idx = rand.Next(0, size * 2); while (sequence[idx] == 1);
                sequence[idx] = 1;
            }

            var ballIdx = 0;
            var holeIdx = 0;
            for (int i = 0; i < size*2; i++)
            {
                if (sequence[i] == 0)
                    balls.Add(new Ball(ballIdx++, pointVector.X, pointVector.Y));
                else
                    holes.Add(new Hole(holeIdx++, pointVector.X, pointVector.Y));
                      
                pointVector = Vector3.Transform(pointVector, rotationMatrix);
            }

            task.Balls = balls;
            task.Holes = holes;

            return task;
        }
    }

}
