using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
namespace SimpleJson_impala_webapi
{
    public class App
    {

        private static long tbInterval = 60000;
        private static long tsInterval = 180000;
        public static Dictionary<string, string> sqllist = new Dictionary<string, string>();
        public static Dictionary<string, DateTime> datatick = new Dictionary<string, DateTime>();
        public static Dictionary<string, object> datalist = new Dictionary<string, object>();
        public static Dictionary<string, Task> tasklist = new Dictionary<string, Task>();
        public static void Run()
        {
            DbHelper.InitConnect();
            List<string> keylist = sqllist.Keys.ToList();
            for (int i = 0; i < keylist.Count; i++)
            {
                Console.WriteLine("load sqlname:" + sqllist[keylist[i]]);
            }
        }

        public static List<object> GetQueryResult(object req) 
        {
            List<object> result = new List<object>();
            try
            {
                var json = JsonConvert.DeserializeObject<JObject>(req.ToString());
                var targets = json["targets"].Value<JArray>();
                if (targets == null)
                    return result;
                for (int i = 0; i < targets.Count; i++)
                {
                    string target = targets[i]["target"].ToString();
                    string type = targets[i]["type"].ToString();
                    result.Add(App.GetResult(target, type));
                }
            }
            catch
            {

            }
            return result;
        }
        private static void UpdateData(string target, string type)
        {   
            datalist[target + "^" + type] = GetData(target, type);
            datatick[target + "^" + type] = DateTime.Now;
        }
        private static void UpdateData()
        {
            List<string> keylist = datalist.Keys.ToList();
            for (int i = 0; i < datalist.Count; i++) 
            {
                string[] keys = keylist[i].Split('^');
                datalist[keylist[i]] = GetData(keys[0], keys[1]);
            }
        }
        private static object GetData(string target, string type)
        {
            DataTable dt = DbHelper.ExecuteDataTable(sqllist[target]);
            if (type == "timeserie")
            {
                List<List<object>> datapoints = new List<List<object>>();
                foreach (DataRow dr in dt.Rows)
                {
                    datapoints.Add(new List<object>(dr.ItemArray));
                }
                TimeserieResultModel timeResult = new TimeserieResultModel();
                timeResult.target = target;
                timeResult.datapoints = datapoints;
                return timeResult;
            }
            else
            {
                List<ColumnModel> columns = new List<ColumnModel>();
                List<List<object>> rows = new List<List<object>>();
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    columns.Add(new ColumnModel(dt.Columns[i].ColumnName, "string"));
                }
                foreach (DataRow dr in dt.Rows)
                {
                    rows.Add(new List<object>(dr.ItemArray));
                }
                TableResultModel tableResult = new TableResultModel();
                tableResult.columns = columns;
                tableResult.rows = rows;
                tableResult.type = "table";
                return tableResult;
            }
        }
        public static object GetResult(string target, string type) 
        {
            if (datalist.ContainsKey(target + "^" + type))
            {
                long ts = (long)(DateTime.Now - datatick[target + "^" + type]).TotalMilliseconds;
                if (type == "timeserie")
                {
                    if (ts > tsInterval) 
                    {
                        if (tasklist[target + "^" + type].Status != TaskStatus.Running)
                        {
                            tasklist[target + "^" + type] = new Task(() => UpdateData(target, type));
                            tasklist[target + "^" + type].Start();
                        }
                        
                    }
                }
                else 
                {
                    if (ts > tbInterval)
                    {
                        if (tasklist[target + "^" + type].Status != TaskStatus.Running)
                        {
                            tasklist[target + "^" + type] = new Task(() => UpdateData(target, type));
                            tasklist[target + "^" + type].Start();
                        }
                    }
                }
                return datalist[target + "^" + type];
            }
            else 
            {
                object data = GetData(target, type);
                datalist.Add(target + "^" + type, data);
                datatick.Add(target + "^" + type, DateTime.Now);
                tasklist.Add(target + "^" + type, new Task(() => UpdateData(target, type)));
                return data;
            }
        }
    }
    public static class JsonUntity
    {
        public static string SerializeDictionaryToJsonString<TKey, TValue>(Dictionary<TKey, TValue> dict)
        {
            if (dict.Count == 0)
                return "";
            string jsonStr = JsonConvert.SerializeObject(dict);
            return jsonStr;
        }
        public static Dictionary<TKey, TValue> DeserializeStringToDictionary<TKey, TValue>(string jsonStr)
        {
            if (string.IsNullOrEmpty(jsonStr))
                return new Dictionary<TKey, TValue>();
            Dictionary<TKey, TValue> jsonDict = JsonConvert.DeserializeObject<Dictionary<TKey, TValue>>(jsonStr);
            return jsonDict;
        }
    }
    public class TimeserieResultModel
    {
        public string target { get; set; }
        public List<List<object>> datapoints { get; set; }

    }
    public class TableResultModel
    {
        public List<ColumnModel> columns { get; set; }
        public List<List<object>> rows { get; set; }
        public string type  { get; set; }

    }
    public class ColumnModel
    {
        public ColumnModel(string text,string type)
        {
            this.text = text;
            this.type = type;
        }
        public string text { get; set; }
        public string type { get; set; }

    }
}
