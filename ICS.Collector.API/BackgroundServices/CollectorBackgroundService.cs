using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ICS.Models.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PRS.Collector.BackgroundServices
{
    public class CollectorBackgroundService : BackgroundService
    {
        private ILogger<CollectorBackgroundService> _serviceLogger;
        private readonly IServiceProvider _serviceProvider;
        private DateTime date;
        private JObject bb_Investiments;
        private List<Selic> selic_last_12_periods;
        private Selic selic_Last_Annualized_252;
        private List<Ipca> ipca_Monthly_Variation_Last_12_Periods;
        private Ipca ipca_Annual_Average_Last_Period;
        private IpcaCalculated ipca_Calculated_For_Investiments;

        public CollectorBackgroundService(ILogger<CollectorBackgroundService> serviceLogger, IServiceProvider serviceProvider)
        {
            _serviceLogger = serviceLogger;
            _serviceProvider = serviceProvider;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            this._serviceLogger.LogTrace("Iniciando CollectorBackgroundService ...");
            Debug.WriteLine("Iniciando verificação de data ...");

            stoppingToken.Register(() => _serviceLogger.LogTrace("CollectorBackgroundService interrompido!"));

            while (!stoppingToken.IsCancellationRequested)
            {                
                _serviceLogger.LogTrace("CollectorBackgroundService executando ...");
                Debug.WriteLine($"CollectorBackgroundService executando ...");

                if (checkNeedReload())
                {
                    ResetVariables();
                }
                await Task.Delay(60000);
            }

            this._serviceLogger.LogTrace("Verificação de data finalizada!");
        }

        private void ResetVariables()
        {
            bb_Investiments = null;
            selic_last_12_periods = null;
            selic_Last_Annualized_252 = null;
            ipca_Monthly_Variation_Last_12_Periods = null;
            ipca_Annual_Average_Last_Period = null;
            ipca_Calculated_For_Investiments = null;
            date = DateTime.Now.Date;
            this._serviceLogger.LogTrace("Variáveis resetadas !!");
        }

        private bool checkNeedReload()
        {
            DateTime now = DateTime.Now.Date;
            int result = DateTime.Compare(date, now);

            if (result == 0)
                return false;

            else if (result > 0)
                return true;

            return true;
        }

        public void Set_BBInvestiments(JObject data)
        {
            date = checkNeedReload() ? DateTime.Now.Date : date;
            bb_Investiments = data;
        }

        public JObject Get_BBInvestiments()
        {
            bool checkDate = checkNeedReload();
            if (checkDate)
                ResetVariables();
            return bb_Investiments;
        }

        public void Set_Selic_last_12_periods(List<Selic> list)
        {
            date = checkNeedReload() ? DateTime.Now.Date : date;
            selic_last_12_periods = list;
        }

        public List<Selic> Get_Selic_Last_12_Periods()
        {
            bool checkDate = checkNeedReload();
            if (checkDate)
                ResetVariables();
            return selic_last_12_periods;
        }

        public void Set_Selic_Last_Annualized_252(Selic selic)
        {
            date = checkNeedReload() ? DateTime.Now.Date : date;
            selic_Last_Annualized_252 = selic;
        }

        public Selic Get_Selic_Last_Annualized_252()
        {
            bool checkDate = checkNeedReload();
            if (checkDate)
                ResetVariables();
            return selic_Last_Annualized_252;
        }

        public void Set_Ipca_Monthly_Variation_Last_12_Periods(List<Ipca> list)
        {
            date = checkNeedReload() ? DateTime.Now.Date : date;
            ipca_Monthly_Variation_Last_12_Periods = list;
        }

        public List<Ipca> Get_Ipca_Monthly_Variation_Last_12_Periods()
        {
            bool checkDate = checkNeedReload();
            if (checkDate)
                ResetVariables();
            return ipca_Monthly_Variation_Last_12_Periods;
        }

        public void Set_Ipca_Annual_Average_Last_Period(Ipca ipca)
        {
            date = checkNeedReload() ? DateTime.Now.Date : date;
            ipca_Annual_Average_Last_Period = ipca;
        }

        public Ipca Get_Ipca_Annual_Average_Last_Period()
        {
            bool checkDate = checkNeedReload();
            if (checkDate)
                ResetVariables();
            return ipca_Annual_Average_Last_Period;
        }
        
        public void Set_Ipca_Calculated_For_Investiments(IpcaCalculated ipca)
        {
            date = checkNeedReload() ? DateTime.Now.Date : date;
            ipca_Calculated_For_Investiments = ipca;
        }

        public IpcaCalculated Get_Ipca_Calculated_For_Investiments()
        {
            bool checkDate = checkNeedReload();
            if (checkDate)
                ResetVariables();
            return ipca_Calculated_For_Investiments;
        }

    }
}
