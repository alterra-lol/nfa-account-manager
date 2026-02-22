using System;
using System.Collections.Generic;
using System.Text;

namespace nfa
{
    public static class VdfSerializer
    {
        public static string Serialize(Dictionary<string, object> data)
        {
            var sb = new StringBuilder();
            SerializeInternal(data, sb, 0);
            return sb.ToString();
        }

        private static void SerializeInternal(Dictionary<string, object> data, StringBuilder sb, int indent)
        {
            foreach (var kvp in data)
            {
                if (kvp.Value is Dictionary<string, object> dict)
                {
                    sb.Append('\t', indent);
                    sb.AppendLine($"\"{kvp.Key}\"");
                    sb.Append('\t', indent);
                    sb.AppendLine("{");
                    SerializeInternal(dict, sb, indent + 1);
                    sb.Append('\t', indent);
                    sb.AppendLine("}");
                }
                else
                {
                    sb.Append('\t', indent);
                    sb.AppendLine($"\"{kvp.Key}\"\t\t\"{kvp.Value}\"");
                }
            }
        }
    }
}
