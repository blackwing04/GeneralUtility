global using Xunit;
global using Moq;
global using Generic.StaticUtil;
global using DAOStandard.StaticUtil.Models;
global using DAOStandard.StaticUtil.Enums;
global using System.Data;
global using System.Data.Common;
global using DAOCustomizeException;
global using DAOStandard.Services;
global using DAOStandard.StaticUtil;
global using static Generic.StaticUtil.Models.DataModel;
internal static class Global
{
    internal const string ConnectionString="Server=myServer;Database=myDatabase;User Id=myUser;Password=myPassword;";
}
