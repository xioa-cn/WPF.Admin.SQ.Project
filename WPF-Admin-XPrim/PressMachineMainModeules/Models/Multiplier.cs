namespace PressMachineMainModeules.Models {
    public enum Multiplier {
        None,
        _10,
        _100,
        _1000,
        _10000,
    }

    public enum Calculation {
        None,
        /// <summary>
        /// *
        /// </summary>
        Multiplication,
        /// <summary>
        /// /
        /// </summary>
        Division,
    }

    public enum POperation {
        Read,
        Write,
    }
}