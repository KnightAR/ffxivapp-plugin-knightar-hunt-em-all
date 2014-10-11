// FFXIVAPP.Plugin.Knightarhuntemall
// HttpHelper.cs
// 
// Copyright © 2014 KnightAR - All Rights Reserved
// 
// Redistribution and use in source and binary forms, with or without 
// modification, are permitted provided that the following conditions are met: 
// 
//  * Redistributions of source code must retain the above copyright notice, 
//    this list of conditions and the following disclaimer. 
//  * Redistributions in binary form must reproduce the above copyright 
//    notice, this list of conditions and the following disclaimer in the 
//    documentation and/or other materials provided with the distribution. 
//  * Neither the name of SyndicatedLife nor the names of its contributors may 
//    be used to endorse or promote products derived from this software 
//    without specific prior written permission. 
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE 
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE 
// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE 
// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR 
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF 
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN 
// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
// POSSIBILITY OF SUCH DAMAGE. 

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVAPP.Plugin.Knightarhuntemall.Utilities
{
    public static class HttpHelper
    {
        public static HttpWebResponse MakeRequest(HttpWebRequest request)
        {
            try
            {
                return (HttpWebResponse)request.GetResponse();
            }
            catch (WebException we)
            {
                if (we.Response != null)
                {
                    return (HttpWebResponse)we.Response;
                }
                throw;
            }
        }

        public static HttpWebRequest GetRequest(String url, NameValueCollection nameValueCollection)
        {
            // Here we convert the nameValueCollection to POST data.
            // This will only work if nameValueCollection contains some items.
            var parameters = new StringBuilder();

            foreach (string key in nameValueCollection.Keys)
            {
                parameters.AppendFormat("{0}={1}&",
                    WebUtility.UrlEncode(key),
                    WebUtility.UrlEncode(nameValueCollection[key]));
            }

            parameters.Length -= 1;
            String par = parameters.ToString();

            // Here we create the request and write the POST data to it.
            var request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = par.Length;

            using (var writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(par);
            }

            return request;
        }

        public static void FinishWebRequest(IAsyncResult result)
        {
            HttpWebResponse response = (result.AsyncState as HttpWebRequest).EndGetResponse(result) as HttpWebResponse;
        }
    }
}
