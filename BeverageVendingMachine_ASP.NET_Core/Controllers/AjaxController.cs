using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BeverageVendingMachine_ASP.NET_Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace BeverageVendingMachine_ASP.NET_Core.Controllers
{
    public class Balance
    {
        private Coins coins;
        private int balance;
        private enum COINS
        {
            Coin_1,
            Coin_2,
            Coin_5,
            Coin_10,
            Undefined
        }
        public Balance()
        {
            coins = new Coins(50,50,50,50);
            balance = 0;
        }
        public void AddBalance(byte N)
        {
            bool flag = false;
            
            switch (N)
            {
                case 1:
                    if (coins.Coin_1 <= 99)
                    {
                        coins.Coin_1++;
                        flag = true;
                    }
                    break;
                case 2:
                    if (coins.Coin_2 <= 99)
                    {
                        coins.Coin_2++;
                        flag = true;
                    }
                    break;
                case 5:
                    if (coins.Coin_5 <= 99)
                    {
                        coins.Coin_5++;
                        flag = true;
                    }
                    break;
                case 10:
                    if (coins.Coin_10 <= 99)
                    {
                        coins.Coin_10++;
                        flag = true;
                    }
                    break;
            }

            if(flag)
                balance += N;
        }
        public bool TakeBalance(int N)
        {
            if (balance - N >= 0)
                balance -= N;
            else
                return false;
            return true;
        }
        public string CheckingCoins()
        {
            string result = "";
            if (coins.Coin_1 > 99)
                result += result + "C1.";
            if (coins.Coin_2 > 99)
                result += result + "C2.";
            if (coins.Coin_5 > 99)
                result += result + "C5.";
            if (coins.Coin_10 > 99)
                result += result + "C10.";
            return result;
        }

        public string GetSurrender()
        {
            int more;
            byte C1 = 0;
            byte C2 = 0;
            byte C5 = 0;
            byte C10 = 0;

            while (Startup.balance.GetBalance() > 0)
            {
                more = 0;
                COINS C = COINS.Undefined;
                if (coins.Coin_1 > more)
                {
                    more = coins.Coin_1;
                    C = COINS.Coin_1;
                }
                if (coins.Coin_2 > more)
                {
                    more = coins.Coin_2;
                    C = COINS.Coin_2;
                }
                if (coins.Coin_5 > more)
                {
                    more = coins.Coin_5;
                    C = COINS.Coin_5;
                }
                if (coins.Coin_10 > more)
                {
                    more = coins.Coin_10;
                    C = COINS.Coin_10;
                }

                switch (C)
                {
                    case COINS.Coin_1:
                        if (TakeBalance(1))
                        {
                            coins.Coin_1--;
                            C1++;
                        }
                        break;
                    case COINS.Coin_2:
                        if (TakeBalance(2))
                        {
                            coins.Coin_2--;
                            C2++;
                        }
                        break;
                    case COINS.Coin_5:
                        if (TakeBalance(5))
                        {
                            coins.Coin_5--;
                            C5++;
                        }
                        break;
                    case COINS.Coin_10:
                        if (TakeBalance(10))
                        {
                            coins.Coin_10--;
                            C10++;
                        }
                        break;
                    default:
                        return "Error";
                }

                if(Startup.balance.GetBalance()==1)
                {
                    TakeBalance(1);
                    coins.Coin_1--;
                    C1++;
                } else if (Startup.balance.GetBalance() == 2)
                {
                    TakeBalance(2);
                    coins.Coin_2--;
                    C2++;
                }else if (Startup.balance.GetBalance() == 3)
                {
                    TakeBalance(1);
                    coins.Coin_1--;
                    C1++;
                    TakeBalance(2);
                    coins.Coin_2--;
                    C2++;
                }else if (Startup.balance.GetBalance() == 4)
                {
                    TakeBalance(2);
                    coins.Coin_2--;
                    C2++;
                    TakeBalance(2);
                    coins.Coin_2--;
                    C2++;
                }else if (Startup.balance.GetBalance() == 5)
                {
                    TakeBalance(5);
                    coins.Coin_5--;
                    C5++;
                }
            }
            return C1 + "_" + C2 + "_" + C5 + "_" + C10;
        }
        
    public int GetBalance()
        {
            return balance;
        }
    }

    [Route("[controller]")]
    [ApiController]
    public class AjaxController : Controller
    {
        private ApplicationContext db;
        [HttpGet]
        public ActionResult<string> Get()
        {
            return Startup.balance.GetBalance() + "";
        }

        [HttpPost]
        public ActionResult<IEnumerable<string>> Post()
        {
            string function = Request.Form.FirstOrDefault(p => p.Key == "function").Value;
            int value = Convert.ToInt32(Request.Form.FirstOrDefault(p => p.Key == "value").Value);

            if (function == "GetBalance")
            {
                return new string[] { Startup.balance.GetBalance()+"", Startup.balance.CheckingCoins() };
            }
            else if(function == "ChangeBalance")
            {
                if (value >= 0)
                {
                    Startup.balance.AddBalance(Convert.ToByte(value));
                    return new string[] { Startup.balance.GetBalance() + "", Startup.balance.CheckingCoins() };
                }
                else
                {
                    Startup.balance.TakeBalance(value);
                    return new string[] { Startup.balance.GetBalance() + "", Startup.balance.CheckingCoins() };
                }
            }
            else if(function == "Surrender")
            {
                return new string[] { Startup.balance.GetSurrender() , Startup.balance.GetBalance()+"" };
            }else if (function == "BuyDrink")
            {
                string Status = "Denied";
                int Amount;
                using (db = new ApplicationContext())
                {
                    Beverage beverage = db.Beverage.Find(value);
                    
                    if (Startup.balance.TakeBalance(beverage.Price))
                    {
                        Status = "Ok";
                        beverage.Amount--;
                    }

                    Amount = beverage.Amount;
                    db.SaveChanges();
                }
                return new string[] { Status, Startup.balance.GetBalance() + "", Amount + "" };
            }
            return new string[] { "Error", Startup.balance.CheckingCoins()};
        }
    }
}