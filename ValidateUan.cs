
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace UanValidation
{
    public class ValidateUan
    {
        private readonly ILogger _logger;

        public ValidateUan(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ValidateUan>();
        }

        [Function("ValidateUan")]
        public HttpResponseData Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
        {
            var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
            string uan = query.Get("uan");

            if (string.IsNullOrWhiteSpace(uan))
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                badResponse.WriteString("Please pass ?uan=123456789012");
                return badResponse;
            }

            bool isValid = ValidateUanNumber(uan);

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.WriteAsJsonAsync(new
            {
                uan,
                isValid,
                message = isValid ? "Valid UAN Number" : "Invalid UAN Number"
            });

            return response;
        }

        private bool ValidateUanNumber(string uan)
        {
            // must be 12 digits
            if (uan.Length != 12)
                return false;

            // numbers only
            if (!long.TryParse(uan, out _))
                return false;

            // cannot start with 0
            if (uan.StartsWith("0"))
                return false;

            return true;
        }
    }
}
