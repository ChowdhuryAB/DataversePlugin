using System;
using System.Activities;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Apg.Shared.Core
{
    public abstract class CodeActivityBase : CodeActivity
    {
        protected override void Execute(CodeActivityContext context)
        {
            using (var portfolio = new PluginPortfolio(context))
            {
                var watch = Stopwatch.StartNew();
                try
                {
                    Execute(portfolio);
                }
                catch (Exception e)
                {
                    portfolio.trace("*** Exception ***\n{0}", e);
                    throw;
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
    public partial class PluginPortfolio
    {
        CodeActivityContext CodeActivityContext { get; set; }
        public T GetCodeActivityParameter<T>(InArgument<T> parameter)
        {
            T result = parameter.Get(CodeActivityContext);
            return result;
        }

        public void SetCodeActivityParameter<T>(OutArgument<T> parameter, T value)
        {
            parameter.Set(CodeActivityContext, value);
        }
    }
}
