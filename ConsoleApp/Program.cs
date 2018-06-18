using System;
using System.Text;
using System.Net.Http;
using System.Web;
using System.IO;
using Newtonsoft.Json;
using Root;
using BreakTitle;
using Strings;

namespace Breaking
{

    class Program
    {
        public static Messages messages = new Messages();
        static void Main(string[] args)
        {
            string line;
            Console.WriteLine("Enter title --> ");
            line = Console.ReadLine();
            MakeRequest(line);
            System.Threading.Thread.Sleep(2000);
            Console.WriteLine(messages.ExitMessage);
            Console.ReadLine();
        }

        public static void MakeRequest(string q)
        {
            q = q.Replace("'", "");
            //q = q.Replace("£", "$");
            Queryparts BreakTitles = new Queryparts();

            BreakTitles = BreakTitles.BreakQuery(q);

            if (BreakTitles.isBroken == true && BreakTitles.seperator != messages.Comma)
            {
                if (BreakTitles.part[1].ToLower().Trim().Equals(messages.FreeShipping))
                {
                    RequestOneToLuis(q, BreakTitles.part[0], messages.OfferTypePlusFreeship);
                }
                else if (BreakTitles.part[1].ToLower().Trim().Contains(messages.FreeShipping))
                {
                    RequestTwoToLuis(q, BreakTitles.part[0], BreakTitles.part[1], messages.RestrictionsIndep);
                }
                else if (BreakTitles.part[1].ToLower().Contains(messages.Cashback))
                {
                    RequestTwoToLuis(q, BreakTitles.part[0], BreakTitles.part[1], messages.RestrictionOfp2Top1);
                }
                else
                {
                    RequestTwoToLuis(q, BreakTitles.part[0], BreakTitles.part[1], messages.RestrictionsIndep);
                }
            }
            else if (BreakTitles.isBroken == true)
            {
                RequestMoreToLuis(q, BreakTitles);
            }
            else
            {
                RequestOneToLuis(q, q, messages.RestrictionsIndep);
            }
        }

        //ID to specify free shipping
        static async void RequestOneToLuis(string query, string part, int id)
        {
            var client = new HttpClient();

            // The request header contains your subscription key
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", messages.subscriptionKey);

            var uri = "https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/" + messages.luisAppId + "?q=" + HttpUtility.UrlEncode(part);

            var response = await client.GetAsync(uri);

            var strResponseContent = await response.Content.ReadAsStringAsync();

            // Display the JSON result from LUIS
            //Console.WriteLine(strResponseContent.ToString());
            string body = strResponseContent.Replace("'", "\"");

            Rootobject title = JsonConvert.DeserializeObject<Rootobject>(body);


            string[] features = title.getFeatures();
            if (id == messages.OfferTypePlusFreeship)
            {
                //Add free shipping to offer type
                features[0] = query;
                features[1] += messages.Comma + messages.FreeShipping;
            }
            SaveToFile(title, features);
        }

        //ID specifies whether restrictions are applied to both title parts or not
        // id = 0 --> Restrictions of part1 applied to part2
        // id = 1 --> Restrictions are applied as predicted by the model
        static async void RequestTwoToLuis(string query, string p1, string p2, int id)
        {
            var client = new HttpClient();

            // The request header contains your subscription key
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", messages.subscriptionKey);

            var uri1 = "https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/" + messages.luisAppId + "?q=" + HttpUtility.UrlEncode(p1);
            var uri2 = "https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/" + messages.luisAppId + "?q=" + HttpUtility.UrlEncode(p2);

            var response1 = await client.GetAsync(uri1);
            var response2 = await client.GetAsync(uri2);

            var strResponseContent1 = await response1.Content.ReadAsStringAsync();
            var strResponseContent2 = await response2.Content.ReadAsStringAsync();

            // Display the JSON result from LUIS
            //Console.WriteLine(strResponseContent.ToString());
            string body1 = strResponseContent1.Replace("'", "\"");
            string body2 = strResponseContent2.Replace("'", "\"");

            Rootobject title1 = JsonConvert.DeserializeObject<Rootobject>(body1);
            Rootobject title2 = JsonConvert.DeserializeObject<Rootobject>(body2);

            string[] features1 = title1.getFeatures();
            string[] features2 = title2.getFeatures();

            if (id == messages.RestrictionOfp2Top1)
            {
                if (features1[1] != "None")
                {
                    for (int i = 4; i < 14; i++)
                    {
                        if (features2[i] == "NA")
                        {
                            features2[i] = features1[i];
                        }
                    }
                    features1[0] = query;
                    features2[0] = "";
                    SaveToFile(title1, features1);
                    SaveToFile(title2, features2);
                }
                else
                {
                    RequestOneToLuis(query, query, messages.RestrictionsIndep);
                }

            }
            else if (id == messages.RestrictionsIndep)
            {
                if (features1[1] == "None" && features2[1] != "None")
                {
                    RequestOneToLuis(query, query, messages.RestrictionsIndep);
                }
                else if (features1[1] != "None" && features2[1] == "None")
                {
                    RequestOneToLuis(query, query, messages.RestrictionsIndep);
                }
                else if (features1[1] != "None" && features2[1] != "None")
                {
                    features1[0] = query;
                    features2[0] = "";
                    SaveToFile(title1, features1);
                    SaveToFile(title2, features2);
                }
                else
                {
                    RequestOneToLuis(query, query, messages.RestrictionsIndep);
                }
            }


        }

        static async void RequestMoreToLuis(string query, Queryparts breakTitle)
        {
            var client = new HttpClient();
            // The request header contains your subscription key
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", messages.subscriptionKey);


            for (int i = 0; i < breakTitle.part.Length; i++)
            {
                var uri = "https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/" + messages.luisAppId + "?q=" + HttpUtility.UrlEncode(breakTitle.part[i]);
                var response = await client.GetAsync(uri);

                var strResponseContent = await response.Content.ReadAsStringAsync();

                // Display the JSON result from LUIS
                //Console.WriteLine(strResponseContent.ToString());
                string body = strResponseContent.Replace("'", "\"");

                Rootobject title = JsonConvert.DeserializeObject<Rootobject>(body);

                string[] features = title.getFeatures();
                if (features[1] == "None" && i < breakTitle.part.Length - 1)
                {
                    breakTitle.part[i + 1] = breakTitle.part[i] + "," + breakTitle.part[i + 1];
                }
                else
                {
                    SaveToFile(title, features);
                }
            }
        }



        public static void SaveToFile(Rootobject title, string[] features)
        {
            string op = features[0];
            for (int j = 1; j < 14; j++)
            {
                op += "\t" + features[j];
            }
            

            //System.Threading.Thread.Sleep(2000);
            Console.WriteLine(op);

        }

    }
}
