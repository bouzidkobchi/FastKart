namespace FastKart.Controllers
{

    public partial class AuthController
    {
        public class UserDoesntExistException : Exception
        {
            public UserDoesntExistException() : base()
            {
            }

            public UserDoesntExistException(string message) : base(message)
            {
            }
        }

    }
}
