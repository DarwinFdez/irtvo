﻿/*
 * theme.cs
 * 
 * Theme class:
 * 
 * Loads and stores the theme settings from the settings.ini. 
 * Available image filenames and overlay types are defined here.
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
// additional
using Ini;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.IO;

namespace iRTVO
{
    class Theme
    {

        public enum dataset
        {
            standing, 
            followed,
            sessionstate
        }

        public enum dataorder
        {
            position,
            laptime,
            classposition,
            classlaptime
        }

        public enum ThemeTypes
        {
            Overlay,
            Image,
            Ticker,
        }

        public enum ButtonActions
        {
            show,
            hide,
            toggle
        }

        public enum flags
        {
            none,
            green,
            yellow,
            white,
            checkered
        }

        public enum lights 
        {
            off,
            red,
            green
        }

        public struct ObjectProperties
        {
            public int top;
            public int left;
            
            public int width;
            public int height;

            public dataset dataset;
            public dataorder dataorder;

            public LabelProperties[] labels;

            public int zIndex;
            public string name;

            // sidepanel & results only
            public int itemCount;
            public int itemHeight;
            public int page;

            public Boolean visible;
        }

        public struct ImageProperties
        {
            public string filename;
            public int zIndex;
            public Boolean visible;
            public string name;
            public flags flag;
            public lights light;
        }

        public struct TickerProperties
        {
            public int top;
            public int left;
            
            public int width;
            public int height;

            public dataset dataset;
            public dataorder dataorder;

            public LabelProperties[] labels;

            public int zIndex;
            public string name;

            public Boolean fillVertical;

            public Boolean visible;
        }

        public struct ButtonProperties
        {
            public string name;
            public string text;
            public string[][] actions;
        }

        public struct LabelProperties
        {
            public string text;

            // Position
            public int top;
            public int left;

            // Size
            public int width;
            public int height;

            // Font
            public System.Windows.Media.FontFamily font;
            public int fontSize;
            public System.Windows.Media.SolidColorBrush fontColor;
            public System.Windows.FontWeight fontBold;
            public System.Windows.FontStyle fontItalic;
            public System.Windows.HorizontalAlignment textAlign;
        }

        public string name;
        public int width, height;
        public string path;
        private IniFile settings;

        public ObjectProperties[] objects;
        public ImageProperties[] images;
        public TickerProperties[] tickers;
        public ButtonProperties[] buttons;

        public Dictionary<string, string> translation = new Dictionary<string, string>();

        public Dictionary<string, string> carClass = new Dictionary<string, string>();

        public Theme(string themeName)
        {
            path = "themes\\" + themeName;

            // if theme not found pick the first theme on theme dir
            if (!File.Exists(Directory.GetCurrentDirectory() + "\\" + path + "\\settings.ini"))
            {
                DirectoryInfo d = new DirectoryInfo(Directory.GetCurrentDirectory() + "\\themes\\");
                DirectoryInfo[] dis = d.GetDirectories();
                foreach (DirectoryInfo di in dis)
                {
                    if (File.Exists(Directory.GetCurrentDirectory() + "\\themes\\" + di.Name + "\\settings.ini"))
                    {
                        themeName = di.Name;
                        break;
                    }
                }
            }

            settings = new IniFile(path + "\\settings.ini");

            name = themeName;
            width = Int32.Parse(getIniValue("General", "width"));
            height = Int32.Parse(getIniValue("General", "height"));

            // load objects
            string tmp = getIniValue("General", "overlays");
            string[] overlays = tmp.Split(',');
            objects = new ObjectProperties[overlays.Length];

            for(int i = 0; i < overlays.Length; i++) {
                objects[i].name = overlays[i];
                objects[i].dataorder = (dataorder)Enum.Parse(typeof(dataorder), getIniValue("Overlay-" + overlays[i], "sort"));
                objects[i].width = Int32.Parse(getIniValue("Overlay-" + overlays[i], "width"));
                objects[i].height = Int32.Parse(getIniValue("Overlay-" + overlays[i], "height"));
                objects[i].left = Int32.Parse(getIniValue("Overlay-" + overlays[i], "left"));
                objects[i].top = Int32.Parse(getIniValue("Overlay-" + overlays[i], "top"));
                objects[i].zIndex = Int32.Parse(getIniValue("Overlay-" + overlays[i], "zIndex"));
                objects[i].dataset = (dataset)Enum.Parse(typeof(dataset), getIniValue("Overlay-" + overlays[i], "dataset"));
                if (objects[i].dataset == dataset.standing)
                {
                    objects[i].itemCount = Int32.Parse(getIniValue("Overlay-" + overlays[i], "number"));
                    objects[i].itemHeight = Int32.Parse(getIniValue("Overlay-" + overlays[i], "itemHeight"));
                    objects[i].height = objects[i].itemCount * objects[i].itemHeight;
                    objects[i].page = -1;
                }

                // load labels
                tmp = getIniValue("Overlay-" + overlays[i], "labels");
                string[] labels = tmp.Split(',');
                objects[i].labels = new LabelProperties[labels.Length];
                for (int j = 0; j < labels.Length; j++)
                    objects[i].labels[j] = loadLabelProperties("Overlay-" + overlays[i], labels[j]);

                objects[i].visible = false;
            }

            // load images
            tmp = getIniValue("General", "images");
            string[] files = tmp.Split(',');
            images = new  ImageProperties[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                images[i].filename = getIniValue("Image-" + files[i], "filename");
                images[i].zIndex = Int32.Parse(getIniValue("Image-" + files[i], "zIndex"));
                images[i].visible = false;
                images[i].name = files[i];
                images[i].flag = (flags)Enum.Parse(typeof(flags), getIniValue("Image-" + files[i], "flag"));
                images[i].light = (lights)Enum.Parse(typeof(lights), getIniValue("Image-" + files[i], "light"));
            }

            // load tickers
            tmp = getIniValue("General", "tickers");
            string[] tickersnames = tmp.Split(',');
            tickers = new TickerProperties[tickersnames.Length];
            for (int i = 0; i < tickersnames.Length; i++)
            {
                tickers[i].name = tickersnames[i];
                tickers[i].dataorder = (dataorder)Enum.Parse(typeof(dataorder), getIniValue("Ticker-" + tickersnames[i], "sort"));
                tickers[i].width = Int32.Parse(getIniValue("Ticker-" + tickersnames[i], "width"));
                tickers[i].height = Int32.Parse(getIniValue("Ticker-" + tickersnames[i], "height"));
                tickers[i].left = Int32.Parse(getIniValue("Ticker-" + tickersnames[i], "left"));
                tickers[i].top = Int32.Parse(getIniValue("Ticker-" + tickersnames[i], "top"));
                tickers[i].zIndex = Int32.Parse(getIniValue("Ticker-" + tickersnames[i], "zIndex"));
                tickers[i].dataset = (dataset)Enum.Parse(typeof(dataset), getIniValue("Ticker-" + tickersnames[i], "dataset"));

                if (getIniValue("Ticker-" + tickersnames[i], "fillvertical") == "true")
                    tickers[i].fillVertical = true;
                else
                    tickers[i].fillVertical = false;

                // load labels
                tmp = getIniValue("Ticker-" + tickersnames[i], "labels");
                string[] labels = tmp.Split(',');
                tickers[i].labels = new LabelProperties[labels.Length];
                for (int j = 0; j < labels.Length; j++)
                    tickers[i].labels[j] = loadLabelProperties("Ticker-" + tickersnames[i], labels[j]);

                tickers[i].visible = false;
            }

            // load buttons
            tmp = getIniValue("General", "buttons");
            string[] btns = tmp.Split(',');
            buttons = new ButtonProperties[btns.Length];
            for (int i = 0; i < btns.Length; i++)
            {
                foreach (ThemeTypes type in Enum.GetValues(typeof(ThemeTypes)))
                {
                    buttons[i].name = btns[i];
                    buttons[i].text = getIniValue("Button-" + btns[i], "text");
                    buttons[i].actions = new string[Enum.GetValues(typeof(ButtonActions)).Length][];

                    foreach (ButtonActions action in Enum.GetValues(typeof(ButtonActions)))
                    {
                        tmp = getIniValue("Button-" + btns[i], action.ToString());
                        if (tmp != "0")
                        {
                            string[] objs = tmp.Split(',');

                            buttons[i].actions[(int)action] = new string[objs.Length];
                            for (int j = 0; j < objs.Length; j++)
                            {
                                buttons[i].actions[(int)action][j] = objs[j];
                            }
                        }
                        else
                        {
                            buttons[i].actions[(int)action] = null;
                        }
                    }
                }
            }

            SharedData.refreshButtons = true;

            string[] translations = new string[13] { // default translations
                    "lap",
                    "laps",
                    "minutes",
                    "of",
                    "race",
                    "qualify",
                    "practice",
                    "out",
                    "remaining",
                    "gridding",
                    "pacelap",
                    "finallap",
                    "finishing",
            };

            foreach (string word in translations)
            {
                string translatedword = getIniValue("Translation", word);
                if(translatedword == "0") // default is the name of the property
                    translation.Add(word, word);
                else
                    translation.Add(word, translatedword);
            }

            // signs
            if (getIniValue("General", "switchsign") == "true")
            {
                translation.Add("ahead", "+");
                translation.Add("behind", "-");
            }
            else
            {
                translation.Add("ahead", "-");
                translation.Add("behind", "+");
            }

        }
        /*
        private ObjectProperties loadProperties(string prefix)
        {
            ObjectProperties o = new ObjectProperties();

            o.left = Int32.Parse(getIniValue(prefix, "left"));
            o.top = Int32.Parse(getIniValue(prefix, "top"));
            o.size = Int32.Parse(getIniValue(prefix, "number"));
            o.width = Int32.Parse(getIniValue(prefix, "width"));
            if(Int32.Parse(getIniValue(prefix, "itemheight")) > 0)
                o.height = o.size * Int32.Parse(getIniValue(prefix, "itemheight"));
            else
                o.height = Int32.Parse(getIniValue(prefix, "height"));
            o.itemHeight = Int32.Parse(getIniValue(prefix, "itemheight"));

            o.Num = loadLabelProperties(prefix, "num");
            o.Name = loadLabelProperties(prefix, "name");
            o.Diff = loadLabelProperties(prefix, "diff");
            o.Info = loadLabelProperties(prefix, "info");

            return o;
        }
        */
        private LabelProperties loadLabelProperties(string prefix, string suffix)
        {
            LabelProperties lp = new LabelProperties();

            lp.text = getIniValue(prefix + "-" + suffix, "text");
            if (lp.text == "0")
                lp.text = "";

            lp.fontSize = Int32.Parse(getIniValue(prefix + "-" + suffix, "fontsize"));
            if (lp.fontSize == 0)
                lp.fontSize = 12;

            lp.left = Int32.Parse(getIniValue(prefix + "-" + suffix, "left"));
            lp.top = Int32.Parse(getIniValue(prefix + "-" + suffix, "top"));
            lp.width = Int32.Parse(getIniValue(prefix + "-" + suffix, "width"));
            lp.height = (int)((double)Int32.Parse(getIniValue(prefix + "-" + suffix, "fontsize")) * 1.5);

            if (File.Exists(@Directory.GetCurrentDirectory() + "\\" + path + "\\" + getIniValue(prefix + "-" + suffix, "font")))
            {
                lp.font = new System.Windows.Media.FontFamily(new Uri(Directory.GetCurrentDirectory() + "\\" + path + "\\" + getIniValue(prefix + "-" + suffix, "font")), getIniValue(prefix + "-" + suffix, "font"));
            }
            else if (getIniValue(prefix + "-" + suffix, "font") == "0")
                lp.font = new System.Windows.Media.FontFamily("Arial");
            else
                lp.font = new System.Windows.Media.FontFamily(getIniValue(prefix + "-" + suffix, "font"));
            
            if(getIniValue(prefix + "-" + suffix, "fontcolor") == "0")
                lp.fontColor = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);
            else
                lp.fontColor = (System.Windows.Media.SolidColorBrush)new System.Windows.Media.BrushConverter().ConvertFromString(getIniValue(prefix + "-" + suffix, "fontcolor"));

            if (getIniValue(prefix + "-" + suffix, "fontbold") == "true")
                lp.fontBold = System.Windows.FontWeights.Bold;
            else
                lp.fontBold = System.Windows.FontWeights.Normal;

            if (getIniValue(prefix + "-" + suffix, "fontitalic") == "true")
                lp.fontItalic = System.Windows.FontStyles.Italic;
            else
                lp.fontItalic = System.Windows.FontStyles.Normal;

            switch (getIniValue(prefix + "-" + suffix, "align"))
            {
                case "center":
                    lp.textAlign = System.Windows.HorizontalAlignment.Center;
                    break;
                case "right":
                    lp.textAlign = System.Windows.HorizontalAlignment.Right;
                    break;
                default:
                    lp.textAlign = System.Windows.HorizontalAlignment.Left;
                    break;
            }

            return lp;
        }

        public string getIniValue(string section, string key)
        {
            string retVal = settings.IniReadValue(section, key);

            if (retVal.Length == 0)
                return "0";
            else
                return retVal;
        }

        // *-name *-info
        public string[] getFollewedFormats(SharedData.DriverInfo driver, SharedData.LapInfo lapinfo, int pos)
        {
            TimeSpan laptime = DateTime.Now - driver.lastNewLap;

            string[] output = new string[19] {
                driver.name,
                driver.shortname,
                driver.initials,
                driver.license,
                driver.club,
                driver.car,
                getCarClass(driver.car), //driver.carclass.ToString(),
                (driver.numberPlate).ToString(),
                iRTVO.Overlay.floatTime2String(lapinfo.fastLap, true, false),
                iRTVO.Overlay.floatTime2String(driver.previouslap, true, false),
                "", // currentlap (live) // 10
                lapinfo.completedLaps.ToString(),
                "", // fastlap speed mph
                "", // prev lap speed mph
                "", // fastlap speed kph
                "", // prev lap speed kph
                pos.ToString(),
                "", // ordinal
                "",
                
            };

            if (laptime.TotalMinutes > 60)
                output[10] = "-.--";
            else if (((DateTime.Now - driver.lastNewLap).TotalSeconds < 5 && driver.previouslap != 1.0) || driver.onTrack == false)
            {
                output[10] = iRTVO.Overlay.floatTime2String(driver.previouslap, true, false);
            }
            else {
                output[10] = iRTVO.Overlay.floatTime2String((float)(DateTime.Now - driver.lastNewLap).TotalSeconds, true, false);
            }

            if (driver.fastestlap > 0)
            {
                output[12] = ((3600 * SharedData.track.length / (1609.344 * driver.fastestlap))).ToString("0.00");
                output[14] = ((3600 * SharedData.track.length / (3.6 * driver.fastestlap))).ToString("0.00");
            }
            else
            {
                output[12] = "-";
                output[14] = "-";
            }

            if (driver.previouslap > 0)
            {
                output[13] = ((3600 * SharedData.track.length / (1609.344 * driver.previouslap))).ToString("0.00");
                output[15] = ((3600 * SharedData.track.length / (1609.344 * driver.previouslap))).ToString("0.00");
            }
            else
            {
                output[13] = "-";
                output[15] = "-";
            }

            if(pos <= 0) // invalid input
                output[17] = "-";
            else if (pos == 11)
                output[17] = "11th";
            else if (pos == 12)
                output[17] = "12th";
            else if (pos == 13)
                output[17] = "13th";
            else if ((pos % 10) == 1)
                output[17] = pos.ToString() + "st";
            else if ((pos % 10) == 2)
                output[17] = pos.ToString() + "nd";
            else if ((pos % 10) == 3)
                output[17] = pos.ToString() + "rd";
            else
                output[17] = pos.ToString() + "th";

            if ((DateTime.Now - driver.offTrackSince).TotalMilliseconds > 1000 && SharedData.allowRetire)
            {
                output[18] = translation["out"];
            }
            if (lapinfo.lapDiff > 0 && SharedData.sessions[SharedData.overlaySession].type == iRacingTelem.eSessionType.kSessionTypeRace)
                output[18] = translation["behind"] + lapinfo.lapDiff + translation["lap"];
            else if (SharedData.standing[SharedData.overlaySession][0].fastLap > 0)
                output[18] = iRTVO.Overlay.floatTime2String((lapinfo.fastLap - SharedData.standing[SharedData.overlaySession][0].fastLap), true, false);

            return output;
        }
        public string formatFollowedText(string text, int driver, int session)
        {
            int position = 0;

            Dictionary<string, int> formatMap = new Dictionary<string, int>()
            {
                {"fullname", 0},
                {"shortname", 1},
                {"initials", 2},
                {"license", 3},
                {"club", 4},
                {"car", 5},
                {"class", 6},
                {"carnum", 7},
                {"fastlap", 8},
                {"prevlap", 9},
                {"curlap", 10},
                {"lapnum", 11},
                {"speedfast_mph", 12},
                {"speedprev_mph", 13},
                {"speedfast_kph", 14},
                {"speedprev_kph", 15},
                {"position", 16},
                {"position_ord", 17},
                {"interval", 18},
            };

            StringBuilder t = new StringBuilder(text);

            foreach (KeyValuePair<string, int> pair in formatMap)
            {
                t.Replace("{" + pair.Key + "}", "{" + pair.Value + "}");
            }

            int i = 0;
            foreach (SharedData.LapInfo lapinfo in SharedData.standing[session])
            {
                i++;
                if (lapinfo.id == driver) {
                    position = i;
                    break;
                }
            }

            if (position > 0)
                return String.Format(t.ToString(), getFollewedFormats(SharedData.drivers[driver], SharedData.standing[session][position-1], position));
            else
                return String.Format(t.ToString(), getFollewedFormats(SharedData.drivers[driver], new SharedData.LapInfo(), 0));
        }


        public string[] getSessionstateFormats(SharedData.SessionInfo session)
        {
            string[] output = new string[6] {
                session.laps.ToString(),
                session.lapsRemaining.ToString(),
                iRTVO.Overlay.floatTime2String(session.time, false, true),
                iRTVO.Overlay.floatTime2String(session.timeRemaining, false, true),
                (session.laps - session.lapsRemaining).ToString(),
                iRTVO.Overlay.floatTime2String(session.time - session.timeRemaining, false, true),
            };

            return output;
        }

        public string formatSessionstateText(string text, int session)
        {

            Dictionary<string, int> formatMap = new Dictionary<string, int>()
            {
                {"lapstotal", 0},
                {"lapsremaining", 1},
                {"timetotal", 2},
                {"timeremaining", 3},
                {"lapscompleted", 4},
                {"timepassed", 5},
            };

            StringBuilder t = new StringBuilder(text);

            foreach (KeyValuePair<string, int> pair in formatMap)
            {
                t.Replace("{" + pair.Key + "}", "{" + pair.Value + "}");
            }

            return String.Format(t.ToString(), getSessionstateFormats(SharedData.sessions[session]));
        }

        /* unused
        public string[] getFormats(SharedData.LapInfo lapinfo)
        {
            string[] output = new string[2] {
                lapinfo.diff.ToString(),
                lapinfo.fastLap.ToString()
            };

            return output;
        }
        */

        private string getCarClass(string car)
        {
            if (car != null)
            {
                try
                {
                    return carClass[car];
                }
                catch
                {
                    string name = getIniValue("Multiclass", car);
                    if (name != "0")
                    {
                        carClass.Add(car, name);
                        return name;
                    }
                    else
                    {
                        carClass.Add(car, car);
                        return car;
                    }
                }
            }
            else
                return "";
        }
    }
}