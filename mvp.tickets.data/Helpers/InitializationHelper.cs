using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using mvp.tickets.data.Models;
using mvp.tickets.data.Procedures;
using System.Reflection;

namespace mvp.tickets.data.Helpers
{
    public class InitializationHelper
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<InitializationHelper>>();

                try
                {
                    var context = services.GetRequiredService<ApplicationDbContext>();
                    context.Database.Migrate();

                    if (!context.Users.Any(s => s.Email == "tickets@mvp-stack.ru"))
                    {
                        context.Users.Add(new User
                        {
                            Email = "tickets@mvp-stack.ru",
                            FirstName = "Admin",
                            LastName = "Admin",
                            IsLocked = false,
                            Permissions = domain.Enums.Permissions.Admin | domain.Enums.Permissions.Employee | domain.Enums.Permissions.User,
                            DateCreated = DateTimeOffset.Now,
                            DateModified = DateTimeOffset.Now
                        });
                        context.SaveChanges();
                    }

                    ProceduresInit(context);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, ex.Message);
                    throw;
                }
            }
        }

        private static void ProceduresInit(ApplicationDbContext context)
        {
            Assembly currentAssem = Assembly.GetExecutingAssembly();
            var procedures = currentAssem.GetTypes().Where(s => s.CustomAttributes.Any(a => a.AttributeType == typeof(ProcedureAttribute))).ToList();
            foreach (var procedure in procedures)
            {
                var name = (string)procedure.GetProperty("Name").GetValue(null);
                var version = (int)procedure.GetProperty("Version").GetValue(null);
                var text = (string)procedure.GetProperty("Text").GetValue(null);
                var procedureVersion = context.ProcedureVersions.FirstOrDefault(pv => pv.Name == name);
                bool needCreate = true;
                if (procedureVersion != null)
                {
                    if (version > procedureVersion.Version)
                    {
                        string dropProcedure =
                            $@"IF EXISTS (select * from sysobjects where id = object_id(N'[{name}]')
                                    and OBJECTPROPERTY(id, N'IsProcedure') = 1)
	                            DROP PROCEDURE [{name}]";
                        context.Database.ExecuteSqlRaw(dropProcedure);
                    }
                    else
                        needCreate = false;
                }
                else
                {
                    procedureVersion = new ProcedureVersion { Name = name, Version = 0, DateCreated = DateTimeOffset.Now };
                    context.ProcedureVersions.Add(procedureVersion);
                    context.SaveChanges();
                }
                if (needCreate)
                {
                    context.Database.ExecuteSqlRaw(text);
                    procedureVersion.Version = version;
                    context.SaveChanges();
                }
            }
        }
    }
}
