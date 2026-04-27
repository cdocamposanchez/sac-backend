// Script auxiliar para generar el hash BCrypt del primer Director.
// Ejecutar con: dotnet script GenerarHashDirector.csx
// Requiere dotnet-script: dotnet tool install -g dotnet-script
//
// Si no se quiere usar dotnet-script, este código se puede pegar
// en LinqPad, en una consola .NET nueva, o en cualquier proyecto
// que ya tenga referencia a BCrypt.Net-Next.

#r "nuget: BCrypt.Net-Next, 4.0.3"

using BCrypt.Net;

// Cambiar por la contraseña deseada del primer Director
string password = "director123";

string hash = BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);

Console.WriteLine("====================================================");
Console.WriteLine("Hash BCrypt para el primer Director:");
Console.WriteLine();
Console.WriteLine(hash);
Console.WriteLine();
Console.WriteLine("Use este hash en el script SQL InsertPrimerDirector.sql");
Console.WriteLine("====================================================");
