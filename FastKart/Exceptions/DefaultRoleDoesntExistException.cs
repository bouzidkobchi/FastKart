namespace FastKart.Exceptions
{
    public class DefaultRoleDoesntExistException : Exception
    {
        public DefaultRoleDoesntExistException() : base()
        {
        }
        public DefaultRoleDoesntExistException(string message) : base(message)
        {
        }
    }
}
