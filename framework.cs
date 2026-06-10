[HttpPost]
[ValidateAntiForgeryToken]
public ActionResult Create([Bind(Include = "Id,FirstName,LastName,EmailAddress,DateOfBirth,CarYear,CarMake,CarModel,DUI,SpeedingTickets,CoverageType,Quote")] Insuree insuree)
{
    if (ModelState.IsValid)
    {
        // 1. Start with a base of $50 / month
        decimal baseQuote = 50m;

        // 2. Age Calculations
        int age = DateTime.Today.Year - insuree.DateOfBirth.Year;
        // Adjust age if the birthday hasn't occurred yet this year
        if (insuree.DateOfBirth.Date > DateTime.Today.AddYears(-age)) age--;

        if (age <= 18)
        {
            baseQuote += 100;
        }
        else if (age >= 19 && age <= 25)
        {
            baseQuote += 50;
        }
        else // 26 or older
        {
            baseQuote += 25;
        }

        // 3. Car Year Calculations
        if (insuree.CarYear < 2000)
        {
            baseQuote += 25;
        }
        else if (insuree.CarYear > 2015)
        {
            baseQuote += 25;
        }

        // 4. Car Make & Model Calculations
        if (insuree.CarMake.ToLower() == "porsche")
        {
            baseQuote += 25;
            
            if (insuree.CarModel.ToLower() == "911 carrera")
            {
                baseQuote += 25;
            }
        }

        // 5. Speeding Tickets Calculation ($10 per ticket)
        baseQuote += (insuree.SpeedingTickets * 10);

        // 6. DUI Calculation (Add 25% to the total)
        if (insuree.DUI)
        {
            baseQuote += (baseQuote * 0.25m);
        }

        // 7. Coverage Type Calculation (Add 50% for full coverage)
        if (insuree.CoverageType) // Assuming true represents Full Coverage
        {
            baseQuote += (baseQuote * 0.50m);
        }

        // Assign the final calculated total to the insuree object's Quote property
        insuree.Quote = baseQuote;

        // Save to database
        db.Insurees.Add(insuree);
        db.SaveChanges();
        return RedirectToAction("Index");
    }

    return View(insuree);
}
@* Remove or delete this entire block so users cannot change or see the Quote input field *@
<div class="form-group">
    @Html.LabelFor(model => model.Quote, htmlAttributes: new { @class = "control-label col-md-2" })
    <div class="col-md-10">
        @Html.EditorFor(model => model.Quote, new { htmlAttributes = new { @class = "form-control" } })
        @Html.ValidationMessageFor(model => model.Quote, "", new { @class = "text-danger" })
    </div>
</div>
// GET: Insuree/Admin
public ActionResult Admin()
{
    // Pass the entire list of records to the view
    return View(db.Insurees.ToList());
}
@model IEnumerable<YourProjectName.Models.Insuree> @* Replace YourProjectName with your actual namespace *@

@{
    ViewBag.Title = "Admin Dashboard";
}

<h2>Admin Dashboard - Issued Quotes</h2>

<table class="table table-striped table-hover">
    <thead>
        <tr>
            <th>First Name</th>
            <th>Last Name</th>
            <th>Email Address</th>
            <th>Calculated Monthly Quote</th>
        </tr>
    </thead>
    <tbody>
        @foreach (var item in Model)
        {
            <tr>
                <td>@Html.DisplayFor(modelItem => item.FirstName)</td>
                <td>@Html.DisplayFor(modelItem => item.LastName)</td>
                <td>@Html.DisplayFor(modelItem => item.EmailAddress)</td>
                <td class="text-success font-weight-bold">@string.Format("{0:C}", item.Quote)</td>
            </tr>
        }
    </tbody>
</table>

