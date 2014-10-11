// FFXIVAPP.Plugin.Knightarhuntemall
// HuntsPublisher.cs
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
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using FFXIVAPP.Common.Core.Memory;
using FFXIVAPP.Common.RegularExpressions;
using FFXIVAPP.Common.Utilities;
using FFXIVAPP.Plugin.Knightarhuntemall.Properties;
using FFXIVAPP.Plugin.Knightarhuntemall.RegularExpressions;
using NLog;
using System.Collections;
using FFXIVAPP.Plugin.Knightarhuntemall.Models;

namespace FFXIVAPP.Plugin.Knightarhuntemall.Utilities
{
    public static class HuntsPublisher
    {
        public static string currentPlayer { get; set; }

        public static void Process(ChatLogEntry chatLogEntry)
        {
            try
            {
                Boolean widgetEnabled = Settings.Default.ShowHuntWidgetOnLoad;
                Boolean OneServiceEnabled = (widgetEnabled || Settings.Default.EnableNotifyMyAndroidService || Settings.Default.EnablePushoverService || Settings.Default.EnableGrowl);
                Boolean checkChats = (widgetEnabled || Settings.Default.EnablePushNotifications);

                /* If there is no services enabled, don't bother checking the chats. */
                if (!checkChats || !OneServiceEnabled)
                {
                    return;
                }

                if (getSource(chatLogEntry) != "Unknown")
                {
                    Logging.Log(LogManager.GetCurrentClassLogger(), "Current Player: " + HuntsPublisher.currentPlayer);

                    FoundHunt FoundHunt = IsMatched(chatLogEntry);

                    switch (FoundHunt.rank)
                    {
                        case "A":
                            if (!Settings.Default.EnableARankNotifications)
                            {
                                return;
                            }
                            break;
                        case "S":
                            if (!Settings.Default.EnableSRankNotifications)
                            {
                                return;
                            }
                            break;
                        default:
                            break;
                    }

                    if (FoundHunt.found)
                    {
                        PastHunts.Add(FoundHunt);
                        if (Settings.Default.EnablePushNotifications)
                        {
                            SendPost(FoundHunt, chatLogEntry);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.Log(LogManager.GetCurrentClassLogger(), "", ex);
            }
        }

        public static void SendPost(FoundHunt FoundHunt, ChatLogEntry chatLogEntry)
        {
            try
            {
                String title;
                String Message = FoundHunt.friendlyname + " Popped at: " + FoundHunt.location.ToString() + "\nCalled out in " + FoundHunt.source + " chat by " + FoundHunt.caller;

                if (!FoundHunt.isNamed())
                {
                    title = FoundHunt.rank + " Rank Popped!";
                    Message += "\n" + chatLogEntry.Line;
                }
                else
                {
                    title = FoundHunt.rank + " Rank: " + FoundHunt.friendlyname + " Popped!";
                }

                Boolean pushoverSendRankNotification = ((FoundHunt.rank == "S" && Settings.Default.pushoverSendSRankNotifications) || (FoundHunt.rank == "A" && Settings.Default.pushoverSendARankNotifications));
                if (Settings.Default.EnablePushoverService && pushoverSendRankNotification)
                {
                    NotificationPublisher.PublishPushOver(title, Message);
                }

                Boolean nmaSendRankNotification = ((FoundHunt.rank == "S" && Settings.Default.notifymyandroidSendSRankNotifications) || (FoundHunt.rank == "A" && Settings.Default.notifymyandroidSendARankNotifications));
                if (Settings.Default.EnableNotifyMyAndroidService && nmaSendRankNotification)
                {
                    NotificationPublisher.PublishNotifyMyAndroidKey(title, Message);
                }

                Boolean growlSendRankNotification = ((FoundHunt.rank == "S" && Settings.Default.growlSendSRankNotifications) || (FoundHunt.rank == "A" && Settings.Default.growlSendARankNotifications));
                if (Settings.Default.EnableGrowl)
                {
                    GrowlPublisher.growlNotify(title, Message);
                }
            }
            catch (Exception ex)
            {
                Logging.Log(LogManager.GetCurrentClassLogger(), "", ex);
            }
        }

        private static String getSource(ChatLogEntry entry)
        {
            switch (entry.Code)
            {
                case "000A":
                    return "Say";
                case "000B":
                    return "Shout";
                case "000E":
                    return "Party";
                case "000C":
                case "000D":
                    return "Tell";
                case "0010":
                    return "LS1";
                case "0011":
                    return "LS2";
                case "0012":
                    return "LS3";
                case "0013":
                    return "LS4";
                case "0014":
                    return "LS5";
                case "0015":
                    return "LS6";
                case "0016":
                    return "LS7";
                case "0017":
                    return "LS8";
                case "0018":
                    return "FC";
                case "001E":
                    return "Yell";
                /*case "0039": // This should be removed after any debugging
                    return "Echo";*/
                default:
                    return "Unknown";
            }
        }

        private static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public static FoundHunt IsMatched(ChatLogEntry chatLogEntry)
        {
            Hashtable Hunts;
            ArrayList Zones;
            switch (Constants.GameLanguage)
            {
                case "French":
                    Hunts = HuntsRegEx.HuntsFr;
                    Zones = HuntsRegEx.validZonesFr;
                    break;
                case "Japanese":
                    Hunts = HuntsRegEx.HuntsJp;
                    Zones = HuntsRegEx.validZonesJp;
                    break;
                case "German":
                    Hunts = HuntsRegEx.HuntsDe;
                    Zones = HuntsRegEx.validZonesDe;
                    break;
                default:
                    Hunts = HuntsRegEx.HuntsEn;
                    Zones = HuntsRegEx.validZonesEn;
                    break;
            }

            foreach (string Name in HuntsRegEx.SHuntNames)
            {
                HuntEntry HuntData = (HuntEntry)Hunts[Name];
                Regex HuntRegex = HuntData.regex;
                if (HuntRegex.IsMatch(chatLogEntry.Line))
                {
                    PastHunts.CheckSingle(Name);
                    PastHunts.Check();

                    if (!PastHunts.AlreadySent(Name))
                    {
                        String HuntName = Name;
                        HuntZoneLocation CalledHuntLocation = getLocation(chatLogEntry);
                        String CalledHuntCallersName = getCallerName(chatLogEntry);
                        String CalledSource = getSource(chatLogEntry);
                        FoundHunt FoundHunt = new FoundHunt(HuntName, HuntData.translatedname, HuntData.rank, CalledHuntLocation, CalledHuntCallersName, CalledSource);
                        return FoundHunt;
                    }
                    else
                    {
                        //Logging.Log(LogManager.GetCurrentClassLogger(), "Already sent a notification for this hunt recently " + HuntName);
                        return new FoundHunt();
                    }
                }
            }

            foreach (string Name in HuntsRegEx.AHuntNames)
            {
                HuntEntry HuntData = (HuntEntry)Hunts[Name];
                Regex HuntRegex = HuntData.regex;
                if (HuntRegex.IsMatch(chatLogEntry.Line))
                {
                    PastHunts.CheckSingle(Name);
                    PastHunts.Check();

                    if (!PastHunts.AlreadySent(Name))
                    {
                        String HuntName = Name;
                        HuntZoneLocation CalledHuntLocation = getLocation(chatLogEntry);
                        String CalledHuntCallersName = getCallerName(chatLogEntry);
                        String CalledSource = getSource(chatLogEntry);
                        FoundHunt FoundHunt = new FoundHunt(HuntName, HuntData.translatedname, HuntData.rank, CalledHuntLocation, CalledHuntCallersName, CalledSource);
                        return FoundHunt;
                    }
                    else
                    {
                        //Logging.Log(LogManager.GetCurrentClassLogger(), "Already sent a notification for this hunt recently " + HuntName);
                        return new FoundHunt();
                    }
                }
            }

            if (Constants.GameLanguage == "English")
            {
                Regex SHuntRegex = HuntsRegEx.SRank;
                if (SHuntRegex.IsMatch(chatLogEntry.Line))
                {
                    HuntZoneLocation CalledHuntLocation = getLocation(chatLogEntry);
                    if (Zones.Contains(CalledHuntLocation.zone) && !PastHunts.HasRecentReportsInSameZone("S", CalledHuntLocation.zone))
                    {
                        String CalledHuntCallersName = getCallerName(chatLogEntry);
                        String CalledSource = getSource(chatLogEntry);
                        FoundHunt FoundHunt = new FoundHunt("S", CalledHuntLocation, CalledHuntCallersName, CalledSource);
                        return FoundHunt;
                    }
                    else
                    {
                        //Logging.Log(LogManager.GetCurrentClassLogger(), "Already sent a notification for this hunt recently: S Rank");
                        return new FoundHunt();
                    }
                }

                Regex AHuntRegex = HuntsRegEx.ARank;
                if (AHuntRegex.IsMatch(chatLogEntry.Line))
                {
                    HuntZoneLocation CalledHuntLocation = getLocation(chatLogEntry);
                    if (Zones.Contains(CalledHuntLocation.zone) && !PastHunts.HasRecentReportsInSameZone("A", CalledHuntLocation.zone))
                    {
                        String CalledHuntCallersName = getCallerName(chatLogEntry);
                        String CalledSource = getSource(chatLogEntry);
                        FoundHunt FoundHunt = new FoundHunt("A", CalledHuntLocation, CalledHuntCallersName, CalledSource);
                        return FoundHunt;
                    }
                    else
                    {
                        //Logging.Log(LogManager.GetCurrentClassLogger(), "Already sent a notification for this hunt recently: A Rank");
                        return new FoundHunt();
                    }
                }
            }

            return new FoundHunt();
        }

        public static HuntZoneLocation getLocation(ChatLogEntry chatLogEntry)
        {
            MatchCollection LocMatched = Regex.Matches(chatLogEntry.Line, HuntsRegEx.MapLocationRegEx);
            foreach (Match match in LocMatched)
            {
                String Zone = match.Groups[1].Value;
                Int32 X = System.Convert.ToInt32(match.Groups[2].Value);
                Int32 Y = System.Convert.ToInt32(match.Groups[3].Value);
                return new HuntZoneLocation(Zone, X, Y);
                //HuntLocation = string.Format("{0} ({1},{2})", Zone, Loc1, Loc2);
            }
            return new HuntZoneLocation();
        }

        public static string getCallerName(ChatLogEntry chatLogEntry)
        {
            String callerName = "";
            MatchCollection calMatched = Regex.Matches(chatLogEntry.Line, HuntsRegEx.CallerNameRegEx);
            foreach (Match match in calMatched)
            {
                callerName = match.Groups[1].Value;
            }
            return callerName;
        }
    }
}
