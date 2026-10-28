



using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Resturant.Domain.Entity;

using System.ComponentModel;
using Microsoft.Extensions.Hosting;

namespace Restuarant.Application.BackgrouondServices
{
    public class DatabaseBackGround : BackgroundService
    {
        private readonly ILogger<DatabaseBackGround> _logger;
        private readonly IServiceProvider _serviceProvider;
    
   
    

        public DatabaseBackGround(ILogger <DatabaseBackGround> logger, IServiceProvider serviceProvider)
        {
             _logger = logger;
            _serviceProvider = serviceProvider;
       
          

        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
              while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    //the name of the class that holds the irepo, not the implementation of the class
                    // var ctx = scope.ServiceProvider.GetRequiredService<IRepo<CartItems>>();
                    //
                    //
                    //
                    // //  _guidy.Display(); 
                    //
                    //
                    //
                    // var p = await ctx.CartItemsAsync();  
                    // Console.WriteLine("hey i'm from the background service");
                    //   foreach (var items in p)
                    // {
                    //     await Console.Out.WriteLineAsync(items.Item?.ToString());
                    //     if (items.Item?.ToString() == "Tofu") {
                    //         await Console.Out.WriteLineAsync("send and email here if values are low");
                    //     }
                    // } 
                }

                _logger.LogInformation("hello my dude");
                
                //every 5 seconds 
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}
