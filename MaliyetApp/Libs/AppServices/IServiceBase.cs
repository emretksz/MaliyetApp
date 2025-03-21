using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaliyetApp.Libs.AppServices
{
    public interface IServiceBase<T>
    {
      public void Insert(T entity);
      public void Update(T entity);
      public void Delete(T entity);
      T Get(long id);
      List<T> GetAll(long id);

    }
}
