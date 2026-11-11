using CloudFullStackAF.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace CloudFullStackAF;

public class GetBookingQuote
{
    private readonly ILogger<GetBookingQuote> _logger;

    public GetBookingQuote(ILogger<GetBookingQuote> logger)
    {
        _logger = logger;
    }

    [Function("GetBookingQuote")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("GetBookingQuote called.");
        
        
        // STEP 1: Read and validate the query-string values
        if (!int.TryParse(req.Query["bayId"], out int bayId))
        {
            return new BadRequestObjectResult("Please enter a valid bayId.");
        }

        if (!decimal.TryParse(req.Query["hours"], out decimal hours) || hours <= 0)
        {
            return new BadRequestObjectResult("Please enter a valid number of hours (more than 0).");
        }

        // STEP 2: Look up the bay's hourly rate with a parameterised SELECT
        string? connectionString = Environment.GetEnvironmentVariable("ConnectionString");

        using SqlConnection connection = new SqlConnection(connectionString);
        connection.Open();

        string sql = @"
            SELECT Id, BayCode, HourlyRate
            FROM dbo.Bay
            WHERE Id = @BayId";

        using SqlCommand command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@BayId", bayId);

        using SqlDataReader reader = command.ExecuteReader();

        if (!reader.Read())
        {
            return new NotFoundObjectResult($"Bay {bayId} was not found.");
        }

        // STEP 3: Calculate the cost and return it as JSON
        decimal rate = (decimal)reader["HourlyRate"];

        BookingQuote quote = new BookingQuote
        {
            BayId = (int)reader["Id"],
            BayCode = (string)reader["BayCode"],
            HourlyRate = rate,
            Hours = hours,
            TotalCost = Math.Round(rate * hours, 2)
        };

        return new OkObjectResult(quote);
    }
}