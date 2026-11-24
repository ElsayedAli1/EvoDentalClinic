using EvoDentalClinic.BLL.Common;
using EvoDentalClinic.BLL.Services.Interfaces;
using EvoDentalClinic.DAL.Constants;
using EvoDentalClinic.PL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EvoDentalClinic.PL.Controllers;

[Authorize(Roles = Roles.Doctor)]
public class ReportsController : Controller
{
    private readonly IReportService _reports;
    private readonly IClinicClock _clock;

    public ReportsController(IReportService reports, IClinicClock clock)
    {
        _reports = reports;
        _clock = clock;
    }

    public async Task<IActionResult> Income(DateOnly? fromDate, DateOnly? toDate)
    {
        var today = _clock.Today;
        var monthStart = new DateOnly(today.Year, today.Month, 1);
        var prevEnd = monthStart.AddDays(-1);

        var model = new IncomeReportViewModel
        {
            From = fromDate ?? monthStart,
            To = toDate ?? today,
            Today = today,
            MonthStart = monthStart,
            PrevMonthStart = new DateOnly(prevEnd.Year, prevEnd.Month, 1),
            PrevMonthEnd = prevEnd
        };

        var result = await _reports.GetIncomeAsync(model.From, model.To);

        if (result.IsSuccess)
            model.Report = result.Value;
        else
            model.Error = result.Error;

        return View(model);
    }

    public async Task<IActionResult> Outstanding()
        => View(await _reports.GetOutstandingAsync());
}