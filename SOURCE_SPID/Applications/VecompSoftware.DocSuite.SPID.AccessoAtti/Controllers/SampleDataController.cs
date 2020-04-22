using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using Newtonsoft.Json;

namespace VecompSoftware.SPID.AccessoAtti.Controllers
{
    [Route("api/[controller]")]
    public class SampleDataController : Controller
    {
        private static string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        [HttpGet()]
        public ODataResult Get()
        {
            var rng = new Random();
            Thread.Sleep(2000);
            return new ODataResult()
            {
                results = new List<WorkflowStatus>(){
                    new WorkflowStatus
                    {
                        Id = Guid.NewGuid(),
                        Status = 0,
                        WorkflowDate = DateTimeOffset.Now.AddDays(-10),
                        Name = Summaries[rng.Next(Summaries.Length)]
                    },
                    new WorkflowStatus
                    {
                        Id = Guid.NewGuid(),
                        Status = 8,
                        WorkflowDate = DateTimeOffset.Now.AddDays(-5),
                        Name = Summaries[rng.Next(Summaries.Length)]
                    },
                    new WorkflowStatus
                    {
                        Id = Guid.NewGuid(),
                        Status = 8,
                        WorkflowDate = DateTimeOffset.Now.AddDays(-15),
                        Name = Summaries[rng.Next(Summaries.Length)]
                    },
                    new WorkflowStatus
                    {
                        Id = Guid.NewGuid(),
                        Status = 4,
                        WorkflowDate = DateTimeOffset.Now.AddDays(-1),
                        Name = Summaries[rng.Next(Summaries.Length)]
                    },
                    new WorkflowStatus
                    {
                        Id = Guid.NewGuid(),
                        Status = 0,
                        WorkflowDate = DateTimeOffset.Now.AddDays(-25),
                        Name = Summaries[rng.Next(Summaries.Length)]
                    }
                }
            };
        }

        public class ODataResult
        {
            public ICollection<WorkflowStatus> results;
        }

        public class WorkflowStatus
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public int Status { get; set; }
            public DateTimeOffset WorkflowDate { get; set; }
            public string Model
            {
                get
                {
                    return JsonConvert.SerializeObject(new { Id = Guid.NewGuid() });
                }
            }
        }
    }
}
