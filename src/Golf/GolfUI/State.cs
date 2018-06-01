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
                new List<GolfApp.Structures.Ball>(),
                new List<GolfApp.Structures.Hole>()
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
            var ballsMaxX = (Task.Balls.Count > 0)? Task.Balls.Max(x => x.X) : 0;
            var holesMaxX = (Task.Holes.Count > 0) ? Task.Holes.Max(x => x.X) : 0;
            PointMaxX = ballsMaxX < holesMaxX ? holesMaxX : ballsMaxX;
            NotifyPropertyChanged("PointMaxX");

            var ballsMinX = (Task.Balls.Count > 0) ? Task.Balls.Min(x => x.X) : 0;
            var holesMinX = (Task.Holes.Count > 0) ? Task.Holes.Min(x => x.X) : 0;
            PointMinX = ballsMinX > holesMinX ? holesMinX : ballsMinX;
            NotifyPropertyChanged("PointMinX");

            var ballsMaxY = (Task.Balls.Count > 0) ? Task.Balls.Max(x => x.Y) : 0;
            var holesMaxY = (Task.Holes.Count > 0) ? Task.Holes.Max(x => x.Y) : 0;
            PointMaxY = ballsMaxY < holesMaxY ? holesMaxY : ballsMaxY;
            NotifyPropertyChanged("PointMaxY");

            var ballsMinY = (Task.Balls.Count > 0) ? Task.Balls.Min(x => x.Y) : 0;
            var holesMinY = (Task.Holes.Count > 0) ? Task.Holes.Min(x => x.Y) : 0;
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
                var maxId = (Balls.Count > 0) ? Balls.OrderByDescending(x => x.Id).First().Id : -1;
                var ball = new Ball(maxId+1, NewPointXValue, NewPointYValue);
                Task.Balls.Add(ball);
                Balls.Add(ball);
                NotifyPropertyChanged("Balls");
            }
            else
            {
                var maxId = (Holes.Count > 0) ? Holes.OrderByDescending(x => x.Id).First().Id : -1;
                var hole = new Hole(maxId+1,NewPointXValue, NewPointYValue);
                Task.Holes.Add(hole);
                Holes.Add(hole);
                NotifyPropertyChanged("Holes");
            }

            UpdateBounds();
        }
    }
}
