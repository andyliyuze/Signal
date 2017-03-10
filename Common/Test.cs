using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
  public  class ScaleConfig
    {
        public  string ScaleAnalysis1 = "SumScore#总分#0";
        public static string ScaleAnalysis2 = "SumScore|EatingBehaviorScore|TheToiletScore|WashAndDressScore|DressScore#" +
                                                "总分|进餐行为|如厕|梳洗|穿衣#0|100|56|54|180";
        public static void GetAttribute(int shopcode)
        {
            ScaleConfig o = new ScaleConfig();
            Type t = typeof(ScaleConfig);
            string filedName = "ScaleAnalysis" + shopcode.ToString();
            string filedValue = t.GetField(filedName).GetValue(o).ToString();
            string[] arr = filedValue.Split('#');
            Dictionary<string, string> attributeDic = new Dictionary<string, string>();
            string[] EnArr = arr[0].Split('|');
            string[] ZnArr = arr[1].Split('|');
            for (int i = 0; i < EnArr.Length; i++)
            {

                attributeDic.Add(EnArr[i], ZnArr[i]);

            }
            string jsonStr = JsonConvert.SerializeObject(attributeDic);
        }
    }
}
