using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apg.Shared.Core
{
    public class TracingService : ITracingService, IDisposable
    {
        private readonly ITracingService _trace;
        private List<string> blockstack = new List<string>();

        /// <summary>
        /// Set this property to True to enable extensive tracing of details regarding queries, entities etc.
        /// Note! This may affect performance!
        /// </summary>
        public bool Verbose { get; set; } = false;

        public TracingService(ITracingService trace)
        {
            _trace = trace;
            if (trace != null)
            {
                trace.Trace(DateTime.Now.ToString("yyyy-MM-dd"));
            }
            this.Trace("*** Enter");
        }
        public void Dispose()
        {
            if (blockstack.Count > 0)
            {
                _trace.Trace("[TracingService] Ending unended blocks - check code consistency!");
                while (blockstack.Count > 0)
                {
                    BlockEnd();
                }
            }
            Trace("*** Exit");
        }

        public void Trace(string format, params object[] args)
        {
            if (_trace != null)
            {
                var indent = new string(' ', blockstack.Count * 2);
                var s = format;
                var arguments = string.Join("  \r\n", args);
                if (args.Length > 0)
                {
                    try
                    {
                        s = string.Format(format, args);
                    }
                    catch (FormatException)
                    {
                        s += $"\r\nTrace Parameters:\r\n {arguments} ";
                    }
                }
                _trace.Trace(DateTime.Now.ToString("HH:mm:ss.fff") + "\t" + indent + s);
            }
        }

        public void TraceRaw(string text)
        {
            _trace.Trace(text);
        }

        internal void BlockBegin(string label)
        {
            Trace($"BEGIN {label}");
            blockstack.Add(label);
        }

        internal void BlockEnd()
        {
            var label = "?";
            var pos = blockstack.Count - 1;
            if (pos >= 0)
            {
                label = blockstack[pos];
                blockstack.RemoveAt(pos);
            }
            Trace($"END {label}");
        }
    }
}
