using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace RobotProject;

public class AppConfig
{
    public static IHost Host { get; set; }
    public static IConfiguration Configuration { get; set; }
}
