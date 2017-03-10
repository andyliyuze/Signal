using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalRChat
{
    public class Test
    {
        private static string ScaleAnalysis1 = "SumScore#总分#0";
        private static string ScaleAnalysis2 = "SumScore|FeelScore|ContactScore|ExerciseScore|LanguageScore|SelfCareScore#";
        public static void GetAttribute(int shopcode)
        {
            Type t = Type.GetType("test");
            var value = t.GetProperty("ScaleAnalysis" + shopcode);



        }                           
    }
}