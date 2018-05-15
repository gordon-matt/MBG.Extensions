using MBG.Extensions.Core;

namespace MBG.Extensions.Math
{
    //TODO: Test
    public static class Int32Extensions
    {
        public static int LowestCommonMultiple(int lhs, int rhs)
        {
            int max = lhs > rhs ? rhs : lhs;

            int lcm = -1;
            for (int i = 2; i <= max; i++)
            {
                if (lhs.IsMultipleOf(i) && rhs.IsMultipleOf(i))
                {
                    lcm = i;
                    break;
                }
            }

            return lcm;
        }

        public static int GreatestCommonDivisor(int lhs, int rhs)
        {
            int i = 0;
            while (true)
            {
                i = lhs % rhs;
                if (i == 0)
                {
                    return rhs;
                }

                lhs = rhs;
                rhs = i;
            }
        }
    }
}