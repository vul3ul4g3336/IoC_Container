using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLog_DI
{
    public class Runner
    {
        private readonly ILogger<Runner> _logger;

        public Runner(ILogger<Runner> logger)
        {
            this._logger = logger;
        }

        public void DoAction(string name)
        {
            this._logger.LogInformation("Doing hard work! {Action}", name);
        }
    }
}
