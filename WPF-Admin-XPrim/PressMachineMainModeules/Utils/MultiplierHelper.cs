using PressMachineMainModeules.Models;

namespace PressMachineMainModeules.Utils {
    public class MultiplierHelper {
        public static (Calculation, Multiplier) AnalysisCalculation(string str) {
            str = str.Replace(" ", "").Replace("\r", "").Replace("\n", "");
            return str switch {
                "*10" => (Calculation.Multiplication, Multiplier._10),
                "*100" => (Calculation.Multiplication, Multiplier._100),
                "*1000" => (Calculation.Multiplication, Multiplier._1000),
                "*10000" => (Calculation.Multiplication, Multiplier._10000),
                "%10" => (Calculation.Division, Multiplier._10),
                "%100" => (Calculation.Division, Multiplier._100),
                "%1000" => (Calculation.Division, Multiplier._1000),
                "%10000" => (Calculation.Division, Multiplier._10000),
                _ => (Calculation.None, Multiplier.None)
            };
        }
    }
}