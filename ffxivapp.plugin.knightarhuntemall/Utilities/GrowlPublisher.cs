// FFXIVAPP.Plugin.Knightarhuntemall
// GrowlPublisher.cs
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Growl.Connector;
using NLog;
using FFXIVAPP.Common.Utilities;
using System.IO;
using FFXIVAPP.Plugin.Knightarhuntemall.Properties;

namespace FFXIVAPP.Plugin.Knightarhuntemall.Utilities
{
    public static class GrowlPublisher
    {
        private static GrowlConnector growl;
        private static NotificationType notificationType;
        private static Growl.Connector.Application application;
        private static string sampleNotificationType = "HUNT_NOTIFICATION";
        private static string growlApplicationName = "FFXIVAPP: Gotta Hunt Em All";

        public static void growlSetup()
        {
            notificationType = new NotificationType(sampleNotificationType, "Hunt Notification");

            if (Settings.Default.remoteGrowlIp != "")
            {
                growl = new GrowlConnector(Settings.Default.growlPassword, Settings.Default.remoteGrowlIp, GrowlConnector.TCP_PORT);
            }
            else
            {
                growl = new GrowlConnector(Settings.Default.growlPassword);
            }

            //growl = new GrowlConnector("password");    // use this if you need to set a password - you can also pass null or an empty string to this constructor to use no password
            //growl = new GrowlConnector("password", "hostname", GrowlConnector.TCP_PORT);   // use this if you want to connect to a remote Growl instance on another machine

            growl.NotificationCallback += new GrowlConnector.CallbackEventHandler(growlNotificationCallback);

            // set this so messages are sent in plain text (easier for debugging)
            growl.EncryptionAlgorithm = Cryptography.SymmetricAlgorithmType.PlainText;

            application = new Growl.Connector.Application(growlApplicationName);
        }

        public static void growlRegister()
        {
            try
            {
                growlSetup();
                growl.Register(application, new NotificationType[] { notificationType });

            }
            catch (Exception ex)
            {
                Logging.Log(LogManager.GetCurrentClassLogger(), "", ex);
            }
        }

        public static void growlNotify(String Title, String Message)
        {
            try
            {
                growlSetup();
                //CallbackContext callbackContext = new CallbackContext("some fake information", "fake data");

                Notification notification = new Notification(application.Name, notificationType.Name, DateTime.Now.Ticks.ToString(), Title, Message);
                growl.Notify(notification);
            }
            catch (Exception ex)
            {
                Logging.Log(LogManager.GetCurrentClassLogger(), "", ex);
            }
        }

        public static void growlNotificationCallback(Response response, CallbackData callbackData, object state)
        {
            string text = String.Format("Response Type: {0}\r\nNotification ID: {1}\r\nCallback Data: {2}\r\nCallback Data Type: {3}\r\n", callbackData.Result, callbackData.NotificationID, callbackData.Data, callbackData.Type);
            Logging.Log(LogManager.GetCurrentClassLogger(), text);
        }
    }
}
