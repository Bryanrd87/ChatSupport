namespace ChatSupport.Application.Exceptions;
public class InvalidUserException : Exception
{
    public InvalidUserException(object sender,int id) : base($"{sender} with Id {id} was not found")
    {
    }
}
