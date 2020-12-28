using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Quartz;
using static System.Console;

namespace JobLibrary
{
    public class SimplyJob : IJob
    {
        private int prevNumberOfCustomers = 90;
        private readonly string nwConn = ConfigurationManager.ConnectionStrings["nwConn"].ConnectionString;
        private readonly string customerQuery = $"Select CustomerID from dbo.Customers";
        private readonly string recordQuery = $"Select count(*) from dbo.Customers";

        public Task Execute(IJobExecutionContext context)
        {
            return Task.Run(()=> 
            {
                var noOfCustomers = GetNumberOfRecords();
                if (noOfCustomers != prevNumberOfCustomers) ;
                {
                    prevNumberOfCustomers = noOfCustomers;
                    GetCustomers();
                }
            }); 
        }

        private void RowsCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            WriteLine($"Action is: {e.Action}");
            if (e.NewItems != null) 
            {
                foreach (var item in e.NewItems)
                {
                    WriteLine($"{item}");
                }
            }
        }

        private int GetNumberOfRecords() 
        {
            var numberOfCustomerRecords = -1;
            using (var conn = new SqlConnection(nwConn))
            {
                conn.Open();
                var cmd = new SqlCommand(recordQuery, conn);
                numberOfCustomerRecords = (int)cmd.ExecuteScalar();
            }
            return numberOfCustomerRecords;
        }
        private void GetCustomers() 
        {
            var rowsCollection = new ObservableCollection<string>();
            rowsCollection.CollectionChanged += RowsCollection_CollectionChanged;

            using (var conn = new SqlConnection(nwConn))
            {
                //example of DataReader aka data house forward only read only
                conn.Open();
                SqlCommand cmd = new SqlCommand(customerQuery, conn);
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        rowsCollection.Add(dr.GetString(0));
                    }
                }
                else
                {
                    WriteLine($"No more rows,");
                }
                dr.Close();
            }
        }
    }
}
