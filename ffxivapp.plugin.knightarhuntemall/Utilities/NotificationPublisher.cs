// FFXIVAPP.Plugin.Knightarhuntemall
// NotificationPublisher.cs
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
using System.IO;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Dynamic;
using System.Xml.Linq;
using FFXIVAPP.Common.Utilities;
using FFXIVAPP.Plugin.Knightarhuntemall.Properties;
using NLog;
using Newtonsoft.Json;

namespace FFXIVAPP.Plugin.Knightarhuntemall.Utilities
{
    public static class NotificationPublisher
    {
        public static void PublishPushOver(String title, String Message)
        {
            NameValueCollection pushoverParamaters = new NameValueCollection();
            pushoverParamaters.Add("token", Settings.Default.pushoverApplicationToken);
            pushoverParamaters.Add("user", Settings.Default.pushoverUserToken);
            pushoverParamaters.Add("title", title);
            pushoverParamaters.Add("message", Message);

            HttpWebRequest pushoverAsyncRequest = HttpHelper.GetRequest("https://api.pushover.net/1/messages.json", pushoverParamaters);
            pushoverAsyncRequest.BeginGetResponse(new AsyncCallback(HttpHelper.FinishWebRequest), pushoverAsyncRequest);
        }

        public static void PublishNotifyMyAndroidKey(String title, String Message)
        {
            NameValueCollection notifymyandroidParamaters = new NameValueCollection();
            notifymyandroidParamaters.Add("apikey", Settings.Default.notifymyandroidKey);
            notifymyandroidParamaters.Add("application", "FFXIVApp");
            notifymyandroidParamaters.Add("event", title);
            notifymyandroidParamaters.Add("description", Message);

            HttpWebRequest notifymyandroidAsyncRequest = HttpHelper.GetRequest("https://www.notifymyandroid.com/publicapi/notify", notifymyandroidParamaters);
            notifymyandroidAsyncRequest.BeginGetResponse(new AsyncCallback(HttpHelper.FinishWebRequest), notifymyandroidAsyncRequest);
        }

        public static Int16 verifyUserKeyPushover()
        {
            try
            {
                NameValueCollection pushoverParamaters = new NameValueCollection();
                pushoverParamaters.Add("token", Settings.Default.pushoverApplicationToken);
                pushoverParamaters.Add("user", Settings.Default.pushoverUserToken);

                HttpWebRequest pushoverRequest = HttpHelper.GetRequest("https://api.pushover.net/1/users/validate.json", pushoverParamaters);

                String ResponseContent;
                using (HttpWebResponse response = (HttpWebResponse)HttpHelper.MakeRequest(pushoverRequest))
                {
                    using (Stream data = response.GetResponseStream())
                    {
                        ResponseContent = new StreamReader(data).ReadToEnd();
                    }
                    response.Close();
                }

                var dummyObject = new { status = 0, token = "", user = "" };
                var decoded = JsonConvert.DeserializeAnonymousType(ResponseContent, dummyObject);

                if (decoded.status == 1)
                {
                    return 0;
                }

                Int16 returnInt = 0;

                if (decoded.token == "invalid")
                {
                    returnInt += 1;
                }
                if (decoded.user == "invalid")
                {
                    returnInt += 2;
                }

                return returnInt;
            }
            catch (Exception ex)
            {
                Logging.Log(LogManager.GetCurrentClassLogger(), "", ex);
                return 3;
            }
        }

        public static Boolean verifyUserKeyNotifyMyAndroid()
        {
            try
            {
                String ResponseContent;

                HttpWebRequest notifymyandroidRequest = (HttpWebRequest)HttpWebRequest.Create("https://www.notifymyandroid.com/publicapi/verify?apikey=" + Settings.Default.notifymyandroidKey);
                using (HttpWebResponse response = (HttpWebResponse)HttpHelper.MakeRequest(notifymyandroidRequest))
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        response.Close();
                        return false;
                    }

                    using (Stream data = response.GetResponseStream())
                    {
                        ResponseContent = new StreamReader(data).ReadToEnd();
                    }
                    response.Close();
                }

                dynamic parser = new DynamicXmlParser(ResponseContent);

                if (parser.success != null && parser.success["code"] == "200")
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Logging.Log(LogManager.GetCurrentClassLogger(), "", ex);
            }

            return false;
        }
    }

    class DynamicXmlParser : DynamicObject
    {
        XElement element;

        public DynamicXmlParser(string xml)
        {
            element = XElement.Parse(xml);
        }

        private DynamicXmlParser(XElement el)
        {
            element = el;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (element == null)
            {
                result = null;
                return true;
            }

            XElement sub = element.Element(binder.Name);

            if (sub == null)
            {
                result = null;
                return true;
            }
            else
            {
                result = new DynamicXmlParser(sub);
                return true;
            }
        }

        public override string ToString()
        {
            if (element != null)
            {
                return element.Value;
            }
            else
            {
                return string.Empty;
            }
        }

        public string this[string attr]
        {
            get
            {
                if (element == null)
                {
                    return string.Empty;
                }

                return element.Attribute(attr).Value;
            }
        }
    }
}
