// FFXIVAPP.Plugin.Knightarhuntemall
// SettingsViewModel.cs
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

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using FFXIVAPP.Common.Helpers;
using FFXIVAPP.Common.ViewModelBase;
using FFXIVAPP.Plugin.Knightarhuntemall.Properties;
using FFXIVAPP.Plugin.Knightarhuntemall.Views;
using FFXIVAPP.Plugin.Knightarhuntemall.Utilities;
using FFXIVAPP.Common.Utilities;
using NLog;
using System;

namespace FFXIVAPP.Plugin.Knightarhuntemall.ViewModels
{
    internal sealed class SettingsViewModel : INotifyPropertyChanged
    {
        #region Property Bindings

        private static SettingsViewModel _instance;

        public static SettingsViewModel Instance
        {
            get { return _instance ?? (_instance = new SettingsViewModel()); }
        }

        #endregion

        #region Declarations

        public ICommand OpenLinkToPushoverNetCommand { get; private set; }
        public ICommand OpenLinkToNotifyMyAndroidCommand { get; private set; }
        public ICommand RegisterGrowlCommand { get; private set; }
        public ICommand TestGrowlCommand { get; private set; }
        public ICommand OpenLinkToSetupGuideCommand { get; private set; }
        public ICommand OpenHuntWidgetCommand { get; private set; }
        public ICommand ResetHuntWidgetCommand { get; private set; }

        #endregion

        public SettingsViewModel()
        {
            OpenLinkToPushoverNetCommand = new DelegateCommand(OpenLinkToPushoverNet);
            OpenLinkToNotifyMyAndroidCommand = new DelegateCommand(OpenLinkToNotifyMyAndroid);
            RegisterGrowlCommand = new DelegateCommand(RegisterGrowl);
            TestGrowlCommand = new DelegateCommand(TestGrowl);
            OpenLinkToSetupGuideCommand = new DelegateCommand(OpenLinkToSetupGuide);
            OpenHuntWidgetCommand = new DelegateCommand(OpenHuntWidget);
            ResetHuntWidgetCommand = new DelegateCommand(ResetHuntWidget);
        }

        #region Loading Functions

        #endregion

        #region Utility Functions

        #endregion

        #region Command Bindings

        private static void OpenLinkToPushoverNet()
        {
            System.Diagnostics.Process.Start("https://pushover.net/login");
        }

        private static void OpenLinkToNotifyMyAndroid()
        {
            System.Diagnostics.Process.Start("https://www.notifymyandroid.com/login.jsp");
        }

        private static void OpenLinkToSetupGuide()
        {
            System.Diagnostics.Process.Start("https://github.com/KnightAR/ffxivapp-plugin-knightar-hunt-em-all/blob/master/guide.md");
        }

        public void OpenHuntWidget()
        {
            Settings.Default.ShowHuntWidgetOnLoad = true;
            Widgets.Instance.ShowHuntWidget();
        }

        public void ResetHuntWidget()
        {
            Settings.Default.HuntWidgetUIScale = Settings.Default.Properties["HuntWidgetUIScale"].DefaultValue.ToString();
            Settings.Default.HuntWidgetTop = Int32.Parse(Settings.Default.Properties["HuntWidgetTop"].DefaultValue.ToString());
            Settings.Default.HuntWidgetLeft = Int32.Parse(Settings.Default.Properties["HuntWidgetLeft"].DefaultValue.ToString());
            Settings.Default.HuntWidgetHeight = Int32.Parse(Settings.Default.Properties["HuntWidgetHeight"].DefaultValue.ToString());
            Settings.Default.HuntWidgetWidth = Int32.Parse(Settings.Default.Properties["HuntWidgetWidth"].DefaultValue.ToString());
            Settings.Default.HuntVisibility = Settings.Default.Properties["HuntVisibility"].DefaultValue.ToString();
        }

        private static void RegisterGrowl()
        {
            try
            {
                GrowlPublisher.growlRegister();
            }
            catch (Exception ex)
            {
                Logging.Log(LogManager.GetCurrentClassLogger(), "", ex);
            }
        }

        private static void TestGrowl()
        {
            try
            {
                GrowlPublisher.growlNotify("Test Message", "If you have recieved this message then Growl is working");
            }
            catch (Exception ex)
            {
                Logging.Log(LogManager.GetCurrentClassLogger(), "", ex);
            }
        }

        #endregion

        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void RaisePropertyChanged([CallerMemberName] string caller = "")
        {
            PropertyChanged(this, new PropertyChangedEventArgs(caller));
        }

        #endregion
    }
}
