using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using RealtyCRM.Api.Interfaces;
using RealtyCRM.Api.Models;

namespace RealtyCRM.Api.Services
{
    /// <summary>
    /// Сервис для регистрации и аутентификации пользователей.
    /// </summary>
    public class AuthService
    {
        private readonly IUserRepository _userRepository;

        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// Регистрирует нового пользователя в системе.
        /// </summary>
        public async Task<User> RegisterAsync(User user, string password)
        {
            if (string.IsNullOrWhiteSpace(user.Email))
                throw new ArgumentException("Email не может быть пустым.");

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Пароль не может быть пустым.");

            var existingUser = await _userRepository.GetByEmailAsync(user.Email);
            if (existingUser != null)
                throw new InvalidOperationException($"Пользователь с email '{user.Email}' уже зарегистрирован.");

            user.PasswordHash = HashPassword(password);
            await _userRepository.AddAsync(user);
            return user;
        }

        /// <summary>
        /// Аутентифицирует пользователя по email и паролю.
        /// </summary>
        public async Task<User?> AuthenticateAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return null;

            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
                return null;

            var hash = HashPassword(password);
            if (user.PasswordHash != hash)
                return null;

            return user;
        }

        /// <summary>
        /// Хеширует пароль по алгоритму SHA256.
        /// </summary>
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToHexString(hash).ToLower();
        }
    }
}
