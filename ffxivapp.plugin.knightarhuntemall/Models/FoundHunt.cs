// FFXIVAPP.Plugin.Knightarhuntemall
// FoundHunt.cs
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


using FFXIVAPP.Plugin.Knightarhuntemall.Properties;
using System;

namespace FFXIVAPP.Plugin.Knightarhuntemall.Models
{
    public class FoundHunt
    {
        private const Int32 Timeout = 60; //In Minutes

        public Boolean named { get; private set; }
        public Boolean found { get; private set; }
        public string name { get; private set; }
        public string friendlyname { get; private set; }
        public string rank { get; private set; }
        public HuntZoneLocation location { get; private set; }
        public string caller { get; private set; }
        public string source { get; private set; }
        public DateTime time { get; private set; }
        public Int64 expiration { get; private set; }

        public FoundHunt(string Rank, HuntZoneLocation Location, string Caller, string Source, Int64 Expiration = Timeout)
        {
            time = DateTime.Now;
            found = true;
            named = false;
            name = Rank + "Rank/" + Location.zone;
            friendlyname = Rank + " Rank";
            rank = Rank;
            location = Location;
            caller = Caller;
            source = Source;
            expiration = Expiration;
        }

        public FoundHunt(string Name, string Friendlyname, string Rank, HuntZoneLocation Location, string Caller, string Source, Int64 Expiration = Timeout)
        {
            time = DateTime.Now;
            found = true;
            named = true;
            name = Name;
            friendlyname = Friendlyname;
            rank = Rank;
            location = Location;
            caller = Caller;
            source = Source;
            expiration = Expiration;
        }

        public FoundHunt()
        {
            time = DateTime.Now;
            found = false;
        }

        public Boolean isExpired()
        {
            DateTime Now = DateTime.Now;
            TimeSpan travelTime = Now - time;

            if (travelTime.TotalMinutes > expiration)
            {
                return true;
            }
            return false;
        }

        public Boolean canDisplay()
        {
            DateTime Now = DateTime.Now;
            TimeSpan travelTime = Now - time;

            if (travelTime.TotalMinutes <= Double.Parse(Settings.Default.HuntWidgetHuntTimeout))
            {
                return true;
            }
            return false;
        }

        public Boolean isNamed()
        {
            return named;
        }
    }
}
