using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace TeamCompetition.Models
{
    public class Team : ObservableObject
    {
        public string Name { get; set; }
        private string backstroke;
        public string Backstroke
        {
            get
            {
                if (IsNumber(backstroke)) return $"{ToTime(backstroke)} [{PeneltyBackstroke}]";
                else return backstroke;
            }
            set
            {
                string[] values = value.Split(" [");
                if (values.Length > 1)
                {
                    values[1] = values[1].Trim(']');
                    if (PeneltyBackstroke != Int32.Parse(values[1])) PeneltyBackstroke = Int32.Parse(values[1]);
                }
                
                if (values[0].Contains(".") || values[0].Contains(",") || values[0].Contains(":") || values[0].Contains(";")) backstroke = RemoveDots(values[0]);
                else backstroke = values[0];
                
                if (IsNumber(backstroke)) backstroke = CalculateTime(backstroke, SwimStyles.backstroke);
                OnPropertyChanged();
            }
        }
        private int peneltyBackstroke;
        public int PeneltyBackstroke
        {
            get { return peneltyBackstroke; }
            set
            {
                peneltyBackstroke = value;
                if (IsNumber(backstroke)) backstroke = CalculateTime(backstroke, SwimStyles.backstroke);
                OnPropertyChanged();
            }
        }
        private string breaststroke;
        public string Breaststroke
        {
            get
            {
                if (IsNumber(breaststroke)) return $"{ToTime(breaststroke)} [{PeneltyBreaststroke}]";
                else return breaststroke;
            }
            set
            {
                string[] values = value.Split(" [");
                if (values.Length > 1)
                {
                    values[1] = values[1].Trim(']');
                    if (PeneltyBreaststroke != Int32.Parse(values[1])) PeneltyBreaststroke = Int32.Parse(values[1]);
                }

                if (values[0].Contains(".") || values[0].Contains(",") || values[0].Contains(":") || values[0].Contains(";")) breaststroke = RemoveDots(values[0]);
                else breaststroke = values[0];
                if (IsNumber(breaststroke)) breaststroke = CalculateTime(breaststroke, SwimStyles.breaststroke);
                OnPropertyChanged();
            }
        }
        private int peneltyBreaststroke;
        public int PeneltyBreaststroke
        {
            get { return peneltyBreaststroke; }
            set
            {
                peneltyBreaststroke = value;
                if (IsNumber(breaststroke)) breaststroke = CalculateTime(breaststroke, SwimStyles.breaststroke);
                OnPropertyChanged();
            }
        }
        private string butterfly;
        public string Butterfly
        {
            get
            {
                if (IsNumber(butterfly)) return $"{ToTime(butterfly)} [{PeneltyButterfly}]";
                else return butterfly;
            }
            set
            {
                string[] values = value.Split(" [");
                if (values.Length > 1)
                {
                    values[1] = values[1].Trim(']');
                    if (PeneltyButterfly != Int32.Parse(values[1])) PeneltyButterfly = Int32.Parse(values[1]);
                }

                if (values[0].Contains(".") || values[0].Contains(",") || values[0].Contains(":") || values[0].Contains(";")) butterfly = RemoveDots(values[0]);
                else butterfly = values[0];
                if (IsNumber(butterfly)) butterfly = CalculateTime(butterfly, SwimStyles.butterfly);
                OnPropertyChanged();
            }
        }
        private int peneltyButterfly;
        public int PeneltyButterfly
        {
            get { return peneltyButterfly; }
            set
            {
                peneltyButterfly = value;
                if (IsNumber(butterfly)) butterfly = CalculateTime(butterfly, SwimStyles.butterfly);
                OnPropertyChanged();
            }
        }
        private string crawl;
        public string Crawl
        {
            get
            {
                if (IsNumber(crawl)) return $"{ToTime(crawl)} [{PeneltyCrawl}]";
                else return crawl;
            }
            set
            {
                string[] values = value.Split(" [");
                if (values.Length > 1)
                {
                    values[1] = values[1].Trim(']');
                    if (PeneltyCrawl != Int32.Parse(values[1])) PeneltyCrawl = Int32.Parse(values[1]);
                }

                if (values[0].Contains(".") || values[0].Contains(",") || values[0].Contains(":") || values[0].Contains(";")) crawl = RemoveDots(values[0]);
                else crawl = values[0];
                if (IsNumber(crawl)) crawl = CalculateTime(crawl, SwimStyles.crawl);
                OnPropertyChanged();
            }
        }
        private int peneltyCrawl;
        public int PeneltyCrawl
        {
            get { return peneltyCrawl; }
            set
            {
                peneltyCrawl = value;
                if (IsNumber(crawl)) crawl = CalculateTime(crawl, SwimStyles.crawl);
                OnPropertyChanged();
            }
        }
        private int peneltySummery;
        public int PeneltySummery
        {
            get
            {
                peneltySummery = PeneltyBackstroke + PeneltyButterfly + PeneltyBreaststroke + PeneltyCrawl;
                OnPropertyChanged();
                return peneltySummery;
            }
        } 
        private string result;
        public string Result
        {
            get
            {
                if (IsNumber(result)) return $"{ToTime(result)} [inkl. +{PeneltySummery*UserSettings.PENELTYSECONDS}s]";
                else return result;
            }
            set
            {
                result = value;
                OnPropertyChanged();
            }
        }
        private string placement;
        public string Placement {
            get
            {
                return placement;
            } 
            set
            {
                placement = value;
                OnPropertyChanged();
            } 
        }
        public Team() { }

        /// <summary>
        /// Removes dots from the time if input time is with styliesed with dots.
        /// </summary>
        /// <param name="str"></param>
        /// <returns>returns a string witout dots</returns>
        private string RemoveDots(string str)
        {
            if (str.Contains(".")) 
                str = str.Replace(".", "");
            if (str.Contains(","))
                str = str.Replace(",", "");
            if (str.Contains(":"))
                str = str.Replace(":", "");
            if (str.Contains(";"))
                str = str.Replace(";", "");
            return str;
        }

        /// <summary>
        /// Converts an string of integers to a string formated as time in mm:ss.ff.
        /// </summary>
        /// <param name="toTime"></param>
        /// <returns>Returns the string in format "hh:mm.ff"</returns>
        private string ToTime(string toTime)
        {
            if (toTime.Length > 4) 
            {
                toTime = toTime.Insert(toTime.Length - 4, ":");
                toTime = toTime.Insert(toTime.Length - 2, ".");
            }
            else if (toTime.Length > 2)
            {
                toTime = toTime.Insert(toTime.Length - 2, ".");
               
            }
            return toTime;
        }

        /// <summary>
        /// Check to see if the string is an integer.
        /// </summary>
        /// <param name="number"></param>
        /// <returns>Returns true if the string is parsable to an integer</returns>
        private bool IsNumber(string number)
        {
            int num;
            return Int32.TryParse(number, out num);
        }

        /// <summary>
        /// Calculates the time by spliting a string in to relevant parts and calculating the resulting time of the individual parts.
        /// </summary>
        /// <param name="time"></param>
        /// <param name="swimStyle"></param>
        /// <returns>A string calculated in mm:ss.ff but as a continuas string of numbers</returns>
        private string CalculateTime(string time, SwimStyles swimStyle)
        {
            int minuts = 0;
            int seconds = 0;
            int hundredths = 0; 
            
            if (time.Length > 4) minuts += Int32.Parse(time.Substring(0, time.Length - 4));
            if (time.Length > 2) 
            {
                if (time.Length == 3) seconds += Int32.Parse(time.Substring(0, 1));
                else seconds += Int32.Parse(time.Substring(time.Length - 4, 2)); 
                switch (swimStyle)
                {
                    case SwimStyles.backstroke:
                        seconds += PeneltyBackstroke*UserSettings.PENELTYSECONDS;
                        break;
                    case SwimStyles.butterfly:
                        seconds += PeneltyButterfly * UserSettings.PENELTYSECONDS;
                        break;
                    case SwimStyles.breaststroke:
                        seconds += PeneltyBreaststroke * UserSettings.PENELTYSECONDS;
                        break;
                    case SwimStyles.crawl:
                        seconds += PeneltyCrawl * UserSettings.PENELTYSECONDS;
                        break;
                }
            }
            if (time.Length < 2) hundredths += Int32.Parse(time);
            else hundredths += Int32.Parse(time.Substring(time.Length - 2, 2)); 

            seconds += hundredths / 100;
            hundredths = hundredths % 100;
            minuts += seconds / 60;
            seconds = seconds % 60;

            return minuts.ToString("0") + seconds.ToString("00") + hundredths.ToString("00");
        }

        /// <summary>
        /// Adds the individual event results to the total result time.
        /// </summary>
        public void AddingResults()
        {
            if (IsNumber(crawl) && IsNumber(backstroke) && IsNumber(breaststroke) && IsNumber(butterfly))
            {
                int minuts = 0;
                int seconds = 0;
                int hundradelar = 0;

                // Compiles a list of the teams results in every event
                List<string> list = new List<string>() { crawl, backstroke, breaststroke, butterfly };

                // Iterates through the list of results to add up the total
                foreach (string s in list)
                {
                    if (s.Length > 4) minuts += Int32.Parse(s.Substring(0, s.Length - 4));
                    if (s.Length > 2)
                    {
                        if (s.Length == 3) seconds += Int32.Parse(s.Substring(0, 1));
                        else seconds += Int32.Parse(s.Substring(s.Length - 4, 2));
                    }
                    if (s.Length < 2) hundradelar += Int32.Parse(s);
                    else hundradelar += Int32.Parse(s.Substring(s.Length - 2, 2));
                }
                
                seconds += hundradelar / 100;
                hundradelar = hundradelar % 100;
                minuts += seconds / 60;
                seconds = seconds % 60;

                Result = minuts.ToString("0") + seconds.ToString("00") + hundradelar.ToString("00");
            }
            else Result = "";
        }

        /// <summary>
        /// Compare to teams results to one another.
        /// </summary>
        /// <param name="opponent"></param>
        /// <returns></returns>
        public bool Compare(Team opponent)
        {
            if (opponent != null)
            {
                return TextTimeToInt() < opponent.TextTimeToInt();
            }
            else return true;
        }

        public int TextTimeToInt()
        {
            int minuts = Int32.Parse(Result.Split(':')[0]);
            int seconds = Int32.Parse(Result.Split(':')[1].Split('.')[0]);
            int hundredths = Int32.Parse(Result.Split(':')[1].Split('.')[1].Split(" [")[0]);
            return (((minuts * 60) + seconds) * 100) + hundredths;
        }

        public override string ToString()
        {
            return Name + ";" + Backstroke + ";" + Breaststroke + ";" + Butterfly + ";" + Crawl + ";" + Result + ";" + Placement;
        }
    }
}
