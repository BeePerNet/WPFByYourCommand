using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace WPFByYourCommand.Commands
{
    public class DirectCommand : IMenuCommand, INotifyPropertyChanged
    {
        public DirectCommand() { }

        /// <summary>
        /// Name - Declared time Name of the property/field where it is
        ///              defined, for serialization/debug purposes only.
        ///     Ex: public static RoutedCommand New  { get { new RoutedCommand("New", .... ) } }
        ///          public static RoutedCommand New = new RoutedCommand("New", ... ) ;
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
        }

        /// <summary>
        ///     Owning type of the property
        /// </summary>
        public Type OwnerType
        {
            get
            {
                return _ownerType;
            }
        }

        private string _name;
        private Type _ownerType;
        private InputGestureCollection _inputGestureCollection;

        private KeyGesture _keyGesture;
        public KeyGesture KeyGesture { get => _keyGesture; set => SetProperty(ref _keyGesture, value); }

        private bool _useDisablingImage = true;
        public bool UseDisablingImage { get => _useDisablingImage; set => SetProperty(ref _useDisablingImage, value); }

        private object _Icon;
        public object Icon { get => _Icon; set => SetProperty(ref _Icon, value); }

        private string _text;
        public string Text { get => _text; set => SetProperty(ref _text, value); }

        private string _tag;
        public string Tag { get => _tag; set => SetProperty(ref _tag, value); }

        public DirectCommand(string name, string text, string iconSource, Type ownerType,
            Action<DirectCommand, object> execute, bool keepTargetAlive = false, params InputGesture[] gestures) :
            this(name, text, iconSource, ownerType,
            execute, null, keepTargetAlive, gestures)
        { }


        public DirectCommand(string name, string text, string iconSource, Type ownerType,
            Action<DirectCommand, object> execute, Func<DirectCommand, object, bool> canExecute, bool keepTargetAlive = false, params InputGesture[] gestures)
        {
            if (execute != null)
            {
                _execute = new WeakAction<DirectCommand, object>(execute, keepTargetAlive);
            }

            if (canExecute != null)
            {
                _canExecute = new WeakFunc<DirectCommand, object, bool>(canExecute, keepTargetAlive);
            }

            _name = name;
            _ownerType = ownerType;
            _inputGestureCollection = new InputGestureCollection(gestures);

            this._text = text;
            this._Icon = iconSource;
            this.KeyGesture = gestures.OfType<KeyGesture>().FirstOrDefault();
        }





        //The interface only includes this evennt
        public event PropertyChangedEventHandler PropertyChanged;

        //Common implementations of SetProperty
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName]string name = null)
        {
            bool propertyChanged = false;

            //If we have a different value, do stuff
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                OnPropertyChanged(name);
                propertyChanged = true;
            }

            return propertyChanged;
        }

        //The C#6 version of the common implementation
        protected void OnPropertyChanged([CallerMemberName]string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


        public void FillCommandSource(ICommandSource commandSource)
        {
            CommandUtils.FillCommandSource(this, commandSource);
        }

        public void UnFillCommandSource(ICommandSource commandSource)
        {
            CommandUtils.UnFillCommandSource(this, commandSource);
        }

        private readonly WeakAction<DirectCommand, object> _execute;

        private readonly WeakFunc<DirectCommand, object, bool> _canExecute;



#if SILVERLIGHT
        /// <summary>
        /// Occurs when changes occur that affect whether the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;
#elif NETFX_CORE
        /// <summary>
        /// Occurs when changes occur that affect whether the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;
#elif XAMARIN
        /// <summary>
        /// Occurs when changes occur that affect whether the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;
#else
        private EventHandler _requerySuggestedLocal;
#endif


        /// <summary>
        /// Occurs when changes occur that affect whether the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (_canExecute != null)
                {
                    // add event handler to local handler backing field in a thread safe manner
                    EventHandler handler2;
                    EventHandler canExecuteChanged = _requerySuggestedLocal;

                    do
                    {
                        handler2 = canExecuteChanged;
                        EventHandler handler3 = (EventHandler)Delegate.Combine(handler2, value);
                        canExecuteChanged = System.Threading.Interlocked.CompareExchange<EventHandler>(
                            ref _requerySuggestedLocal,
                            handler3,
                            handler2);
                    }
                    while (canExecuteChanged != handler2);

                    CommandManager.RequerySuggested += value;
                }
            }

            remove
            {
                if (_canExecute != null)
                {
                    // removes an event handler from local backing field in a thread safe manner
                    EventHandler handler2;
                    EventHandler canExecuteChanged = this._requerySuggestedLocal;

                    do
                    {
                        handler2 = canExecuteChanged;
                        EventHandler handler3 = (EventHandler)Delegate.Remove(handler2, value);
                        canExecuteChanged = System.Threading.Interlocked.CompareExchange<EventHandler>(
                            ref this._requerySuggestedLocal,
                            handler3,
                            handler2);
                    }
                    while (canExecuteChanged != handler2);

                    CommandManager.RequerySuggested -= value;
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="CanExecuteChanged" /> event.
        /// </summary>
        [SuppressMessage(
            "Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "The this keyword is used in the Silverlight version")]
        [SuppressMessage(
            "Microsoft.Design",
            "CA1030:UseEventsWhereAppropriate",
            Justification = "This cannot be an event")]
        public void RaiseCanExecuteChanged()
        {
#if SILVERLIGHT
            var handler = CanExecuteChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
#elif NETFX_CORE
            var handler = CanExecuteChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
#elif XAMARIN
            var handler = CanExecuteChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
#else
            CommandManager.InvalidateRequerySuggested();
#endif
        }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">This parameter will always be ignored.</param>
        /// <returns>true if this command can be executed; otherwise, false.</returns>
        public virtual bool CanExecute(object parameter)
        {
            return _canExecute == null
                || (_canExecute.IsStatic || _canExecute.IsAlive)
                    && _canExecute.Execute(this, parameter);
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked. 
        /// </summary>
        /// <param name="parameter">This parameter will always be ignored.</param>
        public virtual void Execute(object parameter)
        {
            if (CanExecute(parameter)
                && _execute != null
                && (_execute.IsStatic || _execute.IsAlive))
            {
                _execute.Execute(this, parameter);
            }
        }

    }
}
