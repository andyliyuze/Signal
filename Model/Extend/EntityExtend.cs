using Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Extend
{
  public static  class EntityExtend
    {
      public static PrivateMessageViewModel ConvertToViewModel(this PrivateMessage model)
      {



          return new PrivateMessageViewModel { content = model.content, CreateTime = model.CreateTime, RecevierId = model.RecevierId, SenderId = model.SenderId, SenderName = model.SenderName };


      }
      public static List<PrivateMessageViewModel> ConvertToViewList(this List<PrivateMessage> list)
      {

          List<PrivateMessageViewModel> viewlist = new List<PrivateMessageViewModel>();
          foreach (var item in list) {

              viewlist.Add(item.ConvertToViewModel());
          }


          return viewlist;
      }

    }
}
