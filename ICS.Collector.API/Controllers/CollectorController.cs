using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using ICS.Models.Builders;
using ICS.Models.Enumerators;
using ICS.Models.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PRS.Collector.BackgroundServices;
using System.Globalization;
using System.Net;
using System.Text;

namespace ICS.Collector.Controllers
{
    [Route("[Controller]")]
    [ApiController]
    public class CollectorController : Controller
    {

        private CollectorBackgroundService collectorService;

        public CollectorController(CollectorBackgroundService hostedService)
        {
            this.collectorService = hostedService;
        }

        [HttpGet("bbinvestiments")]
        [AllowAnonymous]
        public async Task<ActionResult> BBInvestiments()
        {
            try
            {
                JObject investiments = collectorService.Get_BBInvestiments();

                if (investiments == null)
                    investiments = ColectBB();

                return Ok(JsonConvert.SerializeObject(investiments));
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpGet("selic")]
        [AllowAnonymous]
        public async Task<ActionResult> GetSelic()
        {
            try
            {
                List<Selic> list = collectorService.Get_Selic_Last_12_Periods();

                if (list == null)
                {
                    HttpClient request = new HttpClientBuilder().Build();
                    var responseBCAPI = await request.GetAsync(Links.GetLink(Link.SELIC_LAST_12_PERIODS));
                    var statusCode = responseBCAPI.StatusCode;

                    if (statusCode != HttpStatusCode.OK)
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, TxTResponses.GetTxTResponse(TxTResponse.Failure_GetIndicator));
                    }

                    list = JsonConvert.DeserializeObject<List<Selic>>(JToken.Parse(responseBCAPI.Content.ReadAsStringAsync().Result).ToString());

                    collectorService.Set_Selic_last_12_periods(list);
                }

                return Ok(JsonConvert.SerializeObject(list));
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpGet("ipca")]
        [AllowAnonymous]
        public async Task<ActionResult> GetIPCA()
        {
            try
            {
                List<Ipca> list = collectorService.Get_Ipca_Monthly_Variation_Last_12_Periods();

                if (list == null)
                {
                    HttpClient request = new HttpClientBuilder().Build();
                    var responseIBGEAPI = await request.GetAsync(Links.GetLink(Link.IPCA_MONTHLY_VARIATION_LAST_12_PERIODS));

                    var statusCode = responseIBGEAPI.StatusCode;
                    if (statusCode != HttpStatusCode.OK)
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, TxTResponses.GetTxTResponse(TxTResponse.Failure_GetIndicator));
                    }

                    list = JsonConvert.DeserializeObject<List<Ipca>>(JToken.Parse(responseIBGEAPI.Content.ReadAsStringAsync().Result).ToString());

                    collectorService.Set_Ipca_Monthly_Variation_Last_12_Periods(list);
                }
                return Ok(JsonConvert.SerializeObject(list));
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpGet("lastannualizedselic252")]
        [AllowAnonymous]
        public async Task<ActionResult> LastAnnualizedSelic252()
        {
            try
            {
                Selic selic = collectorService.Get_Selic_Last_Annualized_252();

                if (selic == null)
                {
                    HttpClient request = new HttpClientBuilder().Build();
                    var responseBCB = await request.GetAsync(Links.GetLink(Link.SELIC_ANUALIZADA_BASE_252_LAST_VALUE));

                    var statusCode = responseBCB.StatusCode;
                    if (statusCode != HttpStatusCode.OK)
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, TxTResponses.GetTxTResponse(TxTResponse.Failure_GetIndicator));
                    }

                    selic = JsonConvert.DeserializeObject<List<Selic>>(JToken.Parse(responseBCB.Content.ReadAsStringAsync().Result).ToString())[0];

                    collectorService.Set_Selic_Last_Annualized_252(selic);
                }
                return Ok(JsonConvert.SerializeObject(selic));
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpGet("lastannualavarageipca")]
        [AllowAnonymous]
        public async Task<ActionResult> LastAnnualAvarageIPCA()
        {
            try
            {
                Ipca ipca = collectorService.Get_Ipca_Annual_Average_Last_Period();

                if (ipca == null)
                {
                    HttpClient request = new HttpClientBuilder().Build();
                    var responseIBGE = await request.GetAsync(Links.GetLink(Link.IPCA_ANNUAL_AVERAGE_LAST_PERIOD));

                    var statusCode = responseIBGE.StatusCode;
                    if (statusCode != HttpStatusCode.OK)
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, TxTResponses.GetTxTResponse(TxTResponse.Failure_GetIndicator));
                    }

                    ipca = JsonConvert.DeserializeObject<List<Ipca>>(JToken.Parse(responseIBGE.Content.ReadAsStringAsync().Result).ToString())[1];

                    collectorService.Set_Ipca_Annual_Average_Last_Period(ipca);
                }

                return Ok(JsonConvert.SerializeObject(ipca));
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpGet("ipcatotal")]
        [AllowAnonymous]
        public async Task<ActionResult> GetIPCATotal()
        {
            try
            {
                IpcaCalculated ipcaCalculate = collectorService.Get_Ipca_Calculated_For_Investiments();

                if (ipcaCalculate == null)
                {
                    int currentMonth = DateTime.Now.Month;
                    string monthBase = (currentMonth - 1).ToString("00");
                    int currentYear = DateTime.Now.Year;

                    string startDate = (currentYear - 3).ToString() + monthBase;
                    string finalDate = currentYear.ToString() + monthBase;

                    string linkBase = Links.GetLink(Link.IPCA_MONTHLY_VARIATION_IN_PERIODS).Replace("{0}", startDate).Replace("{1}", finalDate);

                    HttpClient request = new HttpClientBuilder().Build();
                    var responseIBGEAPI = await request.GetAsync(linkBase);

                    var statusCode = responseIBGEAPI.StatusCode;
                    if (statusCode != HttpStatusCode.OK)
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, TxTResponses.GetTxTResponse(TxTResponse.Failure_GetIndicator));
                    }

                    List<Ipca> ipca36List = new List<Ipca>();
                    ipca36List = JsonConvert.DeserializeObject<List<Ipca>>(JToken.Parse(responseIBGEAPI.Content.ReadAsStringAsync().Result).ToString());
                    ipca36List.Remove(ipca36List[0]);

                    double[] ipca36ListValues = ipca36List.Select(x => double.Parse(x.V, CultureInfo.InvariantCulture)).ToArray();
                    //double ipcaIndex36 = ipca36ListValues.Aggregate((total, x) => total + x);
                    double ipca36Value = ipca36ListValues.Sum();

                    List<Ipca> ipca24List = ipca36List.Where(x => int.Parse(x.D2C) >= int.Parse((currentYear - 2).ToString() + monthBase)).ToList();
                    double[] ipca24ListValues = ipca24List.Select(x => double.Parse(x.V, CultureInfo.InvariantCulture)).ToArray();
                    double ipca24Value = ipca24ListValues.Sum();

                    List<Ipca> ipca12List = ipca24List.Where(x => int.Parse(x.D2C) >= int.Parse((currentYear - 1).ToString() + monthBase)).ToList();
                    double[] ipca12ListValues = ipca12List.Select(x => double.Parse(x.V, CultureInfo.InvariantCulture)).ToArray();
                    double ipca12Value = ipca12ListValues.Sum();

                    double ipcaBaseValue = ipca12ListValues[ipca12ListValues.Length - 1];

                    IpcaCalculated ipcaCalculate2 = new IpcaCalculated();
                    ipcaCalculate2.month = ipcaBaseValue;
                    ipcaCalculate2.month12 = ipca12Value;
                    ipcaCalculate2.month24 = ipca24Value;
                    ipcaCalculate2.month36 = ipca36Value;

                    collectorService.Set_Ipca_Calculated_For_Investiments(ipcaCalculate2);

                    return Ok(JsonConvert.SerializeObject(ipcaCalculate2));
                }

                return Ok(JsonConvert.SerializeObject(ipcaCalculate));
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }


        private JObject ColectBB()
        {
            JObject response = new JObject();
            response.Add("shortTerm", ColectBBTable(3, "Fundos com carteira de curto prazo", null));
            response.Add("longTerm", ColectBBTable(5, "Fundos com carteira de longo prazo", null));
            response.Add("stock", ColectBBTable(8, "Fundos", "Ações"));
            collectorService.Set_BBInvestiments(response);

            return response;
        }

        private JObject ColectBBTable(int tablePosition, string headerCaption, string? defaultType)
        {
            try
            {
                var wc = new WebClient();
                wc.Encoding = Encoding.UTF8;

                string page = System.Web.HttpUtility.HtmlDecode(wc.DownloadString(Links.GetLink(Link.BB_PAGE)));

                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(page);

                JObject tableData = new JObject();
                List<string> headerTable = new List<string>();
                string type = defaultType != null ? defaultType : string.Empty;


                foreach (HtmlNode node in htmlDocument.QuerySelectorAll($"#formulario > table:nth-child({tablePosition}) > thead > tr:nth-child(2) > th"))
                {
                    headerTable.Add(node.InnerText.Trim());
                }

                InvestmentFundHeader header = new InvestmentFundHeader();
                header.name = headerTable[0];
                header.day = headerTable[1];
                header.accumulatedMonth = headerTable[2];
                header.month = headerTable[3];
                header.year = headerTable[4];
                header.month12 = headerTable[5];
                header.month24 = headerTable[6];
                header.month36 = headerTable[7];
                header.month12_PL_Avarage = headerTable[8];
                header.annualAdministrationFee = headerTable[9];
                header.quotaDate = headerTable[10];
                header.quotaValue = headerTable[11];
                header.startDate = headerTable[12];
                List<InvestmentFund> bodyData = new List<InvestmentFund>();

                foreach (HtmlNode trNode in htmlDocument.QuerySelectorAll($"#formulario > table:nth-child({tablePosition}) > tbody > tr"))
                {

                    var totalElements = trNode.GetChildElements();

                    if (trNode.GetChildElements().Count() == 1)
                    {
                        type = trNode.InnerText.Trim();
                    }
                    else
                    {
                        InvestmentFund investmentFundItem = new InvestmentFund();
                        List<string> itemData = new List<string>();

                        foreach (HtmlNode tdNode in trNode.GetChildElements())
                        {
                            itemData.Add(tdNode.InnerText.Trim());
                        }

                        investmentFundItem.name = itemData[0].Replace("\n", "").Replace("\t", "");
                        investmentFundItem.day = itemData[1] != null && itemData[1] != "" && itemData[1] != "-" ? double.Parse(itemData[1].Replace(",", "."), CultureInfo.InvariantCulture) : 0.00;
                        investmentFundItem.accumulatedMonth = itemData[2] != null && itemData[2] != "" && itemData[2] != "-" ? double.Parse(itemData[2].Replace(",", "."), CultureInfo.InvariantCulture) : 0.00;
                        investmentFundItem.month = itemData[3] != null && itemData[3] != "" && itemData[3] != "-" ? double.Parse(itemData[3].Replace(",", "."), CultureInfo.InvariantCulture) : 0.00;
                        investmentFundItem.year = itemData[4] != null && itemData[4] != "" && itemData[4] != "-" ? double.Parse(itemData[4].Replace(",", "."), CultureInfo.InvariantCulture) : 0.00;
                        investmentFundItem.month12 = itemData[5] != null && itemData[5] != "" && itemData[5] != "-" ? double.Parse(itemData[5].Replace(",", "."), CultureInfo.InvariantCulture) : 0.00;
                        investmentFundItem.month24 = itemData[6] != null && itemData[6] != "" && itemData[6] != "-" ? double.Parse(itemData[6].Replace(",", "."), CultureInfo.InvariantCulture) : 0.00;
                        investmentFundItem.month36 = itemData[7] != null && itemData[7] != "" && itemData[7] != "-" ? double.Parse(itemData[7].Replace(",", "."), CultureInfo.InvariantCulture) : 0.00;
                        investmentFundItem.month12_PL_Avarage = itemData[8] != null && itemData[8] != "" && itemData[8] != "-" ? int.Parse(itemData[8].Replace(".", ""), CultureInfo.InvariantCulture) : 0;
                        investmentFundItem.annualAdministrationFee = itemData[9];
                        investmentFundItem.quotaDate = itemData[10];
                        investmentFundItem.quotaValue = itemData[11] != null && itemData[11] != "" && itemData[11] != "-" ? double.Parse(itemData[11].Replace(",", "."), CultureInfo.InvariantCulture) : 0.00;
                        investmentFundItem.startDate = itemData[12];
                        investmentFundItem.type = type;

                        bodyData.Add(investmentFundItem);
                    }
                }

                tableData.Add("name", headerCaption);
                tableData.Add("header", JToken.FromObject(header));
                tableData.Add("data", JToken.FromObject(bodyData));

                return tableData;
            }
            catch (Exception e)
            {
                return null;
            }

        }


    }
}
