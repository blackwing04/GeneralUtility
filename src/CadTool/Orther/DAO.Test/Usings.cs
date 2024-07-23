global using Xunit;
global using Moq;
global using StaticUtil.Generic;
global using StaticUtil.Models.DAO;
global using StaticUtil.Enums;
global using System.Data;
global using System.Data.Common;
global using DAOCustomizeException;

internal static class Global {
   internal const string ConnectionString="Server=myServer;Database=myDatabase;User Id=myUser;Password=myPassword;";
}
