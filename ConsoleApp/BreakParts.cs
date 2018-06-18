using Strings;
using System;
namespace BreakTitle
{
    public class Queryparts
    {
        public bool isBroken;

        public string[] part;

        public string seperator;

        public Queryparts BreakQuery(string query)
        {
            Messages messages = new Messages();
            Queryparts obj = this;
            int position;

            // If query contains "plus"
            if (query.ToLower().IndexOf(messages.Plus) != -1)
            {
                position = query.ToLower().IndexOf(messages.Plus);
                obj.isBroken = true;
                obj.part = new string[2];
                obj.part[0] = query.Substring(0, position - 1);
                if (query.Length - position < 3)
                {
                    obj.isBroken = false;
                    return obj;
                }
                obj.part[1] = query.Substring(position + messages.Plus.Length).Trim();
                obj.seperator = messages.Plus;
                return obj;
            }
            // If query contains "+"
            else if (query.IndexOf(messages.PlusSign) != -1)
            {
                position = query.IndexOf(messages.PlusSign);
                obj.isBroken = true;
                obj.part = new string[2];
                obj.part[0] = query.Substring(0, position).Trim();
                if (query.Length - position < 3)
                {
                    obj.isBroken = false;
                    return obj;
                }
                obj.part[1] = query.Substring(position + messages.PlusSign.Length).Trim();
                obj.seperator = messages.PlusSign;
                return obj;
            }
            // If query contains ","
            else if (query.IndexOf(messages.Comma) != -1)
            {
                obj.isBroken = true;
                obj.seperator = messages.Comma;
                int i = 0;
                string q = query;
                int count = q.Length - q.Replace(",", "").Length;
                obj.part = new string[count + 1];
                while (query != null && query.IndexOf(messages.Comma) != -1)
                {
                    position = query.IndexOf(messages.Comma);
                    obj.part[i] = query.Substring(0, position).Trim();
                    if (query.Length < 3)
                    {
                        break;
                    }
                    query = query.Substring(position + messages.Comma.Length).Trim();
                    i++;
                }
                obj.part[i] = query;
                return obj;
            }

            obj.isBroken = false;
            return obj;
        }
    }
}