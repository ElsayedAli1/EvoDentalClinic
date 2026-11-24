using System;
using System.Collections.Generic;
using System.Text;

namespace EvoDentalClinic.BLL.Common
{
    public static class FdiTooth
    {
        public static bool IsValid(int number)
        {
            var quadrant = number / 10;
            var position = number % 10;

            var permanent = quadrant is >= 1 and <= 4 && position is >= 1 and <= 8;
            var deciduous = quadrant is >= 5 and <= 8 && position is >= 1 and <= 5;

            return permanent || deciduous;
        }
    }
}
