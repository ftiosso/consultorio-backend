using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace web_api.Configurations
{
    public class Cache
    {
        public static int GetDefaultCacheTimeInSeconds()
        {
            return int.Parse(System.Configuration.ConfigurationManager.AppSettings["defaultCacheTimeInSeconds"]);
        }
    }
}