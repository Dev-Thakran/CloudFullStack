using CloudFullStackAF.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace CloudFullStackAF;

public class GetAvailableBays
{
    private readonly ILogger<GetAvailableBays> _logger;

    public GetAvailableBays(ILogger<GetAvailableBays> logger)
    {
        _logger = logger;
    }

    [Function("GetAvailableBays")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        if (!int.TryParse(req.Query["lotId"], out int lotId))
        {
            return new BadRequestObjectResult("Please enter a valid lotId.");
        }

        var styles = DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal;
        if (!DateTime.TryParse(req.Query["start"], CultureInfo.InvariantCulture, styles, out DateTime start) ||
            !DateTime.TryParse(req.Query["end"], CultureInfo.InvariantCulture, styles, out DateTime end))
        {
            return new BadRequestObjectResult("Please enter valid start and end times, e.g. 2026-11-06T20:00.");
        }

        if (end <= start)
        {
            return new BadRequestObjectResult("End time must be after start time.");
        }

        // STEP 2: Parameterised SELECT. Active bays at this lot with NO overlapping booking.
        string? connectionString = Environment.GetEnvironmentVariable("ConnectionString");
        List<AvailableBay> results = new List<AvailableBay>();

        using SqlConnection connection = new SqlConnection(connectionString);
        connection.Open();

        string sql = @"
            SELECT b.Id, b.BayCode, b.Type, b.ChargerSpeed, b.HourlyRate
            FROM dbo.Bay b
            WHERE b.ParkingLotId = @LotId
              AND b.IsActive = 1
              AND NOT EXISTS (
                  SELECT 1 FROM dbo.Booking bk
                  WHERE bk.BayId = b.Id
                    AND bk.Status <> 'Cancelled'
                    AND bk.StartUtc < @End
                    AND @Start < bk.EndUtc)
            ORDER BY b.BayCode";

        using SqlCommand command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@LotId", lotId);
        command.Parameters.AddWithValue("@Start", start);
        command.Parameters.AddWithValue("@End", end);

        // STEP 3: Read each row into the list
        using SqlDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            results.Add(new AvailableBay
            {
                Id = (int)reader["Id"],
                BayCode = (string)reader["BayCode"],
                Type = (string)reader["Type"],
                ChargerSpeed = (string)reader["ChargerSpeed"],
                HourlyRate = (decimal)reader["HourlyRate"]
            });
        }

        // STEP 4: Return a JSON array (empty if nothing is free)
        return new OkObjectResult(results);
    }
}