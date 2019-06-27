using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BeverageVendingMachine_ASP.NET_Core.Models
{
    public static class Access
    {
        public static bool GetAccess()
        {
            return Startup.Access;
        }
        public static void ChangeAccess()
        {
            Startup.Access = true;
        }
        public static void TimerAccess()
        {
            //Thread.Sleep(1800000‬); //30 Minutes
            Thread.Sleep(1800000);
            Startup.Access = false;
        }
        public static void TimerAttempts()
        {
            //Thread.Sleep(600000‬); //10 Minutes
            Thread.Sleep(600000);
            Startup.Attempts = 1;
        }
        public static void RecoverAttempts()
        {
            Startup.Attempts = 3;
        }
    }

    public class Coins
    {
        public byte Coin_1 { get; set; }
        public byte Coin_2 { get; set; }
        public byte Coin_5 { get; set; }
        public byte Coin_10 { get; set; }

        public Coins(byte C1, byte C2, byte C5, byte C10)
        {
            Coin_1 = C1;
            Coin_2 = C2;
            Coin_5 = C5;
            Coin_10 = C10;
        }
    }

    public class Beverage
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public int Calories { get; set; }
        public string Description { get; set; }
        public string PathToImage { get; set; }
        public int Amount { get; set; }
        public DateTime Created { get; set; }
        public DateTime Removed { get; set; }
    }

    public class ApplicationContext : DbContext
    {
        public DbSet<Beverage> Beverage { get; set; }

        public ApplicationContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=BeverageDB;Trusted_Connection=True;");
        }
    }
}
