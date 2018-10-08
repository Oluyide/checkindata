using Hangfire;
using Hangfire.Dashboard;
using Hangfire.SqlServer;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Checkin.Startup))]
namespace Checkin
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            GlobalConfiguration.Configuration.UseSqlServerStorage("DefaultConnection")
                                             .UseDashboardMetric(SqlServerStorage.ActiveConnections)
                                             .UseDashboardMetric(SqlServerStorage.TotalConnections)
                                             .UseDashboardMetric(DashboardMetrics.FailedCount);

            RecurringJob.AddOrUpdate(() => new Controllers.HomeController().CheckingData(""), "0 */3 * * *");
            //RecurringJob.AddOrUpdate(() => new Controllers.HomeController().CheckingData(""), "*/3 * * * *");
            app.UseHangfireDashboard();
            app.UseHangfireServer();

        }
    }
}
