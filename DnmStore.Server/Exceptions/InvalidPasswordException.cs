using System;
using System.Runtime.Serialization;

namespace DnmStore.Server.Exceptions;

[Serializable]
internal class InvalidPasswordException : Exception {
    public InvalidPasswordException() {
    }

    public InvalidPasswordException(string? message) : base(message) {
    }

    public InvalidPasswordException(string? message, Exception? innerException) : base(message, innerException) {
    }

#pragma warning disable SYSLIB0051 // Type or member is obsolete
    protected InvalidPasswordException(SerializationInfo info, StreamingContext context) : base(info, context) {
    }
#pragma warning restore SYSLIB0051 // Type or member is obsolete
}