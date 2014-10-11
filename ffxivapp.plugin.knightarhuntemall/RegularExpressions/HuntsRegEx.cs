// FFXIVAPP.Plugin.Knightarhuntemall
// HuntsRegEx.cs
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
using System.Text.RegularExpressions;
using FFXIVAPP.Common.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using FFXIVAPP.Common.Utilities;
using NLog;

namespace FFXIVAPP.Plugin.Knightarhuntemall.RegularExpressions
{
    public class HuntEntry
    {
        public string huntname { get; private set; }
        public string rank { get; private set; }
        public Regex regex { get; private set; }
        public string translatedname { get; private set; }
        public string location { get; private set; }
        public HuntEntry(string Name, string Rank, String Location, String LangName)
        {
            huntname = Name;
            rank = Rank;
            translatedname = LangName;
            location = Location;
            regex = new Regex(string.Format(HuntsRegEx.RegExFormat, HuntsRegEx.ArrowChar, Location, LangName), SharedRegEx.DefaultOptions | RegexOptions.IgnoreCase);
        }
    }
    internal static class HuntsRegEx
    {
        public static string[,] Hunts = new string[17, 12] {
            /* Southern Thanalan - Nunyunuwi - Zanig'oh
            * 南ザナラーン - ヌニュヌウィ - ザニゴ
            * Thanalan méridional
            * Südliches Thanalan
            */
            {
                "Southern Thanalan", "Nunyunuwi", "Zanig'oh", //En
                "南ザナラーン", "ヌニュヌウィ", "ザニゴ", //Jp
                "Thanalan méridional", "Nunyunuwi", "Zanig'oh", //Fr
                "Südliches Thanalan", "Nunyunuwi", "Zanig'oh" //De
            },
            /* Mor Dhona - Agrippa - Kurrea
            * モードゥナ - アグリッパ - クーレア
            * Mor Dhona
            * Mor Dhona
            */
            {
                "Mor Dhona", "Agrippa", "Kurrea", //En
                "モードゥナ", "アグリッパ", "クーレア", //Jp
                "Mor Dhona", "Agrippa", "Kurrea", //Fr
                "Mor Dhona", "Agrippa", "Kurrea"//De
            },
            /* Eastern La Noscea - Garlok - Hellsclaw
            * 東ラノシア - ガーロック - 魔導ヘルズクロー
            * Noscea orientale
            * Östliches La Noscea
            */
            {
                "Eastern La Noscea", "Garlok", "Hellsclaw", //En
                "東ラノシア", "ガーロック", "魔導ヘルズクロー", //Jp
                "Noscea orientale", "Garlok", "Hellsclaw", //Fr
                "Östliches La Noscea", "Garlok", "Hellsclaw" //De
            },
            /* Upper La Noscea - Nandi - Marberry
            * 高地ラノシア - ナンディ - マーベリー
            * Haute-Noscea
            * Oberes La Noscea
            */
            {
                "Upper La Noscea", "Nandi", "Marberry", //En
                "高地ラノシア", "ナンディ", "マーベリー", //Jp
                "Haute-Noscea", "Nandi", "Marberry", //Fr
                "Oberes La Noscea", "Nandi", "Marberry" //De
            },
            /* Central Shroud - Laideronnette - Forneus
            * 黒衣森：中央森林 - レドロネット - ファルネウス
            * Forêt centrale
            * Tiefer Wald
            */
            {
                "Central Shroud", "Laideronnette", "Forneus", //En
                "黒衣森：中央森林", "レドロネット", "ファルネウス", //Jp
                "Forêt centrale", "Laideronnette", "Forneus", //Fr
                "Tiefer Wald", "Laideronnette", "Forneus" //De
            },
            /* South Shroud - Mindflayer - Ghede Ti Malice
            * 黒衣森：南部森林 - マインドフレア - ゲーデ
            * Forêt du sud
            * Südwald
            */
            {
                "South Shroud", "Mindflayer", "Ghede Ti Malice", //En
                "黒衣森：南部森林", "マインドフレア", "ゲーデ", //Jp
                "Forêt du sud", "Mindflayer", "Ghede Ti Malice", //Fr
                "Südwald", "Mindflayer", "Ghede Ti Malice" //De
            },
            /* Western Thanalan - Zona Seeker - Alectyron
            * 西ザナラーン - ゾーナ・シーカー - アレクトリオン
            * Thanalan occidental
            * Westliches Thanalan
            */
            {
                "Western Thanalan", "Zona Seeker", "Alectyron", //En
                "西ザナラーン", "ゾーナ・シーカー", "アレクトリオン", //Jp
                "Thanalan occidental", "Zona Seeker", "Alectyron", //Fr
                "Westliches Thanalan", "Zona Seeker", "Alectyron" //De
            },
            /* North Shroud - Thousand-cast Theda - Girtab
            * 黒衣森：北部森林 - サウザンドキャスト・セダ
            * Forêt du nord
            * Nordwald
            */
            {
                "North Shroud", "Thousand-cast Theda", "Girtab", //En
                "黒衣森：北部森林", "サウザンドキャスト・セダ", "ギルタブ", //Jp
                "Forêt du nord", "Thousand-cast Theda", "Girtab", //Fr
                "Nordwald", "Thousand-cast Theda", "Girtab" //De
            },
            /* Northern Thanalan - Minhocao - Dalvag's Final Flame
            * 北ザナラーン - ミニョーカオン - ファイナルフレイム
            * Thanalan septentrional
            * Nördliches Thanalan
            */
            {
                "Northern Thanalan", "Minhocao", "Dalvag's Final Flame", //En
                "北ザナラーン", "ミニョーカオン", "ファイナルフレイム", //Jp
                "Thanalan septentrional", "Minhocao", "Dalvag's Final Flame", //Fr
                "Nördliches Thanalan", "Minhocao", "Dalvag's Final Flame" //De
            },
            /* Middle La Noscea - Croque-Mitaine - Vogaal Ja
            * 中央ラノシア - クロック・ミテーヌ - 醜男のヴォガージャ
            * Noscea centrale
            * Zentrales La Noscea
            */
            {
                "Middle La Noscea", "Croque-Mitaine", "Vogaal Ja", //En
                "中央ラノシア", "クロック・ミテーヌ", "醜男のヴォガージャ", //Jp
                "Noscea centrale", "Croque-Mitaine", "Vogaal Ja", //Fr
                "Zentrales La Noscea", "Croque-Mitaine", "Vogaal Ja" //De
            },
            /* Eastern Thanalan - Lampalagua - Maahes
            * 東ザナラーン - バルウール
            * Thanalan oriental
            * Östliches Thanalan
            */
            {
                "Eastern Thanalan", "Lampalagua", "Maahes", //En
                "東ザナラーン", "バルウール", "マヘス", //Jp
                "Thanalan oriental", "Lampalagua", "Maahes", //Fr
                "Östliches Thanalan", "Lampalagua", "Maahes" //De
            },
            /* Outer La Noscea - Mahisha - Cornu
            * 外地ラノシア - チェルノボーグ
            * Noscea extérieure
            * Äußeres La Noscea
            */
            {
                "Outer La Noscea", "Mahisha", "Cornu", //En
                "外地ラノシア", "チェルノボーグ", "コンヌ", //Jp
                "Noscea extérieure", "Mahisha", "Cornu", //Fr
                "Äußeres La Noscea", "Mahisha", "Cornu" //De
            },
            /* East Shroud - Wulgaru - Melt
            * 黒衣森：東部森林 - ウルガル
            * Forêt de l'est
            * Ostwald
            */
            {
                "East Shroud", "Wulgaru", "Melt", //En
                "黒衣森：東部森林", "ウルガル", "メルティゼリー", //Jp
                "Forêt de l'est", "Wulgaru", "Melt", //Fr
                "Ostwald", "Wulgaru", "Melt" //De
            },
            /* Western La Noscea - Bonnacon - Nahn
            * 西ラノシア - ボナコン - ナン
            * Noscea occidentale
            * Westliches La Noscea
            */
            {
                "Western La Noscea", "Bonnacon", "Nahn", //En
                "西ラノシア", "ボナコン", "ナン", //Jp
                "Noscea occidentale", "Bonnacon", "Nahn", //Fr
                "Westliches La Noscea", "Bonnacon", "Nahn" //De
            },
            /* Coerthas Central Highlands - Safat - Marraco
            * クルザス中央高地 - サファト - マラク
            * Hautes terres du Coerthas central
            * Zentrales Hochland von Coerthas
            */
            {
                "Coerthas Central Highlands", "Safat", "Marraco", //En
                "クルザス中央高地", "サファト", "マラク", //Jp
                "Hautes terres du Coerthas central", "Safat", "Marraco", //Fr
                "Zentrales Hochland von Coerthas", "Safat", "Marraco" //De
            },
            /* Lower La Noscea - Croakadile - Unktehi
            * 低地ラノシア - ケロゲロス - ウンクテヒ
            * Basse-Noscea
            * Unteres La Noscea
            */
            {
                "Lower La Noscea", "Croakadile", "Unktehi", //En
                "低地ラノシア", "ケロゲロス", "ウンクテヒ", //Jp
                "Basse-Noscea", "Croakadile", "Unktehi", //Fr
                "Unteres La Noscea", "Croakadile", "Unktehi" //De
            },
            /* Central Thanalan - Brontes - Sabotender Bailarina
            * 中央ザナラーン - ブロンテス - サボテンダー・バイラリーナ
            * Thanalan central
            * Zentrales Thanalan
            */
            {
                "Central Thanalan", "Brontes", "Sabotender Bailarina", //En
                "中央ザナラーン", "ブロンテス", "サボテンダー・バイラリーナ", //Jp
                "Thanalan central", "Brontes", "Sabotender Bailarina", //Fr
                "Zentrales Thanalan", "Brontes", "Sabotender Bailarina" //De
            }
        };

        public static string[] SHuntNames = {};
        public static string[] AHuntNames = {};

        public static ArrayList validZonesEn = new ArrayList();
        public static ArrayList validZonesJp = new ArrayList();
        public static ArrayList validZonesFr = new ArrayList();
        public static ArrayList validZonesDe = new ArrayList();

        public static Hashtable HuntsEn = new Hashtable();
        public static Hashtable HuntsJp = new Hashtable();
        public static Hashtable HuntsFr = new Hashtable();
        public static Hashtable HuntsDe = new Hashtable();

        public static string ArrowChar = "";
        public static string RegExFormat = @"{0}{1}.*{2}|.*?{2}.*{0}{1}";

        public static string MapLocationRegEx = @"(.+?) \((.+?),(.+?)\)";
        public static string MapLocationZoneRegEx = @"{0} \((.+?),(.+?)\)";
        public static string CallerNameRegEx = @"^(.+?):";

        public static Regex SRank;
        public static Regex ARank;

        public static string AlphaRank = @"{0}.*?(.+) \((.+),(.+)\).*{1}|.*?{1}.*?{0}(.+) \((.+),(.+)\)";

        public static void Load()
        {
            SRank = new Regex(string.Format(AlphaRank, ArrowChar, @"(S Rank|Rank S)"), SharedRegEx.DefaultOptions | RegexOptions.IgnoreCase);
            ARank = new Regex(string.Format(AlphaRank, ArrowChar, @"(A Rank|Rank A)"), SharedRegEx.DefaultOptions | RegexOptions.IgnoreCase);

            List<string> Slist = new List<string>();
            List<string> Alist = new List<string>();
            for (int i = 0; i < 17; i++)
            {
                string EnLocation = Hunts[i, 0];
                validZonesEn.Add(EnLocation);

                string EnSName = Hunts[i, 1];
                string EnAName = Hunts[i, 2];

                Slist.Add(EnSName);
                Alist.Add(EnAName);

                HuntEntry Hunt;

                Hunt = new HuntEntry(EnSName, "S", EnLocation, EnSName);
                HuntsEn.Add(EnSName, Hunt);
                
                Hunt = new HuntEntry(EnAName, "A", EnLocation, EnAName);
                HuntsEn.Add(EnAName, Hunt);
                
                string JpLocation = Hunts[i, 3];
                validZonesJp.Add(JpLocation);

                string JpSName = Hunts[i, 4];
                string JpAName = Hunts[i, 5];

                Hunt = new HuntEntry(EnAName, "S", JpLocation, JpSName);
                HuntsJp.Add(EnSName, Hunt);

                Hunt = new HuntEntry(EnAName, "A", JpLocation, JpAName);
                HuntsJp.Add(EnAName, Hunt);
                
                string FrLocation = Hunts[i, 6];
                validZonesFr.Add(FrLocation);

                string FrSName = Hunts[i, 7];
                string FrAName = Hunts[i, 8];

                Hunt = new HuntEntry(EnAName, "S", FrLocation, FrSName);
                HuntsFr.Add(EnSName, Hunt);

                Hunt = new HuntEntry(EnAName, "A", FrLocation, FrAName);
                HuntsFr.Add(EnAName, Hunt);

                string DeLocation = Hunts[i, 9];
                validZonesDe.Add(DeLocation);

                string DeSName = Hunts[i, 10];
                string DeAName = Hunts[i, 11];

                Hunt = new HuntEntry(EnAName, "S", DeLocation, DeSName);
                HuntsDe.Add(EnSName, Hunt);

                Hunt = new HuntEntry(EnAName, "A", DeLocation, DeAName);
                HuntsDe.Add(EnAName, Hunt);
            }
            SHuntNames = Slist.ToArray();
            AHuntNames = Alist.ToArray();
        }
    }
}