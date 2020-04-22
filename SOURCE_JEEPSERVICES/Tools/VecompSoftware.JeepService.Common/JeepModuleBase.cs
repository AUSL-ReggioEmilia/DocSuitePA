using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using VecompSoftware.Services.Logging;

namespace VecompSoftware.JeepService.Common
{
    public abstract class JeepModuleBase<T> : InfiniteMarshalByRefObject, IJeepModule, INotifyPropertyChanged
        where T : IJeepParameter, new()
    {
        public event MessageEventArgs.MessageEventHandler Message;
        public event PropertyChangedEventHandler PropertyChanged;

        #region [ Fields ]

        private bool _isWorking;

        private readonly Object _thisLock = new Object();

        private string _name;

        #endregion

        #region [ Properties ]

        public T Parameters { get; set; }

        public bool IsWorking
        {
            get
            {
                lock (_thisLock)
                {
                    return _isWorking;
                }
            }
            set
            {
                lock (_thisLock)
                {
                    _isWorking = value;
                }
            }
        }

        public bool IsBusy
        {
            get { return IsWorking; }
        }

        public DateTime? LastExecution { get; set; }

        public DateTime? NextExecution { get; set; }

        public bool Cancel
        {
            get { return _cancel; }
            set
            {
                _cancel = value;
                RaisePropertyChanged();
            }
        }

        private bool _enabled;
        private bool _cancel;

        public bool Enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                FileLogger.Debug(ThreadedModuleLogger, String.Format("Ricevuto Enabled a {0}", value));
                _enabled = value;
            }
        }

        /// <summary> Nome del modulo. </summary>
        /// <remarks> Determina il nome dell'appender. </remarks>
        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_name))
                {
                    _name = GetType().Name;
                }
                return _name;
            }
            set { _name = value; }
        }

        public string ThreadedModuleLogger
        {
            get { return String.Format("{0}_ThreadedModule", Name); }
        }

        #endregion

        #region [ Constructor ]

        protected JeepModuleBase()
        {
            Parameters = new T();
            Enabled = true;
        }

        #endregion

        #region [ Methods ]

        public virtual void DoWork()
        {
            if (IsWorking)
            {
                return;
            }
            try
            {
                IsWorking = true;
                SingleWork();
            }
            catch (Exception ex)
            {
                FileLogger.Error(Name, "Errore in esecuzione Modulo.", ex);
                SendMessage(string.Format("Problema in esecuzione modulo: {0} - Stacktrace: {1}", ex.Message, FullStacktrace(ex)));
            }
            finally
            {
                FileLogger.Debug(Name, string.Format("Previous Execution {0}.", LastExecution));
                LastExecution = DateTime.Now;
                FileLogger.Info(Name, string.Format("Last Execution {0}.", LastExecution));
                IsWorking = false;
                GC.Collect(GC.MaxGeneration);
            }
        }

        public virtual void Initialize(List<Parameter> parameters)
        {
            // Carico la versione 
            Parameters.Initialize(parameters);
        }

        /// <summary> Metodo di lavoro a thread singolo. </summary>
        /// <remarks> Implementa già tutti i controlli necessari ad essere usato dal jeepservice. </remarks>
        public abstract void SingleWork();

        public void ReceiveMessage(string customer, int version, string message)
        {
            //throw new NotImplementedException();
        }

        public string Version { get; set; }
        
        /// <summary> Inoltra il messaggio al JeepService. </summary>
        /// <param name="message"> Testo del messaggio. </param>
        public virtual void SendMessage(String message)
        {
            var args = new MessageEventArgs(message);
            this.SendMessage(args);
        }

        public virtual void SendMessage(MessageEventArgs args)
        {
            if (this.Message != null)
            {
                this.Message(this, args);
            }
        }

        public virtual string FullStacktrace(Exception ex)
        {
            var currentException = ex;
            var fullStracktrace = String.Empty;
            while (currentException != null)
            {
                fullStracktrace += String.Format("\n\n{0}\n{1}", currentException.Message, currentException.StackTrace);
                currentException = currentException.InnerException;
            }
            return fullStracktrace;
        }

        //-- INotifyPropertyChanged implementation. CallerMemberName serve per capire qual'e' la proprieta' che ha fatto scattare l'evento
        private void RaisePropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        #endregion

    }
}
