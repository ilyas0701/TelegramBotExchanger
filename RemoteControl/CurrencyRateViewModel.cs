using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RemoteControl
{
    internal class CurrencyRateViewModel
    {
        private decimal _buy;
        private decimal _sale;

        public decimal Buy
        {
            get { return _buy; }
            private set
            {
                _buy = value;
                OnPropertyChanged("Buy");
            }
        }

        public decimal Sale
        {
            get { return _sale; }
            private set
            {
                _sale = value;
                OnPropertyChanged("Sale");
            }
        }

        public async Task LoadData()
        {
            string url = "https://api.privatbank.ua/p24api/pubinfo?exchange&coursid=5";
            try
            {
                HttpClient client = new HttpClient();
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var currencyRates = JsonConvert.DeserializeObject<List<CurrencyRate>>(content);

                var usdRate = currencyRates.FirstOrDefault(val => val.Ccy == "USD");

                if (usdRate != null)
                {
                    Buy = usdRate.Buy;
                    Sale = usdRate.Sale;
                }
                else
                {
                    // If we don't have value;
                }
            }
            catch (Exception ex)
            {
                // log...
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
