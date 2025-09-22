using HandyControl.Controls;
using PressMachineMainModeules.Utils;
using WPF.Admin.Models;
using WPF.Admin.Service.Logger;
using WPF.Admin.Service.Services;
using WPF.Admin.Themes.Helper;

namespace PressMachineMainModeules.Models {
    public class ParameterBase : BindableBase {
        public string? Max { get; set; }
        public string? Min { get; set; }
        public string Desc { get; set; }

        public string Type { get; set; }

        private bool CheckValue(float value, out string message) {
            var result = CheckValue(value);
            message = result.Item2;
            return result.Item1;
        }

        public (bool, string) CheckValue(float? value = null) {
            try
            {
                var pdValue = value ?? this.Value;
                if (string.IsNullOrEmpty(this.Max) || string.IsNullOrEmpty(this.Min))
                {
                    return (true, "无校验");
                }

                var min = float.Parse(this.Min);
                var max = float.Parse(this.Max);
                if (pdValue > max || pdValue < min)
                {
                    return (false, $"{this.Desc}:值得应该在{Min}~{Max}之间");
                }

                return (true, "OK");
            }
            catch (Exception e)
            {
                XLogGlobal.Logger?.LogError(e.Message, e);
                return (false, this.Desc + e.Message);
            }
        }

        private OpValueType? _ValueType;

        public OpValueType ValueType {
            get { return _ValueType ??= GetValueType(); }
        }

        public OpValueType GetValueType() {
            string type = this.Type ?? "float";
            return OpValueTypeHelper.GetValueType(type);
        }

        public Multiplier Multiplier { get; set; }

        public Calculation Calculation { get; set; }

        public float RealWriteValue() {
            var operation = POperation.Write;
            if (this.Multiplier == Multiplier.None || this.Calculation == Calculation.None)
            {
                return this.Value;
            }

            return operation switch {
                POperation.Write when this.Calculation == Calculation.Division => this.Multiplier switch {
                    Multiplier._10 => this._value / 10,
                    Multiplier._100 => this._value / 100,
                    Multiplier._1000 => this._value / 1000,
                    Multiplier._10000 => this._value / 10000,
                    _ => this._value
                },
                POperation.Write when this.Calculation == Calculation.Multiplication => this.Multiplier switch {
                    Multiplier._10 => this._value * 10,
                    Multiplier._100 => this._value * 100,
                    Multiplier._1000 => this._value * 1000,
                    Multiplier._10000 => this._value * 10000,
                    _ => this._value
                },
                POperation.Read when this.Calculation == Calculation.Division => this.Multiplier switch {
                    Multiplier._10 => this._value * 10,
                    Multiplier._100 => this._value * 100,
                    Multiplier._1000 => this._value * 1000,
                    Multiplier._10000 => this._value * 10000,
                    _ => this._value
                },
                POperation.Read when this.Calculation == Calculation.Multiplication => this.Multiplier switch {
                    Multiplier._10 => this._value / 10,
                    Multiplier._100 => this._value / 100,
                    Multiplier._1000 => this._value / 1000,
                    Multiplier._10000 => this._value / 10000,
                    _ => this._value
                },
                _ => this.Value
            };
        }

        public void RealReadValue(float numValue) {
            var operation = POperation.Read;
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                if (this.Multiplier == Multiplier.None || this.Calculation == Calculation.None)
                {
                    this.Value = numValue;
                    return;
                }

                switch (operation)
                {
                    case POperation.Read when this.Calculation == Calculation.Division: {
                        switch (this.Multiplier)
                        {
                            case Multiplier._10: this.Value = numValue * 10; break;
                            case Multiplier._100: this.Value = numValue * 100; break;
                            case Multiplier._1000: this.Value = numValue * 1000; break;
                            case Multiplier._10000: this.Value = numValue * 10000; break;
                            default: this.Value = numValue; break;
                        }

                        break;
                    }
                    case POperation.Read when this.Calculation == Calculation.Multiplication: {
                        switch (this.Multiplier)
                        {
                            case Multiplier._10: this.Value = numValue / 10; break;
                            case Multiplier._100: this.Value = numValue / 100; break;
                            case Multiplier._1000: this.Value = numValue / 1000; break;
                            case Multiplier._10000: this.Value = numValue / 10000; break;
                            default: this.Value = numValue; break;
                        }

                        break;
                    }
                }
            });
        }

        private float _value = 0f;

        public float Value {
            get => _value;
            set
            {
                if (!CheckValue(value, out var message))
                {
                    WPF.Admin.Service.Services.DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        SnackbarHelper.Show(message);
                    });
                    value = 0;
                    Growl.Warning(message);
                }

                value = (float)Math.Round(value, 2);
                if (SetProperty(ref _value, value))
                {
                    OnPropertyChanged(nameof(Value));
                }
            }
        }
    }
}