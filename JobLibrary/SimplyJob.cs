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
        public Task Execute(IJobExecutionContext context)
        {
            return Task.Run(()=> 
            {
                var rowsCollection = new ObservableCollection<string>();
                rowsCollection.CollectionChanged += RowsCollection_CollectionChanged;
                var nwConn = ConfigurationManager.ConnectionStrings["nwConn"].ConnectionString;
                using (var con = new SqlConnection(nwConn)) 
                {
                    //example of DataReader aka data house forward only read only
                    con.Open();
                    var query = $"Select CustomerID from dbo.Customers";
                    SqlCommand cmd = new SqlCommand(query, con);
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
    }
}
