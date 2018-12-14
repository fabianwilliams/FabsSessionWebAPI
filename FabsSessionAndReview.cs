using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace fabsesrev
{
    public static class FabsSessionAndReview
    {
        static List<FabsSession> fs = new List<FabsSession>();

        [FunctionName("CreateSession")]
        public static async Task<IActionResult> CreateSession(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "sesrev")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Creating a new Session that Fabian will or have give");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic input = JsonConvert.DeserializeObject<CreateSession>(requestBody);
            
            var sesrev = new FabsSession()
            {
                SessionNumber = input.SessionNumber,
                SessionName = input.SessionName,
                SessionDate = input.SessionDate,
                SessionCity = input.SessionCity,
                SessionRegionState = input.SessionRegionState,
                SessionCountry = input.SessionCountry
            };
            fs.Add(sesrev);

            return name != null
                ? (ActionResult)new OkObjectResult(sesrev)
                : new BadRequestObjectResult("Please pass a valid Session JSON Payload in the request body");
        }

        [FunctionName("GetSessions")]
        public static IActionResult GetSessions(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "sesrev")]HttpRequest req, ILogger log)
        {
            log.LogInformation("Getting all of Fabian Sessions");
            return new OkObjectResult(fs);
        }

        [FunctionName("GetTodoById")]
        public static IActionResult GetTodoById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "sesrev/{id}")]HttpRequest req, 
            ILogger log, string id)
        {
            var sesrev = fs.FirstOrDefault(s => s.Id == id);
            if (sesrev == null)
            {
                return new NotFoundResult();
            }
            return new OkObjectResult(sesrev);
        }

        [FunctionName("UpdateSession")]
        public static async Task<IActionResult> UpdateSession(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "todo/{id}")]HttpRequest req,
            ILogger log, string id)
        {
            var todo = fs.FirstOrDefault(t => t.Id == id);
            if (todo == null)
            {
                return new NotFoundResult();
            }

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var updated = JsonConvert.DeserializeObject<CreateSession>(requestBody);
            /* 
            todo.IsCompleted = updated.IsCompleted;
            if (!string.IsNullOrEmpty(updated.TaskDescription))
            {
                todo.TaskDescription = updated.TaskDescription;
            }
            */
            return new OkObjectResult(todo);
        }

        [FunctionName("DeleteSession")]
        public static IActionResult DeleteSession(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "sesrev/{id}")]HttpRequest req,
            ILogger log, string id)
        {
            var todo = fs.FirstOrDefault(s => s.Id == id);
            if (todo == null)
            {
                return new NotFoundResult();
            }
            fs.Remove(todo);
            return new OkResult();
        }

    }
}
