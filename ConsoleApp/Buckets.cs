using System;

namespace Root
{
    public class Rootobject
    {
        public string query { get; set; }
        public Topscoringintent topScoringIntent { get; set; }
        public Intent[] intents { get; set; }
        public Entity[] entities { get; set; }
        public Sentimentanalysis sentimentAnalysis { get; set; }

        public string[] getFeatures()
        {
            string[] features = new string[14];
            for (int i = 0; i < 14; i++)
            {
                features[i] = "-";
            }

            if (this.topScoringIntent == null)
            {
                return features;
            }

            //Query
            features[0] = this.query;
            string intent = this.topScoringIntent.intent;
            //Offer Type
            features[1] = intent;
            string percent = "";
            for (int i = 0; i < this.entities.Length; i++)
            {
                string type = this.entities[i].type;
                string entity = this.entities[i].entity;
                switch (type)
                {
                    //Upto Offer
                    case "Offer::upto":
                        features[1] = "Upto" + intent;
                        break;
                    //Atleast Offer
                    case "Offer::atleast":
                        features[1] = "Atleast" + intent;
                        break;
                    //Percentage value
                    case "Percent":
                        percent = entity;
                        if (percent != "")
                            features[2] = percent.Substring(0, percent.Length - 1);
                        break;
                    //Amount value with currency
                    case "Amount":
                        if (entity[0] == 'r')
                        {
                            features[2] = entity.Substring(4).Trim();
                            features[3] = "rs";
                        }
                        else
                        {
                            features[2] = entity.Substring(1).Trim();
                            features[3] = entity.Substring(0, 1);
                        }
                        break;
                    case "Value":
                        features[2] = entity;
                        break;
                    //Category of the offer
                    case "Category":
                        features[4] = entity;
                        break;
                    //Min purchase needed for offer to be applicable			
                    case "MinimumPurchase":
                        features[5] = entity;
                        break;
                    //Type of customer for which offer is applicable
                    case "CustomerType":
                        features[6] = entity;
                        break;
                    //Payment method on which offer is applicable
                    case "Payment":
                        features[7] = entity;
                        break;
                    //Type of registriation required for the offer 
                    case "Registration":
                        features[8] = entity;
                        break;
                    //Min number of items needed for offer to be applicable
                    case "MinNumOfItems":
                        features[9] = entity;
                        break;
                    case "Gift":
                        features[10] = entity;
                        break;
                    //Occation for which offer belongs to
                    case "Occasion":
                        features[11] = entity;
                        break;
                    //Store specifies wheather offer is applicable if puchase is made online or in-store or both
                    case "Store":
                        features[12] = "Online,Instore";
                        break;
                    case "Store::instore":
                        features[12] = "Instore";
                        break;
                    case "Store::online":
                        features[12] = "Online";
                        break;
                    //Indicates type of refferal needed for the offer to be applicable	
                    case "Referral":
                        features[13] = entity;
                        break;
                    default:
                        break;
                }
            }
            return features;

        }
    }

    public class Topscoringintent
    {
        public string intent { get; set; }
        public float score { get; set; }
    }

    public class Sentimentanalysis
    {
        public string label { get; set; }
        public float score { get; set; }
    }

    public class Intent
    {
        public string intent { get; set; }
        public float score { get; set; }
    }

    public class Entity
    {
        public string entity { get; set; }
        public string type { get; set; }
        public int startIndex { get; set; }
        public int endIndex { get; set; }
        public float score { get; set; }
    }

}
