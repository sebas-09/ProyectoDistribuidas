using System;
using System.Linq;
using DAL; // Referencia al acceso a datos
using BCrypt.Net; // Para el hash seguro de contraseñas

public class AuthenticationService
{
    private readonly Sales_DBEntities _db;

    public AuthenticationService()
    {
        _db = new Sales_DBEntities(); // Conexión al modelo EF
    }

    public bool AuthenticateUser(string username, string password)
    {
        var user = _db.Users.FirstOrDefault(u => u.Username == username);
        if (user == null) return false;

        // Verifica el hash de la contraseña
        return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
    }

    public bool RegisterUser(string username, string password, string role)
    {
        if (_db.Users.Any(u => u.Username == username))
            throw new Exception("El usuario ya existe.");

        // Genera un hash seguro para la contraseña
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

        // Crea y guarda el usuario
        _db.Users.Add(new Users
        {
            Username = username,
            PasswordHash = hashedPassword,
            Role = role
        });
        _db.SaveChanges();

        return true;
    }
}
