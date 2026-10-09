namespace OdontoSoft.Web.Helpers
{
    public static class AutoNumericHelper
    {
        public static string IntegerSettings()
        {
            return "{ \"aSep\": \".\", \"aDec\": \",\", \"mDec\": 0, \"aPad\": false }";
        }

        public static string IntegerSettings(int? min, int? max)
        {
            return "{ \"aSep\": \".\", \"aDec\": \",\", \"mDec\": 0, \"aPad\": false " + (min.HasValue ? ", \"vMin\": " + min.Value.ToString().Replace(",", ".") : string.Empty) + (max.HasValue ? ", \"vMax\": " + max.Value.ToString().Replace(",", ".") : string.Empty) + " }";
        }

        public static string NullableIntegerSettings()
        {
            return "{ \"aSep\": \".\", \"aDec\": \",\", \"mDec\": 0, \"aPad\": false, \"isNullable\": true }";
        }

        public static string NullableIntegerSettings(int? min, int? max)
        {
            return "{ \"aSep\": \".\", \"aDec\": \",\", \"mDec\": 0, \"aPad\": false, \"isNullable\": true " + (min.HasValue ? ", \"vMin\": " + min.Value.ToString().Replace(",", ".") : string.Empty) + (max.HasValue ? ", \"vMax\": " + max.Value.ToString().Replace(",", ".") : string.Empty) + " }";
        }

        public static string DecimalSettings(int decimalPlaces)
        {
            return "{ \"aSep\": \".\", \"aDec\": \",\", \"mDec\": " + decimalPlaces + ", \"aPad\": false }";
        }

        public static string NullableDecimalSettings(int decimalPlaces)
        {
            return "{ \"aSep\": \".\", \"aDec\": \",\", \"mDec\": " + decimalPlaces + ", \"aPad\": false, \"isNullable\": true }";
        }
    }
}