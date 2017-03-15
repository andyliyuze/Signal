using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class UploadHelper
    {

        public static void SaveImageFromBase64(string data, string path)
        {

            byte[] bytes1 = Convert.FromBase64String(data);  //将2进制编码转换为8位无符号整数数组.
           



            //图1
            FileStream fs1 = new FileStream(path, System.IO.FileMode.Create);
            fs1.Write(bytes1, 0, bytes1.Length);
            fs1.Close();
    
    }


        
    }
}
