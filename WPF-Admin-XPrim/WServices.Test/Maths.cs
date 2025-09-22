namespace WServices.Test {
    public class Maths {

        [Fact]
        public void Test() {
            var result = BitwisePow(2, 5);
        }
        
        public static double BitwisePow(double baseNum, int exponent)
        {
            double result = 1;
            double current = baseNum;
            int n = Math.Abs(exponent);

            while (n > 0)
            {
                if ((n & 1) == 1) // 替代 n % 2 == 1
                    result *= current;
            
                current *= current;
                n >>= 1; // 替代 n /= 2
            }

            return exponent >= 0 ? result : 1 / result;
        }
    }
}