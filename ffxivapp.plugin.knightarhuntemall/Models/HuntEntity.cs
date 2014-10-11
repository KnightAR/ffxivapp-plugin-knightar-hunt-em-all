// FFXIVAPP.Plugin.Knightarhuntemall
// HuntEntity.cs
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
using FFXIVAPP.Common.Helpers;
using FFXIVAPP.Plugin.Knightarhuntemall.Interfaces;
using System.ComponentModel;

namespace FFXIVAPP.Plugin.Knightarhuntemall.Models
{
    public class HuntEntity : IHuntEntity
    {
        public string Name {
            get {
                return string.Format("[{0}] {1}", Rank, _Name);
            }
        }
        public string _Name { get; set; }
        public string Rank { get; set; }
        public HuntZoneLocation Location { get; set; }
        public string Caller { get; set; }
        public DateTime _Time { get; set; }

        public HuntEntity(String name, String rank, HuntZoneLocation location, string caller, DateTime time)
        {
            _Name = name;
            Rank = rank;
            Location = location;
            Caller = caller;
            _Time = time;
        }

        public String Time {
            get {
                DateTime Now = DateTime.Now;
                TimeSpan travelTime = Now - _Time;
                Double minutes = Math.Floor(travelTime.TotalMinutes);
                String minute = "minute";
                if (minutes != 1)
                {
                    minute += 's';
                }
                return string.Format("{0} {1} ago", minutes, minute);
            }
        }
    }
}