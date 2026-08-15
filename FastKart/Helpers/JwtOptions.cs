namespace FastKart.Helpers
{
    public class JwtOptions
    {
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public int LifeTime { get; set; }
        public string SigningKey { get; set; }
        public int RefreshTokenLifeTime { get; set; }
    }
}