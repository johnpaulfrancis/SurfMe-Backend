using SurfMe.Data;
using SurfMe.Models.TableSchema;
using System.Text;

namespace SurfMe.Service
{
    public class APILoggerService
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<APILoggerService> _logger;

        public APILoggerService(RequestDelegate next, ILogger<APILoggerService> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context, ApplicationDbContext dbContext)
        {
            // Only log /api requests
            if (!context.Request.Path.StartsWithSegments("/api"))
            {
                await _next(context);
                return;
            }

            // --- Capture Request ---
            context.Request.EnableBuffering();
            var requestBody = "";
            using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true))
            {
                requestBody = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;
            }

            var originalResponseBody = context.Response.Body;

            using (var newResponseBody = new MemoryStream())
            {
                context.Response.Body = newResponseBody;

                await _next(context); // Call the next middleware

                // --- Capture Response ---
                newResponseBody.Seek(0, SeekOrigin.Begin);
                var responseBody = await new StreamReader(newResponseBody).ReadToEndAsync();
                newResponseBody.Seek(0, SeekOrigin.Begin);
                await newResponseBody.CopyToAsync(originalResponseBody);

                // Determine if request was successful
                bool isSuccess = context.Response.StatusCode >= 200 && context.Response.StatusCode < 300;

                // Get IP Address
                string ipAddress = context.Connection.RemoteIpAddress?.ToString();

                // --- Save to DB ---
                var log = new APILoggerTableModel
                {
                    Path = context.Request.Path,
                    Method = context.Request.Method,
                    Request = requestBody,
                    Response = responseBody,
                    StatusCode = context.Response.StatusCode,
                    IpAddress = ipAddress,
                    LogDateInUTC = DateTime.UtcNow,
                    IsSuccess = isSuccess
                };

                dbContext.Tbl_APILogger.Add(log);
                await dbContext.SaveChangesAsync();

                _logger.LogInformation($"Logged API Request: {log.Path}");
            }
        }
    }
}
