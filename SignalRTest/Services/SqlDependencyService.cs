using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using SignalRTest.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRTest.Services
{
    public class SqlDependencyService : ISqlDependency
    {
        private readonly IConfiguration configuration;
        private readonly IHubContext<SignalServer> hubContext;
        public SqlDependencyService(IConfiguration _configuration, IHubContext<SignalServer> _hubContext)
        {
            configuration = _configuration;
            hubContext = _hubContext;
        }

        public void Config()
        {
            MonitorChanges();
        }

        private void MonitorChanges()
        {
            using (var conn = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                conn.Open();
                using (var cmd = new SqlCommand(@"SELECT Id FROM [dbo].People", conn))
                {
                    cmd.Notification = null;
                    SqlDependency dependency = new SqlDependency(cmd);
                    dependency.OnChange += actualizarPersonas;
                    SqlDependency.Start(configuration.GetConnectionString("DefaultConnection"));
                    cmd.ExecuteReader();
                }
            }
        }

        public void actualizarPersonas(object sender, SqlNotificationEventArgs e)
        {
            if (e.Info.Equals(SqlNotificationInfo.Insert))
            {
                hubContext.Clients.All.SendAsync("newPersonDetected");
            }

            MonitorChanges();
        }
    }
}
