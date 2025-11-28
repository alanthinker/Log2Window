using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Log2Window.Log
{
    internal class JsonLogMsg
    {
        public string timestamp { get; set; }
        public string level { get; set; }
        public System.Text.Json.Nodes.JsonObject fields { get; set; }
        public string target { get; set; }
        public System.Text.Json.Nodes.JsonArray spans { get; set; }
        public string threadId { get; set; }

        public string ToMessage()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(fields["message"]?.GetValue<string>());

            foreach (var field in fields)
            {
                if (field.Key != "message")
                { sb.AppendLine(field.Key + " = " + field.Value); }
            }

            return sb.ToString();
        }
    }
}
