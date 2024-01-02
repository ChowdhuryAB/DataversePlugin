using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Apg.Shared.Core
{
    /// <summary>   
    /// PluginBase implements IPlugin to encapsulate a PluginPortfolio containing stuff from the service and context, and to log service requests to the Tracing Service.
    /// </summary>
    public abstract class PluginBase : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            using (var portfolio = new PluginPortfolio(serviceProvider))
            {
                var watch = Stopwatch.StartNew();
                try
                {
                    //portfolio.TracingService.TraceContext(portfolio.PluginContext, false, true, true, portfolio.Service);
                    Execute(portfolio);
                }
                catch (Exception e)
                {
                    //portfolio.trace(e.ToString());
                    throw e;
                }
                finally
                {
                    watch.Stop();
                    portfolio.trace("Internal execution time: {0} ms", watch.ElapsedMilliseconds);
                }
            }
        }

        public abstract void Execute(PluginPortfolio portfolio);
    }
}
