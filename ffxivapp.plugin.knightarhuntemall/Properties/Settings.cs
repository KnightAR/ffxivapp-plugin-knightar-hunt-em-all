// FFXIVAPP.P   lugin.Ed
// Settings.cs
// 
// Copyright © 2007 - 2014 Ryan Wilson - All Rights Reserved
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
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using System.Xml.Linq;
using FFXIVAPP.Common.Helpers;
using FFXIVAPP.Common.Models;
using FFXIVAPP.Common.Utilities;
using NLog;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;
using FontFamily = System.Drawing.FontFamily;
using FFXIVAPP.Plugin.Knightarhuntemall.Utilities;
using FFXIVAPP.Plugin.Knightarhuntemall.Views;
using System.Collections.Specialized;

namespace FFXIVAPP.Plugin.Knightarhuntemall.Properties
{
    public class Settings : ApplicationSettingsBase, INotifyPropertyChanged
    {
        private static Settings _default;

        public static Settings Default
        {
            get { return _default ?? (_default = ((Settings)(Synchronized(new Settings())))); }
        }

        public override void Save()
        {
            // this call to default settings only ensures we keep the settings we want and delete the ones we don't (old)
            DefaultSettings();
            SaveSettingsNode();
            Constants.XSettings.Save(Path.Combine(Common.Constants.PluginsSettingsPath, "FFXIVAPP.Plugin.Knightarhuntemall.xml"));
        }

        private void DefaultSettings()
        {
            Constants.Settings.Clear();
            //General Hunt Settings
            Constants.Settings.Add("EnablePushNotifications");
            Constants.Settings.Add("EnableARankNotifications");
            Constants.Settings.Add("EnableSRankNotifications");

            //Pushover.net
            Constants.Settings.Add("pushoverApplicationToken");
            Constants.Settings.Add("pushoverUserToken");
            Constants.Settings.Add("EnablePushoverService");
            Constants.Settings.Add("pushoverSendARankNotifications");
            Constants.Settings.Add("pushoverSendSRankNotifications");

            //NotifyMyAndroid
            Constants.Settings.Add("notifymyandroidKey");
            Constants.Settings.Add("EnableNotifyMyAndroidService");
            Constants.Settings.Add("notifymyandroidSendARankNotifications");
            Constants.Settings.Add("notifymyandroidSendSRankNotifications");

            //Growl
            Constants.Settings.Add("EnableGrowl");
            Constants.Settings.Add("growlPassword");
            Constants.Settings.Add("remoteGrowlIp");
            Constants.Settings.Add("growlSendARankNotifications");
            Constants.Settings.Add("growlSendSRankNotifications");

            //Widget
            Constants.Settings.Add("HuntWidgetWidth");
            Constants.Settings.Add("HuntWidgetHeight");
            Constants.Settings.Add("HuntWidgetUIScale");
            Constants.Settings.Add("ShowHuntWidgetOnLoad");
            Constants.Settings.Add("HuntWidgetTop");
            Constants.Settings.Add("HuntWidgetLeft");
            Constants.Settings.Add("HuntVisibility");
            Constants.Settings.Add("WidgetOpacity");
        }

        public new void Reset()
        {
            DefaultSettings();
            foreach (var key in Constants.Settings)
            {
                var settingsProperty = Default.Properties[key];
                if (settingsProperty == null)
                {
                    continue;
                }
                var value = settingsProperty.DefaultValue.ToString();
                SetValue(key, value, CultureInfo.InvariantCulture);
            }
        }

        public void SetValue(string key, string value, CultureInfo cultureInfo)
        {
            try
            {
                var type = Default[key].GetType()
                                       .Name;
                switch (type)
                {
                    case "Boolean":
                        Default[key] = Boolean.Parse(value);
                        break;
                    case "Color":
                        var cc = new ColorConverter();
                        var color = cc.ConvertFrom(value);
                        Default[key] = color ?? Colors.Black;
                        break;
                    case "Double":
                        Default[key] = Double.Parse(value, cultureInfo);
                        break;
                    case "Font":
                        var fc = new FontConverter();
                        var font = fc.ConvertFromString(value);
                        Default[key] = font ?? new Font(new FontFamily("Microsoft Sans Serif"), 12);
                        break;
                    case "Int32":
                        Default[key] = Int32.Parse(value, cultureInfo);
                        break;
                    default:
                        Default[key] = value;
                        break;
                }
            }
            catch (Exception ex)
            {
                Logging.Log(LogManager.GetCurrentClassLogger(), "", ex);
            }
            RaisePropertyChanged(key);
        }


        #region Property Bindings (Settings)

        #region Styles
        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("#FF000000")]
        public Color ChatBackgroundColor
        {
            get { return ((Color)(this["ChatBackgroundColor"])); }
            set
            {
                this["ChatBackgroundColor"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("#FF800080")]
        public Color TimeStampColor
        {
            get { return ((Color)(this["TimeStampColor"])); }
            set
            {
                this["TimeStampColor"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("Microsoft Sans Serif, 12pt")]
        public Font ChatFont
        {
            get { return ((Font)(this["ChatFont"])); }
            set
            {
                this["ChatFont"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("100")]
        public Double Zoom
        {
            get { return ((Double)(this["Zoom"])); }
            set
            {
                this["Zoom"] = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Pushover Settings

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("")]
        public string pushoverApplicationToken
        {
            get { return ((string)(this["pushoverApplicationToken"])); }
            set
            {
                this["pushoverApplicationToken"] = value;
                if (value == "")
                {
                    this["EnablePushoverService"] = false;
                    RaisePropertyChanged("EnablePushoverService");
                }
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("")]
        public string pushoverUserToken
        {
            get { return ((string)(this["pushoverUserToken"])); }
            set
            {
                this["pushoverUserToken"] = value;
                if (value == "")
                {
                    this["EnablePushoverService"] = false;
                    RaisePropertyChanged("EnablePushoverService");
                }
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("False")]
        public bool EnablePushoverService
        {
            get { return ((bool)(this["EnablePushoverService"])); }
            set
            {
                if (this["pushoverApplicationToken"] == "" || this["pushoverUserToken"] == "")
                {
                    this["EnablePushoverService"] = false;
                }
                else
                {
                    this["EnablePushoverService"] = value;
                    if (value)
                    {
                        Int16 Verified = NotificationPublisher.verifyUserKeyPushover();
                        if (Verified != 0)
                        {
                            this["EnablePushoverService"] = false;
                            if (Verified == 1)
                            {
                                SettingsView.View.pushoverApptext.Foreground = System.Windows.Media.Brushes.Red;
                                SettingsView.View.pushoverUsertext.Foreground = System.Windows.Media.Brushes.Black;
                            }
                            else if (Verified == 2)
                            {
                                SettingsView.View.pushoverUsertext.Foreground = System.Windows.Media.Brushes.Red;
                                SettingsView.View.pushoverApptext.Foreground = System.Windows.Media.Brushes.Black;
                            }
                            else if (Verified == 3)
                            {
                                SettingsView.View.pushoverUsertext.Foreground = System.Windows.Media.Brushes.Red;
                                SettingsView.View.pushoverApptext.Foreground = System.Windows.Media.Brushes.Red;
                            }
                        }
                        else
                        {
                            SettingsView.View.pushoverApptext.Foreground = System.Windows.Media.Brushes.Black;
                            SettingsView.View.pushoverUsertext.Foreground = System.Windows.Media.Brushes.Black;
                        }
                    }
                }
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("True")]
        public bool pushoverSendSRankNotifications
        {
            get { return ((bool)(this["pushoverSendSRankNotifications"])); }
            set
            {
                this["pushoverSendSRankNotifications"] = value;
                RaisePropertyChanged();
            }
        }
        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("True")]
        public bool pushoverSendARankNotifications
        {
            get { return ((bool)(this["pushoverSendARankNotifications"])); }
            set
            {
                this["pushoverSendARankNotifications"] = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region NotifyMyAndroid Settings

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("")]
        public string notifymyandroidKey
        {
            get { return ((string)(this["notifymyandroidKey"])); }
            set
            {
                this["notifymyandroidKey"] = value;
                if (value == "")
                {
                    this["EnableNotifyMyAndroidService"] = false;
                    RaisePropertyChanged("EnableNotifyMyAndroidService");
                }

                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("False")]
        public bool EnableNotifyMyAndroidService
        {
            get { return ((bool)(this["EnableNotifyMyAndroidService"])); }
            set
            {
                if (this["notifymyandroidKey"] == "")
                {
                    this["EnableNotifyMyAndroidService"] = false;
                }
                else
                {
                    this["EnableNotifyMyAndroidService"] = value;
                    if (value)
                    {
                        Boolean Verified = NotificationPublisher.verifyUserKeyNotifyMyAndroid();
                        if (!Verified)
                        {
                            this["EnableNotifyMyAndroidService"] = false;
                        }
                    }
                }
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("True")]
        public bool EnablePushNotifications
        {
            get { return ((bool)(this["EnablePushNotifications"])); }
            set
            {
                this["EnablePushNotifications"] = value;
                RaisePropertyChanged();
            }
        }


        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("True")]
        public bool notifymyandroidSendSRankNotifications
        {
            get { return ((bool)(this["notifymyandroidSendSRankNotifications"])); }
            set
            {
                this["notifymyandroidSendSRankNotifications"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("True")]
        public bool notifymyandroidSendARankNotifications
        {
            get { return ((bool)(this["notifymyandroidSendARankNotifications"])); }
            set
            {
                this["notifymyandroidSendARankNotifications"] = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Growl Settings

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("False")]
        public bool EnableGrowl
        {
            get { return ((bool)(this["EnableGrowl"])); }
            set
            {
                this["EnableGrowl"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("")]
        public string remoteGrowlIp
        {
            get { return ((string)(this["remoteGrowlIp"])); }
            set
            {
                this["remoteGrowlIp"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("")]
        public string growlPassword
        {
            get { return ((string)(this["growlPassword"])); }
            set
            {
                this["growlPassword"] = value;
                RaisePropertyChanged();
            }
        }


        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("True")]
        public bool growlSendSRankNotifications
        {
            get { return ((bool)(this["growlSendSRankNotifications"])); }
            set
            {
                this["growlSendSRankNotifications"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("True")]
        public bool growlSendARankNotifications
        {
            get { return ((bool)(this["growlSendARankNotifications"])); }
            set
            {
                this["growlSendARankNotifications"] = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Global Settings

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("True")]
        public bool EnableARankNotifications
        {
            get { return ((bool)(this["EnableARankNotifications"])); }
            set
            {
                this["EnableARankNotifications"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("True")]
        public bool EnableSRankNotifications
        {
            get { return ((bool)(this["EnableSRankNotifications"])); }
            set
            {
                this["EnableSRankNotifications"] = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Widget Settings

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("True")]
        public bool ShowTitlesOnWidgets
        {
            get { return ((bool)(this["ShowTitlesOnWidgets"])); }
            set
            {
                this["ShowTitlesOnWidgets"] = value;
                RaisePropertyChanged();
            }
        }

        [ApplicationScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue(@"<?xml version=""1.0"" encoding=""utf-16""?>
<ArrayOfString xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <string>0.8</string>
  <string>0.9</string>
  <string>1.0</string>
  <string>1.1</string>
  <string>1.2</string>
  <string>1.3</string>
  <string>1.4</string>
  <string>1.5</string>
</ArrayOfString>")]
        public StringCollection HuntWidgetUIScaleList
        {
            get { return ((StringCollection)(this["HuntWidgetUIScaleList"])); }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("False")]
        public bool WidgetClickThroughEnabled
        {
            get { return ((bool)(this["WidgetClickThroughEnabled"])); }
            set
            {
                this["WidgetClickThroughEnabled"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("True")]
        public bool ShowHuntWidgetOnLoad
        {
            get { return ((bool)(this["ShowHuntWidgetOnLoad"])); }
            set
            {
                this["ShowHuntWidgetOnLoad"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("200")]
        public int HuntWidgetTop
        {
            get { return ((int)(this["HuntWidgetTop"])); }
            set
            {
                this["HuntWidgetTop"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("100")]
        public int HuntWidgetLeft
        {
            get { return ((int)(this["HuntWidgetLeft"])); }
            set
            {
                this["HuntWidgetLeft"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("1.0")]
        public string HuntWidgetUIScale
        {
            get { return ((string)(this["HuntWidgetUIScale"])); }
            set
            {
                this["HuntWidgetUIScale"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("350")]
        public int HuntWidgetWidth
        {
            get { return ((int)(this["HuntWidgetWidth"])); }
            set
            {
                this["HuntWidgetWidth"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("450")]
        public int HuntWidgetHeight
        {
            get { return ((int)(this["HuntWidgetHeight"])); }
            set
            {
                this["HuntWidgetHeight"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("0")]
        public string HuntVisibility
        {
            get { return ((string)(this["HuntVisibility"])); }
            set
            {
                this["HuntVisibility"] = value;
                RaisePropertyChanged();
            }
        }

        [ApplicationScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue(@"<?xml version=""1.0"" encoding=""utf-16""?>
<ArrayOfString xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <string>0</string>
  <string>50</string>
  <string>100</string>
  <string>150</string>
  <string>200</string>
  <string>250</string>
  <string>300</string>
</ArrayOfString>")]
        public StringCollection HuntVisibilityList
        {
            get { return ((StringCollection)(this["HuntVisibilityList"])); }
            set
            {
                this["HuntVisibilityList"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("0.7")]
        public string WidgetOpacity
        {
            get { return ((string)(this["WidgetOpacity"])); }
            set
            {
                this["WidgetOpacity"] = value;
                RaisePropertyChanged();
            }
        }

        [ApplicationScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue(@"<?xml version=""1.0"" encoding=""utf-16""?>
<ArrayOfString xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <string>0.5</string>
  <string>0.6</string>
  <string>0.7</string>
  <string>0.8</string>
  <string>0.9</string>
  <string>1.0</string>
</ArrayOfString>")]
        public StringCollection WidgetOpacityList
        {
            get { return ((StringCollection)(this["WidgetOpacityList"])); }
            set
            {
                this["WidgetOpacityList"] = value;
                RaisePropertyChanged();
            }
        }

        [ApplicationScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue(@"<?xml version=""1.0"" encoding=""utf-16""?>
<ArrayOfString xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"">
  <string>1</string>
  <string>2</string>
  <string>3</string>
  <string>4</string>
  <string>5</string>
  <string>10</string>
  <string>15</string>
  <string>20</string>
  <string>25</string>
  <string>30</string>
  <string>35</string>
  <string>40</string>
  <string>45</string>
  <string>50</string>
  <string>55</string>
  <string>60</string>
</ArrayOfString>")]
        public StringCollection HuntWidgetHuntTimeoutList
        {
            get { return ((StringCollection)(this["HuntWidgetHuntTimeoutList"])); }
            set
            {
                this["HuntWidgetHuntTimeoutList"] = value;
                RaisePropertyChanged();
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("10")]
        public string HuntWidgetHuntTimeout
        {
            get { return ((string)(this["HuntWidgetHuntTimeout"])); }
            set
            {
                this["HuntWidgetHuntTimeout"] = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #endregion

        #region Implementation of INotifyPropertyChanged

        public new event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void RaisePropertyChanged([CallerMemberName] string caller = "")
        {
            PropertyChanged(this, new PropertyChangedEventArgs(caller));
        }

        #endregion

        #region Iterative Settings Saving

        private void SaveSettingsNode()
        {
            if (Constants.XSettings == null)
            {
                return;
            }
            var xElements = Constants.XSettings.Descendants()
                                     .Elements("Setting");
            var enumerable = xElements as XElement[] ?? xElements.ToArray();
            foreach (var setting in Constants.Settings)
            {
                var element = enumerable.FirstOrDefault(e => e.Attribute("Key")
                                                              .Value == setting);
                if (element == null)
                {
                    var xKey = setting;
                    var xValue = Default[xKey].ToString();
                    var keyPairList = new List<XValuePair>
                    {
                        new XValuePair
                        {
                            Key = "Value",
                            Value = xValue
                        }
                    };
                    XmlHelper.SaveXmlNode(Constants.XSettings, "Settings", "Setting", xKey, keyPairList);
                }
                else
                {
                    var xElement = element.Element("Value");
                    if (xElement != null)
                    {
                        xElement.Value = Default[setting].ToString();
                    }
                }
            }
        }

        #endregion
    }
}
