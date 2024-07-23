global using Xunit;
global using Moq;
global using DAO.Services;
global using Generic.StaticUtil;
global using DAO.StaticUtil.Models;
global using DAO.StaticUtil.Enums;
global using DAO.StaticUtil;
global using System.Data;
global using System.Data.Common;
global using DAOCustomizeException;
global using static Generic.StaticUtil.Models.DataModel;
internal static class Global {
   internal const string ConnectionString="Server=myServer;Database=myDatabase;User Id=myUser;Password=myPassword;";
}
