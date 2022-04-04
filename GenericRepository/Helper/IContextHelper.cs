using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Base.DataAccess.GenericRepository.Helper
{
    public interface IContextHelper
    {
        JObject GetUser();
        int GetUserId();
        DateTime GetUTCDateTime();
        bool IsSkippingBaseProperties(string entityName);

    }
}
