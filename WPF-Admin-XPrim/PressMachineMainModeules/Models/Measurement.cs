using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PressMachineMainModeules.Models
{
    public class Measurement : ObservableObject
    {
        private int _id;
        /// <summary>
        /// ID
        /// </summary>
        public int Id
        {
            get { return _id; }
            set => SetProperty(ref _id, value);
        }

        private DateTime _timeStamp;
        /// <summary>
        /// Time stamp
        /// </summary>
        public DateTime TimeStamp
        {
            get { return _timeStamp; }
            set => SetProperty(ref _timeStamp, value);
        }

        private float _time;
        /// <summary>
        /// Time
        /// </summary>
        public float Time
        {
            get { return _time; }
            set => SetProperty(ref _time, value);
        }

        private float _position;
        /// <summary>
        /// Position
        /// </summary>
        public float Position
        {
            get { return _position; }
            set => SetProperty(ref _position, value);
        }

        private float _pressure;
        /// <summary>
        /// Pressure
        /// </summary>
        public float Pressure
        {
            get { return _pressure; }
            set => SetProperty(ref _pressure, value);
        }

        public Measurement GetCopy()
        {
            return new Measurement
            {
                Id = this.Id,
                TimeStamp = this.TimeStamp,
                Time = this.Time,
                Position = this.Position,
                Pressure = this.Pressure
            };
        }
    }
}
