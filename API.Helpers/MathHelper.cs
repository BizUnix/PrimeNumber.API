namespace API.Helpers
{
    public static class MathHelper
    {
        public static bool IsPrimeNumber(int inputInt)
        {
            if (inputInt <= 1)
                return false;
            if (inputInt == 2)
                return true;
            if (inputInt % 2 == 0)
                return false;

            var boundary = (int)Math.Floor(Math.Sqrt(inputInt));

            for (int i = 3; i <= boundary; i += 2)
                if (inputInt % i == 0)
                    return false;

            return true;
        }
    }
}