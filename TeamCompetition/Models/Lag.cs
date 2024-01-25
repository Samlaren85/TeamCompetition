using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace TeamCompetition.Models
{
    public class Lag : ObservableObject
    {
        public string Namn { get; set; }
        private string ryggsim;
        public string Ryggsim
        {
            get
            {
                if (ÄrNummer(ryggsim)) return TillTid(ryggsim);
                else return ryggsim;
            }
            set
            {
                if (value.Contains(".") || value.Contains(",") || value.Contains(":") || value.Contains(";")) ryggsim = Punkter(value);
                else ryggsim = value;
                if (ÄrNummer(ryggsim)) ryggsim = RäknaTid(ryggsim, Simsätten.ryggsim);
                OnPropertyChanged();
            }
        }
        private int tilläggRygg;
        public int TilläggRygg
        {
            get { return tilläggRygg; }
            set
            {
                tilläggRygg = value;
                if (ÄrNummer(ryggsim)) ryggsim = RäknaTid(ryggsim, Simsätten.ryggsim);
                OnPropertyChanged();
            }
        }
        private string bröstsim;
        public string Bröstsim
        {
            get
            {
                if (ÄrNummer(bröstsim)) return TillTid(bröstsim);
                else return bröstsim;
            }
            set
            {
                if (value.Contains(".") || value.Contains(",") || value.Contains(":") || value.Contains(";")) bröstsim = Punkter(value);
                else bröstsim = value;
                if (ÄrNummer(bröstsim)) bröstsim = RäknaTid(bröstsim, Simsätten.bröstsim);
                OnPropertyChanged();
            }
        }
        private int tilläggBröst;
        public int TilläggBröst
        {
            get { return tilläggBröst; }
            set
            {
                tilläggBröst = value;
                if (ÄrNummer(bröstsim)) bröstsim = RäknaTid(bröstsim, Simsätten.bröstsim);
                OnPropertyChanged();
            }
        }
        private string fjärilssim;
        public string Fjärilssim
        {
            get
            {
                if (ÄrNummer(fjärilssim)) return TillTid(fjärilssim);
                else return fjärilssim;
            }
            set
            {
                if (value.Contains(".") || value.Contains(",") || value.Contains(":") || value.Contains(";")) fjärilssim = Punkter(value);
                else fjärilssim = value;
                if (ÄrNummer(fjärilssim)) fjärilssim = RäknaTid(fjärilssim, Simsätten.fjärilsim);
                OnPropertyChanged();
            }
        }
        private int tilläggFjäril;
        public int TilläggFjäril
        {
            get { return tilläggFjäril; }
            set
            {
                tilläggFjäril = value;
                if (ÄrNummer(fjärilssim)) fjärilssim = RäknaTid(fjärilssim, Simsätten.fjärilsim);
                OnPropertyChanged();
            }
        }
        private string frisim;
        public string Frisim
        {
            get
            {
                if (ÄrNummer(frisim)) return TillTid(frisim);
                else return frisim;
            }
            set
            {
                if (value.Contains(".") || value.Contains(",") || value.Contains(":") || value.Contains(";")) frisim = Punkter(value);
                else frisim = value;
                if (ÄrNummer(frisim)) frisim = RäknaTid(frisim, Simsätten.frisim);
                OnPropertyChanged();
            }
        }
        private int tilläggFrisim;
        public int TilläggFrisim
        {
            get { return tilläggFrisim; }
            set
            {
                tilläggFrisim = value;
                if (ÄrNummer(frisim)) frisim = RäknaTid(frisim, Simsätten.frisim);
                OnPropertyChanged();
            }
        }
        private int tilläggSummering;
        public int TilläggSummering
        {
            get
            {
                tilläggSummering = TilläggRygg + TilläggFjäril + TilläggBröst + TilläggFrisim;
                OnPropertyChanged();
                return tilläggSummering;
            }
        } 
        private string summering;
        public string Summering
        {
            get
            {
                if (ÄrNummer(summering)) return TillTid(summering);
                else return summering;
            }
            set
            {
                summering = value;
                OnPropertyChanged();
            }
        }
        private string placering;
        public string Placering {
            get
            {
                return placering;
            } 
            set
            {
                placering = value;
                OnPropertyChanged();
            } 
        }
        public Lag() { }

        private string Punkter(string str)
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

        private string TillTid(string tillTid)
        {
            if (tillTid.Length > 4) 
            {
                tillTid = tillTid.Insert(tillTid.Length - 4, ":");
                tillTid = tillTid.Insert(tillTid.Length - 2, ".");
            }
            else if (tillTid.Length > 2)
            {
                tillTid = tillTid.Insert(tillTid.Length - 2, ".");
               
            }
            return tillTid;
        }

        private bool ÄrNummer(string nummer)
        {
            int num;
            return Int32.TryParse(nummer, out num);
        }

        private string RäknaTid(string tid, Simsätten Simsätt)
        {
            int minuter = 0;
            int sekunder = 0;
            int hundradelar = 0; 
            
            if (tid.Length > 4) minuter += Int32.Parse(tid.Substring(0, tid.Length - 4));
            if (tid.Length > 2) 
            {
                if (tid.Length == 3) sekunder += Int32.Parse(tid.Substring(0, 1));
                else sekunder += Int32.Parse(tid.Substring(tid.Length - 4, 2)); 
                switch (Simsätt)
                {
                    case Simsätten.ryggsim:
                        sekunder += TilläggRygg*UserSettings.TILLÄGGSSEKUNDER;
                        break;
                    case Simsätten.fjärilsim:
                        sekunder += TilläggFjäril * UserSettings.TILLÄGGSSEKUNDER;
                        break;
                    case Simsätten.bröstsim:
                        sekunder += TilläggBröst * UserSettings.TILLÄGGSSEKUNDER;
                        break;
                    case Simsätten.frisim:
                        sekunder += TilläggFrisim * UserSettings.TILLÄGGSSEKUNDER;
                        break;
                }
            }
            if (tid.Length < 2) hundradelar += Int32.Parse(tid);
            else hundradelar += Int32.Parse(tid.Substring(tid.Length - 2, 2)); 

            sekunder += hundradelar / 100;
            hundradelar = hundradelar % 100;
            minuter += sekunder / 60;
            sekunder = sekunder % 60;

            return minuter.ToString("0") + sekunder.ToString("00") + hundradelar.ToString("00");
        }

        public void SummeraResultat()
        {
            if (ÄrNummer(frisim) && ÄrNummer(ryggsim) && ÄrNummer(bröstsim) && ÄrNummer(fjärilssim))
            {
                int minuter = 0;
                int sekunder = 0;
                int hundradelar = 0;
                List<string> list = new List<string>() { frisim, ryggsim, bröstsim, fjärilssim };

                foreach (string s in list)
                {
                    if (s.Length > 4) minuter += Int32.Parse(s.Substring(0, s.Length - 4));
                    if (s.Length > 2)
                    {
                        if (s.Length == 3) sekunder += Int32.Parse(s.Substring(0, 1));
                        else sekunder += Int32.Parse(s.Substring(s.Length - 4, 2));
                    }
                    if (s.Length < 2) hundradelar += Int32.Parse(s);
                    else hundradelar += Int32.Parse(s.Substring(s.Length - 2, 2));
                }
                
                sekunder += hundradelar / 100;
                hundradelar = hundradelar % 100;
                minuter += sekunder / 60;
                sekunder = sekunder % 60;

                Summering = minuter.ToString("0") + sekunder.ToString("00") + hundradelar.ToString("00");
            }
            else Summering = "";
        }

        public bool Compare(string motståndare)
        {
            if (motståndare != "")
            {
                int minuter = Int32.Parse(Summering.Split(':')[0]);
                int sekunder = Int32.Parse(Summering.Split(':')[1].Split('.')[0]);
                int hundradelar = Int32.Parse(Summering.Split(':')[1].Split('.')[1]);
                int lagtid = (((minuter * 60) + sekunder) * 100) + hundradelar;
                minuter = Int32.Parse(motståndare.Split(':')[0]);
                sekunder = Int32.Parse(motståndare.Split(':')[1].Split('.')[0]);
                hundradelar = Int32.Parse(motståndare.Split(':')[1].Split('.')[1]);
                int motståndartid = (((minuter * 60) + sekunder) * 100) + hundradelar;

                return lagtid < motståndartid;
            }
            else return true;
        }

        public override string ToString()
        {
            return Namn + ";" + Ryggsim + ";" + Bröstsim + ";" + Fjärilssim + ";" + Frisim + ";" + Summering + ";" + Placering;
        }
    }
}
