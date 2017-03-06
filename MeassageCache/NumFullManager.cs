//using ServiceStack.Redis;
//using ServiceStack.Redis.Support;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using Model;
//using DAL;
//using Autofac;
 
////using NServiceKit.Redis;
////using NServiceKit.Redis.Support;
//namespace MeassageCache
//{
//    internal class NumFullManager
//    {
//      public event EventHandler<NumFullEventArgs> Numfull;
//      private EventHandler<NumFullEventArgs> d_NumFull;
    
//      protected virtual void OnNumFull(NumFullEventArgs e) 
      
//      {
//          EventHandler<NumFullEventArgs> temp = Interlocked.CompareExchange(ref d_NumFull, null, null);
//          //如果委托有注册了方法
//          if (temp != null) 
//          {
//          //开始调用委托，执行回调方法
//              temp(this, e);
//          }
//      }



//    }


//  internal sealed class NumFullEventArgs : EventArgs
//    {
//      private readonly int count;
//      public NumFullEventArgs(int n_count)
//    {
//        count = n_count; 
//    }

//      public int Count { get { return count; } }
    
//    }

//  internal class NumObserver 
//  {

//      public void PushMembers(Object sender ,NumFullEventArgs e  ) 
//      {

//          int pushCount = e.Count / 2;
//         // DAL.AddBroadcastMessage()
      
//      }
  
//  }

//  public class PopCache 
//  {
//      //private readonly ILifetimeScope _hubLifetimeScope;
//      //private readonly IBroadcastMessages_DAL _service;
//      //public PopCache(ILifetimeScope lifetimeScope) {

//      //    _hubLifetimeScope = lifetimeScope.BeginLifetimeScope();

//      //    // Resolve dependencies from the hub lifetime scope.
//      //    _service = _hubLifetimeScope.Resolve<IBroadcastMessages_DAL>();
      
      
//      //}

//      public void handler(Object o)
//      {


          
          
         
//          while (true)
//          {
//              using (RedisClient redisClient = new RedisClient("127.0.0.1", 6379))
//              {

//                  Byte[] buffer = redisClient.RPop("BroadCastList");
//                    var ser = new ObjectSerializer();
//                  if (buffer != null)
//                  {
                    
//                      List<BroadcastMessage> list = (List<BroadcastMessage>)ser.Deserialize(buffer);
//                      BroadcastMessages_DAL dal = new BroadcastMessages_DAL();
//                      dal.AddList(list);
                           
                      
//                  }
//                  Byte[] buffer2 = redisClient.RPop("PrivateList");


//                        if (buffer2 != null)
//                        {

//                            List<PrivateMessage> list = (List<PrivateMessage>)ser.Deserialize(buffer2);
//                            PrivateMessages_DAL dal = new PrivateMessages_DAL();
//                            dal.AddList(list);


//                        }
//                  else
//                  {
//                      Thread.Sleep(3000);//如果队列中没有数据，休息避免造成CPU的空转.
//                  }
//              }

//          }


//      }

//      public   void push()
//      {

//          ThreadPool.QueueUserWorkItem(new WaitCallback(handler));


//      }
  
//  }

//}
