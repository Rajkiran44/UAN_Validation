
using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace ValidateUanFunction
{
    public static class ValidateUan
    {
        [FunctionName("ValidateUan")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]
            HttpRequest req)
        {
            string uan = req.Query["uan"];

            if (string.IsNullOrWhiteSpace(uan))
            {
                return new BadRequestObjectResult("Please provide a UAN number. Example: ?uan=123456789012");
            }

            bool isValid = ValidateUanNumber(uan);

            return new OkObjectResult(new
            {
                uan,
                isValid,
                message = isValid ? "Valid UAN number" : "Invalid UAN number"
            });
        }

        private static bool ValidateUanNumber(string uan)
        {
            // UAN must be 12 digits
            if (uan.Length != 12)
                return false;

            // Must contain only numbers
            if (!long.TryParse(uan, out _))
                return false;

            // Cannot start with 0
            if (uan.StartsWith("0"))
                return false;

            return true;
        }
    }
}
