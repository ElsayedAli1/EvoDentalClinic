using System;
using System.Collections.Generic;
using System.Text;

namespace EvoDentalClinic.BLL.Common
{
    public interface IClinicClock
    {
        DateOnly Today { get; }
        TimeOnly TimeNow { get; }
        DateOnly ToClinicDate(DateTime utc);
    }
}
