using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Urok11TSLAB.Commands;

namespace Urok11TSLAB.ViewModels
{
    public class VM : BaseVM
    {
        #region ====== Fields ======================================================
        List<decimal> _pnls = new List<decimal>();
        List<Candle> _candles = new List<Candle>();

        IFormatProvider formatter = new NumberFormatInfo { NumberDecimalSeparator = "." };
        #endregion
        #region ====== Properties ======================================================
        public decimal Depo
        {
            get => _depo;
            set
            {
                _depo = value;
                OnPropertyChanged(nameof(Depo));
            }
        }
        public decimal _depo = 100000;
        #endregion

        #region =============================== Commands =============================
        private DelegateCommand commandCalculate;
        public ICommand CommandCalculate
        {
            get
            {
                if (commandCalculate == null)
                {
                    commandCalculate = new DelegateCommand(Calculate);
                }
                return commandCalculate;
            }
        }

        private DelegateCommand commandLoadCSV;
        public ICommand CommandLoadCSV
        {
            get
            {
                if (commandLoadCSV == null)
                {
                    commandLoadCSV = new DelegateCommand(LoadCSV);
                }
                return commandLoadCSV;
            }
        }
        #endregion
        #region ================================ Methods =====================================
        private void Calculate(object obj)
        {
            List<Candle> candles = new List<Candle>();

            List<decimal> equity = new List<decimal>();
            decimal lastPnl = 0;
            decimal summOrig = 0;
            decimal summMod = 0;

            if (_pnls.Count > 0)
            {
                foreach (decimal pnl in _pnls)
                {
                    decimal eq = pnl;
                    if (lastPnl > 0)
                    {
                        eq = pnl * 3;
                    }
                    equity.Add(eq);
                    lastPnl = pnl;
                }
            }
            summMod = equity.Sum();
            summOrig = _pnls.Sum();
        }

        private void SaveCSV(Candle candle)
        {

            if (!File.Exists("C:\\_1\\save.csv"))
            {
                string header = "Заголовок";
                using (StreamWriter writer = new StreamWriter("C:\\_1\\save.csv", false))
                {
                    writer.WriteLine(header);
                    writer.Close();
                }

            }
            using (StreamWriter writer = new StreamWriter("C:\\_1\\save.csv", true))
            {
                string str = candle.Open.ToString();
                writer.WriteLine(str + ";" + "bla-bla");
                writer.Close();
            }
        }
    
        private void LoadCSV(object obj)
        {
            List<decimal> pnls = new List<decimal>();
            List<Candle> candles = new List<Candle>();

            decimal H = 8m;
            decimal Mult = 3m;
            decimal Length = 500m;

            OpenFileDialog fileDialog = new OpenFileDialog();
            if (fileDialog.ShowDialog() == true)
            {
                string fileName = fileDialog.FileName;
                if (!System.IO.File.Exists(fileName))
                {
                    return;
                }

                string error = "";
                int x = 0;
                try
                {
                    string[] listStrings = System.IO.File.ReadAllLines(fileName);
                    if (listStrings == null)
                    {
                        return;
                    }
                    foreach (string str in listStrings)
                    {
                        if (!string.IsNullOrEmpty(str))
                        {
                            Candle split1 = new Candle();
                            split1.SetCandleFromString(str);
                            //candles[x].Add(split1);    
                             SaveCSV(split1);
                            candles.Add(split1);
                            //CalculateNWE();
                               string[] split = str.Split(',');
                               if (split.Length > 8) 
                            {
                            decimal pnl=0;
                            error = split[8];
                            if (decimal.TryParse(split[8], out pnl))
                            {
                               if(pnl!=0)
                               {
                                pnl = decimal.Parse(split[8], formatter);
                                pnls.Add(pnl);
                               }
                            }
                            pnls.Add(pnl);
                         }
                    }
                    x++;
                }
                    _pnls = pnls;


                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message+" :"+error);
                }
            }
        }
        #endregion
    }
}
