namespace LuisBot.Dialogs
{
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.FormFlow;
    using Microsoft.Bot.Builder.Luis;
    using Microsoft.Bot.Builder.Luis.Models;
    using Microsoft.Bot.Connector;
    using System.Data.SqlClient;
    using System.Data;
    using System.Net;
    using Newtonsoft.Json;

    [LuisModel("32d3e1e6-dac9-4998-a21f-be62485eb2ee", "88d4bd09440c47c0a92da63bff14b6ac",LuisApiVersion.V2,"southeastasia.api.cognitive.microsoft.com")]
    [Serializable]
    public class RootLuisDialog : LuisDialog<object>
    {
        public const string Job = "职务";
        public const string Department = "院系";
        public const string Name = "姓名";
        public const string Before = "任序";
        public const string Phone = "电话";
        public const string Dbconnect = "Server=tcp:databaseforchenlu.database.windows.net,1433;Initial Catalog=chenludata;Persist Security Info=False;User ID=chenlu;Password=123321qw3e?;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        public int pre;


        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"不好意思，不太能理解你说的话";

            await context.PostAsync(message);

            context.Wait(this.MessageReceived);
        }

        [LuisIntent("greetings")]
        public async Task greetings(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("你好，我是南京理工大学的bot");

            context.Wait(this.MessageReceived);
        }
        
        [LuisIntent("询问领导")]
        public async Task askforjob(IDialogContext context, LuisResult result)
        {
            EntityRecommendation job, department,name,before;

            if (result.TryFindEntity(Job, out job) && result.TryFindEntity(Department, out department) && result.TryFindEntity(Before, out before))
            {
                string t = getvalue(department);
                SqlConnection c = new SqlConnection(Dbconnect);
                c.Open();
                SqlCommand cmd = c.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "select * from 花名册 where 职务=N'" + job.Entity + "' AND 院系 = N'" + t + "' and 前任 is not null";

                

                SqlDataReader ans = cmd.ExecuteReader();
                
                if (!ans.HasRows)
                {
                    await context.PostAsync($"查询不到{t}的{job.Entity}的前任是谁，官网没有这条信息");
                }
                else
                {
                    ans.Read();
                    pre = (int )ans["ID"];
                    string re = ans["前任"] as string;
                    await context.PostAsync($"{t}的{job.Entity}的{before.Entity}是{re}");
                }
                
                c.Close();
               
            }

            if (result.TryFindEntity(Job, out job)  && result.TryFindEntity(Before, out before))
            {
                //string t = getvalue(department);
                SqlConnection c = new SqlConnection(Dbconnect);
                c.Open();
                SqlCommand cmd = c.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "select * from 花名册 where 职务=N'" + job.Entity + "' and 前任 is not null";



                SqlDataReader ans = cmd.ExecuteReader();

                if (!ans.HasRows)
                {
                    await context.PostAsync($"查询不到我们学校的{job.Entity}的前任是谁，官网没有这条信息");
                }
                else
                {
                    ans.Read();
                    pre = (int)ans["ID"];
                    string re = ans["前任"] as string;
                    await context.PostAsync($"我们学校的{job.Entity}的{before.Entity}是{re}");
                }

                c.Close();

            }

            else if (result.TryFindEntity(Job, out job) && result.TryFindEntity(Department, out department) && result.TryFindEntity(Name, out name))
            {
                string t = getvalue(department);
                SqlConnection c = new SqlConnection(Dbconnect);
                c.Open();
                SqlCommand cmd = c.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "select * from 花名册 where 职务=N'" + job.Entity + "' AND 院系 = N'" + t + "'";

                bool flag = false;
                SqlDataReader ans = cmd.ExecuteReader();
                
                if (!ans.HasRows)
                {
                    await context.PostAsync($"查询不到{t}的{job.Entity}是谁，官网没有这条信息");
                }
                while (ans.Read())
                {
                    string re = ans["姓名"] as string;
                    if (re == name.Entity)
                    {
                        await context.PostAsync($"是的，{re}是{t}的{job.Entity}");
                        flag = true;
                    }
                }
                if (flag == false)
                {
                    await context.PostAsync($"不是，{name.Entity}不是{t}的{job.Entity}");
                }
                c.Close();
                
            }

            else if( result.TryFindEntity(Job,out job) && !result.TryFindEntity(Department,out department))
            {
                SqlConnection c = new SqlConnection(Dbconnect);
                c.Open();
                SqlCommand cmd = c.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "select * from 花名册 where 职务=N'" + job.Entity + "'";

                SqlDataReader ans = cmd.ExecuteReader();
                
                if (!ans.HasRows)
                {
                    await context.PostAsync($"查询不到我们学校的{job.Entity}是谁，官网没有这条信息");
                }
                string re = "我们学校的" + job.Entity + "是";

                while (ans.Read())
                {
                    pre = (int)ans["ID"];
                    re += ans["姓名"] as string;
                    
                }
                await context.PostAsync($"{re}");
            }

            
            else if(result.TryFindEntity(Job, out job) && result.TryFindEntity(Department, out department))
            {
               
                string t = getvalue(department);
                //await context.PostAsync($"{t}");
                SqlConnection c = new SqlConnection(Dbconnect);
                c.Open();
                SqlCommand cmd = c.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "select * from 花名册 where 职务=N'" + job.Entity + "' AND 院系 = N'"+ t +"'";
                
                SqlDataReader ans = cmd.ExecuteReader();
                
                if(!ans.HasRows)
                {
                    await context.PostAsync($"查询不到{t}的{job.Entity}是谁，官网没有这条信息");
                }
                string re = t + "的" + job.Entity + "是";
               
                while (ans.Read())
                {
                    pre = (int)ans["ID"];
                    re += ans["姓名"] as string;
                    
                }
                await context.PostAsync($"{re}");
                c.Close();
                
                
            }
            else
            {
                await context.PostAsync("请明确要查询的院系和职务");
            }
            //o:;
            context.Wait(this.MessageReceived);
        }

        [LuisIntent("问地点")]
        public async Task asklocation(IDialogContext context, LuisResult result)
        {
            await qna(result.Query, context);
            context.Wait(this.MessageReceived);
        }

        [LuisIntent("问数量")]
        public async Task asknum(IDialogContext context, LuisResult result)
        {
            await qna(result.Query, context);
            context.Wait(this.MessageReceived);
        }

        [LuisIntent("问年份")]
        public async Task askyear(IDialogContext context, LuisResult result)
        {
            await qna(result.Query, context);
            context.Wait(this.MessageReceived);
        }

        [LuisIntent("名词解释")]
        public async Task askn(IDialogContext context, LuisResult result)
        {
            await qna(result.Query, context);
            context.Wait(this.MessageReceived);
        }

        [LuisIntent("问电话")]
        public async Task askphone(IDialogContext context, LuisResult result)
        {
            EntityRecommendation department;
            EntityRecommendation name;

            if (result.TryFindEntity(Department, out department))
            {
                string s = getvalue(department);
                SqlConnection d = new SqlConnection(Dbconnect);
                d.Open();
                SqlCommand cmd2 = d.CreateCommand();
                cmd2.CommandType = CommandType.Text;
                cmd2.CommandText = "select * from 花名册 where 院系 = N'" + s + "'" + "and 电话 is not null";
                SqlDataReader ans2 = cmd2.ExecuteReader();

                if (!ans2.HasRows)
                {
                    await context.PostAsync($"官网找不到{s}的电话");
                }
                else
                {
                    ans2.Read();
                    string re = ans2["电话"] as string;
                    await context.PostAsync($"{s}的电话是{re}");
                }
            }
            else if(result.TryFindEntity(Name,out name))
            {
                string s = name.Entity;
                SqlConnection d = new SqlConnection(Dbconnect);
                d.Open();
                SqlCommand cmd2 = d.CreateCommand();
                cmd2.CommandType = CommandType.Text;
                cmd2.CommandText = "select * from 花名册 where 姓名 = N'" + s + "'" + "and 电话 is not null";
                SqlDataReader ans2 = cmd2.ExecuteReader();

                if (!ans2.HasRows)
                {
                    await context.PostAsync($"官网找不到{s}的电话");
                }
                else
                {
                    ans2.Read();
                    pre = (int)ans2["ID"];
                    string re = ans2["电话"] as string;
                    await context.PostAsync($"{s}的电话是{re}");
                }
            }
            context.Wait(this.MessageReceived);
        }

        [LuisIntent("多轮")]
        private async Task previous(IDialogContext context, LuisResult result)
        {

            EntityRecommendation before;
            EntityRecommendation phone;
            if(result.TryFindEntity(Before,out before))
            {
                string t = getvalue(before);
                if (t == "前任")
                {

                    
                    SqlConnection c = new SqlConnection(Dbconnect);
                    c.Open();
                    SqlCommand cmd = c.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "select * from 花名册 where ID = " + pre.ToString() + "and 前任 is not null";
                    
                    

                    SqlDataReader ans = cmd.ExecuteReader();

                    if (!ans.HasRows)
                    {
                        await context.PostAsync($"找不到前任");
                    }
                    else
                    {
                        ans.Read();
                        await context.PostAsync($"{ans["姓名"]}的前任是{ans["前任"]}");
                        string s = ans["前任"] as string;
                       

                        SqlConnection d = new SqlConnection(Dbconnect);
                        d.Open();
                        SqlCommand cmd2 = d.CreateCommand();
                        cmd2.CommandType = CommandType.Text;
                        cmd2.CommandText = "select * from 花名册 where 姓名 = N'" + s + "'";
                        SqlDataReader ans2 = cmd2.ExecuteReader();
                        ans2.Read();
                        pre = (int)ans2["ID"];

                        d.Close();
                    }
                    c.Close();
                }
                else if(t == "下任")
                {
                    //pre.Read();
                    SqlConnection c = new SqlConnection(Dbconnect);
                    c.Open();
                    SqlCommand cmd = c.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "select * from 花名册 where ID = " + pre.ToString();
                    SqlDataReader ans = cmd.ExecuteReader();
                    ans.Read();
                    string s = ans["姓名"] as string;
                    c.Close();

                    SqlConnection d = new SqlConnection(Dbconnect);
                    d.Open();
                    SqlCommand cmd2 = d.CreateCommand();
                    cmd2.CommandType = CommandType.Text;
                    cmd2.CommandText = "select * from 花名册 where 前任 = N'" + s + "'";
                    SqlDataReader ans2 = cmd2.ExecuteReader();
                    if(ans2.HasRows)
                    ans2.Read();
                    pre = (int)ans2["ID"];
                    
                    

                    await context.PostAsync($"{s}的下任是{ans2["姓名"]}");
                    d.Close();
                }
                
            }
            else if(result.TryFindEntity(Phone,out phone))
            {
                
                SqlConnection d = new SqlConnection(Dbconnect);
                d.Open();
                SqlCommand cmd2 = d.CreateCommand();
                cmd2.CommandType = CommandType.Text;
                cmd2.CommandText = "select * from 花名册 where ID = " + pre.ToString() + "and 电话 is not null";
                SqlDataReader ans2 = cmd2.ExecuteReader();

                if (!ans2.HasRows)
                {
                    await context.PostAsync($"官网找不到他的电话");
                }
                else
                {
                    ans2.Read();
                    pre = (int)ans2["ID"];
                    string re = ans2["电话"] as string;
                    await context.PostAsync($"{ans2["姓名"]}的电话是{re}");
                }
            }
            context.Wait(this.MessageReceived);
        }

        private async Task qna(string x,IDialogContext context)
        {
            x = x.Replace("你们学校", "贵校");
            x = x.Replace("你们", "贵校");
            x = x.Replace("你校", "贵校");
            string responseString = string.Empty;

            var query = x; 
            var knowledgebaseId = "c2e66af1-6824-4bd8-868b-f2086383b00f"; 
            var qnamakerSubscriptionKey = "b94a33fb607b402c892fb96e11b97122"; 

           

           
            Uri qnamakerUriBase = new Uri("https://westus.api.cognitive.microsoft.com/qnamaker/v1.0");
            var builder = new UriBuilder($"{qnamakerUriBase}/knowledgebases/{knowledgebaseId}/generateAnswer");

            
            var postBody = $"{{\"question\": \"{query}\"}}";
            QnAMakerResult re;
           
            using (WebClient client = new WebClient())
            {
        
                client.Encoding = System.Text.Encoding.UTF8;

                
                client.Headers.Add("Ocp-Apim-Subscription-Key", qnamakerSubscriptionKey);
                client.Headers.Add("Content-Type", "application/json");
                responseString = client.UploadString(builder.Uri, postBody);
                re = JsonConvert.DeserializeObject<QnAMakerResult>(responseString);

            }
            await context.PostAsync(re.Answer);
        }

        

        private class QnAMakerResult
        {
            [JsonProperty(PropertyName = "answer")]
            public string Answer { get; set; }

            [JsonProperty(PropertyName = "score")]
            public double Score { get; set; }
        }

        private string getvalue(EntityRecommendation x)
        {
            IDictionary<string, object> s = x.Resolution;
            JArray ss = s["values"] as JArray;
            string t = ss[0].ToString();
            return t;
        }

    }
}
