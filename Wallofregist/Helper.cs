using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Web;
using Wallofregist.Models;
/// помощник в работе с базой данных
namespace Wallofregist
{
    public class Helper
    {
        BazaDan db = Helper.GetContext();
        private static BazaDan s_Strah_Med_CompanyEntities;

        public static BazaDan GetContext() {
            if (s_Strah_Med_CompanyEntities == null)
            {
                s_Strah_Med_CompanyEntities = new BazaDan();
            }
            return s_Strah_Med_CompanyEntities;
        }
        public void CreateClients(Models.Client client)
        {
            s_Strah_Med_CompanyEntities.Client.Add(client);
            s_Strah_Med_CompanyEntities.SaveChanges();
        }
        public void UbdateClients(Models.Client client)
        {
            s_Strah_Med_CompanyEntities.Entry(client).State=EntityState.Modified;
            s_Strah_Med_CompanyEntities.SaveChanges();
        }
        public void RemoveClient(int IDClienta)
        {
            var clients = s_Strah_Med_CompanyEntities.Client.Find(IDClienta);
            s_Strah_Med_CompanyEntities.Client.Remove(clients);
            s_Strah_Med_CompanyEntities.SaveChanges();
        }
        public void FiltrClients()
        {
            var clients = s_Strah_Med_CompanyEntities.Client.Where(x => x.Imya.StartsWith("E")||x.Imya.StartsWith("K"));
        }
        public void SortClients()
        {
            var clients = s_Strah_Med_CompanyEntities.Client.OrderBy(x => x.Imya);
        }
       
    }

}