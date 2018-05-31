using GolfApp.Algorithm;
using GolfApp.Structures;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace GolfUI
{
    public class RelayCommand : ICommand
    {
        #region Fields 
        readonly Action<object> _execute;
        readonly Predicate<object> _canExecute;
        #endregion // Fields 
        #region Constructors 
        public RelayCommand(Action<object> execute) : this(execute, null) { }
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");
            _execute = execute; _canExecute = canExecute;
        }
        #endregion // Constructors 
        #region ICommand Members 
        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        public void Execute(object parameter) { _execute(parameter); }
        #endregion // ICommand Members 
    }

    class State : INotifyPropertyChanged
    {
        private IPlanarMatchingFinder PlanarMatchingFinder { get; }
        private IBalancedHitFinder BalancedHitFinder { get; }

        public GolfApp.Structures.Task Task { get; private set; } = new GolfApp.Structures.Task(
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
                    new GolfApp.Structures.Hole(2, -50, 170),
                    new GolfApp.Structures.Hole(3, 160, 160)
                }
            );


        private PointType _currentPointType;
        public string CurrentPointType
        {
            get => Enum.GetName(typeof(PointType), _currentPointType);
            set
            {
                _currentPointType = (PointType) Enum.Parse(typeof(PointType), value);
                NotifyPropertyChanged("IsBallTypeSelected");
                NotifyPropertyChanged("IsHoleTypeSelected");
            }
        }

        public IEnumerable<string> PointTypes => Enum.GetNames(typeof(PointType));

        public bool IsBallTypeSelected => _currentPointType == PointType.Ball;
        public bool IsHoleTypeSelected => _currentPointType == PointType.Hole;

        public double NewPointXValue { get; set; }
        public double NewPointYValue { get; set; }

        private ICommand _removeItem;

        public double PointMaxX { get; set; }
        public double PointMaxY { get; set; }
        public double PointMinX { get; set; }
        public double PointMinY { get; set; }

        public ICommand RemoveItem => _removeItem ?? (_removeItem = new RelayCommand(RemoveItemCommand));

        private void RemoveItemCommand(object item)
        {
            if (_currentPointType == PointType.Ball)
            {
                var ball = (Ball)item;
                Balls.Remove(ball);
                Task.Balls.Remove(ball);

                for (var i = ball.Id; i < Task.Balls.Count; i++)
                    --Task.Balls[i].Id;

                NotifyPropertyChanged("Balls");
            }
            else
            {
                var hole = (Hole)item;
                Holes.Remove(hole);
                Task.Holes.Remove(hole);

                for (var i = hole.Id; i < Task.Holes.Count; i++)
                    --Task.Holes[i].Id;

                NotifyPropertyChanged("Holes");
            }

            UpdateBounds();
        }

        public GolfApp.Structures.Matching Matching { get; private set; } = new GolfApp.Structures.Matching();

        public ObservableCollection<Ball> Balls { get; set; }

        public ObservableCollection<Hole> Holes { get; set; }

        public IEnumerable<Hit> Hits => Matching;

        public State()
        {
            BalancedHitFinder = new BalancedHitFinder();
            PlanarMatchingFinder = new PlanarMatchingFinder(BalancedHitFinder);

            Balls = new ObservableCollection<Ball>(Task.Balls);
            Holes = new ObservableCollection<Hole>(Task.Holes);


            _currentPointType = PointType.Ball;
            UpdateBounds();
        }

        private void UpdateBounds()
        {
            var ballsMaxX = Task.Balls.Max(x => x.X);
            var holesMaxX = Task.Holes.Max(x => x.X);
            PointMaxX = ballsMaxX < holesMaxX ? holesMaxX : ballsMaxX;
            NotifyPropertyChanged("PointMaxX");

            var ballsMinX = Task.Balls.Min(x => x.X);
            var holesMinX = Task.Holes.Min(x => x.X);
            PointMinX = ballsMinX > holesMinX ? holesMinX : ballsMinX;
            NotifyPropertyChanged("PointMinX");

            var ballsMaxY = Task.Balls.Max(x => x.Y);
            var holesMaxY = Task.Holes.Max(x => x.Y);
            PointMaxY = ballsMaxY < holesMaxY ? holesMaxY : ballsMaxY;
            NotifyPropertyChanged("PointMaxY");

            var ballsMinY = Task.Balls.Min(x => x.Y);
            var holesMinY = Task.Holes.Min(x => x.Y);
            PointMinY = ballsMinY > holesMinY ? holesMinY : ballsMinY;
            NotifyPropertyChanged("PointMinY");
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        internal void LoadTask(Task task)
        {
            Task = task;
            NotifyPropertyChanged("Balls");
            NotifyPropertyChanged("Holes");
        }

        internal void FindPlanarMatching()
        {
            try
            {
                Matching = PlanarMatchingFinder.FindPlanarMatching(Task.Balls, Task.Holes);
                NotifyPropertyChanged("Hits");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }

        internal void ClearMatching()
        {
            Matching = new Matching();
            NotifyPropertyChanged("Hits");
        }

        public void AddPoint()
        {
            if (Balls.Any(p => Math.Abs(p.X - NewPointXValue) < Double.Epsilon && Math.Abs(p.Y - NewPointYValue) < Double.Epsilon)
                || Holes.Any(p => Math.Abs(p.X - NewPointXValue) < Double.Epsilon && Math.Abs(p.Y - NewPointYValue) < Double.Epsilon))
            {
                MessageBox.Show("Point with given coordinates already exists !");
                return;
            }

            if (_currentPointType == PointType.Ball)
            {
                var maxId = Balls.OrderByDescending(x => x.Id).First().Id;
                var ball = new Ball(maxId+1, NewPointXValue, NewPointYValue);
                Task.Balls.Add(ball);
                Balls.Add(ball);
                NotifyPropertyChanged("Balls");
            }
            else
            {
                var maxId = Holes.OrderByDescending(x => x.Id).First().Id;
                var hole = new Hole(maxId+1,NewPointXValue, NewPointYValue);
                Task.Holes.Add(hole);
                Holes.Add(hole);
                NotifyPropertyChanged("Holes");
            }

            UpdateBounds();
        }
    }
}
