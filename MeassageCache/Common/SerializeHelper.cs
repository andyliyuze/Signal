
using ServiceStack.Redis.Support;
using System;
using ServiceStack;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.Redis;
namespace MeassageCache.Common
{
    class SerializeHelper
    {

        public static Byte[] SerializeModel<T>(T model)
    {
        var ser = new ObjectSerializer();
                         byte[] buffer = ser.Serialize(model);
                         return buffer;
        
        
        }

        //将实体对象键值对化
        public static Dictionary<string, string> ConvetToKeyValuePairs(object model) {

            Dictionary<string, string> map = new Dictionary<string, string>();
            Type modelType = model.GetType();
            foreach (var property in modelType.GetProperties())  //把所有属性名称加在map的key中
            {
                var val = modelType.GetProperty(property.Name).GetValue(model).ToString();
                map.Add(property.Name, val);
                //新增解决Dictionary不能遍历赋值
            }
            return map;
        }



        public static T ConvetToEntity<T>(Dictionary<string, string> map) where T:new()
        {

           var model=  map.ToJson().FromJson<T>();
           
            //T model = new T();
            //var modelType = model.GetType();
            //foreach (var property in modelType.GetProperties())  //把所有属性名称加在map的key中
            //{
            //    modelType.GetProperty(property.Name).SetValue(model, map[property.Name]);
               
            //    //新增解决Dictionary不能遍历赋值
            //}
            return model;
        }
    }
}
