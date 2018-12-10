using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USDConfigurationMigration.Models
{
    public class ImportResult
    {

        public ImportResult()
        {
            StartedOn = DateTime.Now;
        }

        public int TotalProcessed { get; set; }
        public int SuccessCount { get; set; }
        public int FailureCount { get; set; }

        public int CreateCount { get; set; }

        public int UpdateCount { get; set; }

        public int AssociateCount { get; set; }

        public int DisassociateCount { get; set; }

        public List<string> Errors { get; set; }

        public DateTime StartedOn { get; set; }

        public DateTime EndedOn { get; set; }

        public string EntityLogicalName { get; set; }
    }
}
