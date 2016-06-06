using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.IO;

namespace Expose178.Com.GadgetScripts
{
    public class GetKeyWordsFromSearchEngine
    {
        public GetKeyWordsFromSearchEngine(){}

        private string[][] GetEnginers()
        {
            XDocument doc = null;
            using (var fs = File.OpenRead(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Engines.xml")))
            {
                doc = XDocument.Load(fs);
            }

            string[][] engines = new string[doc.Element("engines").Elements().Count()][];
            int i = 0;
            foreach (XElement engine in doc.Element("engines").Elements())
            {
                engines[i] = new string[] { engine.Attribute("name").Value, engine.Attribute("encode").Value, engine.Attribute("key").Value, engine.Attribute("url").Value, engine.Value };
                i++;
            }
            return engines;
        }

        //搜索引擎特征
        private string[][] _Enginers = new string[][]
        {
            new string[]{"google.com","utf8","q"},
            new string[]{"google.cn","utf8","q"},
            new string[]{"g.cn","utf8","q"},
            new string[]{"cn.bing","utf8","q"},
            new string[]{"bing.com","utf8","q"},
            new string[]{"baidu","gb2312","wd"},
            new string[]{"yahoo","utf8","p"},
            new string[]{"yisou","utf8","search"},
            new string[]{"live","utf8","q"},
            new string[]{"tom","gb2312","word"},
            new string[]{"163","gb2312","q"},
            new string[]{"iask","gb2312","k"},
            new string[]{"soso","gb2312","w"},
            new string[]{"sogou","gb2312","query"},
            new string[]{"zhongsou","gb2312","w"},
            new string[]{"3721","gb2312","p"},
            new string[]{"openfind","utf8","q"},
            new string[]{"alltheweb","utf8","q"},
            new string[]{"lycos","utf8","query"},
            new string[]{"onseek","utf8","q"}
　　      };

        //搜索引擎名称
        private string _EngineName = "";
        public string EngineName
        {
            get
            {
                return _EngineName;
            }
        }

        //搜索引擎中文名称
        private string _EngineNameCN = "";
        public string EngineNameCN
        {
            get
            {
                return _EngineNameCN;
            }
        }

        //搜索引擎URL
        private string _EngineUrl = "";
        public string EngineUrl
        {
            get
            {
                return _EngineUrl;
            }
        }

        //搜索引擎编码
        private string _Coding = "utf8";
        public string Coding
        {
            get
            {
                return _Coding;
            }
        }

        //搜索引擎关键字查询参数名称
        private string _RegexWord = "";
        public string RegexWord
        {
            get
            {
                return _RegexWord;
            }
        }

        private string _Regex = @"(";
        //搜索引擎关键字
        //建立搜索关键字正则表达式
        public void EngineRegEx(string myString)
        {
            for (int i = 0, j = GetEnginers().Length; i < j; i++)
            {
                if (myString.Contains(_Enginers[i][0]))
                {
                    _EngineName = _Enginers[i][0];
                    _Coding = _Enginers[i][1];
                    _RegexWord = _Enginers[i][2];
                    _EngineUrl = _Enginers[i][3];
                    _EngineNameCN = _Enginers[i][4];
                    _Regex += _EngineName + @".+.*[?/&]" + _RegexWord + @"[=:])(?<key>[^&]*)";
                    break;
                }
            }
        }

        //得到搜索引擎关键字
        public string SearchKey(string myString)
        {
            EngineRegEx(myString.ToLower());
            if (_EngineName != "")
            {
                Regex myReg = new Regex(_Regex, RegexOptions.IgnoreCase);
                Match matche = myReg.Match(myString);
                myString = matche.Groups["key"].Value;
                //去处表示为空格的+
                myString = myString.Replace("+", " ");
                if (_Coding == "gb2312")
                {
                    myString = GetUTF8String(myString);
                }
                else
                {
                    myString = Uri.UnescapeDataString(myString);
                }
            }
            return myString;
        }
        //整句转码
        public string GetUTF8String(string myString)
        {
            Regex myReg = new Regex("(?<key>%..%..)", RegexOptions.IgnoreCase);
            MatchCollection matches = myReg.Matches(myString);
            string myWord;
            for (int i = 0, j = matches.Count; i < j; i++)
            {
                myWord = matches[i].Groups["key"].Value.ToString();
                myString = myString.Replace(myWord, GB2312ToUTF8(myWord));
            }
            return myString;
        }
        //单字GB2312转UTF8 URL编码
        public string GB2312ToUTF8(string myString)
        {
            string[] myWord = myString.Split('%');
            byte[] myByte = new byte[] { Convert.ToByte(myWord[1], 16), Convert.ToByte(myWord[2], 16) };
            Encoding GB = Encoding.GetEncoding("GB2312");
            Encoding U8 = Encoding.UTF8;
            myByte = Encoding.Convert(GB, U8, myByte);
            char[] Chars = new char[U8.GetCharCount(myByte, 0, myByte.Length)];
            U8.GetChars(myByte, 0, myByte.Length, Chars, 0);
            return new string(Chars);
        }

        //判断否为搜索引擎爬虫,并返回其类型
        public string isCrawler(string SystemInfo)
        {
            string[] BotList = new string[] { "Google", "Baidu", "MSN", "Yahoo", "TMCrawler", "iask", "Sogou" };
            foreach (string Bot in BotList)
            {
                if (SystemInfo.ToLower().Contains(Bot.ToLower()))
                {
                    return Bot;
                }
            }
            return "null";
        }
    }
}

