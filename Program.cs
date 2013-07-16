using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NetDimension.Weibo;


namespace WeiboConsole
{
    class Program
    {
        static string AppKey = Settings.Default.AppKey;
        static string AppSecret = Settings.Default.AppSecrect;


        static void Main(string[] args)
        {
            OAuth oauth = null;
            string accessToken = Settings.Default.AccessToken;
            if (string.IsNullOrEmpty(accessToken))	//判断配置文件中有没有保存到AccessToken，如果没有就进入授权流程
            {
                oauth = Authorize();

                if (!string.IsNullOrEmpty(oauth.AccessToken))
                {

                    Console.WriteLine("获取AccessToken{{{0}}}成功！", oauth.AccessToken);
                    Console.Write("保存Token到配置文件可以避免每次运行程序都授权，是否保存？[y/n]:");
                    string KeyStr = Console.ReadKey(true).Key.ToString();
                    Console.Write(KeyStr);
                    if (KeyStr.Equals("Y"))// Console.ReadKey(true).Key == ConsoleKey.Y)
                    {
                        //Console.Write(Console.ReadKey(true).Key.ToString());
                        Settings.Default.AccessToken = oauth.AccessToken;
                        Settings.Default.Save();
                        Console.WriteLine();
                        Console.WriteLine("配置文件已保存。如果要撤销AccessToken请删除ConsoleApp.exe.config中AcceessToken节点中的Token值。");
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine("AccessToken未保存。");
                        Console.WriteLine();
                    }
                }
                
            }
            else//如果配置文件中保存了AccesssToken
            {
                Console.WriteLine("获取到已保存的AccessToken{{{0}}}！", accessToken);
                oauth = new OAuth(AppKey, AppSecret, accessToken, "");	//用Token实例化OAuth无需再次进入验证流程
                Console.WriteLine("验证Token有效性...");
                TokenResult result = oauth.VerifierAccessToken();	//测试保存的AccessToken的有效性
                if (result == TokenResult.Success)
                {
                    Console.WriteLine("AccessToken有效！");
                    Console.Write("删除已保存的AccessToken可以再下次运行程序时演示授权过程，是否删除？[y/n]:");
                    string KeyStr = Console.ReadKey(true).Key.ToString();
                    Console.Write(KeyStr);
                    if (KeyStr.Equals("Y"))//Console.ReadKey(true).Key == ConsoleKey.Y)
                    {
                        //如果想演示下登录授权，那就得把配置文件中的Token删除，所以做那么一个判断。
                        Settings.Default.AccessToken = string.Empty;
                        Settings.Default.Save();
                        Console.WriteLine();
                        Console.WriteLine("已从配置文件移除AccessToken值，重新运行程序来演示授权过程。");
                    }
                    else
                    {
                        Console.WriteLine();
                        Console.WriteLine("继续执行。");
                    }
                }
                else
                {
                    Console.WriteLine("AccessToken无效！因为：{0}", result);
                    Settings.Default.AccessToken = string.Empty;
                    Settings.Default.Save();
                    Console.WriteLine("已从配置文件移除AccessToken值，重新运行程序获取有效的AccessToken");
                    return; //AccessToken无效，继续执行没有任何意义，反正也无法调用API
                }
            }
            //好吧，授权至此应该成功了。下一步调用接口吧。
            Client Sina = new Client(oauth);
            try
            {

                //.Net其他版本
                string uid = Sina.API.Entity.Account.GetUID();	//获取UID
                //这里用VS2010的var关键字和可选参数最惬意了。
                //如果用VS2005什么的你得这样写：
                //NetDimension.Weibo.Entities.user.Entity userInfo = Sina.API.Users.Show(uid,null);
                //如果用VS2008什么的也不方便，你得把参数写全：
                //var userInfo = Sina.API.Users.Show(uid,null);
                var userInfo = Sina.API.Entity.Users.Show(uid);
                Console.WriteLine();
                Console.WriteLine("昵称：{0}", userInfo.ScreenName);
                Console.WriteLine("来自：{0}", userInfo.Location);
                Console.WriteLine("-----------------------------------");
                Console.WriteLine("请选择操作：F 发微博｜G 获取微博：");
                if (Console.ReadKey(true).Key == ConsoleKey.F)
                {
                    Console.WriteLine("F 发微博：");
                    Console.WriteLine();
                    string UpdateText = Console.ReadLine();
                    var statusInfo = Sina.API.Entity.Statuses.Update(UpdateText);
                    Console.WriteLine();

                    Console.WriteLine("微博已发送，发送时间：{0}", statusInfo.CreatedAt);


                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine("功能完善中...");
                }
            }
            catch (WeiboException ex)
            {
                Console.WriteLine("出错啦！" + ex.Message);
            }
            Console.WriteLine();
            Console.Write("演示结束，按任意键继续...");
            Console.ReadKey();
        }
        static OAuth Authorize()
        {
            OAuth o = new OAuth(Settings.Default.AppKey, Settings.Default.AppSecrect, Settings.Default.CallbackUrl);

            //让使用者自行选择一个授权方式

            //Console.WriteLine("请选择授权模式");
            //Console.WriteLine("1. 标准授权方式");
            //Console.WriteLine("2. 模拟授权方式");//一键授权登录
            //ConsoleKeyInfo key = Console.ReadKey(true);
            //Console.WriteLine();

            //if (key.Key == ConsoleKey.D2)
            if (true)
            {
                if (!ClientLogin(o))	//使用模拟方法
                {
                    Console.WriteLine("授权登录失败，请重试。");
                }
                else
                    Console.WriteLine("授权成功，获取AccessToken...");

            }
            else
            {
                string authorizeUrl = o.GetAuthorizeURL();
                System.Diagnostics.Process.Start(authorizeUrl);
                Console.Write("复制浏览器中的Code:");
                string code = Console.ReadLine();
                try
                {
                    AccessToken accessToken = o.GetAccessTokenByAuthorizationCode(code); //请注意这里返回的是AccessToken对象，不是string
                }
                catch (WeiboException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return o;
        }

        private static bool ClientLogin(OAuth o)
        {
            Console.Write("微博账号:380282341@qq.com");
            //string passport = Console.ReadLine();
            string passport = "380282341@qq.com";
            Console.WriteLine(" 登录密码:******");

            //ConsoleColor originColor = Console.ForegroundColor;
            //Console.ForegroundColor = Console.BackgroundColor; //知道这里是在干啥不？其实是为了不让你们看到我的密码^_^

            Console.ForegroundColor = ConsoleColor.Green;
            string password = "369258";
            //string password = Console.ReadLine();

            //Console.ForegroundColor = originColor; //恢复前景颜色。

            return o.ClientLogin(passport, password);
        }
    }
}