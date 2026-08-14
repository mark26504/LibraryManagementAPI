namespace LibraryManagement.Services.Implementations.Authentication
{
    internal sealed class TokenProvider : ITokenProvider
    {
        private readonly IOptions<JwtOptions> _options;

        public TokenProvider(IOptions<JwtOptions> options)
        {
            _options = options;
        }
        public (string Token, DateTime ExpiresAt) GenerateAccessToken(AuthenticationUserDto userDto)
        {
            List<Claim> claims = new() 
            {
                new Claim(JwtRegisteredClaimNames.Sub, userDto.Id),
                new Claim(JwtRegisteredClaimNames.Email, userDto.Email),
            };

            foreach (var role  in userDto.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Value.SecretKey));
            var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var expiresAt = DateTime.UtcNow.AddMinutes(_options.Value.AccessTokenExpirationMinutes);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiresAt,
                SigningCredentials = signingCredentials,
                Issuer = _options.Value.Issuer,
                Audience = _options.Value.Audience
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return (tokenString, expiresAt);
        }

        public string GenerateRefreshToken()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(randomBytes);
        }

        public string HashRefreshToken(string refreshToken)
        {
            var bytes = Encoding.UTF8.GetBytes(refreshToken);
            var hash = SHA256.HashData(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
