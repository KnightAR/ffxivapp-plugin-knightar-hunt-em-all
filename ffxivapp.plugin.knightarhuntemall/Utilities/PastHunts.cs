// FFXIVAPP.Plugin.Knightarhuntemall
// PastHunts.cs
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
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Timers;
using NLog;
using FFXIVAPP.Common.Utilities;
using FFXIVAPP.Plugin.Knightarhuntemall.Models;
using FFXIVAPP.Plugin.Knightarhuntemall.Views;
using FFXIVAPP.Plugin.Knightarhuntemall.ViewModels;

namespace FFXIVAPP.Plugin.Knightarhuntemall.Utilities
{
    public static class PastHunts
    {
        private static Hashtable _hunts = new Hashtable();

        public static void Add(FoundHunt FoundHunt)
        {
            String HuntName = FoundHunt.name;
            if (!_hunts.ContainsKey(HuntName))
            {
                _hunts.Add(HuntName, FoundHunt);
            }
            updateWidget();
        }

        public static void updateWidget() {
            List<HuntEntity> _HuntEntity = new List<HuntEntity>();
            foreach (DictionaryEntry hunt in _hunts)
            {
                FoundHunt HuntData = (FoundHunt)hunt.Value;
                if (HuntData.canDisplay())
                {
                    HuntEntity Hunt = new HuntEntity(HuntData.friendlyname, HuntData.rank, HuntData.location, HuntData.caller, HuntData.time);
                    _HuntEntity.Add(Hunt);
                }
            }
            List<HuntEntity> HuntEntity = _HuntEntity.OrderByDescending(p => p._Time.Ticks).ToList();
            HuntWidgetViewModel.Instance.HuntEntity = HuntEntity;
        }

        public static DateTime getHuntDate(String HuntName) {
            FoundHunt HuntData = (FoundHunt)_hunts[HuntName];
            return (DateTime)HuntData.time;
        }

        public static void Check()
        {
            foreach (DictionaryEntry hunt in _hunts)
            {
                string HuntName = (String)hunt.Key;
                FoundHunt HuntData = (FoundHunt)hunt.Value;
                if (HuntData.isExpired())
                {
                    _hunts.Remove(HuntName);
                    updateWidget();
                    break;
                }
            }
        }

        public static void CheckSingle(String HuntName)
        {
            if (_hunts.ContainsKey(HuntName))
            {
                FoundHunt HuntData = (FoundHunt)_hunts[HuntName];
                if (HuntData.isExpired())
                {
                    _hunts.Remove(HuntName);
                    updateWidget();
                }
            }
        }

        public static Boolean AlreadySent(string HuntName)
        {
            return _hunts.ContainsKey(HuntName);
        }

        public static Boolean CheckRecentReports()
        {
            DateTime saveNow = DateTime.Now;
            foreach (DictionaryEntry hunt in _hunts)
            {
                string HuntName = (String)hunt.Key;
                FoundHunt HuntData = (FoundHunt)_hunts[HuntName];
                if (!HuntData.isExpired())
                {
                    return true;
                }
            }
            return false;
        }

        public static Boolean HasRecentReportsInSameZone(String Rank, String Zone)
        {
            DateTime saveNow = DateTime.Now;
            foreach (DictionaryEntry hunt in _hunts)
            {
                string HuntName = (String)hunt.Key;
                FoundHunt HuntData = (FoundHunt)hunt.Value;
                if (HuntData.rank == Rank && HuntData.location.zone == Zone && !HuntData.isExpired())
                {
                    return true;
                }
            }
            return false;
        }

        public static Boolean IsRecent(String HuntName)
        {
            if (_hunts.ContainsKey(HuntName))
            {
                FoundHunt HuntData = (FoundHunt)_hunts[HuntName];
                if (!HuntData.isExpired())
                {
                    return true;
                }
            }
            return false;
        }
    }
}
