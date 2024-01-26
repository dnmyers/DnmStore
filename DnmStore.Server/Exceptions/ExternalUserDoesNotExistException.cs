using System.Runtime.Serialization;

[Serializable]
internal class ExternalUserDoesNotExistException : Exception
{
    public ExternalUserDoesNotExistException()
    {
    }

    public ExternalUserDoesNotExistException(string? message) : base(message)
    {
    }

    public ExternalUserDoesNotExistException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected ExternalUserDoesNotExistException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}