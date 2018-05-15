using GolfApp.Algorithm;
using GolfApp.Algorithm.Impl;
using GolfApp.Structures;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace GolfUI
{
    class State : INotifyPropertyChanged
    {
        private IPlanarMatchingFinder PlanarMatchingFinder { get; }
        private IBalancedHitFinder BalancedHitFinder { get; }

        public GolfApp.Structures.Task Task { get; set; } = new GolfApp.Structures.Task(
                new List<GolfApp.Structures.Ball>
                {
                    new GolfApp.Structures.Ball(0, 10, 10),
                    new GolfApp.Structures.Ball(1, 80, 20),
                    new GolfApp.Structures.Ball(2, 100, 180),
                    new GolfApp.Structures.Ball(3, 0, 140)
                },
                new List<GolfApp.Structures.Hole>
                {
                    new GolfApp.Structures.Hole(0, 90, 80),
                    new GolfApp.Structures.Hole(1, 150, 70),
                    new GolfApp.Structures.Hole(2, 50, 170),
                    new GolfApp.Structures.Hole(3, 160, 160)
                }
            );
        public GolfApp.Structures.Matching Matching { get; private set; } = new GolfApp.Structures.Matching();
        public ICollection<Ball> Balls => Task.Balls;
        public ICollection<Hole> Holes => Task.Holes;
        public IEnumerable<Hit> Hits => Matching;

        public State()
        {
            BalancedHitFinder = new BalancedHitFinderImpl();
            PlanarMatchingFinder = new PlanarMatchingFinderImpl(BalancedHitFinder);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        internal void FindPlanarMatching()
        {
            Matching = PlanarMatchingFinder.FindPlanarMatching(Task.Balls, Task.Holes);
            NotifyPropertyChanged("Hits");
        }

        internal void ClearMatching()
        {
            Matching = new Matching();
        }

    }
}
