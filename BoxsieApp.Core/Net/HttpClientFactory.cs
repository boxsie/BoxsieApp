using System;
using System.Collections.Generic;
using System.Text;

namespace BoxsieApp.Core.Net
{
    public class HttpClientFactory
    {
        public HttpClientFactory()
        {
            
        }

        public HttpFileDownload GetHttpFileDownload()
        {
            return new HttpFileDownload();
        }
    }
}
